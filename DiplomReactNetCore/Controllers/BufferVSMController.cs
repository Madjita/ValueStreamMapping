using System;
using System.Collections.Generic;
using System.Linq;
using DiplomReactNetCore.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomReactNetCore.Controllers
{
    [Route("api/[controller]")]
    public class BufferVSMController : Controller
    {
        private readonly MyContext _context;
        public BufferVSMController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("{name}")]
        public Object Get(string name)
        {
            var data = _context.BufferVSM.Where(i => i.Name == name).ToList().First();

            var result = new
            {
                Name = data.Name,
                Value = data.Value,
                Min = data.MinHold,
                Max = data.Max
            };

            return result;
        }

       /* [HttpGet("{name}")]
        public IEnumerable<Object> Get(string name)
        {
            /*int id = Int32.Parse(Id);

            if(id == 0)
            {
                return null;
            }

            var product = _context.Production.Where(i => i.Name == name).First();

            var data = _context.CardVSM.Where(i => i.ProductionId == product.Id).Include(u => u.BufferVSM).ToList();

            List<Object> result = new List<Object>();

            foreach(var list in data)
            {
                if (list.BufferVSM != null)
                {
                    result.Add(
                     new
                     {
                         Name = list.BufferVSM.Name,
                         Value = list.BufferVSM.Value,
                         Min = list.BufferVSM.MinHold,
                         Max = list.BufferVSM.Max
                     });
                }
            }

            return result;
        }*/
    }
}
