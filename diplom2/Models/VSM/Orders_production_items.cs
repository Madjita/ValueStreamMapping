using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomReactNetCore.DAL.Models.DataBase
{



    public class Orders_production_items
    {
        public int Id { get; set; }
        public OrderRole OrderRole { get; set; }

        public int? Priority { get; set; }
        public int Part { get; set; }
        

        public DateTime? TStart { get; set; }
        public DateTime? TStop { get; set; }

        public float? TDefault { get; set; } // Время которое должно быть затраченно по умолчанию
        public float? TActual { get; set; } // актуальное время
        public float? TFuture { get; set; } // Время  спрогназированное


        public bool? Simulation { get; set; }

       /* public int? ActualEtapVSMId { get; set; }
        public int? ActualEtapSectionsId { get; set; }
        public int? ActualBufferVSMId { get; set; }*/

        public virtual List<ActualOrderCurrentSection> OrderCurrentSection { get; set; } = new List<ActualOrderCurrentSection>();

        public int Orders_productionId { get; set; }

        [JsonIgnore]
        public Orders_production Orders_production { get; set; }

    }
}
