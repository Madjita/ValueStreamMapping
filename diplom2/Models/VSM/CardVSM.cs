
using System.Collections.Generic;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class CardVSM
    {
        public int Id { get; set; }
        public int? ProductionsId { get; set; }
        public int? EtapNumeric { get; set; }
        public int? EtapVSMId { get; set; }
        public virtual Productions Production { get; set; }
        public virtual BufferVSM BufferVSM { get; set; }
        public virtual EtapVSM EtapVSM { get; set; }

       // public virtual List<EtapSections> ResourceСenter { get; set; } = new List<EtapSections>();
    }
}
