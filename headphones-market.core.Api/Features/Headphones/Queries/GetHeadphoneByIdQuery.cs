using System.Threading;
using System.Threading.Tasks;
using MediatR;
using headphones_market.core.Api.Data;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Features.Headphones.Queries;

public record GetHeadphoneByIdQuery(int Id) : IRequest<Headphone?>;

public class GetHeadphoneByIdQueryHandler : IRequestHandler<GetHeadphoneByIdQuery, Headphone?>
{
    private readonly IHeadphoneRepository _repo;

    public GetHeadphoneByIdQueryHandler(IHeadphoneRepository repo) => _repo = repo;

    public Task<Headphone?> Handle(GetHeadphoneByIdQuery request, CancellationToken cancellationToken)
        => _repo.GetByIdAsync(request.Id);
}