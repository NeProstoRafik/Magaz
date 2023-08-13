using Microsoft.AspNetCore.Mvc.Rendering;
using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Magaz.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magaz.DAL.Data;

namespace Magaz.DAL.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly Context _db;
        public OrderHeaderRepository(Context db): base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }
    }
}
