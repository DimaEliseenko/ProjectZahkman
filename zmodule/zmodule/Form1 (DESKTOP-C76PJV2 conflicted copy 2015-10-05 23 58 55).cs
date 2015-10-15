using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;

namespace zmodule
{
    public partial class Form1 : Form
    {

        Graphics g;  // Графическая переменная, изменения в ней изменяют pictureBox, далее "полотно"


        int bx = 1000; // запоминает положение стрелки по Х
        int by = 1000; // по У
        int x1 = 0, y1 = 0;// для рисования линии

        bool bRet = false;
        bool bLine = false;
        bool bDrawLine = false;
        bool bEdit = false;

        Pen rPen = new Pen(Color.Red, 2);
        Pen clearPen = new Pen(Color.Gray, 2);
        GraphicsPath gPath;

        List<GraphicsPath> listGPath = new List<GraphicsPath>(); // запоминает все фигуры, нарисованные на pictureBox


        public Form1()
        {
            InitializeComponent();
            this.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            this.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            imageBox.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 250;
            imageBox.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height - 150;
            DoubleBuffered = true;
            g = imageBox.CreateGraphics();
            g.Clear(Color.Gray);
          //  timer1.Interval = 400; //частота очистки экрана

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        public void Correct_Work() // модуль проверки корректной работы программы при старте. Проверяет наличие необходимых папок и файлов
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button15_Click(object sender, EventArgs e)
        {

        }
    }
}
