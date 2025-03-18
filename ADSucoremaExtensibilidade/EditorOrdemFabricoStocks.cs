using IntBE100;
using StdBE100;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public partial class EditorOrdemFabricoStocks : Form
    {
        private ErpBS100.ErpBS BSO;
        private StdPlatBS100.StdBSInterfPub PSO;
        private IntBE100.IntBEDocumentoInterno DocumentoStock;
        public EditorOrdemFabricoStocks(ErpBS100.ErpBS bSO, StdPlatBS100.StdBSInterfPub pSO, IntBE100.IntBEDocumentoInterno documentoStock)
        {
            InitializeComponent();
            BSO = bSO;
            PSO = pSO;
            DocumentoStock = documentoStock;
            GetValores();
        }

        private void GetValores()
        {

            var ordemFabricoSQL = GetInforOrdemFabrico();

            var projeto = ordemFabricoSQL.DaValor<string>("CDU_CodigoProjeto");


            var todasOrdemFabricoProjeto = GetInfoListaOrdemFabricoProjeto(projeto);

            //var todasOrdensFabricoSub = GetInfoOrdemFabricoSub(todasOrdemFabricoProjeto);

            DataTable todasOrdensFabricoSub = GetInfoOrdemFabricoSubGrid(todasOrdemFabricoProjeto);

            dgvOrdensFabrico.DataSource = todasOrdensFabricoSub;

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

        private DataTable GetInfoOrdemFabricoSubGrid(StdBELista todasOrdemFabricoProjeto)
        {
            DataTable dt = new DataTable();

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
                    if(numlinhas > 0)
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
            var documento = BSO.Internos.Documentos.Edita(this.DocumentoStock.Tipodoc, this.DocumentoStock.NumDoc, this.DocumentoStock.Serie, this.DocumentoStock.Filial);
            foreach (DataGridViewRow row in dgvOrdensFabrico.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Selecionado"].Value))
                {
                    DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                    linhasSelecionadas.Add(dataRow);
                    var artigo = dataRow["Artigo"].ToString();
                    var lote = dataRow["OrdemFabrico"].ToString();
                    var uni = dataRow["Unidade"].ToString();
                    //var qtd = dataRow["QtFabricada"].ToString();

                    var Qtf = dataRow["QtFabricada"].ToString();
                    var total = dataRow["Total"].ToString();
                    var qtd = double.Parse(Qtf);
                    var tot = double.Parse(total);
                    // Converter as variáveis para números
                    double quantidadeFabricada = 0;
                    double totalValue = 0;
                    double qtdi = 0.000;
                    if (double.TryParse(Qtf, out quantidadeFabricada) && double.TryParse(total, out totalValue))
                    {
                        // Verificar se o total não é zero para evitar divisão por zero
                        if (totalValue != 0)
                        {
                            if (quantidadeFabricada != 0) {
                                double resultado = totalValue / quantidadeFabricada;
                                qtdi = resultado;
                            }
                            else
                            {
                                double resultado = totalValue / 1.000;
                                qtdi = resultado;
                            }
                        }
                        else
                        {
                            double resultado = totalValue / 1.000;
                            qtdi = resultado;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erro ao converter os valores para números.");
                    }



                    var infoArtigo = BSO.Base.Artigos.Edita(artigo);
                    var unidade = infoArtigo.UnidadeBase;
                    var descricao = infoArtigo.Descricao;
                    var armazem = infoArtigo.ArmazemSugestao;

                    IntBELinhaDocumentoInterno linha = new IntBELinhaDocumentoInterno()
                    {
                        Artigo = artigo,
                        Lote = lote,
                        Unidade = uni,
                        Descricao = descricao,
                        Armazem = "A1",
                        Quantidade = qtd,
                        PrecoUnitario = tot,
                        INV_EstadoOrigem = "DISP"

                    };

                    documento.Linhas.Insere(linha);
                }
            }
            var error = "";
            BSO.Internos.Documentos.ValidaActualizacao(documento, ref  error);
            BSO.Internos.Documentos.Actualiza(documento);
           
            // Faça algo com as linhas selecionadas, como exibi-las ou processá-las
            MessageBox.Show($"{linhasSelecionadas.Count} linhas Adcionadas.");
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
    }
}
