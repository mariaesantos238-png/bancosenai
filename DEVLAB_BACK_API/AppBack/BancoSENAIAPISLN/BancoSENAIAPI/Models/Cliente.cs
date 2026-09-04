namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        public int CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string Cpf { get; set; }
        public int NumeroAgencia { get; set; }
        public decimal Saldototal { get; set; }
        public string Sexo { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}