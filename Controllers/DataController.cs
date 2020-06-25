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
using System.IO;
using System.Net.Http.Headers;

namespace Quant_BackTest_Backend.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    public class DataController : ApiController {

        private EngineUtils utils = new EngineUtils();

        [Route("api/data")]
        [HttpPost]
        public object GetData(object json) {
            var body = JsonConverter.Decode(json);

            var user_id = body["user_id"];

            using (var ctx = new quantEntities()) {
                var list_public = ctx.data.Where(a => a.data_type == 0).ToList();

                var list = new List<DataResult>();

                foreach(var p in list_public) {
                    var d = new DataResult(p.data_id, p.data_type, p.data_name, p.start_time, p.end_time, p.data_path);
                    list.Add(d);
                }

                var list_private = ctx.data.Where(a => a.user_id == user_id && a.data_type == 1);

                foreach (var p in list_private) {
                    var d = new DataResult(p.data_id, p.data_type, p.data_name, p.start_time, p.end_time, p.data_path);
                    list.Add(d);
                }

                var data = new {
                    datas = list
                };

                return Helper.JsonConverter.BuildResult(data);

            }
        }
    }
}
