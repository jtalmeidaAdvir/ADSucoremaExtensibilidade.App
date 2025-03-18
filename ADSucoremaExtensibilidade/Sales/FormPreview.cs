using System;
using System.Windows.Forms;

// Referências do Crystal Reports
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ErpBS100;

namespace ADSucoremaExtensibilidade.Sales
{
    public partial class FormPreview : Form
    {
        public FormPreview()
        {
            InitializeComponent();
        }

        // Método para carregar o ficheiro .rpt e exibi-lo no CrystalReportViewer
        public void CarregarRelatorio(string caminhoRelatorio, ErpBS bSO, StdPlatBS100.StdBSInterfPub pSO)

        {

            try

            {

                ReportDocument relatorio = new ReportDocument();

                relatorio.Load(caminhoRelatorio);
                System.Data.Common.DbConnection conn = bSO.DSO.BDAPL;
                string connectionString = conn.ConnectionString;
                MessageBox.Show(connectionString);

                var sqlBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

                // --- Definir ligação à BD (exemplo para SQL) ---
                ConnectionInfo connectionInfo = new ConnectionInfo();
                connectionInfo.ServerName = sqlBuilder.DataSource;
                connectionInfo.DatabaseName = sqlBuilder.InitialCatalog;
                connectionInfo.UserID = sqlBuilder.UserID;
                connectionInfo.Password = sqlBuilder.Password;
                connectionInfo.IntegratedSecurity = true;

                // Tabelas do relatório principal

                Tables tables = relatorio.Database.Tables;

                foreach (Table table in tables)

                {

                    TableLogOnInfo tableLogOnInfo = table.LogOnInfo;

                    tableLogOnInfo.ConnectionInfo = connectionInfo;

                    table.ApplyLogOnInfo(tableLogOnInfo);

                }

                // Sub-relatórios, se existirem

                foreach (Section section in relatorio.ReportDefinition.Sections)

                {

                    foreach (ReportObject reportObject in section.ReportObjects)

                    {

                        if (reportObject.Kind == ReportObjectKind.SubreportObject)

                        {

                            SubreportObject subreportObject = (SubreportObject)reportObject;

                            ReportDocument subReportDoc = relatorio.OpenSubreport(subreportObject.SubreportName);

                            foreach (Table subTable in subReportDoc.Database.Tables)

                            {

                                TableLogOnInfo subTableLogOnInfo = subTable.LogOnInfo;

                                subTableLogOnInfo.ConnectionInfo = connectionInfo;

                                subTable.ApplyLogOnInfo(subTableLogOnInfo);

                            }

                        }

                    }

                }

                // --- Configurar o formato A4 ---
                relatorio.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                // --- Fim da configuração da BD ---

                // Associar ao CrystalReportViewer

                crystalReportViewer1.ReportSource = relatorio;

                crystalReportViewer1.Refresh();

            }

            catch (Exception ex)

            {

                MessageBox.Show("Erro ao carregar o relatório: " + ex.Message);

            }

        }


    }
}