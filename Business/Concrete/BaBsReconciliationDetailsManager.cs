using Business.Abstract;
using Business.Constans;
using Core.Aspects.Autofac.Transaction;
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
    public class BaBsReconciliationDetailsManager: IBaBsReconciliationDetailsService
    {
        private readonly IBaBsReconciliationDetailsDal _baBsReconciliationDetailsDal;

        public BaBsReconciliationDetailsManager(IBaBsReconciliationDetailsDal baBsReconciliationDetailsDal)
        {
            _baBsReconciliationDetailsDal = baBsReconciliationDetailsDal;
        }

        public IResult Add(BaBsReconciliationDetails baBsReconciliationDetails)
        {
            _baBsReconciliationDetailsDal.Add(baBsReconciliationDetails);
            return new SuccessResult(Messages.AddedBaBsReconciliationDetail);
        }

        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int baBsReconciliationId)
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
                            double amount = reader.GetDouble(2);
                          

                            BaBsReconciliationDetails baBsReconciliationDetails = new BaBsReconciliationDetails()
                            {
                                Date = date,
                                Amount = Convert.ToInt16(amount),
                                BaBsReconciliationId = baBsReconciliationId,
                                Description = description
                            };
                            _baBsReconciliationDetailsDal.Add(baBsReconciliationDetails);
                        }
                    }
                }
            }
            return new SuccessResult(Messages.AddedBaBsReconciliationDetail);
        }

        public IResult Delete(BaBsReconciliationDetails baBsReconciliationDetails)
        {
            _baBsReconciliationDetailsDal.Delete(baBsReconciliationDetails);
            return new SuccessResult(Messages.DeletedBaBsReconciliationDetail);
        }

        public IDataResult<BaBsReconciliationDetails> GetById(int id)
        {
            return new SuccessDataResult<BaBsReconciliationDetails>(_baBsReconciliationDetailsDal.Get(x => x.Id == id));
        }

        public IDataResult<List<BaBsReconciliationDetails>> GetList(int baBsReconciliationId)
        {
            return new SuccessDataResult<List<BaBsReconciliationDetails>>(_baBsReconciliationDetailsDal.GetList(x => x.BaBsReconciliationId == baBsReconciliationId));
        }

        public IResult Update(BaBsReconciliationDetails baBsReconciliationDetails)
        {
            _baBsReconciliationDetailsDal.Update(baBsReconciliationDetails);
            return new SuccessResult(Messages.UpdatedBaBsReconciliationDetail);
        }
    }
}
