using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClienteController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var clientes = await _context.Cliente.ToListAsync();

            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Cliente novoCliente)
        {
            if (await _context.Cliente.AnyAsync(
              c => c.CodigoCliente == novoCliente.CodigoCliente))
            {
                return BadRequest(new
                {
                    message = "Este código de cliente já existe."
                });
            }

            await _context.Cliente.AddAsync(novoCliente);
            await _context.SaveChangesAsync();

            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(
                    c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });

            return Ok(cliente);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(
            int codigo,
            [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = await _context.Cliente
              .FirstOrDefaultAsync(
                  c => c.CodigoCliente == codigo);

            if (clienteExistente == null)
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });

            clienteExistente.NomeCliente =
                clienteAtualizado.NomeCliente;

            clienteExistente.Cpf =
                clienteAtualizado.Cpf;

            clienteExistente.NumeroAgencia =
                clienteAtualizado.NumeroAgencia;

            clienteExistente.Saldototal =
                clienteAtualizado.Saldototal;

            clienteExistente.Sexo =
                clienteAtualizado.Sexo;

            clienteExistente.Endereco =
                clienteAtualizado.Endereco;

            clienteExistente.Cidade =
                clienteAtualizado.Cidade;

            clienteExistente.Estado =
                clienteAtualizado.Estado;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var cliente = await _context.Cliente
               .FirstOrDefaultAsync(
                   c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });

            _context.Cliente.Remove(cliente);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cliente excluído com sucesso."
            });
        }
    }
}