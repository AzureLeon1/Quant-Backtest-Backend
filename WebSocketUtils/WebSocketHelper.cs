using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Quant_BackTest_Backend.WebSocketUtils {
    public class WebSocketHelper {

        // 把 string 转成 websocket 可以发送的 bytes buffer
        public static ArraySegment<byte> string2byteBuffer(string s) {
            var bytes = Encoding.UTF8.GetBytes(s);
            var buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
            return buffer;
        }

        
    }
}