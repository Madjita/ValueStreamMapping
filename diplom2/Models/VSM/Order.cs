using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class Order
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("orderRole")]
        public OrderRole OrderRole { get; set; }
        [JsonPropertyName("priority")]
        public int? Priority { get; set; }

        [JsonPropertyName("tAdd")]
        public DateTime? TAdd { get; set; } //время добавления (Полное время прохождения заказа на Партию)
        [JsonPropertyName("tStart")]
        public DateTime? TStart { get; set; } //начальное время (Полное время прохождения заказа на Партию)
        [JsonPropertyName("tStop")]
        public DateTime? TStop { get; set; } //конечное время (Полное время прохождения заказа на Партию)

        [JsonPropertyName("tActual")]
        public float? TActual { get; set; } // актуальное время
        [JsonPropertyName("tPlan")]
        public float? TPlan { get; set; } // Время которое должно быть затраченно по умолчанию
        [JsonPropertyName("tFuture")]
        public float? TFuture { get; set; } // прогназируемое

        [JsonPropertyName("simulation")]
        public bool? Simulation { get; set; }

        public List<Orders_production> Orders_production { get; set; }  = new List<Orders_production>();
    }
}
