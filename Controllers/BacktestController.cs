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
using Quant_BackTest_Backend.BackTestEngine;

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    public class BacktestController : ApiController {

        private EngineUtils utils = new EngineUtils();

        string common_path = @"C:\Users\leon\NET_FINAL\strategy_backtest\examples";

        // 只用于存储code
        [Route("api/backtest")]
        [HttpPost]
        public object Backtest(object json) {
            
            var body = JsonConverter.Decode(json);

            var code = body["code"];
            var strategy_id = body["strategy_id"];
            var time = body["time"];
            var user_id = body["user_id"];
            // generate report path
            var report_path = "";

            using (var ctx = new quantEntities()) {
                DateTime dt = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss",
                                System.Globalization.CultureInfo.InvariantCulture);
                string new_time = dt.ToString("yyyyMMddHHmmss");
                string path = common_path + @"\" + user_id;
                string file = new_time + ".py";
                utils.saveFile(code, path, file);
                //utils.copyFile(path + @"\" + file, common_path + @"\" + file);

                var data = new {
                    save_file = "ok"
                };
                return Helper.JsonConverter.BuildResult(data);
            }
        }

        [HttpDelete]
        public object DeleteBacktest(int id) {
            using (var ctx = new quantEntities()) {
                backtest b = ctx.backtest.Find(id);
                if(b != null) {
                    ctx.backtest.Remove(b);
                    ctx.SaveChanges();
                }

            }
            var data = new {
                id = id
            };
            return Helper.JsonConverter.BuildResult(data);
        }


    }
}
