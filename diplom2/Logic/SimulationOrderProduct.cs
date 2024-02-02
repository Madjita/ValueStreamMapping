using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace diplom2.Logic
{
    public class SimulationOrderProduct  // IComparable<SimulationOrderProduct>
    {

        public Orders_production _orders_production;
        public Order _order;


        List<SimulationOrderProductItem> listOrderProductItems = new List<SimulationOrderProductItem>();

      //  int _count = 0;


        //
        public SimulationCardVSM _simulationCardVSM;
        public List<List<SimulationSection>> sections = new List<List<SimulationSection>>(); // Секции в карте
        //


        public SimulationOrderProduct(Orders_production orderProduct, Order order) //: base(order)
        {
            _orders_production = orderProduct;
            _order = order;

            foreach(var orderProductItem in orderProduct.Orders_production_items)
            {
                listOrderProductItems.Add(new SimulationOrderProductItem(orderProductItem,_orders_production,_order));
            }
        }

        public int getId()
        {
            return _orders_production.Id;
        }

        public Orders_production getOrderProduct()
        {
            return _orders_production;
        }

        public int getTime()
        {
            if(_order.TActual == null)
            {
                return 0;
            }
            return (int)_order.TActual;
        }

        public SimulationCardVSM createCard(List<SimulationBufferVSM> _all_bufferVSM, List<SimulationEtapVSM> _all_EtapVSM)
        {
            _simulationCardVSM = new SimulationCardVSM(_orders_production.Production,_all_bufferVSM, _all_EtapVSM);
            _simulationCardVSM.setSimOrderProduction(this);

            //var newData = (DateTime)_order.TAdd;
            //_orders_production.TPlan = newData.AddSeconds(_simulationCardVSM.TWork());

            _orders_production.TPlan = _simulationCardVSM.TWork(_orders_production);

            return _simulationCardVSM;
        }

        public void UpdatePriority(int priority)
        {
            
            using (var _context = new Context(DBConnect.options))
            {
                _context.Orders_production.Attach(_orders_production);
                _orders_production.Priority = priority;

                _context.SaveChanges();

                _context.Entry(_orders_production).State = EntityState.Detached;
            }

            foreach(var orderProductItem in listOrderProductItems)
            {
                orderProductItem.UpdatePriority(priority);
            }

        }

        public void UpdateTime(int time)
        {
            if (_orders_production.OrderRole == OrderRole.Work)
            {
                using (var _context = new Context(DBConnect.options))
                {
                    var obj =  _context.Orders_production.Where(o => o.Id == _orders_production.Id).FirstOrDefault();




                    //Прогнозируем
                    //var newData = (DateTime)_order.TStart;
                    //_orders_production.TPlan = newData.AddSeconds(_simulationCardVSM.TWork());
                    _orders_production.TFuture = _simulationCardVSM.TWork(_orders_production);

                    //Console.WriteLine("TIME TPLAN = "+ _simulationCardVSM.TWork());

                    _orders_production.TActual = time;


                    if(obj != null)
                    {
                        obj.OrderRole = _orders_production.OrderRole;
                        obj.TFuture = _orders_production.TFuture;
                        obj.TActual = _orders_production.TActual;


                        _context.SaveChanges();
                    }
                }



                foreach (var orderProductItem in listOrderProductItems)
                {
                    orderProductItem.UpdateTime(time);
                }
            }
        }

        public JObject ToJson()
        {

            JObject order = new JObject(
                new JProperty("id", _orders_production.Id),
                new JProperty("orderRole", _orders_production.OrderRole),
                new JProperty("priority", _orders_production.Priority),
                new JProperty("quantity", _orders_production.Quantity),
                new JProperty("simulation", _orders_production.Simulation),
                new JProperty("tActual", _orders_production.TActual),
                new JProperty("tAdd", _orders_production.TAdd),
                new JProperty("tFuture", _orders_production.TFuture),
                new JProperty("tPlan", _orders_production.TPlan),
                new JProperty("tStart", _orders_production.TStart),
                new JProperty("tStop", _orders_production.TStop),
                new JProperty("orders_production_items", GetJArrayProductionItems())
            // new JProperty("simulation", _order.Orders_production),
            );

            return order;
        }

        public JArray GetJArrayProductionItems()
        {
            JArray _array_orders = new JArray();
            foreach (var item in listOrderProductItems)
            {

                _array_orders.Add(item.ToJson());

            }

            return _array_orders;
        }


        public List<SimulationOrderProductItem> getOrderProductItems()
        {
            return listOrderProductItems;
        }

        public int CompareTo(SimulationOrderProduct other)
        {

            if (this._orders_production.Priority == null)
            {
                this._orders_production.Priority = 0;
            }

            var p = Int32.Parse(this._orders_production.Priority.ToString()).CompareTo(other._orders_production.Priority);
            // var p = this._order.Priority.ToString().CompareTo(other._order.Priority.ToString());
            return p;
        }

        public float getTPlan()
        {
            return (float)_orders_production.TPlan;
        }

        public float getTFuture()
        {
            return (float)_orders_production.TFuture;
        }

        public async Task StartSim()
        {
            _orders_production.OrderRole = OrderRole.Work;
            _orders_production.Simulation = true;
            _orders_production.TStart = _order.TStart;

            using (var _context = new Context(DBConnect.options))
            {
                var obj = _context.Orders_production.Where(o => o.Id == _orders_production.Id).FirstOrDefault();

                obj.TStart = _order.TStart;
                obj.Simulation = _orders_production.Simulation;
                obj.OrderRole = _orders_production.OrderRole;

                _context.SaveChanges();
            }


            foreach (var item in listOrderProductItems)
            {
                await item.StartSim();
            }
        }

        internal void StopSim()
        {
            _orders_production.OrderRole = OrderRole.Stoped;
            _orders_production.TActual = 0;

            using (var _context = new Context(DBConnect.options))
            {
                var obj = _context.Orders_production.Where(o => o.Id == _orders_production.Id).FirstOrDefault();


                obj.Simulation = _orders_production.Simulation;
                obj.OrderRole = _orders_production.OrderRole;
                obj.TActual = _orders_production.TActual;


                _context.SaveChanges();
            }

            
            foreach (var item in listOrderProductItems)
            {
                item.StopSim();
            }
        }

        internal void Done()
        {
            _orders_production.OrderRole = OrderRole.Archive;
            _orders_production.TActual = 0;
            _orders_production.Simulation = false;

            using (var _context = new Context(DBConnect.options))
            {
                var obj = _context.Orders_production.Where(o => o.Id == _orders_production.Id).FirstOrDefault();

                obj.Simulation = _orders_production.Simulation;
                obj.OrderRole = _orders_production.OrderRole;
                obj.TActual = _orders_production.TActual;


                _context.SaveChanges();
            }

            /*
            foreach (var item in listOrderProductItems)
            {
                item.Done();
            }*/
        }

        internal int SetCard(SimulationCardVSM simulationCardVSM)
        {
            _simulationCardVSM = simulationCardVSM;
            _simulationCardVSM.setSimOrderProduction(this);

            _orders_production.TPlan = _simulationCardVSM.TWork(_orders_production);


            using (var _context = new Context(DBConnect.options))
            {
                var obj = _context.Orders_production.Where(o => o.Id == _orders_production.Id).FirstOrDefault();
                obj.TPlan = _orders_production.TPlan;
                _context.SaveChanges();
            }


            return (int)_orders_production.TPlan;
        }

    }
}
