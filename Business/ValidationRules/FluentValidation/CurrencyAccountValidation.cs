using Core.Entities.Concrete;
using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class CurrencyAccountValidation : AbstractValidator<CurrencyAccount>
    {
        public CurrencyAccountValidation()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Firma Adı Boş olamaz");
            RuleFor(p => p.Name).MinimumLength(4).WithMessage("Firma Adı en az 4 karakter olmalıdır");
            RuleFor(p => p.Address).NotEmpty().WithMessage("Adress Boş olamaz");
            RuleFor(p => p.Address).MinimumLength(4).WithMessage("Adress en az 4 karakter olmalıdır");
            //RuleFor(p => p.Email).NotEmpty().WithMessage("Email boş olmaz");
            //RuleFor(p => p.Email).EmailAddress().WithMessage("Geçerli Bir Mail Adresi yazın");

        }
    }
}
