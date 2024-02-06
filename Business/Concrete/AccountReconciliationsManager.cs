using Business.Abstract;
using Business.Attributes;
using Business.BusinessAspects;
using Business.Constans;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Entities.Concrete;
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
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using SendMailDto = Entities.Dtos.SendMailDto;

namespace Business.Concrete
{
    public class AccountReconciliationsManager : IAccountReconciliationsService
    {
        private readonly IAccountReconciliationsDal _accountReconciliationsDal;
        private readonly ICurrencyAccountService _currencyAccountService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IMailParameterService _mailParameterService;
        public AccountReconciliationsManager(IAccountReconciliationsDal accountReconciliationsDal, ICurrencyAccountService currencyAccountService, IMailService mailService, IMailTemplateService mailTemplateService, IMailParameterService mailParameterService)
        {
            _accountReconciliationsDal = accountReconciliationsDal;
            _currencyAccountService = currencyAccountService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;
            _mailParameterService = mailParameterService;
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliations.Add,Admin")]
        [RemoveCacheAspect("IAccountReconciliationsService.Get")]
        public IResult Add(AccountReconciliations accountReconciliations)
        {
            string quid = Guid.NewGuid().ToString();
            accountReconciliations.Guid = quid;
            _accountReconciliationsDal.Add(accountReconciliations);
            return new SuccessResult(Messages.AddedAccountReconciliations);
        }


        [PerformanceAspect(1)]
        //[SecuredOperation("AccountReconciliations.Add,Admin")]
        [RemoveCacheAspect("IAccountReconciliationsService.Get")]
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
                           

                                DateTime startingDate = reader.GetDateTime(1);
                                DateTime endingDate = reader.GetDateTime(2);
                                double currencyId = reader.GetDouble(3);
                                double debit = reader.GetDouble(4);
                                double credit = reader.GetDouble(5);
                                int currencyAccountid = _currencyAccountService.GetByCode(code, companyId).Data.Id;
                            string quid = Guid.NewGuid().ToString();
                            
                            AccountReconciliations accountReconciliations = new AccountReconciliations()
                                {
                                    CurrencyAccountId = currencyAccountid,
                                    CompanyId = companyId,
                                    CurrencyCredit = Convert.ToDecimal(credit),
                                    CurrencyDebit = Convert.ToDecimal(debit),
                                    CurrencyId = Convert.ToInt16(currencyId),
                                    StartingDate = startingDate,
                                    EndingDate = endingDate,
                                    Guid = quid
                                };
                                _accountReconciliationsDal.Add(accountReconciliations);
                            
                        }
                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Messages.AddedAccountReconciliations);
        }


        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliations.Delete,Admin")]
        [RemoveCacheAspect("IAccountReconciliationsService.Get")]
        public IResult Delete(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Delete(accountReconciliations);
            return new SuccessResult(Messages.DeletedAccountReconciliations);
        }

        [PerformanceAspect(1)]
        public IDataResult<AccountReconciliations> GetByCode(string code)
        {
            return new SuccessDataResult<AccountReconciliations>(_accountReconciliationsDal.Get(x => x.Guid == code));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliations.Get,Admin")]
        [CacheAspect(60)]
        public IDataResult<AccountReconciliations> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliations>(_accountReconciliationsDal.Get(x => x.Id == id));
        }


        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliations.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<AccountReconciliations>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliations>>(_accountReconciliationsDal.GetList(x => x.CompanyId == companyId));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliations.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<Entities.Dtos.AccountReconcilationDto>> GetListDto(int companyId)
        {
            return new SuccessDataResult<List<AccountReconcilationDto>>(_accountReconciliationsDal.GetAllDto(companyId));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliations.SendMail,Admin")]
        public IResult SendReconcilationMail(AccountReconcilationDto accountReconciliationsDto)
        {
            string subject = "Mutabakat Maili";
            string body = $"Bizim Şirket:{accountReconciliationsDto.CompanyName} <br />"+
                $"Şirket Vergi Dairesi:{accountReconciliationsDto.CompanyTaxDepartment} <br />"+
                $"Şirket Vergi Numarası:{accountReconciliationsDto.CompanyTaxIdNumber} - {accountReconciliationsDto.CompanyIdentityNumber} <br /> <hr>"+
                $"Karşı Şirket{accountReconciliationsDto.AccountName} <br />"+
                $"Karşı Şirket Vergi Dairesi:{accountReconciliationsDto.AccountTaxDepartment} <br />" +
                $"Karşı Şirket Vergi Numarası:{accountReconciliationsDto.AccountTaxIdNumber} - {accountReconciliationsDto.AccountIdentityNumber} <br /> <hr>"+
                $"Borç {accountReconciliationsDto.CurrencyDebit} {accountReconciliationsDto.CurrencyCode} <br />" +
                $"Alacak {accountReconciliationsDto.CurrencyCredit} {accountReconciliationsDto.CurrencyCode} <br />" 
                ;
            string link = "https://localhost:7030/api/AccountReconciliations/GetByCode?code=" + accountReconciliationsDto.Guid;
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
                Email = accountReconciliationsDto.AccountEmail,
                Subject = subject,
                Body = templatebody,

            };
            _mailService.SendMail(sendMailDto);
            return new SuccessResult(Messages.MailSendeSuccessful);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliations.Update,Admin")]
        [RemoveCacheAspect("IAccountReconciliationsService.Get")]
        public IResult Update(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Update(accountReconciliations);
            return new SuccessResult(Messages.UpdatedAccountReconciliations);
        }
    }
}
