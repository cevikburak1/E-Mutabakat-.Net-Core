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
    public class BaBsReconciliationManager : IBaBsReconciliationService
    {
        private readonly IBaBsReconciliationDal _baBsReconciliationDal;
        private readonly ICurrencyAccountService _currencyAccountService;
        public BaBsReconciliationManager(IBaBsReconciliationDal baBsReconciliationDal, ICurrencyAccountService currencyAccountService)
        {
            _baBsReconciliationDal = baBsReconciliationDal;
            _currencyAccountService = currencyAccountService;
        }


        [RemoveCacheAspect("IBaBsReconciliationService.Get")]
        public IResult Add(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliationDal.Add(baBsReconciliation);
            return new SuccessResult(Messages.AddedBaBsReconciliation);
        }

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
                            };
                            _baBsReconciliationDal.Add(baBsReconciliation);

                        }
                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Messages.AddedBaBsReconciliation);
        }

        [RemoveCacheAspect("IBaBsReconciliationService.Get")]
        public IResult Delete(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliationDal.Delete(baBsReconciliation);
            return new SuccessResult(Messages.DeletedBaBsReconciliation);
        }

        [CacheAspect(60)]
        public IDataResult<BaBsReconciliation> GetById(int id)
        {
            return new SuccessDataResult<BaBsReconciliation>(_baBsReconciliationDal.Get(x => x.Id == id));
        }

        [CacheAspect(60)]
        public IDataResult<List<BaBsReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<BaBsReconciliation>>(_baBsReconciliationDal.GetList(x => x.CompanyId == companyId));
        }

        [RemoveCacheAspect("IBaBsReconciliationService.Get")]
        public IResult Update(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliationDal.Update(baBsReconciliation);
            return new SuccessResult(Messages.UpdatedBaBsReconciliation);
        }

       
    }
}
