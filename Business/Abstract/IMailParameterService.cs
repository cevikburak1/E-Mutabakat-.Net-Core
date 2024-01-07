using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IMailParameterService
    {
        //Mail paramatresi yolladıgımda  daha önce bir mail paramtresi kaydedilmiş mi onu sorgilicak eger yoksa kayıt oluşturcak eger varsa update edicek
        IResult Update(MailParameter mailParameter);

        //sonuç almam lazım çünkü data bazında o yüzden ıdataresult kullanıyorum
       IDataResult<MailParameter> Get(int companyId);
    }
}
