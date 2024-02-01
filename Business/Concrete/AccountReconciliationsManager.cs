using Business.Abstract;
using Business.Constans;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AccountReconciliationsManager : IAccountReconciliationsService
    {
        private readonly IAccountReconciliationsDal _accountReconciliationsDal;
        private readonly ICurrencyAccountService _currencyAccountService;

        public AccountReconciliationsManager(IAccountReconciliationsDal accountReconciliationsDal, ICurrencyAccountService currencyAccountService)
        {
            _accountReconciliationsDal = accountReconciliationsDal;
            _currencyAccountService = currencyAccountService;
        }

        [RemoveCacheAspect("IAccountReconciliationsService.Get")]
        public IResult Add(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Add(accountReconciliations);
            return new SuccessResult(Messages.AddedAccountReconciliations);
        }

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
                                AccountReconciliations accountReconciliations = new AccountReconciliations()
                                {
                                    CurrencyAccountId = currencyAccountid,
                                    CompanyId = companyId,
                                    CurrencyCredit = Convert.ToDecimal(credit),
                                    CurrencyDebit = Convert.ToDecimal(debit),
                                    CurrencyId = Convert.ToInt16(currencyId),
                                    StartingDate = startingDate,
                                    EndingDate = endingDate
                                };
                                _accountReconciliationsDal.Add(accountReconciliations);
                            
                        }
                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Messages.AddedAccountReconciliations);
        }

        [RemoveCacheAspect("IAccountReconciliationsService.Get")]
        public IResult Delete(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Delete(accountReconciliations);
            return new SuccessResult(Messages.DeletedAccountReconciliations);
        }

        public IDataResult<AccountReconciliations> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliations>(_accountReconciliationsDal.Get(x => x.Id == id));
        }

        [CacheAspect(60)]
        public IDataResult<List<AccountReconciliations>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliations>>(_accountReconciliationsDal.GetList(x => x.CompanyId == companyId));
        }

        [RemoveCacheAspect("IAccountReconciliationsService.Get")]
        public IResult Update(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Update(accountReconciliations);
            return new SuccessResult(Messages.UpdatedAccountReconciliations);
        }
    }
}
