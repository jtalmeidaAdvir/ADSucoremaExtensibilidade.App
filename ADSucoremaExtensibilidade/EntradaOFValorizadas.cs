using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.CustomForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public partial class EntradaOFValorizadas : CustomForm
    {

        public EntradaOFValorizadas()
        {
            InitializeComponent();
        }

        private void GetValores()
        {
            var query = $@"
                        SELECT 
                            G.OrdemFabrico,
                            G.Artigo,
                            G.QtFabricada,
                            T.TotalCusto,
                            T.TotalCustoCom30
                        FROM GPR_OrdemFabrico G
                        JOIN (
                            SELECT 
                                OrdemFabrico,
                                SUM(CustoMateriaisReal + CustoTransformacaoReal + CustoSubprodutosReal + OutrosCustosReal) AS TotalCusto,
                                SUM(CustoMateriaisReal * 1.3) + SUM(CustoTransformacaoReal) + SUM(CustoSubprodutosReal) + SUM(OutrosCustosReal) AS TotalCustoCom30
                            FROM GPR_OrdemFabrico
                            WHERE Fechada = 1 
                                AND Estado = 5 
                                AND Confirmada = 1 
                            GROUP BY OrdemFabrico
                            HAVING SUM(CustoMateriaisReal + CustoTransformacaoReal + CustoSubprodutosReal + OutrosCustosReal) != 
                                   (SUM(CustoMateriaisReal * 1.3) + SUM(CustoTransformacaoReal) + SUM(CustoSubprodutosReal) + SUM(OutrosCustosReal))
                        ) T ON G.OrdemFabrico = T.OrdemFabrico
                        WHERE G.Fechada = 1 
                            AND G.Estado = 5 
                            AND G.Confirmada = 1
                            AND G.QtFabricada > 0  -- Adding the condition that QtFabricada must be greater than 0
                        ORDER BY G.OrdemFabrico;


            ";

            var lista = BSO.Consulta(query);

            var num = lista.NumLinhas();


            lista.Inicio();

            for (int i = 0; i < num; i++)
            {
                var of = lista.DaValor<string>("OrdemFabrico");
                var artigo = lista.DaValor<string>("Artigo");
                var valor = lista.DaValor<string>("TotalCusto");
                var valorC30 = lista.DaValor<string>("TotalCustoCom30");

                // Adicionando uma linha diretamente
                dataGridView1.Rows.Add(of, artigo, valor, valorC30);

                lista.Seguinte();
            }

        }


        private void EntradaOFValorizadas_Load(object sender, EventArgs e)
        {
            // Carrega os primeiros registros ao iniciar
            GetValores();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Percorre todas as linhas do DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Verifica se o checkbox na primeira célula da linha é do tipo DataGridViewCheckBoxCell
                var checkBoxCell = row.Cells[4] as DataGridViewCheckBoxCell;

                if (checkBoxCell != null)
                {
                    // Define o valor do checkbox conforme o estado de checkBox1
                    checkBoxCell.Value = checkBox1.Checked;
                }
            }
        }

        private void bt_atualizar_Click(object sender, EventArgs e)
        {
            // Crie uma lista para armazenar os artigos selecionados
            var artigosSelecionados = new List<string>();

            // Percorre todas as linhas do DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Verifica se a célula de checkbox na coluna 4 (índice 4) é marcada
                var checkBoxCell = row.Cells[4] as DataGridViewCheckBoxCell;

                if (checkBoxCell != null && checkBoxCell.Value != null && (bool)checkBoxCell.Value)
                {
                    // Se o checkbox estiver marcado, pega o valor da coluna 2 (Artigo)
                    var artigo = row.Cells[1].Value?.ToString(); // Índice 1, pois a coluna "Artigo" é a segunda

                    if (!string.IsNullOrEmpty(artigo))
                    {
                        artigosSelecionados.Add(artigo); // Adiciona o artigo à lista
                    }
                }
            }

            // Verifica se a série foi selecionada
            if (cb_serie.SelectedItem == null || artigosSelecionados.Count == 0)
            {
                // Se nenhum artigo foi selecionado ou a série não foi escolhida, não faz nada
                MessageBox.Show("Selecione ao menos um artigo e uma série para atualizar.");
                return; // Não executa o restante do código
            }

            // Agora você tem os artigos selecionados na lista 'artigosSelecionados'
            foreach (var artigo in artigosSelecionados)
            {
                // Aqui você pode usar o valor de 'artigo' como necessário, por exemplo, executando a consulta
                // Aqui usamos cb_serie.SelectedItem.ToString(), assumindo que você tenha configurado corretamente
                var serieSelecionada = cb_serie.SelectedItem.ToString();

                var query = $@"SELECT LI.Artigo, LI.PrecoLiquido, LI.PrecUnit, CI.NumDoc
                        FROM CabecInternos CI
                        JOIN LinhasInternos LI ON CI.ID = LI.IdCabecInternos
                        WHERE CI.TipoDoc = 'EOF' 
                        AND LI.Artigo = '{artigo}' 
                        AND Serie = '{serieSelecionada}';";

                // Execute sua consulta aqui, usando o artigo e a série
                var resultado = BSO.Consulta(query);

                if (resultado != null && resultado.DaValor<string>("PrecoLiquido") != null)
                {
                    // Exibe o preço líquido retornado da consulta
                    MessageBox.Show($"Preço Líquido para o artigo {artigo}: {resultado.DaValor<string>("PrecoLiquido")} : {resultado.DaValor<string>("PrecUnit")} : {resultado.DaValor<string>("NumDoc")}");
                }
                else
                {
                    MessageBox.Show($"Não foi possível encontrar o preço líquido para o artigo {artigo}.");
                }
            }
        }


    }
}
