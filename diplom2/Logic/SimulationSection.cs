using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diplom2.Data;

namespace diplom2.Logic
{
    public class SimulationSection
    {
        public SimulationBufferVSM _simulationBufferVSM;
        public SimulationEtapVSM _simulationEtap;


        public object locker = new object();

        public int EtapNumeric;
        public SimulationSection(int etapNumeric, SimulationBufferVSM simulationBufferVSM, SimulationEtapVSM simulationEtapVSM)
        {
            EtapNumeric = etapNumeric;
            _simulationBufferVSM = simulationBufferVSM;
            _simulationEtap = simulationEtapVSM;
        }

        public void Work(SimulationOrderProductItem orderItem, List<Task> tasks, System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine("Ждем Секцию : " + orderItem.getOrderProductItem().Part);
            tasks.Add(Task.Run(async () => await _simulationEtap.Work(orderItem, _simulationBufferVSM, cancellationToken), cancellationToken));
        }

        public int getWorkTime(DiplomReactNetCore.DAL.Models.DataBase.Orders_production _orders_production)
        {
            float TPlan = 0;

            lock(locker)
            {
                var allCountINBufQ = _simulationBufferVSM.getWaitTime_allCountInBuffer(_orders_production.Priority);

                var time_workEtap = _simulationEtap.GetEtap().DefaultTimeCircle;

                var TPlan_real = allCountINBufQ * time_workEtap; // Время плана на реальный момент времени по этой секции

                var Add_TPlan = _orders_production.Quantity * time_workEtap;


                //Посчитать план для каждого заказика по штучно
                for (int i = 0; i < _orders_production.Orders_production_items.Count; i++)
                {

                    var Add_TPlan_item = (i + 1) * time_workEtap;


                    using (var _context = new Context(DBConnect.options))
                    {
                        var obj = _context.Orders_production_items.Where(o => o.Id == _orders_production.Id).FirstOrDefault();

                        obj.TFuture = _orders_production.TFuture;
                        _context.SaveChanges();
                    }
                }

                TPlan = (float)(TPlan_real + Add_TPlan);
            }

            return (int)TPlan;
        }

        internal void DeleteOrder(List<SimulationOrderProductItem> list)
        {
            _simulationBufferVSM.DeleteOrders(list);

        }
    }
}
