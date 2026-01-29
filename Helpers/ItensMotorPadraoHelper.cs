using MecAppIN.Enums;
using MecAppIN.Models;
using System.Collections.ObjectModel;

namespace MecAppIN.Helpers
{
    public static class ItensMotorPadraoHelper
    {
        public static ObservableCollection<ItemOrdemServico> CriarItensBiela()
        {
            return new ObservableCollection<ItemOrdemServico>
            {
                new ItemOrdemServico
                {
                    Bloco = EBlocoMotor.Biela,
                    Servico = "Retificar",
                    Quantidade = 0,
                    ValorUnitario = 120,
                    IsPeca = false,
                    ValorEditavel = true
                },
                new ItemOrdemServico
                {
                    Bloco = EBlocoMotor.Biela,
                    Servico = "Acoplar",
                    Quantidade = 0,
                    ValorUnitario = 50,
                    IsPeca = false,
                    ValorEditavel = true
                },
                new ItemOrdemServico
                {
                    Bloco = EBlocoMotor.Biela,
                    Servico = "Embuchar",
                    Quantidade = 0,
                    ValorUnitario = 0,
                    IsPeca = false,
                    ValorEditavel = true
                }
            };
        }

        public static ObservableCollection<ItemOrdemServico> CriarItensBloco()
        {
            return new ObservableCollection<ItemOrdemServico>
            {
                new ItemOrdemServico { Bloco = EBlocoMotor.Bloco, Servico = "Retificar Cilindro", Quantidade = 0, ValorUnitario = 150, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Bloco, Servico = "Encamisar Cilindro", Quantidade = 0, ValorUnitario = 200, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Bloco, Servico = "Brunir Cilindro", Quantidade = 0, ValorUnitario = 50, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Bloco, Servico = "Mandrilhar Mancais", Quantidade = 0, ValorUnitario = 225, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Bloco, Servico = "Facear", Quantidade = 0, ValorUnitario = 400, IsPeca = false, ValorEditavel = true }
            };
        }

        public static ObservableCollection<ItemOrdemServico> CriarItensCabecote()
        {
            return new ObservableCollection<ItemOrdemServico>
            {
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Retificar Sede", Quantidade = 0, ValorUnitario = 12.50M, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Retificar Válvulas", Quantidade = 0, ValorUnitario = 12.50M, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Descarb./Esmer./Montar", Quantidade = 0, ValorUnitario = 21.25M, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Facear", Quantidade = 0, ValorUnitario = 200, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Calibrar Válvulas", Quantidade = 0, ValorUnitario = 25, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Mandrilhar", Quantidade = 0, ValorUnitario = 350, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Substituir Guia", Quantidade = 0, ValorUnitario = 80, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Recondic./ Solda", Quantidade = 0, ValorUnitario = 30, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote, Servico = "Extração de parafuso", Quantidade = 0, ValorUnitario = 70, IsPeca = false, ValorEditavel = true }
            };
        }

        public static ObservableCollection<ItemOrdemServico> CriarItensEixo()
        {
            return new ObservableCollection<ItemOrdemServico>
            {
                new ItemOrdemServico { Bloco = EBlocoMotor.Eixo, Servico = "Retificar Eixo", Quantidade = 0, ValorUnitario = 320, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Eixo, Servico = "Polir Eixo", Quantidade = 0, ValorUnitario = 140, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Eixo, Servico = "Polir Eixo de comando", Quantidade = 0, ValorUnitario = 80, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Eixo, Servico = "Recuperar lateral", Quantidade = 0, ValorUnitario = 180, IsPeca = false, ValorEditavel = true }
            };
        }

        public static ObservableCollection<ItemOrdemServico> CriarItensMotor()
        {
            return new ObservableCollection<ItemOrdemServico>
            {
                new ItemOrdemServico { Bloco = EBlocoMotor.Motor, Servico = "Descarbonizar Motor", Quantidade = 0, ValorUnitario = 100, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Motor, Servico = "Montagem parcial", Quantidade = 0, ValorUnitario = 0, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Motor, Servico = "Montagem completa", Quantidade = 0, ValorUnitario = 0, IsPeca = false, ValorEditavel = true },
                new ItemOrdemServico { Bloco = EBlocoMotor.Motor, Servico = "Serviço de Troca", Quantidade = 0, ValorUnitario = 0, IsPeca = false, ValorEditavel = true }
            };
        }
    }
}
