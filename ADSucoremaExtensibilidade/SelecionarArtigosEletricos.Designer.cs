namespace ADSucoremaExtensibilidade
{
    partial class SelecionarArtigosEletricos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelecionarArtigosEletricos));
            this.dgvArtigosEletricos = new System.Windows.Forms.DataGridView();
            this.btnSelecionarTodos = new System.Windows.Forms.Button();
            this.btnDeselecionarTodos = new System.Windows.Forms.Button();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArtigosEletricos)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvArtigosEletricos
            // 
            this.dgvArtigosEletricos.AllowUserToAddRows = false;
            this.dgvArtigosEletricos.AllowUserToDeleteRows = false;
            this.dgvArtigosEletricos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvArtigosEletricos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvArtigosEletricos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvArtigosEletricos.Location = new System.Drawing.Point(12, 60);
            this.dgvArtigosEletricos.Name = "dgvArtigosEletricos";
            this.dgvArtigosEletricos.Size = new System.Drawing.Size(876, 350);
            this.dgvArtigosEletricos.TabIndex = 0;
            // 
            // btnSelecionarTodos
            // 
            this.btnSelecionarTodos.Location = new System.Drawing.Point(12, 30);
            this.btnSelecionarTodos.Name = "btnSelecionarTodos";
            this.btnSelecionarTodos.Size = new System.Drawing.Size(120, 25);
            this.btnSelecionarTodos.TabIndex = 1;
            this.btnSelecionarTodos.Text = "Selecionar Todos";
            this.btnSelecionarTodos.UseVisualStyleBackColor = true;
            this.btnSelecionarTodos.Click += new System.EventHandler(this.btnSelecionarTodos_Click);
            // 
            // btnDeselecionarTodos
            // 
            this.btnDeselecionarTodos.Location = new System.Drawing.Point(138, 30);
            this.btnDeselecionarTodos.Name = "btnDeselecionarTodos";
            this.btnDeselecionarTodos.Size = new System.Drawing.Size(120, 25);
            this.btnDeselecionarTodos.TabIndex = 2;
            this.btnDeselecionarTodos.Text = "Deselecionar Todos";
            this.btnDeselecionarTodos.UseVisualStyleBackColor = true;
            this.btnDeselecionarTodos.Click += new System.EventHandler(this.btnDeselecionarTodos_Click);
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmar.BackColor = System.Drawing.Color.LightGreen;
            this.btnConfirmar.Location = new System.Drawing.Point(732, 420);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(75, 30);
            this.btnConfirmar.TabIndex = 3;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.UseVisualStyleBackColor = false;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.LightCoral;
            this.btnCancelar.Location = new System.Drawing.Point(813, 420);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 30);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(295, 20);
            this.lblTitulo.TabIndex = 5;
            this.lblTitulo.Text = "Selecionar Artigos da Parte Elétrica";
            // 
            // SelecionarArtigosEletricos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 462);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.btnDeselecionarTodos);
            this.Controls.Add(this.btnSelecionarTodos);
            this.Controls.Add(this.dgvArtigosEletricos);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelecionarArtigosEletricos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selecionar Artigos Elétricos";
            ((System.ComponentModel.ISupportInitialize)(this.dgvArtigosEletricos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvArtigosEletricos;
        private System.Windows.Forms.Button btnSelecionarTodos;
        private System.Windows.Forms.Button btnDeselecionarTodos;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblTitulo;
    }
}