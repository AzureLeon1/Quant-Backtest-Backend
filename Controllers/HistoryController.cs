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
    [RoutePrefix("api/history")]
    public class HistoryController : ApiController
    {

        [HttpPost]
        public object GetHistoryBacktest(object json) {

            var body = JsonConverter.Decode(json);

            int strategy_id = int.Parse(body["strategy_id"]);
            var user_id = body["user_id"];

            using (var ctx = new quantEntities()) {

                List<backtest> bts = ctx.backtest.Where(a => a.strategy_id == strategy_id).ToList();

                List<HistoryResult> list = new List<HistoryResult>();

                foreach (var bt in bts) {
                    HistoryResult hr = new HistoryResult(
                        bt.backtest_id,
                        bt.sy,
                        bt.nsy,
                        bt.hc,
                        bt.xp,
                        bt.report_path,
                        bt.time);
                    list.Add(hr);
                }
                
                
                var ordered_list = list.OrderBy(a => a.time);

                var data = new {
                    history_result = ordered_list
                };
                return Helper.JsonConverter.BuildResult(data);
            }

        }
    }
}
