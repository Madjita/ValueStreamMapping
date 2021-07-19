using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DiplomReactNetCore.DAL.Models.DataBase;
using DiplomReactNetCore.DAL.Context;
using System.Threading.Tasks;
using DiplomReactNetCore.L.Simulation;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using DiplomReactNetCore.DAL.Models.Work;

namespace DiplomReactNetCore.Controllers
{

    public class JsonProduction
    {
        public string ProductionName { get; set; }
        public int Quantity { get; set; }
    }

    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly MyContext _context;
        private Manufacture _manufacture;

        public OrderController(MyContext context, Manufacture manufacture)
        {
            _context = context;
            _manufacture = manufacture;
        }


        [HttpGet]
        public IEnumerable<Object> Get()
        {
            var obj = _context.OrderListForWork.Include(i => i.Order).Include(i => i.Order.Production).ToList();

            List<Object> newObj = new List<Object>();

            foreach(var i in obj)
            {
                newObj.Add(new
                {
                    Id = i.Order.Id,
                    Name = i.Order.Production.Name,
                    Quantity = i.Order.Quantity,
                    TimeActual = (i.Order.TimeActual - i.Order.TimeStart).ToString("h'h 'm'm 's's'"),
                    //TimeDefault = i.Order.TimeDefault,
                    Simulation = i.Order.Simulation
                });
            }

            return newObj;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonProduction item)
        {
            var obj = _context.Production.Where(i => i.Name == item.ProductionName).First();

            if(obj != null)
            {
                Order newObj = new Order
                {
                    ProductionId = obj.Id,
                    Quantity = item.Quantity
                };
                _context.Order.Add(newObj);
                await _context.SaveChangesAsync();

                var newobjWork = new OrderListForWork
                {
                    OrderId = newObj.Id
                };
                _context.OrderListForWork.Add(newobjWork);
                await _context.SaveChangesAsync();

                _manufacture.AddOrder(newObj);

                return Ok(obj.Id);
            }
            else
            {
                return Ok(false);
            }
        }

    }
}


/*
 * 
 * 
            //1) Валидация
            //2) Другой проект работает с базой

           // var lol = _context.Production.Include(i => i.Orders); // .Find(item.Production.Id);

                .Orders.Add(
                new Order {
                    Quantity = item.Quantity
                });

*
* 
 */