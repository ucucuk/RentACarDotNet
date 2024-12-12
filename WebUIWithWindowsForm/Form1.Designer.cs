namespace WebUIWithWindowsForm
{
    partial class Form1
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
            txtusername = new TextBox();
            label1 = new Label();
            dataGridView1 = new DataGridView();
            button1 = new Button();
            dateofbirthyear = new DateTimePicker();
            label2 = new Label();
            txtpassword = new TextBox();
            label3 = new Label();
            txtfirstname = new TextBox();
            label4 = new Label();
            txtlastname = new TextBox();
            label5 = new Label();
            txtnationalidentity = new TextBox();
            label6 = new Label();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // txtusername
            // 
            txtusername.Location = new Point(344, 47);
            txtusername.Name = "txtusername";
            txtusername.Size = new Size(100, 23);
            txtusername.TabIndex = 0;
            //txtusername.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(202, 51);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 1;
            label1.Text = "User Name";
            //label1.Click += label1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 288);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(776, 150);
            dataGridView1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(311, 244);
            button1.Name = "button1";
            button1.Size = new Size(166, 23);
            button1.TabIndex = 3;
            button1.Text = "Login";
            button1.UseVisualStyleBackColor = true;
            //button1.Click += button1_Click;
            // 
            // dateofbirthyear
            // 
            dateofbirthyear.CustomFormat = "yyyy";
            dateofbirthyear.Format = DateTimePickerFormat.Custom;
            dateofbirthyear.Location = new Point(294, 193);
            dateofbirthyear.Name = "dateofbirthyear";
            dateofbirthyear.Size = new Size(200, 23);
            dateofbirthyear.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(206, 80);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 7;
            label2.Text = "Password";
            // 
            // txtpassword
            // 
            txtpassword.Location = new Point(344, 76);
            txtpassword.Name = "txtpassword";
            txtpassword.Size = new Size(100, 23);
            txtpassword.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(202, 109);
            label3.Name = "label3";
            label3.Size = new Size(64, 15);
            label3.TabIndex = 9;
            label3.Text = "First Name";
            // 
            // txtfirstname
            // 
            txtfirstname.Location = new Point(344, 105);
            txtfirstname.Name = "txtfirstname";
            txtfirstname.Size = new Size(100, 23);
            txtfirstname.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(203, 139);
            label4.Name = "label4";
            label4.Size = new Size(63, 15);
            label4.TabIndex = 11;
            label4.Text = "Last Name";
            // 
            // txtlastname
            // 
            txtlastname.Location = new Point(344, 135);
            txtlastname.Name = "txtlastname";
            txtlastname.Size = new Size(100, 23);
            txtlastname.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(187, 168);
            label5.Name = "label5";
            label5.Size = new Size(95, 15);
            label5.TabIndex = 13;
            label5.Text = "National Identity";
            // 
            // txtnationalidentity
            // 
            txtnationalidentity.Location = new Point(344, 164);
            txtnationalidentity.Name = "txtnationalidentity";
            txtnationalidentity.Size = new Size(100, 23);
            txtnationalidentity.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(184, 197);
            label6.Name = "label6";
            label6.Size = new Size(100, 15);
            label6.TabIndex = 14;
            label6.Text = "Date Of Birth Year";
            //label6.Click += label6_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(txtnationalidentity);
            Controls.Add(label4);
            Controls.Add(txtlastname);
            Controls.Add(label3);
            Controls.Add(txtfirstname);
            Controls.Add(label2);
            Controls.Add(txtpassword);
            Controls.Add(dateofbirthyear);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Controls.Add(label1);
            Controls.Add(txtusername);
            Name = "Form1";
            Text = "Login User";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtusername;
        private Label label1;
        private DataGridView dataGridView1;
        private Button button1;
        private DateTimePicker dateofbirthyear;
        private Label label2;
        private TextBox txtpassword;
        private Label label3;
        private TextBox txtfirstname;
        private Label label4;
        private TextBox txtlastname;
        private Label label5;
        private TextBox txtnationalidentity;
        private Label label6;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
