
using IntBE100;
using InvBE100;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Production.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;


namespace ADSucoremaExtensibilidade
{
    public class Rececao : EditorSubcontratacaoRececao
    {
        public override void DepoisDeGravarDocumentoStock(IntBEDocumentoInterno DocumentoStock, ExtensibilityEventArgs e)
        {


            var idordemfabrico = DocumentoStock.IdOrdemFabrico.ToString();

            var query = $"SELECT IDOrdemFabrico, SubContratacao, Descricao FROM GPR_OrdemFabricoOperacoes WHERE IDOrdemFabrico = '{idordemfabrico}' AND SubContratacao = 1";
            var sqlbso = BSO.Consulta(query);

            var exite = sqlbso.DaValor<bool>("SubContratacao");
        
            if (exite) 
            {
                if (DocumentoStock.Tipodoc.ToString() == "SOF")
                {
                    var update = $@"UPDATE LinhasInternos
                                SET PrecUnit = 0
                                WHERE idcabecinternos = '{DocumentoStock.ID.ToString()}' ";

                    BSO.DSO.ExecuteSQL(update);
                }

             


            }




        }


   
    }
}
