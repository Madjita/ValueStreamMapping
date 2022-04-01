
using System;
using System.Collections.Generic;
using auntification.Models;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class EtapVSM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float? ActualTimeCircle { get; set; }
        public float? DefaultTimeCircle { get; set; }
        public float? ActualTimePreporation { get; set; }
        public float? DefaultTimePreporation { get; set; }
        public float? ActualAvailability { get; set; }
        public float? DefaultAvailability { get; set; }
        public bool? Parallel { get; set; }

        public virtual List<EtapSections> EtapSections { get; set; } = new List<EtapSections>();
    }
}
