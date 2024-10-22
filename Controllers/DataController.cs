﻿using MongoDB.Bson.Serialization.Attributes;
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
using FileManagement;

namespace Quant_BackTest_Backend.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    public class DataController : ApiController {

        private EngineUtils utils = new EngineUtils();

        string path = @"C:\Users\leon\NET_FINAL\strategy_backtest\tests\test_data\stock";

        FileManagement.FileOperator _file;


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

        [AllowAnonymous]
        [HttpPost]
        [Route("api/data/upload")]
        public async Task<HttpResponseMessage> Upload()
        {
            var user_id = HttpContext.Current.Request.Params["user_id"];
            var data_name = HttpContext.Current.Request.Params["data_name"];
            var start_time = HttpContext.Current.Request.Params["start_time"];
            var end_time = HttpContext.Current.Request.Params["end_time"];

            if (!Request.Content.IsMimeMultipartContent()) {
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();

            string save_path = "";
            foreach (var stream in filesReadToProvider.Contents) {
                // Getting of content as byte[], picture name and picture type
                var fileBytes = await stream.ReadAsByteArrayAsync();
                var fileName = stream.Headers.ContentDisposition.FileName;
                //var contentType = stream.Headers.ContentType.MediaType;

                if (fileName == null)
                    continue;
                else
                    fileName = fileName.Trim('\"');

                string dir = path + @"\" + user_id;
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                save_path = dir + @"\" + fileName;
                if (File.Exists(save_path)) {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }
                File.WriteAllBytes(save_path, fileBytes);
            }

            var new_data = new data {
                data_type = 1,
                user_id = user_id,
                data_path = save_path,
                data_name = data_name,
                start_time = start_time,
                end_time = end_time
            };
            using (var ctx = new quantEntities()) {
                ctx.data.Add(new_data);
                ctx.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK, new_data.data_id);
        }

        // GET: api/data/download
        [HttpPost]
        [Route("api/data/download")]
        public HttpResponseMessage GetFile(object json) {
            var body = JsonConverter.Decode(json);
            int data_id = int.Parse(body["data_id"]);

            var data_path = "";
            using (var ctx = new quantEntities()) {
                var d = ctx.data.Where(a => a.data_id == data_id).Single();
                data_path = d.data_path;
            }

            if (data_path != "") {
                var stream = new FileStream(data_path, FileMode.Open, FileAccess.Read);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StreamContent(stream)
                };
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                var temp = data_path.Split('\\');
                string file_name = temp[temp.Length - 1];
                response.Content.Headers.ContentDisposition.FileName = file_name;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                return response;
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
            
        }

        [HttpDelete]
        public object DeleteData(int id) {
            using (var ctx = new quantEntities()) {
                data d = ctx.data.Find(id);
                if (d != null) {

                    var data_path = d.data_path;
                    if(File.Exists(data_path)) {
                        File.Delete(data_path);
                    }

                    ctx.data.Remove(d);
                    ctx.SaveChanges();

                    
                    int index = data_path.LastIndexOf('\\');
                    var dir = data_path.Substring(0, index);
                    var file_name = data_path.Substring(index + 1);

                    _file = new FileManagement.FileOperator(dir, "");
                    _file.CleanFiles(file_name);
                }

            }
            var data = new {
                id = id
            };
            return Helper.JsonConverter.BuildResult(data);
        }
    }
}
