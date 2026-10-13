using AutoMapper;
using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Models.DTO;
using ImmoConnect.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImmoConnect.Api.Controllers
{
    [Authorize]   // token obligatoire (Admin ou Agent)
    [Route("api/[controller]")]
    [ApiController]
    public class VisitesController : ControllerBase
    {
        private readonly IVisiteRepository visiteRepository;
        private readonly IBienRepository bienRepository;
        private readonly IClientRepository clientRepository;
        private readonly IMapper mapper;

        // 3 repositories : la création d'une visite doit vérifier le bien et le client
        public VisitesController(
            IVisiteRepository visiteRepository,
            IBienRepository bienRepository,
            IClientRepository clientRepository,
            IMapper mapper)
        {
            this.visiteRepository = visiteRepository;
            this.bienRepository = bienRepository;
            this.clientRepository = clientRepository;
            this.mapper = mapper;
        }

        // GET api/visites?bienId=1&clientId=2&statut=Planifiee
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? bienId,
            [FromQuery] int? clientId,
            [FromQuery] StatutVisite? statut)
        {
            var visites = await visiteRepository.GetAllAsync(bienId, clientId, statut);
            return Ok(mapper.Map<List<VisiteDto>>(visites));
        }

        // GET api/visites/4
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var visite = await visiteRepository.GetByIdAsync(id);

            if (visite == null)
                return NotFound();

            return Ok(mapper.Map<VisiteDto>(visite));
        }

        // POST api/visites : 5 règles métier, dans cet ordre
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVisiteDto dto)
        {
            // Règle 1 : le bien existe -> sinon 404
            var bien = await bienRepository.GetByIdAsync(dto.BienId);
            if (bien == null)
                return NotFound("Bien introuvable");

            // Règle 2 : le client existe -> sinon 404
            var client = await clientRepository.GetByIdAsync(dto.ClientId);
            if (client == null)
                return NotFound("Client introuvable");

            // Règle 3 : on ne visite que les biens disponibles -> sinon 400
            if (bien.Statut != StatutBien.Disponible)
                return BadRequest("Ce bien n'est pas disponible à la visite");

            // Règle 4 : la date doit être dans le futur -> sinon 400
            if (dto.DateVisite <= DateTime.UtcNow)
                return BadRequest("La date de visite doit être dans le futur");

            // Règle 5 : pas de visite planifiée à ±60 min sur le même bien -> sinon 409
            if (await visiteRepository.ConflitExisteAsync(dto.BienId, dto.DateVisite))
                return Conflict("Un créneau est déjà réservé pour ce bien à cette heure");

            var visite = mapper.Map<Visite>(dto);
            visite = await visiteRepository.CreateAsync(visite);

            var result = mapper.Map<VisiteDto>(visite);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/visites/4/statut : changer le statut (Planifiee -> Effectuee / Annulee)
        [HttpPut("{id}/statut")]
        public async Task<IActionResult> UpdateStatut(
            [FromRoute] int id,
            [FromBody] UpdateStatutVisiteDto dto)
        {
            var visite = await visiteRepository.UpdateStatutAsync(id, dto.Statut);

            if (visite == null)
                return NotFound();

            return Ok(mapper.Map<VisiteDto>(visite));
        }
    }
}