using MecAppIN.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MecAppIN.Data
{
    public static class DbBootstrapper
    {
        public static void InicializarBanco(int ultimaOsExistente)
        {
            using var db = new AppDbContext();

            //  Cria banco e tabelas se não existir
            db.Database.EnsureCreated();

            //  Se já existir alguma OS, NÃO mexe em nada
            if (db.OrdemServicos.Any())
                return;

            //  Ajusta a sequência para começar após a última OS real
            AjustarSequenciaOs(ultimaOsExistente);
        }

        private static void AjustarSequenciaOs(int ultimaOsExistente)
        {
            using var connection = new SqliteConnection("Data Source=oficina.db");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO sqlite_sequence (name, seq)
                VALUES ('OrdemServicos', $seq)
                ON CONFLICT(name) DO UPDATE SET seq = $seq;
            ";

            cmd.Parameters.AddWithValue("$seq", ultimaOsExistente);
            cmd.ExecuteNonQuery();
        }
    }
}
