using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DiplomReactNetCore.DAL.Context;
using System.Linq;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace DiplomReactNetCore.Controllers
{
    [Route("api/[controller]")]
    public class ProductionController : Controller
    {

        private readonly MyContext _context;
        public ProductionController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Object> Get()
        {
            return _context.Production.Select(i => new { i.Id, i.Name }).ToList();
        }

    }
}
