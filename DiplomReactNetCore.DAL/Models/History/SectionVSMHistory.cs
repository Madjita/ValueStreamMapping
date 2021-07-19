using System;
using System.Collections.Generic;

namespace DiplomReactNetCore.DAL.Models.History
{
    public class SectionVSMHistory
    {
        public int Id { get; set; }

        public virtual List<BufferVSMHistory> BufferVSMHistorys { get; set; }
        public virtual List<EtapVSMHistory> EtapVSMHistorys { get; set; }

        public int QueuqOrderHistoryId { get; set; }
        public virtual List<QueuqOrderHistory> QueuqOrderHistory { get; set; }

        public DateTime TimeAdd { get; set; }
        public int TimeActual { get; set; }
        public DateTime TimeStop { get; set; }
    }
}
