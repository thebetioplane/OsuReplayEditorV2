using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReplayEditor2.MetadataEditor
{
    public class ShortTextBox : TextBox
    {
        public ushort Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.Text = value.ToString();
            }
        }
        private ushort value;
        private string lastValidatedText;

        public ShortTextBox()
        {
            this.lastValidatedText = "0";
            this.LostFocus += OnChange;
        }

        private void OnChange(object sender, EventArgs e)
        {
            this.Validate();
        }

        private bool Validate()
        {
            try
            {
                this.value = Convert.ToUInt16(this.Text);
            }
            catch (Exception)
            {
                this.Text = this.lastValidatedText;
                return false;
            }
            this.Text = this.value.ToString();
            this.lastValidatedText = this.Text;
            return true;
        }
    }
}
