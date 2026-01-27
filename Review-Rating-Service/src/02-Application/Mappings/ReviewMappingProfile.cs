using AutoMapper;
using Review_Rating_Service.src._01_Domain.Core.Aggregates.Review;
using Review_Rating_Service.src._02_Application.DTOs.Requests;
using Review_Rating_Service.src._02_Application.DTOs.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Review_Rating_Service.src._02_Application.Mappings
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<Review, ReviewResponseDto>()
                .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.ReviewDate.Value))
                .ForMember(dest => dest.AttachmentUrls, opt => opt.MapFrom(src => src.Attachments.Select(a => a.Url).ToList()));

            CreateMap<Rating, RatingResponseDto>();

            CreateMap<CreateReviewRequestDto, Review>().ReverseMap();
        }
    }
}
