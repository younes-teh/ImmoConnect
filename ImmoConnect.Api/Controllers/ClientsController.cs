using AutoMapper;
using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Models.DTO;
using ImmoConnect.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImmoConnect.Api.Controllers
{
    [Authorize]   // token obligatoire
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository clientRepository;
        private readonly IMapper mapper;

        public ClientsController(IClientRepository clientRepository, IMapper mapper)
        {
            this.clientRepository = clientRepository;
            this.mapper = mapper;
        }

        // GET api/clients?search=amine
        // search (facultatif) : cherche dans le nom OU l'email
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var clients = await clientRepository.GetAllAsync(search);
            return Ok(mapper.Map<List<ClientDto>>(clients));
        }

        // GET api/clients/3
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var client = await clientRepository.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            return Ok(mapper.Map<ClientDto>(client));
        }

        // POST api/clients
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            // Règle métier : l'email doit être unique -> 409 Conflict
            if (await clientRepository.EmailExistsAsync(dto.Email))
                return Conflict("Cet email existe déjà");

            var client = mapper.Map<Client>(dto);
            client = await clientRepository.CreateAsync(client);

            var result = mapper.Map<ClientDto>(client);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // DELETE api/clients/3 : réservé à l'Admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await clientRepository.DeleteAsync(id);

            return result switch
            {
                DeleteClientResult.NotFound => NotFound(),
                DeleteClientResult.HasVisites => Conflict("Impossible de supprimer un client qui a des visites"),
                _ => NoContent()   // Deleted -> 204
            };
        }
    }
}