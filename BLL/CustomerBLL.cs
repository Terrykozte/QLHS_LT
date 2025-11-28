using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DAL;
using QLTN_LT.DAL.Interfaces;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class CustomerBLL
    {
        private readonly ICustomerRepository _repo;

        public CustomerBLL() : this(new CustomerDAL())
        {
        }

        public CustomerBLL(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public List<CustomerDTO> GetAll()
        {
            return _repo.GetAll().OrderBy(c => c.CustomerName).ToList();
        }

        public CustomerDTO GetById(int id)
        {
            return _repo.GetById(id);
        }

        public void Insert(CustomerDTO customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerName) || string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                throw new ArgumentException("Tên khách hàng và số điện thoại không được để trống.");
            }
            _repo.Insert(customer);
        }

        public void Update(CustomerDTO customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerName) || string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                throw new ArgumentException("Tên khách hàng và số điện thoại không được để trống.");
            }
            _repo.Update(customer);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<CustomerDTO> Search(string keyword)
        {
            return _repo.Search(keyword);
        }
    }
}

