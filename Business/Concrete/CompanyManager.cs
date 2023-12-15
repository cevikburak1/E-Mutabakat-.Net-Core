﻿using Business.Abstract;
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

        public IResult Add(Company company)
        {
            _companyDal.Add(company);
            return new SuccessResult(Messages.AddCompany);
        }
        public IDataResult<List<Company>> GetList()
        {
            return new SuccessDataResult<List<Company>>(_companyDal.GetList(),"İşlem Başarılı");
        }
    }
}
