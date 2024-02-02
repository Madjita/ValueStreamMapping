using System;
using System.Collections.Generic;
using auntification.Models;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public enum ArchiveSectionRole : byte
    {
        Work = 1,
        Archive = 2,
    }

    public class ArchiveSection
    {
        public int Id { get; set; }
        public ArchiveSectionRole ArchiveSectionRole { get; set; }


        public float? Time { get; set; }            //Время затраченное на Секции

        public int? EtapSectionsId { get; set; }
        public virtual EtapSections EtapSections { get; set; }

        public int? Orders_production_itemsId { get; set; }
        public virtual Orders_production_items OrderItem { get; set; }

    }
}
