using System;
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
    public class SimulationCardVSM : ISimulation
    {
        public string _NameProduction { get; set; }
        private int Id { get; set; }

        public List<List<SimulationSection>> Sections { get; set; }
        public Queue<SimulationOrder> queueOrders { get; set; }



        public SimulationCardVSM(string connection)
        {
            _connection = connection;
            queueOrders = new Queue<SimulationOrder>();
            Sections = new List<List<SimulationSection>>();
            Sections.Add(new List<SimulationSection>());
            
        }

        public bool CreateCard(string NameProduction, List<SimulationBufferVSM> list_BufferVSM, List<SimulationEtapVSM> list_EtapVSM)
        {
            _NameProduction = NameProduction;

            using (MyContext _context = getContext())
            {
                var Production = _context.Production
                    .Where(i => i.Name == NameProduction)
                    .ToList()
                    .First();

                if (Production == null)
                {
                    Id = 0;
                    return false;
                }
                else
                {
                    Id = Production.Id;
                }

                List<CardVSM> card = _context.CardVSM
                    .Where(i => i.ProductionId == Id)
                    .OrderBy(i => i.EtapNumeric)
                    .Include(i => i.EtapVSM)
                    .Include(i => i.BufferVSM)
                    .ToList();


                int CountEtaps = 1;
                foreach (CardVSM itemEtap in card)
                {
                    if (itemEtap.EtapNumeric != CountEtaps)
                    {
                        CountEtaps++;
                        Sections.Add(new List<SimulationSection>());
                    }

                    SimulationBufferVSM buf = null;
                    SimulationEtapVSM etap = null;

                    if (itemEtap.BufferVSM != null)
                    {
                        buf = list_BufferVSM.Where(i => i._buf.Id == itemEtap.BufferVSM.Id).First();
                    }

                    if (itemEtap.EtapVSM != null)
                    {
                        etap = list_EtapVSM.Where(i => i._etap.Id == itemEtap.EtapVSM.Id).First();
                    }

                    /*
                    var buf_db = list_BufferVSM.Where(i => i._buf.Id == itemEtap.BufferVSM.Id);
                    var etap_db = list_EtapVSM.Where(i => i._etap.Id == itemEtap.EtapVSM.Id);

                  

                    if (buf_db.Count() < 0)
                    {
                        buf = null;
                    }
                    else
                    {
                        buf = buf_db.First();
                    }

                    if (etap_db.Count() < 0)
                    {
                        etap = null;
                    }
                    else
                    {
                        etap = etap_db.First();
                    }
                    */

                    //SimulationBufferVSM buf = new SimulationBufferVSM(itemBuf, _connection);
                    //SimulationEtapVSM etap = new SimulationEtapVSM(itemEtap.EtapVSM, _connection);

                    Sections[CountEtaps - 1].Add(new SimulationSection(buf, etap));
                }
            }

            return true;
        }


        public JObject ToJson()
        {
            JObject obj = new JObject();
            JArray _array_sections = new JArray();

            foreach (List<SimulationSection> sections in Sections)
            {
                JArray _array_etaps = new JArray();
                foreach (SimulationSection section in sections)
                {
                    _array_etaps.Add(section.ToJson());
                }

                _array_sections.Add(_array_etaps);
            }
            obj.Add(new JProperty("name", this._NameProduction));
            obj.Add(new JProperty("sections", _array_sections));

            return obj;
        }


        public void AddOrder(SimulationOrder order)
        {
            queueOrders.Enqueue(order);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Work));
        }

        //Функция выполнения карты по секциям с параллельными етапами.
        public async void Work(object obj)
        {
            var order = queueOrders.Dequeue();
            foreach (List<SimulationSection> section in Sections)
            {
                List<Task> tasks = new List<Task>();

                foreach (SimulationSection currentSection in section)
                {
                    Console.WriteLine("Запускаем асинхроннную функцию: " + currentSection._SimulationEtapVSM._etap.Name);
                    Console.WriteLine("Дискриптион: " + currentSection._SimulationEtapVSM._etap.Description);
                    currentSection.Work(order, tasks); //запускаем асинхронную функцию
                }
                Console.WriteLine("Ждем Выполнение секций");
                await Task.WhenAll(tasks);
                Console.WriteLine("Идем на следующую секцию");
            }

            //Перенести этот код в Завершенные заказы.
            Console.WriteLine("Заказ готов : "+ order._order.Production.Name +" ("+order._order.Quantity+")");
            order.Stop();
        }
    }
}
