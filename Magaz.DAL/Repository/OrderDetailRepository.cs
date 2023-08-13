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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly Context _db;
        public OrderDetailRepository(Context db): base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail obj)
        {
            _db.OrderDetails.Update(obj);
        }
    }
}
