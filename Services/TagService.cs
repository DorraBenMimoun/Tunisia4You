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
        private readonly PlaceRepository _placeRepository;

        public TagService(TagRepository tagRepository, PlaceRepository placeRepository)
        {
            _tagRepository = tagRepository;
            _placeRepository = placeRepository;
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

   
        public async Task<TagPlace?> GetTagByLibelleAsync(string libelle)
        {
            return await _tagRepository.GetByLibelleAsync(libelle);
        }

        public async Task DeleteTagAsync(string id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag != null)
            {
                // Supprimer le tag de tous les lieux avant de le supprimer de la collection Tag
                await _placeRepository.RemoveTagFromAllPlacesAsync(tag.Libelle);

                // Supprimer le tag de la collection des tags
                await _tagRepository.DeleteAsync(id);
            }
        }

    }

}
