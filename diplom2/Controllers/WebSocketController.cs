using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using diplom2.Data;
using diplom2.Logic;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace diplom2.Controllers
{
    public class WebSocketController : ControllerBase
    {

        private readonly Context _context;
        private Manufacture _manufacture;

        public WebSocketController(Context context, Manufacture manufacture)
        {
            _context = context;
            _manufacture = manufacture;
        }

        private string getRemouteIp()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }

        [HttpGet("/ws")]
        public async Task Get()
        {

    
             if (HttpContext.WebSockets.IsWebSocketRequest)
            {

                using WebSocket webSocket = await
                                    HttpContext.WebSockets.AcceptWebSocketAsync();

                var remoteIpAddress = getRemouteIp();

                if(webSocket.State == WebSocketState.Open)
                {
                    Console.WriteLine("Get  webSocket.State = Open");
                    await Echo(HttpContext, webSocket);

                    Console.WriteLine("Get Echo = ", remoteIpAddress);
                }

                /*
                if (webSocket.State == WebSocketState.Closed)
                {
                    Console.WriteLine("Get  webSocket.State = Closed");
                }

                if (webSocket.State == WebSocketState.None)
                {
                    Console.WriteLine("Get  webSocket.State = None");
                }

                if (webSocket.State == WebSocketState.Aborted)
                {
                    Console.WriteLine("Get  webSocket.State = Aborted");
                }

                if (webSocket.State == WebSocketState.CloseReceived)
                {
                    Console.WriteLine("Get  webSocket.State = CloseReceived");
                }

                if (webSocket.State == WebSocketState.CloseSent)
                {
                    Console.WriteLine("Get  webSocket.State = CloseSent");
                }

                if (webSocket.State == WebSocketState.Connecting)
                {
                    Console.WriteLine("Get  webSocket.State = Connecting");
                }*/

            }
             else
             {
                 HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
             }
        }


        ///
        [HttpGet("/orderlist")]
        public async Task GetOrderList()
        {


            if (HttpContext.WebSockets.IsWebSocketRequest)
            {

                using WebSocket webSocket = await
                                    HttpContext.WebSockets.AcceptWebSocketAsync();

                var remoteIpAddress = getRemouteIp();

                if (webSocket.State == WebSocketState.Open)
                {
                    await SessionOrderList(HttpContext, webSocket);
                }

            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }




        private async Task SessionOrderList(HttpContext httpContext, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;


            JArray ordersAll = _manufacture.getAllOrders();

            if (ordersAll.Count > 0)
            {
                ThreadPool.QueueUserWorkItem(delegate (object state) { _ = SendOrderListToClientAsync(httpContext, webSocket, ordersAll); });
            }
            else
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Server wont", CancellationToken.None);
            }

            do
            {
 
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                Array.Resize(ref buffer, result.Count);
                var str = Encoding.UTF8.GetString(buffer);
            }
            while (!result.CloseStatus.HasValue);

            Console.WriteLine("exit EchoItem");

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }




        private async Task SendOrderListToClientAsync(HttpContext httpContext, WebSocket webSocket, JArray ordersAll)
        {

            while (webSocket.State == WebSocketState.Open)
            {

                //
                //Get a orders in manafacture
                //


                //var orders = _manufacture.getAllOrdersFromProcess();

                var list = await _context
                           .Order
                           .Select(c => new
                           {
                               c.Id,
                               c.Name,
                               c.OrderRole,
                               c.Priority,
                               c.Simulation,
                               c.TActual,
                               c.TAdd,
                               c.TFuture,
                               c.TPlan,
                               c.TStart,
                               c.TStop,
                               Orders_production = c.Orders_production
                                                   .Select(e => new {
                                                       e.Id,
                                                       e.Priority,
                                                       e.OrderRole,
                                                       e.Quantity,
                                                       e.Simulation,
                                                       e.TActual,
                                                       e.TAdd,
                                                       e.TFuture,
                                                       e.TPlan,
                                                       e.TStart,
                                                       e.TStop,
                                                       e.Production,
                                                       Orders_production_items = e.Orders_production_items
                                                           .Select(k => new
                                                           {
                                                               k.Id,
                                                               k.OrderRole,
                                                               k.Part,
                                                               k.Priority,
                                                               k.Simulation,
                                                               k.TActual,
                                                               k.TDefault,
                                                               k.TFuture,
                                                               k.TStart,
                                                               k.TStop,
                                                               k.Orders_production,
                                                           }).ToList()
                                                   })
                                                   .ToList()
                           })
                           //.Where(o=> o.OrderRole == OrderRole.Work)
                           .AsNoTracking()
                           .ToListAsync();



                List<object> newList = new List<object>();

               

                foreach (var order in list)
                {

                    List<object> cards = new List<object>();

                    foreach (var product in order.Orders_production)
                    {



                        using (var _context = new Context(DBConnect.options))
                        {

                            List<CardVSM> findProductCard = await _context.CardVSM
                                                .Include(o => o.Production)

                                                .Include(o => o.BufferVSM)
                                                     .ThenInclude(o => o.BufferVSMQueue
                                                                                .Where(q => q.BufferRole == BufferRole.Wait)
                                                                                .Where(z=> z.CurrentOrderItems.Orders_production.Order.Id == order.Id)
                                                                                                                    .OrderByDescending(z=> z.CurrentOrderItems.Priority))
                                                        .ThenInclude(o => o.CurrentOrderItems)

                                                 .Include(o => o.EtapVSM)
                                                     .ThenInclude(o => o.EtapSections)
                                                        .ThenInclude(o=> o.CurrentOrderItems)
                                                            .ThenInclude(o=> o.Orders_production)
                                                        /*.ThenInclude(o => o.ArchiveSection.Where(q => q.ArchiveSectionRole == ArchiveSectionRole.Work))
                                                            .ThenInclude(o => o.OrderItem)*/
                                                .Include(o=> o.EtapVSM)
                                                    .ThenInclude(o=> o.EtapSections)
                                                        .ThenInclude(o=> o.User)
                                                .Where(o => o.Production.Id == product.Production.Id)
                                                .AsSplitQuery()
                                                .AsNoTracking()
                                                .ToListAsync();


                            


                            cards.Add(new
                            {
                                name = product.Production.Name,
                                sections = findProductCard
                            });

                        }

                    }



                    var obj = new
                    {
                        order = order,
                        cards = cards
                    };

                    newList.Add(obj);

                }

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                //var str2 = newList.ToString();
                string str2 = JsonSerializer.Serialize(newList, serializeOptions);

                var serverMsg = Encoding.UTF8.GetBytes(str2.ToString());

                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), new WebSocketMessageType(), true, CancellationToken.None);

                newList.Clear();


                await Task.Delay(500);

            }

            Console.WriteLine("EXIT SendOrderListToClientAsync");
        }


        ///




        [HttpGet("/wsItem")]
        public async Task GetItem()
        {


            if (HttpContext.WebSockets.IsWebSocketRequest)
            {

                using WebSocket webSocket = await
                                    HttpContext.WebSockets.AcceptWebSocketAsync();

                var remoteIpAddress = getRemouteIp();

                if (webSocket.State == WebSocketState.Open)
                {
                    Console.WriteLine("Get  webSocket.State = Open");
                    await EchoItem(HttpContext, webSocket);

                    Console.WriteLine("Get Echo = ", remoteIpAddress);
                }

                if (webSocket.State == WebSocketState.Closed)
                {
                    Console.WriteLine("Get  webSocket.State = Closed");
                }

                if (webSocket.State == WebSocketState.None)
                {
                    Console.WriteLine("Get  webSocket.State = None");
                }

                if (webSocket.State == WebSocketState.Aborted)
                {
                    Console.WriteLine("Get  webSocket.State = Aborted");
                }

                if (webSocket.State == WebSocketState.CloseReceived)
                {
                    Console.WriteLine("Get  webSocket.State = CloseReceived");
                }

                if (webSocket.State == WebSocketState.CloseSent)
                {
                    Console.WriteLine("Get  webSocket.State = CloseSent");
                }

                if (webSocket.State == WebSocketState.Connecting)
                {
                    Console.WriteLine("Get  webSocket.State = Connecting");
                }

            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        /* private async Task Echo(HttpContext httpContext, WebSocket webSocket)
         {
             //var buffer = new byte[1024 * 4];
             ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);

             WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
             while (!result.CloseStatus.HasValue)
             {
                 string time = DateTime.Now.ToString("dddd d MMMM yyyy HH:mm:ss");

                 string packet = Encoding.UTF8.GetString(buffer);

                 time += " ) " + packet;

                 var newBuf = new byte[1024 * 4];

                 newBuf = Encoding.UTF8.GetBytes(time);


                 await webSocket.SendAsync(new ArraySegment<byte>(newBuf, 0, newBuf.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                 result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
             }
             await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
         }*/

        private async Task Echo(HttpContext httpContext, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            ThreadPool.QueueUserWorkItem(delegate (object state) { _ = RunAsync(httpContext, webSocket); });

            do
            {

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                Array.Resize(ref buffer, result.Count);
                var str = Encoding.UTF8.GetString(buffer);


                Console.WriteLine("sdsds");
            }
            while (!result.CloseStatus.HasValue);

            Console.WriteLine("exit");

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }



        private async Task EchoItem(HttpContext httpContext, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);



            Array.Resize(ref buffer, result.Count);
           var str = Encoding.UTF8.GetString(buffer);
           JObject json = JObject.Parse(str);

            int id = Int32.Parse((string)json["id"]);

            Order findOrder;
            findOrder = _manufacture.FindOrder(new Order { Id = id });

            if (findOrder != null)
            {
                ThreadPool.QueueUserWorkItem(delegate (object state) { _ = RunItemAsync(httpContext, webSocket, findOrder); });
            }
            else
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Server wont", CancellationToken.None);
            }

            do
            {
                if((webSocket.State == WebSocketState.Closed))
                {
                    break;
                }
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                Array.Resize(ref buffer, result.Count);
                str = Encoding.UTF8.GetString(buffer);



            }
            while (!result.CloseStatus.HasValue);

            Console.WriteLine("exit EchoItem");

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }



        private async Task RunItemAsync(HttpContext httpContext, WebSocket webSocket, Order findOrder)
        {
            int id = findOrder.Id;


            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            while (webSocket.State == WebSocketState.Open)
            {

                findOrder = _manufacture.FindOrder(new Order { Id = id });

                if(findOrder == null)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Server wont", CancellationToken.None);
                    break;
                }

                string str2 = JsonSerializer.Serialize(findOrder, options);


                var serverMsg = Encoding.UTF8.GetBytes(str2.ToString());

                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), new WebSocketMessageType(), true, CancellationToken.None);

                await Task.Delay(500);

            }

            Console.WriteLine("EXIT ITEM");
        }





            private async Task RunAsync(HttpContext httpContext, WebSocket webSocket)
        {
            /* Array.Resize(ref buffer, result.Count);
             var str = Encoding.UTF8.GetString(buffer);
             JObject json = JObject.Parse(str);
             if (json.Count > 0)
             {
                 json["user"] = "Server";
                 json["message"] = "Server: Hello. You said: " + json["message"];
             }*/

            JObject json = new JObject
            {
                {"user", "server" },
                {"message", "Server: Hello."  }
            };


            int count = 0;



            while (webSocket.State == WebSocketState.Open)
            {
                count++;
                var serverMsg = Encoding.UTF8.GetBytes(json.ToString());

                if (count > 5)
                {
                    json["message"] = "Server: Waiting = " + count;

                    await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), new WebSocketMessageType(), true, CancellationToken.None);
                }

                if (count > 10)
                {
                    json["message"] = "Server: Rebut = " + count;

                    await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), new WebSocketMessageType(), true, CancellationToken.None);
                }

                if (count > 15)
                {
                    count = 0;
                    json["message"] = "Server: Hello. = "+count;

                   // await webSocket.CloseAsync( WebSocketCloseStatus.InternalServerError, "Server wont", CancellationToken.None);
                  // break;
                }

             
              
                await Task.Delay(1000);
            }

            Console.WriteLine("Exit thread");
        }
    }
}
