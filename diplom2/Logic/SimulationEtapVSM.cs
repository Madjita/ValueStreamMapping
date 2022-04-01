using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace diplom2.Logic
{
    public class queueObject
    {
        public SimulationBufferVSM buf { get; set; }
        public SimulationOrderItems orderItem { get; set; }
        public ManualResetEvent worker { get; set; }
    }

    public class SimulationEtapVSM
    {

        public List<SimulationEtapSection> resourceCenter;

        public ManualResetEvent worker { get; set; }
        SimulationBufferVSM _buf;
        SimulationOrderProductItem _currentOrderItem;


        List<Task> tasks = new List<Task>();
        List<float> time = new List<float>();

        object locker = new object();
        object locker2 = new object();

        object locker3 = new object();

        EtapVSM _etapVSM;
        int _cardId;
        public SimulationEtapVSM(EtapVSM etapVSM, List<SimulationUser> allUsersUseInCards)
        {

            _etapVSM = etapVSM;

            resourceCenter = new List<SimulationEtapSection>();
            foreach (var item in etapVSM.EtapSections)
            {
                using (var _context = new Context(DBConnect.options))
                {
                    item.User = _context.EtapSections.Where(i => i.Id == item.Id).Include(i => i.User).FirstOrDefault().User;
                }

                var userUse = allUsersUseInCards.Where(i => i.Get().Id == item.User.Id).FirstOrDefault();

                resourceCenter.Add(new SimulationEtapSection(item, userUse));
            }


            // ThreadPool.QueueUserWorkItem(delegate (object state) { WorkEtap(); });

        }

        public int getWorkTime()
        {
            /*  if(_etapVSM.ActualTimeCircle == null)
              {
                  return (int)_etapVSM.DefaultTimeCircle;
              }*/

            //return (int)_etapVSM.ActualTimeCircle;
            return (int)_etapVSM.DefaultTimeCircle;
        }


        /*  ~SimulationEtapVSM()
          {
              task_stop = true;
              task.Wait();
          }*/


        public void SetCardId(int CardId)
        {
            _cardId = CardId;
        }

        public void AddSectionInUsers(SimulationSection section)
        {

            foreach (var item in resourceCenter)
            {
                item.Add(section);
                item.SetCardId(_cardId);
            }
        }

        public EtapVSM GetEtap()
        {
            return _etapVSM;
        }

        public async Task Work(SimulationOrderProductItem orderItem, SimulationBufferVSM buf, CancellationToken cancellationToken)
        {

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _currentOrderItem = orderItem;
                _buf = buf;

                //await _currentOrderItem.UpdateBuf(_buf.GetBuffer().Id);

                Task task = null;


                lock(locker3)
                {
                    Console.WriteLine("Хотим дабавить часть: " + orderItem.getOrderProductItem().Part);
                    _buf.Insert(orderItem);
                    Console.WriteLine("Добавили");
                }
            

              //  lock (locker2)
              //  {

                  
                    bool flag = true;
                    var worker = new ManualResetEvent(false);

                    while (flag)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        foreach (var resource in resourceCenter)
                        {

                            lock (locker)
                            {
                                bool flatUser = resource.IsWorkUser();
                                if (flatUser == false)
                                {
                                    //Нашли свободного рабочего для этой задачи
                                    //Console.WriteLine("Запускаем Юзера на этапе :" + _etapVSM.Name + " Buf = " + _buf.Count());
                                    task = resource.StartWorkUser(tasks, worker, cancellationToken);

                                    if (task == null)
                                    {
                                        continue;
                                    }
                                    flag = false;
                                    break;
                                }

                                cancellationToken.ThrowIfCancellationRequested();
                            }
                        }
                        await Task.Delay(1000);
                    }


                worker.WaitOne(); //Ждем когда другой поток разрешит продолжить выполнение


                //   }

                try
                {
                    if(tasks.Count > 0)
                    {
                        if (tasks.Any(o => o.Id == task.Id))
                        {
                            tasks.Remove(task);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                try
                {
                    //Посчитать сколько времени заняло выполнение Этапа
                    foreach (var resource in resourceCenter)
                    {
                        time.Add(resource.TACtual);
                    }

                    if(time.Count > 0)
                    {
                        _etapVSM.ActualTimeCircle = time.Max();
                    }
                    else
                    {
                        _etapVSM.ActualTimeCircle = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }



                time.Clear();

                using (var _context = new Context(DBConnect.options))
                {
                    EntityState state = _context.Entry(_etapVSM).State;

                    var obj = _context.EtapVSM.Where(o => o.Id == _etapVSM.Id).FirstOrDefault();

                    obj.ActualTimeCircle = _etapVSM.ActualTimeCircle;


                    await _context.SaveChangesAsync();

                    try
                    {
                        // _context.EtapVSM.Attach(_etapVSM);
                        // _context.Entry(_etapVSM).Property(o => o.ActualTimeCircle).IsModified = true;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                var orderItemDB = _currentOrderItem.getOrderProductItem();

                Console.WriteLine("Заказ : " + orderItemDB.Orders_production.Order.Name + "(" + orderItemDB.Part.ToString() + ") Выполнился Этапом: " + _etapVSM.Name);
                Console.WriteLine("Description: " + _etapVSM.Description);

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Останавливаем симуляцию на этапе");
                _buf.Stop(orderItem);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка Експетион " + e.ToString());
            }

        }

    }
}
