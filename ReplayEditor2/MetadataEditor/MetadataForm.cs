using System;
using System.Windows.Forms;

namespace ReplayEditor2.MetadataEditor
{
    public class MetadataForm : Form
    {
        private static string[] modNames = new string[14] { "No Fail", "Hard Rock", "Relax", "Spun Out", "Double Time", "Easy", "Hidden", "Autopilot", "Perfect", "Nightcore", "Half Time", "Flashlight", "Auto", "Sudden Death" };
        private ReplayAPI.Replay replay = null;
        private RadioSelection gameMode;
        private IntTextBox version;
        private StringTextBox playerName;
        private StringTextBox mapHash;
        private StringTextBox replayHash;
        private ShortTextBox count300;
        private ShortTextBox count100;
        private ShortTextBox count50;
        private ShortTextBox countGeki;
        private ShortTextBox countKatsu;
        private ShortTextBox countMiss;
        private ShortTextBox maxCombo;
        private IntTextBox score;
        private RadioSelection isPerfect;
        private ModCheckBox[] mods;
        private ReplayAPI.Mods modValue = ReplayAPI.Mods.None;
        private Button ok;
        private Button cancel;

        public MetadataForm()
        {
            this.Hide();
            this.Owner = MainForm.self;
            this.Text = "Metadata Editor";
            this.Width = 440;
            this.Height = 445;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            this.gameMode = this.PlaceRadioSelectionAt(0, 0, "Game Mode", new string[] { "Standard", "Taiko", "Catch the Beat", "Mania" });
            this.version = this.PlaceIntTextBoxAt(0, 1, "Osu Version");
            this.playerName = this.PlaceStringTextBoxAt(0, 2, "Player Name");
            this.mapHash = this.PlaceStringTextBoxAt(0, 3, "Map Hash");
            this.replayHash = this.PlaceStringTextBoxAt(0, 4, "Replay Hash");
            this.count300 = this.PlaceShortTextBoxAt(0, 5, "Number of 300's");
            this.count100 = this.PlaceShortTextBoxAt(0, 6, "Number of 100's");
            this.count50 = this.PlaceShortTextBoxAt(0, 7, "Number of 50's");
            this.score = this.PlaceIntTextBoxAt(0, 8, "Total Score");
            this.countGeki = this.PlaceShortTextBoxAt(200, 5, "Number of Geki's");
            this.countKatsu = this.PlaceShortTextBoxAt(200, 6, "Number of Katsu's");
            this.countMiss = this.PlaceShortTextBoxAt(200, 7, "Number of Misses");
            this.maxCombo = this.PlaceShortTextBoxAt(200, 8, "Max Combo");
            this.isPerfect = this.PlaceRadioSelectionAt(0, 9, "Perfect Combo", new string[] { "Yes", "No" });
            this.mods = new ModCheckBox[14];
            this.PlaceLabelAt(0, 10 * 25, "Mods");
            for (int i = 0; i < 14; i ++)
            {
                this.mods[i] = this.PlaceModCheckBoxAt(105 + 100 * (i / 5), 10 + (i % 5), modNames[i], i);
                this.mods[i].CheckedChanged += Mods_CheckedChanged;
            }
            this.ok = this.PlaceButtonAt(260, 15, "OK");
            this.ok.Click += ok_Click;
            this.cancel = this.PlaceButtonAt(340, 15, "Cancel");
            this.cancel.Click += cancel_Click;
        }
        private void ok_Click(object sender, EventArgs e)
        {
            // Submits changes
            this.Apply();
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            // Reverts changes
            this.LoadReplay(-1);
            this.Close();
        }

        private void Mods_CheckedChanged(object sender, EventArgs e)
        {
            this.modValue = ReplayAPI.Mods.None;
            for (int i = 0; i < 14; i ++)
            {
                if (this.mods[i].Checked)
                {
                    this.modValue |= mods[i].ModValue;
                }
            }
        }

        public void LoadReplay(int id)
        {
            if (id >= 0)
            {
                this.replay = MainForm.self.CurrentReplays[id];
            }
            if (this.replay == null)
            {
                this.gameMode.Value = 0;
                this.version.Value = 0;
                this.playerName.Value = "";
                this.mapHash.Value = "";
                this.replayHash.Value = "";
                this.count300.Value = 0;
                this.count100.Value = 0;
                this.count50.Value = 0;
                this.score.Value = 0;
                this.countGeki.Value = 0;
                this.countKatsu.Value = 0;
                this.countMiss.Value = 0;
                this.maxCombo.Value = 0;
                this.isPerfect.Value = 0;
                this.modValue = ReplayAPI.Mods.None;
            }
            else
            {
                this.gameMode.Value = (byte)this.replay.GameMode;
                this.version.Value = this.replay.FileFormat;
                this.playerName.Value = this.replay.PlayerName;
                this.mapHash.Value = this.replay.MapHash;
                this.replayHash.Value = this.replay.ReplayHash;
                this.count300.Value = this.replay.Count300;
                this.count100.Value = this.replay.Count100;
                this.count50.Value = this.replay.Count50;
                this.score.Value = (int)this.replay.TotalScore;
                this.countGeki.Value = this.replay.CountGeki;
                this.countKatsu.Value = this.replay.CountKatu;
                this.countMiss.Value = this.replay.CountMiss;
                this.maxCombo.Value = this.replay.MaxCombo;
                this.isPerfect.Value = Convert.ToByte(! this.replay.IsPerfect);
                this.modValue = this.replay.Mods;
            }
            for (int i = 0; i < 14; i++)
            {
                mods[i].Checked = this.modValue.HasFlag(this.mods[i].ModValue);
            }
        }

        private void Apply()
        {
            if (this.replay == null)
            {
                return;
            }
            this.replay.GameMode = (ReplayAPI.GameModes)this.gameMode.Value;
            this.replay.FileFormat = this.version.Value;
            this.replay.PlayerName = this.playerName.Value;
            this.replay.MapHash = this.mapHash.Value;
            this.replay.ReplayHash = this.replayHash.Value;
            this.replay.Count300 = this.count300.Value;
            this.replay.Count100 = this.count100.Value;
            this.replay.Count50 = this.count50.Value;
            this.replay.TotalScore = (uint)this.score.Value;
            this.replay.CountGeki = this.countGeki.Value;
            this.replay.CountKatu = this.countKatsu.Value;
            this.replay.CountMiss = this.countMiss.Value;
            this.replay.MaxCombo = this.maxCombo.Value;
            this.replay.IsPerfect = ! Convert.ToBoolean(this.isPerfect.Value);
            this.replay.Mods = this.modValue;
            MainForm.self.Canvas.ApplyMods(this.replay);
        }

        private Button PlaceButtonAt(int x, int row, string text)
        {
            int y = row * 25;
            Button obj = new Button();
            obj.Left = x;
            obj.Top = y;
            obj.Text = text;
            this.Controls.Add(obj);
            return obj;
        }

        private ModCheckBox PlaceModCheckBoxAt(int x, int row, string label, int index)
        {
            int y = row * 25;
            ModCheckBox obj = new ModCheckBox(x, y + 3, label, index);
            this.Controls.Add(obj);
            return obj;
        }

        private RadioSelection PlaceRadioSelectionAt(int x, int row, string label, string[] labels)
        {
            int y = row * 25;
            this.PlaceLabelAt(x, y, label);
            RadioSelection obj = new RadioSelection(x + 105, y, labels);
            this.Controls.Add(obj);
            return obj;
        }

        private StringTextBox PlaceStringTextBoxAt(int x, int row, string label)
        {
            int y = row * 25;
            this.PlaceLabelAt(x, y, label);
            StringTextBox obj = new StringTextBox();
            obj.Left = x + 105;
            obj.Top = y;
            this.Controls.Add(obj);
            return obj;
        }

        private ShortTextBox PlaceShortTextBoxAt(int x, int row, string label)
        {
            int y = row * 25;
            this.PlaceLabelAt(x, y, label);
            ShortTextBox obj = new ShortTextBox();
            obj.Left = x + 105;
            obj.Top = y;
            this.Controls.Add(obj);
            return obj;
        }

        private IntTextBox PlaceIntTextBoxAt(int x, int row, string label)
        {
            int y = row * 25;
            this.PlaceLabelAt(x, y, label);
            IntTextBox obj = new IntTextBox();
            obj.Left = x + 105;
            obj.Top = y;
            this.Controls.Add(obj);
            return obj;
        }

        private Label PlaceLabelAt(int x, int y, string text)
        {
            Label label = new Label();
            label.AutoSize = false;
            label.Width = 100;
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            label.Left = x;
            label.Top = y;
            this.Controls.Add(label);
            return label;
        }
    }
}
