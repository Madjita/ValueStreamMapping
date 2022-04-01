using System;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace diplom2.IntermediateData
{
    public class DataUpdateOrderFormFront
    {
        public Order newOrder { get; set; }
        public Order oldOrder { get; set; }
    }
}
