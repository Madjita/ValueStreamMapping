using System;
using System.Collections.Generic;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace diplom2.IntermediateData
{
    public class DataAllCardVSM
    {
        public string name { get; set; }
        public List<CardVSM> sections { get; set; } = new List<CardVSM>();      
    }
}
