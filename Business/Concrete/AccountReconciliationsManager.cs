using Business.Abstract;
using Business.Constans;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
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

        public AccountReconciliationsManager(IAccountReconciliationsDal accountReconciliationsDal)
        {
            _accountReconciliationsDal = accountReconciliationsDal;
        }

        public IResult Add(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Add(accountReconciliations);
            return new SuccessResult(Messages.AddedAccountReconciliations);
        }

        public IResult Delete(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Delete(accountReconciliations);
            return new SuccessResult(Messages.DeletedAccountReconciliations);
        }

        public IDataResult<AccountReconciliations> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliations>(_accountReconciliationsDal.Get(x => x.Id == id));
        }

        public IDataResult<List<AccountReconciliations>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliations>>(_accountReconciliationsDal.GetList(x => x.CompanyId == companyId));
        }

        public IResult Update(AccountReconciliations accountReconciliations)
        {
            _accountReconciliationsDal.Update(accountReconciliations);
            return new SuccessResult(Messages.UpdatedAccountReconciliations);
        }
    }
}
