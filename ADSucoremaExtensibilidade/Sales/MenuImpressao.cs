using System;
using System.Windows.Forms;

// Referências PRIMAVERA
using VndBE100;
using ErpBS100;

namespace ADSucoremaExtensibilidade.Sales
{
    public partial class MenuImpressao : Form
    {
        private VndBEDocumentoVenda DocumentoVenda;
        private ErpBS BSO;
        private StdPlatBS100.StdBSInterfPub PSO;

        public MenuImpressao(VndBEDocumentoVenda documentoVenda, ErpBS bSO, StdPlatBS100.StdBSInterfPub pSO)
        {
            InitializeComponent();

            DocumentoVenda = documentoVenda;
            BSO = bSO;
            PSO = pSO;

            
        }

        // Botão para gerar PDF diretamente
        private void btnGerarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                // Definir o valor do mapa baseado na seleção do ComboBox
                string mapa;
                if (cb_Mapa.SelectedItem != null)
                {
                    // Verifica qual foi o item selecionado no ComboBox e define o mapa
                    if (cb_Mapa.SelectedItem.ToString() == "SUCOREMA - Doc. Orçamento (PT)")
                    {
                        mapa = "DOCORC1";
                    }
                    else if (cb_Mapa.SelectedItem.ToString() == "SUCOREMA - Doc. Orçamento (Inglês)")
                    {
                        mapa = "DOCORC2";
                    }
                    else
                    {
                        // Caso não haja uma correspondência, podemos lançar um erro ou definir um mapa padrão
                        MessageBox.Show("Seleção de mapa inválida.");
                        return;
                    }
                }
                else
                {
                    // Se não houver item selecionado no ComboBox
                    MessageBox.Show("Selecione um mapa.");
                    return;
                }

                // Novo nome, conforme solicitado
                string nome = "Orçamentos OR " + DocumentoVenda.Tipodoc + "." + DocumentoVenda.Serie + "-" + DocumentoVenda.NumDoc;

                // Define o caminho completo do arquivo PDF com o nome gerado
                string caminhoPDF = @"C:\Mapas\" + nome + ".pdf";  // Usa o nome para gerar o caminho completo

                // Obtém o número de vias selecionado no NumericUpDown
                int numeroVias = (int)numerovias.Value;

                // Geramos o PDF
                bool resultado = BSO.Vendas.Documentos.ImprimeDocumento(
                    DocumentoVenda.Tipodoc,
                    DocumentoVenda.Serie,
                    DocumentoVenda.NumDoc,
                    DocumentoVenda.Filial,
                    numeroVias,             // número de vias
                    mapa,                   // nome do mapa
                    false,                  // visualizar (false = não abre janela do PRIMAVERA)
                    caminhoPDF              // Usa o novo caminho com o nome gerado
                );

                if (resultado)
                {
                    // Se quiseres abrir o PDF logo de seguida:
                    System.Diagnostics.Process.Start(caminhoPDF);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ocorreu um problema ao gerar o PDF.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }



        // Botão para pré-visualizar num Form com CrystalReportViewer
        /*private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                // Nome do mapa (ou ficheiro RPT)
                string nomeMapa = DocumentoVenda.MapaImpressao;

                // Exemplo: se "nomeMapa" for apenas o nome do layout, podes ter de construir o caminho completo do .rpt
                // Por ex: string caminhoRelatorio = @"C:\Mapas\PRIMAVERA\" + nomeMapa + ".rpt";
                // Se o "nomeMapa" for já o caminho completo, basta passá-lo diretamente.

                string caminhoRelatorio = @"C:\Program Files\PRIMAVERA\SG100\Mapas\EV\NOVOS\" + nomeMapa + ".rpt";

                // Instanciar o Form de preview
                FormPreview form = new FormPreview();
                // Carregar o relatório
                form.CarregarRelatorio(caminhoRelatorio,BSO, PSO);

                // Mostrar o form (modal)
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao pré-visualizar: " + ex.Message);
            }
        }*/
    }
}