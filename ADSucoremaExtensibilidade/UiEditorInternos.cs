using Primavera.Extensibility.Internal.Editors;
using System;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using StdBE100;
using System.Windows.Forms;

namespace ExtensibilityPrimaveraJLA
{
    public class UiEditorInternos : EditorInternos
    {


        #region Primavera

        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {
            if (this.DocumentoInterno.Tipodoc == "RC")
            {
                if (KeyCode == Convert.ToInt32(Keys.K))
                {
                    OpenProjectForm();
                }
            }
        }

        #endregion

        #region HelpFuncions

        private void OpenProjectForm()
        {
            FrmProject frm = new FrmProject(BSO, PSO);
            frm.ShowDialog();
            string project = frm.project;
            string projectDescription = frm.project;
            DateTime startDate = frm.startDate;
            DateTime endDate = frm.endDate;


            string strSQL = $@"select l.Artigo,l.Descricao, l.Quantidade,l.Desconto1,l.PrecUnit, o.ID as ObraID from LinhasCompras l
                                inner join CabecCompras c on l.IdCabecCompras=c.Id
                                inner join COP_Obras O ON l.ObraID=O.ID
                                where o.Codigo='{project}' and c.TipoDoc='VFA'
                                AND c.DataDoc between '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'" ;

            StdBELista lst = BSO.Consulta(strSQL);
            if (lst.NumLinhas() > 0)
            {
                while (!lst.NoFim())
                {
                    string artigo = Convert.ToString(lst.Valor("Artigo"));
                    string obraID = Convert.ToString(lst.Valor("ObraID"));
                    double qtd = Math.Abs(Convert.ToDouble(lst.Valor("Quantidade")));
                    double desc = Math.Abs(Convert.ToDouble(lst.Valor("Desconto1")));
                    double precunit = Math.Abs(Convert.ToDouble(lst.Valor("PrecUnit")));


                    BSO.Internos.Documentos.AdicionaLinha(this.DocumentoInterno,artigo,"","","",precunit,desc,qtd);
                    int numItems = this.DocumentoInterno.Linhas.NumItens;
                    this.DocumentoInterno.Linhas.GetEdita(numItems).ObraID = obraID;
                    
                    

                    lst.Seguinte();
                }
            }

            this.DocumentoInterno.Observacoes = $"Consumos de Projeto desde Data inicial:{startDate.ToString("yyyy-MM-dd")} até Datafinal:{endDate.ToString("yyyy-MM-dd")}";

        }


        #endregion

    }
}
