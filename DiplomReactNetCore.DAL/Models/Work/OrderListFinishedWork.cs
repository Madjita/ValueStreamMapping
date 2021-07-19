using System;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace DiplomReactNetCore.DAL.Models.Work
{
    public class OrderListFinishedWork
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
