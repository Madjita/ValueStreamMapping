using System;
using System.Threading;
using DiplomReactNetCore.DAL.Context;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using DiplomReactNetCore.L.Simulation.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiplomReactNetCore.L.Simulation
{


    public class SimulationBufferVSM : ISimulation
    {

        private Thread threadWorker;
        private ManualResetEvent mreWorker;
        private bool _startThreadWorker;
        private bool _exitThreadWorker;

        public BufferVSM _buf;
        public Queue<SimulationBufferQueueVSM> _queueB;
        public Queue<Order> _queue;
        public SimulationBufferVSM(BufferVSM buf, Queue<SimulationBufferQueueVSM> queue, string connection)
        {
            _connection = connection;
            _buf = buf;
            _queueB = queue;

            Console.WriteLine("START SimulationBufferVSM = "+ _buf.Name);
            StartReplenishment();
        }

        public void StartReplenishment()
        {
          if (_buf == null)
          {
              return;
          }
          mre = new ManualResetEvent(false);
          thread = new Thread(WorkerReplenishment);
          thread.Start();
          locker = new object();
          _start = true;
          mre.Set();
        }

        public void StartThreadTimer()
        {
            mreWorker = new ManualResetEvent(false);
            threadWorker = new Thread(WorkerReplenishment);
        }


        private void UpdateReplenishment()
        {
            int timeUpdateMSec = _buf.ReplenishmentSec * 1000;

            Thread.Sleep(timeUpdateMSec);

            using (MyContext _context = getContext())
            {
                var obj = _context.BufferVSM.Find(_buf.Id);
                if(obj.Max <= obj.Value)
                {
                    return;
                }

                _buf.Value += _buf.ReplenishmentCount;
                obj.Value = _buf.Value;
                _context.SaveChanges();
            }
        }


        public virtual void WorkerReplenishment()
        {
            while (true)
            {
                mre.WaitOne(); // Флаг ждет запуска потока
                if (_exit) { Console.WriteLine("Exit Thread SimulationBufferVSM"); return; } // Exit

                Console.WriteLine("WorkerReplenishment new Thread SimulationBufferVSM");
                while (_start)
                {
                    if (_exit) { Console.WriteLine("Exit Thread"); return; } // Exit

                    lock (locker) 
                    {
                        UpdateReplenishment();
                    }
                }
                _start = false;
                mre.Reset();
            }
        }

        private void Worker()
        {
            while (true)
            {
                mreWorker.WaitOne(); // Флаг ждет запуска потока
                if (!_exitThreadWorker) { Console.WriteLine("Exit Thread SimulationBufferVSM: Worker"); return; } // Exit

                Console.WriteLine("Worker new Thread SimulationBufferVSM");
                int timer_sec = 0;

                while (_startThreadWorker)
                {
                    if (_exitThreadWorker) { Console.WriteLine("Exit Thread"); return; } // Exit

                    lock (locker)
                    {
                        StartTimer(timer_sec);
                    }
                }
                _startThreadWorker = false;
                mre.Reset();
            }
        }

        private void StartTimer(int timer_sec)
        {
            Thread.Sleep(1000);
            timer_sec++;
            _queueB.Peek().UpdateTime(timer_sec);
        }


        public JObject ToJson()
        {

            JObject buf = new JObject(
                new JProperty("name", _buf.Name),
                new JProperty("value", _buf.Value),
                new JProperty("max", _buf.Max),
                new JProperty("minHold", _buf.MinHold),
                new JProperty("parallel", _buf.Parallel)
            );

            return buf;
        }


        async public Task Work(SimulationOrder order)
        {

            //Проверить есть ли Нужные детали для прохождения в этап
            //order._order.Quantity
            bool flag = true;
            order.SetWaitBuffer(flag); //добавили флаг что нужно ждать

            int need_value = 0;
            var loker2 = new object();
            while (flag)
            {
                Thread.Sleep(1);

                lock (loker2)
                {
                    need_value = _buf.ValueDefault * order._order.Quantity;
                    if (_buf.Value < need_value)
                    {
                        //плохо нужно ждать когда появится нужное количество
                    }
                    else
                    {
                        //хорошо мы можем начать производить данный заказ
                        flag = false;
                    }
                }
            }
            order.SetWaitBuffer(flag); //добавили флаг что ждать не нужно

            using (MyContext _context = getContext())
            {
                var obj = _context.BufferVSM.Find(_buf.Id);
                _buf.Value -= need_value;
                obj.Value = _buf.Value;
                _context.SaveChanges();
            }


            /* var obj = new SimulationBufferQueueVSM(_connection);
            obj.Add(order._order, _buf.Id);
            _queueB.Enqueue(obj);

            return order._order;
            */
        }
    }
}
