using System.Security.Cryptography;
using System.Text;

namespace MecAppIN.Helpers
{
    public static class FinanceiroSenhaHelper
    {
        // ALTERE A SENHA AQUI
        private const string SenhaBase = "2026";

        public static bool Validar(string senhaDigitada)
        {
            return GerarHash(senhaDigitada) == GerarHash(SenhaBase);
        }

        private static string GerarHash(string valor)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(valor);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }
}
