using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;

namespace headphones_market.core.Api.Features.Keyboards.Commands;

public record DeleteKeyboardCommand(int Id) : IRequest<bool>;

public class DeleteKeyboardCommandHandler : IRequestHandler<DeleteKeyboardCommand, bool>
{
    private readonly IKeyboardRepository _repo;

    public DeleteKeyboardCommandHandler(IKeyboardRepository repo) => _repo = repo;

    public Task<bool> Handle(DeleteKeyboardCommand request, CancellationToken cancellationToken)
        => _repo.DeleteAsync(request.Id);
}