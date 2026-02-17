using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Headphones.Commands;

public record UpdateHeadphoneCommand(int Id, Headphone Headphone) : IRequest<bool>;

public class UpdateHeadphoneCommandHandler : IRequestHandler<UpdateHeadphoneCommand, bool>
{
    private readonly IHeadphoneRepository _repo;

    public UpdateHeadphoneCommandHandler(IHeadphoneRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateHeadphoneCommand request, CancellationToken cancellationToken)
    {
        if (request.Id != request.Headphone.Id) return false;
        var existing = await _repo.GetByIdAsync(request.Id);
        if (existing is null) return false;
        return await _repo.UpdateAsync(request.Headphone);
    }
}