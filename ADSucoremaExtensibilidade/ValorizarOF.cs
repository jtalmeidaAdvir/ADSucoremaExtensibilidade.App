using GprBE100;
using IntBE100;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.CustomCode;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public class ValorizarOF : CustomCode
    {
        public string OrdemFabrico { get; private set; }

        public void ValorizaEOF()
        {
            try
            {
                var query = $@"SELECT DISTINCT C.TipoDoc, O.OrdemFabrico, L.Artigo,
                              C.NumDoc, C.Serie, C.IDOperadorGPR, C.Filial, C.IdOrdemFabrico, L.PrecUnit,O.CDU_Valorizado
                       FROM CabecInternos C
                       INNER JOIN GPR_OrdemFabrico O ON C.IdOrdemFabrico = O.IDOrdemFabrico 
                       INNER JOIN LinhasInternos L ON C.Id = L.IdCabecInternos
                       WHERE  C.TipoDoc = 'EOF' AND O.CustoMateriaisReal <> 0 AND O.Estado = 5 AND O.CDU_Valorizado = 0";

                var lista = BSO.Consulta(query);
                var numLinhas = lista.NumLinhas();
                

                lista.Inicio();
                for (int i = 0; i < numLinhas; i++)
                {
                    try
                    {
                        var tipoDoc = lista.DaValor<string>("TipoDoc");
                        var numDoc = lista.DaValor<int>("NumDoc");
                        var serie = lista.DaValor<string>("Serie");
                        var filial = lista.DaValor<string>("Filial");
                        var artigo = lista.DaValor<string>("Artigo");
                        OrdemFabrico = lista.DaValor<string>("OrdemFabrico");
                        var IdOrdemFabrico = lista.DaValor<int>("IdOrdemFabrico");

                        var ordemSQL = $@"SELECT 
                                ROUND(ISNULL(CustoMateriaisReal, 0) * 0.3, 2) AS Acrescimo30PorCento
                            FROM 
                                GPR_OrdemFabrico
                            WHERE 
                                IDOrdemFabrico = '{IdOrdemFabrico}' AND
                                ISNULL(CustoMateriaisReal, 0) <> 0;
                            ";

                        var ordem = BSO.Consulta(ordemSQL);
                        if (ordem == null || ordem.NumLinhas() == 0)
                        {
                            lista.Seguinte();
                            continue;
                        }

                        var CustoTotalComAcrescimo = ordem.DaValor<double>("Acrescimo30PorCento");
                        if (CustoTotalComAcrescimo < 0)
                        {
                            MessageBox.Show($"Custo calculado negativo para a Ordem de Fabrico {IdOrdemFabrico}.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lista.Seguinte();
                            continue;
                        }

                        IntBEDocumentoInterno doc;
                        try
                        {
                            doc = BSO.Internos.Documentos.Edita(tipoDoc, numDoc, serie, filial);
                        }
                        catch (Exception docEx)
                        {
                            MessageBox.Show($"Erro ao editar documento {tipoDoc} {numDoc}/{serie} - {docEx.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lista.Seguinte();
                            continue;
                        }

                        var numLinhasDoc = doc.Linhas.NumItens;

                        for (int j = 1; j <= numLinhasDoc; j++)
                        {
                            try
                            {
                                var linha = doc.Linhas.GetEdita(j);
                                if (linha.Artigo == artigo)
                                {


                                    BSO.Producao.OrdensFabrico.ReabreOrdemFabrico(OrdemFabrico);
                                    var querySelect = $@"SELECT Descricao FROM GPR_OrdemFabricoOutrosCustos WHERE IDOrdemFabrico = {IdOrdemFabrico} AND Descricao = 'Encargo de 30% sobre materiais'";
                                    var resultadoSelect = BSO.Consulta(querySelect);
                                    if (
                                        resultadoSelect == null ||
                                        resultadoSelect.NumLinhas() == 0)

                                    {

                                        string custoFormatado = CustoTotalComAcrescimo.ToString().Replace(",", ".");
                                        var queryUpdate = $@"UPDATE GPR_OrdemFabrico
                                                        SET CDU_Valorizado = 1
                                                        WHERE IDOrdemFabrico = {IdOrdemFabrico}";


                                        var queryinsert = $@"INSERT INTO GPR_OrdemFabricoOutrosCustos 
                                                    (Custo, Data, Descricao, IDOrdemFabrico)
                                                VALUES 
                                                    ('{custoFormatado}', '{DateTime.Now}', 'Encargo de 30% sobre materiais', {IdOrdemFabrico});";

                                        BSO.DSO.ExecuteSQL(queryinsert);
                                        BSO.DSO.ExecuteSQL(queryUpdate);

                                       
                                        // Atualiza o valor na linha do documento interno
                                        var aviso = string.Empty;
                                        var ordensFabrico = new OrderedDictionary
                                        {
                                            { Guid.NewGuid().ToString(), IdOrdemFabrico }
                                        };
                                        BSO.Producao.OrdensFabrico.ProcessaValorizacao(ordensFabrico, true, ref aviso);
                               
                                        // se o aviso for diferente de vazio, significa que houve erro na valorização (a mensagem tem que dizer para fechar o editor da ordem de fabrico 'aviso')
                                        var ordem2 = OrdemFabrico;
                                        string of = aviso;



                                        if (string.Equals(of?.Trim(), OrdemFabrico?.Trim(), StringComparison.OrdinalIgnoreCase))

                                        {
                                            MessageBox.Show("As seguintes Ordens de Fabrico não foram valorizadas por estarem bloqueadas para edição ou não se encontrarem no estado correto:\r\n" + aviso, "Erro na Valorização", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            DesvalorizaEOF(IdOrdemFabrico);
                                            lista.Seguinte();
                                            continue;
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
                    catch (Exception innerEx)
                    {
                        MessageBox.Show($"Erro ao processar uma linha da lista principal: {innerEx.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    lista.Seguinte();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro geral na valorização de EOF:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        public void DesvalorizaEOF()
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

                        // Se fizer sentido editar/actualizar o documento interno (LinhasInternos),
                        // pode repetir a lógica que utilizou no ValorizaEOF, consoante a sua necessidade.
                        // Exemplo (opcional):
                        // try
                        // {
                        //     IntBEDocumentoInterno doc = BSO.Internos.Documentos.Edita(tipoDoc, numDoc, serie, filial);
                        //     // Se precisar de tratar algo nas linhas, editar etc.
                        // }
                        // catch (Exception exDoc)
                        // {
                        //     MessageBox.Show($"Erro ao editar documento {tipoDoc} {numDoc}/{serie}: {exDoc.Message}",
                        //                     "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // }

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
                MessageBox.Show($"Erro geral na desvalorização de EOF:\n{ex.Message}",
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public void ReabreOF()
        {
  
            BSO.Producao.OrdensFabrico.ActualizaPrecoPrevistoOF("2400951.00");
            BSO.Producao.OrdensFabrico.ActualizaPrecoPrevisto();
            MessageBox.Show("Valorizado");
        }

    }
}

