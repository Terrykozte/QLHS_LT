using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;
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
            ValidateSeafood(seafood, isUpdate: true);
            _repo.Update(seafood);
        }

        public void Insert(SeafoodDTO seafood)
        {
            ValidateSeafood(seafood, isUpdate: false);
            _repo.Insert(seafood);
        }

        private void ValidateSeafood(SeafoodDTO seafood, bool isUpdate)
        {
            if (seafood == null) throw new ArgumentNullException(nameof(seafood));

            if (string.IsNullOrWhiteSpace(seafood.SeafoodName))
                throw new ArgumentException("Tên hải sản không được để trống.");

            if (seafood.CategoryID <= 0)
                throw new ArgumentException("Vui lòng chọn danh mục.");

            if (seafood.UnitPrice < 0)
                throw new ArgumentException("Đơn giá phải >= 0.");

            if (seafood.Quantity < 0)
                throw new ArgumentException("Số lượng phải >= 0.");

            // Duplicate check by Name + Category (case-insensitive)
            var all = _repo.GetAll() ?? new List<SeafoodDTO>();
            var name = seafood.SeafoodName.Trim().ToLower();
            bool dup = all.Any(s => s.SeafoodName?.Trim().ToLower() == name && s.CategoryID == seafood.CategoryID && (!isUpdate || s.SeafoodID != seafood.SeafoodID));
            if (dup)
                throw new ArgumentException("Tên hải sản đã tồn tại trong danh mục này.");
        }

        public List<SeafoodDTO> GetAll()
        {
            var list = _repo.GetAll();
            return list.OrderBy(s => s.SeafoodName).ToList();
        }
    }
}

