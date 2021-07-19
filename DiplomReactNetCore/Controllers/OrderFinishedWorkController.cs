using System;
using System.Collections.Generic;
using System.Linq;
using DiplomReactNetCore.DAL.Context;
using DiplomReactNetCore.L.Simulation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomReactNetCore.Controllers
{
    [Route("api/[controller]")]
    public class OrderFinishedWorkController : Controller
    {
        private readonly MyContext _context;
        private Manufacture _manufacture;

        public OrderFinishedWorkController(MyContext context, Manufacture manufacture)
        {
            _context = context;
            _manufacture = manufacture;
        }

        [HttpGet]
        public IEnumerable<Object> Get()
        {
            var obj = _context.OrderListFinishedWork.Include(i => i.Order).Include(i => i.Order.Production).ToList();

            List<Object> newObj = new List<Object>();

            foreach (var i in obj)
            {
                newObj.Add(new
                {
                    Id = i.Order.Id,
                    Name = i.Order.Production.Name,
                    Quantity = i.Order.Quantity,
                    TimeActual = (i.Order.TimeActual - i.Order.TimeStart).ToString("h'h 'm'm 's's'"),
                    Simulation = i.Order.Simulation
                });
            }

            return newObj;
        }

    }
}
