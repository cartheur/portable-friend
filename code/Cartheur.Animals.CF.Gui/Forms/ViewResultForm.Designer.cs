namespace Cartheur.Animals.CF.Gui.Forms
{
    partial class ViewResultForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.CloseMenuItem = new System.Windows.Forms.MenuItem();
            this.ResultOutput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.CloseMenuItem);
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Text = "Close";
            this.CloseMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // ResultOutput
            // 
            this.ResultOutput.Location = new System.Drawing.Point(3, 3);
            this.ResultOutput.Multiline = true;
            this.ResultOutput.Name = "ResultOutput";
            this.ResultOutput.ReadOnly = true;
            this.ResultOutput.Size = new System.Drawing.Size(314, 180);
            this.ResultOutput.TabIndex = 5;
            // 
            // ViewResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(320, 186);
            this.Controls.Add(this.ResultOutput);
            this.Menu = this.mainMenu1;
            this.Name = "ViewResultForm";
            this.Text = "View result";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem CloseMenuItem;
        public System.Windows.Forms.TextBox ResultOutput;
    }
}