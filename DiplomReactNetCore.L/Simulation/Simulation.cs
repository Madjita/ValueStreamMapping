using System;
using System.Threading;

namespace DiplomReactNetCore.L.Simulation
{
    public class Simulation
    {
        public static ManualResetEvent mre = new ManualResetEvent(false);
        // Check pause state
        public bool IsPaused { get { return !mre.WaitOne(0); } }

        public bool _exit;

        public bool _start;

        public bool _done;
        public void setDone(bool done)
        {
            _done = done;
            //setStart(!_start);
        }
        public void setStart(bool start) => _start = start;
        public bool getStart() { return _start;}

        public static object locker = new object();

        private Thread thread;

        public Simulation()
        {
            Console.WriteLine("START Simulation");
            thread = new Thread(Worker);
            thread.Start();
        }


        ~Simulation()
        {
            Console.WriteLine("Disposed Simulation");
            _exit = true;

            if (IsPaused)
            {
                mre.Set();
            }
        }


        public void Start(bool start)
        {
            if (start)
            {
                if (_start == false)
                {
                    mre.Set();
                }

                _start = true;
                _done = true;

            }
            else
            {
                //_start = false;
                _done = true;
            }
        }


        public virtual  void Worker()
        {
            while (true) {
                mre.WaitOne(); // Флаг ждет запуска потока
                if(_exit) { Console.WriteLine("Exit Thread"); return; } // Exit

                Console.WriteLine("Worker new Thread");
                while (_start) {
                if(_exit) { Console.WriteLine("Exit Thread"); return; } // Exit

                lock (locker)
                {
                    if (!_done)
                    {
                        Console.WriteLine("Done Exit Thread");
                        _done = true;
                        _start = false;
                        break;
                    }
                }
                }
                _start = false;
                mre.Reset();
            }
        }
    }
}
