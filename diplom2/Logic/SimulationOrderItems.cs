using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace diplom2.Logic
{
    public class SimulationOrderItems : IComparable<SimulationOrderItems>
    {
        public object locker;



        private System.Timers.Timer _timer = new System.Timers.Timer();

        Orders_production_items _orderItem;

        Timer timer;


        private string _connectionString;
        public SimulationOrderItems(Orders_production_items orderItem)
        {
            _orderItem = orderItem;


            timer = new Timer();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            locker = new object();


        }

        private Context GetContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseSqlServer(_connectionString);
            return new Context(optionsBuilder.Options);
        }

        public void SetDBContextOprion(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CompareTo(SimulationOrderItems other)
        {
            if (this._orderItem.Priority == null)
            {
                this._orderItem.Priority = 0;
            }

            var p = Int32.Parse(this._orderItem.Priority.ToString()).CompareTo(other._orderItem.Priority);
            return p;
        }

        public int getTActual()
        {
            return (int)_orderItem.TActual;
        }

        public Orders_production_items Get()
        {
            return _orderItem;
        }

        public async Task SimulationStart()
        {
            using (var _context = new Context(DBConnect.options))
            {
                _context.Orders_production_items.Attach(_orderItem);

                _orderItem.TStart = DateTime.Now;
                _orderItem.TActual = 0;
                _orderItem.Simulation = true;
                _orderItem.OrderRole = OrderRole.Work;



                /*  _context.Entry(_orderItem).Property(o => o.TStart).IsModified = true;
                  _context.Entry(_orderItem).Property(o => o.TActual).IsModified = true;
                  _context.Entry(_orderItem).Property(o => o.Simulation).IsModified = true;
                  _context.Entry(_orderItem).Property(o => o.OrderRole).IsModified = true;*/

                await _context.SaveChangesAsync();

                _context.Entry(_orderItem).State = EntityState.Detached;

            }

            if (timer.Enabled)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
            }
        }



        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            using (var _context = new Context(DBConnect.options))
            {

                _context.Orders_production_items.Attach(_orderItem);
                _orderItem.TActual++;

                // var find = _context.Orders_production_items.Where(i => i.Id == _orderItem.Id).FirstOrDefault();
                // find.TActual = _orderItem.TActual;


                await _context.SaveChangesAsync();

                _context.Entry(_orderItem).State = EntityState.Detached;
            }


        }



        public async Task SimulationStop()
        {
            timer.Stop();
            timer.Dispose();

            using (var _context = new Context(DBConnect.options))
            {
                _context.Orders_production_items.Attach(_orderItem);

                _orderItem.TStop = DateTime.Now;
                _orderItem.Simulation = false;
                _orderItem.OrderRole = OrderRole.Archive;

                // _orderItem.OrderCurrentSection.Clear();


               // _orderItem.ActualEtapVSMId = null;
               // _orderItem.ActualEtapSectionsId = null;
               // _orderItem.ActualBufferVSMId = null;



                await _context.SaveChangesAsync();

                _context.Entry(_orderItem).State = EntityState.Detached;
            }


        }

        public bool FindIndex(SimulationOrderItems obj)
        {
            return this._orderItem.Id == obj._orderItem.Id;
        }


        public async Task UpdateBuf(int bufId)
        {
            using (var _context = new Context(DBConnect.options))
            {
                _context.Orders_production_items.Attach(_orderItem);
               // _orderItem.ActualBufferVSMId = bufId;
               // _orderItem.ActualEtapSectionsId = null;
               // _orderItem.ActualEtapVSMId = null;


                await _context.SaveChangesAsync();


                _context.Entry(_orderItem).State = EntityState.Detached;
            }

        }
    }
}
