using AutoMapper;
using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Models.DTO;
using ImmoConnect.Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ImmoConnect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiensController : ControllerBase
    {
        private readonly IBienRepository bienRepository;
        private readonly IMapper mapper;

        public BiensController(IBienRepository bienRepository, IMapper mapper)
        {
            this.bienRepository = bienRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? ville,
            [FromQuery] TypeBien? type,
            [FromQuery] decimal? prixMax)
        {
            var biens = await bienRepository.GetAllAsync(ville, type, prixMax);
            return Ok(mapper.Map<List<BienDto>>(biens));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bien = await bienRepository.GetByIdAsync(id);
            if (bien == null) return NotFound();
            return Ok(mapper.Map<BienDto>(bien));
        }

        // À toi : Create, Update, Delete

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBienDto createBienDto)
        {
            var bienDomain = mapper.Map<Bien>(createBienDto);

            bienDomain = await bienRepository.CreateAsync(bienDomain);

            var bienDTO = mapper.Map<BienDto>(bienDomain);

            return CreatedAtAction(nameof(GetById), new { id = bienDTO.Id }, bienDTO);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBienDto updateBienDto)
        {
            var bienDomain = mapper.Map<Bien>(updateBienDto);

            bienDomain = await bienRepository.UpdateAsync(id, bienDomain);
            if(bienDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<BienDto>(bienDomain));
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await bienRepository.DeleteAsync(id);

            return result switch
            {
                DeleteBienResult.NotFound => NotFound(),
                DeleteBienResult.HasVisites => BadRequest("Impossible de supprimer ce bien : il a des visites."),
                _ => NoContent()
            };
        }
    }
}