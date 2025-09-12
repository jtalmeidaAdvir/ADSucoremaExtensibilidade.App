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
            cbTipoLista.Items.Add("Matéria Prima");
            cbTipoLista.Items.Add("Adquiridos Hidráulica");
            cbTipoLista.Items.Add("Adquiridos Pneumática");
            cbTipoLista.Items.Add("Adquiridos Mecânicos");
            cbTipoLista.Items.Add("Artigos Fixação");
            cbTipoLista.Items.Add("Serviços");
            cbTipoLista.Items.Add("Todos os Artigos");
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

            switch (cbTipoLista.SelectedIndex)
            {
                case 0: // Artigos Subcontratados
                    CarregarArtigosSubcontratados();
                    break;
                case 1: // Artigos Elétricos
                    CarregarArtigosEletricos();
                    break;
                case 2: // Matéria Prima (001)
                    CarregarArtigosPorFamilia("001");
                    break;
                case 3: // Adquiridos Hidráulica (002)
                    CarregarArtigosPorFamilia("002");
                    break;
                case 4: // Adquiridos Pneumática (003)
                    CarregarArtigosPorFamilia("003");
                    break;
                case 5: // Adquiridos Mecânicos (005)
                    CarregarArtigosPorFamilia("005");
                    break;
                case 6: // Artigos Fixação (006)
                    CarregarArtigosPorFamilia("006");
                    break;
                case 7: // Serviços (011)
                    CarregarArtigosPorFamilia("011");
                    break;
                case 8: // Todos os Artigos
                    CarregarTodosOsArtigos();
                    break;
                default:
                    CarregarArtigosSubcontratados();
                    break;
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

        private StdBELista GetInfoOrdemFabricoSub(string idOrdem)
        {
            var query = $"SELECT IDOrdemFabrico, SubContratacao FROM GPR_OrdemFabricoOperacoes WHERE IDOrdemFabrico = '{idOrdem}' AND SubContratacao = 1";
            return BSO.Consulta(query);
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
            dt.Columns.Add("Familia", typeof(string));
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
                var queryOperacoes = $"SELECT SubContratacao, Descricao FROM GPR_OrdemFabricoOperacoes WHERE IDOrdemFabrico = '{idOrdem}' AND SubContratacao = 1";
                var resultOperacoes = BSO.Consulta(queryOperacoes);

                var queryOrdem = $"SELECT OrdemFabrico, Artigo, QtFabricada, CustoMateriaisReal, CustoTransformacaoReal, CustoSubprodutosReal, CDU_CodigoProjeto FROM GPR_OrdemFabrico WHERE IDOrdemFabrico = '{idOrdem}'";
                var resultOrdem = BSO.Consulta(queryOrdem);

                while (!resultOperacoes.NoFim())
                {
                    decimal SafeGetDecimal(object value) =>
                        value == null || value == DBNull.Value ? 0 : Convert.ToDecimal(value);

                    decimal custoMateriais = SafeGetDecimal(resultOrdem.DaValor<object>("CustoMateriaisReal"));
                    decimal custoTransformacao = SafeGetDecimal(resultOrdem.DaValor<object>("CustoTransformacaoReal"));
                    decimal custoSubprodutos = SafeGetDecimal(resultOrdem.DaValor<object>("CustoSubprodutosReal"));
                    decimal total = custoMateriais + custoTransformacao + custoSubprodutos;

                    // Verifica se o artigo já existe no DocumentoStock.Linhas
                    bool existeNoDocumento = false;
                    for (int y = 1; y <= this.DocumentoStock.Linhas.NumItens; y++)
                    {
                        if (this.DocumentoStock.Linhas.GetEdita(y).Artigo == resultOrdem.DaValor<string>("Artigo"))
                        {
                            existeNoDocumento = true;
                            break;
                        }
                    }

                    var ordemFabricoAtual = resultOrdem.DaValor<string>("OrdemFabrico");
                    var artigoAtual = resultOrdem.DaValor<string>("Artigo");

                    // Verifica se o artigo já foi rececionado para artigos subcontratados
                    bool jaRececionado = false;

                    // Lógica para artigos subcontratados (TipoDoc = 'VFS' e busca na descrição)
                    if (resultOperacoes.DaValor<bool>("SubContratacao"))
                    {
                        var queryRececaoVFS = $@"SELECT 
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
                                                L.Descricao LIKE '%{ordemFabricoAtual}%' 
                                                AND C.TipoDoc = 'VFS';";

                        var resultVFS = BSO.Consulta(queryRececaoVFS);
                        if (resultVFS.NumLinhas() > 0)
                        {
                            jaRececionado = true;
                        }
                    }
                    // Lógica para outros artigos (verificar no documento 'VFA' pelo campo L.artigo)
                    else
                    {
                        var queryRececaoVFA = $@"SELECT 
                                                c.NumDoc,
                                                L.Artigo,
                                                L.Unidade,
                                                L.PrecUnit,
                                                L.PrecoLiquido,
                                                L.Quantidade,
                                                L.IdCabecCompras,
                                                L.ObraID,
                                                CO.Codigo,
                                                C.*
                                            FROM 
                                                LinhasCompras L
                                            INNER JOIN 
                                                CabecCompras C
                                                ON L.IdCabecCompras = C.ID
                                            INNER JOIN 
                                                COP_Obras AS CO ON L.ObraID = CO.ID
                                            WHERE 
                                                L.Artigo = '{artigoAtual}' 
                                                AND C.TipoDoc = 'VFA'
                                                AND CO.Codigo = '{projeto}';";

                        var resultVFA = BSO.Consulta(queryRececaoVFA);
                        if (resultVFA.NumLinhas() > 0)
                        {
                            jaRececionado = true;
                        }
                    }

                    // Se não foi rececionado e não existe no documento, considerar como não rececionado
                    if (!jaRececionado && !existeNoDocumento)
                    {
                        jaRececionado = false;
                    }
                    else if (!jaRececionado && existeNoDocumento)
                    {
                        // Se não foi rececionado mas existe no documento, considerar como não rececionado também
                        jaRececionado = false;
                    }
                    // Se foi rececionado, independente de existir no documento, jáRececionado fica true.

                    dt.Rows.Add(
                        false,
                        resultOrdem.DaValor<string>("OrdemFabrico"),
                        resultOrdem.DaValor<string>("Artigo"),
                        "", // Família será preenchida abaixo
                        "UN",
                        resultOrdem.DaValor<string>("QtFabricada"),
                        0.000,
                        total,
                        resultOperacoes.DaValor<string>("Descricao"),
                        resultOrdem.DaValor<string>("CDU_CodigoProjeto"),
                        jaRececionado, // Usar a verificação real das VFA
                        resultOperacoes.DaValor<bool>("SubContratacao")
                    );

                    // Preencher a família após a criação da linha
                    var infoArtigoSub = BSO.Base.Artigos.Edita(resultOrdem.DaValor<string>("Artigo"));
                    if (infoArtigoSub != null)
                    {
                        // Buscar a descrição da família
                        var queryFamiliaSub = $"SELECT Descricao FROM Familias WHERE Familia = '{infoArtigoSub.Familia}'";
                        var resultFamiliaSub = BSO.Consulta(queryFamiliaSub);

                        if (resultFamiliaSub.NumLinhas() > 0)
                        {
                            dt.Rows[dt.Rows.Count - 1]["Familia"] = resultFamiliaSub.DaValor<string>("Descricao");
                        }
                        else
                        {
                            dt.Rows[dt.Rows.Count - 1]["Familia"] = infoArtigoSub.Familia; // Fallback para o código se não encontrar descrição
                        }
                    }

                    resultOrdem.Seguinte();
                    resultOperacoes.Seguinte();
                }

                todasOrdemFabricoProjeto.Seguinte();
            }

            return dt;
        }


        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvOrdensFabrico.Rows.Count)
            {
                var row = dgvOrdensFabrico.Rows[e.RowIndex];

                // Verificar se é serviço (família 011) através da família, não da subcontratação
                bool isServico = false;
                if (row.Cells["Familia"].Value != null)
                {
                    string familia = row.Cells["Familia"].Value.ToString();
                    // Verificar se a família é "011" ou se a descrição contém "Serviços"
                    isServico = familia == "011" || familia.ToLower().Contains("serviço");
                }

                if (isServico)
                {
                    // Para serviços: cor de fundo amarelo claro e desabilitar checkbox
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    row.Cells["Selecionado"].ReadOnly = true;
                    row.Cells["Selecionado"].Style.BackColor = Color.LightYellow;
                    row.Cells["Selecionado"].Value = false; // Sempre false para serviços
                    return;
                }

                // Para outros artigos, verificar se já existem em SOFs do projeto
                if (dgvOrdensFabrico.Columns[e.ColumnIndex].Name == "Artigo")
                {
                    string artigo = row.Cells["Artigo"].Value?.ToString();
                    if (!string.IsNullOrEmpty(artigo))
                    {
                        bool existe = false;

                        // Verificar se já existe em qualquer SOF do projeto
                        var lista = $@"SELECT COUNT(*) AS count
                               FROM CabecInternos CI
                               JOIN LinhasInternos LI ON CI.Id = LI.IdCabecInternos
                               JOIN [GPR_OrdemFabrico] GF ON CI.IdOrdemFabrico = GF.IDOrdemFabrico
                               WHERE CI.TipoDoc = 'SOF' 
                                 AND GF.CDU_CodigoProjeto = '{projeto}'
                                 AND LI.Artigo = '{artigo}'
                                    AND IDOperadorGPR > 0;";

                        var response = BSO.Consulta(lista);
                        response.Inicio();

                        if (response.DaValor<int>("count") > 0)
                        {
                            existe = true;
                        }

                        // Pinta a linha de cinza claro se o artigo já existir em qualquer SOF do projeto
                        if (existe)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            List<DataRow> linhasSelecionadas = new List<DataRow>();
            List<string> artigosJaExistentes = new List<string>();
            List<string> servicosIgnorados = new List<string>();
            var documento = BSO.Internos.Documentos.Edita(this.DocumentoStock.Tipodoc, this.DocumentoStock.NumDoc, this.DocumentoStock.Serie, this.DocumentoStock.Filial);

            foreach (DataGridViewRow row in dgvOrdensFabrico.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Selecionado"].Value))
                {
                    DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                    var artigo = dataRow["Artigo"].ToString();

                    // Verificar se é serviço e ignorar
                    string familia = dataRow["Familia"].ToString();
                    bool isServico = familia == "011" || familia.ToLower().Contains("serviço");
                    if (isServico)
                    {
                        servicosIgnorados.Add(artigo);
                        continue; // Pula serviços
                    }

                    // Verificar se o artigo já existe em qualquer SOF do projeto
                    var queryVerificacao = $@"SELECT COUNT(*) AS count
                                           FROM CabecInternos CI
                                           JOIN LinhasInternos LI ON CI.Id = LI.IdCabecInternos
                                           JOIN [GPR_OrdemFabrico] GF ON CI.IdOrdemFabrico = GF.IDOrdemFabrico
                                           WHERE CI.TipoDoc = 'SOF' 
                                             AND GF.CDU_CodigoProjeto = '{projeto}'
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
                        if (cbTipoLista.SelectedIndex >= 1) // Artigos elétricos e outras famílias
                        {
                            precoUnitario = totalValue; // Para elétricos e outras famílias, usar o total como preço unitário
                        }
                        else // Artigos subcontratados (índice 0)
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
                    var codigoFamilia = infoArtigo.Familia;

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

            if (servicosIgnorados.Count > 0)
            {
                string servicosTexto = string.Join(", ", servicosIgnorados);
                if (!string.IsNullOrEmpty(mensagem))
                {
                    mensagem += $"\n\nServiços ignorados (apenas para visualização): {servicosTexto}";
                }
                else
                {
                    mensagem = $"Serviços ignorados (apenas para visualização): {servicosTexto}";
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
                // Verificar se é serviço antes de selecionar
                bool isServico = false;
                if (row.Cells["Familia"].Value != null)
                {
                    string familia = row.Cells["Familia"].Value.ToString();
                    // Verificar se a família é "011" ou se a descrição contém "Serviços"
                    isServico = familia == "011" || familia.ToLower().Contains("serviço");
                }

                if (!isServico && !row.Cells["Selecionado"].ReadOnly)
                {
                    row.Cells["Selecionado"].Value = true;
                }
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
                            CC.TipoDoc = 'ECF'
                            AND LC.Artigo IS NOT NULL
                            AND (A.Familia = '004' OR A.Familia = '024')
                            AND LC.ObraID IS NOT NULL
                            AND CO.Codigo = '{codigoProjeto}'";

            return BSO.Consulta(query);
        }

        private void CarregarArtigosPorFamilia(string familia)
        {
            var artigosFamilia = GetArtigosPorFamilia(projeto, familia);
            System.Data.DataTable dtFamilia = ConvertArtigosEletricosToDataTable(artigosFamilia);
            dgvOrdensFabrico.DataSource = dtFamilia;
        }

        private StdBELista GetArtigosPorFamilia(string codigoProjeto, string familia)
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
                            CC.TipoDoc = 'ECF'
                            AND LC.Artigo IS NOT NULL
                            AND A.Familia = '{familia}'
                            AND LC.ObraID IS NOT NULL
                            AND CO.Codigo = '{codigoProjeto}'";

            return BSO.Consulta(query);
        }

        private void CarregarTodosOsArtigos()
        {
            var todosOsArtigos = GetTodosOsArtigos(projeto);
            System.Data.DataTable dtTodos = ConvertArtigosEletricosToDataTable(todosOsArtigos);
            dgvOrdensFabrico.DataSource = dtTodos;
        }

        private StdBELista GetTodosOsArtigos(string codigoProjeto)
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
                            CC.TipoDoc = 'ECF'
                            AND LC.Artigo IS NOT NULL
                            AND (A.Familia = '001' OR A.Familia = '002' OR A.Familia = '003' OR A.Familia = '004' OR A.Familia = '005' OR A.Familia = '006' OR A.Familia = '011' OR A.Familia = '024')
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
                    var familia = infoArtigo.Familia;

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
                                     AND (A.Familia = '001' OR A.Familia = '002' OR A.Familia = '003' OR A.Familia = '004' OR A.Familia = '005' OR A.Familia = '006' OR A.Familia = '011' OR A.Familia = '024')";

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
            dt.Columns.Add("Familia", typeof(string));
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

                // Buscar a descrição da família
                string descricaoFamilia = "";
                bool isServico = false;
                if (infoArtigo != null && !string.IsNullOrEmpty(infoArtigo.Familia))
                {
                    isServico = infoArtigo.Familia == "011"; // Verificar se é serviço

                    var queryFamilia = $"SELECT Descricao FROM Familias WHERE Familia = '{infoArtigo.Familia}'";
                    var resultFamilia = BSO.Consulta(queryFamilia);

                    if (resultFamilia.NumLinhas() > 0)
                    {
                        descricaoFamilia = resultFamilia.DaValor<string>("Descricao");
                    }
                    else
                    {
                        descricaoFamilia = infoArtigo.Familia; // Fallback para o código se não encontrar descrição
                    }
                }

                // Verificar se o artigo foi rececionado nas VFA
                bool jaRececionado = false;
                var queryRececaoVFA = $@"SELECT 
                                        c.NumDoc,
                                        L.Artigo,
                                        L.Unidade,
                                        L.PrecUnit,
                                        L.PrecoLiquido,
                                        L.Quantidade,
                                        L.IdCabecCompras,
                                        L.ObraID,
                                        CO.Codigo,
                                        C.*
                                    FROM 
                                        LinhasCompras L
                                    INNER JOIN 
                                        CabecCompras C
                                        ON L.IdCabecCompras = C.ID
                                    INNER JOIN 
                                        COP_Obras AS CO ON L.ObraID = CO.ID
                                    WHERE 
                                        L.Artigo = '{artigo}' 
                                        AND C.TipoDoc = 'VFA'
                                        AND CO.Codigo = '{projeto}';";

                var resultVFA = BSO.Consulta(queryRececaoVFA);
                if (resultVFA.NumLinhas() > 0)
                {
                    jaRececionado = true;
                }

                dt.Rows.Add(
                    false, // Checkbox sempre false inicialmente
                    ordemFabrico ?? "",
                    artigo,
                    descricaoFamilia,
                    unidade,
                    quantidade,
                    precoLiquido,
                    precoUnitario,
                    descricao,
                    projeto,
                    jaRececionado, // Usar a verificação real das VFA
                    isServico // Marcar se é serviço para controle
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
            dt.Columns.Add("Familia", typeof(string));
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
                    worksheet.Cells[1, 5] = "Familia";


                    // Formatar cabeçalhos
                    Range headerRange = worksheet.Range["A1:E1"];
                    headerRange.Font.Bold = true;
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                    // Adicionar exemplo na segunda linha (opcional)
                    worksheet.Cells[2, 1] = "Exemplo: ART001";
                    worksheet.Cells[2, 2] = 1;
                    worksheet.Cells[2, 3] = 0.00;
                    worksheet.Cells[2, 4] = "Descrição do artigo";
                    worksheet.Cells[2, 5] = "FAM001";


                    // Formatar linha de exemplo em itálico
                    Range exampleRange = worksheet.Range["A2:E2"];
                    exampleRange.Font.Italic = true;
                    exampleRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);

                    // Ajustar largura das colunas
                    worksheet.Columns["A:E"].AutoFit();

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
                    dtImportados.Columns.Add("Familia", typeof(string));
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
                        string familiaExcel = "";

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

                        if (worksheet.Cells[row, 5].Value != null)
                        {
                            familiaExcel = worksheet.Cells[row, 5].Value.ToString();
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
                                    string familiaFinal = !string.IsNullOrEmpty(familiaExcel) ? familiaExcel : infoArtigo.Familia;


                                    dtImportados.Rows.Add(
                                        true, // Selecionado por defeito
                                        "", // OrdemFabrico vazio para importados
                                        artigo,
                                        familiaFinal,
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