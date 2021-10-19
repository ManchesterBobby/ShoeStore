using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ShoeStore.Models
{
    public class OrderViewModel
    {

        public string OrderImportFileError { get; set; }

        public List<Order> Orders { get; set; }


    }
}
