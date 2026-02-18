using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Keyboards.Queries;

public record GetAllKeyboardsQuery() : IRequest<List<Keyboard>>;

public class GetAllKeyboardsQueryHandler : IRequestHandler<GetAllKeyboardsQuery, List<Keyboard>>
{
    private readonly IKeyboardRepository _repo;

    public GetAllKeyboardsQueryHandler(IKeyboardRepository repo) => _repo = repo;

    public Task<List<Keyboard>> Handle(GetAllKeyboardsQuery request, CancellationToken cancellationToken)
        => _repo.GetAllAsync();
}