

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class ResultVSM
    {
        public int Id { get; set; }
        public int TimeFinishActual { get; set; }
        public int TimeFinishDefault { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
