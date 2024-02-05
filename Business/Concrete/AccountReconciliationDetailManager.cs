using Business.Abstract;
using Business.BusinessAspects;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AccountReconciliationDetailManager: IAccountReconciliationDetailService
    {
     private readonly  IAccountReconciliationDetailDal _accountReconciliationDetailDal;

        public AccountReconciliationDetailManager(IAccountReconciliationDetailDal accountReconciliationDetailDal)
        {
            _accountReconciliationDetailDal = accountReconciliationDetailDal;
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliationDetail.Add,Admin")]
        [RemoveCacheAspect("IAccountReconciliationDetailService.Get")]
        public IResult Add(AccountReconciliationDetail accountReconciliationDetail)
        {
            _accountReconciliationDetailDal.Add(accountReconciliationDetail);
            return new SuccessResult(Messages.AddedAccountReconciliationsDetail);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliationDetail.Add,Admin")]
        [RemoveCacheAspect("IAccountReconciliationDetailService.Get")]
        public IResult AddToExcel(string filePath, int accountReconciliationId)
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
                        string description = reader.GetString(1);



                        if (description != "Açıklama" && description != null)
                        {


                            DateTime date = reader.GetDateTime(0);
                            double currencyId = reader.GetDouble(2);
                            double debit = reader.GetDouble(3);
                            double credit = reader.GetDouble(4);

                            AccountReconciliationDetail accountReconciliationDetail = new AccountReconciliationDetail()
                            {
                                Date = date,
                                CurrencyId = Convert.ToInt16(currencyId),
                                CurrencyDebit= Convert.ToDecimal(debit),
                                CurrencyCredit= Convert.ToDecimal(credit),
                                AccountReconciliationId = accountReconciliationId,
                                Description = description
                            };
                            _accountReconciliationDetailDal.Add(accountReconciliationDetail);
                        }
                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Messages.AddedAccountReconciliationsDetail);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliationDetail.Delete,Admin")]
        [RemoveCacheAspect("IAccountReconciliationDetailService.Get")]
        public IResult Delete(AccountReconciliationDetail accountReconciliationDetail)
        {
            _accountReconciliationDetailDal.Delete(accountReconciliationDetail);
            return new SuccessResult(Messages.DeletedAccountReconciliationsDetail);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliationDetail.Get,Admin")]
        [CacheAspect(60)]
        public IDataResult<AccountReconciliationDetail> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliationDetail>(_accountReconciliationDetailDal.Get(x=>x.Id==id));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliationDetail.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<AccountReconciliationDetail>> GetList(int accountReconciliationId)
        {
            return new SuccessDataResult<List<AccountReconciliationDetail>>(_accountReconciliationDetailDal.GetList(x => x.AccountReconciliationId == accountReconciliationId));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("AccountReconciliationDetail.Update,Admin")]
        [RemoveCacheAspect("IAccountReconciliationDetailService.Get")]
        public IResult Update(AccountReconciliationDetail accountReconciliationDetail)
        {
            _accountReconciliationDetailDal.Update(accountReconciliationDetail);
            return new SuccessResult(Messages.UpdatedAccountReconciliationsDetail);
        }
    }
}
