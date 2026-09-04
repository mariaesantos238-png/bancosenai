using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private static List<Cliente> _clientes = new List<Cliente>();

        [HttpGet]
        public IActionResult ListarTodos()
        {
            return Ok(_clientes);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente novoCliente)
        {
            if (_clientes.Any(c => c.CodigoCliente == novoCliente.CodigoCliente))
                return BadRequest(new
                {
                    message = "Este código de cliente já existe."
                });

            _clientes.Add(novoCliente);

            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(
                c => c.CodigoCliente == codigo
            );

            if (cliente == null)
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });

            return Ok(cliente);
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(
            int codigo,
            [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = _clientes.FirstOrDefault(
                c => c.CodigoCliente == codigo
            );

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

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(
                c => c.CodigoCliente == codigo
            );

            if (cliente == null)
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });

            _clientes.Remove(cliente);

            return Ok(new
            {
                message = "Cliente excluído com sucesso."
            });
        }
    }
}