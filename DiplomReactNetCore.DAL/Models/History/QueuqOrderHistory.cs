using System;
using System.Collections.Generic;

namespace DiplomReactNetCore.DAL.Models.History
{
    public class QueuqOrderHistory
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int TimeWait { get; set; }

        public int BufferVSMId { get; set; }
        //public virtual BufferVSM BufferVSM { get; set; }


        public virtual List<OrderHistory> OrderHistorys { get; set; }
    }
}
