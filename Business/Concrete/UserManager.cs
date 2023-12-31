﻿using Business.Abstract;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public void Add(User user)
        {
           _userDal.Add(user);
        }

        public User GetByMail(string email)
        {
            return _userDal.Get(p => p.Email == email);
        }

        public List<OperationClaim> GetClaims(User user,int companyid)
        {
            return _userDal.GetClaims(user,companyid);
        }
    }
}
