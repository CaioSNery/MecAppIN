
namespace MecAppIN.Models
{
    public class Orcamentos
    {
        public int Id{get;set;}

        public DateTime Data{get;set;}=DateTime.Now;

        public string Descricao{get;set;}=string.Empty;

        public string ValorEstimado{get;set;}=string.Empty;


    }
}