using CmpBE100;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Production.Editors;
using System;
using System.Windows.Forms;


namespace ADSucoremaExtensibilidade
{
    public class UiEditorProducao : EditorSubcontratacaoGeracao
    {

        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            if (KeyCode == Convert.ToInt32(Keys.K))
            {
                // Consulta para encontrar lotes novos
                var query = $@"SELECT artigo, OrdemFabrico AS Lote, '1' AS Activo 
                       FROM GPR_OrdemFabrico
                       WHERE OrdemFabrico NOT IN (SELECT lote FROM ArtigoLote)";
                var lotesNovos = BSO.Consulta(query);

                // Verifica o n�mero de lotes novos
                var numLinhas = lotesNovos.NumLinhas();

                if (numLinhas == 0)
                {
                    // Se n�o houver novos lotes, exibe uma mensagem
                    MessageBox.Show("N�o h� lotes novos para inserir.");
                }
                else
                {
                    // Exibe uma mensagem de in�cio do processo de inser��o
                    MessageBox.Show("Inserindo lotes das OFs criadas...");

                    // Move o cursor para o in�cio do conjunto de resultados
                    lotesNovos.NoInicio();

                    // Percorre cada registro em lotesNovos e insere na tabela ArtigoLote
                    for (int i = 0; i < numLinhas; i++)
                    {
                        // Extrai os dados do registro atual
                        string artigo = lotesNovos.Valor("artigo").ToString();
                        string lote = lotesNovos.Valor("Lote").ToString();
                        string activo = lotesNovos.Valor("Activo").ToString();

                        // Cria a consulta de inser��o para cada lote
                        var insertQuery = $@"
                                    INSERT INTO ArtigoLote (artigo, lote, Activo)
                                    VALUES ('{artigo}', '{lote}', '{activo}')";

                        // Executa a inser��o
                        BSO.DSO.ExecuteSQL(insertQuery);

                        // Avan�a para o pr�ximo registro
                        lotesNovos.Seguinte();
                    }

                    // Exibe uma mensagem de sucesso ao concluir a inser��o
                    MessageBox.Show("Lotes das OFs criadas com sucesso.");
                }
            }
        }

        
        public override void AntesDeGravarDocumentoCompra(ref CmpBEDocumentoCompra DocumentoCompra, ref bool Cancel, ExtensibilityEventArgs e)
        {
            /*EDITAR NAS ENCOMENDAS
            base.AntesDeGravarDocumentoCompra(ref DocumentoCompra, ref Cancel, e);
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            for (var i = 1; i <= numLinhas; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                if (linha != null)
                {
                    MessageBox.Show(linha.Artigo);
                    MessageBox.Show(linha.PrecUnit.ToString());
                    linha.PrecUnit = 99.00;
                    linha.Descricao = "Teste";
                }
            }
            */
        }







    }
}
