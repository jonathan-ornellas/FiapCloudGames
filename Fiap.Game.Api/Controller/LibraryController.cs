using AutoMapper;
using Fiap.Game.Api.Contracts.Request;
using Fiap.Game.Api.Contracts.Response;
using Fiap.Game.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fiap.Game.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private ILibraryService _library;
        private IMapper _mapper;
        public LibraryController(ILibraryService library, IMapper mapper)
        {
            _library = library;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibraryItemResponse>>> List(CancellationToken ct)
        {
            var userId = GetUserId();
            var items = await _library.ListAsync(userId, ct); 
            var resp = _mapper.Map<IEnumerable<LibraryItemResponse>>(items);
            return Ok(resp);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest req, CancellationToken ct)
        {
            var userId = GetUserId();
            await _library.PurchaseAsync(userId, req.GameId, ct);
            return NoContent();
        }

        private Guid GetUserId()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            return Guid.Parse(sub!);
        }
    }
}
