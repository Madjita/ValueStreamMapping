
using System;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class QueueBufferVSM
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int TimeWait { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int BufferVSMId { get; set; }
        public virtual BufferVSM BufferVSM { get; set; }

    }
}
