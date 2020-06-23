//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Microsoft.Web.WebSockets;

//namespace Quant_BackTest_Backend.MyWebSocket {
//    public class MyWebSocketHandler : WebSocketHandler {

//        private static WebSocketCollection clients = new WebSocketCollection();

//        public override void OnOpen() {
//            clients.Add(this);
//        }

//        public override void OnMessage(string message) {
//            Send("Echo: " + message);
//        }
//    }
//}