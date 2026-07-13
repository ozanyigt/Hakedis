using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.DailySiteReports.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<DailySiteReportPhoto, DailySiteReportPhotoDto>();
        CreateMap<DailySiteReportWorkforceSnapshot, DailySiteReportWorkforceSnapshotDto>();
        CreateMap<DailySiteReportMaterialLine, DailySiteReportMaterialLineDto>();
        CreateMap<DailySiteReport, DailySiteReportDto>()
            .ForMember(x => x.ProjectName, x => x.MapFrom(y => y.Project.Name))
            .ForMember(x => x.SiteName, x => x.MapFrom(y => y.Site.Name))
            .ForMember(x => x.AuthorName, x => x.MapFrom(y => (y.CreatedByUser.FirstName + " " + y.CreatedByUser.LastName).Trim()))
            .ForMember(x => x.PhotoCount, x => x.MapFrom(y => y.Photos.Count))
            .ForMember(x => x.PostedMaterialCost,
                x => x.MapFrom(y => y.MaterialLines.Sum(line => line.PostedTotalCost ?? 0)));
        CreateMap<DailySiteReport, DailySiteReportListItemDto>()
            .ForMember(x => x.ProjectName, x => x.MapFrom(y => y.Project.Name))
            .ForMember(x => x.SiteName, x => x.MapFrom(y => y.Site.Name))
            .ForMember(x => x.AuthorName, x => x.MapFrom(y => (y.CreatedByUser.FirstName + " " + y.CreatedByUser.LastName).Trim()))
            .ForMember(x => x.PhotoCount, x => x.MapFrom(y => y.Photos.Count));
        CreateMap<IPaginate<DailySiteReport>, GetListResponse<DailySiteReportListItemDto>>();
    }
}
