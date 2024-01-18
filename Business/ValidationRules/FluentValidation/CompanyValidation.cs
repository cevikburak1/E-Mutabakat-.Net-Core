using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class CompanyValidation :AbstractValidator<Company>
    {
        public CompanyValidation()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Şirket Adı Boş Olamaz");
            RuleFor(p => p.Name).MinimumLength(5).WithMessage("Şirket Adı En az 5 karakter olmalı");
            RuleFor(p => p.Address).NotEmpty().WithMessage("Şirket Adresi Adı Boş Olamaz");
        }
    }
}
