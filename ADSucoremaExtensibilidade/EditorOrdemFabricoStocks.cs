using IntBE100;
using StdBE100;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace ADSucoremaExtensibilidade
{
    public partial class EditorOrdemFabricoStocks : Form
    {
        private ErpBS100.ErpBS BSO;
        private StdPlatBS100.StdBSInterfPub PSO;
        private IntBE100.IntBEDocumentoInterno DocumentoStock;
        private string projeto;

        public EditorOrdemFabricoStocks(ErpBS100.ErpBS bSO, StdPlatBS100.StdBSInterfPub pSO, IntBE100.IntBEDocumentoInterno documentoStock)
        {
            InitializeComponent();
            BSO = bSO;
            PSO = pSO;
            DocumentoStock = documentoStock;
            ConfigurarComboBox();
            GetValores();
        }

        private void ConfigurarComboBox()
        {
            // Adicionar itens ao ComboBox
            cbTipoLista.Items.Add("Artigos Subcontratados");
            cbTipoLista.Items.Add("Artigos Elétricos");
            cbTipoLista.SelectedIndex = 0; // Selecionar por defeito os artigos subcontratados
        }

        private void GetValores()
        {
            var ordemFabricoSQL = GetInforOrdemFabrico();

            // Verificar se existe resultado e se tem projeto
            if (ordemFabricoSQL != null && ordemFabricoSQL.NumLinhas() > 0)
            {
                projeto = ordemFabricoSQL.DaValor<string>("CDU_CodigoProjeto");
            }
            else
            {
                projeto = "";
            }

            CarregarListaSelecionada();
        }

        private void CarregarListaSelecionada()
        {
            // Se não há projeto definido, mostrar grid vazio
            if (string.IsNullOrEmpty(projeto))
            {
                System.Data.DataTable dtVazio = CriarDataTableVazio();
                dgvOrdensFabrico.DataSource = dtVazio;
                return;
            }

            if (cbTipoLista.SelectedIndex == 0)
            {
                // Carregar artigos subcontratados
                CarregarArtigosSubcontratados();
            }
            else
            {
                // Carregar artigos elétricos
                CarregarArtigosEletricos();
            }
        }

        private void CarregarArtigosSubcontratados()
        {
            var todasOrdemFabricoProjeto = GetInfoListaOrdemFabricoProjeto(projeto);
            System.Data.DataTable todasOrdensFabricoSub = GetInfoOrdemFabricoSubGrid(todasOrdemFabricoProjeto);
            dgvOrdensFabrico.DataSource = todasOrdensFabricoSub;
        }

        private void CarregarArtigosEletricos()
        {
            var artigosEletricos = GetArtigosEletricos(projeto);
            System.Data.DataTable dtEletricos = ConvertArtigosEletricosToDataTable(artigosEletricos);
            dgvOrdensFabrico.DataSource = dtEletricos;
        }

        private List<StdBELista> GetInfoOrdemFabricoSub(StdBELista todasOrdemFabricoProjeto)
        {
            var ordensComSubcontratacao = new List<StdBELista>(); // Lista para armazenar os resultados
            var num = todasOrdemFabricoProjeto.NumLinhas();

            todasOrdemFabricoProjeto.Inicio();
            for (int i = 0; i < num; i++)
            {
                var idOrdem = todasOrdemFabricoProjeto.DaValor<string>("IDOrdemFabrico");

                // Busca apenas ordens que têm subcontratação ativa
                var query = $"SELECT * FROM GPR_OrdemFabricoOperacoes WHERE IDOrdemFabrico = '{idOrdem}' AND SubContratacao = 1";
                var result = BSO.Consulta(query);

                if (result.NumLinhas() > 0)
                {
                    ordensComSubcontratacao.Add(result);
                }

                todasOrdemFabricoProjeto.Seguinte();
            }

            return ordensComSubcontratacao;
        }

        private StdBELista GetInfoListaOrdemFabricoProjeto(string projecto)
        {
            var query = $"SELECT IDOrdemFabrico,CDU_CodigoProjeto,* FROM GPR_OrdemFabrico WHERE CDU_CodigoProjeto = '{projecto}'";
            var lista = BSO.Consulta(query);
            return lista;
        }

        private StdBELista GetInforOrdemFabrico()
        {
            var idordem = this.DocumentoStock.IdOrdemFabrico.ToString();
            var query = $"SELECT * FROM GPR_OrdemFabrico WHERE IDOrdemFabrico = '{idordem}'";
            var lista = BSO.Consulta(query);
            return lista;
        }

        private System.Data.DataTable GetInfoOrdemFabricoSubGrid(StdBELista todasOrdemFabricoProjeto)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("Selecionado", typeof(bool));
            dt.Columns.Add("OrdemFabrico", typeof(string));
            dt.Columns.Add("Artigo", typeof(string));
            dt.Columns.Add("Unidade", typeof(string));
            dt.Columns.Add("QtFabricada", typeof(string));
            dt.Columns.Add("Liquido", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("Descricao", typeof(string));
            dt.Columns.Add("Projecto", typeof(string));
            dt.Columns.Add("Rececionado", typeof(bool));
            dt.Columns.Add("SubContratacao", typeof(bool));

            var num = todasOrdemFabricoProjeto.NumLinhas();
            todasOrdemFabricoProjeto.Inicio();

            for (int i = 0; i < num; i++)
            {
                var idOrdem = todasOrdemFabricoProjeto.DaValor<string>("IDOrdemFabrico");
                var query = $"SELECT IDOrdemFabrico, SubContratacao, Descricao FROM GPR_OrdemFabricoOperacoes WHERE IDOrdemFabrico = '{idOrdem}' AND SubContratacao = 1";
                var result = BSO.Consulta(query);
                var queryOrdem = $"SELECT OrdemFabrico, Artigo, QtFabricada, CustoMateriaisReal, CustoTransformacaoReal, CustoSubprodutosReal, CDU_CodigoProjeto FROM GPR_OrdemFabrico WHERE IDOrdemFabrico = '{idOrdem}'";
                var resultReal = BSO.Consulta(queryOrdem);

                while (!result.NoFim())
                {
                    decimal SafeGetDecimal(object value) =>
                        value == null || value == DBNull.Value ? 0 : Convert.ToDecimal(value);

                    decimal custoMateriais = SafeGetDecimal(resultReal.DaValor<object>("CustoMateriaisReal"));
                    decimal custoTransformacao = SafeGetDecimal(resultReal.DaValor<object>("CustoTransformacaoReal"));
                    decimal custoSubprodutos = SafeGetDecimal(resultReal.DaValor<object>("CustoSubprodutosReal"));
                    decimal total = custoMateriais + custoTransformacao + custoSubprodutos;

                    // Verifica se o artigo já existe no DocumentoStock.Linhas
                    bool existe = false;
                    for (int y = 1; y <= this.DocumentoStock.Linhas.NumItens; y++)
                    {
                        if (this.DocumentoStock.Linhas.GetEdita(y).Artigo == resultReal.DaValor<string>("Artigo"))
                        {
                            existe = true;
                            break;
                        }
                    }

                    var of = resultReal.DaValor<string>("OrdemFabrico");
                    var queryUnidade = $@"SELECT 
                                        L.Descricao,
                                        L.Unidade,
                                        L.PrecUnit,
                                        L.PrecoLiquido,
                                        L.Quantidade,
                                        L.IdCabecCompras,
                                        C.*
                                    FROM 
                                        LinhasCompras L
                                    INNER JOIN 
                                        CabecCompras C
                                        ON L.IdCabecCompras = C.ID
                                    WHERE 
                                        L.Descricao LIKE '%{of}%' 
                                        AND C.TipoDoc = 'VFS';";

                    var resultvfa = BSO.Consulta(queryUnidade);

                    var numlinhas = resultvfa.NumLinhas();
                    if (numlinhas > 0)
                    {
                        var quantidade = Math.Round(Math.Abs(Convert.ToDouble(resultvfa.DaValor<string>("Quantidade"))), 3);
                        var totalpositivo = Math.Round(Math.Abs(Convert.ToDouble(resultvfa.DaValor<string>("PrecUnit"))), 4);
                        var liquido = Math.Round(Math.Abs(Convert.ToDouble(resultvfa.DaValor<string>("PrecoLiquido"))), 4);


                        var unida = resultvfa.DaValor<string>("Unidade");
                        dt.Rows.Add(
                            false,
                            resultReal.DaValor<string>("OrdemFabrico"),
                            resultReal.DaValor<string>("Artigo"),
                            "UN",
                            quantidade,
                            liquido,
                            totalpositivo,
                            result.DaValor<string>("Descricao"),
                            resultReal.DaValor<string>("CDU_CodigoProjeto"),
                            true,
                            result.DaValor<bool>("SubContratacao")
                        );
                    }
                    else
                    {
                        dt.Rows.Add(
                            false,
                            resultReal.DaValor<string>("OrdemFabrico"),
                            resultReal.DaValor<string>("Artigo"),
                            "UN",
                            resultReal.DaValor<string>("QtFabricada"),
                            0.000,
                            0.000,
                            result.DaValor<string>("Descricao"),
                            resultReal.DaValor<string>("CDU_CodigoProjeto"),
                            false,
                            result.DaValor<bool>("SubContratacao")
                        );
                    }



                    resultReal.Seguinte();
                    result.Seguinte();
                }

                todasOrdemFabricoProjeto.Seguinte();
            }

            return dt;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvOrdensFabrico.Columns[e.ColumnIndex].Name == "Artigo") // Escolha uma coluna relevante
            {
                string artigo = dgvOrdensFabrico.Rows[e.RowIndex].Cells["Artigo"].Value.ToString();
                bool existe = false;

                var lista = $@"SELECT COUNT(*) AS count
                       FROM CabecInternos CI
                       JOIN LinhasInternos LI ON CI.Id = LI.IdCabecInternos
                       WHERE CI.TipoDoc = 'SOF' 
                         AND CI.IdOrdemFabrico = '{this.DocumentoStock.IdOrdemFabrico}'
                         AND LI.Artigo = '{artigo}';";

                var response = BSO.Consulta(lista);
                response.Inicio();

                // Se o número de linhas retornadas for maior que 0, o artigo existe
                if (response.DaValor<int>("count") > 0)
                {
                    existe = true;
                }

                // Pinta a linha de cinza claro se o artigo já existir
                if (existe)
                {
                    dgvOrdensFabrico.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                }
                else
                {
                    dgvOrdensFabrico.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            List<DataRow> linhasSelecionadas = new List<DataRow>();
            List<string> artigosJaExistentes = new List<string>();
            var documento = BSO.Internos.Documentos.Edita(this.DocumentoStock.Tipodoc, this.DocumentoStock.NumDoc, this.DocumentoStock.Serie, this.DocumentoStock.Filial);

            foreach (DataGridViewRow row in dgvOrdensFabrico.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Selecionado"].Value))
                {
                    DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                    var artigo = dataRow["Artigo"].ToString();

                    // Verificar se o artigo já existe no SOF
                    var queryVerificacao = $@"SELECT COUNT(*) AS count
                                           FROM CabecInternos CI
                                           JOIN LinhasInternos LI ON CI.Id = LI.IdCabecInternos
                                           WHERE CI.TipoDoc = 'SOF' 
                                             AND CI.IdOrdemFabrico = '{this.DocumentoStock.IdOrdemFabrico}'
                                             AND LI.Artigo = '{artigo}';";

                    var resultadoVerificacao = BSO.Consulta(queryVerificacao);
                    resultadoVerificacao.Inicio();

                    if (resultadoVerificacao.DaValor<int>("count") > 0)
                    {
                        artigosJaExistentes.Add(artigo);
                        continue; // Pula este artigo e não o adiciona
                    }

                    linhasSelecionadas.Add(dataRow);
                    var lote = dataRow["OrdemFabrico"].ToString();
                    var uni = dataRow["Unidade"].ToString();

                    var Qtf = dataRow["QtFabricada"].ToString();
                    var total = dataRow["Total"].ToString();
                    var qtd = double.Parse(Qtf);
                    var tot = double.Parse(total);

                    // Calcular preço unitário
                    double precoUnitario = 0.000;
                    if (double.TryParse(Qtf, out double quantidadeFabricada) && double.TryParse(total, out double totalValue))
                    {
                        if (cbTipoLista.SelectedIndex == 1) // Artigos elétricos
                        {
                            precoUnitario = totalValue; // Para elétricos, usar o total como preço unitário
                        }
                        else // Artigos subcontratados
                        {
                            if (totalValue != 0)
                            {
                                if (quantidadeFabricada != 0)
                                {
                                    precoUnitario = totalValue / quantidadeFabricada;
                                }
                                else
                                {
                                    precoUnitario = totalValue / 1.000;
                                }
                            }
                            else
                            {
                                precoUnitario = totalValue / 1.000;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erro ao converter os valores para números.");
                        continue;
                    }

                    var infoArtigo = BSO.Base.Artigos.Edita(artigo);
                    var unidade = infoArtigo.UnidadeBase;
                    var descricao = infoArtigo.Descricao;

                    IntBELinhaDocumentoInterno linha = new IntBELinhaDocumentoInterno()
                    {
                        Artigo = artigo,
                        Lote = !string.IsNullOrEmpty(lote) ? lote : "",
                        Unidade = uni,
                        Descricao = descricao,
                        Armazem = "A1",
                        Quantidade = qtd,
                        PrecoUnitario = precoUnitario,
                        INV_EstadoOrigem = "DISP"
                    };

                    documento.Linhas.Insere(linha);
                }
            }

            string mensagem = "";
            if (linhasSelecionadas.Count > 0)
            {
                var error = "";
                BSO.Internos.Documentos.ValidaActualizacao(documento, ref error);
                BSO.Internos.Documentos.Actualiza(documento);
                mensagem = $"{linhasSelecionadas.Count} linhas adicionadas ao SOF.";
            }

            if (artigosJaExistentes.Count > 0)
            {
                string artigosIgnorados = string.Join(", ", artigosJaExistentes);
                if (!string.IsNullOrEmpty(mensagem))
                {
                    mensagem += $"\n\nArtigos já existentes no SOF (ignorados): {artigosIgnorados}";
                }
                else
                {
                    mensagem = $"Os seguintes artigos já existem no SOF e foram ignorados: {artigosIgnorados}";
                }
            }

            if (linhasSelecionadas.Count == 0 && artigosJaExistentes.Count == 0)
            {
                mensagem = "Nenhuma linha foi selecionada.";
            }

            MessageBox.Show(mensagem);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelecionarTodasLinhas();
        }

        private void SelecionarTodasLinhas()
        {
            foreach (DataGridViewRow row in dgvOrdensFabrico.Rows)
            {
                row.Cells["Selecionado"].Value = true;
            }
        }

        private StdBELista GetArtigosEletricos(string codigoProjeto)
        {
            var query = $@"SELECT 
                            LC.Artigo,
                            LC.Descricao,
                            LC.Lote,
                            LC.Quantidade,
                            A.Familia,
                            CC.NumDoc,
                            CC.Serie,
                            CO.Codigo,
                            OFA.OrdemFabrico,
                            LC.PrecUnit,
                            LC.PrecoLiquido
                        FROM 
                            CabecCompras AS CC
                        INNER JOIN 
                            LinhasCompras AS LC ON CC.Id = LC.IdCabecCompras
                        INNER JOIN 
                            Artigo AS A ON LC.Artigo = A.Artigo
                        INNER JOIN 
                            COP_Obras AS CO ON LC.ObraID = CO.ID
                        LEFT JOIN 
                            GPR_OrdemFabrico AS OFA ON LC.Artigo = OFA.Artigo
                        WHERE 
                            CC.TipoDoc = 'VFA'
                            AND LC.Artigo IS NOT NULL
                            AND (A.Familia = '004' OR A.Familia = '024')
                            AND LC.ObraID IS NOT NULL
                            AND CO.Codigo = '{codigoProjeto}'";

            return BSO.Consulta(query);
        }

        private void InserirArtigosEletricosNoSOF(StdBELista artigosEletricos)
        {
            var documento = BSO.Internos.Documentos.Edita(this.DocumentoStock.Tipodoc, this.DocumentoStock.NumDoc, this.DocumentoStock.Serie, this.DocumentoStock.Filial);
            int artigosInseridos = 0;
            var num = artigosEletricos.NumLinhas();

            artigosEletricos.Inicio();
            for (int i = 0; i < num; i++)
            {
                var artigo = artigosEletricos.DaValor<string>("Artigo");
                var lote = artigosEletricos.DaValor<string>("Lote");
                var descricao = artigosEletricos.DaValor<string>("Descricao");

                // Verificar se o artigo já existe no documento
                bool artigoJaExiste = false;
                for (int y = 1; y <= documento.Linhas.NumItens; y++)
                {
                    if (documento.Linhas.GetEdita(y).Artigo == artigo)
                    {
                        artigoJaExiste = true;
                        break;
                    }
                }

                if (!artigoJaExiste)
                {
                    var quantidade = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("Quantidade")));
                    var precoUnitario = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("PrecUnit")));

                    // Obter informações do artigo
                    var infoArtigo = BSO.Base.Artigos.Edita(artigo);
                    var unidade = infoArtigo.UnidadeBase;

                    IntBELinhaDocumentoInterno linha = new IntBELinhaDocumentoInterno()
                    {
                        Artigo = artigo,
                        Lote = !string.IsNullOrEmpty(lote) ? lote : "",
                        Unidade = unidade,
                        Descricao = descricao,
                        Armazem = "A1",
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario,
                        INV_EstadoOrigem = "DISP"
                    };

                    documento.Linhas.Insere(linha);
                    artigosInseridos++;
                }

                artigosEletricos.Seguinte();
            }

            if (artigosInseridos > 0)
            {
                var error = "";
                BSO.Internos.Documentos.ValidaActualizacao(documento, ref error);
                BSO.Internos.Documentos.Actualiza(documento);
            }
        }

        private bool VerificarSeArtigosEletricosJaInseridos(string projeto)
        {
            var queryVerificacao = $@"SELECT COUNT(*) AS count
                                   FROM CabecInternos CI
                                   JOIN LinhasInternos LI ON CI.Id = LI.IdCabecInternos
                                   JOIN Artigo A ON LI.Artigo = A.Artigo
                                   JOIN GPR_OrdemFabrico GF ON CI.IdOrdemFabrico = GF.IDOrdemFabrico
                                   WHERE CI.TipoDoc = 'SOF' 
                                     AND GF.CDU_CodigoProjeto = '{projeto}'
                                     AND (A.Familia = '004' OR A.Familia = '024')";

            var resultado = BSO.Consulta(queryVerificacao);
            resultado.Inicio();

            return resultado.DaValor<int>("count") > 0;
        }

        private System.Data.DataTable ConvertArtigosEletricosToDataTable(StdBELista artigosEletricos)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("Selecionado", typeof(bool));
            dt.Columns.Add("OrdemFabrico", typeof(string));
            dt.Columns.Add("Artigo", typeof(string));
            dt.Columns.Add("Unidade", typeof(string));
            dt.Columns.Add("QtFabricada", typeof(string));
            dt.Columns.Add("Liquido", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("Descricao", typeof(string));
            dt.Columns.Add("Projecto", typeof(string));
            dt.Columns.Add("Rececionado", typeof(bool));
            dt.Columns.Add("SubContratacao", typeof(bool));

            var num = artigosEletricos.NumLinhas();
            artigosEletricos.Inicio();

            for (int i = 0; i < num; i++)
            {
                var artigo = artigosEletricos.DaValor<string>("Artigo");
                var ordemFabrico = artigosEletricos.DaValor<string>("OrdemFabrico");
                var descricao = artigosEletricos.DaValor<string>("Descricao");
                var quantidade = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("Quantidade")));
                var precoUnitario = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("PrecUnit")));
                var precoLiquido = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("PrecoLiquido")));

                // Verificar se o artigo já existe no DocumentoStock.Linhas
                bool existe = false;
                for (int y = 1; y <= this.DocumentoStock.Linhas.NumItens; y++)
                {
                    if (this.DocumentoStock.Linhas.GetEdita(y).Artigo == artigo)
                    {
                        existe = true;
                        break;
                    }
                }

                var infoArtigo = BSO.Base.Artigos.Edita(artigo);
                var unidade = infoArtigo?.UnidadeBase ?? "UN";

                dt.Rows.Add(
                    false,
                    ordemFabrico ?? "",
                    artigo,
                    unidade,
                    quantidade,
                    precoLiquido,
                    precoUnitario,
                    descricao,
                    projeto,
                    !existe,
                    false // Artigos elétricos não são subcontratados
                );

                artigosEletricos.Seguinte();
            }

            return dt;
        }

        private System.Data.DataTable CriarDataTableVazio()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("Selecionado", typeof(bool));
            dt.Columns.Add("OrdemFabrico", typeof(string));
            dt.Columns.Add("Artigo", typeof(string));
            dt.Columns.Add("Unidade", typeof(string));
            dt.Columns.Add("QtFabricada", typeof(string));
            dt.Columns.Add("Liquido", typeof(decimal));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("Descricao", typeof(string));
            dt.Columns.Add("Projecto", typeof(string));
            dt.Columns.Add("Rececionado", typeof(bool));
            dt.Columns.Add("SubContratacao", typeof(bool));

            return dt;
        }

        private void cbTipoLista_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarListaSelecionada();
        }

        private void btnExportarTemplateExcel_Click(object sender, EventArgs e)
        {
            ExportarTemplateExcel();
        }

        private void btnImportarExcel_Click(object sender, EventArgs e)
        {
            ImportarExcel();
        }

        private void ExportarTemplateExcel()
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveDialog.DefaultExt = "xlsx";
                saveDialog.FileName = $"Template_Artigos_{projeto}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                    Workbook workbook = excelApp.Workbooks.Add();
                    Worksheet worksheet = workbook.ActiveSheet;

                    // Definir cabeçalhos
                    worksheet.Cells[1, 1] = "Artigo";
                    worksheet.Cells[1, 2] = "Quantidade";
                    worksheet.Cells[1, 3] = "Preço Unitário";
                    worksheet.Cells[1, 4] = "Descrição";

                    // Formatar cabeçalhos
                    Range headerRange = worksheet.Range["A1:D1"];
                    headerRange.Font.Bold = true;
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                    // Adicionar exemplo na segunda linha (opcional)
                    worksheet.Cells[2, 1] = "Exemplo: ART001";
                    worksheet.Cells[2, 2] = 1;
                    worksheet.Cells[2, 3] = 0.00;
                    worksheet.Cells[2, 4] = "Descrição do artigo";

                    // Formatar linha de exemplo em itálico
                    Range exampleRange = worksheet.Range["A2:D2"];
                    exampleRange.Font.Italic = true;
                    exampleRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);

                    // Ajustar largura das colunas
                    worksheet.Columns["A:D"].AutoFit();

                    // Salvar o arquivo
                    workbook.SaveAs(saveDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    // Liberar objetos COM
                    Marshal.ReleaseComObject(worksheet);
                    Marshal.ReleaseComObject(workbook);
                    Marshal.ReleaseComObject(excelApp);

                    MessageBox.Show($"Template Excel vazio exportado com sucesso para:\n{saveDialog.FileName}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar template Excel: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportarExcel()
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
                openDialog.Title = "Selecionar arquivo Excel com artigos";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                    Workbook workbook = excelApp.Workbooks.Open(openDialog.FileName);
                    Worksheet worksheet = workbook.ActiveSheet;

                    // Criar DataTable para armazenar os dados importados
                    System.Data.DataTable dtImportados = new System.Data.DataTable();
                    dtImportados.Columns.Add("Selecionado", typeof(bool));
                    dtImportados.Columns.Add("OrdemFabrico", typeof(string));
                    dtImportados.Columns.Add("Artigo", typeof(string));
                    dtImportados.Columns.Add("Unidade", typeof(string));
                    dtImportados.Columns.Add("QtFabricada", typeof(string));
                    dtImportados.Columns.Add("Liquido", typeof(decimal));
                    dtImportados.Columns.Add("Total", typeof(decimal));
                    dtImportados.Columns.Add("Descricao", typeof(string));
                    dtImportados.Columns.Add("Projecto", typeof(string));
                    dtImportados.Columns.Add("Rececionado", typeof(bool));
                    dtImportados.Columns.Add("SubContratacao", typeof(bool));

                    // Ler dados do Excel (assumindo que a primeira linha são cabeçalhos)
                    int row = 2;
                    int artigosImportados = 0;
                    List<string> artigosNaoEncontrados = new List<string>();

                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        string artigo = worksheet.Cells[row, 1].Value?.ToString().Trim();

                        // Pular linha de exemplo
                        if (artigo.StartsWith("Exemplo:"))
                        {
                            row++;
                            continue;
                        }

                        double quantidade = 1; // Quantidade padrão
                        double precoUnitario = 0;
                        string descricaoExcel = "";

                        if (worksheet.Cells[row, 2].Value != null)
                        {
                            double.TryParse(worksheet.Cells[row, 2].Value.ToString(), out quantidade);
                        }

                        if (worksheet.Cells[row, 3].Value != null)
                        {
                            double.TryParse(worksheet.Cells[row, 3].Value.ToString(), out precoUnitario);
                        }

                        if (worksheet.Cells[row, 4].Value != null)
                        {
                            descricaoExcel = worksheet.Cells[row, 4].Value.ToString();
                        }

                        if (!string.IsNullOrEmpty(artigo) && quantidade > 0)
                        {
                            // Verificar se o artigo existe na base de dados
                            try
                            {
                                var infoArtigo = BSO.Base.Artigos.Edita(artigo);
                                if (infoArtigo != null)
                                {
                                    // Verificar se já existe no documento
                                    bool existe = false;
                                    for (int y = 1; y <= this.DocumentoStock.Linhas.NumItens; y++)
                                    {
                                        if (this.DocumentoStock.Linhas.GetEdita(y).Artigo == artigo)
                                        {
                                            existe = true;
                                            break;
                                        }
                                    }

                                    // Usar descrição do Excel se fornecida, senão usar da base de dados
                                    string descricaoFinal = !string.IsNullOrEmpty(descricaoExcel) ? descricaoExcel : infoArtigo.Descricao;

                                    dtImportados.Rows.Add(
                                        true, // Selecionado por defeito
                                        "", // OrdemFabrico vazio para importados
                                        artigo,
                                        infoArtigo.UnidadeBase,
                                        quantidade.ToString(),
                                        0.000, // Preço líquido sempre 0 para importados
                                        precoUnitario, // Usar preço do Excel
                                        descricaoFinal,
                                        projeto,
                                        !existe, // Rececionado = true se não existe ainda
                                        false // Determinar depois baseado no tipo
                                    );

                                    artigosImportados++;
                                }
                            }
                            catch (Exception)
                            {
                                // Artigo não encontrado na base de dados
                                artigosNaoEncontrados.Add(artigo);
                            }
                        }

                        row++;
                    }

                    workbook.Close();
                    excelApp.Quit();

                    // Liberar objetos COM
                    Marshal.ReleaseComObject(worksheet);
                    Marshal.ReleaseComObject(workbook);
                    Marshal.ReleaseComObject(excelApp);

                    if (artigosImportados > 0)
                    {
                        // Substituir o DataSource do DataGridView pelos dados importados
                        dgvOrdensFabrico.DataSource = dtImportados;

                        string mensagem = $"{artigosImportados} artigos importados com sucesso do Excel!";

                        if (artigosNaoEncontrados.Count > 0)
                        {
                            mensagem += $"\n\nArtigos não encontrados na base de dados: {string.Join(", ", artigosNaoEncontrados)}";
                        }

                        MessageBox.Show(mensagem, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (artigosNaoEncontrados.Count > 0)
                        {
                            MessageBox.Show($"Nenhum artigo válido encontrado. Artigos não existentes: {string.Join(", ", artigosNaoEncontrados)}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Nenhum artigo válido encontrado no Excel.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao importar Excel: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
