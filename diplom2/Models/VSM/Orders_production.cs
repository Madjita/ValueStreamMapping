using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public enum OrderRole : byte
    {
        Actual = 1,
        Work = 2,
        Archive = 3,
        Stoped = 4,
    }

    public class Orders_production
    {

        public int Id { get; set; }
        public OrderRole OrderRole { get; set; }
        public int? Priority { get; set; }
        public int? Quantity { get; set; }   //Колчиство деталей в партии
        public bool? Simulation { get; set; }

        public DateTime? TAdd { get; set; } //время добавления (Полное время прохождения заказа на Партию)

        public DateTime? TStart { get; set; } //начальное время (Полное время прохождения заказа на Партию)
        public DateTime? TStop { get; set; } //конечное время (Полное время прохождения заказа на Партию)


        public float? TActual { get; set; } // актуальное время
        //public DateTime? TPlan { get; set; } // Время которое должно быть затраченно по умолчанию
        public float? TPlan { get; set; } // Время которое должно быть затраченно по умолчанию
        public float? TFuture { get; set; } // прогназируемое

        public List<Orders_production_items> Orders_production_items { get; set; } = new List<Orders_production_items>();

        public int ProductionsId { get; set; }
        public Productions Production { get; set; }

        public int OrderId { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }

      

    }
}
