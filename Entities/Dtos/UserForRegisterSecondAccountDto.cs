using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class UserForRegisterSecondAccountDto :UserForRegister
    {
        public int CompanyId { get; set; }
    }
}
