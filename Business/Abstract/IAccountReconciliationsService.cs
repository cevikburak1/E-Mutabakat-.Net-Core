using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAccountReconciliationsService
    {
        IResult Add(AccountReconciliations accountReconciliations);

        //IResult AddToExcel(string fileName, int companyId);
        IResult Update(AccountReconciliations accountReconciliations);
        IResult Delete(AccountReconciliations accountReconciliations);
        IDataResult<AccountReconciliations> GetById(int id);

        //Şirkete Göre  Cari listesi gelicek
        IDataResult<List<AccountReconciliations>> GetList(int companyId);
    }
}
