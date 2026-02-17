using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;

namespace headphones_market.core.Api.Features.Headphones.Commands;

public record DeleteHeadphoneCommand(int Id) : IRequest<bool>;

public class DeleteHeadphoneCommandHandler : IRequestHandler<DeleteHeadphoneCommand, bool>
{
    private readonly IHeadphoneRepository _repo;

    public DeleteHeadphoneCommandHandler(IHeadphoneRepository repo) => _repo = repo;

    public Task<bool> Handle(DeleteHeadphoneCommand request, CancellationToken cancellationToken)
        => _repo.DeleteAsync(request.Id);
}