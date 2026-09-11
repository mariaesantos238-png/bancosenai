using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private static List<Carteira> _carteiras = new List<Carteira>();

        
        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_carteiras);
        }

     
        [HttpPost]
        public IActionResult Cadastrar([FromBody] Carteira novaCarteira)
        {
        
            if (_carteiras.Any(c => c.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new
                {
                    message = "Este número de carteira já existe."
                });

           
            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new
                {
                    message = "O apetite da carteira não pode ser negativo."
                });

         
            if (novaCarteira.ApetiteCarteira == 0)
                novaCarteira.ApetiteCarteira = 1000000;

            _carteiras.Add(novaCarteira);

            return Created("", novaCarteira);
        }

        [HttpGet("{numero}")]
        public IActionResult ConsultarPorNumero(int numero)
        {
            var carteira = _carteiras.FirstOrDefault(
                c => c.NumeroCarteira == numero
            );

            if (carteira == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

            return Ok(carteira);
        }

      
        [HttpPut("{numero}")]
        public IActionResult Alterar(
            int numero,
            [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = _carteiras.FirstOrDefault(
                c => c.NumeroCarteira == numero
            );

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

            return NoContent();
        }

      
        [HttpDelete("{numero}")]
        public IActionResult Excluir(int numero)
        {
            var carteira = _carteiras.FirstOrDefault(
                c => c.NumeroCarteira == numero
            );

            if (carteira == null)
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });

            _carteiras.Remove(carteira);

            return Ok(new
            {
                message = "Carteira excluída com sucesso."
            });
        }
    }
}
