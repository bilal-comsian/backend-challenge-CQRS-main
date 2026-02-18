using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Keyboards.Commands;

public record UpdateKeyboardCommand(int Id, Keyboard Keyboard) : IRequest<bool>;

public class UpdateKeyboardCommandHandler : IRequestHandler<UpdateKeyboardCommand, bool>
{
    private readonly IKeyboardRepository _repo;

    public UpdateKeyboardCommandHandler(IKeyboardRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateKeyboardCommand request, CancellationToken cancellationToken)
    {
        if (request.Id != request.Keyboard.Id) return false;
        var existing = await _repo.GetByIdAsync(request.Id);
        if (existing is null) return false;
        return await _repo.UpdateAsync(request.Keyboard);
    }
}