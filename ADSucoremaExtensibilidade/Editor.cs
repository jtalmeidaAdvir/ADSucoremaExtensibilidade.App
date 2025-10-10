using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Production.Editors;

namespace ADSucoremaExtensibilidade
{
    public class Editor : EditorStocksProducao
    {
        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            var verificaDocumentoExiste = $@"SELECT * FROM CabecInternos WHERE TipoDoc = 'SOF' AND NumDoc = '{this.DocumentoStock.NumDoc}'";
            var rs = BSO.Consulta(verificaDocumentoExiste);
            var numlinhas = rs.NumLinhas();
            if (numlinhas > 0)
            {
                if (KeyCode == 67) // Código ASCII para a tecla 'C'
                {
                    EditorOrdemFabricoStocks editor = new EditorOrdemFabricoStocks(BSO, PSO, DocumentoStock);
                    editor.Show();
                }
            }
            else{
                PSO.MensagensDialogos.MostraMensagem(StdPlatBS100.StdBSTipos.TipoMsg.PRI_Detalhe, "Tem de gravar o documento antes de continuar!");
            }

        }
    }
}
