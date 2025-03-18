namespace ADSucoremaExtensibilidade.Sales
{
    partial class FormPreview
    {
        private System.ComponentModel.IContainer components = null;

        // Controlo CrystalReportViewer
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();

            this.SuspendLayout();
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.Location = new System.Drawing.Point(0, 0);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
           // this.crystalReportViewer1.ShowCloseButton = true;
           // this.crystalReportViewer1.ShowCopyButton = true;
            this.crystalReportViewer1.ShowExportButton = true;
            this.crystalReportViewer1.ShowGotoPageButton = true;
            this.crystalReportViewer1.ShowGroupTreeButton = false;
           // this.crystalReportViewer1.ShowParameterPanelButton = true;
            this.crystalReportViewer1.ShowRefreshButton = true;
            this.crystalReportViewer1.ShowTextSearchButton = true;
            this.crystalReportViewer1.Size = new System.Drawing.Size(800, 600);
            this.crystalReportViewer1.TabIndex = 0;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // FormPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.crystalReportViewer1);
            this.Name = "FormPreview";
            this.Text = "Pré-Visualização do Relatório";
            this.ResumeLayout(false);
        }
    }
}