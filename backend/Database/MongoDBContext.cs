using Microsoft.EntityFrameworkCore;
using backend.Models.MongoDB;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace backend.Database
{
    public class MongoDBContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AirplaneMongo> Airplanes { get; set; }


        public static MongoDBContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<MongoDBContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AirplaneMongo>().ToCollection("airplanes");
        }
    }
}
