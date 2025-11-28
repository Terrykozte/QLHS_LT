using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL.Interfaces
{
    public interface ISeafoodService
    {
        List<SeafoodDTO> GetAll();
    }
}

