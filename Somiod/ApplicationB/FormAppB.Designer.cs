
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
            this.SuspendLayout();
            // 
            // button_lightOn
            // 
            this.button_lightOn.Location = new System.Drawing.Point(178, 85);
            this.button_lightOn.Name = "button_lightOn";
            this.button_lightOn.Size = new System.Drawing.Size(195, 105);
            this.button_lightOn.TabIndex = 0;
            this.button_lightOn.Text = "Light ON";
            this.button_lightOn.UseVisualStyleBackColor = true;
            this.button_lightOn.Click += new System.EventHandler(this.button_lightOn_Click);
            // 
            // button_lightOff
            // 
            this.button_lightOff.Location = new System.Drawing.Point(178, 232);
            this.button_lightOff.Name = "button_lightOff";
            this.button_lightOff.Size = new System.Drawing.Size(195, 105);
            this.button_lightOff.TabIndex = 1;
            this.button_lightOff.Text = "Light OFF";
            this.button_lightOff.UseVisualStyleBackColor = true;
            this.button_lightOff.Click += new System.EventHandler(this.button_lightOff_Click);
            // 
            // FormAppB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_lightOff);
            this.Controls.Add(this.button_lightOn);
            this.Name = "FormAppB";
            this.Text = "AppB";
            this.Load += new System.EventHandler(this.FormAppB_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_lightOn;
        private System.Windows.Forms.Button button_lightOff;
    }
}

