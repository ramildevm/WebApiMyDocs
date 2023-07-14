using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiMyDocs.Models
{
    public class MongoDBContext
    {
        private readonly MongoClient client;
        private readonly IGridFSBucket gridFS;
        public MongoDBContext() 
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string connectionString = configuration.GetConnectionString("MyDocsMongoDatabase");
            client = new MongoClient(connectionString);
            var db = client.GetDatabase("mydocs-mongodb");
            gridFS = new GridFSBucket(db);
        }
        private ObjectId SaveMethod(string base64String, string filename)
        {
            byte[] fileData = Convert.FromBase64String(base64String);
            var options = new GridFSUploadOptions
            {
                ChunkSizeBytes = 4096,
                Metadata = new BsonDocument
            {
                { "filename", filename }
            }
            };

            using (var uploadStream = gridFS.OpenUploadStream(filename, options))
            {
                uploadStream.Write(fileData, 0, fileData.Length);
                uploadStream.Close();

                return uploadStream.Id;
            }
        }
        public string SaveUpdateBase64File(string base64String, string objectId, string filename)
        {
            if(base64String==null && objectId == null)
                return null;
            else if(base64String!=null&& objectId == null)
            {
                return SaveMethod(base64String, filename).ToString();
            }
            else if(base64String == null && objectId != null)
            {
                if (ObjectId.TryParse(objectId, out ObjectId fileId))
                {
                    var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, fileId);
                    gridFS.Find(filter).ToList().ForEach(fileInfo =>
                    {
                        gridFS.Delete(fileInfo.Id);
                    });
                }return null;
            }
            else
            {
                return UpdateMethod(base64String, objectId).ToString();
            }

        }

        private ObjectId UpdateMethod(string base64String, string objectId)
        {
            var fileId = ObjectId.Parse(objectId);
            byte[] fileData = Convert.FromBase64String(base64String);
            var options = new GridFSUploadOptions
            {
                ChunkSizeBytes = 4096,
                Metadata = new BsonDocument
            {
                { "filename", objectId }
            }
            };

            using (var uploadStream = gridFS.OpenUploadStream(fileId, objectId, options))
            {
                uploadStream.Write(fileData, 0, fileData.Length);
                uploadStream.Close();

                return uploadStream.Id;
            }
        }

        public string GetBase64File(ObjectId fileId)
        {
            var downloadStream = gridFS.OpenDownloadStream(fileId);

            using (var memoryStream = new MemoryStream())
            {
                downloadStream.CopyTo(memoryStream);

                byte[] fileData = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(fileData);

                return base64String;
            }
        }

        public static string GenerateRandomFilename(Guid id)
        {
            string uniqueId = id.ToString("N");
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);

            string filename = $"{timestamp}_{uniqueId}_{randomSuffix}";

            return filename;
        }
    }
}
