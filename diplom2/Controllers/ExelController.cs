using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using diplom2.Data;
using diplom2.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace diplom2.Controllers
{
    [Route("api/exel")]
    public class ExelController : Controller
    {
        private readonly Context _context;
        private Manufacture _manufacture;

        public ExelController(Context context, Manufacture manufacture)
        {
            _context = context;
            _manufacture = manufacture;
        }


        [HttpPost("pdf")]
        public IActionResult pdf([FromForm] IFormFile file) //[FromForm] 
        {

            return Ok();
        }

        [HttpPost("add")]
        public IActionResult ParseData([FromForm] IFormFile file) //[FromForm] 
        {
            var exel = new MyExel.Exel(file);
            var productionsSMFromExel = exel.GetRowDataProductions();
            var usersFromExel = exel.GetRowDataUsers();
            var bufferVSMsFromExel = exel.GetRowDataBufferVSM();
            var etapVSMsFromExel = exel.GetRowDataEtapVSM();
            var etapSectionsFromExel = exel.GetRowDataEtapSections();
            var cardVSMFromExel = exel.GetRowDataCardVSM();


            List<CreatedResult> dataProductions = new List<CreatedResult>();
            List<CreatedResult> dataUsers = new List<CreatedResult>();
            List<CreatedResult> dataBufferVSMs = new List<CreatedResult>();
            List<CreatedResult> dataEtapVSMs = new List<CreatedResult>();
            List<CreatedResult> dataEtapSections = new List<CreatedResult>();
            List<CreatedResult> dataCardVSM = new List<CreatedResult>();


            foreach (var production in productionsSMFromExel.Values)
            {
                var result = _manufacture.CreateProduction(production);
                dataProductions.Add(Created(result.Exeption.ToString(), result.production));
            }

            foreach (var user in usersFromExel.Values)
            {
                var result = _manufacture.CreateUser(user);
                dataUsers.Add(Created(result.Exeption.ToString(), result.user));
            }

            foreach (var bufferVSM in bufferVSMsFromExel.Values)
            {
                var result = _manufacture.CreateBufferVSM(bufferVSM);
                dataBufferVSMs.Add(Created(result.Exeption.ToString(), result.bufferVSM));
            }

            foreach (var etapVSM in etapVSMsFromExel.Values)
            {
                var result = _manufacture.CreateEtapVSM(etapVSM);
                dataEtapVSMs.Add(Created(result.Exeption.ToString(), result.etapVSM));
            }

            foreach (var etapSection in etapSectionsFromExel.Values)
            {
                var result = _manufacture.CreateEtapSections(etapSection);
                dataEtapSections.Add(Created(result.Exeption.ToString(), result.etapSections));
            }

            foreach (var cardVSM in cardVSMFromExel.Values)
            {
                var result = _manufacture.CreateCardVSM(cardVSM);
                dataCardVSM.Add(Created(result.Exeption.ToString(), result.cardVSM));
            }


            _manufacture.UpdateCard();

            Console.WriteLine("EXEL");

            return Ok(new
            {
                dataProductions = dataProductions,
                dataUsers = dataUsers,
                dataBufferVSMs = dataBufferVSMs,
                dataEtapVSMs = dataEtapVSMs,
                dataEtapSections = dataEtapSections,
                dataCardVSM = dataCardVSM
            });
        }


    }
}
