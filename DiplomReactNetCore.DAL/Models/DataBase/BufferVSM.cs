using System.Collections.Generic;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class BufferVSM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int MinHold { get; set; }
        public int Max { get; set; }
        public int Value { get; set; }
        public int ValueDefault { get; set; }       //Сколько нужно для производства 1 штуки 
        public int ReplenishmentSec { get; set; }   //Сколько секунд нужно для пополнения запасов
        public int ReplenishmentCount { get; set; } //Сколько запасов пополнится когда придет время пополнения
        public bool Parallel { get; set; }


        public virtual List<QueueBufferVSM> QueueBufferVSMs { get; set; }

    }
}
