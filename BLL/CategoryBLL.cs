using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;
using QLTN_LT.DAL;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.BLL
{
    public class CategoryBLL
    {
        private readonly ICategoryRepository _repo;

        public CategoryBLL() : this(new CategoryDAL())
        {
        }

        public CategoryBLL(ICategoryRepository repo)
        {
            _repo = repo;
        }

                public List<CategoryDTO> GetAll()
        {
            return _repo.GetAll().OrderBy(c => c.CategoryName).ToList();
        }

        public CategoryDTO GetById(int id)
        {
            return _repo.GetById(id);
        }

        public void Insert(CategoryDTO category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new ArgumentException("Tên danh mục không được để trống.");
            }
            _repo.Insert(category);
        }

        public void Update(CategoryDTO category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new ArgumentException("Tên danh mục không được để trống.");
            }
            _repo.Update(category);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<CategoryDTO> Search(string keyword)
        {
            return _repo.Search(keyword);
        }
    }
}

