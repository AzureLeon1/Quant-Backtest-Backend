using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Quant_BackTest_Backend.Models;
using Quant_BackTest_Backend.Helper;

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/Login")]
    public class LoginController : ApiController
    {
        private readonly IMongoCollection<UserIns> _user;
        //private readonly SecurityMaintainLib.SecurityOperatorClass HashTool;
        private readonly NameHash.HashOperator NameHashTool;

        public LoginController() {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("quant");
            _user = database.GetCollection<UserIns>("user");
            NameHashTool = new NameHash.HashOperator();
            //HashTool = new SecurityMaintainLib.SecurityOperatorClass();
        }

        [HttpPost]
        public string Login(object json) {

            var body = JsonConverter.Decode(json);

            var user_id = body["user"];
            var passord = body["password"];

            using (var ctx = new quantEntities()) {
                var q = ctx.user.Where(_user => _user.user_id == user_id);
                if (!q.Any()) {
                    return "fail";
                }
                user user = q.Single();
                if (user.password.Equals(passord)) {
                    return "success";
                }
                else {
                    return "fail";
                }

                //var filter = Builders<UserIns>.Filter.Eq("User", userInfo.User);
                //Console.WriteLine(userInfo.User);
                //var checkUser = _user.Find(filter).FirstOrDefault();
                //if (checkUser != null && checkUser.Password==userInfo.Password)  // 用户名不存在或密码错误
                //    return "success";
                //else
                //    return "fail";
            }


        }
    }
}
