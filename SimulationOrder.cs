using System;
using System.Threading;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace diplom2.Logic
{
    public class SimulationOrder
    {
        int _count = 0;

        Order _order;
        public SimulationOrder()
        {
        }


        public void Run()
        {
            CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(8));

            Console.WriteLine("Starting action loop. ( " + _order.Name + " )");
            RepeatActionEvery(() => Do(),
              TimeSpan.FromSeconds(1), cancellation.Token).Wait();
            Console.WriteLine("Finished action loop. ( " + _order.Name + " )");
        }


        public void Do()
        {

            _count++;

            using (var _context = new Context(DBConnect.options))
            {

            }
        }

        public async Task RepeatActionEvery(Action action,
          TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                action();
                Task task = Task.Delay(interval); //cancellationToken

                try
                {
                    await task;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }
    }
}
