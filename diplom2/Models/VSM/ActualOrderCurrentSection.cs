using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public enum OrderSectionState : byte
    {
        Wait = 1,
        Work = 2,
        waitingNext = 3
    }


    public class ActualOrderCurrentSection
    {
        public int Id { get; set; }
        public OrderSectionState OrderSectionState { get; set; }

        public int? ActualEtapVSMId { get; set; }
        public int? ActualEtapSectionsId { get; set; }
        public int? ActualBufferVSMId { get; set; }

        public int Orders_production_itemsId { get; set; }
        [JsonIgnore]
        public virtual Orders_production_items Orders_production_items { get; set; }
    }
}
