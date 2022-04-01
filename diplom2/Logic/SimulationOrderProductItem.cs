using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace diplom2.Logic
{
    public class SimulationOrderProductItem : IComparable<SimulationOrderProductItem>
    {

        Orders_production_items _orderProductItem;
        Orders_production _orderProduct;
        Order _order;

     
        public SimulationOrderProductItem(Orders_production_items orderProductItem, Orders_production orderProduct, Order order)
        {
            _orderProductItem = orderProductItem;
            _orderProduct = orderProduct;
            _order = order;
        }

        public int getId()
        {
            return _orderProductItem.Id;
        }

        public int getTActual()
        {
            return (int)_orderProductItem.TActual;
        }

        public void UpdatePriority(int priority)
        {

            using (var _context = new Context(DBConnect.options))
            {
                _context.Entry(_orderProductItem).State = EntityState.Detached;
                _context.Orders_production_items.Attach(_orderProductItem);
                _orderProductItem.Priority = priority;
                _context.SaveChanges();

                
            }

        }

        public void UpdateTime(int time)
        {
            if (_orderProductItem.OrderRole == OrderRole.Work)
            {
                _orderProductItem.TActual = time;

                using (var _context = new Context(DBConnect.options))
                {
                    var obj =  _context.Orders_production_items.Where(o => o.Id == _orderProductItem.Id).FirstOrDefault();

                    if (obj != null)
                    {
                        obj.TActual = _orderProductItem.TActual;
                        _context.SaveChanges();
                    }
                }
            }
        }




        public Orders_production_items getOrderProductItem()
        {
            return _orderProductItem;
        }

        public async Task StartSim()
        {
            _orderProductItem.OrderRole = OrderRole.Work;
            _orderProductItem.TActual = 0;
            _orderProductItem.Simulation = true;
            _orderProductItem.TStart = _order.TStart;

            using (var _context = new Context(DBConnect.options))
            {
                var obj = await _context.Orders_production_items.Where(o => o.Id == _orderProductItem.Id).FirstOrDefaultAsync();

                obj.TStart = _orderProductItem.TStart;
                obj.TStop = null;
                obj.Simulation = _orderProductItem.Simulation;
                obj.TActual = _orderProductItem.TActual;
                obj.OrderRole = _orderProductItem.OrderRole;

                await _context.SaveChangesAsync();
            }
        }

        internal void StopSim()
        {
            if(_orderProductItem.OrderRole == OrderRole.Archive)
            {
                return;
            }

            _orderProductItem.OrderRole = OrderRole.Stoped;
            _orderProductItem.TActual = 0;
            _orderProductItem.Simulation = false;

            using (var _context = new Context(DBConnect.options))
            {
                var obj = _context.Orders_production_items.Where(o => o.Id == _orderProductItem.Id).FirstOrDefault();

                obj.Simulation = _orderProductItem.Simulation;
                obj.TActual = _orderProductItem.TActual;
                obj.OrderRole = _orderProductItem.OrderRole;
                obj.TStop = DateTime.Now;

                _context.SaveChanges();
            }
        }

        internal void Done()
        {
            if(_orderProductItem.OrderRole != OrderRole.Archive)
            {
                _orderProductItem.OrderRole = OrderRole.Archive;
                _orderProductItem.TActual = 0;
                _orderProductItem.Simulation = false;

                using (var _context = new Context(DBConnect.options))
                {
                    var obj = _context.Orders_production_items.Where(o => o.Id == _orderProductItem.Id).FirstOrDefault();

                    obj.Simulation = _orderProductItem.Simulation;
                    obj.TActual = _orderProductItem.TActual;
                    obj.OrderRole = _orderProductItem.OrderRole;
                    obj.TStop = DateTime.Now;

                    _context.SaveChanges();
                }
            }
        }


        public async Task UpdateBuf(int bufId)
        {
            using (var _context = new Context(DBConnect.options))
            {
                _context.Orders_production_items.Attach(_orderProductItem);
                //_orderProductItem.ActualBufferVSMId = bufId;
                //_orderProductItem.ActualEtapSectionsId = null;
                //_orderProductItem.ActualEtapVSMId = null;


                await _context.SaveChangesAsync();


                _context.Entry(_orderProductItem).State = EntityState.Detached;
            }

        }

        public int CompareTo([AllowNull] SimulationOrderProductItem other)
        {
            if (this._orderProductItem.Priority == null)
            {
                this._orderProductItem.Priority = 0;
            }

            int this_p = (int)this._orderProductItem.Priority; //Int32.Parse(this._orderProductItem.Priority.ToString());


            var p = this_p.CompareTo(other._orderProductItem.Priority);
            return p;
        }

        public JObject ToJson()
        {

            JObject order = new JObject(
                new JProperty("id", _orderProductItem.Id),
                new JProperty("orderRole", _orderProductItem.OrderRole),
                new JProperty("priority", _orderProductItem.Priority),
                new JProperty("part", _orderProductItem.Part),
                new JProperty("simulation", _orderProductItem.Simulation),
                new JProperty("tDefault", _orderProductItem.TDefault),
                new JProperty("tActual", _orderProductItem.TActual),
                new JProperty("tFuture", _orderProductItem.TFuture),
                new JProperty("tStart", _orderProductItem.TStart),
                new JProperty("tStop", _orderProductItem.TStop)
              
            // new JProperty("simulation", _order.Orders_production),
            );

            return order;
        }

    }
}
