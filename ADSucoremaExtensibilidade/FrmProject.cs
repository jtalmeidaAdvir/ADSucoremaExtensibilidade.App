using ErpBS100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ExtensibilityPrimaveraJLA
{
    public partial class FrmProject : Form
    {
        ErpBS BSO;
        StdBSInterfPub PSO;
        public string project;
        public string description;
        public DateTime startDate;
        public DateTime endDate;

        public FrmProject(ErpBS mBSO, StdBSInterfPub mPSO)
        {
            InitializeComponent();
            BSO = mBSO;
            PSO = mPSO;
        }

        private void btnF4_Click(object sender, EventArgs e)
        {
            try
            {
                string code = string.Empty;
                string desc = string.Empty;

                GetProjects(ref code, ref desc);

                txtProject.Text = code;
                txtDescription.Text = desc;
            }
            catch (Exception ex)
            {
                PSO.MensagensDialogos.MostraAviso(ex.Message, StdBSTipos.IconId.PRI_Critico, ex.ToString());
            }
        }

        private void txtProject_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F4)
                {
                    string code = string.Empty;
                    string desc = string.Empty;

                    GetProjects(ref code, ref desc);

                    txtProject.Text = code;
                    txtDescription.Text = desc;
                }
            }
            catch (Exception ex)
            {
                PSO.MensagensDialogos.MostraAviso(ex.Message, StdBSTipos.IconId.PRI_Critico, ex.ToString());
            }
        }

        private void GetProjects(ref string code, ref string desc)
        {
            string NomeLista = "Projetos";
            string Campos = "Codigo, Descricao";
            string Tabela = " COP_OBRAS (NOLOCK)";
            string Where = "";
            string CamposF4 = "Codigo, Descricao";
            string orderby = "Codigo, Descricao";

            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, this, ref ResQuery);

            if (ResQuery.Count > 0)
            {
                code = ResQuery.ElementAt(0);
                desc = ResQuery.ElementAt(1);
            }
        }

        public void OpenF4List(string Campos, string Tabela, string Where, string CamposF4, string orderby, string NomeLista, Form frm, ref List<string> ResQuery)
        {
            string strSQL = "select distinct " + Campos + " FROM " + Tabela;

            if (Where.Length > 0)
            {
                strSQL = strSQL + " WHERE " + Where;
            }

            strSQL = strSQL + " Order by " + orderby;


            string result = Convert.ToString(PSO.Listas.GetF4SQL(NomeLista, strSQL, CamposF4, frm));

            if (!string.IsNullOrEmpty(result))
            {
                string[] itemQuery = result.Split('\t');

                foreach (string item in itemQuery)
                {
                    ResQuery.Add(item);
                }
            }


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            project = txtProject.Text;
            description = txtDescription.Text;
            startDate = dtpStartDate.Value; 
            endDate = dtpEndDate.Value;

            this.Close();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            project = txtProject.Text;
            description = txtDescription.Text;
            startDate = dtpStartDate.Value;
            endDate = dtpEndDate.Value;

            this.Close();
        }
    }
}
