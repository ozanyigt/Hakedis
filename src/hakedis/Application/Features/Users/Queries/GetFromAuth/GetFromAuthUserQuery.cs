using Application.Common.Authorization;
using Application.Services.CurrentUser;
using Application.Services.Repositories;
using Application.Features.Users.Queries.GetById;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;

namespace Application.Features.Users.Queries.GetFromAuth;

public class GetFromAuthUserQuery : IRequest<GetByIdUserResponse>, IAuthenticatedRequest
{
    public class GetFromAuthUserQueryHandler : IRequestHandler<GetFromAuthUserQuery, GetByIdUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetFromAuthUserQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetByIdUserResponse> Handle(
            GetFromAuthUserQuery request,
            CancellationToken cancellationToken
        )
        {
            if (!_currentUserService.UserId.HasValue)
            {
                throw new AuthorizationException("You are not authenticated.");
            }

            User? user = await _userRepository.GetAsync(
                predicate: item => item.Id == _currentUserService.UserId.Value,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            if (user == null)
            {
                throw new AuthorizationException("User not found.");
            }

            return _mapper.Map<GetByIdUserResponse>(user);
        }
    }
}
