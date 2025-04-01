using GprBE100;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.CustomCode;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public class ValorizarOF : CustomCode
    {

        public void ExecutaValorizacao()
        {
            try
            {
                StdBE100.StdBELista ListaDeOfs = null;
                Primavera.Platform.Collections.PrimaveraOrderedDictionary ColecaoOfs = new Primavera.Platform.Collections.PrimaveraOrderedDictionary();
                ListaDeOfs = BSO.Consulta("SELECT TOP 100 IDOrdemFabrico FROM GPR_OrdemFabrico WHERE Estado = 5 AND Fechada = 0");


                while (!ListaDeOfs.NoFim())
                {
                    //Adicionar ID da ordem de fabrico à coleção.
                    ColecaoOfs.Add(Guid.NewGuid().ToString(), PSO.Utils.FStr(ListaDeOfs.Valor("IDOrdemFabrico")));
                    ListaDeOfs.Seguinte();
                }
                if (ColecaoOfs.Count > 0)
                {
                    string Avisos = "";
                    bool FechaOrdemFabrico = true;
                    bool AtualizaPrecoEntrada = true;
                    //Executar a valorização para todas as ordens de fabrico existentes na coleção.
                    BSO.Producao.OrdensFabrico.ProcessaValorizacao(ColecaoOfs, FechaOrdemFabrico, ref Avisos, AtualizaPrecoEntrada);

                }
            }
            catch (Exception ex)
            {
                // Captura de exceções e exibição de erro
                MessageBox.Show($"Erro geral na execução da valorização: {ex.Message}", "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
