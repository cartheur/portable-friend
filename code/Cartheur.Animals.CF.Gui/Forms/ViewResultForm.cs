using System;
using System.Windows.Forms;

namespace Cartheur.Animals.CF.Gui.Forms
{
    public partial class ViewResultForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewResultForm"/> class.
        /// </summary>
        public ViewResultForm()
        {
            InitializeComponent();
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Sets the output message to the window.
        /// </summary>
        public string OutputMessage
        {
            set
            {
                ResultOutput.Text = value;
            }
        }
    }
}
