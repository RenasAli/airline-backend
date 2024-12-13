﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace backend.Models.MongoDB
{
    [Collection("users")]
    public class UserMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public int MySQLKey {  get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("role")]
        public UserRole Role { get; set; }
    
    }
}
