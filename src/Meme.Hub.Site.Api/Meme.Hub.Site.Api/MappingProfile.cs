using AutoMapper;
using Meme.Hub.Site.Models;

namespace Meme.Hub.Site.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SubmitSocialsClaimModel, ApprovedSocialsModel>();
            // Add more mappings here
        }
    }
}
