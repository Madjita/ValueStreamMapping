
using System;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class EtapVSM
    {
        public int Id { get; set; }
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
        public Order Order { get; set; }

    }
}
