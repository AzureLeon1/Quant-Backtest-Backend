using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quant_BackTest_Backend.Models;

namespace Quant_BackTest_Backend.Helper {
    public class JsonConverter {
        public static Dictionary<string, string> Decode(object json) {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json.ToString());
        }

        public static object Error(int code, string message) {
            return new {
                code,
                message
            };
        }

        public static object BuildResult(object data, int code = 200, string message = "ok") {
            return new {
                data,
                code,
                message
            };
        }
    }
}