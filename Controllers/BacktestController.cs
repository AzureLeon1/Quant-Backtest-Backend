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

        private readonly IMongoCollection<StrategyInMongo> _strategy_code;

        string common_path = @"C:\Users\leon\NET_FINAL\strategy_backtest\examples";

        public BacktestController() {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("quant");
            _strategy_code = database.GetCollection<StrategyInMongo>("strategy_code");
        }

        // 只用于存储code
        [Route("api/backtest")]
        [HttpPost]
        public object Backtest(object json) {
            
            var body = JsonConverter.Decode(json);

            var code = body["code"];
            var strategy_id = int.Parse(body["strategy_id"]);
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

                // save to mongodb
                //string hashName = NameHashTool.HashGivenString(strategy_id);
                //HashTool.HashNameAndPassword(hashName, userInfo.Password, out string hashCode);
                //userInfo.Password = hashCode;
                var strategy_code = new StrategyInMongo {
                    StrategyId = strategy_id,
                    Code = code
                };
                var filter = Builders<StrategyInMongo>.Filter.Eq("StrategyId", strategy_code.StrategyId);
                var checkCode = _strategy_code.Find(filter).FirstOrDefault();
                if (checkCode != null) {  // 策略代码已经存在
                    var update = Builders<StrategyInMongo>.Update.Set("Code", code);
                    _strategy_code.UpdateOne(filter, update);
                }
                else {
                    _strategy_code.InsertOne(strategy_code);
                }
                //checkCode = _strategy_code.Find(filter).FirstOrDefault();
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
