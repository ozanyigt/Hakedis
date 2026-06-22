using Application.Features.ProgressEntries.Constants;
using Application.Features.ProgressEntries.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.ProgressEntries.Constants.ProgressEntriesOperationClaims;

namespace Application.Features.ProgressEntries.Queries.GetById;

public class GetByIdProgressEntryQuery : IRequest<GetByIdProgressEntryResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdProgressEntryQueryHandler : IRequestHandler<GetByIdProgressEntryQuery, GetByIdProgressEntryResponse>
    {
        private readonly IMapper _mapper;
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ProgressEntryBusinessRules _progressEntryBusinessRules;

        public GetByIdProgressEntryQueryHandler(IMapper mapper, IProgressEntryRepository progressEntryRepository, ProgressEntryBusinessRules progressEntryBusinessRules)
        {
            _mapper = mapper;
            _progressEntryRepository = progressEntryRepository;
            _progressEntryBusinessRules = progressEntryBusinessRules;
        }

        public async Task<GetByIdProgressEntryResponse> Handle(GetByIdProgressEntryQuery request, CancellationToken cancellationToken)
        {
            ProgressEntry? progressEntry = await _progressEntryRepository.GetAsync(predicate: pe => pe.Id == request.Id, cancellationToken: cancellationToken);
            await _progressEntryBusinessRules.ProgressEntryShouldExistWhenSelected(progressEntry);

            GetByIdProgressEntryResponse response = _mapper.Map<GetByIdProgressEntryResponse>(progressEntry);
            return response;
        }
    }
}