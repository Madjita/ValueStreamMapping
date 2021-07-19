using System.Collections.Generic;


namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class Production
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Order> Orders { get; set; }
    }
}
