using System;
using System.Windows.Forms;

namespace ReplayEditor2.MetadataEditor
{
    public class ModCheckBox : CheckBox
    {
        public ReplayAPI.Mods ModValue { get; set; }

        public ModCheckBox(int x, int y, string text, int index)
        {
            this.Left = x;
            this.Top = y;
            this.Text = text;
            this.AutoSize = true;
            /*
            00 : No Fail
            01 : Hard Rock
            02 : Relax
            03 : Spun Out
            04 : Double Time
            05 : Easy
            06 : Hidden
            07 : Autopilot
            08 : Perfect
            09 : Nightcore
            10 : Half Time
            11 : Flashlight
            12 : Auto
            13 : Sudden Death
            */
            if (index == 0)
            {
                this.ModValue = ReplayAPI.Mods.NoFail;
            }
            else if (index == 1)
            {
                this.ModValue = ReplayAPI.Mods.HardRock;
            }
            else if (index == 2)
            {
                this.ModValue = ReplayAPI.Mods.Relax;
            }
            else if (index == 3)
            {
                this.ModValue = ReplayAPI.Mods.SpunOut;
            }
            else if (index == 4)
            {
                this.ModValue = ReplayAPI.Mods.DoubleTime;
            }
            else if (index == 5)
            {
                this.ModValue = ReplayAPI.Mods.Easy;
            }
            else if (index == 6)
            {
                this.ModValue = ReplayAPI.Mods.Hidden;
            }
            else if (index == 7)
            {
                this.ModValue = ReplayAPI.Mods.AutoPilot;
            }
            else if (index == 8)
            {
                this.ModValue = ReplayAPI.Mods.Perfect;
            }
            else if (index == 9)
            {
                this.ModValue = ReplayAPI.Mods.NightCore;
            }
            else if (index == 10)
            {
                this.ModValue = ReplayAPI.Mods.HalfTime;
            }
            else if (index == 11)
            {
                this.ModValue = ReplayAPI.Mods.FlashLight;
            }
            else if (index == 12)
            {
                this.ModValue = ReplayAPI.Mods.Auto;
            }
            else if (index == 13)
            {
                this.ModValue = ReplayAPI.Mods.SuddenDeath;
            }
            else
            {
                this.ModValue = ReplayAPI.Mods.None;
            }
        }
    }
}
