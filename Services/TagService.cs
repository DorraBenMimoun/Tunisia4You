using MiniProjet.Models;
using MiniProjet.Repositories;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniProjet.Services
{
    public class TagService
    {
        private readonly TagRepository _tagRepository;

        public TagService(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagPlace>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<TagPlace> GetTagByIdAsync(string id)
        {
            return await _tagRepository.GetByIdAsync(id);
        }

        public async Task CreateTagAsync(TagPlace tag)
        {
            await _tagRepository.CreateAsync(tag);
        }

        public async Task UpdateTagAsync(string id, TagPlace tag)
        {
            await _tagRepository.UpdateAsync(id, tag);
        }

        public async Task DeleteTagAsync(string id)
        {
            await _tagRepository.DeleteAsync(id);
        }
        public async Task<TagPlace?> GetTagByLibelleAsync(string libelle)
        {
            return await _tagRepository.GetByLibelleAsync(libelle);
        }
    }
   
}
