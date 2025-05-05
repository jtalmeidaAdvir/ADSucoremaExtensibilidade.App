
using IntBE100;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Production.Editors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ADSucoremaExtensibilidade
{
    public class ValorizaOfProducao : QuadroControloOrdensFabrico
    {

        public bool valoriza = false;
        public string IdOrdemFabrico = string.Empty;
        public override void AntesDeGravar(DataTable RegistosAlterados, ref bool Cancel, ExtensibilityEventArgs e)
        {



            foreach (DataRow row in RegistosAlterados.Rows)
            {
                //quero ver os registos que tem no RegistosAlterados
                string estado = row["Estado"].ToString();
                // if Estado == 5 
                if (estado == "5")
                {
                    //IDOrdemFabrico
                    IdOrdemFabrico = row["IDOrdemFabrico"].ToString();

                    valoriza = true;
                }
            }


        }
        
        public override void DepoisDeGravar(ExtensibilityEventArgs e)
        {
            if (valoriza) {

                ValorizaEOF(IdOrdemFabrico);
                valoriza = false;
            }
        }

        public void ValorizaEOF(string Of)
        {
            try
            {
                var query = $@"SELECT DISTINCT C.TipoDoc, O.OrdemFabrico, L.Artigo,
                           C.NumDoc, C.Serie, C.IDOperadorGPR, C.Filial, C.IdOrdemFabrico, L.PrecUnit,O.CDU_Valorizado
                    FROM CabecInternos C
                    INNER JOIN GPR_OrdemFabrico O ON C.IdOrdemFabrico = O.IDOrdemFabrico 
                    INNER JOIN LinhasInternos L ON C.Id = L.IdCabecInternos
                    WHERE  C.TipoDoc = 'EOF' AND O.CustoMateriaisReal <> 0 AND O.Estado = 5 AND O.CDU_Valorizado = 0 AND O.IDOrdemFabrico = '{Of}'";

                var lista = BSO.Consulta(query);
                if (lista == null || lista.NumLinhas() == 0)
                {
                    //MessageBox.Show($"Nenhuma informação encontrada para a OF {Of}.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                lista.Inicio();

                var tipoDoc = lista.DaValor<string>("TipoDoc");
                var numDoc = lista.DaValor<int>("NumDoc");
                var serie = lista.DaValor<string>("Serie");
                var filial = lista.DaValor<string>("Filial");
                var artigo = lista.DaValor<string>("Artigo");
                var OrdemFabrico = lista.DaValor<string>("OrdemFabrico");
                var IdOrdemFabrico = lista.DaValor<int>("IdOrdemFabrico");

                var ordemSQL = $@"SELECT ROUND(ISNULL(CustoMateriaisReal, 0) * 0.3, 2) AS Acrescimo30PorCento
                          FROM GPR_OrdemFabrico
                          WHERE IDOrdemFabrico = '{IdOrdemFabrico}' AND ISNULL(CustoMateriaisReal, 0) <> 0;";
                var ordem = BSO.Consulta(ordemSQL);
                if (ordem == null || ordem.NumLinhas() == 0)
                {
                    MessageBox.Show($"Custo não encontrado para a OF {Of}.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var CustoTotalComAcrescimo = ordem.DaValor<double>("Acrescimo30PorCento");
                if (CustoTotalComAcrescimo < 0)
                {
                    MessageBox.Show($"Custo calculado negativo para a Ordem de Fabrico {IdOrdemFabrico}.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                IntBEDocumentoInterno doc;
                try
                {
                    doc = BSO.Internos.Documentos.Edita(tipoDoc, numDoc, serie, filial);
                }
                catch (Exception docEx)
                {
                    MessageBox.Show($"Erro ao editar documento {tipoDoc} {numDoc}/{serie} - {docEx.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                for (int j = 1; j <= doc.Linhas.NumItens; j++)
                {
                    try
                    {
                        var linha = doc.Linhas.GetEdita(j);
                        if (linha.Artigo == artigo)
                        {
                            BSO.Producao.OrdensFabrico.ReabreOrdemFabrico(OrdemFabrico);

                            var querySelect = $@"SELECT Descricao FROM GPR_OrdemFabricoOutrosCustos 
                                         WHERE IDOrdemFabrico = {IdOrdemFabrico} 
                                         AND Descricao = 'Encargo de 30% sobre materiais'";
                            var resultadoSelect = BSO.Consulta(querySelect);
                            if (resultadoSelect == null || resultadoSelect.NumLinhas() == 0)
                            {
                                string custoFormatado = CustoTotalComAcrescimo.ToString().Replace(",", ".");

                                string dataFormatada = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                                var queryUpdate = $@"UPDATE GPR_OrdemFabrico
                                             SET CDU_Valorizado = 1
                                             WHERE IDOrdemFabrico = {IdOrdemFabrico}";
                                var queryInsert = $@"INSERT INTO GPR_OrdemFabricoOutrosCustos 
                                             (Custo, Data, Descricao, IDOrdemFabrico)
                                             VALUES 
                                             ('{custoFormatado}', '{dataFormatada}', 'Encargo de 30% sobre materiais', {IdOrdemFabrico});";

                                BSO.DSO.ExecuteSQL(queryInsert);
                                BSO.DSO.ExecuteSQL(queryUpdate);

                                var aviso = string.Empty;
                                var ordensFabrico = new OrderedDictionary
                        {
                            { Guid.NewGuid().ToString(), IdOrdemFabrico }
                        };
                                BSO.Producao.OrdensFabrico.ProcessaValorizacao(ordensFabrico, true, ref aviso);
                            
                                if (string.Equals(aviso?.Trim(), OrdemFabrico?.Trim(), StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!string.IsNullOrWhiteSpace(aviso))
                                    {
                                        //MessageBox.Show("A Ordem de Fabrico não foi valorizada por estar bloqueada para edição ou não estar no estado correto:\r\n" + aviso, "Erro na Valorização", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        DesvalorizaEOF(IdOrdemFabrico);
                                        //return;
                                    }
                                }
                                else
                                {
                                   
                                }

                                
                            }
                        }
                    }
                    catch (Exception linhaEx)
                    {
                        MessageBox.Show($"Erro ao editar linha {j} do documento {tipoDoc} {numDoc}/{serie}: {linhaEx.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro geral na valorização da Ordem de Fabrico:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void DesvalorizaEOF(int iDOrdemFabrico)
        {
            try
            {
                // Obter as Ordens de Fabrico que estão marcadas como valorizadas (CDU_Valorizado = 1)
                // e que tenham documentos do tipo EOF.
                // Ajuste a query de acordo com as colunas/tabelas que sejam necessárias no seu cenário.
                var query = $@"
                SELECT DISTINCT 
                C.TipoDoc,
                C.NumDoc,
                C.Serie,
                C.IDOperadorGPR,
                C.Filial,
                C.IdOrdemFabrico,
                O.OrdemFabrico
            FROM CabecInternos C
            INNER JOIN GPR_OrdemFabrico O ON C.IdOrdemFabrico = O.IDOrdemFabrico 
            WHERE C.TipoDoc = 'EOF'
              AND O.CDU_Valorizado = 1
              AND O.Estado != 5
              AND O.IDOrdemFabrico = {iDOrdemFabrico}
        ";

                var lista = BSO.Consulta(query);
                var numLinhas = lista.NumLinhas();
                if (numLinhas == 0)
                {

                    return;
                }

                lista.Inicio();
                for (int i = 0; i < numLinhas; i++)
                {
                    try
                    {
                        var tipoDoc = lista.DaValor<string>("TipoDoc");
                        var numDoc = lista.DaValor<int>("NumDoc");
                        var serie = lista.DaValor<string>("Serie");
                        var filial = lista.DaValor<string>("Filial");
                        var idOrdemFabrico = lista.DaValor<int>("IdOrdemFabrico");

                        // 1) Eliminar o registo na GPR_OrdemFabricoOutrosCustos
                        var deleteQuery = $@"
                    DELETE FROM GPR_OrdemFabricoOutrosCustos
                     WHERE IDOrdemFabrico = {idOrdemFabrico}
                       AND Descricao = 'Encargo de 30% sobre materiais'
                ";
                        BSO.DSO.ExecuteSQL(deleteQuery);

                        // 2) Voltar a marcar a Ordem de Fabrico como não valorizada
                        var updateQuery = $@"
                    UPDATE GPR_OrdemFabrico
                       SET CDU_Valorizado = 0
                     WHERE IDOrdemFabrico = {idOrdemFabrico}
                ";
                        BSO.DSO.ExecuteSQL(updateQuery);

                    }
                    catch (Exception innerEx)
                    {
                        MessageBox.Show($"Ocorreu um erro ao tentar desvalorizar a Ordem de Fabrico: {innerEx.Message}",
                                        "Erro",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }

                    lista.Seguinte();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao tentar desvalorizar a Ordem de Fabrico: {ex.Message}",
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }



        }
    }
}
