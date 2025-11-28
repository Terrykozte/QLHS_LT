using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;
using QLTN_LT.BLL.Mocks;
using QLTN_LT.BLL.Interfaces;
using QLTN_LT.DAL.Interfaces;
using QLTN_LT.DAL;

namespace QLTN_LT.BLL
{
    public class SeafoodBLL : ISeafoodService
    {
        private readonly ISeafoodRepository _repo;

        public SeafoodBLL() : this(new SeafoodDAL())
        {
        }

        public SeafoodBLL(ISeafoodRepository repo)
        {
            _repo = repo;
        }

        public List<SeafoodDTO> Search(string keyword)
        {
            return _repo.Search(keyword);
        }

                public void Delete(int id)
        {
            _repo.Delete(id);
        }

                public SeafoodDTO GetById(int id)
        {
            return _repo.GetById(id);
        }

        public void Update(SeafoodDTO seafood)
        {
            if (string.IsNullOrWhiteSpace(seafood.SeafoodName))
            {
                throw new ArgumentException("Tên hải sản không được để trống.");
            }

            if (seafood.CategoryID <= 0)
            {
                throw new ArgumentException("Vui lòng chọn danh mục.");
            }

            _repo.Update(seafood);
        }

        public void Insert(SeafoodDTO seafood)
        {
            if (string.IsNullOrWhiteSpace(seafood.SeafoodName))
            {
                throw new ArgumentException("Tên hải sản không được để trống.");
            }

            if (seafood.CategoryID <= 0)
            {
                throw new ArgumentException("Vui lòng chọn danh mục.");
            }

            _repo.Insert(seafood);
        }

        public List<SeafoodDTO> GetAll()
        {
            try
            {
                if (DesignTimeData.UseMock)
                {
                    return DesignTimeData.Seafoods().OrderBy(s => s.SeafoodName).ToList();
                }

                var list = _repo.GetAll();
                return list.OrderBy(s => s.SeafoodName).ToList();
            }
            catch
            {
                // Fallback: nếu DB chưa sẵn sàng, dùng dữ liệu mock
                return DesignTimeData.Seafoods().OrderBy(s => s.SeafoodName).ToList();
            }
        }
    }
}

