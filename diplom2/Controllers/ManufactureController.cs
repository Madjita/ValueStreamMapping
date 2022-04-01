using System;
using Microsoft.AspNetCore.Mvc;
using DiplomReactNetCore.DAL.Models.DataBase;
using System.Threading.Tasks;
using diplom2.Data;
using System.Linq;
using diplom2.IntermediateData;
using Microsoft.EntityFrameworkCore;
using diplom2.Logic;
using System.Collections.Generic;

namespace diplom2.Controllers
{
    [Route("api/manufacture")]
    public class ManufactureController : Controller
    {
        private readonly Context _context;
        private Manufacture _manufacture;

        public ManufactureController(Context context, Manufacture manufacture) //Manufacture manufacture
        {
            _context = context;
            _manufacture = manufacture;
        }


        [HttpPost("updateInfoOrder")]
        public IActionResult updateInfoOrder([FromBody] Order order)
        {
            var findOrder = _manufacture.FindOrder(order);

            return Ok(findOrder);
        }


        [HttpGet("orders2")]
        public async Task<IEnumerable<object>> orders2()
        {
            //var find  = _manufacture.FindOrder(item);

            var list = await _context
                           .Order
                           .Select(c => new
                           {
                               c.Id,
                               c.Name,
                               c.OrderRole,
                               c.Priority,
                               c.Simulation,
                               c.TActual,
                               c.TAdd,
                               c.TFuture,
                               c.TPlan,
                               c.TStart,
                               c.TStop,
                               Orders_production = c.Orders_production
                                                   .Select(e => new {
                                                       e.Id,
                                                       e.Priority,
                                                       e.OrderRole,
                                                       e.Quantity,
                                                       e.Simulation,
                                                       e.TActual,
                                                       e.TAdd,
                                                       e.TFuture,
                                                       e.TPlan,
                                                       e.TStart,
                                                       e.TStop,
                                                       e.Production,
                                                       Orders_production_items = e.Orders_production_items
                                                           .Select(k => new
                                                           {
                                                               k.Id,
                                                               k.OrderRole,
                                                               k.Part,
                                                               k.Priority,
                                                               k.Simulation,
                                                               k.TActual,
                                                               k.TDefault,
                                                               k.TFuture,
                                                               k.TStart,
                                                               k.TStop,
                                                               k.Orders_production,
                                                           }).ToList()
                                                   })
                                                   .ToList()
                           }).AsNoTracking().ToListAsync();



            List<object> newList = new List<object>();

            foreach(var order in list)
            {
              


                List<List<CardVSM>> cards = new List<List<CardVSM>>();

                foreach(var product in order.Orders_production)
                {
                    var findProductCard = await _context.CardVSM
                                        .Include(o => o.Production)
                                         .Include(o => o.BufferVSM)
                                             .ThenInclude(o => o.BufferVSMQueue.Where(q => q.BufferRole == BufferRole.Wait))
                                                .ThenInclude(o => o.CurrentOrderItems)

                                         .Include(o => o.EtapVSM)
                                             .ThenInclude(o => o.EtapSections)
                                                .ThenInclude(o => o.ArchiveSection.Where(q => q.ArchiveSectionRole == ArchiveSectionRole.Work))
                                                    .ThenInclude(o => o.OrderItem)
                                        .Where(o => o.Production.Id == product.Production.Id)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .ToListAsync();
                                       

                    cards.Add(findProductCard);
                }


                var obj = new
                {
                    order = order,
                    cards = cards
                };

                newList.Add(obj);

            }


            return newList;
        }



        [HttpPost("startSimulationOrder")]
        public IActionResult StartSimulation([FromBody] Order item)
        {

            _manufacture.StartSimulation(item);

            return Ok();
        }


        [HttpGet("getAllProducts")]
        public async Task<IEnumerable<object>> getAllProducts()
        {
            //var list = _context.Productions.ToList();


            var list = await _context
                      .Productions
                      .Select(c => new
                      {
                          c.Id,
                          c.Name,
                      }).AsNoTracking().ToListAsync();

            return list;
        }


        [HttpPost("updateOrder")]
        public IActionResult updateOrder([FromBody] DataUpdateOrderFormFront order)
        {
            Console.WriteLine("UPDATEORDER");
            //Проверить есть ли такой заказ в базе
            var findItem = _context.Order
                .Where(o => o.Name == order.oldOrder.Name)
                .Include(o=> o.Orders_production)
                .ThenInclude(o=> o.Orders_production_items)
                .AsSplitQuery()
                .FirstOrDefault();


            findItem.Name = order.newOrder.Name;

            findItem.OrderRole = order.newOrder.OrderRole;

            if(findItem.Priority != order.newOrder.Priority)
            {
                findItem.Priority = order.newOrder.Priority;
                foreach (var itemOrderProduct in findItem.Orders_production)
                {
                    itemOrderProduct.Priority = findItem.Priority;

                    foreach(var itemOrderProductionItem in itemOrderProduct.Orders_production_items)
                    {
                        itemOrderProductionItem.Priority = findItem.Priority;
                    }

                }
            }
            

            _context.SaveChanges();


            _manufacture.UpdateOrder(findItem);

            return Ok();
        }

        [HttpPost("addOrder")]
        public IActionResult addOrder([FromBody]  DataAddOrderFromFront item)
        {
            //Проверить есть ли такой заказ в базе
            var findItem = _context.Order.Where(o => o.Name == item.name).FirstOrDefault();

            if(findItem == null)
            {
                //Если нету то добавить предварительно найдем продукты

                var newOrder = new Order();
                newOrder.Name = item.name;
                newOrder.Priority = item.priority;
                newOrder.OrderRole = OrderRole.Actual;
                newOrder.Simulation = false;
                newOrder.TAdd = DateTime.UtcNow;


                foreach (var productItem in item.products)
                {
                    var newOrderProduct = new Orders_production();

                    try
                    {
                        //Поиск продукции
                        var findProduction = _context.Productions.Where(o => o.Name == productItem.name).FirstOrDefault();

                        if (findProduction == null)
                        {
                            throw new Exception();
                        }


                        newOrderProduct.TAdd = newOrder.TAdd;
                        newOrderProduct.ProductionsId = findProduction.Id;
                        newOrderProduct.OrderRole = OrderRole.Actual;
                        newOrderProduct.Priority = newOrder.Priority;
                        newOrderProduct.Quantity = productItem.quantity;


                        //Создаем заказики под этот продукт

                        for(int i=0; i < newOrderProduct.Quantity;i++)
                        {
                            var newOrderProductItem = new Orders_production_items();

                            newOrderProductItem.OrderRole = OrderRole.Actual;
                            newOrderProductItem.Part = i + 1;
                            newOrderProductItem.Priority = newOrderProduct.Priority;

                            newOrderProduct.Orders_production_items.Add(newOrderProductItem);
                        }

                        newOrder.Orders_production.Add(newOrderProduct);
                    }
                    catch(Exception e)
                    {
                        return BadRequest(e);
                    }

                }


                _context.Order.Attach(newOrder);
                _context.SaveChanges();


                _manufacture.addSimOrder(newOrder);
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("orders")]
        public async Task<IEnumerable<object>> Orders()
        {

            /* var list =  _context.Order
                 .Include(o=> o.Orders_production)
                 .ThenInclude(o=> o.Production)
                 .ToList(); */


            var list = await _context
                        .Order
                        .Select(c => new
                        {
                            c.Id,
                            c.Name,
                            c.OrderRole,
                            c.Priority,
                            c.Simulation,
                            c.TActual,
                            c.TAdd,
                            c.TFuture,
                            c.TPlan,
                            c.TStart,
                            c.TStop,
                            Orders_production = c.Orders_production
                                                .Select(e => new {
                                                    e.Id,
                                                    e.Priority,
                                                    e.OrderRole,
                                                    e.Quantity,
                                                    e.Simulation,
                                                    e.TActual,
                                                    e.TAdd,
                                                    e.TFuture,
                                                    e.TPlan,
                                                    e.TStart,
                                                    e.TStop,
                                                    e.Production,
                                                    Orders_production_items = e.Orders_production_items
                                                        .Select(k => new
                                                        {
                                                            k.Id,
                                                            k.OrderRole,
                                                            k.Part,
                                                            k.Priority,
                                                            k.Simulation,
                                                            k.TActual,
                                                            k.TDefault,
                                                            k.TFuture,
                                                            k.TStart,
                                                            k.TStop,
                                                            k.Orders_production,
                                                        }).ToList()
                                                })
                                                .ToList()
                        }).AsNoTracking().ToListAsync();

            return  list;
        }


        [HttpPost("updateInfoBuf")]
        public async Task<IActionResult> UpdateInfoBuf([FromBody] BufferVSM buf)
        {

            var find = await _context.BufferVSM
                .Select(o => new
                {
                    o.Id,
                    o.Max,
                    o.MinHold,
                    o.Name,
                    o.ReplenishmentCount,
                    o.ReplenishmentSec,
                    o.Type,
                    o.Value,
                    o.ValueDefault,
                    BufferVSMQueue = o.BufferVSMQueue
                            .Select(k => new
                            {
                                k.Id,
                                k.BufferRole,
                                k.TAdd,
                                k.TFuture,
                                k.TPop,
                                k.TWait,
                                
                                CurrentOrderItems = new 
                                {
                                    Id = k.CurrentOrderItems.Id,
                                    Part = k.CurrentOrderItems.Part,
                                    Priority = k.CurrentOrderItems.Priority,
                                    Orders_production = new 
                                    {
                                        Id = k.CurrentOrderItems.Orders_production.Id,
                                        Quantity = k.CurrentOrderItems.Orders_production.Quantity,
                                        Production = k.CurrentOrderItems.Orders_production.Production
                                    }

                                },


                            })
                            .Where(o => o.BufferRole == BufferRole.Wait)
                            .ToList()
                }).Where(o => o.Id == buf.Id)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            return Ok(find);
        }
    }
}
