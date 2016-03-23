using System;
using System.Windows.Forms;

namespace ReplayEditor2.MetadataEditor
{
    public class RadioSelection : Panel
    {
        public byte Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                (this.Controls[value] as RadioButton).Checked = true;
            }
        }
        private byte value;

        public RadioSelection(int x, int y, string[] labels)
        {
            this.Left = x;
            this.Top = y;
            this.Width = 400;
            this.Height = 25;
            int xOffset = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                xOffset += this.PlaceRadioButtonAt(xOffset, labels[i]).Width + 5;
            }
            this.Value = 0;
        }

        private RadioButton PlaceRadioButtonAt(int x, string label)
        {
            RadioButton obj = new RadioButton();
            obj.AutoSize = true;
            obj.Left = x;
            obj.Top = 3;
            obj.Text = label;
            obj.CheckedChanged += obj_CheckedChanged;
            this.Controls.Add(obj);
            return obj;
        }

        private void obj_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton obj = sender as RadioButton;
            for (int i = 0; i < this.Controls.Count; i ++)
            {
                if (this.Controls[i].Equals(obj))
                {
                    this.value = (byte)i;
                    break;
                }
            }
        }
    }
}
