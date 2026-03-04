using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace i_freeze.Utilities
{
    public enum PathChoice
    {
        None,
        File,
        Folder
    }

    public class ChooseFileFolderForm : Form
    {
        public PathChoice Choice { get; private set; } = PathChoice.None;

        private Button btnFile;
        private Button btnFolder;
        private Label lblText;

        public ChooseFileFolderForm()
        {
            Text = "Select File or Folder";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Width = 420;
            Height = 140;

            lblText = new Label
            {
                Text = "Select a file or folder to be encrypted",
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Left = 12,
                Top = 12,
                Width = ClientSize.Width - 24,
                Height = 40
            };

            btnFile = new Button
            {
                Text = "File",
                Width = 100,
                Left = 120,
                Top = 64
            };
            btnFile.Click += (s, e) =>
            {
                Choice = PathChoice.File;
                DialogResult = DialogResult.OK;
            };

            btnFolder = new Button
            {
                Text = "Folder",
                Width = 100,
                Left = 230,
                Top = 64
            };
            btnFolder.Click += (s, e) =>
            {
                Choice = PathChoice.Folder;
                DialogResult = DialogResult.OK;
            };

            Controls.Add(lblText);
            Controls.Add(btnFile);
            Controls.Add(btnFolder);
        }
    }
}
