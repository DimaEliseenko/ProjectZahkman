using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using zmodule_db;


namespace zmodule
{
    public partial class new_profile : Form
    {
        Dictionary<string, string> dct;
        public new_profile()
        {
            InitializeComponent();
            dct = groupBox1.Controls.OfType<TextBox>()
                .ToDictionary(x => x.Name, y => y.Tag.ToString());
            groupBox1.Controls.OfType<TextBox>()
                .ToList()
                .ForEach(x => x.Leave += x_Leave);
        }
        string ProviderSelect = Properties.Settings.Default.DB_Provider;
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        void x_Leave(object sender, EventArgs e)
        {
            var textBox = ((TextBox)sender);
            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Any(x => !char.IsLetter(x)))
                MessageBox.Show("Некорректный вводя для: " + dct[textBox.Name]);
        }

        private void button1_Click(object sender, EventArgs e) // создание профиля
        {
            string pathDBS = Application.StartupPath + "\\zmodule.accdb"; // путь к БД
            string firstName = textBox1.Text;
            string SecondName = textBox2.Text;
            string Fac = textBox3.Text;
            string Group_U = textBox4.Text;
            string Profile = "\\" + firstName + " " + SecondName;
            DataBaseCenter dbc = new DataBaseCenter();
            dbc.New_Profile(pathDBS, ProviderSelect, firstName, SecondName, Group_U, Fac, Profile);
            string Repository = Properties.Settings.Default.RepositoryPath;
            string Path = Repository + Profile;
            Directory.CreateDirectory(Path); // создание папки основной
            // создание подпапок
            Directory.CreateDirectory(Path + "\\Images");
            Directory.CreateDirectory(Path + "\\Projects");
            MessageBox.Show("Профиль успешно создан", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
        }
    }
}
