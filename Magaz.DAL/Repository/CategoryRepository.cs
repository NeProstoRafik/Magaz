using Magaz.DAL.Data;
using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magaz.DAL.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly Context _db;

        public CategoryRepository(Context db):base(db)
        {
            _db = db;            
        }

        public void Update(Category category)
        {
            var objFromDb=base.FirstOrDefault(c => c.Id == category.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = category.Name;
                objFromDb.DisplayOrder= category.DisplayOrder;  
            }
            _db.Update(category);
        }
    }
}
