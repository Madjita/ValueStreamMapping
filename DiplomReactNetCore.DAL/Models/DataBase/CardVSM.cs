
namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class CardVSM
    {
        public int Id { get; set; }
        public int ProductionId { get; set; }
        public int EtapNumeric { get; set; }
        //public int BufferVSMId { get; set; }
        public int EtapVSMId { get; set; }
        public virtual Production Production { get; set; }
        public virtual BufferVSM BufferVSM { get; set; }
        public virtual EtapVSM EtapVSM { get; set; }
        public virtual ResultVSM ResultVSM { get; set; }
    }
}
