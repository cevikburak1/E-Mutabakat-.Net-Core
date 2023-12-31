﻿using Core.Utilities.Results.Abstract;
using Entities.Concrete;
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
        IResult CompanyExist(Company company);
        IResult UserCompanyAdd(int userId,int companyId);
        IDataResult<List<Company>> GetList();


    }
}
