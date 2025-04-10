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


                    DesvalorizaEOF(this.OrdemFabrico.IDOrdemFabrico);


                    var mudarConfirmacao = $@"update GPR_OrdemFabrico
                                            set Confirmada = 0
                                            from GPR_OrdemFabrico
                                            where ordemfabrico='{this.OrdemFabrico.OrdemFabrico}'";
                   // BSO.DSO.ExecuteSQL(mudarConfirmacao);
                    
                    MessageBox.Show("Ordem de fabrico reaberta. Deve reabrir a ordem de fabrico novamente.");

                }

               

            }
        }

        private void DesvalorizaEOF(int iDOrdemFabrico)
        {
            try
            {
                // Obter as Ordens de Fabrico que estão marcadas como valorizadas (CDU_Valorizado = 1)
                // e que tenham documentos do tipo EOF.
                // Ajuste a query de acordo com as colunas/tabelas que sejam necessárias no seu cenário.
                var query = $@"
                SELECT DISTINCT 
                C.TipoDoc,
                C.NumDoc,
                C.Serie,
                C.IDOperadorGPR,
                C.Filial,
                C.IdOrdemFabrico,
                O.OrdemFabrico
            FROM CabecInternos C
            INNER JOIN GPR_OrdemFabrico O ON C.IdOrdemFabrico = O.IDOrdemFabrico 
            WHERE C.TipoDoc = 'EOF'
              AND O.CDU_Valorizado = 1
              AND O.Estado != 5
              AND O.IDOrdemFabrico = {iDOrdemFabrico}
        ";

                var lista = BSO.Consulta(query);
                var numLinhas = lista.NumLinhas();
                if (numLinhas == 0)
                {

                    return;
                }

                lista.Inicio();
                for (int i = 0; i < numLinhas; i++)
                {
                    try
                    {
                        var tipoDoc = lista.DaValor<string>("TipoDoc");
                        var numDoc = lista.DaValor<int>("NumDoc");
                        var serie = lista.DaValor<string>("Serie");
                        var filial = lista.DaValor<string>("Filial");
                        var idOrdemFabrico = lista.DaValor<int>("IdOrdemFabrico");

                        // 1) Eliminar o registo na GPR_OrdemFabricoOutrosCustos
                        var deleteQuery = $@"
                    DELETE FROM GPR_OrdemFabricoOutrosCustos
                     WHERE IDOrdemFabrico = {idOrdemFabrico}
                       AND Descricao = 'Encargo de 30% sobre materiais'
                ";
                        BSO.DSO.ExecuteSQL(deleteQuery);

                        // 2) Voltar a marcar a Ordem de Fabrico como não valorizada
                        var updateQuery = $@"
                    UPDATE GPR_OrdemFabrico
                       SET CDU_Valorizado = 0
                     WHERE IDOrdemFabrico = {idOrdemFabrico}
                ";
                        BSO.DSO.ExecuteSQL(updateQuery);

                    }
                    catch (Exception innerEx)
                    {
                        MessageBox.Show($"Ocorreu um erro ao tentar desvalorizar a Ordem de Fabrico: {innerEx.Message}",
                                        "Erro",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }

                    lista.Seguinte();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao tentar desvalorizar a Ordem de Fabrico: {ex.Message}",
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }



        }

    }
}
