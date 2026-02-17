using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Headphones.Queries;

public record GetAllHeadphonesQuery() : IRequest<List<Headphone>>;

public class GetAllHeadphonesQueryHandler : IRequestHandler<GetAllHeadphonesQuery, List<Headphone>>
{
    private readonly IHeadphoneRepository _repo;

    public GetAllHeadphonesQueryHandler(IHeadphoneRepository repo) => _repo = repo;

    public Task<List<Headphone>> Handle(GetAllHeadphonesQuery request, CancellationToken cancellationToken)
        => _repo.GetAllAsync();
}