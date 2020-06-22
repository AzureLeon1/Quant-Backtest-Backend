using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Quant_BackTest_Backend.Models;
using MongoDB.Bson.IO;
using Quant_BackTest_Backend.Helper;

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {
        private quantEntities ctx = new quantEntities();

        private readonly IMongoCollection<UserIns> _user;
        //private readonly SecurityMaintainLib.SecurityOperatorClass HashTool;
        private readonly NameHash.HashOperator NameHashTool;
        //public class UserIns {
        //    [BsonId]
        //    [BsonRepresentation(BsonType.ObjectId)]
        //    public string Id { get; set; }

        //    [BsonElement("user")]
        //    public string User { get; set; }

        //    [BsonElement("password")]
        //    public string Password { get; set; }
        //}

        public RegisterController() {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("quant");
            _user = database.GetCollection<UserIns>("user");
            NameHashTool = new NameHash.HashOperator();
            //HashTool = new SecurityMaintainLib.SecurityOperatorClass();
        }

        [HttpPost]
        public string Register(object json) {


            var body = JsonConverter.Decode(json);

            var user_id = body["User"];
            var password = body["Password"];

            if (ctx.user.Any(_user => _user.user_id == user_id)) {
                return "fail";  // 用户已存在
            }

            var new_user = new user {
                user_id = user_id,
                password = password
            };
            ctx.user.Add(new_user);

            ValidationHelper.safeSaveChanges(ctx);
            return "success";

            //var filter = Builders<UserIns>.Filter.Eq("User", userInfo.User);
            //var checkUser = _user.Find(filter).FirstOrDefault();
            //if (checkUser != null)  // 用户名已经存在
            //    return "userExist";
            //string hashName = NameHashTool.HashGivenString(userInfo.User);
            ////HashTool.HashNameAndPassword(hashName, userInfo.Password, out string hashCode);
            ////userInfo.Password = hashCode;
            //_user.InsertOne(userInfo);
            //checkUser = _user.Find(filter).FirstOrDefault();
            //if (checkUser == null)
            //    return "fail";
            //else
            //    return "success";
        }

    }
}
