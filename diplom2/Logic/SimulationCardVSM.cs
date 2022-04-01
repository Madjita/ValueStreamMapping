using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace diplom2.Logic
{
    public class SimulationCardVSM : ICloneable
    {


        /// <summary>
        SimulationOrderProduct _orderProduction;
        /// </summary>

        public bool simulation;

        public Productions _product;

        public List<CardVSM> _card; //Карта с этапами и буферами
        public List<List<SimulationSection>> sections; // Секции в карте

        public List<SimulationOrder> simulationOrder; // лист из заказов по партиям

        //public List<SimulationOrderItems> simulationOrderItems; // лист заказов 

        public Heap<SimulationOrderProduct> queueOrders; //Очередь с приоритетом заказов
        public Heap<SimulationOrderProductItem> queueOrdersItems; // Очередь с приоритетом по 1 заказу

        public List<SimulationOrderItems> Result_OrderItems; //Результат выполненных

        //
        private List<SimulationBufferVSM> _all_bufferVSM; // Лист всех буферов (для создания симуляции работы буфера)
        private List<SimulationEtapVSM> _all_EtapVSM;     // Лист всех этапов (для создания симуляции работы этапа)
        //

        public object lockerCard = new object();


        ///
        Thread thread;

        public SimulationCardVSM(Productions item, List<SimulationBufferVSM> all_bufferVSM, List<SimulationEtapVSM> all_EtapVSM)
        {
            _product = item;

            simulation = false;

            Comparer<SimulationOrderProduct> comparer = Comparer<SimulationOrderProduct>.Default;
            queueOrders = new Heap<SimulationOrderProduct>(comparer);

            Comparer<SimulationOrderProductItem> comparer2 = Comparer<SimulationOrderProductItem>.Default;
            queueOrdersItems = new Heap<SimulationOrderProductItem>(comparer2);

            sections = new List<List<SimulationSection>>();

            Result_OrderItems = new List<SimulationOrderItems>();

            simulationOrder = new List<SimulationOrder>();
            //simulationOrderItems = new List<SimulationOrderItems>();

            _all_bufferVSM = all_bufferVSM;
            _all_EtapVSM = all_EtapVSM;

            CreateCard();
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }


        public void CreateCard()
        {
            try
            {
                using (var _context = new Context(DBConnect.options))
                {
                    _card = _context.CardVSM
                        .Where(i => i.ProductionsId == _product.Id)
                        .Include(i => i.BufferVSM)
                        .Include(i => i.EtapVSM)
                        .ThenInclude(i=> i.EtapSections)
                        .ThenInclude(i=> i.User)
                        .ToList();
                }

                int CountEtaps = 1;
                sections.Add(new List<SimulationSection>());
                foreach (CardVSM item in _card)
                {
                    if (item.EtapNumeric != CountEtaps)
                    {
                        CountEtaps++;
                        sections.Add(new List<SimulationSection>());
                    }

                    var buf = _all_bufferVSM.Where(i => i.GetBuffer().Id == item.BufferVSM.Id).FirstOrDefault();
                    var etap = _all_EtapVSM.Where(i => i.GetEtap().Id == item.Id).FirstOrDefault();
                    var section = new SimulationSection((int)item.EtapNumeric, buf, etap);

                    etap.SetCardId(item.Id);
                    etap.AddSectionInUsers(section);


                    sections.Last().Add(section);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }


        public void setSimOrderProduction(SimulationOrderProduct simulationOrderProduct)
        {
            if(_orderProduction != null)
            {

            }
            _orderProduction = simulationOrderProduct;
        }

        public int TWork(Orders_production _orders_production)
        {
            int _twork = 0;

            foreach(var sectionList in sections)
            {
                if (sectionList.Count > 0)
                {
                    _twork += sectionList.Max(o => o.getWorkTime(_orders_production));
                }
                //Console.WriteLine(" lol = " + sectionList.Max(o => o.getWorkTime()));
            }

            return _twork; 
        }


        public async Task StartSim(CancellationToken cancellationToken)
        {
            Console.WriteLine("Start");
            var list = _orderProduction.getOrderProductItems();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

             
                await _orderProduction.StartSim();

                // await Task.Run(async () => await Work(_orderProduction, cancellationToken), cancellationToken); // изменил на асинхронный

                await Work(_orderProduction, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("STOP Exception");

                foreach (var item in list)
                {
                    queueOrdersItems.Delete(item);
                }


                //
                foreach (List<SimulationSection> section in sections)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (SimulationSection currentSection in section)
                    {
                        currentSection.DeleteOrder(list);
                    }
                }
                //
                return;
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception "+ e.ToString());
            }
        }


        //
        public static SyncingTable syncing = new SyncingTable();
        //

        //Функция выполнения карты по секциям с параллельными етапами.
        public async Task Work(SimulationOrderProduct orderProduction, CancellationToken cancellationToken)
        {
            try
            {
                List<Task> tasks = new List<Task>();

                Console.WriteLine("Ждем Выполнения заказа");

                var orderItemsList = orderProduction.getOrderProductItems();

                //создаем объект
                // Stopwatch stopwatch = new Stopwatch();
                //засекаем время начала операции
                //stopwatch.Start();
                //выполняем какую-либо операцию


                int pos = 0;

                foreach (var item in orderItemsList)
                {
                    pos++;
                    Console.WriteLine("Create = " + pos.ToString());
                    cancellationToken.ThrowIfCancellationRequested();


                    tasks.Add(Task.Run(async () => await WorkItemsInOrder(item, cancellationToken), cancellationToken));

                    /* tasks.Add(new Thread(async delegate (object state)
                     {

                         await WorkItemsInOrder(item, cancellationToken);

                     }
                     )); //изменил на асинхронный*/
                }
                pos = 0;

               /* foreach (var task in tasks)
                {
                    task.Start();
                }


                foreach (var task in tasks)
                {
                    await task.Join();
                }*/

                // await WorkVSMCard(orderItemsList, cancellationToken);



                //останавливаем счётчик
                // stopwatch.Stop();
                //смотрим сколько миллисекунд было затрачено на выполнение
                // Console.WriteLine("CARD StartSim : " + stopwatch.ElapsedMilliseconds);
                //  TimeSpan ts = stopwatch.Elapsed;

                // string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                //  ts.Hours, ts.Minutes, ts.Seconds,
                //  ts.Milliseconds / 10);
                //   Console.WriteLine(elapsedTime);
                //   Console.WriteLine("===================================");

                //WaitHandle.WaitAll(events.ToArray());

                await Task.WhenAll(tasks);

                tasks.Clear();

                cancellationToken.ThrowIfCancellationRequested();


                var orderDB = orderProduction.getOrderProduct();

                //Перенести этот код в Завершенные заказы.
                Console.WriteLine("Заказ готов : " + orderDB.Order.Name);
                orderProduction.Done();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Останавливаем симуляцию на заказе");

                orderProduction.StopSim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка Експетион " + e.ToString());
            }
        }





        /*   async public Task WorkVSMCard(List<SimulationOrderProductItem> order, CancellationToken cancellationToken)
           {

               try
               {

                   foreach (List<SimulationSection> numericSection in sections)
                   {
                       List<Task> tasks = new List<Task>();
                       foreach (SimulationSection currentEtap in numericSection)
                       {
                           //Console.WriteLine("Запускаем асинхроннную функцию: " + currentSection._simulationEtap.GetEtap().Name);
                           //Console.WriteLine("Дискриптион: " + currentSection._simulationEtap.GetEtap().Description);
                           tasks.Add(Task.Run(() => currentEtap.WorkAsync(order, tasks, cancellationToken), cancellationToken)); //изменил на асинхронный


                           cancellationToken.ThrowIfCancellationRequested();
                       }
                       //Console.WriteLine("Ждем Выполнение секций");
                       await Task.WhenAll(tasks);
                       // tasks.Clear();
                       //Console.WriteLine("Идем на следующую секцию");

                       cancellationToken.ThrowIfCancellationRequested();

                   }

                   var orderItem = order.getOrderProductItem();
                   Console.WriteLine("Заказ готов : " + orderItem.Orders_production.Production.Name + " (" + orderItem.Part + ")");
                   order.Done();
               }
               catch (OperationCanceledException)
               {
                   var orderItem = order.getOrderProductItem();
                   Console.WriteLine("Останавливаем симуляцию на заказе : " + orderItem.Orders_production.Production.Name + " (" + orderItem.Part + ")");
                   order.StopSim();
               }
               catch (Exception e)
               {
                   Console.WriteLine("Ошибка Експетион " + e.ToString());
               }
           }*/


        async public Task WorkItemsInOrder(SimulationOrderProductItem order, CancellationToken cancellationToken)
         {

             try
             {

                 foreach (List<SimulationSection> numericSection in sections)
                 {
                     List<Task> tasks = new List<Task>();
                     foreach (SimulationSection currentEtap in numericSection)
                     {
                        //Console.WriteLine("Запускаем асинхроннную функцию: " + currentSection._simulationEtap.GetEtap().Name);
                        //Console.WriteLine("Дискриптион: " + currentSection._simulationEtap.GetEtap().Description);
                        //tasks.Add(); //изменил на асинхронный
                        await Task.Run(() => currentEtap.Work(order, tasks, cancellationToken), cancellationToken);


                        cancellationToken.ThrowIfCancellationRequested();
                     }
                     Console.WriteLine("Ждем Выполнение секций");
                     await Task.WhenAll(tasks);
                     tasks.Clear();
                     Console.WriteLine("Идем на следующую секцию");

                     cancellationToken.ThrowIfCancellationRequested();

                 }

                 var orderItem = order.getOrderProductItem();
                 Console.WriteLine("Заказ готов : " + orderItem.Orders_production.Production.Name + " (" + orderItem.Part + ")");
                 order.Done();
             }
             catch (OperationCanceledException)
             {
                 var orderItem = order.getOrderProductItem();
                 Console.WriteLine("Останавливаем симуляцию на заказе : " + orderItem.Orders_production.Production.Name + " (" + orderItem.Part + ")");
                 order.StopSim();
             }
             catch (Exception e)
             {
                 Console.WriteLine("Ошибка Експетион " + e.ToString());
             }
         }
    }
}
