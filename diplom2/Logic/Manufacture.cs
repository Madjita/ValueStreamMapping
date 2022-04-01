
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using auntification.Models;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;
using diplom2.IntermediateData;

namespace diplom2.Logic
{

    public enum ExeptionMessage
    {
        Success,
        WarningWeHaveItemInBD,
        Error
    };

    public class ExeptionVSM
    {
        public ExeptionMessage Exeption;
    }

    public class ProductionExeption : EtapVSMExeption
    {
        public Productions production;
    }
    public class CardVSMExeption : ExeptionVSM
    {
        public CardVSM cardVSM;
    }

    public class BufferVSMExeption : ExeptionVSM
    {
        public BufferVSM bufferVSM;
    }

    public class EtapVSMExeption : ExeptionVSM
    {
        public EtapVSM etapVSM;
    }

    public class EtapSectionsExeption : ExeptionVSM
    {
        public EtapSections etapSections;
    }

    public class UserExeption : ExeptionVSM
    {
        public User user;
    }




    public class Manufacture
    {

        object loker = new object();

        public static List<SimulationOrder> _list_Order { get; set; } = new List<SimulationOrder>();    // Лист заказаов которы нужно произвести

       

        public static List<SimulationUser> _users = new List<SimulationUser>(); // Лис юзеров для симуляций
        //
        private static List<SimulationBufferVSM> _all_bufferVSM = new List<SimulationBufferVSM>(); // Лист всех буферов (для создания симуляции работы буфера)
        private static List<SimulationEtapVSM> _all_EtapVSM = new List<SimulationEtapVSM>();     // Лист всех этапов (для создания симуляции работы этапа)
        //

        private static List<SimulationCardVSM> _all_CardVSM = new List<SimulationCardVSM>();


        public Manufacture()
        {
          
            GetAllUserUsedInCards();
            GetAllBufferVSM();
            GetAllEtapVSM();
            GetAllCardVSM();


            getAllOrderFromDB();

            /* for(int i=0;i < 20; i++)
            {
                ThreadPool.QueueUserWorkItem(delegate (object state) { addSimOrder("Thread "+ i); });
            }*/

            /*  List<Thread> threads = new List<Thread>();

              for (int i = 0; i < 1000; i++)
              {
                  Thread thread = new Thread(delegate (object state) { addSimOrder("Thread " + i); });
                  threads.Add(thread);
                  thread.Start();
              }*/

        }



        internal List<DataAllCardVSM> GetAllCards()
        {
            List<DataAllCardVSM> cards = new List<DataAllCardVSM>();

            foreach(var simCard in _all_CardVSM)
            {
                var obj = new DataAllCardVSM {
                    sections = simCard._card,
                    name = simCard._product.Name
                };

                cards.Add(obj);
            }

            return cards;
        }

        //Создали все карты которые есть в базе
        private void GetAllCardVSM()
        {
            try
            {
                List<Productions> list;

                using (var _context = new Context(DBConnect.options))
                {
                    list = _context.Productions.ToList();
                }

                if (list.Count > 0)
                {
                    foreach (var product in list)
                    {
                        var newObj = new SimulationCardVSM(product, _all_bufferVSM, _all_EtapVSM);

                        _all_CardVSM.Add(newObj);
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal void UpdateCard()
        {
            GetAllUserUsedInCards();
            GetAllBufferVSM();
            GetAllEtapVSM();
            addCardVSM();
        }

        //не доработанно добавление и  обновление карты
        public void addCardVSM()
        {
            try
            {
                List<Productions> list;

                using (var _context = new Context(DBConnect.options))
                {
                    list = _context.Productions.ToList();
                }

                if (list.Count > 0)
                {
                    foreach (var product in list)
                    {

                        var newObj = new SimulationCardVSM(product, _all_bufferVSM, _all_EtapVSM);

                        _all_CardVSM.Add(newObj);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /*internal List<SimulationOrder> getAllOrders()
        {
            return _list_Order;
        }*/


        public JArray getAllOrders()
        {
            JArray _array_orders = new JArray();
            foreach (var item in _list_Order)
            {

                _array_orders.Add(item.ToJson());

            }

            return _array_orders;
        }




        private SimulationCardVSM findCardVSM(Productions product)
        {
            var findCard = (SimulationCardVSM)_all_CardVSM.Find(o => o._product.Id == product.Id).Clone();
            return findCard;
        }

        public BufferVSMExeption CreateBufferVSM(BufferVSM bufferVSM)
        {
            var bufferFromBD = new BufferVSMExeption();

            using (var _context = new Context(DBConnect.options))
            {
                //Проверить на существование
                var check = _context.BufferVSM
                    .Where(i => i.Name == bufferVSM.Name)
                    .Include(i=> i.BufferVSMQueue)
                    .FirstOrDefault();

                if (check == null)
                {
                    _context.BufferVSM.Add(bufferVSM);
                    _context.SaveChanges();

                    bufferFromBD.bufferVSM = bufferVSM;
                    bufferFromBD.Exeption = ExeptionMessage.Success;
                }
                else
                {
                    //мы пытаемся добавить уже сушествующего пользователя
                    bufferVSM = check;
                    bufferFromBD.bufferVSM = bufferVSM;
                    bufferFromBD.Exeption = ExeptionMessage.WarningWeHaveItemInBD;
                }
            }

            return bufferFromBD;
        }

        public JArray getAllOrdersFromProcess()
        {
            JArray _array_orders = new JArray();
            foreach (var item in _list_Order)
            {

                _array_orders.Add(item.ToJson());

            }

            return _array_orders;
        }

        public CardVSMExeption CreateCardVSM(JObject cardVSM)
        {
            var cardVSMFromBD = new CardVSMExeption();

            using (var _context = new Context(DBConnect.options))
            {
                //Поиск production
                var production = _context.Productions.Where(i => i.Name == cardVSM.GetValue("production").ToString()).FirstOrDefault();

                if (production == null)
                {
                    cardVSMFromBD.cardVSM = null;
                    cardVSMFromBD.Exeption = ExeptionMessage.Error;
                    return cardVSMFromBD;
                }


                //Поиск bufer
                var bufer = _context.BufferVSM.Where(i => i.Name == cardVSM.GetValue("bufer").ToString()).FirstOrDefault();

                if (bufer == null)
                {
                    cardVSMFromBD.cardVSM = null;
                    cardVSMFromBD.Exeption = ExeptionMessage.Error;
                    return cardVSMFromBD;
                }

                //Поиск etap
                var etap = _context.EtapVSM.Include(i => i.EtapSections).Where(i => i.Name == cardVSM.GetValue("etap").ToString()).FirstOrDefault();

                if (etap == null)
                {
                    cardVSMFromBD.cardVSM = null;
                    cardVSMFromBD.Exeption = ExeptionMessage.Error;
                    return cardVSMFromBD;
                }

                //Проверить на существование
                var check = _context.CardVSM
                    .Include(i => i.Production)
                    .Include(i => i.BufferVSM)
                    .Include(i => i.EtapVSM)
                    .Where(i => (i.Production.Name == production.Name) &&
                                (i.BufferVSM.Id == bufer.Id) &&
                                (i.EtapVSM.Id == etap.Id) &&
                                (i.EtapNumeric == int.Parse(cardVSM.GetValue("sections").ToString()))
                          )
                    .FirstOrDefault();


                if (check == null)
                {

                    cardVSMFromBD.cardVSM = new CardVSM();
                    var card = cardVSMFromBD.cardVSM;

                    card.EtapNumeric = int.Parse(cardVSM.GetValue("sections").ToString());
                    card.Production = production;
                    card.BufferVSM = bufer;
                    card.EtapVSM = etap;


                    _context.CardVSM.Add(card);
                    _context.SaveChanges();

                    cardVSMFromBD.Exeption = ExeptionMessage.Success;


                }
                else
                {
                    //мы пытаемся добавить уже сушествующего пользователя
                    cardVSMFromBD.cardVSM = check;
                    cardVSMFromBD.Exeption = ExeptionMessage.WarningWeHaveItemInBD;
                }
            }

            return cardVSMFromBD;
        }


        public EtapVSMExeption CreateEtapVSM(EtapVSM etapVSM)
        {
            var etapFromBD = new EtapVSMExeption();

            using (var _context = new Context(DBConnect.options))
            {
                //Проверить на существование
                var check = _context.EtapVSM.Where(i => i.Name == etapVSM.Name).FirstOrDefault();

                if (check == null)
                {
                    _context.EtapVSM.Add(etapVSM);
                    _context.SaveChanges();

                    etapFromBD.etapVSM = etapVSM;
                    etapFromBD.Exeption = ExeptionMessage.Success;
                }
                else
                {
                    //мы пытаемся добавить уже сушествующего пользователя
                    etapVSM = check;
                    etapFromBD.etapVSM = etapVSM;
                    etapFromBD.Exeption = ExeptionMessage.WarningWeHaveItemInBD;
                }
            }


            return etapFromBD;
        }

        public EtapSectionsExeption CreateEtapSections(JObject etapSections)
        {
            var etapSectionsFromBD = new EtapSectionsExeption();

            using (var _context = new Context(DBConnect.options))
            {
                //Поиск User
                var user = _context.Users.Where(i => i.Name == etapSections.GetValue("worker").ToString()).FirstOrDefault();

                //Поиск Этапа
                var etap = _context.EtapVSM.Include(i => i.EtapSections).Where(i => i.Name == etapSections.GetValue("etap").ToString()).FirstOrDefault();

                //Проверить на существование
                var check = _context.EtapSections.Include(i => i.User).Where(i => i.User.Name == user.Name && i.EtapVSMId == etap.Id).FirstOrDefault();


                if (check == null)
                {
                    etapSectionsFromBD.etapSections = new EtapSections();
                    var setion = etapSectionsFromBD.etapSections;
                    setion.User = user;

                    etap.EtapSections.Add(setion);
                    _context.EtapSections.Add(setion);
                    _context.SaveChanges();

                    etapSectionsFromBD.Exeption = ExeptionMessage.Success;

                }
                else
                {
                    //мы пытаемся добавить уже сушествующего пользователя
                    etapSectionsFromBD.etapSections = check;
                    etapSectionsFromBD.Exeption = ExeptionMessage.WarningWeHaveItemInBD;
                }
            }

            return etapSectionsFromBD;
        }


        public UserExeption CreateUser(auntification.Models.User user)
        {
            var userFromBD = new UserExeption();

            using (var _context = new Context(DBConnect.options))
            {

                //Проверить на существование
                var check = _context.Users.Where(i => i.Name == user.Name).FirstOrDefault();

                if (check == null)
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    userFromBD.user = user;
                    userFromBD.Exeption = ExeptionMessage.Success;
                }
                else
                {
                    //мы пытаемся добавить уже сушествующего пользователя
                    user = check;
                    userFromBD.user = user;
                    userFromBD.Exeption = ExeptionMessage.WarningWeHaveItemInBD;
                }
            }

            return userFromBD;
        }


        //Создание продукта в базе
        public ProductionExeption CreateProduction(Productions production)
        {
            var productionFromBD = new ProductionExeption();

            //Проверить на существование

            using (var _context = new Context(DBConnect.options))
            {
                var check = _context.Productions
                .Where(i => i.Name == production.Name)
                .FirstOrDefault();


                if (check == null)
                {

                    _context.Productions.Add(production);
                    _context.SaveChanges();

                    productionFromBD.production = production;
                    productionFromBD.Exeption = ExeptionMessage.Success;


                }
                else
                {
                    //мы пытаемся добавить уже сушествующего пользователя
                    productionFromBD.production = check;
                    productionFromBD.Exeption = ExeptionMessage.Success;
                }

            }

            return productionFromBD;
        }


        public Order FindOrder(Order order)
        {
            var findObj = _list_Order.Find(o => o.getId() == order.Id);

            if (findObj == null)
            {
                return null;
            }

            

            return findObj.getORder();
        }

       

        public void UpdateOrder(Order order)
        {
            var findObj = _list_Order.Find(o => o.getId() == order.Id);

            if (findObj == null)
            {
               
            }
            else
            {
                findObj.Update(order);
            }
        }

        public void getAllOrderFromDB()
        {
            try
            {

                using (var _context = new Context(DBConnect.options))
                {
                    var orders = _context.Order
                        .Include(o => o.Orders_production)
                        .ThenInclude(o=> o.Production)
                        .Include(o=> o.Orders_production)
                        .ThenInclude(o=> o.Orders_production_items)
                        .ToList();

                    foreach (var item in orders)
                    {
                        addSimOrder(item);


                        if ((bool)item.Simulation)
                        {
                            StartSimulation(item);
                        }
                    }

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void StartSimulation(Order order)
        {

            lock (loker)
            {
                Console.WriteLine("Заблокировали");
                var findObj = _list_Order.Find(o => o.getId() == order.Id);


                if ((bool)order.Simulation)
                {
                    Console.WriteLine("FIND START");
                    findObj.StartSim();
                }
                else
                {
                    Console.WriteLine("FIND STOP");
                    findObj.StopSim();
                }
                Console.WriteLine("Отпустили ");
            }
        }

        public void addSimOrder(Order order)
        {
            var obj = new SimulationOrder(order);
          

            List<float> times = new List<float>();

            foreach (var item in order.Orders_production)
            {
                //Создаем карту для продукта в заказе
                var TPlan_product = obj.AddCard(item.Production,findCardVSM(item.Production));

                times.Add(TPlan_product);
            }

            var TPlan = times.Max();

            using (var _context = new Context(DBConnect.options))
            {
                var objDB = _context.Order.Where(o => o.Id == obj.getId()).FirstOrDefault();

                //Прогнозируем
                obj.getORder().TPlan = TPlan;


                //Прогнозируем
                objDB.TPlan = TPlan;

                _context.SaveChanges();
            }


            _list_Order.Add(obj);


        }


        public SimulationOrderProduct getSimOrderProduct()
        {
            return _list_Order[0].GetSimulationOrderProduct();
        }


        public void GetAllUserUsedInCards()
        {
            using (var _context = new Context(DBConnect.options))
            {
                try
                {
                    var Users = _context.EtapSections.Include(i => i.User).Select(i => i.User).Distinct().AsNoTracking();

                    //Находим всех User которые используются в этапах
                    foreach (var item in Users)
                    {
                        var find = _users.Where(i => i.Get().Id == item.Id).FirstOrDefault();
                        if (find == null)
                        {
                            _users.Add(new SimulationUser(item));
                        }

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        private void GetAllBufferVSM()
        {
            using (var _context = new Context(DBConnect.options))
            {
                try
                {
                    List<BufferVSM> bufferVSMs = _context.BufferVSM.AsNoTracking().ToList();
                    foreach (var item in bufferVSMs)
                    {
                        var find = _all_bufferVSM.Where(i => i.GetBuffer().Id == item.Id).FirstOrDefault();
                        if (find == null)
                        {
                            _all_bufferVSM.Add(new SimulationBufferVSM(item));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


            }
        }

        private void GetAllEtapVSM()
        {
            using (var _context = new Context(DBConnect.options))
            {
                try
                {
                    List<EtapVSM> etapVSMs = _context.EtapVSM
                        .Include(i => i.EtapSections)
                        .ThenInclude(i => i.ArchiveSection)
                        .AsNoTracking()
                        .ToList();

                    foreach (EtapVSM item in etapVSMs)
                    {
                        var find = _all_EtapVSM.Where(i => i.GetEtap().Id == item.Id).FirstOrDefault();
                        if (find == null)
                        {
                            _all_EtapVSM.Add(new SimulationEtapVSM(item, _users));
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        internal List<SimulationCardVSM> FindCard(Order order)
        {
            List<SimulationCardVSM> card = new List<SimulationCardVSM>();
            foreach(var item in order.Orders_production)
            {
                card.Add(_all_CardVSM.Find(o => o._product.Id == item.Production.Id));
            }

            return card;
        }
    }
}
