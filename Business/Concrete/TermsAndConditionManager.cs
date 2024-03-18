using Business.Abstract;
using Business.BusinessAspects;
using Business.Constans;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class TermsAndConditionManager : ITermsAndConditionService
    {
        private readonly ITermsAndConditionDal _termsAndConditionDal;

        public TermsAndConditionManager(ITermsAndConditionDal termsAndConditionDal)
        {
            _termsAndConditionDal = termsAndConditionDal;
        }

        //[SecuredOperation("Admin")]
        //kapalı oalcak çünkü  ilk kez kayıt olanın sözleşmeyi görmesi gerekli
        public IDataResult<TermsAndCondition> Get()
        {
           return new SuccessDataResult<TermsAndCondition>(_termsAndConditionDal.GetList().FirstOrDefault());
        }

        [SecuredOperation("Admin")]
        public IResult Update(TermsAndCondition termsAndCondition)
        {
            var result = _termsAndConditionDal.GetList().FirstOrDefault();
            if (result != null) 
            {
                result.Description = termsAndCondition.Description;
                _termsAndConditionDal.Update(result);
            }
            else
            {
                _termsAndConditionDal.Add(termsAndCondition);
            }
            return new SuccessResult(Messages.UpdatedTermsAndConditionm);
        }
    }
}
