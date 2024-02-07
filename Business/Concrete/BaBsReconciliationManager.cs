using Business.Abstract;
using Business.BusinessAspects;
using Business.Constans;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendMailDto = Entities.Dtos.SendMailDto;

namespace Business.Concrete
{
    public class BaBsReconciliationManager : IBaBsReconciliationService
    {
        private readonly IBaBsReconciliationDal _baBsReconciliationDal;
        private readonly ICurrencyAccountService _currencyAccountService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IMailParameterService _mailParameterService;
        public BaBsReconciliationManager(IBaBsReconciliationDal baBsReconciliationDal, ICurrencyAccountService currencyAccountService, IMailService mailService = null, IMailTemplateService mailTemplateService = null, IMailParameterService mailParameterService = null)
        {
            _baBsReconciliationDal = baBsReconciliationDal;
            _currencyAccountService = currencyAccountService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;
            _mailParameterService = mailParameterService;
        }


        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.Add,Admin")]
        [RemoveCacheAspect("IBaBsReconciliationService.Get")]
        public IResult Add(BaBsReconciliation baBsReconciliation)
        {
            string quid = Guid.NewGuid().ToString();
            baBsReconciliation.Guid = quid;
            _baBsReconciliationDal.Add(baBsReconciliation);
            return new SuccessResult(Messages.AddedBaBsReconciliation);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.Add,Admin")]
        [RemoveCacheAspect("IBaBsReconciliationService.Get")]
        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int companyId)
        {
            //program hata vermesin diye
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        //EXCELİN EN BAIASNDAKİ BAŞLIKLARI ALMAMASI İÇİN 
                        string code = reader.GetString(0);



                        if (code != "Cari Kodu" && code != null)
                        {
                            string type = reader.GetString(1);
                            double mounth = reader.GetDouble(2);
                            double year = reader.GetDouble(3);
                            double quantity = reader.GetDouble(4);
                            double total = reader.GetDouble(5);
                            string quid = Guid.NewGuid().ToString();
                            int currencyAccountid = _currencyAccountService.GetByCode(code, companyId).Data.Id;
                            BaBsReconciliation baBsReconciliation = new BaBsReconciliation()
                            {
                                CurrencyAccountId = currencyAccountid,
                                CompanyId = companyId,
                                Type = type,
                                Mounth = Convert.ToInt16(mounth),
                                Year= Convert.ToInt16(year),
                                Quantity = Convert.ToInt16(quantity),
                                Total = Convert.ToInt16(total),
                                Guid = quid,
                            };
                            _baBsReconciliationDal.Add(baBsReconciliation);

                        }
                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Messages.AddedBaBsReconciliation);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.Delete,Admin")]
        [RemoveCacheAspect("IBaBsReconciliationService.Get")]
        public IResult Delete(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliationDal.Delete(baBsReconciliation);
            return new SuccessResult(Messages.DeletedBaBsReconciliation);
        }


        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.Get,Admin")]
        [CacheAspect(60)]
        public IDataResult<BaBsReconciliation> GetByCode(string code)
        {
            return new SuccessDataResult<BaBsReconciliation>(_baBsReconciliationDal.Get(x => x.Guid == code));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.Get,Admin")]
        [CacheAspect(60)]
        public IDataResult<BaBsReconciliation> GetById(int id)
        {
            return new SuccessDataResult<BaBsReconciliation>(_baBsReconciliationDal.Get(x => x.Id == id));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<BaBsReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<BaBsReconciliation>>(_baBsReconciliationDal.GetList(x => x.CompanyId == companyId));
        }

        public IDataResult<List<BaBsReconciliationDto>> GetListDto(int companyId)
        {
            return new SuccessDataResult<List<BaBsReconciliationDto>>(_baBsReconciliationDal.GetAllDto(companyId));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.SendMail,Admin")]
        public IResult SendReconcilationMail(BaBsReconciliationDto baBsReconciliationDto)
        {
            string subject = "Mutabakat Maili";
            string body = $"Bizim Şirket:{baBsReconciliationDto.CompanyName} <br />" +
                $"Şirket Vergi Dairesi:{baBsReconciliationDto.CompanyTaxDepartment} <br />" +
                $"Şirket Vergi Numarası:{baBsReconciliationDto.CompanyTaxIdNumber} - {baBsReconciliationDto.CompanyIdentityNumber} <br /> <hr>" +
                $"Karşı Şirket{baBsReconciliationDto.AccountName} <br />" +
                $"Karşı Şirket Vergi Dairesi:{baBsReconciliationDto.AccountTaxDepartment} <br />" +
                $"Karşı Şirket Vergi Numarası:{baBsReconciliationDto.AccountTaxIdNumber} - {baBsReconciliationDto.AccountIdentityNumber} <br /> <hr>" +
                $"Adet {baBsReconciliationDto.Quantity} <br />" +
                $"Ay {baBsReconciliationDto.Mounth} <br />" +
                $"Yıl {baBsReconciliationDto.Year} <br />" +
                $"Alacak {baBsReconciliationDto.Total} {baBsReconciliationDto.CurrencyCode} <br />"
                ;
            string link = "https://localhost:7030/api/BaBsReconciliation/GetByCode?code=" + baBsReconciliationDto.Guid;
            string linkDescripiton = "Mutabakatı Cevaplamak İçin Tıklayın";

            var mailtemplate = _mailTemplateService.GetByTemplateName("Kayıt", 3);
            string templatebody = mailtemplate.Data.Value;
            templatebody = templatebody.Replace("{{title}}", subject);
            templatebody = templatebody.Replace("{{message}}", body);
            templatebody = templatebody.Replace("{{link}}", link);
            templatebody = templatebody.Replace("{{linkDescription}}", linkDescripiton);
            var mailparameter = _mailParameterService.Get(4);
            SendMailDto sendMailDto = new SendMailDto()
            {
                mailParameter = mailparameter.Data,
                Email = baBsReconciliationDto.AccountEmail,
                Subject = subject,
                Body = templatebody,

            };
            _mailService.SendMail(sendMailDto);
            return new SuccessResult(Messages.MailSendeSuccessful);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("BaBsReconciliation.Update,Admin")]
        [RemoveCacheAspect("IBaBsReconciliationService.Get")]
        public IResult Update(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliationDal.Update(baBsReconciliation);
            return new SuccessResult(Messages.UpdatedBaBsReconciliation);
        }


    }
}
