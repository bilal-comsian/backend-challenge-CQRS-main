using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Keyboards.Queries;

public record GetKeyboardByIdQuery(int Id) : IRequest<Keyboard?>;

public class GetKeyboardByIdQueryHandler : IRequestHandler<GetKeyboardByIdQuery, Keyboard?>
{
    private readonly IKeyboardRepository _repo;

    public GetKeyboardByIdQueryHandler(IKeyboardRepository repo) => _repo = repo;

    public Task<Keyboard?> Handle(GetKeyboardByIdQuery request, CancellationToken cancellationToken)
        => _repo.GetByIdAsync(request.Id);
}