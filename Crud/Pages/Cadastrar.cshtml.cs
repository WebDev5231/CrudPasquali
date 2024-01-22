using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Pages
{
    public class CadastroModel : PageModel
    {
        private readonly ILogger<CadastroModel> _logger;

        public CadastroModel(ILogger<CadastroModel> logger)
        {
            _logger = logger;
        }

        public string StatusMensagem { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Obter os dados diretamente do formulário
                string nomeCompleto = Request.Form["NomeCompleto"];
                string dataNascimentoBrasileira = Request.Form["DataNascimento"];
                string valorRenda = Request.Form["ValorRenda"];
                string cpf = Request.Form["CPF"];

                // Remover as máscaras
                dataNascimentoBrasileira = RemoverMascara(dataNascimentoBrasileira);
                valorRenda = RemoverMascara(valorRenda);
                cpf = RemoverMascara(cpf);

                // Validar os campos obrigatórios
                if (string.IsNullOrEmpty(nomeCompleto) ||
                    string.IsNullOrEmpty(dataNascimentoBrasileira) ||
                    string.IsNullOrEmpty(valorRenda) ||
                    string.IsNullOrEmpty(cpf))
                {
                    ModelState.AddModelError("", "Todos os campos são obrigatórios.");
                    return Page();
                }

                DateTime dataNascimentoAmericana = ConverterData(dataNascimentoBrasileira);

                var cadastroData = new
                {
                    NomeCompleto = nomeCompleto,
                    DataNascimento = dataNascimentoAmericana.ToString("yyyy-MM-dd"), // Formato americano
                    ValorRenda = valorRenda,
                    CPF = cpf
                };

                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:7166/api/Usuarios/insert";

                    string jsonContent = JsonConvert.SerializeObject(cadastroData);

                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        StatusMensagem = "Cadastro realizado com sucesso!";
                        return Page(); // Retorna para a página atual (Cadastrar.cshtml) com a mensagem de sucesso
                    }
                    else
                    {
                        // Se houver um erro na API, trate conforme necessário
                        StatusMensagem = $"Erro na API: {response.StatusCode}";
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                // Lidar com exceções, se necessário
                StatusMensagem = $"Erro ao enviar dados para a API: {ex.Message}";
                return Page();
            }
        }

        private DateTime ConverterData(string dataBrasileira)
        {
            // Tenta converter a data para o formato americano (ano-mês-dia)
            if (DateTime.TryParseExact(dataBrasileira, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataConvertida))
            {
                return dataConvertida;
            }

            // Se a tentativa de conversão falhar, lança uma exceção
            throw new FormatException("Formato de data inválido");
        }

        private string RemoverMascara(string input)
        {
            return input.Replace(".", "").Replace("/", "").Replace("-", "").Replace(",", "");
        }
    }
}
