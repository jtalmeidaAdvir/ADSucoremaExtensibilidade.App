namespace ADSucoremaExtensibilidade.Sales
{
    partial class MenuImpressao
    {
        private System.ComponentModel.IContainer components = null;

        // Botões
        private System.Windows.Forms.Button btnGerarPDF;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuImpressao));
            this.btnGerarPDF = new System.Windows.Forms.Button();
            this.cb_Mapa = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numerovias = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numerovias)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGerarPDF
            // 
            this.btnGerarPDF.Location = new System.Drawing.Point(228, 96);
            this.btnGerarPDF.Name = "btnGerarPDF";
            this.btnGerarPDF.Size = new System.Drawing.Size(100, 30);
            this.btnGerarPDF.TabIndex = 0;
            this.btnGerarPDF.Text = "Gerar PDF";
            this.btnGerarPDF.UseVisualStyleBackColor = true;
            this.btnGerarPDF.Click += new System.EventHandler(this.btnGerarPDF_Click);
            // 
            // cb_Mapa
            // 
            this.cb_Mapa.FormattingEnabled = true;
            this.cb_Mapa.Items.AddRange(new object[] {
            "SUCOREMA - Doc. Orçamento (PT)",
            "SUCOREMA - Doc. Orçamento (Inglês)"});
            this.cb_Mapa.Location = new System.Drawing.Point(95, 25);
            this.cb_Mapa.Name = "cb_Mapa";
            this.cb_Mapa.Size = new System.Drawing.Size(233, 22);
            this.cb_Mapa.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "Configuração:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "N.º de vias:";
            // 
            // numerovias
            // 
            this.numerovias.Location = new System.Drawing.Point(95, 52);
            this.numerovias.Name = "numerovias";
            this.numerovias.Size = new System.Drawing.Size(120, 22);
            this.numerovias.TabIndex = 5;
            this.numerovias.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MenuImpressao
            // 
            this.ClientSize = new System.Drawing.Size(340, 138);
            this.Controls.Add(this.numerovias);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_Mapa);
            this.Controls.Add(this.btnGerarPDF);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MenuImpressao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu de Impressão";
            ((System.ComponentModel.ISupportInitialize)(this.numerovias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ComboBox cb_Mapa;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numerovias;
    }
}