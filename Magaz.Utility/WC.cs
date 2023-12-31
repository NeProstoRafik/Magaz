﻿using System.Collections.ObjectModel;

namespace Magaz.Utility
{
    public static class WC
    {
        // веб константы
        public const string ImagePath = @"\images\product\";
        public const string SessionCart = "ShopingCartSession";
        public const string SessionInquiryId = "InquirySession";
        

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";
        public const string AdminEmail = "hz@hz.ru";

        public const string CategoryName = "Category";
        public const string ApplicationTypeName= "ApplicationType";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
            new List<string>
            {
                StatusApproved,StatusCancelled,StatusInProcess,StatusPending,StatusRefunded,StatusShipped
            });
    }
}
