using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CarteiraController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var carteiras = await _context.Carteira.ToListAsync();

            return Ok(carteiras);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Carteira novaCarteira)
        {

            if (await _context.Carteira.AnyAsync(
               c => c.NumeroCarteira == novaCarteira.NumeroCarteira))
            {
                return BadRequest(new
                {
                    message = "Este número de carteira já existe."
                });
            }

            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new
                {
                    message = "O apetite da carteira não pode ser negativo."
                });

         
            if (novaCarteira.ApetiteCarteira == 0)
                novaCarteira.ApetiteCarteira = 1000000;

            await _context.Carteira.AddAsync(novaCarteira);
            await _context.SaveChangesAsync();

            return Created("", novaCarteira);
        }

        [HttpGet("{numero}")]
        public async Task<IActionResult> ConsultarPorNumero(int numero)
        {
            var carteira = await _context.Carteira
               .FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteira == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

            return Ok(carteira);
        }

      
        [HttpPut("{numero}")]
        public async Task<IActionResult> Alterar(
            int numero,
            [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = await _context.Carteira.FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteiraExistente == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

       
            if (carteiraAtualizada.ApetiteCarteira < 0)
                return BadRequest(new
                {
                    message = "O apetite da carteira não pode ser negativo."
                });

            carteiraExistente.NomeCarteira =
                carteiraAtualizada.NomeCarteira;

            carteiraExistente.ApetiteCarteira =
             carteiraAtualizada.ApetiteCarteira;

            await _context.SaveChangesAsync();


            return NoContent();
        }

      
        [HttpDelete("{numero}")]
        public async Task<IActionResult> Excluir(int numero)
        {
            var carteira = await _context.Carteira.FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteira == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

            _context.Carteira.Remove(carteira);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Carteira excluída com sucesso."
            });
        }
    }
}
