using System;
using System.Collections.Generic;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace DiplomReactNetCore.DAL.Models.History
{
    public class OrderHistory
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }


        public int Quantity { get; set; }
        public bool Simulation { get; set; }

        public DateTime TimeAdd { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeStop { get; set; }
        public DateTime TimeActual { get; set; }
        public DateTime TimeDefault { get; set; }

        public int ProductionId { get; set; }
        public virtual Production Production { get; set; }


        //public virtual List<EtapVSMHistory> EtapVSMHistorys { get; set; }
        //public virtual List<BufferVSMHistory> BufferVSMHistorys { get; set; }


    }
}

