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
    public class EfBaBsReconciliationDal: EfEntityRepositoryBase<BaBsReconciliation, ContextDb>, IBaBsReconciliationDal
    {
        public List<BaBsReconciliationDto> GetAllDto(int companyId)
        {
            using (var context = new ContextDb())
            {
                var result = from reconciliations in context.BaBsReconciliations.Where(x => x.CompanyId == companyId)
                             join company in context.Companies on reconciliations.CompanyId equals company.Id
                             join account in context.CurrencyAccounts on reconciliations.CurrencyAccountId equals account.Id
                             select new BaBsReconciliationDto
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
                                 Total = reconciliations.Total,
                                 EmailReadDate = reconciliations.EmailReadDate,
                                 Guid = reconciliations.Guid,
                                 Id = reconciliations.Id,
                                 IsEmailRead = reconciliations.IsEmailRead,
                                 IsResultSucceed = reconciliations.IsResultSucceed,
                                 IsSendEmail = reconciliations.IsSendEmail,
                                 ResultDate = reconciliations.ResultDate,
                                 ResultNote = reconciliations.ResultNote,
                                 SendEmailDate = reconciliations.SendEmailDate,
                                 CurrencyCode = "TL",
                                 AccountEmail = account.Email,
                                 Mounth = reconciliations.Mounth,
                                 Type = reconciliations.Type,
                                 Quantity = reconciliations.Quantity,
                                 Year = reconciliations.Year
                             };
                return result.ToList();
            }
        }
    }
}
