using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Newtonsoft.Json.Linq;

namespace diplom2.Logic
{

  

    class SimulationOrderEqualityComparer : IEqualityComparer<SimulationOrder>
    {
        public static SimulationOrderEqualityComparer Comparer { get; } = new SimulationOrderEqualityComparer();
        private SimulationOrderEqualityComparer()
        {

        }

        public bool Equals(SimulationOrder b1, SimulationOrder b2)
        {
            if (b2 == null && b1 == null)
                return true;
            else if (b1 == null || b2 == null)
                return false;
            else if (b1.getORder().Id == b2.getORder().Id && b1.getORder().Name == b2.getORder().Name)
                return true;
            else
                return false;
        }

        public int GetHashCode(SimulationOrder bx)
        {
            int hCode = bx.getORder().Id;
            return hCode.GetHashCode();
        }
    }

    /*
    public class frontCardSection
    {
        public SimulationBufferVSM _simulationBufferVSM;
        public SimulationEtapVSM _simulationEtapVSM;
    }

    public class frontCards
    {
        public Productions _product;

        public List<CardVSM> _card; //Карта с этапами и буферами
        public List<List<frontCardSection>> sections; // Секции в карте

    }

    public class frontOrder
    {
        public Order _order;
        public List<frontCards> cards;
    }*/

    public struct SyncingTable
    {
        public EventWaitHandle go;
        public bool ready;
        public object loker;
    }


    public class SimulationOrder
    {
       
        int _count = 0;
        bool _threadStart = true;
        object myLock = new object();
        object loker1 = new object();
       

        Order _order;
        CancellationTokenSource cts;

        List<SimulationOrderProduct> listOrderProducts = new List<SimulationOrderProduct>();

        //Внутри заказа должна построиться карта относительно продуктов которые есть в казаке
        List<SimulationCardVSM> simulationCardVSMs = new List<SimulationCardVSM>();

        public SimulationOrder(Order order)
        {
            _order = order;

            foreach(var product in _order.Orders_production)
            {
                listOrderProducts.Add(new SimulationOrderProduct(product,_order));
            }

        }

        public JObject ToJson()
        {

            JObject order = new JObject(
                new JProperty("id", _order.Id),
                new JProperty("name", _order.Name),
                new JProperty("orderRole", _order.OrderRole),
                new JProperty("priority", _order.Priority),
                new JProperty("simulation", _order.Simulation),
                new JProperty("tActual", _order.TActual),
                new JProperty("tAdd", _order.TAdd),
                new JProperty("tFuture", _order.TFuture),
                new JProperty("tPlan", _order.TPlan),
                new JProperty("tStart", _order.TStart),
                new JProperty("tStop", _order.TStart),
                new JProperty("orders_production", GetJArrayProduction())
            // new JProperty("simulation", _order.Orders_production),
            );
            return order;
        }

        public JArray GetJArrayProduction()
        {
            JArray _array_orders = new JArray();
            foreach (var item in listOrderProducts)
            {

                _array_orders.Add(item.ToJson());

            }

            return _array_orders;
        }


        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                SimulationOrder p = (SimulationOrder)obj;
                return (_order.Id == p._order.Id);
            }
        }


        public void createCard(List<SimulationBufferVSM> _all_buffer, List<SimulationEtapVSM> _all_Etap)
        {
            int _TFuture = 0;

            foreach (var product in listOrderProducts)
            {
                //Создаем карту для продукта
                var card = product.createCard(_all_buffer, _all_Etap);
                simulationCardVSMs.Add(card);


                _TFuture += product.getTime();

            }

            Console.WriteLine("Карты созданы примерное время выполения: " + _TFuture);

        }


        public void Run()
        {
            CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(99999));

            Console.WriteLine("Starting action loop. ( " + _order.Name + " )");
            RepeatActionEvery(() =>  Do(), TimeSpan.FromSeconds(1), cancellation.Token).Wait();

            Console.WriteLine("Finished action loop. ( " + _order.Name + " )");

        }


        public int getId()
        {
            return _order.Id;
        }

        public Order getORder()
        {
            return _order;
        }

        public void StartSim()
        {
            Console.WriteLine("Start sim");
            //ThreadPool.QueueUserWorkItem(delegate (object state) { Run(); });
            ThreadPool.QueueUserWorkItem(delegate (object state) { Run(); });
            //ThreadPool.QueueUserWorkItem(async delegate (object state) {await StartSimInCard(); });
          
        }

        public void StopSim()
        {
            _order.OrderRole = OrderRole.Stoped;

            lock (myLock)
            {
                cts.Cancel();
                _threadStart = false;
            }
        }

        public void SimDone()
        {
            _order.OrderRole = OrderRole.Archive;

            lock (myLock)
            {
                _threadStart = false;
                
            }
        }

        public SimulationOrderProduct GetSimulationOrderProduct()
        {
            return listOrderProducts[0];
        }

        public void Update(Order order)
        {
            if(_order.Name != order.Name)
            {
                _order.Name = order.Name;
            }

            if (_order.Priority != order.Priority)
            {
                _order.Priority = order.Priority;


                foreach (var product in listOrderProducts)
                {
                    product.UpdatePriority((int)_order.Priority);
                }
            }

            if (_order.OrderRole != order.OrderRole)
            {
                _order.OrderRole = order.OrderRole;
            }
        }


        public void Do()
        {
           //lock (loker1)
          // {

            try
            {
               
                    _count++;
                    _order.TFuture = 0;
                    List<float> times = new List<float>();
                    foreach (var orderProduct in listOrderProducts)
                    {
                        if (_threadStart == false)
                        {
                            break;
                        }


                        orderProduct.UpdateTime(_count);
                        var time = orderProduct.getTFuture();
                        times.Add(time);
                        //Console.WriteLine("Time = " + time);
                    }

                    using (var _context = new Context(DBConnect.options))
                    {
                        var obj = _context.Order.Where(o => o.Id == _order.Id).FirstOrDefault();

                        //_context.Order.Attach(_order);
                        _order.TActual = _count;
                        //Прогнозируем
                        _order.TFuture = times.Max();
                        //Console.WriteLine("Max Time = " + times.Max());

                        obj.TActual = _order.TActual;
                        //Прогнозируем
                        obj.TFuture = _order.TFuture;

                         _context.SaveChanges();
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exeption " + e.ToString());
            }

        }



        

        private async Task StartSimInCard()
        {
            List<Task> tasks = new List<Task>();

            cts = new CancellationTokenSource();

            foreach (var card in simulationCardVSMs)
            {
                tasks.Add(Task.Run(async () => await card.StartSim(cts.Token), cts.Token));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("Все карты завершины");

            if (!cts.IsCancellationRequested)
            {
                SimDone();
                return;
            }
            //StopSim();
        }


        /*  private void StartSimInCard()
          {
              List<Task> tasks = new List<Task>();

              cts = new CancellationTokenSource();

              foreach (var card in simulationCardVSMs)
              {
                  tasks.Add(Task.Run(() =>card.StartSim(cts.Token), cts.Token));
              }

              await Task.WhenAll(tasks);
              Console.WriteLine("Все карты завершины");

              if(!cts.IsCancellationRequested)
              {
                  SimDone();
                  return;
              }
             //StopSim();
          } */



        public async Task RepeatActionEvery(Action action,
          TimeSpan interval, CancellationToken cancellationToken)
        {
          

            using (var _context = new Context(DBConnect.options))
            {
                _order.Simulation = true;
                _order.OrderRole = OrderRole.Work;
                _order.TStart = DateTime.UtcNow;

                var obj = _context.Order.Where(o => o.Id == _order.Id).FirstOrDefault();
                obj.Simulation = _order.Simulation;
                obj.OrderRole =  _order.OrderRole;
                obj.TStart =  _order.TStart;
               

                await _context.SaveChangesAsync();
            }

            foreach (var orderProduct in listOrderProducts)
            {
                _ = orderProduct.StartSim();
            }

            _ = StartSimInCard();

            //Подумать но можно не запусткать таймер пока карты не создадутся

            _threadStart = true;

            while (true)
            {
                if(_threadStart == false)
                {
                    break;
                }
                action();
                Task task = Task.Delay(interval); //cancellationToken

               
                try
                {
                    await task;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }

            _threadStart = true;
            _count = 0;

            if ( _order.OrderRole != OrderRole.Archive)
            {
                foreach (var orderProduct in listOrderProducts)
                {
                    orderProduct.StopSim();
                }

                using (var _context = new Context(DBConnect.options))
                {
                    var obj = _context.Order.Where(o => o.Id == _order.Id).FirstOrDefault();

                    _order.TActual = _count;
                    _order.Simulation = false;
                    _order.OrderRole = OrderRole.Stoped;
                    _order.TStop = DateTime.UtcNow;


                    obj.TActual = _order.TActual;
                    obj.Simulation = _order.Simulation;
                    obj.OrderRole = _order.OrderRole;
                    obj.TStop = _order.TStop;


                    await _context.SaveChangesAsync();
                }
            }
            else
            {
               
                using (var _context = new Context(DBConnect.options))
                {
                    var obj = _context.Order.Where(o => o.Id == _order.Id).FirstOrDefault();

                    _order.TActual = _count;
                    _order.Simulation = false;
                    _order.OrderRole = OrderRole.Archive;
                    _order.TStop = DateTime.UtcNow;


                    obj.TActual = _order.TActual;
                    obj.Simulation = _order.Simulation;
                    obj.OrderRole = _order.OrderRole;
                    obj.TStop = _order.TStop;


                    await _context.SaveChangesAsync();
                }
            }

            Console.WriteLine("_order.OrderRole = " + _order.OrderRole);

        }

        public int AddCard(Productions pr,SimulationCardVSM simulationCardVSM)
        {
            var obj = listOrderProducts.Find(o => o.getOrderProduct().Production.Id == pr.Id);
            var TPlan = obj.SetCard(simulationCardVSM);

            simulationCardVSMs.Add(simulationCardVSM);

            return TPlan;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /*internal void setPlan()
        {
           DateTime timePlan;
           float timePlan2 = 0;

           foreach(var product in listOrderProducts)
           {
                product.setPlan();
           }
        }*/
    }
}
