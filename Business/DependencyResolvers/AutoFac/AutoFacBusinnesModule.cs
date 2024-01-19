using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Castle.DynamicProxy;
using Core.Utilities.Interception;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DependencyResolvers.AutoFac
{
    public class AutoFacBusinnesModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CompanyManager>().As<ICompanyService>();
            builder.RegisterType<EfCompanyDal>().As<ICompanyDal>();

            builder.RegisterType<AccountReconciliationDetailManager>().As<IAccountReconciliationDetailService>();
            builder.RegisterType<EfAccountReconciliationDetailDal>().As<IAccountReconciliationDetailDal>();


            builder.RegisterType<AccountReconciliationsManager>().As<IAccountReconciliationsService>();
            builder.RegisterType<EfAccountReconciliationsDal>().As<IAccountReconciliationsDal>();


            builder.RegisterType<BaBsReconciliationDetailsManager>().As<IBaBsReconciliationDetailsService>();
            builder.RegisterType<EfBaBsReconciliationDetailsDal>().As<IBaBsReconciliationDetailsDal>();


            builder.RegisterType<BaBsReconciliationManager>().As<IBaBsReconciliationService>();
            builder.RegisterType<EfBaBsReconciliationDal>().As<IBaBsReconciliationDal>();


            builder.RegisterType<CurrencyAccountManager>().As<ICurrencyAccountService>();
            builder.RegisterType<EfCurrencyAccountDal>().As<ICurrencyAccountDal>();

            builder.RegisterType<CurrencyManager>().As<ICurrencyService>();
            builder.RegisterType<EfCurrencyDal>().As<ICurrencyDal>();

            builder.RegisterType<MailParameterManager>().As<IMailParameterService>();
            builder.RegisterType<EfMailParameterDal>().As<IMailParameterDal>();


            builder.RegisterType<MailTemplateManager>().As<IMailTemplateService>();
            builder.RegisterType<EfMailTemplateDal>().As<IMailTemplateDal>();

            builder.RegisterType<MailManager>().As<IMailService>();
            builder.RegisterType<EfMailDal>().As<IMailDal>();

            builder.RegisterType<UserManager>().As<IUserService>();
            builder.RegisterType<EfUserDal>().As<IUserDal>();

            builder.RegisterType<AuthManager>().As<IAuthService>();

            builder.RegisterType<JwtHelper>().As<ITokenHelper>();


            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces().EnableInterfaceInterceptors(new ProxyGenerationOptions()
            {
                Selector = new AspectInterceptorSelector()
            }).SingleInstance();
        }
    }
}
