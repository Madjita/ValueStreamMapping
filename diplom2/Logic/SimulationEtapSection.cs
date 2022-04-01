using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using diplom2.Data;
using DiplomReactNetCore.DAL.Models.DataBase;

namespace diplom2.Logic
{
    public class SimulationEtapSection
    {
        EtapSections _etapSection;
        SimulationUser _simulationUser;

        //  List<SimulationSection> sections;

        //SimulationUser user

        public SimulationEtapSection(EtapSections etapSection, SimulationUser user)
        {
            _etapSection = etapSection;
            _simulationUser = user;
        }

        public void SetCardId(int cardId)
        {
            using (var _context = new Context(DBConnect.options))
            {
                //_etapSection.CardVSMId = cardId;

                var obj = _context.EtapSections.Where(i => i.Id == _etapSection.Id).FirstOrDefault();

                //obj.CardVSMId = cardId;

                _context.SaveChanges();
            }

        }

        public void Add(SimulationSection item)
        {
            _simulationUser.AddSections(item);
        }

        public float TACtual => (float)_etapSection.TActual;


        //Проверить какой из юзеров свободен
        public bool IsWorkUser() => _simulationUser.IsUserBusy();

        public Task StartWorkUser(List<Task> tasks, ManualResetEvent worker, CancellationToken cancellationToken)
        {
            return _simulationUser.CreateTask(tasks, _etapSection, worker, cancellationToken);
        }

    }
}
