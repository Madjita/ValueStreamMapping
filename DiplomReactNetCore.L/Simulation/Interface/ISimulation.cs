using System;
using System.Collections.Generic;
using System.Threading;
using DiplomReactNetCore.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace DiplomReactNetCore.L.Simulation.Interface
{
    public class ISimulation
    {
        public ManualResetEvent mre { get; set; }

        public List<ManualResetEvent> resetEvents { get; set; }

        public bool IsPaused { get { return !mre.WaitOne(0); } }
        public object locker { get; set; }
        public Thread thread { get; set; }

        public bool _exit { get; set; }
        public bool _start { get; set; }
        public bool _done { get; set; }

        public void setDone(bool done)
        {
            _done = done;
        }
        public void setStart(bool start) => _start = start;
        public bool getStart() { return _start; }

        protected string _connection { get; set; }
        public DbContextOptions<MyContext> ConnectionBD()
        {
            return new DbContextOptionsBuilder<MyContext>()
                       .UseSqlite(_connection)
                       .Options;
        }

        public MyContext getContext()
        {
            return new MyContext(ConnectionBD());
        }
    }
}
