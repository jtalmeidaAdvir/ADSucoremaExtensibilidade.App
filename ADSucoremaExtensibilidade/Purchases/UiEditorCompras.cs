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
                        
                        // Obtenha a linha atual e verifique se não é null
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
                            MessageBox.Show($"A linha {i} é inválida e foi ignorada.");
                        }
                    }

                    // Exibe o PesoTotal em uma MessageBox

                    // Passa o PesoTotal ao formulário Priform e exibe o formulário
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
                MessageBox.Show("A tecla 'A' foi pressionada. Esta funcionalidade ainda não está implementada.");
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
                    PSO.MensagensDialogos.MostraMensagem(TipoMsg.PRI_Detalhe, "Preços unitários atualizados com sucesso!");
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
                             ResultMsg Resposta = PSO.MensagensDialogos.MostraMensagem(TipoMsg.PRI_SimNao, "Definir o preço como zero?", IconId.PRI_Informativo);
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
                         // Continua a execução caso ocorra um erro
                     }

                     StdBELista doc2 = null;

                     try
                     {
                         // Segunda consulta para obter dados da ordem de fabricação
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
                         // Continua a execução caso ocorra um erro
                     }

                     StdBELista doc3 = null;

                     try
                     {
                         // Terceira consulta para obter a descrição do artigo de serviço
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
                         // Continua a execução caso ocorra um erro
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
                                 if (!linha.Descricao.Contains($" - Serviço: {descricao}"))
                                 {
                                     linha.Descricao = $"{descri} - Serviço: {descricao}";
                                 }

                             }
                         }
                     }

                     else
                     {

                         comen = $@"- Peso Total: {Math.Round(pesoTotal, 2).ToString()}Kg; - Serviço: {descricao}";
                     }



                     MessageBox.Show(comen);
                     var comentario = comen;


                     bool comentarioExistente = false;

                     // Percorre as linhas do documento de compra para localizar um comentário existente.
                     for (var i = 1; i <= numlinhas; i++)
                     {
                         var linha = DocumentoCompra.Linhas.GetEdita(i);

                         // Verifica se a linha é do tipo comentário (TipoLinha "60").
                         if (linha != null && linha.TipoLinha == "60")
                         {

                             // Verifica se a descrição da linha contém o código do artigo.
                             if (linha.Descricao.Contains("Peso Total:"))
                             {
                                 // Atualiza a descrição do comentário existente com o novo conteúdo.
                                 linha.Descricao = comentario;
                                 comentarioExistente = true;
                                 break; // Encerra a busca, pois o comentário já foi atualizado.
                             }
                         }
                     }

                     // Caso nenhum comentário correspondente tenha sido encontrado, adiciona uma nova linha de comentário.
                     if (!comentarioExistente)
                     {
                         BSO.Compras.Documentos.AdicionaLinhaEspecial(
                             this.DocumentoCompra,
                             compTipoLinhaEspecial.compLinha_Comentario, // Define o tipo da linha como comentário
                             numlinhas, // Adiciona na última linha disponível
                             comentario  // Insere o comentário com a descrição e peso total
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
                    // Define preço como zero após confirmação do usuário
                    DefinirPrecoComoZeroNaConfirmacao();

                    // Atualiza IntrastatMassaLiq com base em IntrastatPesoLiquidoEx do artigo
                    AtualizarMassaLíquidaIntrastat();

                    // Calcula o peso total
                    pesoTotal = CalcularPesoTotal();

                    // Consulta para obter os dados da ordem de fabricação
                    string descricao = ObterDescricaoDoServicoDeEncomenda(out bool orderExists);

                    // Monta o comentário com o peso total e descrição de serviço (se existir)
                    var comentario = ConstruirComentário(pesoTotal, descricao, orderExists);

                    //MessageBox.Show(comentario);

                    // Insere ou atualiza o comentário no documento de compra
                    InserirOuAtualizarComentário(comentario);

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
                    // Mensagem informando que é obrigatório atualizar
                    MessageBox.Show(
                        "Existem linhas com Preço Unitário vazio.\n" +
                        "É obrigatório atualizar os preços antes de gravar.\n" +
                        "Clique em OK para atualizar os preços.",
                        "Preço Unitário Vazio - Atualização Obrigatória",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // Atualiza os preços de todas as linhas com preço zero
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

                    // Agora que atualizou os preços, deixa gravar
                    Cancel = false; // garantia que não cancela
                }
            }*/

        }


        // Função para definir preço como zero após confirmação do usuário
        private void DefinirPrecoComoZeroNaConfirmacao()
        {
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            for (int i = 1; i <= numLinhas; i++)
            {
                if (DocumentoCompra.Linhas.GetEdita(i).PrecUnit != 0)
                {
                    ResultMsg resposta = PSO.MensagensDialogos.MostraMensagem(TipoMsg.PRI_SimNao, "Definir o preço como zero?", IconId.PRI_Informativo);
                    if (resposta == ResultMsg.PRI_Sim)
                    {
                        DocumentoCompra.Linhas.GetEdita(i).PrecUnit = 0;
                    }
                }
            }
        }

        // Função para atualizar IntrastatMassaLiq baseado no peso líquido do artigo
        private void AtualizarMassaLíquidaIntrastat()
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

        // Função para calcular o peso total do documento de compra
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

        // Função para obter a descrição do serviço de uma ordem de fabricação
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
                Console.WriteLine($"Erro ao obter descrição do serviço: {ex.Message}");
            }

            return descricao;
        }

        // Função auxiliar para obter o ID da ordem
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

        // Função auxiliar para obter o artigo de serviço da ordem
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

        // Função auxiliar para obter a descrição do artigo de serviço
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

        // Função para construir o comentário com peso total e descrição do serviço
        private string ConstruirComentário(double pesoTotal, string descricao, bool orderExists)
        {
            string peso = $"Peso Total: {Math.Round(pesoTotal, 2)}Kg";
            if (DocumentoCompra.CargaDescarga.ATDocCodeID == "")
            {
                AdicionarServicoAsDescricoesDaLinha(descricao);
                return peso;
            }
            else
            {
                return $"{peso}; - Serviço: {descricao}";
            }
        }

        // Função para adicionar a descrição de serviço nas linhas do documento
        private void AdicionarServicoAsDescricoesDaLinha(string descricao)
        {
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            for (var i = 1; i <= numLinhas; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                if (!string.IsNullOrEmpty(linha.Artigo) && !linha.Descricao.Contains($" - Serviço: {descricao}"))
                {
                    linha.Descricao += $" - Serviço: {descricao}";
                }
            }
        }

        // Função para inserir ou atualizar o comentário no documento de compra
        private void InserirOuAtualizarComentário(string comentario)
        {
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            bool comentarioExistente = false;

            // Verifica se já existe uma linha de comentário com o peso total
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

            // Adiciona uma nova linha de comentário se não houver um existente
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
