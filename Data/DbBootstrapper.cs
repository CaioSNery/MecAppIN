using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;

namespace MecAppIN.Data
{
    public static class DbBootstrapper
    {
        public static void InicializarBanco()
        {
            using var db = new AppDbContext();

            // 1️⃣ Garante banco
            db.Database.EnsureCreated();

            // 2️⃣ Garante tabela SequenciasOs
            using var conn = db.Database.GetDbConnection();
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS SequenciasOs (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            TipoMotor TEXT NOT NULL,
            UltimoNumero INTEGER NOT NULL
        );";
                cmd.ExecuteNonQuery();
            }

            // =========================
            // GASOLINA
            // =========================
            if (!db.SequenciasOs.Any(s => s.TipoMotor == "Gasolina"))
            {
                int ultimoGasolina;

                var existeGasolina = db.OrdemServicos.Any(o => o.TipoMotor == "Gasolina");

                if (existeGasolina)
                {
                    ultimoGasolina = db.OrdemServicos
                        .Where(o => o.TipoMotor == "Gasolina")
                        .Max(o => o.Id);
                }
                else
                {
                    ultimoGasolina = 666;// início Gasolina
                }

                db.SequenciasOs.Add(new SequenciaOs
                {
                    TipoMotor = "Gasolina",
                    UltimoNumero = ultimoGasolina
                });
            }

            // =========================
            // DIESEL
            // =========================
            if (!db.SequenciasOs.Any(s => s.TipoMotor == "Diesel"))
            {
                int ultimoDiesel;

                var existeDiesel = db.OrdemServicos.Any(o => o.TipoMotor == "Diesel");

                if (existeDiesel)
                {
                    ultimoDiesel = db.OrdemServicos
                        .Where(o => o.TipoMotor == "Diesel")
                        .Max(o => o.Id);
                }
                else
                {
                    ultimoDiesel = 666; // início Diesel
                }

                db.SequenciasOs.Add(new SequenciaOs
                {
                    TipoMotor = "Diesel",
                    UltimoNumero = ultimoDiesel
                });
            }

            db.SaveChanges();
        }

    }


}
