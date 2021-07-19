using System;
using System.Collections.Generic;
using DiplomReactNetCore.DAL.Context;
using DiplomReactNetCore.DAL.Models.DataBase;
using DiplomReactNetCore.L.Simulation.Interface;
using Microsoft.EntityFrameworkCore;

namespace DiplomReactNetCore.L.Simulation
{
    public class SimulationBufferQueueVSM : ISimulation
    {
        private QueueBufferVSM _queueBufferVSM;

        public SimulationBufferQueueVSM(string connection)
        {
            _connection = connection;
        }

        public SimulationBufferQueueVSM(QueueBufferVSM queueBufferVSM, string connection)
        {
            _connection = connection;
            _queueBufferVSM = queueBufferVSM;
        }


        public void Add(Order order, int bufId)
        {
           if(_queueBufferVSM == null)
           {
              _queueBufferVSM = new QueueBufferVSM();
              _queueBufferVSM.BufferVSMId = bufId;
              _queueBufferVSM.OrderId = order.Id;
              _queueBufferVSM.TimeWait = 0;

              using (var _context = getContext())
              {
                    _context.QueueBufferVSM.Add(_queueBufferVSM);
                    _context.SaveChanges();
              }
           }
        }

        public void UpdateTime(int time_sec)
        {
            using (var _context = getContext())
            {
                _queueBufferVSM.TimeWait += time_sec;
                _context.SaveChanges();
            }
        }
    }
}
