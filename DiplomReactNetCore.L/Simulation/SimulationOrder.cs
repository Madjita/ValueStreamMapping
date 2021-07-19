using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using DiplomReactNetCore.DAL.Models.DataBase;
using DiplomReactNetCore.DAL.Models.Work;
using DiplomReactNetCore.L.Simulation.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace DiplomReactNetCore.L.Simulation
{
    public class SimulationOrder : ISimulation
    {
        public Order _order;
        DateTime _start_time;

        private System.Timers.Timer _timer = new System.Timers.Timer();
        private volatile bool _requestStop = true;

        //
        JObject _jobject_order;
        JArray _jobject_waits;

        public SimulationOrder(string connection, Order order)
        {
            _connection = connection;
            _order = order;
            resetEvents = new List<ManualResetEvent>();

            SetTimeAdd();

            _timer.Interval = 1000;
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = false;

            JSonInit();
        }

        private void JSonInit()
        {
            var Production = new JObject(
               new JProperty("name", _order.Production.Name)
               );
            _jobject_order = new JObject(
                new JProperty("id", _order.Id),
                new JProperty("quantity", _order.Quantity),
                new JProperty("timeActual", (_order.TimeActual - _order.TimeStart).ToString("h'h 'm'm 's's'")),
                new JProperty("production", Production),
                new JProperty("wait", false)
            );
        }

        public void SetWaitBuffer(bool wait)
        {
            _jobject_order["wait"] = wait;
        }

        public JObject ToJson()
        {
            _jobject_order["id"] = _order.Id;
            _jobject_order["quantity"] = _order.Quantity;
            _jobject_order["timeActual"] = (_order.TimeActual - _order.TimeStart).ToString("h'h 'm'm 's's'");
            _jobject_order["production"]["name"] = _order.Production.Name;

            return _jobject_order;
        }


        private void SetTimeAdd()
        {
            using(var _context = getContext())
            {
                _order = _context.Order.Where(i=>i.Id == _order.Id).Include(i => i.Production).ToList().First();
                _order.TimeAdd = DateTime.Now;
                _context.SaveChanges();
            }
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan newTime = DateTime.Now - _start_time;
            //Console.WriteLine("_order.TimeActual = " + newTime);

            using (var _context = getContext())
            {
                _order = _context.Order.Where(i => i.Id == _order.Id).Include(i => i.Production).ToList().First();
                _order.TimeActual = _order.TimeStart + newTime;
                _context.SaveChanges();
            }


            if (!_requestStop)
             {
                 _timer.Start();//restart the timer
             }
        }

        public void Stop()
        {
            _requestStop = true;
            _timer.Stop();

            using (var _context = getContext())
            {
                var obj = _context.OrderListForWork.Where(i=> i.OrderId == _order.Id).SingleOrDefault();
                _context.OrderListForWork.Remove(obj);

                _order = _context.Order.Where(i => i.Id == _order.Id).Include(i => i.Production).ToList().First();
                _order.TimeStop = _order.TimeActual;
                _order.Simulation = false;

                //Добавить в лист сделанных

                _context.OrderListFinishedWork.Add(new OrderListFinishedWork { OrderId = _order.Id });

                _context.SaveChanges();
            }
        }

        public void Start()
        {
            _start_time = DateTime.Now;

            using (var _context = getContext())
            {
                _order = _context.Order.Where(i => i.Id == _order.Id).Include(i => i.Production).ToList().First();
                _order.TimeStart = _start_time;
                _order.TimeActual = _start_time;
                _order.Simulation = true;
                _context.SaveChanges();
            }
            _requestStop = false;
            _timer.Start();
        }


    }

}
