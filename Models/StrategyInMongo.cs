using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Quant_BackTest_Backend.Models {
    public class StrategyInMongo {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("StrategyId")]
        public int StrategyId { get; set; }

        [BsonElement("Code")]
        public string Code { get; set; }

    }
}