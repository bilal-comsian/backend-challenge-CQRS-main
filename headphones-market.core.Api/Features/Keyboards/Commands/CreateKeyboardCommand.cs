using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Keyboards.Commands;

public record CreateKeyboardCommand(Keyboard Keyboard) : IRequest<Keyboard>;

public class CreateKeyboardCommandHandler : IRequestHandler<CreateKeyboardCommand, Keyboard>
{
    private readonly IKeyboardRepository _repo;

    public CreateKeyboardCommandHandler(IKeyboardRepository repo) => _repo = repo;

    public Task<Keyboard> Handle(CreateKeyboardCommand request, CancellationToken cancellationToken)
        => _repo.AddAsync(request.Keyboard);
}