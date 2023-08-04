using Magaz.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magaz.DAL.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<IdentityUser>
    {
        // void Update(IdentityUser category);
    }
}
