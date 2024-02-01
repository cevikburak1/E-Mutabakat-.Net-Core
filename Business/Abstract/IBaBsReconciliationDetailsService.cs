using Core.Utilities.Results.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IBaBsReconciliationDetailsService
    {
        IResult Add(BaBsReconciliationDetails baBsReconciliationDetails);

        IResult AddToExcel(string filePath, int baBsReconciliationId);
        IResult Update(BaBsReconciliationDetails baBsReconciliationDetails);
        IResult Delete(BaBsReconciliationDetails baBsReconciliationDetails);
        IDataResult<BaBsReconciliationDetails> GetById(int id);
        IDataResult<List<BaBsReconciliationDetails>> GetList(int baBsReconciliationId);
    }
}
