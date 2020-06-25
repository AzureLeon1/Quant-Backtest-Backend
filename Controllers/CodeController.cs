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
    public class CodeController : ApiController
    {
        private EngineUtils utils = new EngineUtils();

        private readonly IMongoCollection<StrategyInMongo> _strategy_code;

        public CodeController() {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("quant");
            _strategy_code = database.GetCollection<StrategyInMongo>("strategy_code");
        }

        // 获取已经存储再mongodb中的code
        [Route("api/code")]
        [HttpPost]
        public object GetCode(object json) {

            var body = JsonConverter.Decode(json);

            var strategy_id = int.Parse(body["strategy_id"]);

            var filter = Builders<StrategyInMongo>.Filter.Eq("StrategyId", strategy_id);
            var checkCode = _strategy_code.Find(filter).FirstOrDefault();

            if (checkCode != null) {
                var data = new {
                    code = checkCode.Code
                };
                return Helper.JsonConverter.BuildResult(data);
            }   
            else {
                var data = new {
                    code = ""
                };
                return Helper.JsonConverter.BuildResult(data);
            }   
        }

    }
}
