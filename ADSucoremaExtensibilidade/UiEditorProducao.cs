using CmpBE100;
using IntBE100;
using InvBE100;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Production.Editors;
using System;
using System.Text.RegularExpressions;
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

                // Verifica o número de lotes novos
                var numLinhas = lotesNovos.NumLinhas();

                if (numLinhas == 0)
                {
                    // Se não houver novos lotes, exibe uma mensagem
                    MessageBox.Show("Não há lotes novos para inserir.");
                }
                else
                {
                    // Exibe uma mensagem de início do processo de inserção
                    MessageBox.Show("Inserindo lotes das OFs criadas...");

                    // Move o cursor para o início do conjunto de resultados
                    lotesNovos.NoInicio();

                    // Percorre cada registro em lotesNovos e insere na tabela ArtigoLote
                    for (int i = 0; i < numLinhas; i++)
                    {
                        // Extrai os dados do registro atual
                        string artigo = lotesNovos.Valor("artigo").ToString();
                        string lote = lotesNovos.Valor("Lote").ToString();
                        string activo = lotesNovos.Valor("Activo").ToString();

                        // Cria a consulta de inserção para cada lote
                        var insertQuery = $@"
                                    INSERT INTO ArtigoLote (artigo, lote, Activo)
                                    VALUES ('{artigo}', '{lote}', '{activo}')";

                        // Executa a inserção
                        BSO.DSO.ExecuteSQL(insertQuery);

                        // Avança para o próximo registro
                        lotesNovos.Seguinte();
                    }

                    // Exibe uma mensagem de sucesso ao concluir a inserção
                    MessageBox.Show("Lotes das OFs criadas com sucesso.");
                }
            }

        }
        public override void AntesDeGravarDocumentoTransferencia(ref InvBEDocumentoTransf DocumentoTransferencia, ref bool Cancel, ExtensibilityEventArgs e)
        {
            var num = DocumentoTransferencia.LinhasOrigem.NumItens;

            if (num == 0)
            {
                return;
            }

            var linhas = new IntBELinhasDocumentoInterno();
            for (int i = 1; i < num + 1; i++)
            {
                var artigo = DocumentoTransferencia.LinhasOrigem.GetEdita(i).Artigo;
                var quantidades = DocumentoTransferencia.LinhasOrigem.GetEdita(i).Quantidade;
                string lote = DocumentoTransferencia.LinhasOrigem.GetEdita(i).Lote.ToString();

                if (string.IsNullOrWhiteSpace(artigo))
                {
                    continue;
                }
                var artigoSQL = BSO.Base.Artigos.Edita(artigo);

                var linha = new IntBELinhaDocumentoInterno
                {
                    Descricao = artigoSQL.Descricao,
                    Armazem = "A1",
                    Artigo = artigo,
                    Lote = lote,
                    Quantidade = Convert.ToDouble(quantidades),
                    CodigoIva = "23",
                    TaxaIva = 23,
                    Unidade = artigoSQL.UnidadeBase,
                    TipoLinha = "13",
                    PercIncidenciaIVA = 100,
                    DataEntrega = DateTime.Now,
                    ContabilizaTotais = true,
                    PercIvaDedutivel = 100,
                    MovStock = "S",
                    INV_EstadoDestino = "DISP"
                };
                linhas.Add(linha);


            }
          
            var documentoInterno = new IntBEDocumentoInterno
            {
                Tipodoc = "ES",
                Data = DateTime.Now,
                DataVencimento = DateTime.Now.AddDays(30),
                Filial = "000",
                Linhas = linhas,
                Serie = "2025",
                Moeda = BSO.Contexto.MoedaBase,
                Cambio = 1,
                CambioMBase = 1,
                CambioMAlt = 1,
            };
            try
            {
                var doc = BSO.Internos.Documentos.PreencheDadosRelacionados(documentoInterno);
                BSO.Internos.Documentos.Actualiza(doc);
            }
            catch (Exception ex)
            {
                PSO.MensagensDialogos.MostraErro("Erro ao criar o documento interno: " + ex.Message, StdBE100.StdBETipos.IconId.PRI_Critico);
                return;
            }


        }


    }
}
