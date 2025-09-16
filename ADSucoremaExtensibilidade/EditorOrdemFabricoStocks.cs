
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
using System.Linq;

namespace ADSucoremaExtensibilidade
{
    public partial class EditorOrdemFabricoStocks : Form
    {
        private ErpBS100.ErpBS BSO;
        private StdPlatBS100.StdBSInterfPub PSO;
        private IntBE100.IntBEDocumentoInterno DocumentoStock;
        private string projeto;

        // Cache para otimização de consultas
        private Dictionary<string, bool> _cacheArtigosSOF = new Dictionary<string, bool>();

        // Variáveis de paginação
        private int _paginaAtual = 1;
        private int _itensPorPagina = 25;
        private int _totalItens = 0;
        private int _totalPaginas = 0;
        private System.Data.DataTable _dadosCompletos = null;

        // Contador para limpeza de cache
        private int _contadorOperacoes = 0;

        // Cache para receções
        private Dictionary<string, bool> _cacheRececaoVFS = new Dictionary<string, bool>();
        private Dictionary<string, bool> _cacheRececaoVFA = new Dictionary<string, bool>();

        public EditorOrdemFabricoStocks(ErpBS100.ErpBS bSO, StdPlatBS100.StdBSInterfPub pSO, IntBE100.IntBEDocumentoInterno documentoStock)
        {
            InitializeComponent();
            BSO = bSO;
            PSO = pSO;
            DocumentoStock = documentoStock;
            ConfigurarComboBox();
            ConfigurarPaginacao();
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

        private void LimparCachesSeNecessario()
        {
            _contadorOperacoes++;

            // Limpar caches a cada 100 operações para evitar memory leaks
            if (_contadorOperacoes % 100 == 0)
            {
                if (_cacheArtigosSOF.Count > 1000)
                {
                    _cacheArtigosSOF.Clear();
                }

                if (_cacheRececaoVFS.Count > 500)
                {
                    _cacheRececaoVFS.Clear();
                }

                if (_cacheRececaoVFA.Count > 500)
                {
                    _cacheRececaoVFA.Clear();
                }

                // Forçar garbage collection se necessário
                if (_contadorOperacoes % 500 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                System.Diagnostics.Debug.WriteLine($"Cache limpo após {_contadorOperacoes} operações");
            }
        }

        private void ConfigurarPaginacao()
        {
            cbItensPorPagina.SelectedIndex = 0; // 25 itens por defeito
            AtualizarEstadoBotoesPaginacao();
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
            // Limpar caches periodicamente para evitar memory leaks
            LimparCachesSeNecessario();

            // Se não há projeto definido, mostrar grid vazio
            if (string.IsNullOrEmpty(projeto))
            {
                _dadosCompletos = CriarDataTableVazio();
                AplicarPaginacao();
                return;
            }

            // Carregar dados completos baseado na seleção
            switch (cbTipoLista.SelectedIndex)
            {
                case 0: // Artigos Subcontratados
                    CarregarArtigosSubcontratadosCompleto();
                    break;
                case 1: // Artigos Elétricos
                    CarregarArtigosEletricosCompleto();
                    break;
                case 2: // Matéria Prima (001)
                    CarregarArtigosPorFamiliaCompleto("001");
                    break;
                case 3: // Adquiridos Hidráulica (002)
                    CarregarArtigosPorFamiliaCompleto("002");
                    break;
                case 4: // Adquiridos Pneumática (003)
                    CarregarArtigosPorFamiliaCompleto("003");
                    break;
                case 5: // Adquiridos Mecânicos (005)
                    CarregarArtigosPorFamiliaCompleto("005");
                    break;
                case 6: // Artigos Fixação (006)
                    CarregarArtigosPorFamiliaCompleto("006");
                    break;
                case 7: // Serviços (011)
                    CarregarArtigosPorFamiliaCompleto("011");
                    break;
                case 8: // Todos os Artigos
                    CarregarTodosOsArtigosCompleto();
                    break;
                default:
                    CarregarArtigosSubcontratadosCompleto();
                    break;
            }

            // Resetar para primeira página e aplicar paginação
            _paginaAtual = 1;
            AplicarPaginacao();
        }

        private void CarregarArtigosSubcontratadosCompleto()
        {
            var todasOrdemFabricoProjeto = GetInfoListaOrdemFabricoProjeto(projeto);
            _dadosCompletos = GetInfoOrdemFabricoSubGrid(todasOrdemFabricoProjeto);
        }

        private void CarregarArtigosEletricosCompleto()
        {
            var artigosEletricos = GetArtigosEletricos(projeto);
            _dadosCompletos = ConvertArtigosEletricosToDataTable(artigosEletricos);
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
            try
            {
                var query = $"SELECT TOP 500 IDOrdemFabrico,CDU_CodigoProjeto,* FROM GPR_OrdemFabrico WITH(NOLOCK) WHERE CDU_CodigoProjeto = '{projecto}' ORDER BY IDOrdemFabrico";
                var lista = BSO.Consulta(query);
                return lista;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao consultar ordens de fabrico: {ex.Message}");
                return BSO.Consulta("SELECT TOP 0 IDOrdemFabrico,CDU_CodigoProjeto FROM GPR_OrdemFabrico WHERE 1=0");
            }
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
            if (num == 0) return dt;

            // Limitar processamento para evitar travamentos
            if (num > 500)
            {
                MessageBox.Show($"Muitos registros encontrados ({num}). Será processado apenas os primeiros 500 para evitar travamentos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                num = 500;
            }

            // Criar lista de IDs das ordens de fabrico para consulta em lote (LIMITADO A 1000)
            var idsOrdens = new List<string>();
            todasOrdemFabricoProjeto.Inicio();
            for (int i = 0; i < Math.Min(num, 1000); i++) // Limitar para evitar consultas muito grandes
            {
                idsOrdens.Add(todasOrdemFabricoProjeto.DaValor<string>("IDOrdemFabrico"));
                todasOrdemFabricoProjeto.Seguinte();
            }

            // CONSULTA ULTRA-OTIMIZADA com índices e TOP
            var idsOrdensStr = string.Join(",", idsOrdens);

            StdBELista resultCompleto;
            try
            {
                var queryCompleta = $@"
                    WITH OrdemFabricoData AS (
                        SELECT TOP 500
                            G.IDOrdemFabrico,
                            G.OrdemFabrico,
                            G.Artigo,
                            G.QtFabricada,
                            ISNULL(G.CustoMateriaisReal, 0) as CustoMateriaisReal,
                            ISNULL(G.CustoTransformacaoReal, 0) as CustoTransformacaoReal,
                            ISNULL(G.CustoSubprodutosReal, 0) as CustoSubprodutosReal,
                            G.CDU_CodigoProjeto,
                            ISNULL(A.Familia, '') as Familia,
                            ISNULL(A.UnidadeBase, 'UN') as UnidadeBase,
                            ISNULL(F.Descricao, A.Familia) as DescricaoFamilia
                        FROM GPR_OrdemFabrico G WITH(NOLOCK)
                        LEFT JOIN Artigo A WITH(NOLOCK) ON G.Artigo = A.Artigo
                        LEFT JOIN Familias F WITH(NOLOCK) ON A.Familia = F.Familia
                        WHERE G.IDOrdemFabrico IN ({idsOrdensStr})
                    ),
                    OperacoesData AS (
                        SELECT 
                            IDOrdemFabrico,
                            CAST(SubContratacao as bit) as SubContratacao,
                            ISNULL(Descricao, '') as Descricao
                        FROM GPR_OrdemFabricoOperacoes WITH(NOLOCK)
                        WHERE IDOrdemFabrico IN ({idsOrdensStr}) 
                          AND SubContratacao = 1
                    )
                    SELECT TOP 500
                        O.IDOrdemFabrico,
                        O.OrdemFabrico,
                        O.Artigo,
                        ISNULL(O.QtFabricada, '0') as QtFabricada,
                        O.CustoMateriaisReal,
                        O.CustoTransformacaoReal,
                        O.CustoSubprodutosReal,
                        ISNULL(O.CDU_CodigoProjeto, '') as CDU_CodigoProjeto,
                        O.DescricaoFamilia as Familia,
                        O.UnidadeBase,
                        ISNULL(Op.SubContratacao, 0) as SubContratacao,
                        ISNULL(Op.Descricao, '') as Descricao,
                        0 as RececionadoVFS, -- Calcular posteriormente se necessário
                        0 as RececionadoVFA  -- Calcular posteriormente se necessário
                    FROM OrdemFabricoData O
                    INNER JOIN OperacoesData Op ON O.IDOrdemFabrico = Op.IDOrdemFabrico
                    ORDER BY O.OrdemFabrico";

                resultCompleto = BSO.Consulta(queryCompleta);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na consulta principal: {ex.Message}");
                MessageBox.Show("Erro ao carregar dados. Tente novamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return dt;
            }

            // Criar set de artigos existentes no documento para verificação rápida
            var artigosExistentesNoDoc = new HashSet<string>();
            for (int y = 1; y <= this.DocumentoStock.Linhas.NumItens; y++)
            {
                artigosExistentesNoDoc.Add(this.DocumentoStock.Linhas.GetEdita(y).Artigo);
            }

            // Processar resultados - OTIMIZADO com controle de timeout
            var numResultados = Math.Min(resultCompleto.NumLinhas(), 500);
            resultCompleto.Inicio();

            // Adicionar indicador de progresso para operações longas
            var processedCount = 0;
            var startTime = DateTime.Now;

            for (int i = 0; i < numResultados; i++)
            {
                try
                {
                    // Verificar timeout a cada 50 registros
                    if (i % 50 == 0)
                    {
                        var elapsed = DateTime.Now - startTime;
                        if (elapsed.TotalSeconds > 30) // Timeout de 30 segundos
                        {
                            MessageBox.Show($"Processamento interrompido após 30 segundos. Processados {i} de {numResultados} registros.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }

                        // Permitir que a UI responda
                        System.Windows.Forms.Application.DoEvents();
                    }

                    var custoMateriais = Convert.ToDecimal(resultCompleto.DaValor<object>("CustoMateriaisReal") ?? 0);
                    var custoTransformacao = Convert.ToDecimal(resultCompleto.DaValor<object>("CustoTransformacaoReal") ?? 0);
                    var custoSubprodutos = Convert.ToDecimal(resultCompleto.DaValor<object>("CustoSubprodutosReal") ?? 0);
                    var total = custoMateriais + custoTransformacao + custoSubprodutos;

                    var ordemFabricoAtual = resultCompleto.DaValor<string>("OrdemFabrico") ?? "";
                    var artigoAtual = resultCompleto.DaValor<string>("Artigo") ?? "";
                    var isSubContratacao = Convert.ToBoolean(resultCompleto.DaValor<object>("SubContratacao") ?? false);

                    // Calcular receção apenas quando necessário (lazy loading) - com timeout
                    bool jaRececionado = false;
                    try
                    {
                        if (isSubContratacao && !string.IsNullOrEmpty(ordemFabricoAtual))
                        {
                            jaRececionado = VerificarRececaoComCache(true, ordemFabricoAtual, artigoAtual);
                        }
                        else if (!string.IsNullOrEmpty(artigoAtual))
                        {
                            jaRececionado = VerificarRececaoComCache(false, ordemFabricoAtual, artigoAtual);
                        }
                    }
                    catch (Exception recepcaoEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao verificar receção para {artigoAtual}: {recepcaoEx.Message}");
                        jaRececionado = false; // Continuar sem receção
                    }

                    dt.Rows.Add(
                        false,
                        ordemFabricoAtual,
                        artigoAtual,
                        resultCompleto.DaValor<string>("Familia") ?? "",
                        resultCompleto.DaValor<string>("UnidadeBase") ?? "UN",
                        resultCompleto.DaValor<string>("QtFabricada") ?? "0",
                        0.000m,
                        total,
                        resultCompleto.DaValor<string>("Descricao") ?? "",
                        resultCompleto.DaValor<string>("CDU_CodigoProjeto") ?? "",
                        jaRececionado,
                        isSubContratacao
                    );

                    processedCount++;
                }
                catch (Exception ex)
                {
                    // Log do erro e continuar processamento
                    System.Diagnostics.Debug.WriteLine($"Erro ao processar linha {i}: {ex.Message}");
                    continue;
                }

                try
                {
                    resultCompleto.Seguinte();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao avançar para próximo registro: {ex.Message}");
                    break;
                }
            }

            System.Diagnostics.Debug.WriteLine($"Processamento concluído: {processedCount} registros processados em {(DateTime.Now - startTime).TotalSeconds:F2} segundos");

            return dt;
        }

        private bool VerificarRececaoComCache(bool isSubcontratacao, string ordemFabrico, string artigo)
        {
            try
            {
                if (isSubcontratacao)
                {
                    if (_cacheRececaoVFS.ContainsKey(ordemFabrico))
                        return _cacheRececaoVFS[ordemFabrico];

                    var queryRececaoVFS = $@"SELECT TOP 1 COUNT(*) as count
                                            FROM LinhasCompras L WITH(NOLOCK)
                                            INNER JOIN CabecCompras C WITH(NOLOCK) ON L.IdCabecCompras = C.ID
                                            WHERE L.Descricao LIKE '%{ordemFabrico}%' AND C.TipoDoc = 'VFS'";

                    var resultVFS = BSO.Consulta(queryRececaoVFS);
                    bool rececionado = resultVFS.DaValor<int>("count") > 0;
                    _cacheRececaoVFS[ordemFabrico] = rececionado;
                    return rececionado;
                }
                else
                {
                    string chaveCache = $"{artigo}_{projeto}";
                    if (_cacheRececaoVFA.ContainsKey(chaveCache))
                        return _cacheRececaoVFA[chaveCache];

                    var queryRececaoVFA = $@"SELECT TOP 1 COUNT(*) as count
                                            FROM LinhasCompras L WITH(NOLOCK)
                                            INNER JOIN CabecCompras C WITH(NOLOCK) ON L.IdCabecCompras = C.ID
                                            INNER JOIN COP_Obras AS CO WITH(NOLOCK) ON L.ObraID = CO.ID
                                            WHERE L.Artigo = '{artigo}' AND C.TipoDoc = 'VFA' AND CO.Codigo = '{projeto}'";

                    var resultVFA = BSO.Consulta(queryRececaoVFA);
                    bool rececionado = resultVFA.DaValor<int>("count") > 0;
                    _cacheRececaoVFA[chaveCache] = rececionado;
                    return rececionado;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar receção: {ex.Message}");
                return false; // Em caso de erro, assumir que não foi rececionado
            }
        }

        private string ObterFamiliaComCache(string artigo, Dictionary<string, object> cacheArtigos, Dictionary<string, string> cacheFamilias)
        {
            if (!cacheArtigos.ContainsKey(artigo))
            {
                var infoArtigo = BSO.Base.Artigos.Edita(artigo);
                cacheArtigos[artigo] = infoArtigo;
            }

            var artigoInfo = cacheArtigos[artigo];
            if (artigoInfo == null) return "";

            string codigoFamilia = ((dynamic)artigoInfo).Familia;
            if (string.IsNullOrEmpty(codigoFamilia)) return "";

            if (!cacheFamilias.ContainsKey(codigoFamilia))
            {
                var queryFamilia = $"SELECT Descricao FROM Familias WHERE Familia = '{codigoFamilia}'";
                var resultFamilia = BSO.Consulta(queryFamilia);

                if (resultFamilia.NumLinhas() > 0)
                {
                    cacheFamilias[codigoFamilia] = resultFamilia.DaValor<string>("Descricao");
                }
                else
                {
                    cacheFamilias[codigoFamilia] = codigoFamilia;
                }
            }

            return cacheFamilias[codigoFamilia];
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
                        bool existe = VerificarArtigoExisteEmSOF(artigo);

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
            ProgressForm progressForm = null;

            try
            {
                // ⚡ ULTRA-OTIMIZADO: Coleta todos os artigos selecionados primeiro (SEM SQL)
                var artigosSelecionados = new List<(string artigo, string familia, DataRow row)>();
                var servicosIgnorados = new List<string>();

                // Primeira passagem: apenas coleta dados (0% CPU SQL)
                foreach (DataGridViewRow row in dgvOrdensFabrico.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Selecionado"].Value))
                    {
                        DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                        var artigo = dataRow["Artigo"].ToString();
                        var familia = dataRow["Familia"].ToString();

                        // Verificar serviços sem SQL
                        bool isServico = familia == "011" || familia.ToLower().Contains("serviço");
                        if (isServico)
                        {
                            servicosIgnorados.Add(artigo);
                            continue;
                        }

                        artigosSelecionados.Add((artigo, familia, dataRow));
                    }
                }

                if (artigosSelecionados.Count == 0)
                {
                    string mensagemVazia = "Nenhum artigo válido selecionado.";
                    if (servicosIgnorados.Count > 0)
                    {
                        mensagemVazia += $"\n\nServiços ignorados: {string.Join(", ", servicosIgnorados)}";
                    }
                    MessageBox.Show(mensagemVazia);
                    return;
                }

                // 🔄 MOSTRAR LOADING
                progressForm = new ProgressForm();
                progressForm.UpdateProgress(0, $"Preparando processamento de {artigosSelecionados.Count} artigos...");
                progressForm.Show();
                progressForm.BringToFront();
                System.Windows.Forms.Application.DoEvents();

                // ⚡ CONSULTA ÚNICA EM LOTE: Verificar TODOS os artigos de uma vez
                progressForm.UpdateProgress(20, "Verificando artigos existentes...");
                System.Windows.Forms.Application.DoEvents();

                var artigosStr = string.Join("','", artigosSelecionados.Select(x => x.artigo));
                var queryBatch = $@"
                    SELECT DISTINCT LI.Artigo
                    FROM CabecInternos CI WITH(NOLOCK)
                    JOIN LinhasInternos LI WITH(NOLOCK) ON CI.Id = LI.IdCabecInternos
                    JOIN GPR_OrdemFabrico GF WITH(NOLOCK) ON CI.IdOrdemFabrico = GF.IDOrdemFabrico
                    WHERE CI.TipoDoc = 'SOF' 
                      AND GF.CDU_CodigoProjeto = '{projeto}'
                      AND LI.Artigo IN ('{artigosStr}')
                      AND CI.IDOperadorGPR > 0";

                // ⚡ UMA ÚNICA CONSULTA SQL para todos os artigos
                var artigosExistentesResult = BSO.Consulta(queryBatch);
                var artigosExistentesHashSet = new HashSet<string>();

                progressForm.UpdateProgress(40, "Processando resultados da consulta...");
                System.Windows.Forms.Application.DoEvents();

                // Processar resultado uma única vez
                var numExistentes = artigosExistentesResult.NumLinhas();
                artigosExistentesResult.Inicio();
                for (int i = 0; i < numExistentes; i++)
                {
                    artigosExistentesHashSet.Add(artigosExistentesResult.DaValor<string>("Artigo"));
                    artigosExistentesResult.Seguinte();
                }

                // ⚡ Separar artigos para processar (sem mais SQL)
                progressForm.UpdateProgress(60, "Separando artigos para processamento...");
                System.Windows.Forms.Application.DoEvents();

                var artigosParaAdicionar = new List<(string artigo, DataRow row)>();
                var artigosJaExistentes = new List<string>();

                foreach (var (artigo, familia, row) in artigosSelecionados)
                {
                    if (artigosExistentesHashSet.Contains(artigo))
                    {
                        artigosJaExistentes.Add(artigo);
                    }
                    else
                    {
                        artigosParaAdicionar.Add((artigo, row));
                    }
                }

                // ⚡ ADICIONAR TODOS de uma vez ao documento
                if (artigosParaAdicionar.Count > 0)
                {
                    progressForm.UpdateProgress(80, $"Adicionando {artigosParaAdicionar.Count} artigos ao documento...");
                    System.Windows.Forms.Application.DoEvents();

                    var documento = BSO.Internos.Documentos.Edita(this.DocumentoStock.Tipodoc, this.DocumentoStock.NumDoc, this.DocumentoStock.Serie, this.DocumentoStock.Filial);

                    int processados = 0;
                    foreach (var (artigo, row) in artigosParaAdicionar)
                    {
                        try
                        {
                            // Atualizar progresso a cada 10 artigos
                            if (processados % 10 == 0)
                            {
                                int progressoArtigos = 80 + (int)((double)processados / artigosParaAdicionar.Count * 15);
                                progressForm.UpdateProgress(progressoArtigos, $"Processando artigo {processados + 1} de {artigosParaAdicionar.Count}: {artigo}");
                                System.Windows.Forms.Application.DoEvents();
                            }

                            // Processar dados sem consultas SQL adicionais
                            var Qtf = row["QtFabricada"].ToString();
                            var total = row["Total"].ToString();

                            // Calcular preço unitário otimizado
                            double precoUnitario = 0.000;
                            if (double.TryParse(Qtf, out double quantidadeFabricada) && double.TryParse(total, out double totalValue))
                            {
                                if (cbTipoLista.SelectedIndex >= 1)
                                {
                                    precoUnitario = totalValue;
                                }
                                else
                                {
                                    if (totalValue != 0)
                                    {
                                        precoUnitario = quantidadeFabricada != 0 ? totalValue / quantidadeFabricada : totalValue;
                                    }
                                }
                            }

                            // ⚡ Adicionar linha sem validações desnecessárias
                            BSO.Internos.Documentos.AdicionaLinha(documento, artigo);
                            processados++;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao processar artigo {artigo}: {ex.Message}");
                        }
                    }

                    progressForm.UpdateProgress(95, "Validando e atualizando documento...");
                    System.Windows.Forms.Application.DoEvents();

                    // ⚡ UMA ÚNICA validação e atualização no final
                    var error = "";
                    BSO.Internos.Documentos.ValidaActualizacao(documento, ref error);
                    BSO.Internos.Documentos.Actualiza(documento);
                }

                progressForm.UpdateProgress(100, "Processamento concluído!");
                System.Windows.Forms.Application.DoEvents();

                // Pequena pausa para mostrar 100%
                System.Threading.Thread.Sleep(500);

                // Fechar loading
                progressForm.Close();
                progressForm = null;

                // Construir mensagem final
                string mensagem = "";
                if (artigosParaAdicionar.Count > 0)
                {
                    mensagem = $"✅ {artigosParaAdicionar.Count} artigos adicionados ao SOF com sucesso!";
                }

                if (artigosJaExistentes.Count > 0)
                {
                    string ignorados = string.Join(", ", artigosJaExistentes.Take(10));
                    if (artigosJaExistentes.Count > 10) ignorados += "...";

                    if (!string.IsNullOrEmpty(mensagem))
                    {
                        mensagem += $"\n\n⚠️ {artigosJaExistentes.Count} artigos já existentes (ignorados): {ignorados}";
                    }
                    else
                    {
                        mensagem = $"⚠️ Artigos já existentes no SOF (ignorados): {ignorados}";
                    }
                }

                if (servicosIgnorados.Count > 0)
                {
                    string servicosTexto = string.Join(", ", servicosIgnorados.Take(5));
                    if (servicosIgnorados.Count > 5) servicosTexto += "...";

                    if (!string.IsNullOrEmpty(mensagem))
                    {
                        mensagem += $"\n\nℹ️ {servicosIgnorados.Count} serviços ignorados: {servicosTexto}";
                    }
                    else
                    {
                        mensagem = $"ℹ️ Serviços ignorados: {servicosTexto}";
                    }
                }

                MessageBox.Show(mensagem, "Processamento Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // Fechar loading em caso de erro
                if (progressForm != null)
                {
                    progressForm.Close();
                }

                MessageBox.Show($"Erro durante o processamento: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            var query = $@"SELECT TOP 1000
                            LC.Artigo,
                            ISNULL(LC.Descricao, '') as Descricao,
                            ISNULL(LC.Lote, '') as Lote,
                            ISNULL(LC.Quantidade, 0) as Quantidade,
                            ISNULL(A.Familia, '') as Familia,
                            CC.NumDoc,
                            CC.Serie,
                            CO.Codigo,
                            ISNULL(OFA.OrdemFabrico, '') as OrdemFabrico,
                            ISNULL(LC.PrecUnit, 0) as PrecUnit,
                            ISNULL(LC.PrecoLiquido, 0) as PrecoLiquido
                        FROM 
                            CabecCompras AS CC WITH(NOLOCK)
                        INNER JOIN 
                            LinhasCompras AS LC WITH(NOLOCK) ON CC.Id = LC.IdCabecCompras
                        INNER JOIN 
                            Artigo AS A WITH(NOLOCK) ON LC.Artigo = A.Artigo
                        INNER JOIN 
                            COP_Obras AS CO WITH(NOLOCK) ON LC.ObraID = CO.ID
                        LEFT JOIN 
                            GPR_OrdemFabrico AS OFA WITH(NOLOCK) ON LC.Artigo = OFA.Artigo
                        WHERE 
                            CC.TipoDoc = 'ECF'
                            AND LC.Artigo IS NOT NULL
                            AND A.Familia IN ('004', '024')
                            AND LC.ObraID IS NOT NULL
                            AND CO.Codigo = '{codigoProjeto}'
                        ORDER BY LC.Artigo";

            return BSO.Consulta(query);
        }

        private void CarregarArtigosPorFamiliaCompleto(string familia)
        {
            var artigosFamilia = GetArtigosPorFamilia(projeto, familia);
            _dadosCompletos = ConvertArtigosEletricosToDataTable(artigosFamilia);
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

        private void CarregarTodosOsArtigosCompleto()
        {
            var todosOsArtigos = GetTodosOsArtigos(projeto);
            _dadosCompletos = ConvertArtigosEletricosToDataTable(todosOsArtigos);
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
            // Usar EXISTS para melhor performance
            var queryVerificacao = $@"SELECT CASE WHEN EXISTS (
                                       SELECT 1
                                       FROM CabecInternos CI
                                       JOIN LinhasInternos LI ON CI.Id = LI.IdCabecInternos
                                       JOIN Artigo A ON LI.Artigo = A.Artigo
                                       JOIN GPR_OrdemFabrico GF ON CI.IdOrdemFabrico = GF.IDOrdemFabrico
                                       WHERE CI.TipoDoc = 'SOF' 
                                         AND GF.CDU_CodigoProjeto = '{projeto}'
                                         AND A.Familia IN ('001','002','003','004','005','006','011','024')
                                   ) THEN 1 ELSE 0 END AS existe";

            var resultado = BSO.Consulta(queryVerificacao);
            resultado.Inicio();

            return resultado.DaValor<int>("existe") == 1;
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
            if (num == 0) return dt;

            // Coletar todos os artigos para consulta em lote
            var todosArtigos = new List<string>();
            artigosEletricos.Inicio();
            for (int i = 0; i < num; i++)
            {
                todosArtigos.Add(artigosEletricos.DaValor<string>("Artigo"));
                artigosEletricos.Seguinte();
            }

            // Consulta otimizada em lote para artigos, famílias e receções
            var artigosStr = string.Join("','", todosArtigos);
            var queryCompleta = $@"
                WITH ArtigosData AS (
                    SELECT 
                        A.Artigo,
                        A.UnidadeBase,
                        A.Familia,
                        F.Descricao as DescricaoFamilia,
                        CASE WHEN A.Familia = '011' THEN 1 ELSE 0 END as IsServico
                    FROM Artigo A
                    LEFT JOIN Familias F ON A.Familia = F.Familia
                    WHERE A.Artigo IN ('{artigosStr}')
                ),
                RececaoVFA AS (
                    SELECT DISTINCT L.Artigo
                    FROM LinhasCompras L
                    INNER JOIN CabecCompras C ON L.IdCabecCompras = C.ID
                    INNER JOIN COP_Obras CO ON L.ObraID = CO.ID
                    WHERE L.Artigo IN ('{artigosStr}')
                      AND C.TipoDoc = 'VFA' 
                      AND CO.Codigo = '{projeto}'
                )
                SELECT 
                    A.Artigo,
                    A.UnidadeBase,
                    ISNULL(A.DescricaoFamilia, A.Familia) as Familia,
                    A.IsServico,
                    CASE WHEN R.Artigo IS NOT NULL THEN 1 ELSE 0 END as Rececionado
                FROM ArtigosData A
                LEFT JOIN RececaoVFA R ON A.Artigo = R.Artigo";

            var resultCompleto = BSO.Consulta(queryCompleta);

            // Criar dicionários para lookup rápido
            var dadosArtigos = new Dictionary<string, (string Unidade, string Familia, bool IsServico, bool Rececionado)>();
            var numResultados = resultCompleto.NumLinhas();
            resultCompleto.Inicio();

            for (int i = 0; i < numResultados; i++)
            {
                var artigo = resultCompleto.DaValor<string>("Artigo");
                dadosArtigos[artigo] = (
                    resultCompleto.DaValor<string>("UnidadeBase") ?? "UN",
                    resultCompleto.DaValor<string>("Familia") ?? "",
                    resultCompleto.DaValor<int>("IsServico") == 1,
                    resultCompleto.DaValor<int>("Rececionado") == 1
                );
                resultCompleto.Seguinte();
            }

            // Criar set de artigos existentes no documento para verificação rápida
            var artigosExistentesNoDoc = new HashSet<string>();
            for (int y = 1; y <= this.DocumentoStock.Linhas.NumItens; y++)
            {
                artigosExistentesNoDoc.Add(this.DocumentoStock.Linhas.GetEdita(y).Artigo);
            }

            // Processar dados dos artigos elétricos
            artigosEletricos.Inicio();
            for (int i = 0; i < num; i++)
            {
                var artigo = artigosEletricos.DaValor<string>("Artigo");
                var ordemFabrico = artigosEletricos.DaValor<string>("OrdemFabrico");
                var descricao = artigosEletricos.DaValor<string>("Descricao");
                var quantidade = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("Quantidade")));
                var precoUnitario = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("PrecUnit")));
                var precoLiquido = Math.Abs(Convert.ToDouble(artigosEletricos.DaValor<string>("PrecoLiquido")));

                // Usar dados do lookup
                var dadosArtigo = dadosArtigos.ContainsKey(artigo)
                    ? dadosArtigos[artigo]
                    : ("UN", "", false, false);

                dt.Rows.Add(
                    false, // Checkbox sempre false inicialmente
                    ordemFabrico ?? "",
                    artigo,
                    dadosArtigo.Item2, // Familia
                    dadosArtigo.Item1, // Unidade
                    quantidade,
                    precoLiquido,
                    precoUnitario,
                    descricao,
                    projeto,
                    dadosArtigo.Item4, // Rececionado
                    dadosArtigo.Item3  // IsServico
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

        private bool VerificarArtigoExisteEmSOF(string artigo)
        {
            if (string.IsNullOrEmpty(artigo)) return false;

            if (_cacheArtigosSOF.ContainsKey(artigo))
                return _cacheArtigosSOF[artigo];

            try
            {
                // Usar EXISTS com NOLOCK para melhor performance
                var query = $@"SELECT CASE WHEN EXISTS (
                                  SELECT TOP 1 1 
                                  FROM CabecInternos CI WITH(NOLOCK)
                                  JOIN LinhasInternos LI WITH(NOLOCK) ON CI.Id = LI.IdCabecInternos
                                  JOIN GPR_OrdemFabrico GF WITH(NOLOCK) ON CI.IdOrdemFabrico = GF.IDOrdemFabrico
                                  WHERE CI.TipoDoc = 'SOF' 
                                    AND GF.CDU_CodigoProjeto = '{projeto}'
                                    AND LI.Artigo = '{artigo}'
                                    AND CI.IDOperadorGPR > 0
                              ) THEN 1 ELSE 0 END AS existe";

                var response = BSO.Consulta(query);
                if (response.NumLinhas() > 0)
                {
                    response.Inicio();
                    bool existe = response.DaValor<int>("existe") == 1;
                    _cacheArtigosSOF[artigo] = existe;
                    return existe;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar artigo {artigo}: {ex.Message}");
            }

            _cacheArtigosSOF[artigo] = false;
            return false;
        }

        // Método otimizado para verificar múltiplos artigos de uma vez
        private Dictionary<string, bool> VerificarArtigosExistemEmSOFBatch(List<string> artigos)
        {
            var resultado = new Dictionary<string, bool>();
            var artigosParaConsultar = new List<string>();

            // Verificar cache primeiro
            foreach (var artigo in artigos)
            {
                if (_cacheArtigosSOF.ContainsKey(artigo))
                {
                    resultado[artigo] = _cacheArtigosSOF[artigo];
                }
                else
                {
                    artigosParaConsultar.Add(artigo);
                }
            }

            // Consultar artigos não encontrados no cache
            if (artigosParaConsultar.Count > 0)
            {
                var artigosStr = string.Join("','", artigosParaConsultar);
                var query = $@"SELECT DISTINCT LI.Artigo
                              FROM CabecInternos CI
                              JOIN LinhasInternos LI ON CI.Id = LI.IdCabecInternos
                              JOIN [GPR_OrdemFabrico] GF ON CI.IdOrdemFabrico = GF.IDOrdemFabrico
                              WHERE CI.TipoDoc = 'SOF' 
                                AND GF.CDU_CodigoProjeto = '{projeto}'
                                AND LI.Artigo IN ('{artigosStr}')
                                AND CI.IDOperadorGPR > 0";

                var response = BSO.Consulta(query);
                var artigosExistentes = new HashSet<string>();

                var numResultados = response.NumLinhas();
                response.Inicio();
                for (int i = 0; i < numResultados; i++)
                {
                    artigosExistentes.Add(response.DaValor<string>("Artigo"));
                    response.Seguinte();
                }

                // Processar resultados e atualizar cache
                foreach (var artigo in artigosParaConsultar)
                {
                    bool existe = artigosExistentes.Contains(artigo);
                    resultado[artigo] = existe;
                    _cacheArtigosSOF[artigo] = existe;
                }
            }

            return resultado;
        }

        private void AplicarPaginacao()
        {
            if (_dadosCompletos == null)
            {
                dgvOrdensFabrico.DataSource = null;
                AtualizarInfoPaginacao();
                return;
            }

            _totalItens = _dadosCompletos.Rows.Count;

            if (cbItensPorPagina.SelectedItem?.ToString() == "Todos" || _itensPorPagina <= 0)
            {
                dgvOrdensFabrico.DataSource = _dadosCompletos;
                _totalPaginas = 1;
                _paginaAtual = 1;
            }
            else
            {
                _totalPaginas = (int)Math.Ceiling((double)_totalItens / _itensPorPagina);

                if (_paginaAtual > _totalPaginas && _totalPaginas > 0)
                    _paginaAtual = _totalPaginas;

                if (_paginaAtual < 1)
                    _paginaAtual = 1;

                var dadosPagina = ObterDadosPagina();
                dgvOrdensFabrico.DataSource = dadosPagina;
            }

            AtualizarInfoPaginacao();
            AtualizarEstadoBotoesPaginacao();
        }

        private System.Data.DataTable ObterDadosPagina()
        {
            var dadosPagina = _dadosCompletos.Clone();

            int inicio = (_paginaAtual - 1) * _itensPorPagina;
            int fim = Math.Min(inicio + _itensPorPagina, _totalItens);

            for (int i = inicio; i < fim; i++)
            {
                dadosPagina.ImportRow(_dadosCompletos.Rows[i]);
            }

            return dadosPagina;
        }

        private void AtualizarInfoPaginacao()
        {
            if (_totalItens == 0)
            {
                lblPagina.Text = "Nenhum item encontrado";
            }
            else if (cbItensPorPagina.SelectedItem?.ToString() == "Todos")
            {
                lblPagina.Text = $"Todos os {_totalItens} itens";
            }
            else
            {
                int inicio = (_paginaAtual - 1) * _itensPorPagina + 1;
                int fim = Math.Min(_paginaAtual * _itensPorPagina, _totalItens);
                lblPagina.Text = $"Página {_paginaAtual} de {_totalPaginas} ({inicio}-{fim} de {_totalItens})";
            }
        }

        private void AtualizarEstadoBotoesPaginacao()
        {
            bool temItens = _totalItens > 0;
            bool multipalasPaginas = _totalPaginas > 1;
            bool naoEPrimeira = _paginaAtual > 1;
            bool naoEUltima = _paginaAtual < _totalPaginas;

            btnPrimeira.Enabled = temItens && multipalasPaginas && naoEPrimeira;
            btnAnterior.Enabled = temItens && multipalasPaginas && naoEPrimeira;
            btnProxima.Enabled = temItens && multipalasPaginas && naoEUltima;
            btnUltima.Enabled = temItens && multipalasPaginas && naoEUltima;
        }

        private void btnPrimeira_Click(object sender, EventArgs e)
        {
            _paginaAtual = 1;
            AplicarPaginacao();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_paginaAtual > 1)
            {
                _paginaAtual--;
                AplicarPaginacao();
            }
        }

        private void btnProxima_Click(object sender, EventArgs e)
        {
            if (_paginaAtual < _totalPaginas)
            {
                _paginaAtual++;
                AplicarPaginacao();
            }
        }

        private void btnUltima_Click(object sender, EventArgs e)
        {
            _paginaAtual = _totalPaginas;
            AplicarPaginacao();
        }

        private void cbItensPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            string valorSelecionado = cbItensPorPagina.SelectedItem?.ToString();

            if (valorSelecionado == "Todos")
            {
                _itensPorPagina = -1; // Indicador para mostrar todos
            }
            else if (int.TryParse(valorSelecionado, out int novoValor))
            {
                _itensPorPagina = novoValor;
            }

            _paginaAtual = 1;
            AplicarPaginacao();
        }

        private void cbTipoLista_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Limpar cache quando mudar de tipo de lista
            _cacheArtigosSOF.Clear();
            _cacheRececaoVFS.Clear();
            _cacheRececaoVFA.Clear();

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
