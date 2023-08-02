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
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly Context _db;

        public ApplicationTypeRepository(Context db):base(db)
        {
            _db = db;            
        }

        public void Update(ApplicationType obj)
        {
            var objFromDb=base.FirstOrDefault(c => c.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;               
            }
            _db.Update(obj);
        }
    }
}
