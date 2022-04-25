using DPAPI;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZipMake
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DocPassword?.Length > 0)
            {
                try
                {
                    this.textBoxPassword.Text = DecryptDPAPI(Properties.Settings.Default.DocPassword);
                }
                catch(Exception exp)
                {
                    MessageBox.Show("パスワードの復元に失敗しました。もう一度パスワードを設定してください。");
                }
            }

            textChangeFlg = false; // パスワード変更フラグ
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textChangeFlg)
            {
                Properties.Settings.Default.DocPassword = EncryptDPAPI(this.textBoxPassword.Text);
                Properties.Settings.Default.Save();
            }
        }

        string EncryptDPAPI(string s)
        {
            DataProtector dp = new DataProtector(
                DataProtector.Store.USE_MACHINE_STORE);
            byte[] dataToEncrypt =
                Encoding.UTF8.GetBytes(s);
            // Not passing optional entropy in this example
            // Could pass random value (stored by the application) for added
            // security when using DPAPI with the machine store.
            return Convert.ToBase64String(dp.Encrypt(dataToEncrypt, null));
        }

        string DecryptDPAPI(string enc)
        {
            DataProtector dp = new
                    DataProtector(DataProtector.Store.USE_MACHINE_STORE);
            byte[] dataToDecrypt =
                Convert.FromBase64String(enc);
            // Optional entropy parameter is null. 
            // If entropy was used within the Encrypt method, the same entropy
            // parameter must be supplied here
            return Encoding.UTF8.GetString(dp.Decrypt(dataToDecrypt, null));
        }

        private void buttonMakeZip_Click(object sender, EventArgs e)
        {
            // 入力ファイル一覧
            var fileInfo = this.listBoxFiles.Items.Cast<ListBoxItemFile>().FirstOrDefault();

            if (fileInfo == null)
            {
                MessageBox.Show("ファイルがありません。");
                return;
            }

            string dir = System.IO.Path.GetDirectoryName(fileInfo.FullFileName);
            string makeZipFileName = System.IO.Path.Combine(
                dir,
                System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullFileName) + ".zip");

            // すでに ZIP ファイルが存在する場合はエラーメッセージを出して処理を終了する。
           if (System.IO.File.Exists(makeZipFileName))
            {
                MessageBox.Show("すでに ZIP ファイルが存在するため ZIPファイルは作れません。");
                return;
            }

            // 最初のファイルがあるディレクトリに ZIP ファイルを作成する。
            try
            {
                //(1)ZIPクラスをインスタンス化
                using (ZipFile zip = new ZipFile(Encoding.GetEncoding("shift_jis")))
                {
                    //(2)圧縮レベルを設定
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                    if (this.textBoxPassword.Text.Length > 0)
                    {
                        // パスワードがある場合は設定する
                        zip.Password = this.textBoxPassword.Text;
                    }

                    foreach (var file in this.listBoxFiles.Items.Cast<ListBoxItemFile>())
                    {
                        //(3)ファイルを追加
                        zip.AddFile(file.FullFileName, "");

                    }
                    //(5)ZIPファイルを保存
                    zip.Save(makeZipFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ZIP作成失敗");
            }


        }

        private void listBoxFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void listBoxFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string fileName
                             in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    ListBoxItemFile item = new ListBoxItemFile()
                    {
                        FileName = System.IO.Path.GetFileName(fileName),
                        FullFileName = fileName,
                    };

                    this.listBoxFiles.Items.Add(item);
                }
            }
        }

        class ListBoxItemFile
        {
            public string FileName { get; set; }

            public string FullFileName { get; set; }

            public override string ToString()
            {
                return FileName;
            }
        }

        bool textChangeFlg = false;

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            textChangeFlg = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.listBoxFiles.Items.Clear();
        }
    }
}
