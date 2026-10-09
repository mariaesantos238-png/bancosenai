using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DocumentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly string _caminhoRaiz = Path.Combine(
           Directory.GetCurrentDirectory(),
           "ClienteArquivos"
       );

        public DocumentosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("uploud/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }
            long limiteTamanho = 2 * 1024 * 1024;

            if (arquivo.Length > limiteTamanho)
            {
                return BadRequest("O arquivo não pode ter mais de 2 MB.");
            }
            string extensao = Path.GetExtension(arquivo.FileName).ToLower();

         
            string[] extensoesPermitidas = { ".pdf", ".jpg", ".png" };

            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest("Extensão de arquivo não permitida. Apenas .pdf, .jpg e .png são aceitos.");
            }
            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }


            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);


            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new Models.DocumentoMetadado
            {
                
                Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };
            await _context.DocumentoMetadado.AddAsync(documentoMetadados);

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }
        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarDocumentos(int codigoCliente)
        {
            var documentos = await _context.DocumentoMetadado
                    .Where(d => d.CodigoCliente == codigoCliente)
                    .ToListAsync();

            if (!documentos.Any())
            {
                return NotFound("Nenhum documento encontrado!");
            }
            return Ok(documentos);
        }
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var documento = await _context.DocumentoMetadado
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound("Arquivo não encontrado no servidor.");
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(documento.Caminho);
 

             string nomeArquivo = documento.Name + documento.Extensao;

            return File(fileBytes, "application/octet-stream", nomeArquivo);
        }
        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var documento = await _context.DocumentoMetadado
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _context.DocumentoMetadado.Remove(documento);

            await _context.SaveChangesAsync();

            return Ok("Documento excluído com sucesso.");
        }

    }
}
