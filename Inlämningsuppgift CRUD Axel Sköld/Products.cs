using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämningsuppgift_CRUD_Axel_Sköld
{
    public class Products
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }

    

        public Products(int productID, string productName,
                        decimal? unitPrice, short? unitsInStock)
        {
            ProductID = productID;
            ProductName = productName;
            UnitPrice = unitPrice;
            UnitsInStock = unitsInStock;
        }
    }
}
