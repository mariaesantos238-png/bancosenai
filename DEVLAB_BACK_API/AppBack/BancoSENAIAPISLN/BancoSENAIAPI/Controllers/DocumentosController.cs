using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentosController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
            );

        private static List<Models.DocumentoMetadado>
        _documentoMetadados= new List<Models.DocumentoMetadado>();

        private static int _nextId = 1;

        [HttpPost("uploud/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string extensao = Path.GetExtension(arquivo.FileName);

            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);


            using (var strean = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(strean);
            }

            var documentoMetadados = new Models.DocumentoMetadado
            {
                Id = _nextId++,
                Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            _documentoMetadados.Add(documentoMetadados);


            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }
        [HttpGet("listar/(codigoCliente")]
        public IActionResult ListarDocumentos(int codigoCliente) {
        var documentos = _documentoMetadados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();

            if (!documentos.Any())
            {
                return NotFound("Nenhum documento encontrado!");
            }
            return Ok(documentos);
        }
    }
}
