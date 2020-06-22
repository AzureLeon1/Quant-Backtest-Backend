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

namespace Quant_BackTest_Backend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {

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
        public string Register(UserIns userInfo) {
            var filter = Builders<UserIns>.Filter.Eq("User", userInfo.User);
            var checkUser = _user.Find(filter).FirstOrDefault();
            if (checkUser != null)  // 用户名已经存在
                return "userExist";
            string hashName = NameHashTool.HashGivenString(userInfo.User);
            //HashTool.HashNameAndPassword(hashName, userInfo.Password, out string hashCode);
            //userInfo.Password = hashCode;
            _user.InsertOne(userInfo);
            checkUser = _user.Find(filter).FirstOrDefault();
            if (checkUser == null)
                return "fail";
            else
                return "success";
        }

    }
}
