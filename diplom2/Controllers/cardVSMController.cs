using System;
using Microsoft.AspNetCore.Mvc;
using DiplomReactNetCore.DAL.Models.DataBase;
using System.Threading.Tasks;
using diplom2.Data;
using System.Linq;
using diplom2.IntermediateData;
using Microsoft.EntityFrameworkCore;
using diplom2.Logic;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace diplom2.Controllers
{

    public class Section
    {
        public List<CardVSM> sections { get; set; } = new List<CardVSM>();
    }
    public class productionCardVSM
    {
        public string Name { get; set; }
        public List<Section> sections { get; set; } = new List<Section>();
    }

    public class findCardVSM
    {
        public List<productionCardVSM> cardVSMs { get; set; }  = new List<productionCardVSM>();
    }



    

    [Route("api/cardVSM")]
    public class CardVSMController : Controller
    {
        private readonly Context _context;
        private Manufacture _manufacture;

        public CardVSMController(Context context, Manufacture manufacture) //Manufacture manufacture
        {
            _context = context;
            _manufacture = manufacture;
        }

        [HttpGet("view")]
        public IActionResult ViewCardsVSM()
        {

           List<DataAllCardVSM> cards =  _manufacture.GetAllCards();

           var json = JsonConvert.SerializeObject(new
           {
                operations = cards
           });

            return Ok(new
            {
                message = "Hello world",
                cards = cards,
            });
        }


        [HttpPost("get")]
        public IActionResult Get([FromBody] Order order)
        {
            var card = new findCardVSM();
            card.cardVSMs = new List<productionCardVSM>();
            var findCard = _manufacture.FindCard(order);


              foreach (var simCard in findCard)
              {
                  var objNew = new productionCardVSM();
                  objNew.Name = simCard._product.Name;
                  objNew.sections = new List<Section>();


                  var section = new Section();

                  int CountEtaps = 1;

                  foreach (CardVSM item in simCard._card)
                  {
                      if ((int)item.EtapNumeric == CountEtaps)
                      {
                        section.sections.Add(item);
                      }
                      else
                      {
                        objNew.sections.Add(section);
                        section = new Section();
                        CountEtaps++;
                        section.sections.Add(item);
                      
                      
                      }
                      

                  }
                objNew.sections.Add(section);

                card.cardVSMs.Add(objNew);

              }



            return Ok(card);
        }


    }
}

/*
 * 1
 * 2
 * 3
 * 
 * 1
 * 2
 * 2
 * 3
 * 
 */

