using ADSucoremaExtensibilidade.Sales;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Sales.Editors;
using StdBE100;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BasBE100.BasBETiposGcp;


namespace ExtensibilityPrimaveraJLA.Sales
{
    public class UiEditorVendas : EditorVendas
    {

        #region Primavera

        public override void ArtigoIdentificado(string Artigo, int NumLinha, ref bool Cancel, ExtensibilityEventArgs e)
        {

            if (this.DocumentoVenda.Tipodoc == "ECL" || this.DocumentoVenda.Tipodoc == "ORC")
            {
                string idioma = string.Empty;
                string caracteristicas = string.Empty;

                //Get entity Language
                string strSQLEntity = $"Select idioma from Clientes where cliente = '{this.DocumentoVenda.Entidade}'";

                StdBELista lstEntity = BSO.Consulta(strSQLEntity);

                if (lstEntity.NumLinhas() > 0)
                {
                    idioma = Convert.ToString(lstEntity.Valor("idioma"));
                }

                // get Artigo Info
                string strSQL = $"Select * from ArtigoIdioma where artigo = '{Artigo}' and idioma = '{idioma}'";

                StdBELista lstProduct = BSO.Consulta(strSQL);

                if (lstProduct.NumLinhas() > 0)
                {
                    caracteristicas = Convert.ToString(lstProduct.Valor("Caracteristicas"));
                }

                //Insert Caracteristics in the Document
                if (!string.IsNullOrEmpty(caracteristicas))
                {
                    //List<string> listCharacteristics = new List<string>();
                    string delimiter = "\r";

                    List<string> listCharacteristics = SplitStringByString(caracteristicas, delimiter);


                    foreach (string str in listCharacteristics)
                    {
                        BSO.Vendas.Documentos.AdicionaLinhaEspecial(this.DocumentoVenda, vdTipoLinhaEspecial.vdLinha_Comentario, 0, str);
                    }
                }
            }
            
        }

        #endregion

        #region HelpFunctions
        public static List<string> SplitStringByString(string input, string delimiter)
        {
            List<string> result = new List<string>();
            int startIndex = 0;
            int delimiterIndex;

            while ((delimiterIndex = input.IndexOf(delimiter, startIndex)) != -1)
            {
                result.Add(input.Substring(startIndex, delimiterIndex - startIndex));
                startIndex = delimiterIndex + delimiter.Length;
            }

            // Add the remaining part of the string
            result.Add(input.Substring(startIndex));

            return result;
        }
        #endregion


        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            if (this.DocumentoVenda.Tipodoc == "ORC")
            {
                MenuImpressao menuImpressao = new MenuImpressao(this.DocumentoVenda, BSO, PSO);
                menuImpressao.ShowDialog();


            }
        }
    }
}
