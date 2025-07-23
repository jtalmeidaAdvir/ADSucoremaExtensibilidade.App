using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Purchases.Editors;
using StdBE100;
using StdPlatBS100;
using System;
using System.Windows.Forms;
using static BasBE100.BasBETiposGcp;
using static StdPlatBS100.StdBSTipos;
using static System.Windows.Forms.LinkLabel;
using StdPlatBS100;
namespace ADSucoremaExtensibilidade.Purchases
{
    public class UiEditorCompras : EditorCompras
    {

        public double PesoTotal { get; set; }


        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            
            PesoTotal = 0;

            // Verifica o tipo de documento antes de proceder
            if (this.DocumentoCompra.Tipodoc == "VFS" && KeyCode == Convert.ToInt32(Keys.K))
            {

                try
                {
                    var numLinhas = this.DocumentoCompra.Linhas.NumItens;
                   

                    // Loop para calcular PesoTotal somando o peso de cada linha
                    for (var i = 1; i <= numLinhas; i++)
                    {
                        
                        // Obtenha a linha atual e verifique se n�o � null
                        var linhaAtual = this.DocumentoCompra.Linhas.GetEdita(i);

                        if (linhaAtual != null)
                        {
                            
                            var Quantidade = linhaAtual.Quantidade;
                            var PrecoLiquido = linhaAtual.IntrastatMassaLiq;
                            
                            // Calcula o peso para a linha atual
                            var pesoLinha = Quantidade * PrecoLiquido;
                            PesoTotal += pesoLinha;  // Soma ao PesoTotal
                        }
                        else
                        {
                            MessageBox.Show($"A linha {i} � inv�lida e foi ignorada.");
                        }
                    }

                    // Exibe o PesoTotal em uma MessageBox

                    // Passa o PesoTotal ao formul�rio Priform e exibe o formul�rio
                    using (Priform form = new Priform(PesoTotal, this.DocumentoCompra,BSO))
                    {
                        form.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    // Trata erros e exibe uma mensagem de erro
                    MessageBox.Show("Ocorreu um erro ao calcular o peso total: " + ex.Message);
                }
            }


            if (KeyCode == Convert.ToInt32(Keys.A))
            {
                MessageBox.Show("A tecla 'A' foi pressionada. Esta funcionalidade ainda n�o est� implementada.");
                if (this.DocumentoCompra.Tipodoc == "VFS")
                {
                    int totalLinhas = this.DocumentoCompra.Linhas.NumItens;

                    for (int i = 1; i <= totalLinhas; i++)
                    {
                        var linha = this.DocumentoCompra.Linhas.GetEdita(i);

                        if (linha.Artigo != null)
                        {
                            var campoQtdReal = linha.CamposUtil["CDU_QtdReal"];
                            var campoPruReal = linha.CamposUtil["CDU_PruReal"];

                            if (campoQtdReal?.Valor != null && campoPruReal?.Valor != null)
                            {
                                double quantidadeReal = (double)campoQtdReal.Valor;
                                double precoUnitReal = (double)campoPruReal.Valor;
                                double quantidade = linha.Quantidade;

                                if (quantidade != 0)
                                {
                                    linha.PrecUnit = (precoUnitReal * quantidadeReal) / quantidade;
                                }
                            }
                        }
                    }
                    PSO.MensagensDialogos.MostraMensagem(TipoMsg.PRI_Detalhe, "Pre�os unit�rios atualizados com sucesso!");
                }

                e.Handled = true;
            }

        }


        /* public override void AntesDeGravar(ref bool Cancel, ExtensibilityEventArgs e)
         {



             if (this.DocumentoCompra.Tipodoc == "VGTSB")
             {
                     double pesoTotal = 0;

                 try
                 {
                     var numLinhas = DocumentoCompra.Linhas.NumItens;
                     for (int i = 1; i <= numLinhas; i++)
                     {
                         if (DocumentoCompra.Linhas.GetEdita(i).PrecUnit != 0)
                         {
                             ResultMsg Resposta = PSO.MensagensDialogos.MostraMensagem(TipoMsg.PRI_SimNao, "Definir o pre�o como zero?", IconId.PRI_Informativo);
                             if (Resposta == ResultMsg.PRI_Sim)
                             {
                                 DocumentoCompra.Linhas.GetEdita(i).PrecUnit = 0;
                             }
                         }
                     }
                     var numlinhas = DocumentoCompra.Linhas.NumItens;


                     for (int i = 1; i <= numlinhas; i++)
                     {
                         var linha = DocumentoCompra.Linhas.GetEdita(i);

                         if (linha.Artigo != "")
                         {
                             var artigo = BSO.Base.Artigos.Edita(linha.Artigo);

                             if (artigo.IntrastatPesoLiquidoEx == 0)
                             {
                                 // pesoTotal += linha.IntrastatMassaLiq;

                             }
                             else
                             {
                                 linha.IntrastatMassaLiq = artigo.IntrastatPesoLiquidoEx;

                             }

                         }
                     }





                     for (int i = 1;i <= numlinhas; i++)
                     {
                         var linha = DocumentoCompra.Linhas.GetEdita(i);
                         pesoTotal += linha.IntrastatMassaLiq * linha.Quantidade;

                     }


                     StdBELista doc = null;
                     string idordem = null;
                     string artigoServico = null;
                     string descricao = null;



                     try
                     {
                         // Primeira consulta para obter o documento
                         var query = $@"SELECT IDCabec3, IDOrdemFabricoOperacao, * FROM GPR_DocumentosRel 
                    WHERE IDCabec3 = '{DocumentoCompra.ID}'";
                         doc = BSO.Consulta(query);

                         // Se a consulta foi bem-sucedida e retornou dados, obtemos o valor
                         if (!doc.Vazia())
                         {
                             idordem = doc.DaValor<string>("IDOrdemFabricoOperacao");
                         }
                     }
                     catch (Exception ex)
                     {
                         Console.WriteLine($"Erro ao executar a primeira consulta: {ex.Message}");
                         // Continua a execu��o caso ocorra um erro
                     }

                     StdBELista doc2 = null;

                     try
                     {
                         // Segunda consulta para obter dados da ordem de fabrica��o
                         if (!string.IsNullOrEmpty(idordem))
                         {
                             var query2 = $@"SELECT * FROM GPR_OrdemFabricoOperacoes 
                         WHERE IDOrdemFabricoOperacao = '{idordem}'";
                             doc2 = BSO.Consulta(query2);

                             // Se a consulta foi bem-sucedida e retornou dados, obtemos o valor
                             if (!doc2.Vazia())
                             {
                                 artigoServico = doc2.Valor("ArtigoServico");
                             }
                         }
                     }
                     catch (Exception ex)
                     {
                         Console.WriteLine($"Erro ao executar a segunda consulta: {ex.Message}");
                         // Continua a execu��o caso ocorra um erro
                     }

                     StdBELista doc3 = null;

                     try
                     {
                         // Terceira consulta para obter a descri��o do artigo de servi�o
                         if (!string.IsNullOrEmpty(artigoServico))
                         {
                             var query3 = $@"SELECT * FROM Artigo 
                         WHERE Artigo = '{artigoServico}'";
                             doc3 = BSO.Consulta(query3);

                             // Se a consulta foi bem-sucedida e retornou dados, obtemos o valor
                             if (!doc3.Vazia())
                             {
                                 descricao = doc3.Valor("Descricao");
                             }
                         }
                     }
                     catch (Exception ex)
                     {
                         Console.WriteLine($"Erro ao executar a terceira consulta: {ex.Message}");
                         // Continua a execu��o caso ocorra um erro
                     }

                     var comen = $@"Peso Total: {Math.Round(pesoTotal, 2).ToString()}Kg";

                     if (DocumentoCompra.CargaDescarga.ATDocCodeID == "")
                     {

                         for (var i = 1; i <= numlinhas; i++)
                         {
                             var linha = DocumentoCompra.Linhas.GetEdita(i);
                             if (linha.Artigo != "")
                             {
                                 var descri = linha.Descricao;
                                 if (!linha.Descricao.Contains($" - Servi�o: {descricao}"))
                                 {
                                     linha.Descricao = $"{descri} - Servi�o: {descricao}";
                                 }

                             }
                         }
                     }

                     else
                     {

                         comen = $@"- Peso Total: {Math.Round(pesoTotal, 2).ToString()}Kg; - Servi�o: {descricao}";
                     }



                     MessageBox.Show(comen);
                     var comentario = comen;


                     bool comentarioExistente = false;

                     // Percorre as linhas do documento de compra para localizar um coment�rio existente.
                     for (var i = 1; i <= numlinhas; i++)
                     {
                         var linha = DocumentoCompra.Linhas.GetEdita(i);

                         // Verifica se a linha � do tipo coment�rio (TipoLinha "60").
                         if (linha != null && linha.TipoLinha == "60")
                         {

                             // Verifica se a descri��o da linha cont�m o c�digo do artigo.
                             if (linha.Descricao.Contains("Peso Total:"))
                             {
                                 // Atualiza a descri��o do coment�rio existente com o novo conte�do.
                                 linha.Descricao = comentario;
                                 comentarioExistente = true;
                                 break; // Encerra a busca, pois o coment�rio j� foi atualizado.
                             }
                         }
                     }

                     // Caso nenhum coment�rio correspondente tenha sido encontrado, adiciona uma nova linha de coment�rio.
                     if (!comentarioExistente)
                     {
                         BSO.Compras.Documentos.AdicionaLinhaEspecial(
                             this.DocumentoCompra,
                             compTipoLinhaEspecial.compLinha_Comentario, // Define o tipo da linha como coment�rio
                             numlinhas, // Adiciona na �ltima linha dispon�vel
                             comentario  // Insere o coment�rio com a descri��o e peso total
                         );
                     }


                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show($"Ocorreu um erro ao processar as consultas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }





                 base.AntesDeGravar(ref Cancel, e);

             }
         }*/

        public override void AntesDeGravar(ref bool Cancel, ExtensibilityEventArgs e)
        {
            if (this.DocumentoCompra.Tipodoc == "VGTSB")
            {
                double pesoTotal = 0;

                try
                {
                    // Define pre�o como zero ap�s confirma��o do usu�rio
                    DefinirPrecoComoZeroNaConfirmacao();

                    // Atualiza IntrastatMassaLiq com base em IntrastatPesoLiquidoEx do artigo
                    AtualizarMassaL�quidaIntrastat();

                    // Calcula o peso total
                    pesoTotal = CalcularPesoTotal();

                    // Consulta para obter os dados da ordem de fabrica��o
                    string descricao = ObterDescricaoDoServicoDeEncomenda(out bool orderExists);

                    // Monta o coment�rio com o peso total e descri��o de servi�o (se existir)
                    var comentario = ConstruirComent�rio(pesoTotal, descricao, orderExists);

                    //MessageBox.Show(comentario);

                    // Insere ou atualiza o coment�rio no documento de compra
                    InserirOuAtualizarComent�rio(comentario);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocorreu um erro ao processar as consultas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                base.AntesDeGravar(ref Cancel, e);
            }

           /*if (this.DocumentoCompra.Tipodoc == "VFS")
            {
                bool precosVaziosEncontrados = false;

                for (int i = 1; i <= this.DocumentoCompra.Linhas.NumItens; i++)
                {
                    var linha = this.DocumentoCompra.Linhas.GetEdita(i);
                    if (linha != null && linha.PrecUnit == 0)
                    {
                        precosVaziosEncontrados = true;
                        break;
                    }
                }

                if (precosVaziosEncontrados)
                {
                    // Mensagem informando que � obrigat�rio atualizar
                    MessageBox.Show(
                        "Existem linhas com Pre�o Unit�rio vazio.\n" +
                        "� obrigat�rio atualizar os pre�os antes de gravar.\n" +
                        "Clique em OK para atualizar os pre�os.",
                        "Pre�o Unit�rio Vazio - Atualiza��o Obrigat�ria",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // Atualiza os pre�os de todas as linhas com pre�o zero
                    for (int i = 1; i <= this.DocumentoCompra.Linhas.NumItens; i++)
                    {
                        var linha = this.DocumentoCompra.Linhas.GetEdita(i);
                        if (linha != null && linha.PrecUnit == 0)
                        {
                            double quantidadeReal = Convert.ToDouble(linha.CamposUtil["CDU_QtdReal"].Valor);
                            double precoUnitReal = Convert.ToDouble(linha.CamposUtil["CDU_PruReal"].Valor);
                            double quantidade = linha.Quantidade;

                            if (quantidade != 0)
                            {
                                linha.PrecUnit = (precoUnitReal * quantidadeReal) / quantidade;
                            }
                        }
                    }

                    // Agora que atualizou os pre�os, deixa gravar
                    Cancel = false; // garantia que n�o cancela
                }
            }*/

        }


        // Fun��o para definir pre�o como zero ap�s confirma��o do usu�rio
        private void DefinirPrecoComoZeroNaConfirmacao()
        {
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            for (int i = 1; i <= numLinhas; i++)
            {
                if (DocumentoCompra.Linhas.GetEdita(i).PrecUnit != 0)
                {
                    ResultMsg resposta = PSO.MensagensDialogos.MostraMensagem(TipoMsg.PRI_SimNao, "Definir o pre�o como zero?", IconId.PRI_Informativo);
                    if (resposta == ResultMsg.PRI_Sim)
                    {
                        DocumentoCompra.Linhas.GetEdita(i).PrecUnit = 0;
                    }
                }
            }
        }

        // Fun��o para atualizar IntrastatMassaLiq baseado no peso l�quido do artigo
        private void AtualizarMassaL�quidaIntrastat()
        {
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            for (int i = 1; i <= numLinhas; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                if (!string.IsNullOrEmpty(linha.Artigo))
                {
                    var artigo = BSO.Base.Artigos.Edita(linha.Artigo);
                    if (artigo.IntrastatPesoLiquidoEx != 0)
                    {
                        linha.IntrastatMassaLiq = artigo.IntrastatPesoLiquidoEx;
                    }
                }
            }
        }

        // Fun��o para calcular o peso total do documento de compra
        private double CalcularPesoTotal()
        {
            double pesoTotal = 0;
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            for (int i = 1; i <= numLinhas; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                pesoTotal += linha.IntrastatMassaLiq * linha.Quantidade;
            }
            return pesoTotal;
        }

        // Fun��o para obter a descri��o do servi�o de uma ordem de fabrica��o
        private string ObterDescricaoDoServicoDeEncomenda(out bool orderExists)
        {
            string descricao = null;
            orderExists = false;

            try
            {
                string idOrdem = ObterIdDaEncomenda();
                if (!string.IsNullOrEmpty(idOrdem))
                {
                    string artigoServico = ObterServicoDaEncomenda(idOrdem);
                    if (!string.IsNullOrEmpty(artigoServico))
                    {
                        descricao = ObterDescricaoDoServico(artigoServico);
                        orderExists = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter descri��o do servi�o: {ex.Message}");
            }

            return descricao;
        }

        // Fun��o auxiliar para obter o ID da ordem
        private string ObterIdDaEncomenda()
        {
            string idOrdem = null;
            var query = $@"SELECT IDCabec3, IDOrdemFabricoOperacao FROM GPR_DocumentosRel 
                   WHERE IDCabec3 = '{DocumentoCompra.ID}'";
            var doc = BSO.Consulta(query);

            if (!doc.Vazia())
            {
                idOrdem = doc.DaValor<string>("IDOrdemFabricoOperacao");
            }

            return idOrdem;
        }

        // Fun��o auxiliar para obter o artigo de servi�o da ordem
        private string ObterServicoDaEncomenda(string idOrdem)
        {
            string artigoServico = null;
            var query = $@"SELECT ArtigoServico FROM GPR_OrdemFabricoOperacoes 
                   WHERE IDOrdemFabricoOperacao = '{idOrdem}'";
            var doc = BSO.Consulta(query);

            if (!doc.Vazia())
            {
                artigoServico = doc.DaValor<string>("ArtigoServico");
            }

            return artigoServico;
        }

        // Fun��o auxiliar para obter a descri��o do artigo de servi�o
        private string ObterDescricaoDoServico(string artigoServico)
        {
            string descricao = null;
            var query = $@"SELECT Descricao FROM Artigo WHERE Artigo = '{artigoServico}'";
            var doc = BSO.Consulta(query);

            if (!doc.Vazia())
            {
                descricao = doc.DaValor<string>("Descricao");
            }

            return descricao;
        }

        // Fun��o para construir o coment�rio com peso total e descri��o do servi�o
        private string ConstruirComent�rio(double pesoTotal, string descricao, bool orderExists)
        {
            string peso = $"Peso Total: {Math.Round(pesoTotal, 2)}Kg";
            if (DocumentoCompra.CargaDescarga.ATDocCodeID == "")
            {
                AdicionarServicoAsDescricoesDaLinha(descricao);
                return peso;
            }
            else
            {
                return $"{peso}; - Servi�o: {descricao}";
            }
        }

        // Fun��o para adicionar a descri��o de servi�o nas linhas do documento
        private void AdicionarServicoAsDescricoesDaLinha(string descricao)
        {
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            for (var i = 1; i <= numLinhas; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                if (!string.IsNullOrEmpty(linha.Artigo) && !linha.Descricao.Contains($" - Servi�o: {descricao}"))
                {
                    linha.Descricao += $" - Servi�o: {descricao}";
                }
            }
        }

        // Fun��o para inserir ou atualizar o coment�rio no documento de compra
        private void InserirOuAtualizarComent�rio(string comentario)
        {
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            bool comentarioExistente = false;

            // Verifica se j� existe uma linha de coment�rio com o peso total
            for (var i = 1; i <= numLinhas; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                if (linha?.TipoLinha == "60" && linha.Descricao.Contains("Peso Total:"))
                {
                    linha.Descricao = comentario;
                    comentarioExistente = true;
                    break;
                }
            }

            // Adiciona uma nova linha de coment�rio se n�o houver um existente
            if (!comentarioExistente)
            {
                BSO.Compras.Documentos.AdicionaLinhaEspecial(
                    this.DocumentoCompra,
                    compTipoLinhaEspecial.compLinha_Comentario,
                    numLinhas,
                    comentario
                );
            }
        }

        //-----------------------------------------------------------------------------------------------------------//

        
        public override void ValidaLinha(int NumLinha, ExtensibilityEventArgs e)
        {
            if (this.DocumentoCompra.Tipodoc == "VFS")
            {
                var linha = this.DocumentoCompra.Linhas.GetEdita(NumLinha);
                if (linha != null && linha.Artigo != null)
                {
                    var qtdRealObj = linha.CamposUtil["CDU_QtdReal"]?.Valor;
                    var pruRealObj = linha.CamposUtil["CDU_PruReal"]?.Valor;

                    if (qtdRealObj != null && pruRealObj != null)
                    {
                        double quantidadeReal = (double)qtdRealObj;
                        double precoUnitReal = (double)pruRealObj;
                        double quantidade = linha.Quantidade;

                        if (quantidade != 0)
                        {
                            linha.PrecUnit = (precoUnitReal * quantidadeReal) / quantidade;
                        }
                    }
                }
            }
        }


    }
}
