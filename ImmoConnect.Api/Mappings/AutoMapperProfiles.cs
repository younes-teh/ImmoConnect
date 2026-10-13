using AutoMapper;
using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Models.DTO;

namespace ImmoConnect.Api.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Bien, BienDto>();
            CreateMap<CreateBienDto, Bien>();
            CreateMap<UpdateBienDto, Bien>();

            CreateMap<Client, ClientDto>();
            CreateMap<CreateClientDto, Client>();

            CreateMap<Visite, VisiteDto>();
            CreateMap<CreateVisiteDto, Visite>();
        }
    }
}