using System.Drawing;

namespace Cartheur.Animals.CF.Gui.Forms
{
    partial class MainForm
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
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.clearConsoleMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.fromRhodaMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.GoMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.lastResultMenuItem = new System.Windows.Forms.MenuItem();
            this.lastRequestMenuItem = new System.Windows.Forms.MenuItem();
            this.shutdownMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.addExtraCodeFragmentsMenuItem = new System.Windows.Forms.MenuItem();
            this.addExtraAssembliesMenuItem = new System.Windows.Forms.MenuItem();
            this.emotionUsedMenuItem = new System.Windows.Forms.MenuItem();
            this.learningUsedMenuItem = new System.Windows.Forms.MenuItem();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.OutputBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.clearConsoleMenuItem);
            this.menuItem1.MenuItems.Add(this.menuItem3);
            this.menuItem1.Text = "File";
            // 
            // clearConsoleMenuItem
            // 
            this.clearConsoleMenuItem.Text = "Clear console";
            this.clearConsoleMenuItem.Click += new System.EventHandler(this.ClearConsoleMenuItemClick);
            // 
            // menuItem3
            // 
            this.menuItem3.MenuItems.Add(this.fromRhodaMenuItem);
            this.menuItem3.Text = "Open aeon...";
            // 
            // fromRhodaMenuItem
            // 
            this.fromRhodaMenuItem.Text = "Rhoda";
            this.fromRhodaMenuItem.Click += new System.EventHandler(this.FromRhodaMenuItemClick);
            // 
            // menuItem2
            // 
            this.menuItem2.MenuItems.Add(this.GoMenuItem);
            this.menuItem2.MenuItems.Add(this.menuItem6);
            this.menuItem2.MenuItems.Add(this.shutdownMenuItem);
            this.menuItem2.MenuItems.Add(this.menuItem4);
            this.menuItem2.MenuItems.Add(this.emotionUsedMenuItem);
            this.menuItem2.MenuItems.Add(this.learningUsedMenuItem);
            this.menuItem2.Text = "Settings";
            // 
            // GoMenuItem
            // 
            this.GoMenuItem.Text = "Go";
            this.GoMenuItem.Click += new System.EventHandler(this.GoMenuItemClick);
            // 
            // menuItem6
            // 
            this.menuItem6.MenuItems.Add(this.lastResultMenuItem);
            this.menuItem6.MenuItems.Add(this.lastRequestMenuItem);
            this.menuItem6.Text = "Debug";
            // 
            // lastResultMenuItem
            // 
            this.lastResultMenuItem.Text = "Last result";
            this.lastResultMenuItem.Click += new System.EventHandler(this.LastResultMenuItemClick);
            // 
            // lastRequestMenuItem
            // 
            this.lastRequestMenuItem.Text = "Last request";
            this.lastRequestMenuItem.Click += new System.EventHandler(this.LastRequestMenuItemClick);
            // 
            // shutdownMenuItem
            // 
            this.shutdownMenuItem.Text = "Shutdown";
            this.shutdownMenuItem.Click += new System.EventHandler(this.ShutdownMenuItemClick);
            // 
            // menuItem4
            // 
            this.menuItem4.MenuItems.Add(this.addExtraCodeFragmentsMenuItem);
            this.menuItem4.MenuItems.Add(this.addExtraAssembliesMenuItem);
            this.menuItem4.Text = "Add extras";
            // 
            // addExtraCodeFragmentsMenuItem
            // 
            this.addExtraCodeFragmentsMenuItem.Text = "Aeon files";
            this.addExtraCodeFragmentsMenuItem.Click += new System.EventHandler(this.addExtraCodeFragmentsMenuItem_Click);
            // 
            // addExtraAssembliesMenuItem
            // 
            this.addExtraAssembliesMenuItem.Text = "Libraries";
            this.addExtraAssembliesMenuItem.Click += new System.EventHandler(this.addExtraAssembliesMenuItem_Click);
            // 
            // emotionUsedMenuItem
            // 
            this.emotionUsedMenuItem.Text = "Mood";
            this.emotionUsedMenuItem.Click += new System.EventHandler(this.EmotionUsedMenuItemClick);
            // 
            // learningUsedMenuItem
            // 
            this.learningUsedMenuItem.Text = "Learning";
            this.learningUsedMenuItem.Click += new System.EventHandler(this.LearningUsedMenuItemClick);
            // 
            // InputBox
            // 
            this.InputBox.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.InputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InputBox.Location = new System.Drawing.Point(0, 158);
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(310, 35);
            this.InputBox.TabIndex = 3;
            // 
            // OutputBox
            // 
            this.OutputBox.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.OutputBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.OutputBox.Location = new System.Drawing.Point(0, 0);
            this.OutputBox.Multiline = true;
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.Size = new System.Drawing.Size(310, 158);
            this.OutputBox.TabIndex = 4;
            this.OutputBox.TextChanged += new System.EventHandler(this.OutputBox_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(310, 186);
            this.Controls.Add(this.OutputBox);
            this.Controls.Add(this.InputBox);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "Emotional toys";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem fromRhodaMenuItem;
        private System.Windows.Forms.MenuItem clearConsoleMenuItem;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem shutdownMenuItem;
        private System.Windows.Forms.TextBox InputBox;
        private System.Windows.Forms.TextBox OutputBox;
        private System.Windows.Forms.MenuItem GoMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem addExtraCodeFragmentsMenuItem;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem lastResultMenuItem;
        private System.Windows.Forms.MenuItem lastRequestMenuItem;
        private System.Windows.Forms.MenuItem addExtraAssembliesMenuItem;
        private System.Windows.Forms.MenuItem emotionUsedMenuItem;
        private System.Windows.Forms.MenuItem learningUsedMenuItem;
    }
}
