namespace CK_CTDL
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnCheck = new Button();
            btnClear = new Button();
            btnOpen = new Button();
            lvHistory = new ListView();
            txtHtml = new TextBox();
            lblGuide = new Label();
            lblOutput = new Label();
            SuspendLayout();
            // 
            // btnCheck
            // 
            btnCheck.Location = new Point(847, 36);
            btnCheck.Name = "btnCheck";
            btnCheck.Size = new Size(332, 52);
            btnCheck.TabIndex = 0;
            btnCheck.Text = "Kiểm tra";
            btnCheck.UseVisualStyleBackColor = true;
            btnCheck.Click += btnCheck_Click;
            btnCheck.MouseEnter += btnCheck_MouseEnter;
            btnCheck.MouseLeave += MouseLeave;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(847, 110);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(148, 52);
            btnClear.TabIndex = 4;
            btnClear.Text = "Xóa";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            btnClear.MouseEnter += btnClear_MouseEnter;
            btnClear.MouseLeave += MouseLeave;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(1031, 110);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(148, 52);
            btnOpen.TabIndex = 5;
            btnOpen.Text = "Mở File";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            btnOpen.MouseEnter += btnOpen_MouseEnter;
            btnOpen.MouseLeave += MouseLeave;
            // 
            // lvHistory
            // 
            lvHistory.BackColor = SystemColors.HighlightText;
            lvHistory.FullRowSelect = true;
            lvHistory.GridLines = true;
            lvHistory.Location = new Point(847, 186);
            lvHistory.Name = "lvHistory";
            lvHistory.Size = new Size(332, 374);
            lvHistory.TabIndex = 8;
            lvHistory.UseCompatibleStateImageBehavior = false;
            lvHistory.View = View.Details;
            lvHistory.MouseClick += lvHistory_MouseClick;
            lvHistory.MouseEnter += lvHistory_MouseEnter;
            lvHistory.MouseLeave += MouseLeave;
            // 
            // txtHtml
            // 
            txtHtml.AllowDrop = true;
            txtHtml.Location = new Point(27, 23);
            txtHtml.Multiline = true;
            txtHtml.Name = "txtHtml";
            txtHtml.PlaceholderText = "Nhập/Kéo thả file HTML tại đây";
            txtHtml.ScrollBars = ScrollBars.Both;
            txtHtml.Size = new Size(787, 306);
            txtHtml.TabIndex = 9;
            txtHtml.TextAlign = HorizontalAlignment.Center;
            txtHtml.Click += txtHtml_Click;
            txtHtml.DragDrop += txtHtml_DragDrop;
            txtHtml.DragEnter += txtHtml_DragEnter;
            // 
            // lblGuide
            // 
            lblGuide.BackColor = SystemColors.Control;
            lblGuide.ForeColor = SystemColors.AppWorkspace;
            lblGuide.Location = new Point(23, 556);
            lblGuide.Name = "lblGuide";
            lblGuide.Size = new Size(787, 28);
            lblGuide.TabIndex = 10;
            lblGuide.Text = "Instruction here!";
            // 
            // lblOutput
            // 
            lblOutput.BackColor = SystemColors.ButtonHighlight;
            lblOutput.BorderStyle = BorderStyle.Fixed3D;
            lblOutput.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblOutput.ForeColor = SystemColors.InactiveCaption;
            lblOutput.Location = new Point(27, 344);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(787, 206);
            lblOutput.TabIndex = 3;
            lblOutput.Text = "Hiển thị kết quả tại đây";
            lblOutput.TextAlign = ContentAlignment.MiddleCenter;
            lblOutput.UseCompatibleTextRendering = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1208, 587);
            Controls.Add(lblGuide);
            Controls.Add(txtHtml);
            Controls.Add(lvHistory);
            Controls.Add(btnOpen);
            Controls.Add(btnClear);
            Controls.Add(lblOutput);
            Controls.Add(btnCheck);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "HTML Validator";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCheck;
        private Label lblOutput;
        private Button btnClear;
        private Button btnOpen;
        private ListView lvHistory;
        private TextBox txtHtml;
        private Label lblGuide;      
    }
   
}
