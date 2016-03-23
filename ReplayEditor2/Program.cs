using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ReplayEditor2
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string[] settings;
            if (File.Exists(MainForm.Path_Settings))
            {
                settings = File.ReadAllLines(MainForm.Path_Settings);
            }
            else
            {
                settings = new string[2] { @"C:\osu!.db", @"C:\osu!\songs\" };
                File.WriteAllLines(MainForm.Path_Settings, settings);
                DialogResult reply = MessageBox.Show("A settings file has been created for you to link to your songs folder. Would you like to edit it now?", "File Created", MessageBoxButtons.YesNo);
                if (reply == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(MainForm.Path_Settings);
                    return;
                }
            }
            MainForm form = new MainForm();
            form.SetSettings(settings);
            form.Show();
            form.Canvas = new Canvas(form.GetPictureBoxHandle(), form);
            form.Canvas.Run();
        }
    }
}
