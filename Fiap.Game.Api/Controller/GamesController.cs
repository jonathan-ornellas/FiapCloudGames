using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fiap.Game.Api.Contracts.Request;
using Fiap.Game.Api.Contracts.Response;
using Fiap.Game.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Api.Controller
{

    [ApiController]
    [Route("api/[controller]")]

    public class GamesController: ControllerBase
    {
        private IGameService _game;
        private IMapper _mapper;
        public GamesController(IGameService game, IMapper mapper)
        {

            _game = game;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GameResponse>>> GetAll(CancellationToken ct)
        {
            var query = _game.ListActive(); 
            var list = await query.ProjectTo<GameResponse>(_mapper.ConfigurationProvider).ToListAsync(ct);
            return Ok(list);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateGameRequest req, CancellationToken ct)
        {
            var game = _mapper.Map<Domain.Entities.Game>(req);
            var id = await _game.CreateAsync(game, ct);
            return CreatedAtAction(nameof(GetAll), new { id }, new { id });
        }

        [HttpPut("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        {
            await _game.ActivateAsync(id, true, ct);
            return NoContent();
        }

        [HttpPut("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            await _game.ActivateAsync(id, false, ct);
            return NoContent();
        }
    }
}
