using Business.Abstract;
using Business.BusinessAspects;
using Business.Constans;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
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
    public class MailTemplateManager : IMailTemplateService
    {
        private readonly IMailTemplateDal _mailTemplateDal;

        public MailTemplateManager(IMailTemplateDal mailTemplateDal)
        {
            _mailTemplateDal = mailTemplateDal;
        }

        [PerformanceAspect(1)]
        [SecuredOperation("MailTemplate.Add,Admin")]
        [RemoveCacheAspect("IMailTemplateService.Get")]
        public IResult Add(MailTemplate mailTemplate)
        {
           _mailTemplateDal.Add(mailTemplate);
            return new SuccessResult(Messages.MailTemplateAdded);
        }

        [PerformanceAspect(1)]
        [SecuredOperation("MailTemplate.Delete,Admin")]
        [RemoveCacheAspect("IMailTemplateService.Get")]
        public IResult Delete(MailTemplate mailTemplate)
        {
            _mailTemplateDal.Delete(mailTemplate);
            return new SuccessResult(Messages.MailTemplateDelete);
        }

        [PerformanceAspect(1)]
        [CacheAspect(60)]
        public IDataResult<MailTemplate> Get(int id)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(m => m.Id == id));

        }

        [PerformanceAspect(1)]
        [SecuredOperation("MailTemplate.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<MailTemplate>> GetAll(int companyId)
        {
            return new SuccessDataResult<List<MailTemplate>>(_mailTemplateDal.GetList(m => m.CompanyId == companyId));
        }


        [CacheAspect(60)]
        public IDataResult<MailTemplate> GetByTemplateName(string name, int companyId)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(m => m.CompanyId == companyId && m.Type==name));
        }

        [PerformanceAspect(1)]
        [SecuredOperation("MailTemplate.Update,Admin")]
        [RemoveCacheAspect("IMailTemplateService.Get")]
        public IResult Update(MailTemplate mailTemplate)
        {
            _mailTemplateDal.Update(mailTemplate);
            return new SuccessResult(Messages.MailTemplateUpdate);
        }
    }
}
