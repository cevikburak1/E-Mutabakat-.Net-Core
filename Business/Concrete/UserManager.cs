using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [ValidationAspect(typeof(UserValidator))]
        [RemoveCacheAspect("IUserService.Get")]
        public void Add(User user)
        {
           _userDal.Add(user);
        }


        [CacheAspect(60)]
        public User GetById(int id)
        {
            return _userDal.Get(u => u.Id == id);
        }

        [CacheAspect(60)]
        public User GetByMail(string email)
        {
            return _userDal.Get(p => p.Email == email);
        }


        [CacheAspect(60)]
        public User GetByMailConfirmValue(string value)
        {
            return _userDal.Get(x => x.MailConfirmValue == value);
        }


        public List<OperationClaim> GetClaims(User user,int companyid)
        {
            return _userDal.GetClaims(user,companyid);
        }
        [RemoveCacheAspect("IUserService.Get")]
        public void Update(User user)
        {
           _userDal.Update(user);
        }
    }
}
