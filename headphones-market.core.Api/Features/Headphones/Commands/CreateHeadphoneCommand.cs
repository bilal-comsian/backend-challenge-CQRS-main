using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Headphones.Commands;

public record CreateHeadphoneCommand(Headphone Headphone) : IRequest<Headphone>;

public class CreateHeadphoneCommandHandler : IRequestHandler<CreateHeadphoneCommand, Headphone>
{
    private readonly IHeadphoneRepository _repo;

    public CreateHeadphoneCommandHandler(IHeadphoneRepository repo) => _repo = repo;

    public Task<Headphone> Handle(CreateHeadphoneCommand request, CancellationToken cancellationToken)
        => _repo.AddAsync(request.Headphone);
}