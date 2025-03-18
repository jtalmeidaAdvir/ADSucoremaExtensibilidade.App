using Primavera.Extensibility.CustomForm;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public partial class EditorArtigosUnidade : CustomForm
    {
        public EditorArtigosUnidade()
        {
            InitializeComponent();
        }

        private void F4_Click(object sender, System.EventArgs e)
        {
            MetodoGetArtigos();
        }

        private void MetodoGetArtigos()
        {
            Dictionary<string, string> artigos = new Dictionary<string, string>();
            GetArtigos(ref artigos);

            if (artigos.Count > 0)
            {
                SetInfoArtigos(artigos);
            }
        }

        private void SetInfoArtigos(Dictionary<string, string> artigos)
        {
            TXT_Artigo.Text = artigos["Artigo"];
            Venda.SelectedItem  = artigos["UnidadeVenda"];
            Compra.SelectedItem = artigos["unidadeCompra"];
            Base.SelectedItem = artigos["UnidadeBase"];
            Saida.SelectedItem = artigos["Unidadesaida"];
            Entrada.SelectedItem = artigos["UnidadeEntrada"];
        }

        private void GetArtigos(ref Dictionary<string, string> artigos)
        {
            string NomeLista = "Artigo";
            string Campos = "Artigo,Descricao,UnidadeVenda,UnidadeBase,Unidadesaida,unidadeCompra,UnidadeEntrada";
            string Tabela = "Artigo (NOLOCK)";
            string Where = "";
            string CamposF4 = "Artigo,Descricao,UnidadeVenda,UnidadeBase,Unidadesaida,unidadeCompra,UnidadeEntrada";
            string orderby = "Artigo";

            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, ref ResQuery);

            if (ResQuery.Count > 0)
            {
                string[] colunas = CamposF4.Split(',');
                for (int i = 0; i < colunas.Length; i++)
                {
                    if (i < ResQuery.Count)
                    {
                        artigos[colunas[i].Trim()] = ResQuery[i].ToString();
                    }
                }
            }
        }

        private void OpenF4List(string campos, string tabela, string where, string camposF4, string orderby, string nomeLista, ref List<string> resQuery)
        {
            string strSQL = "select distinct " + campos + " FROM " + tabela;

            if (where.Length > 0)
            {
                strSQL += " WHERE " + where;
            }

            strSQL += " Order by " + orderby;
            string result = Convert.ToString(PSO.Listas.GetF4SQL(nomeLista, strSQL, camposF4));

            if (!string.IsNullOrEmpty(result))
            {
                string[] itemQuery = result.Split('\t');
                resQuery.AddRange(itemQuery);
            }
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            var updateartigo = $@"
                                UPDATE Artigo
                                SET UnidadeVenda = '{Venda.SelectedItem.ToString()}',
	                                UnidadeBase = '{Base.SelectedItem.ToString()}',
	                                Unidadesaida = '{Saida.SelectedItem.ToString()}',
	                                unidadeCompra = '{Compra.SelectedItem.ToString()}',
	                                UnidadeEntrada = '{Entrada.SelectedItem.ToString()}'
                                WHERE Artigo = '{TXT_Artigo.Text}'";
            BSO.DSO.ExecuteSQL(updateartigo);

            var updateArtigoMoeda = $@"
                                UPDATE ArtigoMoeda
                                SET Unidade = '{Base.SelectedItem.ToString()}'
                                WHERE Artigo = '{TXT_Artigo.Text}'";
            BSO.DSO.ExecuteSQL(updateArtigoMoeda);

            MessageBox.Show("Atualização realizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
    }
}
