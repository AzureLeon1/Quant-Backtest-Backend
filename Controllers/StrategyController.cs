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

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    
    public class StrategyController : ApiController
    {

        private quantEntities ctx = new quantEntities();

        [Route("api/strategy")]
        [HttpPost]
        public object CreateStrategy(object json) {
            var body = JsonConverter.Decode(json);

            var user_id = body["user_id"];
            var strategy_name = body["strategy_name"];
            var create_time = body["create_time"];
            string strategy_hash = "";

            var new_strategy = new strategy {
                user_id = user_id,
                strategy_name = strategy_name,
                create_time = create_time,
                strategy_hash = strategy_hash,
            };
            ctx.strategy.Add(new_strategy);
            try {
                ctx.SaveChanges();
            }
            catch (Exception e) {
                return Helper.JsonConverter.Error(410, "新建策略时发生错误");
            }
            var data = new {
                strategy_id = new_strategy.strategy_id
            };
            return Helper.JsonConverter.BuildResult(data);

        }


    }
}
