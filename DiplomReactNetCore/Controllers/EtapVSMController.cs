using System;
using System.Linq;
using DiplomReactNetCore.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomReactNetCore.Controllers
{
    [Route("api/[controller]")]
    public class EtapVSMController : Controller
    {
        private readonly MyContext _context;

        public EtapVSMController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public Object Get(string id)
        {
            var data = _context.EtapVSM.Where(i => i.Id == Int32.Parse(id)).Include(i=>i.Order).Include(i => i.Order.Production).ToList().First();
            Object result = null;

            if (data.Order == null)
            {
                result = new
                {
                    Name = data.Name,
                    Description = data.Description,
                };
            }
            else
            {
                result = new
                {
                    Name = data.Name,
                    Description = data.Description,
                    ProductionName = data.Order.Production.Name,
                    Quantity = data.Order.Quantity,
                    TimeOrder = (data.Order.TimeActual - data.Order.TimeStart).ToString("h'h 'm'm 's's'"),
                    TimeActual = TimeSpan.FromMilliseconds(data.ActualTimeCircle).ToString("h'h 'm'm 's's'"),
                };
            }
           

            return result;
        }
    }
}
