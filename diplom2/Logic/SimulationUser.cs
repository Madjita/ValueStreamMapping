using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using diplom2.Data;
using auntification.Models;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;


namespace diplom2.Logic
{
    public class SimulationUser
    {


        ArchiveSection archiveItem;
        float TimeCircle;
        float TC;
        /// <summary>
        /// 
        /// </summary>

        User _user;
        EtapVSM _currentEtap;
        BufferVSM _currentBuf;
        EtapSections _etapSections;

        SimulationOrderProductItem _currentOrderItem;


        public object loker;


        List<SimulationSection> _sections;
        int position;

        bool UserIsBusy;


        public SimulationUser(User user)
        {
            _user = user;
            _sections = new List<SimulationSection>();
            position = 0;
            UserIsBusy = false;
            loker = new object();



        }

        public bool IsUserBusy()
        {
            return UserIsBusy;
        }

        public void AddSections(SimulationSection item)
        {
            _sections.Add(item);
        }

        public User Get()
        {
            return _user;
        }


        public Task CreateTask(List<Task> tasks, EtapSections etapSections, ManualResetEvent worker, CancellationToken cancellationToken)
        {
            UserIsBusy = true;

            if (position >= _sections.Count)
            {
                position = 0;
            }

            if (_sections[position]._simulationBufferVSM.queueOrdersItems.Count <= 0)
            {
                position++;
                if (position >= _sections.Count)
                {
                    position = 0;
                }

                UserIsBusy = false;
                return null;
            }



            //Написать код который бы понимал какой заказ взять по наличию в буфере деталий

            _currentOrderItem = _sections[position]._simulationBufferVSM.GetMax();
            _etapSections = etapSections;
            _currentBuf = _sections[position]._simulationBufferVSM.GetBuffer();


            _sections[position]._simulationBufferVSM.Decriment();

            _currentEtap = _sections[position]._simulationEtap.GetEtap();

            // _connectionString = _sections[position]._simulationEtapVSM.GetContextOprion();

            position++;

            if (position >= _sections.Count)
            {
                position = 0;
            }

            Task task = Task.Run(async () => await Work(_currentOrderItem, worker, cancellationToken), cancellationToken);
            tasks.Add(task);

            return task;
        }

        async public Task Work(SimulationOrderProductItem orderItem, ManualResetEvent worker, CancellationToken cancellationToken)
        {

           

            Orders_production_items orderItemDB = orderItem.getOrderProductItem();


            //
            //_sections[position]._simulationBufferVSM.TPop(orderItemDB);
            //

            try
            {
                //Console.WriteLine("Запуск работы Юзера = " + _user.Name);
                //Console.WriteLine("На этапе = " + _currentEtap.Name);

                int orderItem_id = orderItemDB.Id;

                // Console.WriteLine("orderItemDB = " + orderItemDB.OrderCurrentSection.Count());

                // OrderCurrentSection currentDBSection = orderItemDB.OrderCurrentSection.Where(i => i.ActualBufferVSMId == _currentBuf.Id).FirstOrDefault();
                // Console.WriteLine("currentDBSection = " + currentDBSection.Id);


                //Console.WriteLine("Заказ : " + orderItemDB.Orders_production.Production.Name + "(" + orderItemDB.Part.ToString() + " / " + orderItemDB.Orders_production.Quantity.ToString() + ") Выполняется Этапом: " + _currentEtap.Name);
                //Console.WriteLine("Users: " + _user.Name);
                //Ждать выполнение

                TimeCircle = 0;
                archiveItem = new ArchiveSection();
                // archiveItem.Orders_production_itemsId = orderItemDB.Id;





                /*for (int i=0; i < orderItemDB.OrderCurrentSection.Count;i++)
                {
                    if (orderItemDB.OrderCurrentSection[i].ActualBufferVSMId == _currentBuf.Id)
                    {
                        Console.WriteLine("БУФЕР = " + _currentBuf.Name + " = " + _currentBuf.Id);
                        Console.WriteLine("item = " + orderItemDB.OrderCurrentSection[i].ActualBufferVSMId);
                        currentDBSection = orderItemDB.OrderCurrentSection[i];
                        break;
                    }
                }*/




                using (var _context = new Context(DBConnect.options))
                {


                    // var inCurrent =  _context.OrderCurrentSection.Where(i=> i.Id == currentDBSection.Id).FirstOrDefault();

                    /* currentDBSection.ActualEtapVSMId = _currentEtap.Id;
                     currentDBSection.ActualEtapSectionsId = _etapSections.Id;
                     currentDBSection.ActualBufferVSMId = null;
                     currentDBSection.OrderSectionState = OrderSectionState.Work;


                      _context.OrderCurrentSection.Attach(currentDBSection);
                      _context.Entry(currentDBSection).Property(o => o.ActualEtapVSMId).IsModified = true;
                      _context.Entry(currentDBSection).Property(o => o.ActualEtapSectionsId).IsModified = true;
                      _context.Entry(currentDBSection).Property(o => o.ActualBufferVSMId).IsModified = true;
                      _context.Entry(currentDBSection).Property(o => o.OrderSectionState).IsModified = true;
                      _context.SaveChanges();*/

                    // orderItemDB.OrderCurrentSection[currentDBSection].ActualEtapVSMId = _currentEtap.Id;
                    //  orderItemDB.OrderCurrentSection[currentDBSection].ActualEtapSectionsId = _etapSections.Id;
                    //  orderItemDB.OrderCurrentSection[currentDBSection].ActualBufferVSMId = null;
                    //  orderItemDB.OrderCurrentSection[currentDBSection].OrderSectionState = OrderSectionState.Work;


                    //currentSection.ActualEtapVSMId = _currentEtap.Id;
                    //currentSection.ActualEtapSectionsId = _etapSections.Id;
                    //currentSection.ActualBufferVSMId = null;
                    //currentSection.OrderSectionState = OrderSectionState.Work;


                   

                    _context.EtapSections.Attach(_etapSections);
                    _context.Orders_production_items.Attach(orderItemDB);

                    //orderItemDB.ActualEtapVSMId = _currentEtap.Id;
                    //orderItemDB.ActualEtapSectionsId = _etapSections.Id;
                    //orderItemDB.ActualBufferVSMId = null;

                    //_context.Entry(orderItemDB).Property(o => o.ActualEtapVSMId).IsModified = true;
                    //_context.Entry(orderItemDB).Property(o => o.ActualEtapSectionsId).IsModified = true;
                    //_context.Entry(orderItemDB).Property(o => o.ActualBufferVSMId).IsModified = true;

                    //Random rand = new Random();
                    //var t = rand.Next(5, 40);
                    var t = (int)_currentEtap.DefaultTimeCircle;

                    TimeCircle = t;// (float)_currentEtap.DefaultTimeCircle; //можно добавить разное время для этого запросить у базы актуальное время

                    var ts = TimeSpan.FromSeconds(TimeCircle);

                    _etapSections.TActual = ts.Seconds;
                    _etapSections.Orders_production_itemsId = orderItem_id;
                    _etapSections.Work = true;
                    _etapSections.ArchiveSection.Add(archiveItem);


                    var findBuf = await _context.BufferVSM.Where(o => o.Id == _currentBuf.Id).FirstOrDefaultAsync();

                    findBuf.Value = (int?)(findBuf.Value - _currentEtap.DefaultAvailability);

                    var findUser = await _context.Users.Where(o => o.Id == _user.Id).FirstOrDefaultAsync();

                    findUser.UserRole = UserRole.Busy;


                    // _context.Attach(_etapSections);

                    //  var objInBD = _context.EtapSections.Where(i => i.Id == _etapSections.Id).FirstOrDefault();  // Attach(_etapSections);
                    //      objInBD.Work = true;
                    // objInBD.Orders_production_itemsId = orderItemDB.Id;
                    //     objInBD.CurrentOrderItems = orderItemDB;
                    //     objInBD.Archive.Add(archiveItem);

                    await _context.SaveChangesAsync();

                    _context.Entry(_etapSections).State = EntityState.Detached;
                    _context.Entry(orderItemDB).State = EntityState.Detached;


                }

                TC = TimeCircle;

                using (var _context = new Context(DBConnect.options))
                {
                    while (TimeCircle > 0)
                    {
                        var ts = TimeSpan.FromSeconds(TimeCircle);
                        //Console.WriteLine("Этап : " + _currentEtap.Name + " Circle time : " + ts);



                        var dbEtap = _context.EtapVSM.Where(o => o.Id == _currentEtap.Id).FirstOrDefault();
                        dbEtap.ActualTimeCircle = (float)TC;

                        _etapSections.TActual = ts.Seconds;

                        var objInBD = _context.EtapSections.Where(i => i.Id == _etapSections.Id).Include(i => i.ArchiveSection).FirstOrDefault();
                        objInBD.TActual = _etapSections.TActual;

                        objInBD.ArchiveSection.Where(i => i.Id == archiveItem.Id).FirstOrDefault().Time = (float)TC - _etapSections.TActual;


                        //Console.WriteLine("Time " + _etapSections.TActual);
                        await _context.SaveChangesAsync();


                        //  await Task.Delay(1000);
                        await Task.Run(async () => { await Task.Delay(1000); });
                        TimeCircle--;

                        cancellationToken.ThrowIfCancellationRequested();

                    }

                }


                using (var _context = new Context(DBConnect.options))
                {
                    // var inCurrent = _context.OrderCurrentSection.Where(i => i.ActualEtapVSMId == _currentEtap.Id).FirstOrDefault();

                    // inCurrent.OrderSectionState = OrderSectionState.waitingNext;
                    // inCurrent.ActualEtapSectionsId = null;
                    // inCurrent.ActualBufferVSMId = null;

                    // currentDBSection = orderItemDB.OrderCurrentSection.Where(i => i.ActualEtapVSMId == _currentEtap.Id).FirstOrDefault();
                    // currentDBSection.OrderSectionState = OrderSectionState.waitingNext;

                    // _context.OrderCurrentSection.Attach(currentDBSection);
                    // _context.Entry(currentDBSection).Property(o => o.OrderSectionState).IsModified = true;

                    _context.EtapSections.Attach(_etapSections);
                    _context.Orders_production_items.Attach(orderItemDB);

                    //orderItemDB.ActualEtapVSMId = null;
                    //orderItemDB.ActualEtapSectionsId = null;
                    //orderItemDB.ActualBufferVSMId = null;


                    // _context.Entry(orderItemDB).Property(o => o.ActualEtapVSMId).IsModified = true;
                    //_context.Entry(orderItemDB).Property(o => o.ActualEtapSectionsId).IsModified = true;
                    //_context.Entry(orderItemDB).Property(o => o.ActualBufferVSMId).IsModified = true;


                    _etapSections.TActual = TC;
                    _etapSections.CurrentOrderItems = null;
                    _etapSections.Work = false;


                    //var obj = _context.EtapSections.Where(i => i.Id == _etapSections.Id).Include(i => i.Archive).FirstOrDefault();

                    // obj.Archive.Where(i => i.Id == archiveItem.Id).FirstOrDefault().Time = _etapSections.TActual;

                    //obj.TActual = TC;
                    // obj.CurrentOrderItems = null;
                    // obj.Work = false;


                    var findUser = await _context.Users.Where(o => o.Id == _user.Id).FirstOrDefaultAsync();
                    findUser.UserRole = UserRole.Free;


                    await _context.SaveChangesAsync();

                    _context.Entry(_etapSections).State = EntityState.Detached;
                    _context.Entry(orderItemDB).State = EntityState.Detached;
                }

                UserIsBusy = false;
                worker.Set(); //Отпускаем поток
            }
            catch (OperationCanceledException)
            {
                //Console.WriteLine("Отпускаем поток после Ексепшона");

                using (var _context = new Context(DBConnect.options))
                {
                    _context.EtapSections.Attach(_etapSections);
                    _context.Orders_production_items.Attach(orderItemDB);

                    _etapSections.TActual = TC;
                    _etapSections.CurrentOrderItems = null;
                    _etapSections.Work = false;

                    orderItemDB.Simulation = false;
                    orderItemDB.TStop = DateTime.Now;

                    var findUser = await _context.Users.Where(o => o.Id == _user.Id).FirstOrDefaultAsync();
                    findUser.UserRole = UserRole.Free;

                    await _context.SaveChangesAsync();

                    _context.Entry(_etapSections).State = EntityState.Detached;
                    _context.Entry(orderItemDB).State = EntityState.Detached;

                }

                UserIsBusy = false;
                worker.Set(); //Отпускаем поток
            }
            catch (Exception)
            {

            }
        }

    }
}
