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

    public class StrategyController : ApiController {


        [Route("api/strategy")]
        [HttpPost]
        public object CreateStrategy(object json) {

            var body = JsonConverter.Decode(json);

            var user_id = body["user_id"];
            var strategy_name = body["strategy_name"];
            var create_time = body["create_time"];
            string strategy_hash = "";

            using (var ctx = new quantEntities()) {
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

        [Route("api/all_strategy")]
        [HttpPost]
        public object GetAllStrategy(object json) {

            var body = JsonConverter.Decode(json);

            var user_id = body["user_id"];

            using (var ctx = new quantEntities()) {
                var strategys = ctx.strategy.Where(a => a.user_id == user_id).ToList();
                
                var list = new List<StrategyResult>();

                foreach (var each_strategy in strategys) {
                    // 根据backtest表找最后测试时间
                    var strategy_id = each_strategy.strategy_id;
                    var q = ctx.backtest.Where(a => a.strategy_id == strategy_id);
                    string last_modify_time = "";
                    int times = 0;
                    if (q.Any()) {
                        var target_backtests = q.ToList();
                        last_modify_time = target_backtests.Max(a => a.time);
                        times = target_backtests.Count();
                    }
                    StrategyResult sr = new StrategyResult(
                        each_strategy.strategy_id,
                        each_strategy.strategy_name,
                        each_strategy.create_time,
                        each_strategy.strategy_hash,
                        last_modify_time,
                        times
                        );
                    list.Add(sr);
                    //list.Add(new {
                    //    strategy_id = each_strategy.strategy_id,
                    //    name = each_strategy.strategy_name,
                    //    create_time = each_strategy.create_time,
                    //    strategy_hash = each_strategy.strategy_hash,
                    //    last_modify_time = last_modify_time,
                    //    times = times
                    //}) ;
                }

                var sorted_list = list.OrderByDescending(sr => sr.last_modify_time);

                var data = new {
                    strategys = sorted_list
                };

                return Helper.JsonConverter.BuildResult(data);

            }

            
        }
    }
}
