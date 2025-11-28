using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DAL;
using QLTN_LT.DAL.Interfaces;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class SupplierBLL
    {
        private readonly ISupplierRepository _repo;

        public SupplierBLL() : this(new SupplierDAL())
        {
        }

        public SupplierBLL(ISupplierRepository repo)
        {
            _repo = repo;
        }

        public List<SupplierDTO> GetAll()
        {
            return _repo.GetAll().OrderBy(s => s.SupplierName).ToList();
        }

        public SupplierDTO GetById(int id)
        {
            return _repo.GetById(id);
        }

        public void Insert(SupplierDTO supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.SupplierName) || string.IsNullOrWhiteSpace(supplier.PhoneNumber))
            {
                throw new ArgumentException("Tên nhà cung cấp và số điện thoại không được để trống.");
            }
            _repo.Insert(supplier);
        }

        public void Update(SupplierDTO supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.SupplierName) || string.IsNullOrWhiteSpace(supplier.PhoneNumber))
            {
                throw new ArgumentException("Tên nhà cung cấp và số điện thoại không được để trống.");
            }
            _repo.Update(supplier);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<SupplierDTO> Search(string keyword)
        {
            return _repo.Search(keyword);
        }
    }
}

