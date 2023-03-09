using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace LocalStorageBookmarker
{
    public partial class Main : Form
    {
        RegistryKey rkey;
        bool login;

        public Main()
        {
            InitializeComponent();
            rkey = Registry.CurrentUser.CreateSubKey("Local Storage Bookmarker");
            fileItemBindingSource.Clear();
            string[] keys = rkey.GetValueNames();
            for (int i = 0; i < keys.Length; i++)
            {
                fileItemBindingSource.Add(new FileItem(rkey.GetValue(i.ToString("D3")).ToString()));
            }
            login = false;
            tbUser.Enabled = true;
            btnLogIn.Hide();
            btnReset.Hide();
        }

        private void toggleLogIn()
        {
            if (tbUser.Text.Length == 0)
            {
                return;
            }
            if (!login)
            {
                rkey = rkey.CreateSubKey(tbUser.Text);
                fileItemBindingSource.Clear();
                string[] keys = rkey.GetValueNames();
                for (int i = 0; i < keys.Length; i++)
                {
                    fileItemBindingSource.Add(new FileItem(rkey.GetValue(i.ToString("D3")).ToString()));
                }
                login = true;
                tbUser.Enabled = false;
                tbUser.Hide();
                btnTbHolder.Hide();
                btnLogIn.Show();
                btnReset.Show();
            }
            else
            {
                rkey = Registry.CurrentUser.CreateSubKey("Local Storage Bookmarker");
                fileItemBindingSource.Clear();
                string[] keys = rkey.GetValueNames();
                for (int i = 0; i < keys.Length; i++)
                {
                    fileItemBindingSource.Add(new FileItem(rkey.GetValue(i.ToString("D3")).ToString()));
                }
                login = false;
                tbUser.Text = "";
                tbUser.Enabled = true;
                tbUser.Show();
                btnTbHolder.Show();
                btnLogIn.Hide();
                btnReset.Hide();
            }
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            toggleLogIn();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("모든 내용이 삭제됩니다.\n 정말로 초기화하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            string User = tbUser.Text;
            toggleLogIn();
            rkey.DeleteSubKey(User);
            fileItemBindingSource.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog filePath = new OpenFileDialog();
            if (filePath.ShowDialog() != DialogResult.OK) return;
            rkey.SetValue(fileItemBindingSource.Count.ToString("D3") , filePath.FileName);
            fileItemBindingSource.Add(new FileItem(filePath.FileName));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selected = from DataGridViewCell cell in dgvFileItem.SelectedCells select cell.RowIndex;
            foreach (int index in selected)
            {
                rkey.DeleteValue(index.ToString("D3"));
            }
            string[] keys = rkey.GetValueNames();
            string[] values = new string[keys.Length];
            fileItemBindingSource.Clear();
            for (int i = 0; i < keys.Length; i++)
            {
                values[i] = rkey.GetValue(keys[i]).ToString();
                rkey.DeleteValue(keys[i]);
                rkey.SetValue(i.ToString("D3"), values[i]);
                fileItemBindingSource.Add(new FileItem(values[i]));
            }
        }

        private void dgvFileItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            FileItem fileItem = fileItemBindingSource[e.RowIndex] as FileItem;
            if (!fileItem.IsFileValid())
            {
                MessageBox.Show("파일을 찾을 수 없습니다.", "경고");
                return;
            }
            Process.Start(fileItem.path);
        }

        private void tbUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toggleLogIn();
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            foreach(string path in e.Data.GetData(DataFormats.FileDrop) as string[])
            {
                rkey.SetValue(fileItemBindingSource.Count.ToString("D3"), path);
                fileItemBindingSource.Add(new FileItem(path));
            }
        }

        private void tbUser_Enter(object sender, EventArgs e)
        {
            tbUser.Text = "";
            tbUser.UseSystemPasswordChar = true;
            tbUser.ForeColor = System.Drawing.Color.FromArgb(252, 252, 252);
            tbUser.BackColor = System.Drawing.Color.FromArgb(48, 49, 56);
            btnTbHolder.BackColor = System.Drawing.Color.FromArgb(48, 49, 56);
            btnTbHolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 49, 56);
            btnTbHolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(48, 49, 56);
            tbUser.Focus();
        }

        private void tbUser_Leave(object sender, EventArgs e)
        {
            if (tbUser.Text.Length == 0)
            {
                tbUser.ForeColor = System.Drawing.Color.FromArgb(190, 190, 190);
                tbUser.UseSystemPasswordChar = false;
                tbUser.Text = "Import";
                tbUser.BackColor = System.Drawing.Color.FromArgb(41, 42, 47);
                btnTbHolder.BackColor = System.Drawing.Color.FromArgb(41, 42, 47);
                btnTbHolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(41, 42, 47);
                btnTbHolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(41, 42, 47);
            }
        }

        private void dgvFileItem_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            OpenFileDialog filePath = new OpenFileDialog();
            if (filePath.ShowDialog() != DialogResult.OK) return;
            rkey.SetValue(fileItemBindingSource.Count.ToString("D3"), filePath.FileName);
            fileItemBindingSource.Add(new FileItem(filePath.FileName));
        }

        private void dgvFileItem_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var selected = from DataGridViewCell cell in dgvFileItem.SelectedCells select cell.RowIndex;
                foreach (int index in selected)
                {
                    rkey.DeleteValue(index.ToString("D3"));
                }
                string[] keys = rkey.GetValueNames();
                string[] values = new string[keys.Length];
                fileItemBindingSource.Clear();
                for (int i = 0; i < keys.Length; i++)
                {
                    values[i] = rkey.GetValue(keys[i]).ToString();
                    rkey.DeleteValue(keys[i]);
                    rkey.SetValue(i.ToString("D3"), values[i]);
                    fileItemBindingSource.Add(new FileItem(values[i]));
                }
            }
        }
    }
}
