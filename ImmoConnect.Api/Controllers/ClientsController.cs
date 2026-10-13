using AutoMapper;
using ImmoConnect.Api.Models.Domain;
using ImmoConnect.Api.Models.DTO;
using ImmoConnect.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImmoConnect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IClientRepository clientRepository;
        public ClientsController(IMapper mapper, IClientRepository clientRepository)
        {
            this.mapper = mapper;
            this.clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var clientDomain = await clientRepository.GetAllAsync(search);

            return Ok(mapper.Map<List<ClientDto>>(clientDomain));
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientDomain = await clientRepository.GetByIdAsync(id);
            if(clientDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<ClientDto>(clientDomain));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto createClientDto)
        {
            if(await clientRepository.EmailExistsAsync(createClientDto.Email))
            {
                return Conflict("Cet email existe déjà");
            }

            var clientDomain = mapper.Map<Client>(createClientDto);

            clientDomain = await clientRepository.CreateAsync(clientDomain);

            var clientDTO = mapper.Map<ClientDto>(clientDomain);

            return CreatedAtAction(nameof(GetById), new { id = clientDTO.Id }, clientDTO);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await clientRepository.DeleteAsync(id);

            return result switch
            {
                DeleteClientResult.NotFound => NotFound(),
                DeleteClientResult.HasVisites => BadRequest("Impossible de supprimer"),
                _ => NoContent()
            };
            
        }
    }
}
