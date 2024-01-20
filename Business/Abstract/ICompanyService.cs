using Core.Entities.Concrete;
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
    public interface ICompanyService
    {
        //IResult işlem yapar sonuç dönderir
        IResult Add(Company company);
        IResult Update(Company company);
        IDataResult<Company> GetById(int id);
        IResult AddCompanyAddUserCompany(CompanyDto company);
        IResult CompanyExist(Company company);
        IResult UserCompanyAdd(int userId,int companyId);
        IDataResult<UserCompany> GetCompany(int userid);
        IDataResult<List<Company>> GetList();

    }
}
