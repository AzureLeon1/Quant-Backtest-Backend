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
using NetMQ;
using NetMQ.Sockets;
using Quant_BackTest_Backend.BackTestEngine;
using Quant_BackTest_Backend.Models;
using Quant_BackTest_Backend.WebSocketUtils;
using WordReportModule;
using MongoDB.Driver;

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/wsbacktest")]
    public class WebSocketBacktestController : ApiController
    {

        private static List<WebSocket> _sockets = new List<WebSocket>();
        private EngineUtils utils = new EngineUtils();

        private readonly IMongoCollection<StrategyInMongo> _strategy_code;

        string common_path = @"C:\Users\leon\NET_FINAL\strategy_backtest\examples";
        string report_save_path = @"C:\Users\leon\NET_FINAL\strategy_backtest\word_report";

        public WebSocketBacktestController() {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("quant");
            _strategy_code = database.GetCollection<StrategyInMongo>("strategy_code");
        }

        [Route]
        [HttpGet]
        public HttpResponseMessage Connect(string strategy_id, string time, string user_id) {
            HttpContext.Current.AcceptWebSocketRequest(ProcessRequest);//在服务器端接受Web Socket请求，传入的函数作为Web Socket的处理函数，待Web Socket建立后该函数会被调用，在该函数中可以对Web Socket进行消息收发
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols); //构造同意切换至Web Socket的Response.
        }

        public async Task ProcessRequest(AspNetWebSocketContext context) {
            var socket = context.WebSocket;//传入的context中有当前的web socket对象
            _sockets.Add(socket);//此处将web socket对象加入一个静态列表中

            string connectionId = context.AnonymousID;
            //string code = context.QueryString["code"];
            string strategy_id = context.QueryString["strategy_id"];
            string time = context.QueryString["time"];
            string user_id = context.QueryString["user_id"];

            // backtest item
            int data_id = 1;
            var sy = "";
            var nsy = "";
            var hc = "";
            var xp = "";
            string report_path = "";



            DateTime dt = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss",
                                System.Globalization.CultureInfo.InvariantCulture);
            string new_time = dt.ToString("yyyyMMddHHmmss");
            string path = common_path + @"\" + user_id;
            string file = new_time + ".py";
            //utils.saveFile(code, path, file);
            utils.copyFile(path + @"\" + file, common_path + @"\" + file);



            // 回测


            Task t = new Task(() => {
                string sArguments = common_path + @"\" + file;
                EngineRunner runner = new EngineRunner(user_id);
                runner.RunPythonScript(sArguments);
            });
            t.Start();

            byte[] bytes;
            ArraySegment<byte> buffer;


            var topic = user_id;
            using (var subSocket = new SubscriberSocket()) {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345");
                subSocket.Subscribe(topic);
                System.Diagnostics.Debug.Write("Subscriber socket connecting\n");
                while (true) {
                    string messageTopicReceived = subSocket.ReceiveFrameString();
                    string messageReceived = subSocket.ReceiveFrameString();
                    System.Diagnostics.Debug.Write("receive: " + messageReceived);

                    // 向前端发送
                    bytes = Encoding.UTF8.GetBytes(messageReceived);
                    buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

                    // TODO: 如果输出完成还没有释放，则释放Task

                    if (messageReceived[0].Equals('策')) {  
                        System.Diagnostics.Debug.WriteLine(messageReceived);
                        sy = messageReceived.Substring(6);  // 结尾有%\r\n
                    }
                    if (messageReceived[0].Equals('最')) {   
                        hc = messageReceived.Substring(7);
                    }
                    if (messageReceived[0].Equals('年')) {   
                        nsy = messageReceived.Substring(6);
                    }
                    if (messageReceived[0].Equals('夏')) {   // 退出标志：最后一行输出是夏普比率
                        xp = messageReceived.Substring(6);
                        System.Diagnostics.Debug.WriteLine("Exit Backtest Runner");
                        break;
                    }

                }
            }



            // 回测成功后，保存到mysql、file system
            using (var ctx = new quantEntities()) {
                var new_backtest = new backtest {
                    strategy_id = int.Parse(strategy_id),
                    time = time,
                    data_id = data_id,
                    sy = sy,
                    nsy = nsy,
                    hc = hc,
                    xp = xp,
                    report_path = report_path
                };

                ctx.backtest.Add(new_backtest);
                try {
                    ctx.SaveChanges();
                }
                catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    bytes = Encoding.UTF8.GetBytes("保存回测结果时发生错误");
                    buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }

                // 发送 backtest id
                string backtest_id = new_backtest.backtest_id.ToString();
                var content = WebSocketHelper.string2byteBuffer("backtest_id: " + backtest_id);
                await socket.SendAsync(content, WebSocketMessageType.Text, true, CancellationToken.None);


                // save word report
                var filter = Builders<StrategyInMongo>.Filter.Eq("StrategyId", strategy_id);
                var checkCode = _strategy_code.Find(filter).FirstOrDefault();
                var code = "";
                if (checkCode != null) {
                    code = checkCode.Code;
                }
                string file_path = report_save_path + @"\" + backtest_id + @".docx";
                WordFileOperator.CreateReport(code, time, file_path);


                //进入一个无限循环，当web socket close时循环结束
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
}
