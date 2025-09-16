namespace ADSucoremaExtensibilidade
{
    partial class EditorOrdemFabricoStocks
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorOrdemFabricoStocks));
            this.dgvOrdensFabrico = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cbTipoLista = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExportarTemplateExcel = new System.Windows.Forms.Button();
            this.btnImportarExcel = new System.Windows.Forms.Button();
            this.btnPrimeira = new System.Windows.Forms.Button();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.btnProxima = new System.Windows.Forms.Button();
            this.btnUltima = new System.Windows.Forms.Button();
            this.lblPagina = new System.Windows.Forms.Label();
            this.cbItensPorPagina = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdensFabrico)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvOrdensFabrico
            // 
            this.dgvOrdensFabrico.AllowUserToAddRows = false;
            this.dgvOrdensFabrico.AllowUserToDeleteRows = false;
            this.dgvOrdensFabrico.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOrdensFabrico.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrdensFabrico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrdensFabrico.Location = new System.Drawing.Point(12, 48);
            this.dgvOrdensFabrico.Name = "dgvOrdensFabrico";
            this.dgvOrdensFabrico.Size = new System.Drawing.Size(806, 354);
            this.dgvOrdensFabrico.TabIndex = 0;
            this.dgvOrdensFabrico.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(142, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "Adcionar No SOF";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 11);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(124, 30);
            this.button2.TabIndex = 2;
            this.button2.Text = "Selecionar Todos";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbTipoLista
            // 
            this.cbTipoLista.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTipoLista.FormattingEnabled = true;
            this.cbTipoLista.Location = new System.Drawing.Point(350, 16);
            this.cbTipoLista.Name = "cbTipoLista";
            this.cbTipoLista.Size = new System.Drawing.Size(200, 21);
            this.cbTipoLista.TabIndex = 3;
            this.cbTipoLista.SelectedIndexChanged += new System.EventHandler(this.cbTipoLista_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tipo de Lista:";
            // 
            // btnExportarTemplateExcel
            // 
            this.btnExportarTemplateExcel.Location = new System.Drawing.Point(570, 11);
            this.btnExportarTemplateExcel.Name = "btnExportarTemplateExcel";
            this.btnExportarTemplateExcel.Size = new System.Drawing.Size(140, 30);
            this.btnExportarTemplateExcel.TabIndex = 5;
            this.btnExportarTemplateExcel.Text = "Exportar Template Excel";
            this.btnExportarTemplateExcel.UseVisualStyleBackColor = true;
            this.btnExportarTemplateExcel.Click += new System.EventHandler(this.btnExportarTemplateExcel_Click);
            // 
            // btnImportarExcel
            // 
            this.btnImportarExcel.Location = new System.Drawing.Point(716, 11);
            this.btnImportarExcel.Name = "btnImportarExcel";
            this.btnImportarExcel.Size = new System.Drawing.Size(104, 30);
            this.btnImportarExcel.TabIndex = 6;
            this.btnImportarExcel.Text = "Importar Excel";
            this.btnImportarExcel.UseVisualStyleBackColor = true;
            this.btnImportarExcel.Click += new System.EventHandler(this.btnImportarExcel_Click);
            // 
            // btnPrimeira
            // 
            this.btnPrimeira.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrimeira.Location = new System.Drawing.Point(12, 408);
            this.btnPrimeira.Name = "btnPrimeira";
            this.btnPrimeira.Size = new System.Drawing.Size(60, 25);
            this.btnPrimeira.TabIndex = 7;
            this.btnPrimeira.Text = "<<";
            this.btnPrimeira.UseVisualStyleBackColor = true;
            this.btnPrimeira.Click += new System.EventHandler(this.btnPrimeira_Click);
            // 
            // btnAnterior
            // 
            this.btnAnterior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAnterior.Location = new System.Drawing.Point(78, 408);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(60, 25);
            this.btnAnterior.TabIndex = 8;
            this.btnAnterior.Text = "<";
            this.btnAnterior.UseVisualStyleBackColor = true;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            // 
            // btnProxima
            // 
            this.btnProxima.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnProxima.Location = new System.Drawing.Point(240, 408);
            this.btnProxima.Name = "btnProxima";
            this.btnProxima.Size = new System.Drawing.Size(60, 25);
            this.btnProxima.TabIndex = 10;
            this.btnProxima.Text = ">";
            this.btnProxima.UseVisualStyleBackColor = true;
            this.btnProxima.Click += new System.EventHandler(this.btnProxima_Click);
            // 
            // btnUltima
            // 
            this.btnUltima.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUltima.Location = new System.Drawing.Point(306, 408);
            this.btnUltima.Name = "btnUltima";
            this.btnUltima.Size = new System.Drawing.Size(60, 25);
            this.btnUltima.TabIndex = 11;
            this.btnUltima.Text = ">>";
            this.btnUltima.UseVisualStyleBackColor = true;
            this.btnUltima.Click += new System.EventHandler(this.btnUltima_Click);
            // 
            // lblPagina
            // 
            this.lblPagina.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPagina.AutoSize = true;
            this.lblPagina.Location = new System.Drawing.Point(144, 413);
            this.lblPagina.Name = "lblPagina";
            this.lblPagina.Size = new System.Drawing.Size(73, 13);
            this.lblPagina.TabIndex = 9;
            this.lblPagina.Text = "Página 1 de 1";
            // 
            // cbItensPorPagina
            // 
            this.cbItensPorPagina.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbItensPorPagina.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbItensPorPagina.FormattingEnabled = true;
            this.cbItensPorPagina.Items.AddRange(new object[] {
            "Todos",
            "25",
            "50",
            "100"});
            this.cbItensPorPagina.Location = new System.Drawing.Point(710, 410);
            this.cbItensPorPagina.Name = "cbItensPorPagina";
            this.cbItensPorPagina.Size = new System.Drawing.Size(108, 21);
            this.cbItensPorPagina.TabIndex = 12;
            this.cbItensPorPagina.SelectedIndexChanged += new System.EventHandler(this.cbItensPorPagina_SelectedIndexChanged);
            // 
            // EditorOrdemFabricoStocks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 444);
            this.Controls.Add(this.cbItensPorPagina);
            this.Controls.Add(this.btnUltima);
            this.Controls.Add(this.btnProxima);
            this.Controls.Add(this.lblPagina);
            this.Controls.Add(this.btnAnterior);
            this.Controls.Add(this.btnPrimeira);
            this.Controls.Add(this.btnImportarExcel);
            this.Controls.Add(this.btnExportarTemplateExcel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbTipoLista);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgvOrdensFabrico);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditorOrdemFabricoStocks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EditorOrdemFabricoStocks";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdensFabrico)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOrdensFabrico;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cbTipoLista;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExportarTemplateExcel;
        private System.Windows.Forms.Button btnImportarExcel;
        private System.Windows.Forms.Button btnPrimeira;
        private System.Windows.Forms.Button btnAnterior;
        private System.Windows.Forms.Button btnProxima;
        private System.Windows.Forms.Button btnUltima;
        private System.Windows.Forms.Label lblPagina;
        private System.Windows.Forms.ComboBox cbItensPorPagina;
    }
}