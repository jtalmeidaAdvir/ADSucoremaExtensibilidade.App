using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.CustomForm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public partial class EditorValores : CustomForm
    {
        public EditorValores()
        {
            InitializeComponent();
        }

        public string OrdemFabrico { get; private set; }
        public string Artigo { get; private set; }

        private void bt_localizar_Click(object sender, EventArgs e)
        {
            OrdemFabrico = TXT_OrdemFabrico.Text;
           
            var getArtigo = $@"SELECT Artigo FROM GPR_OrdemFabrico WHERE OrdemFabrico = '{OrdemFabrico}'";

            var artigo = BSO.Consulta(getArtigo);

            Artigo = artigo.DaValor<string>("Artigo");

            txt_artigo.Text = Artigo;

            /*var querySOF = $@"		SELECT 
                                ABS(l.PrecUnit) AS PrecUnit,
                                l.Quantidade
                            FROM GPR_OrdemFabrico o
                            JOIN CabecInternos c ON c.IdOrdemFabrico = o.IDOrdemFabrico
                            JOIN LinhasInternos l ON l.IdCabecInternos = c.Id
                            WHERE l.Artigo = '{Artigo}' 
                            AND c.TipoDoc = 'SOF';";

            var valorSOF = BSO.Consulta(querySOF);

              txt_sof.Text = valorSOF.DaValor<string>("PrecUnit");
            */



            var queryQuantidade = $@"SELECT QtFabricada FROM GPR_OrdemFabrico WHERE ordemFabrico = '{OrdemFabrico}'";
            var quantidades = BSO.Consulta(queryQuantidade);

          
            TXT_qtdsof.Text = quantidades.DaValor<string>("QtFabricada");


            var queryEOF = $@"SELECT 
                                ABS(l.PrecUnit) AS PrecUnit,
                                ABS(l.Quantidade) AS Quantidade
                            FROM GPR_OrdemFabrico o
                            JOIN CabecInternos c ON c.IdOrdemFabrico = o.IDOrdemFabrico
                            JOIN LinhasInternos l ON l.IdCabecInternos = c.Id
                            WHERE o.OrdemFabrico = '{OrdemFabrico}' 
                            AND c.TipoDoc = 'EOF';";

            var valorEOF = BSO.Consulta(queryEOF);

            txt_valorEOF.Text = valorEOF.DaValor<string>("PrecUnit");
            TXT_qtdeof.Text = quantidades.DaValor<string>("QtFabricada");

            var queryOF = $@"SELECT 
                            COALESCE(CustoMateriaisReal, 0) +
                            COALESCE(CustoSubprodutosReal, 0) +
                            COALESCE(CustoTransformacaoReal, 0) +
                            COALESCE(OutrosCustosReal, 0) AS TotalCusto
                        FROM GPR_OrdemFabrico
                        WHERE OrdemFabrico = '{OrdemFabrico}';";

            var valorOF = BSO.Consulta(queryOF);

            TXT_ValorOF.Text = valorOF.DaValor<string>("TotalCusto");

            var queryOF30 = $@"SELECT 
                            (COALESCE(CustoMateriaisReal, 0) * 1.3) +
                            COALESCE(CustoSubprodutosReal, 0) +
                            COALESCE(CustoTransformacaoReal, 0) +
                            COALESCE(OutrosCustosReal, 0) AS TotalCusto
                        FROM GPR_OrdemFabrico
                        WHERE OrdemFabrico = '{OrdemFabrico}';";

            var valorOF30 = BSO.Consulta(queryOF30);

            TXT_ValorOF30.Text = valorOF30.DaValor<string>("TotalCusto");

        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TXT_OrdemFabrico.Text))
            {
                // Exibe uma mensagem se o campo estiver vazio ou nulo
                MessageBox.Show("É necessário selecionar uma ordem de fabrico válida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Interrompe a execução do código
            }

            // Verifica se o campo de valor da Ordem de Fabrico está vazio
            if (string.IsNullOrEmpty(TXT_ValorOF30.Text))
            {
                // Exibe uma mensagem indicando que o botão "Localizar Valores" deve ser clicado
                MessageBox.Show("É necessário clicar no botão 'Localizar' antes de continuar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Interrompe a execução do código
            }

            // Pega o valor do campo de texto, que está com a vírgula
            var numberStr = TXT_ValorOF30.Text;

            var quantidadeSOF = TXT_qtdsof.Text;
            var quantidadeEOF = TXT_qtdeof.Text;


            // Substitui a vírgula por ponto
            numberStr = numberStr.Replace(",", ".");

            quantidadeSOF = quantidadeSOF.Replace(",", ".");
            quantidadeEOF = quantidadeEOF.Replace(",", ".");



            //////////////////////////////
            


            var querydocEOF = $@"SELECT 
                                c.TipoDoc, c.NumDoc, c.Serie, c.Filial, l.Artigo
                            FROM GPR_OrdemFabrico g
                            LEFT JOIN CabecInternos c ON g.IDOrdemFabrico = c.IdordemFabrico
                            LEFT JOIN LinhasInternos l ON c.Id = l.IdcabecInternos
                            WHERE g.OrdemFabrico = '{OrdemFabrico}' AND l.Artigo = '{Artigo}';
                            ";
            var docsql = BSO.Consulta(querydocEOF);
            var num = docsql.NumLinhas();
            docsql.Inicio();
            for (int i = 0; i < num; i++)
            {
                var tipoDoc = docsql.DaValor<string>("TipoDoc");
                var numDoc = docsql.DaValor<int>("NumDoc");
                var serieDoc = docsql.DaValor<string>("Serie");
                var filialDoc = docsql.DaValor<string>("Filial");

                var doc = BSO.Internos.Documentos.Edita(tipoDoc, numDoc, serieDoc, filialDoc);
                var numli = doc.Linhas.NumItens;
                for (int y = 1; y < numli + 1; y++)
                {
                    var linha = doc.Linhas.GetEdita(y);
                    if(linha.Artigo == Artigo)
                    {
                        if (double.TryParse(numberStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double valor) &&
                        double.TryParse(quantidadeEOF, NumberStyles.Any, CultureInfo.InvariantCulture, out double quantidade) &&
                        quantidade != 0)
                        {

                            double resultado = valor / quantidade;
                            linha.PrecUnit = resultado;

                        }
                    }

                }

                BSO.Internos.Documentos.Actualiza(doc);

                docsql.Seguinte();
            }


            MessageBox.Show("Ordem de Fabrico atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Fecha o formulário
            this.Close();
        }

        private void BTF4_Click(object sender, EventArgs e)
        {
            string code = string.Empty;

            GetProjects(ref code);

            TXT_OrdemFabrico.Text = code;
            txt_valorEOF.Text = "";
            TXT_ValorOF.Text = "";
            TXT_ValorOF30.Text = "";
            txt_artigo.Text = "";
            txt_sof.Text = "";
        }

        private void GetProjects(ref string code)
        {
            string NomeLista = "Ordens de Fabrico";
            string Campos = "OrdemFabrico, Artigo, CustoMateriaisReal, CustoSubprodutosReal, CustoTransformacaoReal, OutrosCustosReal, CDU_CodigoProjeto";
            string Tabela = "CabecInternos c JOIN GPR_OrdemFabrico o ON c.IdOrdemFabrico = o.IDOrdemFabrico"; // Junção entre CabecInternos e GPR_OrdemFabrico
            string Where = "c.TipoDoc = 'EOF'";  // Condição para TipoDoc
            string CamposF4 = "OrdemFabrico, Artigo, CustoMateriaisReal, CustoSubprodutosReal, CustoTransformacaoReal, OutrosCustosReal, CDU_CodigoProjeto";
            string orderby = "o.OrdemFabrico";  // Ordenação por OrdemFabrico
            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, ref ResQuery);

            if (ResQuery.Count > 0)
            {
                code = ResQuery.ElementAt(0);
               
            }
        }

        private void OpenF4List(string campos, string tabela, string where, string camposF4, string orderby, string nomeLista, ref List<string> resQuery)
        {
            string strSQL = "select distinct " + campos + " FROM " + tabela;

            if (where.Length > 0)
            {
                strSQL = strSQL + " WHERE " + where;
            }

            strSQL = strSQL + " Order by " + orderby;


            string result = Convert.ToString(PSO.Listas.GetF4SQL(nomeLista, strSQL, camposF4));

            if (!string.IsNullOrEmpty(result))
            {
                string[] itemQuery = result.Split('\t');

                foreach (string item in itemQuery)
                {
                    resQuery.Add(item);
                }
            }

        }
    }
}
