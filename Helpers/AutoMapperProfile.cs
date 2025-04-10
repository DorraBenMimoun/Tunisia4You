using AutoMapper;
using MiniProjet.Models;
using MiniProjet.DTOs;

namespace MiniProjet.Helpers
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            // User
            CreateMap<User, UserDTO>();
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>();

            // Place
            CreateMap<Place, PlaceDTO>();
            CreateMap<CreatePlaceDTO, Place>();
            CreateMap<UpdatePlaceDTO, Place>();

            // Liste
            CreateMap<Liste, ListeDTO>();
            CreateMap<CreateListeDTO, Liste>();
            CreateMap<UpdateListeDTO, Liste>();

            // Tag
            CreateMap<TagPlace, TagDTO>();
            CreateMap<CreateTagDTO, TagPlace>();
            CreateMap<UpdateTagDTO, TagPlace>();

            // Review
            CreateMap<Review, ReviewDTO>();
            CreateMap<CreateReviewDTO, Review>();
            CreateMap<UpdateReviewDTO, Review>();
        }
    }
}
