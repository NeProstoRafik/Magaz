using Magaz.DAL.Data;
using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magaz.DAL.Repository
{
    public class ApplicationUserRepository : Repository<IdentityUser>, IApplicationUserRepository
    {
        private readonly Context _db;

        public ApplicationUserRepository(Context db):base(db)
        {
            _db = db;            
        }


    }
}
