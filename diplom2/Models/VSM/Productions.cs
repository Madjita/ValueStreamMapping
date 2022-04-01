using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DiplomReactNetCore.DAL.Models.DataBase
{
    public class Productions
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<Orders_production> Orders_production = new List<Orders_production>();
    }
}
