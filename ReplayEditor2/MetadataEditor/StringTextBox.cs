using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReplayEditor2.MetadataEditor
{
    public class StringTextBox : TextBox
    {
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.Text = value;
            }
        }
        private string value = "";

        public StringTextBox()
        {
            this.Width = 300;
        }
    }
}
