using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämningsuppgift_CRUD_Axel_Sköld
{
    public class Order
    {
        public List<Products> SelectedProducts { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }

        public decimal Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }

        public Order (DateTime orderDate, DateTime requiredDate, 
            DateTime? shippedDate, decimal freight, string shipName, 
            string shipAddress, string shipCity, string shipRegion, string shipPostalCode, string shipCountry, List<Products>selectedProducts)
        {
            OrderDate = orderDate;
            RequiredDate = requiredDate;
            ShippedDate = shippedDate;
            Freight = freight;
            ShipName = shipName;
            ShipAddress = shipAddress;
            ShipCity = shipCity;
            ShipRegion = shipRegion;
            ShipPostalCode = shipPostalCode;
            ShipCountry = shipCountry;
            SelectedProducts = selectedProducts;
        }
    }
}
