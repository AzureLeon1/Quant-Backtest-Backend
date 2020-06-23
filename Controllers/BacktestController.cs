using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System;
using System.Web.Http;
using System.Web.Http.Cors;
using Quant_BackTest_Backend.Models;
using Quant_BackTest_Backend.Helper;
using System.Runtime.InteropServices.ComTypes;
using System.Net.WebSockets;
using System.Web.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Web;

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    public class BacktestController : ApiController {
        private quantEntities ctx = new quantEntities();

        [Route("api/backtest")]
        [HttpPost]
        public object Backtest(object json) {

            var body = JsonConverter.Decode(json);

            var code = body["code"];
            var strategy_id = body["strategy_id"];
            var time = body["time"];
            // generate report path
            var report_path = "";

            

            // 回测


            // 回测成功后，保存到mysql、mongodb、file system

            //ctx.strategy.Add(new_backtest);
            //try {
            //    ctx.SaveChanges();
            //}
            //catch (Exception e) {
            //    return Helper.JsonConverter.Error(410, "新建策略时发生错误");
            //}
            //var data = new {
            //    strategy_id = new_strategy.strategy_id
            //};
            //return Helper.JsonConverter.BuildResult(data);
            return null;

        }



        //[Route("api/backtest")]
        //public async Task ProcessRequest(AspNetWebSocketContext context) {
        //    string connectionId = context.AnonymousID;
        //    string code = context.QueryString["code"];
        //    string strategy_id = context.QueryString["strategy_id"];
        //    string time = context.QueryString["time"];

        //    var socket = context.WebSocket;
        //    while (true) {
        //        var buffer = new ArraySegment<byte>(new byte[1024]);
        //        var receivedResult = await socket.ReceiveAsync(buffer, CancellationToken.None); // 对web socket进行异步接收数据
        //        if (receivedResult.MessageType == WebSocketMessageType.Close) // 客户端关闭连接
        //        {
        //            await socket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
        //            //_resource.FinishOperation(projectId, personId);
        //            break;
        //        }
        //    }
        //}

           


        //private async Task ProcessWebsocketSession(AspNetWebSocketContext context) {
        //    var ws = context.WebSocket;

        //    new Task(() =>
        //    {
        //        var inputSegment = new ArraySegment<byte>(new byte[1024]);

        //        while (true) {
        //            // MUST read if we want the state to get updated...
        //            var result = await ws.ReceiveAsync(inputSegment, CancellationToken.None);

        //            if (ws.State != WebSocketState.Open) {
        //                break;
        //            }
        //        }
        //    }).Start();

        //    while (true) {
        //        if (ws.State != WebSocketState.Open) {
        //            break;
        //        }
        //        else {
        //            byte[] binaryData = { 0xde, 0xad, 0xbe, 0xef, 0xca, 0xfe };
        //            var segment = new ArraySegment<byte>(binaryData);
        //            await ws.SendAsync(segment, WebSocketMessageType.Binary,
        //                true, CancellationToken.None);
        //        }
        //    }
        //}
    }
}
