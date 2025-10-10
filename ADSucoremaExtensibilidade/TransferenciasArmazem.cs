using InvBE100;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.CustomCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSucoremaExtensibilidade
{
    public class TransferenciasArmazem : CustomCode

    {
        public void GetArtigosFamilia008()
        {
            var queryArtigoLotes = @"
        SELECT 
            A.Artigo,
            INV.Armazem,
            INV.Localizacao,
            INV.EstadoStock,
            INV.Lote,
            INV.StkActual
        FROM Artigo AS A
        INNER JOIN V_INV_ArtigoArmazem AS INV ON A.Artigo = INV.Artigo
        WHERE Familia = '008' AND INV.Armazem = 'A1' AND INV.StkActual > 0";

            var resultArtigoLotes = BSO.Consulta(queryArtigoLotes);
            var numresult = resultArtigoLotes.NumLinhas();

            if (numresult == 0)
            {
                MessageBox.Show("Nenhum artigo encontrado.");
                return;
            }

            resultArtigoLotes.Inicio();

            // 👉 Criar o documento apenas uma vez
            InvBEDocumentoTransf invBEDocumentoTransf = new InvBEDocumentoTransf
            {
                Tipodoc = "TRA"
            };

            // Preencher os dados do cabeçalho
            BSO.Inventario.Transferencias.PreencheDadosRelacionados(invBEDocumentoTransf);

            for (int i = 0; i < numresult; i++)
            {
                try
                {
                    var artigo = resultArtigoLotes.DaValor<string>("Artigo");
                    var armazem = resultArtigoLotes.DaValor<string>("Armazem");
                    var localizacao = resultArtigoLotes.DaValor<string>("Localizacao");
                    var estadoStock = resultArtigoLotes.DaValor<string>("EstadoStock");
                    var lote = resultArtigoLotes.DaValor<string>("Lote");
                    var stkActual = resultArtigoLotes.DaValor<double>("StkActual");

                    // 👉 Adiciona linha ao mesmo documento
                    BSO.Inventario.Transferencias.AdicionaLinhaOrigem(
                        invBEDocumentoTransf,
                        artigo,
                        armazem,
                        localizacao,
                        estadoStock,
                        stkActual,
                        lote
                    );

                    resultArtigoLotes.Seguinte();
                }
                catch (Exception ex)
                {
                    PSO.MensagensDialogos.MostraMensagem(
                        StdPlatBS100.StdBSTipos.TipoMsg.PRI_Detalhe,
                        $"Erro na linha {i + 1}: {ex.Message}"
                    );
                }
            }

            try
            {
                // 👉 Só agora grava o documento completo
                string erros = "";
                BSO.Inventario.Transferencias.Actualiza(invBEDocumentoTransf, ref erros);

                if (!string.IsNullOrEmpty(erros))
                    MessageBox.Show("Erros ao gravar: " + erros);
                else
                    MessageBox.Show("Documento de transferência criado com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar documento: " + ex.Message);
            }
        }

    }
}
