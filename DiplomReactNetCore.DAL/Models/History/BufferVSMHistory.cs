using System;
using System.Collections.Generic;

namespace DiplomReactNetCore.DAL.Models.History
{
    public class BufferVSMHistory
    {
        public int Id { get; set; }
        public int Version { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public int MinHold { get; set; }
        public int Max { get; set; }
        public int Value { get; set; }
        public int ValueDefault { get; set; }       //Сколько нужно для производства 1 штуки 
        public int ReplenishmentSec { get; set; }   //Сколько секунд нужно для пополнения запасов
        public int ReplenishmentCount { get; set; } //Сколько запасов пополнится когда придет время пополнения
        public bool Parallel { get; set; }

        public virtual List<OrderHistory> OrderHistory { get; set; }


        public DateTime TimeStart { get; set; }
        public DateTime TimeChange { get; set; }
    }
}
