namespace ADSucoremaExtensibilidade
{
    partial class EntradaOFValorizadas
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.OrdemFabrico = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Artigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Valor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Valor30 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Selecionar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bt_atualizar = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cb_serie = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OrdemFabrico,
            this.Artigo,
            this.Valor,
            this.Valor30,
            this.Selecionar});
            this.dataGridView1.Location = new System.Drawing.Point(3, 42);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(794, 405);
            this.dataGridView1.TabIndex = 0;
            // 
            // OrdemFabrico
            // 
            this.OrdemFabrico.HeaderText = "OrdemFabrico";
            this.OrdemFabrico.Name = "OrdemFabrico";
            this.OrdemFabrico.ReadOnly = true;
            // 
            // Artigo
            // 
            this.Artigo.HeaderText = "Artigo";
            this.Artigo.Name = "Artigo";
            this.Artigo.ReadOnly = true;
            // 
            // Valor
            // 
            this.Valor.HeaderText = "Valor";
            this.Valor.Name = "Valor";
            this.Valor.ReadOnly = true;
            // 
            // Valor30
            // 
            this.Valor30.HeaderText = "Valor (30%)";
            this.Valor30.Name = "Valor30";
            this.Valor30.ReadOnly = true;
            // 
            // Selecionar
            // 
            this.Selecionar.HeaderText = "Selecionar";
            this.Selecionar.Name = "Selecionar";
            // 
            // bt_atualizar
            // 
            this.bt_atualizar.Location = new System.Drawing.Point(3, 8);
            this.bt_atualizar.Name = "bt_atualizar";
            this.bt_atualizar.Size = new System.Drawing.Size(183, 31);
            this.bt_atualizar.TabIndex = 2;
            this.bt_atualizar.Text = "Atualizar Valores";
            this.bt_atualizar.UseVisualStyleBackColor = true;
            this.bt_atualizar.Click += new System.EventHandler(this.bt_atualizar_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(192, 15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(119, 18);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Selecionar Todos";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // cb_serie
            // 
            this.cb_serie.FormattingEnabled = true;
            this.cb_serie.Items.AddRange(new object[] {
            "2025",
            "2024"});
            this.cb_serie.Location = new System.Drawing.Point(676, 11);
            this.cb_serie.Name = "cb_serie";
            this.cb_serie.Size = new System.Drawing.Size(121, 22);
            this.cb_serie.TabIndex = 5;
            // 
            // EntradaOFValorizadas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cb_serie);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.bt_atualizar);
            this.Controls.Add(this.dataGridView1);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "EntradaOFValorizadas";
            this.Size = new System.Drawing.Size(800, 450);
            this.Text = "EntradaOFValorizadas";
            this.Load += new System.EventHandler(this.EntradaOFValorizadas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrdemFabrico;
        private System.Windows.Forms.DataGridViewTextBoxColumn Artigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Valor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Valor30;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Selecionar;
        private System.Windows.Forms.Button bt_atualizar;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox cb_serie;
    }
}