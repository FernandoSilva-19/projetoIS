
namespace ApplicationB
{
    partial class FormAppB
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
            this.button_lightOn = new System.Windows.Forms.Button();
            this.button_lightOff = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_lightOn
            // 
            this.button_lightOn.BackColor = System.Drawing.Color.White;
            this.button_lightOn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_lightOn.Location = new System.Drawing.Point(51, 93);
            this.button_lightOn.Name = "button_lightOn";
            this.button_lightOn.Size = new System.Drawing.Size(195, 105);
            this.button_lightOn.TabIndex = 0;
            this.button_lightOn.Text = "Light ON";
            this.button_lightOn.UseVisualStyleBackColor = false;
            this.button_lightOn.Click += new System.EventHandler(this.button_lightOn_Click);
            // 
            // button_lightOff
            // 
            this.button_lightOff.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button_lightOff.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button_lightOff.Location = new System.Drawing.Point(51, 204);
            this.button_lightOff.Name = "button_lightOff";
            this.button_lightOff.Size = new System.Drawing.Size(195, 105);
            this.button_lightOff.TabIndex = 1;
            this.button_lightOff.Text = "Light OFF";
            this.button_lightOff.UseVisualStyleBackColor = false;
            this.button_lightOff.Click += new System.EventHandler(this.button_lightOff_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.MediumAquamarine;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(23, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 46);
            this.label1.TabIndex = 2;
            this.label1.Text = "Application B";
            // 
            // FormAppB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(290, 361);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_lightOff);
            this.Controls.Add(this.button_lightOn);
            this.Name = "FormAppB";
            this.Text = "AppB";
            this.Load += new System.EventHandler(this.FormAppB_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_lightOn;
        private System.Windows.Forms.Button button_lightOff;
        private System.Windows.Forms.Label label1;
    }
}

