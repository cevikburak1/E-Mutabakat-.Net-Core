using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using Entities.Dtos;
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

        IResult AddToExcel(string filePath, int companyId);
        IResult Update(AccountReconciliations accountReconciliations);
        IResult Delete(AccountReconciliations accountReconciliations);
        IDataResult<AccountReconciliations> GetById(int id);

        IDataResult<AccountReconciliations> GetByCode(string code);
        //Şirkete Göre  Cari listesi gelicek
        IDataResult<List<AccountReconciliations>> GetList(int companyId);

        IDataResult<List<AccountReconcilationDto>> GetListDto(int companyId);
        IResult SendReconcilationMail(AccountReconcilationDto accountReconciliationsDto);


    }
}
