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
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly Context _db;

        public InquiryHeaderRepository(Context db):base(db)
        {
            _db = db;            
        }

        
        public void Update(InquiryHeader product)
        {
            
            _db.InquiryHeader.Update(product);
        }
    }
}
