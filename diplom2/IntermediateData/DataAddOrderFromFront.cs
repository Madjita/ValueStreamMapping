using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace diplom2.IntermediateData
{

    public class DataAddOrderFromFront_OrderProduct
    {
        public string name { get; set; }
        public int quantity { get; set; }
    }

    public class DataAddOrderFromFront
    {
        public string name { get; set; }
        public int priority { get; set; }
        public List<DataAddOrderFromFront_OrderProduct> products { get; set; } = new List<DataAddOrderFromFront_OrderProduct>();
    }

}
