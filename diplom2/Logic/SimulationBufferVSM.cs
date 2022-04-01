using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace diplom2.Logic
{
    public class SimulationBufferVSM
    {
        BufferVSM _bufferVSM;

        public object loker;
        public object loker2;
        public object loker3;
        public object loker4;
        public object loker5;
        public object loker6;

        public Heap<SimulationOrderProductItem> queueOrdersItems; // Очередь с приоритетом по 1 заказу


        public SimulationBufferVSM(BufferVSM bufferVSM)
        {
            _bufferVSM = bufferVSM;

            //_connectionString = connectionString;

            loker = new object();
            loker2 = new object();
            loker3 = new object();
            loker4 = new object();
            loker5 = new object();
            loker6 = new object();

            Comparer<SimulationOrderProductItem> comparer = Comparer<SimulationOrderProductItem>.Default;
            queueOrdersItems = new Heap<SimulationOrderProductItem>(comparer);
        }


        public BufferVSM GetBuffer()
        {
            return _bufferVSM;
        }

        public void Decriment()
        {
           
                using (var _context = new Context(DBConnect.options))
                {
                    var findObj = _context.BufferVSM.Where(o => o.Id == _bufferVSM.Id).FirstOrDefault();

                    /*_context.BufferVSM.Attach(_bufferVSM);

                    if (_bufferVSM.Value > 0)
                    {
                        _bufferVSM.Value -= _bufferVSM.ValueDefault;
                    }*/

                    lock (loker2)
                    {
                        if (_bufferVSM.Value > 0)
                        {
                            _bufferVSM.Value -= _bufferVSM.ValueDefault;
                        }
                    }

                    findObj.Value = _bufferVSM.Value;



                        // _context.Entry(_bufferVSM).Property(o => o.Value).IsModified = true;

                     _context.SaveChanges();

                }
           
        }

        internal void DeleteOrders(List<SimulationOrderProductItem> list)
        {
            foreach (var item in list)
            {
                queueOrdersItems.Delete(item);
            }
        }

        public int getWaitTime_allCountInBuffer(int? priority)
        {
            var copy = queueOrdersItems.ToListCopy();

            var listPriority = copy.Where(o => o.getOrderProductItem().Priority >= priority).ToList();

            return listPriority.Count; 
        }

        public void Insert(SimulationOrderProductItem item)
        {
            lock (loker3)
            {
                addOrder(item);
            }
        }

        public int Count()
        {
            int n = 0;
            lock (loker)
            {
                n = queueOrdersItems.Count;
            }
            return n;
        }

        public void addOrder(SimulationOrderProductItem orderItem)
        {
            lock (loker3)
            {
                try
                {
                    //Console.WriteLine("Loker + " + _bufferVSM.Name);

                    queueOrdersItems.Insert(orderItem);

                    var obj = new BufferVSMQueue
                    {
                        BufferRole = BufferRole.Wait,
                        CurrentOrderItems = orderItem.getOrderProductItem(),
                        TAdd = DateTime.UtcNow,
                        BufferVSMId = _bufferVSM.Id
                    };

                    //Console.WriteLine("queueOrdersItems Insert");

                    using (var _context = new Context(DBConnect.options))
                    {
                        if (_bufferVSM != null)
                        {
                            _context.Entry(_bufferVSM).State = EntityState.Detached;
                            _context.BufferVSM.Attach(_bufferVSM);

                            _bufferVSM.BufferVSMQueue.Add(obj);

                            _context.SaveChanges();
                        }

                        // _context.Entry(_bufferVSM).State = EntityState.Detached;
                    }

                    //Console.WriteLine("Loker END " + _bufferVSM.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void TPop(Orders_production_items orderItemDB)
        {
            lock (loker6)
            {
                var obj = _bufferVSM.BufferVSMQueue.Find(o => o.CurrentOrderItems.Id == orderItemDB.Id);
                obj.TPop = DateTime.UtcNow;
                obj.TWait = ((DateTime)obj.TPop).Subtract((DateTime)obj.TAdd).Seconds;
                obj.BufferRole = BufferRole.Archive;

                using (var _context = new Context(DBConnect.options))
                {
                    var find = _context.BufferVSMQueue.Find(obj);

                    find.TPop = obj.TPop;
                    find.TWait = obj.TWait;
                    find.BufferRole = obj.BufferRole;

                    _context.SaveChanges();
                }

                if (_bufferVSM.BufferVSMQueue.Contains(obj))
                {
                    _bufferVSM.BufferVSMQueue.Remove(obj);
                }
            }
        }

        internal SimulationOrderProductItem GetMax()
        {
          

            lock (loker4)
            {
                var obj = queueOrdersItems.GetMax();

                try
                {
                    var find = _bufferVSM.BufferVSMQueue.Where(o => o.Orders_production_itemsId == obj.getOrderProductItem().Id).FirstOrDefault();

                    if (find != null)
                    {
                        find.BufferRole = BufferRole.Archive;
                        find.TPop = DateTime.UtcNow;
                        find.TWait = ((DateTime)find.TPop).Subtract((DateTime)find.TAdd).Milliseconds;

                        using (var _context = new Context(DBConnect.options))
                        {

                            var objDB = _context.BufferVSMQueue.Where(o => o.Id == find.Id).FirstOrDefault();

                            if(objDB != null)
                            {
                                objDB.TPop = find.TPop;
                                objDB.TWait = find.TWait;
                                objDB.BufferRole = find.BufferRole;
                                _context.SaveChanges();
                            }

                            
                        }

                        if(_bufferVSM.BufferVSMQueue.Any(x=> x.Id == find.Id))
                        {
                            _bufferVSM.BufferVSMQueue.Remove(find);
                        }

                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                return obj;
            }

         
        }

        internal void Stop(SimulationOrderProductItem orderItem)
        {
          
                Console.WriteLine("Loker Stop + " + _bufferVSM.Name);

                var obj = _bufferVSM.BufferVSMQueue.Where(o => o.CurrentOrderItems.Id == orderItem.getId()).FirstOrDefault();

                try
                {
                    lock (loker5)
                    {
                        if (obj == null)
                        {
                            using (var _context = new Context(DBConnect.options))
                            {

                                var find = _context.BufferVSMQueue.Where(o => o.Id == obj.Id).FirstOrDefault();

                                if (find != null)
                                {
                                    find.BufferRole = BufferRole.Stop;

                                    _context.SaveChanges();
                                }

                            }

                            return;
                        }

                        obj.BufferRole = BufferRole.Stop;

                        using (var _context = new Context(DBConnect.options))
                        {

                            var find = _context.BufferVSMQueue.Where(o => o.Id == obj.Id).FirstOrDefault();

                            if (find != null)
                            {
                                find.BufferRole = BufferRole.Stop;

                                _context.SaveChanges();
                            }
                        }


                        if (_bufferVSM.BufferVSMQueue.Any(x => obj.Id == x.Id))
                        {
                            _bufferVSM.BufferVSMQueue.Remove(obj);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("E = " + e.ToString());
                }
            }
    }
}
