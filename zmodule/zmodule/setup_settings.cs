using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Management;

namespace zmodule
{
    public partial class setup_settings : Form
    {
        public setup_settings()
        {
            InitializeComponent();
        }
        string RepName = "\\Zahkman Graphics Editor\\Repository";
        string Repository_way; // путь к Репозиторию

        private void button2_Click(object sender, EventArgs e) // выбор репозитория
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Выберите папку для Репозитория";
            if (fbd.ShowDialog()== DialogResult.OK)
            {
                Repository_way = fbd.SelectedPath;
                textBox1.Text = fbd.SelectedPath;
            }
            else
            {
                Repository_way = "";
            }
        }

        private void button1_Click(object sender, EventArgs e) // запуск создания настроек
        {
            setupStartSettings();
        }

        public void setupStartSettings() // начало создания параметров для работы
        {
            if (!String.IsNullOrWhiteSpace(Repository_way)) // проверяем значение репозиторного пути
            {
                string Provider = null;
                    switch (comboBox1.SelectedItem.ToString())
                    {
                        case "Microsoft ACE 4.0 (Windows XP)": Provider = "Microsoft.Jet.OLEDB.4.0"; break;
                        case "Microsoft ACE 12.0 (Windows 7, 8, 10)": Provider = "Microsoft.ACE.OLEDB.12.0"; break;
                        default: MessageBox.Show("Выберите поставщика!"); break;
                    }
                    string pathRep = "";
                    pathRep = Repository_way + RepName;
                    Repository_way = pathRep;
                    string Conf = Application.StartupPath + "\\Configurations";
                    Directory.CreateDirectory(Repository_way); // создаем Репозиторий
                    Directory.CreateDirectory(Conf); // создаем конф.папку
                // создание настроек
                save_settings(Conf + "\\repository.zgec", Repository_way);
                save_settings(Conf + "\\installPath.zgec", Application.StartupPath);
                save_settings(Conf + "\\configpath.zgec", Conf);
                save_settings(Conf + "\\provider.zgec", Provider);
                save_settings(Conf + "\\update.zgec", "null");
                save_settings(Conf + "\\version_app.zgec", Application.ProductVersion);
                save_settings(Conf + "\\otherSettings.zgec", "");
                save_settings(Conf + "\\db_info.zgec", "");
                save_settings(Conf + "\\fr.zgec", "true");
                MessageBox.Show("Параметры успешно приняты!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Не указан путь для Репозитория, либо указан неверно!", "Внутренняя ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            new_profile n = new new_profile();
            n.ShowDialog();
        }

        private void save_settings(string Path, string value) // путь к файлу, его значение
        {
            string filePath = Path; // путь включая имя файла
            FileStream createFile = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter save = new BinaryWriter(createFile);
            save.Write(value);
            save.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f = new Form1();
            f.ShowDialog();
        }
    }
}
