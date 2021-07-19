using System;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class Order
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public bool Simulation { get; set; }

        public DateTime TimeAdd { get; set; }

        public DateTime TimeStart { get; set; }
        public DateTime TimeStop { get; set; }


        public DateTime TimeActual { get; set; }
        public DateTime TimeDefault { get; set; }

        public int ProductionId { get; set; }
        public virtual Production Production { get; set; }

    }
}
