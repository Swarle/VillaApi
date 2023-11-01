using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class OrderStatusSD
    {
        public static string InProgress => "In Progress";
        public static string Cancelled => "Cancelled";
        public static string Completed => "Completed";
        public static string Returned => "Returned";
        public static string Updated => "Updated";
    }
}
