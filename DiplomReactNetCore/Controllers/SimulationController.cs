using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DiplomReactNetCore.DAL.Models.DataBase;
using DiplomReactNetCore.DAL.Context;
using DiplomReactNetCore.L.Simulation;
using System.Linq;

namespace DiplomReactNetCore.Controllers
{

    public class JsonSimulationOrder
    {
        public int Id { get; set; }
        public string ProductionName { get; set; }
        public bool start { get; set; }
    }

    [Route("api/[controller]")]
    public class SimulationController : Controller
    {
        private IHttpContextAccessor _accessor; // Object ip Client;
        private Manufacture _sim;     // Object simulation;
        private readonly MyContext _context;  // Object Conenction BD;


        public SimulationController(IHttpContextAccessor accessor, MyContext context, Manufacture sim)
        {
            _context = context;
            _sim = sim;
            _accessor = accessor;

            string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            //Console.WriteLine("remoteIpAddress = " + ip);
        }

        [HttpGet]
        public bool Get()
        {
            return _sim.getStart();
        }

        [HttpPost]
        public IActionResult Post([FromBody] JsonSimulationOrder item)
        {

            var obj = _context.Order.Where(i => i.Id == item.Id && i.Production.Name == item.ProductionName).SingleOrDefault();
            obj.Simulation = item.start;
            _sim.StartWork(obj);
            return Ok(_sim.getStart());
        }

    }
}
