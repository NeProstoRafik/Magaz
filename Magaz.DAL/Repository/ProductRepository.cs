using Magaz.DAL.Data;
using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Magaz.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magaz.DAL.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly Context _db;

        public ProductRepository(Context db):base(db)
        {
            _db = db;            
        }

        public IEnumerable<SelectListItem> GetAllDropDownList(string obj)
        {
            if (obj==WC.CategoryName)
            {
                return _db.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                });
            }
            if (obj==WC.ApplicationTypeName)
            {
                return _db.Applications.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                });
            }
            return null;
        }

        public void Update(Product product)
        {
            
            _db.Products.Update(product);
        }
    }
}
