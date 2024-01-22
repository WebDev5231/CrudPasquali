using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Dapper;
using System.Data;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public UsuariosController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        private void OpenConnection()
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
        }

        private void CloseConnection()
        {
            if (_dbConnection.State != ConnectionState.Closed)
            {
                _dbConnection.Close();
            }
        }

        [HttpPost("insert")]
        public IActionResult Insert([FromBody] Cadastro cadastro)
        {
            try
            {
                if (cadastro == null)
                {
                    return BadRequest("Dados inválidos");
                }

                if (string.IsNullOrWhiteSpace(cadastro.NomeCompleto) ||
                    cadastro.DataNascimento == default ||
                    cadastro.ValorRenda == default ||
                    string.IsNullOrWhiteSpace(cadastro.CPF))
                {
                    return BadRequest("Todos os campos são obrigatórios.");
                }

                OpenConnection();

                string insertQuery = "INSERT INTO Cadastro (NomeCompleto, DataNascimento, ValorRenda, CPF) " +
                                     "VALUES (@NomeCompleto, @DataNascimento, @ValorRenda, @CPF)";

                _dbConnection.Execute(insertQuery, cadastro);

                return Ok("Cadastro realizado com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao realizar cadastro: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
        }

        [HttpDelete("delete/{recordId}")]
        public IActionResult Delete(long recordId)
        {
            try
            {
                OpenConnection();

                string checkExistenceQuery = "SELECT COUNT(1) FROM Cadastro WHERE ID = @recordId";
                int recordCount = _dbConnection.QueryFirstOrDefault<int>(checkExistenceQuery, new { recordId });

                if (recordCount == 0)
                {
                    return NotFound($"Registro com ID {recordId} não encontrado");
                }

                string deleteQuery = "DELETE FROM Cadastro WHERE ID = @recordId";
                int affectedRows = _dbConnection.Execute(deleteQuery, new { recordId });

                if (affectedRows > 0)
                {
                    return Ok("Exclusão realizada com sucesso");
                }
                else
                {
                    return NotFound($"Registro com ID {recordId} não encontrado para exclusão");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao realizar exclusão: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(long id, [FromBody] Cadastro cadastro)
        {
            try
            {
                if (cadastro == null)
                {
                    return BadRequest("Dados inválidos");
                }

                if (string.IsNullOrWhiteSpace(cadastro.NomeCompleto) ||
                    cadastro.DataNascimento == default ||
                    cadastro.ValorRenda == default ||
                    string.IsNullOrWhiteSpace(cadastro.CPF))
                {
                    return BadRequest("Todos os campos são obrigatórios.");
                }

                OpenConnection();

                string updateQuery = "UPDATE Cadastro " +
                                     "SET NomeCompleto = @NomeCompleto, " +
                                     "    DataNascimento = @DataNascimento, " +
                                     "    ValorRenda = @ValorRenda, " +
                                     "    CPF = @CPF " +
                                     "WHERE ID = @Id";

                _dbConnection.Execute(updateQuery, new { Id = id, NomeCompleto = cadastro.NomeCompleto, DataNascimento = cadastro.DataNascimento, ValorRenda = cadastro.ValorRenda, CPF = cadastro.CPF });

                return Ok("Atualização realizada com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao realizar atualização: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                OpenConnection();

                string selectQuery = "SELECT * FROM Cadastro";

                var result = _dbConnection.Query<Cadastro>(selectQuery);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter todos os registros: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
