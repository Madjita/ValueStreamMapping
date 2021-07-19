using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DiplomReactNetCore.DAL.Models.DataBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiplomReactNetCore.L.Simulation
{
    public class SimulationSection
    {
        public SimulationBufferVSM _SimulationBufferVSM;
        public SimulationEtapVSM _SimulationEtapVSM;

        public SimulationSection(SimulationBufferVSM SimulationBufferVSM, SimulationEtapVSM SimulationEtapVSM)
        {
            _SimulationBufferVSM = SimulationBufferVSM;
            _SimulationEtapVSM = SimulationEtapVSM;
        }

        public JObject ToJson()
        {
            JObject buf = null;
            if (_SimulationBufferVSM != null)
            {
                buf = _SimulationBufferVSM.ToJson();
            }
            JObject etap = _SimulationEtapVSM.ToJson();

            JObject obj = new JObject();

            obj.Add(new JProperty("buf",buf));
            obj.Add(new JProperty("etap", etap));

            return obj;

        }

        async public void  Work(SimulationOrder order, List<Task> tasks)
        {
            Console.WriteLine("Ждем Секцию");
            tasks.Add(Task.Run(() => _SimulationEtapVSM.Work(order, _SimulationBufferVSM)));
        }
    }
}
