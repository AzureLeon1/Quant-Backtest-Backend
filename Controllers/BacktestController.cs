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
    }
}
