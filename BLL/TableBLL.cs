using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DAL;
using QLTN_LT.DAL.Interfaces;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    public class TableBLL
    {
        private readonly ITableRepository _repo;

        public TableBLL() : this(new TableDAL())
        {
        }

        public TableBLL(ITableRepository repo)
        {
            _repo = repo;
        }

                public List<TableDTO> GetAll()
        {
            return _repo.GetAll().OrderBy(t => t.TableName).ToList();
        }

        public TableDTO GetById(int id)
        {
            return _repo.GetById(id);
        }

        public void Insert(TableDTO table)
        {
            if (string.IsNullOrWhiteSpace(table.TableName))
            {
                throw new ArgumentException("Tên bàn không được để trống.");
            }
            _repo.Insert(table);
        }

        public void Update(TableDTO table)
        {
            if (string.IsNullOrWhiteSpace(table.TableName))
            {
                throw new ArgumentException("Tên bàn không được để trống.");
            }
            _repo.Update(table);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<TableDTO> Search(string keyword)
        {
            return _repo.Search(keyword);
        }
    }
}
