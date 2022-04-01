using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public enum BufferRole : byte
    {
        Wait = 1,
        Archive = 2,
        Stop = 3,
    }

    public class BufferVSMQueue
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Key]
        public int Id { get; set; }
        public BufferRole BufferRole { get; set; }

        public DateTime? TAdd { get; set; } // Время когда добавили в буфер заказ
        public DateTime? TPop { get; set; } // Время когда вытащили из буфера заказ

        public float? TWait { get; set; } // Время ожидания в этом буфере

        //public float? TActual { get; set; } // актуальное время
        public float? TFuture { get; set; } // Время  спрогназированное

        public int? Orders_production_itemsId { get; set; }
        public Orders_production_items CurrentOrderItems { get; set; }

        public int? BufferVSMId { get; set; }

    }
}
