using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.DemoRequests.Commands.Create;

public class CreateDemoRequestCommand : IRequest<CreatedDemoRequestResponse>
{
    public required string CompanyName { get; set; }
    public required string ContactName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Interest { get; set; }
    public string? Message { get; set; }

    public class CreateDemoRequestCommandHandler : IRequestHandler<CreateDemoRequestCommand, CreatedDemoRequestResponse>
    {
        private readonly IDemoRequestRepository _demoRequestRepository;

        public CreateDemoRequestCommandHandler(IDemoRequestRepository demoRequestRepository)
        {
            _demoRequestRepository = demoRequestRepository;
        }

        public async Task<CreatedDemoRequestResponse> Handle(
            CreateDemoRequestCommand request,
            CancellationToken cancellationToken
        )
        {
            DemoRequest demoRequest = new()
            {
                Id = Guid.NewGuid(),
                CompanyName = request.CompanyName.Trim(),
                ContactName = request.ContactName.Trim(),
                Email = request.Email.Trim(),
                Phone = request.Phone.Trim(),
                Interest = request.Interest.Trim(),
                Message = string.IsNullOrWhiteSpace(request.Message) ? null : request.Message.Trim(),
                Status = DemoRequestStatus.New,
            };

            await _demoRequestRepository.AddAsync(demoRequest);

            return new CreatedDemoRequestResponse
            {
                Id = demoRequest.Id,
                Message = "Demo talebiniz alındı. En kısa sürede sizinle iletişime geçeceğiz.",
            };
        }
    }
}

public class CreatedDemoRequestResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
}
