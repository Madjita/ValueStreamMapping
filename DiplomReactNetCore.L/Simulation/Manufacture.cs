using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DiplomReactNetCore.DAL.Context;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using DiplomReactNetCore.L.Simulation.Interface;

namespace DiplomReactNetCore.L.Simulation
{
    public class Manufacture : ISimulation
    {

        private List<SimulationBufferVSM> _list_bufferVSM; // Лист всех буферов (для создания симуляции работы буфера)
        private List<SimulationEtapVSM> _list_EtapVSM;     // Лист всех этапов (для создания симуляции работы этапа)

        public List<SimulationCardVSM> _list_CardVSM; // Лист всех Существующих карт по продуктам
        public List<SimulationOrder> _list_Order;    // Лист заказаов которы нужно произвести

        public Manufacture(string connection)
        {
            _connection = connection;
            _list_bufferVSM = new List<SimulationBufferVSM>();
            _list_EtapVSM = new List<SimulationEtapVSM>();
            _list_CardVSM = new List<SimulationCardVSM>();
            _list_Order = new List<SimulationOrder>();

            GetAllBufferVSM();
            GetAllEtapVSM();
            CreateCardVSM();
        }

        private void GetAllBufferVSM()
        {
            using(MyContext _context = new MyContext(ConnectionBD()))
            {
                List<BufferVSM> bufferVSMs = _context.BufferVSM.Include(i=>i.QueueBufferVSMs).ToList();

                foreach (BufferVSM item in bufferVSMs)
                {
                    Queue<SimulationBufferQueueVSM> list_queue = new Queue<SimulationBufferQueueVSM>();
                    foreach (var q in item.QueueBufferVSMs)
                    {
                        list_queue.Enqueue(new SimulationBufferQueueVSM(q, _connection));
                    }
                    _list_bufferVSM.Add(new SimulationBufferVSM(item, list_queue, _connection));
                }
            }
        }

        private void GetAllEtapVSM()
        {
            using (MyContext _context = new MyContext(ConnectionBD()))
            {
                List<EtapVSM> etapVSMs = _context.EtapVSM.ToList();

                foreach (EtapVSM item in etapVSMs)
                {
                    _list_EtapVSM.Add(new SimulationEtapVSM(item, _connection));
                }
            }
        }

        private void CreateCardVSM()
        {
            using (MyContext _context = new MyContext(ConnectionBD()))
            {
                List<Production> productions = _context.Production.ToList();
                foreach(Production item in productions)
                {

                    _list_CardVSM.Add(new SimulationCardVSM(_connection));
                    _list_CardVSM.Last().CreateCard(
                        item.Name,
                        _list_bufferVSM,
                        _list_EtapVSM
                    );
                }
               
            }
        }

        public void AddOrder(Order obj)
        {
            _list_Order.Add(new SimulationOrder(_connection,obj));
        }

        public JArray ToJson()
        {
            JArray cards = new JArray();

            foreach (SimulationCardVSM item in _list_CardVSM)
            {
                cards.Add(item.ToJson());
            }

            return cards;
        }


        public void StartWork(Order obj)
        {
           var item = _list_Order.Where(i => i._order.Id == obj.Id).SingleOrDefault();

           if(obj.Simulation)
           {
                string name = item._order.Production.Name;

                SimulationCardVSM simulationCardVSM = null;

                //Находим к какой карте относится заказ
                foreach(var card in _list_CardVSM)
                {
                    if(card._NameProduction == name)
                    {
                        simulationCardVSM = card;
                        break;
                    }
                }
                //Добавляем данный заказ в очередь для работы в найденную карту 
                simulationCardVSM.AddOrder(item);
                item.Start();
           }
           else
           {
                item.Stop();
           }
            
        }


        ///Функция реализации бизнес логики работы производства
        ///
        public virtual void Worker()
        {
            while (true)
            {
                mre.WaitOne(); // Флаг ждет запуска потока
                if (_exit) { Console.WriteLine("Exit Thread SimulationBufferVSM"); return; } // Exit

                Console.WriteLine("Worker new Thread SimulationBufferVSM");
                while (_start)
                {
                    if (_exit) { Console.WriteLine("Exit Thread"); return; } // Exit

                    lock (locker)
                    {
                     
                    }
                }
                _start = false;
                mre.Reset();
            }
        }


       public JArray getAllOrdersFromEtap(int id)
       {
            SimulationEtapVSM etap = null;

            foreach(var item in _list_EtapVSM)
            {
                if(item._etap.Id == id)
                {
                    etap = item;
                    break;
                }

            }
            if(etap == null)
            {
                return null;
            }
            var obj = etap.GetQueueOrders();
            return obj;
        }

    }
}
