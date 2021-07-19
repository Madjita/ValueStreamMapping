using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DiplomReactNetCore.DAL.Context;
using DiplomReactNetCore.DAL.Models.DataBase;
using DiplomReactNetCore.L.Simulation.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace DiplomReactNetCore.L.Simulation
{

    public struct queueObject
    {
        public SimulationBufferVSM buf { get; set; }
        public SimulationOrder order { get; set; }
        public ManualResetEvent worker { get; set; }
    }


    public class SimulationEtapVSM : ISimulation
    {
        public Queue<queueObject> _queue;
        public JArray _array_orders;

        public EtapVSM _etap;
        public SimulationEtapVSM(EtapVSM etap,string connection)
        {
            _connection = connection;
            _etap = etap;
            StartThread();
        }

        public void UpdateQueueOrders()
        {
            var lok = new object();

            lock (lok)
            { 
                using (IEnumerator<queueObject> en = _queue.GetEnumerator())
                {
                    _array_orders = new JArray();
                    while (en.MoveNext())
                    {
                        _array_orders.Add(en.Current.order.ToJson());
                    }
                }
            }
        }

        public JArray GetQueueOrders()
        {
            UpdateQueueOrders();
            return _array_orders;
        }

        public JObject ToJson()
        {

            JObject etap = new JObject(
                new JProperty("id", _etap.Id),
                new JProperty("name", _etap.Name),
                new JProperty("actualTimeCircle", _etap.ActualTimeCircle),
                new JProperty("defaultTimeCircle", _etap.DefaultTimeCircle),
                new JProperty("actualTimePreporation", _etap.ActualTimePreporation),
                new JProperty("defaultTimePreporation", _etap.DefaultTimePreporation),
                new JProperty("actualAvailability", _etap.ActualAvailability),
                new JProperty("time", _etap.Time),
                new JProperty("parallel", _etap.Parallel)
            );

            return etap;
        }


        public void StartThread()
        {
            locker = new object();
            _queue = new Queue<queueObject>();
            mre = new ManualResetEvent(false);
            thread = new Thread(Worker);
            thread.Start();
            _start = true;
            _exit = false;
        }

        async private void DoSomething()
        {
            var object_queue = _queue.Peek();

            Console.WriteLine("Ждем есть ли ингридиенты в буфере");
            if(object_queue.buf != null)
                await object_queue.buf.Work(object_queue.order); //Проверить может ли данный буфер попасть в очедь и ждать пока он не попадет.



            var order = object_queue.order;
            Console.WriteLine("Заказ : " + order._order.Production.Name + "(" + order._order.Quantity.ToString() + ") Выполняется Этапом: " + _etap.Name);
            Console.WriteLine("Description: " + _etap.Description);
            //Ждать выполнение

            var TimeCircle = _etap.DefaultTimeCircle;

            while(TimeCircle != 0)
            {
                var ts = TimeSpan.FromSeconds(TimeCircle);
                Console.WriteLine("Этап : " + _etap.Name + " Circle time : " + ts);

                using(var _context = getContext())
                {
                    var obj = _context.EtapVSM.Find(_etap.Id);
                    obj.Order = order._order;
                    obj.ActualTimeCircle = (int)ts.TotalMilliseconds;
                    _context.SaveChanges();
                }

                Thread.Sleep(1000);
                TimeCircle--;
            }

            _queue.Dequeue();
            //Освободить поток
            object_queue.worker.Set();

            using (var _context = getContext())
            {
                var obj = _context.EtapVSM.Where(i => i.Id == _etap.Id).Include(i => i.Order).ToList().First();// Find(_etap.Id);//.Include(i=>i.Order);
                obj.Order = null;
                obj.ActualTimeCircle = 0;
                _context.SaveChanges();
            }
        }


        public virtual void Worker()
        {
            while (true)
            {
                mre.WaitOne(); // Флаг ждет запуска потока
                if (_exit) { Console.WriteLine("Exit Thread SimulationEtapVSM"); return; } // Exit

                Console.WriteLine("Worker new Thread SimulationEtapVSM");
                while (_start)
                {
                    if (_exit) { Console.WriteLine("Exit Thread"); return; } // Exit

                    lock (locker)
                    {
                        if(_queue.Count == 0)
                        {
                            Console.WriteLine("Этап завершился: " + _etap.Name);
                            Console.WriteLine("Description: " + _etap.Description);
                            break;
                        }
                        DoSomething();

                    }
                }
                _start = false;
                mre.Reset();
            }
        }


        async public Task Work(SimulationOrder order, SimulationBufferVSM buf)
        {
            //Console.WriteLine("Заказ : "+ order._order.Production.Name + "("+ order._order.Quantity.ToString()+ ") Ждет Выполнение Этапом: " + _etap.Name);
            //Console.WriteLine("Description: " + _etap.Description);

            var object_queue = new queueObject
            {
                buf = buf,
                order = order,
                worker = new ManualResetEvent(false)
            };

            _queue.Enqueue(object_queue);

            if(IsPaused)
            {
                mre.Set();
                setStart(true);
            }

            object_queue.worker.WaitOne(); //Ждем когда другой поток разрешит продолжить выполнение
            //Console.WriteLine("Заказ : " + order._order.Production.Name + "(" + order._order.Quantity.ToString() + ") Выполнился Этапом: " + _etap.Name);
            //Console.WriteLine("Description: " + _etap.Description);
            // Проверять запущен ли поток ?

        }
    }
}
