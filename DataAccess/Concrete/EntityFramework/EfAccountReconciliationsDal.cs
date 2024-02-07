using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfAccountReconciliationsDal : EfEntityRepositoryBase<AccountReconciliations, ContextDb>, IAccountReconciliationsDal
    {
        public List<AccountReconcilationDto> GetAllDto(int companyId)
        {
            using (var context = new ContextDb())
            {
                var result = from reconciliations in context.AccountReconciliations.Where(x=>x.CompanyId==companyId)
                             join company in context.Companies on reconciliations.CompanyId equals company.Id
                             join account in context.CurrencyAccounts on reconciliations.CurrencyAccountId equals account.Id
                             join currency in context.Currencies on reconciliations.CurrencyId equals currency.Id
                             select new AccountReconcilationDto
                             {
                                 CompanyId = companyId,
                                 CurrencyAccountId = account.Id,
                                 AccountIdentityNumber = account.IdentityNumber,
                                 AccountName = account.Name,
                                 AccountTaxDepartment = account.TaxDepartment,
                                 AccountTaxIdNumber = account.TaxIdNumber,
                                 CompanyIdentityNumber = company.IdentityNumber,
                                 CompanyName = company.Name,
                                 CompanyTaxDepartment = company.TaxDepartment,
                                 CompanyTaxIdNumber = company.TaxIdNumber,
                                 CurrencyCredit = reconciliations.CurrencyCredit,
                                 CurrencyDebit = reconciliations.CurrencyDebit,
                                 CurrencyId = reconciliations.CurrencyId,
                                 EmailReadDate = reconciliations.EmailReadDate,
                                 EndingDate = reconciliations.EndingDate,
                                 Guid = reconciliations.Guid,
                                 Id = reconciliations.Id,
                                 IsEmailRead=reconciliations.IsEmailRead,
                                 IsResultSucceed=reconciliations.IsResultSucceed,
                                 IsSendEmail=reconciliations.IsSendEmail,
                                 ResultDate = reconciliations.ResultDate,
                                 ResultNote = reconciliations.ResultNote,
                                 SendEmailDate = reconciliations.SendEmailDate,
                                 StartingDate = reconciliations.StartingDate,
                                 CurrencyCode = currency.Code,
                                 AccountEmail = account.Email,
                             };
                return result.ToList();
            }
        }
    }
}
