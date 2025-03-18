namespace ADSucoremaExtensibilidade
{
    partial class EditorArtigosUnidade
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
            this.TXT_Artigo = new System.Windows.Forms.TextBox();
            this.F4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Base = new System.Windows.Forms.ComboBox();
            this.Entrada = new System.Windows.Forms.ComboBox();
            this.Saida = new System.Windows.Forms.ComboBox();
            this.Compra = new System.Windows.Forms.ComboBox();
            this.Venda = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Guardar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TXT_Artigo
            // 
            this.TXT_Artigo.Enabled = false;
            this.TXT_Artigo.Location = new System.Drawing.Point(41, 55);
            this.TXT_Artigo.Name = "TXT_Artigo";
            this.TXT_Artigo.Size = new System.Drawing.Size(145, 21);
            this.TXT_Artigo.TabIndex = 0;
            // 
            // F4
            // 
            this.F4.Location = new System.Drawing.Point(192, 53);
            this.F4.Name = "F4";
            this.F4.Size = new System.Drawing.Size(34, 23);
            this.F4.TabIndex = 1;
            this.F4.Text = "F4";
            this.F4.UseVisualStyleBackColor = true;
            this.F4.Click += new System.EventHandler(this.F4_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(238, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Unidade Venda";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Unidade Base";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Unidade Saida";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(230, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Unidade Compra";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Unidade Entrada";
            // 
            // Base
            // 
            this.Base.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Base.FormattingEnabled = true;
            this.Base.Items.AddRange(new object[] {
            "%",
            "H",
            "HR",
            "KG",
            "KM",
            "KW",
            "LT",
            "M2",
            "MIN",
            "MT",
            "UN",
            "VG"});
            this.Base.Location = new System.Drawing.Point(116, 104);
            this.Base.Name = "Base";
            this.Base.Size = new System.Drawing.Size(103, 21);
            this.Base.TabIndex = 12;
            // 
            // Entrada
            // 
            this.Entrada.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Entrada.FormattingEnabled = true;
            this.Entrada.Items.AddRange(new object[] {
            "%",
            "H",
            "HR",
            "KG",
            "KM",
            "KW",
            "LT",
            "M2",
            "MIN",
            "MT",
            "UN",
            "VG"});
            this.Entrada.Location = new System.Drawing.Point(116, 141);
            this.Entrada.Name = "Entrada";
            this.Entrada.Size = new System.Drawing.Size(103, 21);
            this.Entrada.TabIndex = 13;
            // 
            // Saida
            // 
            this.Saida.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Saida.FormattingEnabled = true;
            this.Saida.Items.AddRange(new object[] {
            "%",
            "H",
            "HR",
            "KG",
            "KM",
            "KW",
            "LT",
            "M2",
            "MIN",
            "MT",
            "UN",
            "VG"});
            this.Saida.Location = new System.Drawing.Point(116, 184);
            this.Saida.Name = "Saida";
            this.Saida.Size = new System.Drawing.Size(103, 21);
            this.Saida.TabIndex = 14;
            // 
            // Compra
            // 
            this.Compra.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Compra.FormattingEnabled = true;
            this.Compra.Items.AddRange(new object[] {
            "%",
            "H",
            "HR",
            "KG",
            "KM",
            "KW",
            "LT",
            "M2",
            "MIN",
            "MT",
            "UN",
            "VG"});
            this.Compra.Location = new System.Drawing.Point(322, 141);
            this.Compra.Name = "Compra";
            this.Compra.Size = new System.Drawing.Size(103, 21);
            this.Compra.TabIndex = 15;
            // 
            // Venda
            // 
            this.Venda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Venda.FormattingEnabled = true;
            this.Venda.Items.AddRange(new object[] {
            "%",
            "H",
            "HR",
            "KG",
            "KM",
            "KW",
            "LT",
            "M2",
            "MIN",
            "MT",
            "UN",
            "VG"});
            this.Venda.Location = new System.Drawing.Point(322, 184);
            this.Venda.Name = "Venda";
            this.Venda.Size = new System.Drawing.Size(103, 21);
            this.Venda.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(169, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 19);
            this.label6.TabIndex = 17;
            this.label6.Text = "Editor Unidades";
            // 
            // Guardar
            // 
            this.Guardar.Location = new System.Drawing.Point(359, 55);
            this.Guardar.Name = "Guardar";
            this.Guardar.Size = new System.Drawing.Size(66, 46);
            this.Guardar.TabIndex = 18;
            this.Guardar.Text = "Guardar";
            this.Guardar.UseVisualStyleBackColor = true;
            this.Guardar.Click += new System.EventHandler(this.Guardar_Click);
            // 
            // EditorArtigosUnidade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Guardar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Venda);
            this.Controls.Add(this.Compra);
            this.Controls.Add(this.Saida);
            this.Controls.Add(this.Entrada);
            this.Controls.Add(this.Base);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.F4);
            this.Controls.Add(this.TXT_Artigo);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "EditorArtigosUnidade";
            this.Size = new System.Drawing.Size(455, 246);
            this.Text = "EditorArtigosUnidade";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TXT_Artigo;
        private System.Windows.Forms.Button F4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox Base;
        private System.Windows.Forms.ComboBox Entrada;
        private System.Windows.Forms.ComboBox Saida;
        private System.Windows.Forms.ComboBox Compra;
        private System.Windows.Forms.ComboBox Venda;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button Guardar;
    }
}