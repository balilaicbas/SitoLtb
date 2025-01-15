using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SitoLtb.Data
{
    public class Dati : Controller
    {

        private readonly string _connectionString;
        public Dati(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }


        [HttpPost]
        public IActionResult Submit(string nome, string cognome, string tipologiaIscrizione)
        {
            try
            {
                // Connessione a SQL Server
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Query di inserimento
                    string query = "INSERT INTO Soci (nome, cognome, tipologiaIscrizione) VALUES (@nome, @cognome, @tipologiaIscrizione)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar) { Value = nome });
                        cmd.Parameters.Add(new SqlParameter("@cognome", SqlDbType.VarChar) { Value = cognome });
                        cmd.Parameters.Add(new SqlParameter("@tipologiaIscrizione", SqlDbType.VarChar) { Value = tipologiaIscrizione });


                        cmd.ExecuteNonQuery();
                    }
                }

                // Successo
                return RedirectToAction("InserimentoDatiSoci");
            }
            catch (Exception ex)
            {
                // Errore
                return Content($"Errore: {ex.Message}");
            }
        }
        [Route("/Dati/InserimentoSoci")]
        public IActionResult InserimentoDatiSoci()
        {
            return View();
        }
    }
}

