using CRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public List<Cadastro> DadosDaApi { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            var apiEndpoint = "https://localhost:7166/api/Usuarios/get-all";

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    DadosDaApi = JsonConvert.DeserializeObject<List<Cadastro>>(content);
                }
                else
                {
                    _logger.LogError($"Falha ao chamar a API. Status Code: {response.StatusCode}");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            try
            {
                var apiEndpoint = $"https://localhost:7166/api/Usuarios/delete/{id}";

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var response = await httpClient.DeleteAsync(apiEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        SuccessMessage = "Exclusão realizada com sucesso";
                        return RedirectToPage();
                    }
                    else
                    {
                        _logger.LogError($"Falha ao chamar a API de delete. Status Code: {response.StatusCode}");
                        return new JsonResult($"Erro ao excluir o registro: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao chamar a API de delete: {ex.Message}");
                return new JsonResult($"Erro ao excluir o registro: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostEditarAsync(Cadastro cadastro)
        {
            try
            {
                var apiEndpoint = $"https://localhost:7166/api/Usuarios/update/{cadastro.ID}";

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(cadastro), Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync(apiEndpoint, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        SuccessMessage = "Edição realizada com sucesso";
                        return RedirectToPage();
                    }
                    else
                    {
                        _logger.LogError($"Falha ao chamar a API de edição. Status Code: {response.StatusCode}");
                        return new JsonResult($"Erro ao editar o registro: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao chamar a API de edição: {ex.Message}");
                return new JsonResult($"Erro ao editar o registro: {ex.Message}");
            }
        }
    }
}
