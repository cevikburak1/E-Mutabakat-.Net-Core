 using Business.Abstract;
using Business.BusinessAspects;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CompanyManager : ICompanyService
    {
        //Kullanıcı Yetkili mi ??
        //Transcaption gerektiren işlem varmı ???
        //LOG
        //VALİDATİON
        //DEPENDECY INJECTİON İLE ENTİTYFREAMWORKA BAGLILIKTAN KURULDUM KURUMSAL MİMARİ :)
        private readonly ICompanyDal _companyDal;
        public CompanyManager(ICompanyDal companyDal)
        {
            _companyDal = companyDal;
        }
        [ValidationAspect(typeof(CompanyValidation))]
        [RemoveCacheAspect("ICompanyService.Get")]
        public IResult Add(Company company)
        {
            _companyDal.Add(company);
            return new SuccessResult(Messages.AddCompany);
        }

        [ValidationAspect(typeof(CompanyValidation))]
        [TransactionScopeAspect]
        [RemoveCacheAspect("ICompanyService.Get")]
        public IResult AddCompanyAddUserCompany(CompanyDto company)
        {
            _companyDal.Add(company.Company);
            _companyDal.UserCompanyAdd(company.UserId,company.Company.Id);
            return new SuccessResult(Messages.AddCompany);
        }

        public IResult CompanyExist(Company company)
        {
            var result = _companyDal.Get(c => c.Name == company.Name && c.TaxDepartment == company.TaxDepartment && c.TaxIdNumber == company.TaxIdNumber &&c.IdentityNumber==company.IdentityNumber);
            if (result != null)
            {
                return new ErrorResult(Messages.CompanyAlreadyExist);
            }
            return new SuccessResult();
        }

        [CacheAspect(60)]
        public IDataResult<Company> GetById(int id)
        {
            return new SuccessDataResult<Company>(_companyDal.Get(x => x.Id == id));
        }

        [CacheAspect(60)]
        public IDataResult<UserCompany> GetCompany(int userid)
        {
           return new SuccessDataResult<UserCompany>(_companyDal.GetCompany(userid));
        }

        [CacheAspect(60)]
        public IDataResult<List<Company>> GetList()
        {
            return new SuccessDataResult<List<Company>>(_companyDal.GetList(),"İşlem Başarılı");
        }

        [PerformanceAspect(1)]
        [SecuredOperation("Company.Update,Admin")]
        [ValidationAspect(typeof(CompanyValidation))]
        [RemoveCacheAspect("ICompanyService.Get")]
        public IResult Update(Company company)
        {
            _companyDal.Update(company);
            return new SuccessResult(Messages.UpdatedCompany);
        }

        [RemoveCacheAspect("ICompanyService.Get")]
        public IResult UserCompanyAdd(int userId, int companyId)
        {
            //var result kullanamam çünkü sonuç döndermedim ben
            _companyDal.UserCompanyAdd(userId, companyId);
            return new SuccessResult();
        }
    }
}
