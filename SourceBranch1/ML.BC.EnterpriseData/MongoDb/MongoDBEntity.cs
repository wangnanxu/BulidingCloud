using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ML.BC.EnterpriseData.MongoDb
{
    public abstract class MongoDBEntity
    {
        //  _id可由MongoDB自动生成
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
