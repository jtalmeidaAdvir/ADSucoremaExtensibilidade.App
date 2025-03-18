using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Production.Editors;

namespace ADSucoremaExtensibilidade
{
    public class Editor : EditorStocksProducao
    {
        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            if (KeyCode == 67) // Código ASCII para a tecla 'C'
            {
                EditorOrdemFabricoStocks editor = new EditorOrdemFabricoStocks(BSO, PSO, DocumentoStock);
                editor.Show();
            }
        }
    }
}
