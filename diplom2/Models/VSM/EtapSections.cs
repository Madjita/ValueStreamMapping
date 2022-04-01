
using System;
using System.Collections.Generic;
using auntification.Models;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class EtapSections
    {
        public int Id { get; set; }
        public bool? Work { get; set; }

        public float? TActual { get; set; }
        public float? TMax { get; set; }
        public float? TMin { get; set; }

       

        public int? EtapVSMId { get; set; }
        //public int? CardVSMId { get; set; }

        public User User { get; set; }
        public int? Orders_production_itemsId { get; set; }
        public virtual Orders_production_items CurrentOrderItems { get; set; }

        public virtual List<ArchiveSection> ArchiveSection { get; set; } = new List<ArchiveSection>();

    }
}
