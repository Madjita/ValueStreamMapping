using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using DiplomReactNetCore.DAL.Context;
using DiplomReactNetCore.L.Simulation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DiplomReactNetCore.Controllers
{
    [Route("api/[controller]")]
    public class ManufactureController : Controller
    {
        private readonly MyContext _context;
        private  Manufacture _manufacture;

        public ManufactureController(MyContext context, Manufacture manufacture) 
        {
            _context = context;
            _manufacture = manufacture;
        }

        [HttpGet]
        public ContentResult Get()
        {
            var lol = _manufacture.ToJson();
            return Content(lol.ToString(), "application/json");
        }

        [HttpGet("{id}")]
        public ContentResult Orders(string id)
        {
            var lol = _manufacture.getAllOrdersFromEtap(Int32.Parse(id));
            if(lol == null)
            {
                lol = new JArray();
            }
            return Content(lol.ToString(), "application/json");
        }

    }
}
