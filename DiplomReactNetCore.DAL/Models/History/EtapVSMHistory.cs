using System;
using System.Collections.Generic;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace DiplomReactNetCore.DAL.Models.History
{
    public class EtapVSMHistory
    {
        public int Id { get; set; }
        public int Version { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int ActualTimeCircle { get; set; }
        public int DefaultTimeCircle { get; set; }
        public int ActualTimePreporation { get; set; }
        public int DefaultTimePreporation { get; set; }
        public int ActualAvailability { get; set; }
        public int Time { get; set; }
        public bool Parallel { get; set; }

        /*public int OrderId { get; set; } */
        public virtual List<OrderHistory> OrderHistory { get; set; }

        public DateTime TimeStart { get; set; }
        public DateTime TimeChange { get; set; }
    }
}
