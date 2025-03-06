using MiniProjet.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Repositories
{
    public class TagRepository : IRepository<TagPlace>
    {
        private readonly IMongoCollection<TagPlace> _tags;

        public TagRepository(IMongoDatabase database)
        {
            _tags = database.GetCollection<TagPlace>("Tag");
            CreateIndexes();

        }
        private void CreateIndexes()
        {
            var indexKeysDefinition = Builders<TagPlace>.IndexKeys.Ascending(tag => tag.Libelle);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<TagPlace>(indexKeysDefinition, indexOptions);

            _tags.Indexes.CreateOne(indexModel);
        }
        public async Task<List<TagPlace>> GetAllAsync()
        {
            return await _tags.Find(tag => true).ToListAsync();
        }
        public async Task<TagPlace?> GetByLibelleAsync(string libelle)
        {
            return await _tags.Find(tag => tag.Libelle == libelle).FirstOrDefaultAsync();
        }
        public async Task<TagPlace> GetByIdAsync(string id)
        {
            return await _tags.Find(tag => tag.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(TagPlace tag)
        {
            try
            {
                await _tags.InsertOneAsync(tag);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new Exception("Le libellé du tag doit être unique.");
            }
        }

        public async Task UpdateAsync(string id, TagPlace tag)
        {
            var filter = Builders<TagPlace>.Filter.Eq(t => t.Id, id);
            var update = Builders<TagPlace>.Update.Set(t => t.Libelle, tag.Libelle);
            await _tags.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            await _tags.DeleteOneAsync(tag => tag.Id == id);
        }



      
    }
}
