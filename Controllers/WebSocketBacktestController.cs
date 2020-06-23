using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using System.Web.Http.Cors;

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/wsbacktest")]
    public class WebSocketBacktestController : ApiController
    {
        private static List<WebSocket> _sockets = new List<WebSocket>();

        [Route]
        [HttpGet]
        public HttpResponseMessage Connect(string code, string strategy_id, string time) {
            HttpContext.Current.AcceptWebSocketRequest(ProcessRequest);//在服务器端接受Web Socket请求，传入的函数作为Web Socket的处理函数，待Web Socket建立后该函数会被调用，在该函数中可以对Web Socket进行消息收发
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols); //构造同意切换至Web Socket的Response.
        }

        public async Task ProcessRequest(AspNetWebSocketContext context) {
            var socket = context.WebSocket;//传入的context中有当前的web socket对象
            _sockets.Add(socket);//此处将web socket对象加入一个静态列表中

            string connectionId = context.AnonymousID;
            string code = context.QueryString["code"];
            string strategy_id = context.QueryString["strategy_id"];
            string time = context.QueryString["time"];



            double sy = 33.33;
            double nsy = 44.44;
            double hc = 55.55;
            double xp = 66.66;
            double sx = 77.77;
            string text = "{\"sy\":" + sy + ",\"nsy\":" + nsy + ",\"hc\":" + hc + ",\"xp\":" + xp + ",\"sx\":" + sx + "}";
            var bytes = Encoding.UTF8.GetBytes(text);
            var buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);



            //进入一个无限循环，当web socket close是循环结束
            while (true) {
                //var buffer = new ArraySegment<byte>(new byte[1024]);
                var receivedResult = await socket.ReceiveAsync(buffer, CancellationToken.None);//对web socket进行异步接收数据
                if (receivedResult.MessageType == WebSocketMessageType.Close) {
                    await socket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);//如果client发起close请求，对client进行ack
                    _sockets.Remove(socket);
                    break;
                }

                if (socket.State == System.Net.WebSockets.WebSocketState.Open) {
                    string recvMsg = Encoding.UTF8.GetString(buffer.Array, 0, receivedResult.Count);
                    var recvBytes = Encoding.UTF8.GetBytes(recvMsg);
                    var sendBuffer = new ArraySegment<byte>(recvBytes);
                    foreach (var innerSocket in _sockets)//当接收到文本消息时，对当前服务器上所有web socket连接进行广播
                    {
                        if (innerSocket != socket) {
                            await innerSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
        }
    }
}
