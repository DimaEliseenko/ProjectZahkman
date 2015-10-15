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
using zmodule_db;

namespace zmodule
{
    public partial class authorization : Form
    {
        public authorization()
        {
            InitializeComponent();
            
        }
        Form1 f = new Form1();
        string UserNow;
        string GroupNow;
        string WorkFoldNow;
        string DBPath = Application.StartupPath + "\\zmodule.accdb";
        string Providers = null;

        private void button2_Click(object sender, EventArgs e) // сама авторизация
        {
           
        }

        private string Load_Settings(string PathFile)
        {
            string loadParams = null;
            FileStream myStream = new FileStream(PathFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readSet = new BinaryReader(myStream);
            loadParams = readSet.ReadString();
            return loadParams;
        }

        private void authorization_Load(object sender, EventArgs e)
        {
            Providers = Load_Settings(Application.StartupPath + "\\Configurations\\provider.zgec");
        }
    }
}
