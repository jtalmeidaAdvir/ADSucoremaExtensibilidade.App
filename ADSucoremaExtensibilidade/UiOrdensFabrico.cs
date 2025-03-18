using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Production.Editors;
using System;
using System.Windows.Forms;


namespace ADSucoremaExtensibilidade
{
    public class UiOrdensFabrico : EditorOrdensFabrico
    {
        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            if (KeyCode == Convert.ToInt32(Keys.A))
            {

                var query = $@"SELECT Estado,IDOrdemFabrico,* FROM GPR_OrdemFabrico where ordemfabrico='{this.OrdemFabrico.OrdemFabrico}'";
                var estado = BSO.Consulta(query);
                var query2 = $@"SELECT Estado,SubContratacao,IDOrdemFabricoOperacao,* 
                                FROM GPR_OrdemFabricoOperacoes
                                where IDOrdemFabrico = '{estado.DaValor<int>("IDOrdemFabrico")}'";

                var estado2 = BSO.Consulta(query2);

                var numLinhas = estado2.NumLinhas();

                var idOrdemFabrico = estado.DaValor<int>("IDOrdemFabrico");

                if (estado.DaValor<string>("Estado") != "2")
                {
                    estado2.Inicio();
                    for (int i = 0; i < numLinhas; i++)
                    {
                        var IDOrdemFabricoOperacao = estado2.DaValor<int>("IDOrdemFabricoOperacao");


                            var mudarEstado2 = $@"UPDATE GPR_OrdemFabricoOperacoes
                                                SET Estado = 7
                                                WHERE IDOrdemFabrico = '{idOrdemFabrico}'
                                                AND IDOrdemFabricoOperacao = '{IDOrdemFabricoOperacao}'";
                            BSO.DSO.ExecuteSQL(mudarEstado2);
     
                        
                        estado2.Seguinte();
                    }


                    var mudarEstado = $@"update GPR_OrdemFabrico
                                set estado='2', Fechada=0
                                from GPR_OrdemFabrico
                                where ordemfabrico='{this.OrdemFabrico.OrdemFabrico}'";

                    BSO.DSO.ExecuteSQL(mudarEstado);

                    
                    var mudarConfirmacao = $@"update GPR_OrdemFabrico
                                            set Confirmada = 0
                                            from GPR_OrdemFabrico
                                            where ordemfabrico='{this.OrdemFabrico.OrdemFabrico}'";
                   // BSO.DSO.ExecuteSQL(mudarConfirmacao);
                    
                    MessageBox.Show("Ordem de fabrico reaberta. Deve reabrir a ordem de fabrico novamente.");

                }

               

            }
        }

    }
}
