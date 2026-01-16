

namespace MecAppIN.Models
{
   public class OrdemServicos
{
    public int Id{get;set;}

    public DateTime Data{get;set;}=DateTime.Now;
    public string Descricao{get;set;}=string.Empty;

    public string Valor{get;set;}=string.Empty;

    public Clientes cliente{get;set;}=null!;
    public int ClienteID{get;set;}
}
}