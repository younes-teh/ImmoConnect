using AutoMapper;
using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Models.DTO;
using ImmoConnect.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImmoConnect.Api.Controllers
{
    [Authorize]   // toutes les actions exigent un token valide (Admin ou Agent)
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

        // GET api/biens?ville=Rabat&type=Appartement&prixMax=2000000
        // Les 3 filtres sont facultatifs (nullable) : absent = pas de filtre
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? ville,
            [FromQuery] TypeBien? type,
            [FromQuery] decimal? prixMax)
        {
            var biens = await bienRepository.GetAllAsync(ville, type, prixMax);

            // Liste d'entités -> liste de DTO (VisitesCount calculé par AutoMapper)
            return Ok(mapper.Map<List<BienDto>>(biens));
        }

        // GET api/biens/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var bien = await bienRepository.GetByIdAsync(id);

            if (bien == null)
                return NotFound();   // 404

            return Ok(mapper.Map<BienDto>(bien));
        }

        // POST api/biens
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBienDto dto)
        {
            // DTO -> entité, puis enregistrement en base
            var bien = mapper.Map<Bien>(dto);
            bien = await bienRepository.CreateAsync(bien);

            var result = mapper.Map<BienDto>(bien);

            // 201 Created + en-tête Location pointant vers GetById
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/biens/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBienDto dto)
        {
            var bien = mapper.Map<Bien>(dto);
            var updated = await bienRepository.UpdateAsync(id, bien);

            if (updated == null)
                return NotFound();   // le bien n'existe pas

            return Ok(mapper.Map<BienDto>(updated));
        }

        // DELETE api/biens/5 : réservé à l'Admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await bienRepository.DeleteAsync(id);

            // switch expression : chaque résultat du repository -> un code HTTP
            return result switch
            {
                DeleteBienResult.NotFound => NotFound(),
                DeleteBienResult.HasVisites => Conflict("Impossible de supprimer un bien qui a des visites"),
                _ => NoContent()   // Deleted -> 204
            };
        }
    }
}