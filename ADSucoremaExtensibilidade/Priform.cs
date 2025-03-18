using CmpBE100;
using ErpBS100;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using static BasBE100.BasBETiposGcp;
using static System.Windows.Forms.LinkLabel;

namespace ADSucoremaExtensibilidade
{
    public partial class Priform : Form
    {
        
        public double TotalPL { get; set; }
        public double pu = 0;
        public double pl = 0;
        public ErpBS BSO { get; set; }
        List<double> ListaPU { get; set; }

        public double PesoTotal { get; set; }

        public CmpBEDocumentoCompra DocumentoCompra { get; set; }

        public Priform(double pesoTotal, CmpBEDocumentoCompra doc, ErpBS bSO)
        {
            BSO = bSO;
            PesoTotal = pesoTotal;
            DocumentoCompra = doc;
            InitializeComponent();
            // Configura propriedades dos controles NumericUpDown para valores decimais
            ConfigureNumericUpDowns();
            LoadArtigos();

            // Associa os eventos ValueChanged aos métodos de cálculo
            PrecoKGFornecedor.ValueChanged += PrecoKGFornecedor_ValueChanged;
            KGFornecedor.ValueChanged += KGFornecedor_ValueChanged;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            // Calcula automaticamente ao iniciar
            CalculateTotals();
        }
        private void ConfigureNumericUpDowns()
        {
            // Configura propriedades dos NumericUpDown para valores decimais
            PrecoKGFornecedor.DecimalPlaces = 2;
            PrecoKGFornecedor.Increment = 0.01M;
            PrecoKGFornecedor.Minimum = 0;
            PrecoKGFornecedor.Maximum = 10000;

            KGFornecedor.DecimalPlaces = 2;
            KGFornecedor.Increment = 0.01M;
            KGFornecedor.Minimum = 0;
            KGFornecedor.Maximum = 10000;

            KGSucorema.DecimalPlaces = 2;
            KGSucorema.Increment = 0.01M;
            KGSucorema.Minimum = 0;
            KGSucorema.Maximum = 10000;
            KGSucorema.Value = Convert.ToDecimal(PesoTotal);

            TotalFornecedor_TEXT.ReadOnly = true;
            TotalFornecedor_TEXT.Enabled = false;

            TotalSucorema_TEXT.ReadOnly = true;
            TotalSucorema_TEXT.Enabled = false;


            PrecoKGSucorema.DecimalPlaces = 8; // Para precisão maior
            PrecoKGSucorema.ReadOnly = true;
            PrecoKGSucorema.Maximum = 100000;

            RemoveUpDownButtons(PrecoKGFornecedor);
            RemoveUpDownButtons(KGFornecedor);
            RemoveUpDownButtons(KGSucorema);
        }

        private void RemoveUpDownButtons(NumericUpDown numericUpDown)
        {
            // Itera sobre os controles internos do NumericUpDown e esconde os botões
            foreach (Control control in numericUpDown.Controls)
            {
                if (control is Button)
                {
                    control.Visible = false;  // Esconde os botões up/down
                }
            }
        }
        private void LoadArtigos()
        {
            // Limpa os itens do ComboBox antes de carregar novos
            comboBox1.Items.Clear();
            // Carrega os artigos do DocumentoCompra
            for (int i = 1; i <= DocumentoCompra.Linhas.NumItens; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                if (linha != null)
                {
                    var artigo = BSO.Base.Artigos.Edita(linha.Artigo);
                    if (artigo != null)
                    {
                        // Adiciona a descrição juntamente com o código do artigo
                        string itemDescricao = $"{linha.Artigo}"; // Inclui código do artigo
                        if (!comboBox1.Items.Contains(itemDescricao))
                        {
                            comboBox1.Items.Add(itemDescricao);
                        }
                    }
                }
            }

            // Seleciona o primeiro item se houver itens no ComboBox
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }



        private void CalculateTotals()
        {
            // Calcula o TotalFornecedor baseado em Fornecedor KG e Preço KG
            decimal totalFornecedor = PrecoKGFornecedor.Value * KGFornecedor.Value;
            TotalFornecedor_TEXT.Text = totalFornecedor.ToString("0.00");
            totalFornecedor = Math.Round(totalFornecedor, 2);

            // Define o TotalSucorema como o Total do Fornecedor
            decimal totalSucorema = totalFornecedor;
            TotalSucorema_TEXT.Text = totalSucorema.ToString("0.00");

            // Verifica se o KG Sucorema é maior que zero para evitar divisão por zero
            if (KGSucorema.Value > 0)
            {
                // Calcula o Preço por KG do Sucorema com precisão de até 8 casas decimais
                PrecoKGSucorema.Value = TotalSucorema.Value / KGSucorema.Value;
            }
            else
            {
                PrecoKGSucorema.Value = 0;
            }

            var numLinhas = DocumentoCompra.Linhas.NumItens;
            ListaPU = new List<double>();
            pu = 0;
            for (var i = 1; i <= numLinhas; i++)
            {

                // Obtenha a linha atual e verifique se não é null
                var linhaAtual = this.DocumentoCompra.Linhas.GetEdita(i);
                
                if (linhaAtual != null)
                {
                    pu = (double)PrecoKGSucorema.Value * linhaAtual.IntrastatMassaLiq;
                    ListaPU.Add(pu);
                }
                else
                {
                    MessageBox.Show($"A linha {i} é inválida e foi ignorada.");
                }

            }
            string puResultados = string.Join(", ", ListaPU);
            TotalPL = 0;
            List<double> listaPL = new List<double>();
            for (var i = 1; i <= numLinhas; i++)
            {
                var linhaAtual = this.DocumentoCompra.Linhas.GetEdita(i);

                
                pl = ListaPU[i-1] * linhaAtual.Quantidade;
                listaPL.Add(pl);
                TotalPL += pl;
            }
            string plResultados = string.Join(", ", listaPL);
            //TotalSucorema.Value = (decimal)TotalPL;
            
        }

        private void PrecoKGFornecedor_ValueChanged(object sender, EventArgs e)
        {
            // Update TotalFornecedor when PrecoKGFornecedor changes
            CalculateTotals();
        }

        private void KGFornecedor_ValueChanged(object sender, EventArgs e)
        {
            // Update TotalFornecedor when KGFornecedor changes
            CalculateTotals();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (KGFornecedor.Value == 0 || PrecoKGFornecedor.Value == 0)
            {
                MessageBox.Show("Operação interrompida: Por favor, verifique se os campos 'Peso do Fornecedor' e 'Preço por KG' foram preenchidos corretamente.");
                return;
            }

            // Obtém o artigo selecionado
            string artigoSelecionado = comboBox1.SelectedItem.ToString();
            var numLinhas = DocumentoCompra.Linhas.NumItens;
            bool linhaAtualizada = false;

            string artigoCodigo = artigoSelecionado; // Ajuste conforme necessário

            // Obtém a unidade do artigo
            var artigoParaAtualizar = BSO.Base.Artigos.Edita(artigoCodigo);
            string unidadeArtigo = artigoParaAtualizar.UnidadeBase;

            // Define o comentário com base na unidade do artigo
            string comentario;
            if (unidadeArtigo == "UN")
            {
                // Comentário com unidade UN
                comentario = $"Artigo: {artigoCodigo}; Quantidade do fornecedor: {KGFornecedor.Value} UN; Preço por UN: {PrecoKGFornecedor.Value}; Total: {Math.Round(TotalFornecedor.Value, 2)}";
            }
            else
            {
                // Comentário com unidade KG (padrão)
                comentario = $"Artigo: {artigoCodigo}; Valores de fornecedor KG: {KGFornecedor.Value}Kg; Preço KG: {PrecoKGFornecedor.Value}; Total: {Math.Round(TotalFornecedor.Value, 2)}";
            }

            // Atualiza as linhas do documento
            for (var i = 1; i <= numLinhas; i++)
            {
                var linhaAtual = DocumentoCompra.Linhas.GetEdita(i);

                if (linhaAtual != null)
                {
                    var artigo = BSO.Base.Artigos.Edita(linhaAtual.Artigo);
                    if (artigo != null && artigo.Artigo == artigoCodigo)
                    {
                        // Atualiza apenas a linha correspondente ao artigo selecionado
                        linhaAtual.PrecUnit = ListaPU[i - 1];
                        linhaAtualizada = true;
                    }
                }
            }

            // Verifica se a linha correspondente foi atualizada
            if (!linhaAtualizada)
            {
                MessageBox.Show("Nenhuma linha correspondente ao artigo selecionado foi encontrada.");
                return;
            }

            // Atualiza ou adiciona o comentário nas observações do documento
            bool observacaoAtualizada = false;
            string[] observacoesExistentes = DocumentoCompra.Observacoes.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < observacoesExistentes.Length; i++)
            {
                if (observacoesExistentes[i].Contains(artigoCodigo)) // Verifica se a observação contém o código do artigo
                {
                    observacoesExistentes[i] = comentario; // Atualiza a linha do comentário
                    observacaoAtualizada = true;
                    break;
                }
            }

            // Se a observação não foi atualizada, adiciona como nova
            if (!observacaoAtualizada)
            {
                DocumentoCompra.Observacoes += Environment.NewLine + comentario; // Adiciona nova observação
            }
            else
            {
                // Recria a string de observações com as atualizações
                DocumentoCompra.Observacoes = string.Join(Environment.NewLine, observacoesExistentes);
            }

            // Verifica e atualiza o comentário nas linhas do documento
            bool comentarioExistente = false;
            for (var i = 1; i <= numLinhas; i++)
            {
                var linha = DocumentoCompra.Linhas.GetEdita(i);
                if (linha != null && linha.TipoLinha == "60") // TipoLinha "60" é para comentários
                {
                    if (linha.Descricao.Contains(artigoCodigo)) // Verifica se a descrição contém o código do artigo
                    {
                        linha.Descricao = comentario; // Atualiza o comentário existente
                        comentarioExistente = true;
                        break;
                    }
                }
            }

            // Se não existia um comentário, adiciona um novo
            if (!comentarioExistente)
            {
                BSO.Compras.Documentos.AdicionaLinhaEspecial(
                    DocumentoCompra,
                    compTipoLinhaEspecial.compLinha_Comentario,
                    numLinhas, // Adiciona na última linha disponível
                    comentario
                );
            }

            // Atualiza o preço padrão do artigo
            artigoParaAtualizar.PCPadrao = (double)PrecoKGFornecedor.Value;
            BSO.Base.Artigos.Actualiza(artigoParaAtualizar);

            this.Close();
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtém o índice selecionado
            int selectedIndex = comboBox1.SelectedIndex;

            // Verifica se um item está selecionado
            if (selectedIndex >= 0)
            {
                string selectedItem = comboBox1.SelectedItem.ToString();

                // Extrai o código do artigo da string selecionada
                // Aqui assumimos que o código do artigo está contido na descrição
                string artigoCodigo = selectedItem;// selectedItem.Substring(selectedItem.LastIndexOf('-') + 2).Trim(); // Ajuste conforme necessário

                decimal totalPesoLiq = 0; // Variável para acumular o peso líquido total

                // Acessa as linhas do DocumentoCompra
                var numLinhas = DocumentoCompra.Linhas.NumItens;
                for (var i = 1; i <= numLinhas; i++)
                {
                    var linhaAtual = DocumentoCompra.Linhas.GetEdita(i);
                    if (linhaAtual != null)
                    {
                        // Verifica se o artigo atual é o mesmo que o selecionado
                        if (linhaAtual.Artigo == artigoCodigo) // Compara pelo código do artigo
                        {
                            if(linhaAtual.Unidade == "UN")
                            {
                                label8.Text = "UN";
                                
                            }
                            else
                            {
                                label8.Text = "KG";
                            }


                            // Calcula o peso líquido da linha atual (IntrastatMassaLiq * Quantidade)
                            decimal pesoLiqLinha = (decimal)linhaAtual.IntrastatMassaLiq * (decimal)linhaAtual.Quantidade;
                          
                            // Acumula o peso líquido total
                            totalPesoLiq += pesoLiqLinha;
                        }
                    }
                }

                // Atualiza KGSucorema com o total do peso líquido
                KGSucorema.Value = totalPesoLiq;

                // Recalcula os totais para refletir a mudança, se necessário
                CalculateTotals();
            }
        }


        private void TotalSucorema_ValueChanged(object sender, EventArgs e)
        {

        }

        private void TotalFornecedor_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Priform_Load(object sender, EventArgs e)
        {

        }

        private void TotalFornecedor_TEXT_TextChanged(object sender, EventArgs e)
        {

        }

        private void TotalSucorema_TEXT_TextChanged(object sender, EventArgs e)
        {

        }
    }




}
