using AutoMapper;
using HGV.Warhorn.Api.Data;
using HGV.Warhorn.Api.Transfer;

namespace HGV.Warhorn.Api.Profiles
{
    public class PuzzleProfile : Profile
    {
        public PuzzleProfile()
	    {
		    CreateMap<PuzzleEntity, PuzzleDTO>();
            CreateMap<PuzzleDTO, PuzzleEntity>();

            CreateMap<PuzzleVoteDTO, PuzzleVoteEntity>();
            CreateMap<PuzzleVoteEntity, PuzzleVoteDTO>();
	    }
    }
}
