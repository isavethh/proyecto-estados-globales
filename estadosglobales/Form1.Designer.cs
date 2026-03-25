namespace estadosglobales
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlSimulation = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.layoutControls = new System.Windows.Forms.TableLayoutPanel();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnP3Send = new System.Windows.Forms.Button();
            this.btnP2Send = new System.Windows.Forms.Button();
            this.btnP1Send = new System.Windows.Forms.Button();
            this.btnSnapshot = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel2MinSize = 360;
            this.splitContainer1.SplitterWidth = 6;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlSimulation);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1400, 800);
            this.splitContainer1.SplitterDistance = 1040;
            this.splitContainer1.TabIndex = 0;
            // 
            // pnlSimulation
            // 
            this.pnlSimulation.BackColor = System.Drawing.Color.White;
            this.pnlSimulation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSimulation.Location = new System.Drawing.Point(0, 0);
            this.pnlSimulation.Name = "pnlSimulation";
            this.pnlSimulation.Size = new System.Drawing.Size(1000, 800);
            this.pnlSimulation.TabIndex = 0;
            this.pnlSimulation.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlSimulation_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.layoutControls);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(16);
            this.groupBox1.Size = new System.Drawing.Size(354, 800);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controles";
            // 
            // layoutControls
            // 
            this.layoutControls.ColumnCount = 1;
            this.layoutControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutControls.Controls.Add(this.btnP1Send, 0, 0);
            this.layoutControls.Controls.Add(this.btnP2Send, 0, 1);
            this.layoutControls.Controls.Add(this.btnP3Send, 0, 2);
            this.layoutControls.Controls.Add(this.btnSnapshot, 0, 3);
            this.layoutControls.Controls.Add(this.btnInfo, 0, 4);
            this.layoutControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControls.Location = new System.Drawing.Point(16, 32);
            this.layoutControls.Name = "layoutControls";
            this.layoutControls.RowCount = 6;
            this.layoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.layoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.layoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutControls.Size = new System.Drawing.Size(322, 752);
            this.layoutControls.TabIndex = 5;
            // 
            // btnInfo
            // 
            this.btnInfo.BackColor = System.Drawing.Color.LightCyan;
            this.btnInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnInfo.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(322, 45);
            this.btnInfo.TabIndex = 4;
            this.btnInfo.Text = "Mostrar notas visuales";
            this.btnInfo.UseVisualStyleBackColor = false;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnP3Send
            // 
            this.btnP3Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnP3Send.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnP3Send.Name = "btnP3Send";
            this.btnP3Send.Size = new System.Drawing.Size(322, 55);
            this.btnP3Send.TabIndex = 3;
            this.btnP3Send.Text = "P3: Enviar Mensaje a P1";
            this.btnP3Send.UseVisualStyleBackColor = true;
            this.btnP3Send.Click += new System.EventHandler(this.btnP3Send_Click);
            // 
            // btnP2Send
            // 
            this.btnP2Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnP2Send.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnP2Send.Name = "btnP2Send";
            this.btnP2Send.Size = new System.Drawing.Size(322, 55);
            this.btnP2Send.TabIndex = 2;
            this.btnP2Send.Text = "P2: Enviar Mensaje a P3";
            this.btnP2Send.UseVisualStyleBackColor = true;
            this.btnP2Send.Click += new System.EventHandler(this.btnP2Send_Click);
            // 
            // btnP1Send
            // 
            this.btnP1Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnP1Send.Name = "btnP1Send";
            this.btnP1Send.Size = new System.Drawing.Size(322, 55);
            this.btnP1Send.TabIndex = 1;
            this.btnP1Send.Text = "P1: Enviar Mensaje a P2";
            this.btnP1Send.UseVisualStyleBackColor = true;
            this.btnP1Send.Click += new System.EventHandler(this.btnP1Send_Click);
            // 
            // btnSnapshot
            // 
            this.btnSnapshot.BackColor = System.Drawing.Color.LightCoral;
            this.btnSnapshot.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSnapshot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSnapshot.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.btnSnapshot.Name = "btnSnapshot";
            this.btnSnapshot.Size = new System.Drawing.Size(322, 70);
            this.btnSnapshot.TabIndex = 0;
            this.btnSnapshot.Text = "Iniciar Snapshot (Chandy-Lamport) desde P1";
            this.btnSnapshot.UseVisualStyleBackColor = false;
            this.btnSnapshot.Click += new System.EventHandler(this.btnSnapshot_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Simulador de Estados Globales y Algoritmo Chandy-Lamport";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlSimulation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel layoutControls;
        private System.Windows.Forms.Button btnSnapshot;
        private System.Windows.Forms.Button btnP3Send;
        private System.Windows.Forms.Button btnP2Send;
        private System.Windows.Forms.Button btnP1Send;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Timer timer1;
    }
}
