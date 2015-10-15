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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace zmodule
{
    public partial class Form1 : Form
    {

        /* Описание нововведений в обновлении

        Родион, v0.117 , 10.10.2015 20:04
        Реализована полная переносимость линий и квадратов. 
        Возможность "тянуть" линию за один из концов убрана за ненадобностью.
        (Эта возможность была тестовой)
        Сделан класс, который в будущем необходимо будет сериализовать.
        В данный момент необходимо изучить сериализацию классов
        Сериализация на данный момент не работает

        Родион, v0, 116, 10.10.2015 0:57
        Теперь графический движок использует новый графический класс с расширенными возможностями. 
        Переписана большая часть кода
        Чтобы изменить линию, нужно нажать кнопку "Редактировать" и ткнуть мышью на ее ПЕРВУЮ ТОЧКУ,
        ( от которого вы протянули вторую точку). При успешной операции, точка прилипнет к курсору 
        и будет доступно редактирование.

        Родион, v0.112, 07.10.2015 , 1:07
        добавлена функция поиска графических элементов, и функция удаления.
        Теперь пользователь может выделить элемент или отправить команду на его удаление (при вклчении
        "Режима удаления". Для распознавания линии нужно щелкнуть мышкой на ее концах, они похожи на кружки.
        При успешном распозновании объект станет зеленым или будет удален.

        */


        #region Системный раздел (территория Димы)
        // Считывание необходимых переменных

        string Repository = Properties.Settings.Default.RepositoryPath;
        string User = Properties.Settings.Default.UserNow;
        string WorkFold = Properties.Settings.Default.WorkDirectory;
        private void button17_Click(object sender, EventArgs e)
        {
            new_profile prof = new new_profile();
            prof.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        //public void Correct_Work() // модуль проверки корректной работы программы при старте. Проверяет наличие необходимых папок и файлов
        //{

        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.First_Run = "";
            Properties.Settings.Default.Save();
            string firstRUN = Properties.Settings.Default.First_Run;
            if (firstRUN != "TRUE")
            {
                setup_settings ss = new setup_settings();
                ss.ShowDialog();
            }
            if (User == null)
            {
                authorization aut = new authorization();
                aut.ShowDialog();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        { // Сериализация на данный момент не работает!!
            try {
                SerializableArrayGraphicsObjects SMGO = new SerializableArrayGraphicsObjects();
                SMGO.setObject(0, listGObject);
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog1.FileName;
                    FileStream myStream = new FileStream(fileName, FileMode.Create);
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(myStream, SMGO);
                    myStream.Close();

                    // BW.Write(SMGO);

                }
             }
             catch(System.Runtime.Serialization.SerializationException) { MessageBox.Show("Ошибка сериализации!"); }
        }

        private void openButton_Click(object sender, EventArgs e)
        {

        }

        #endregion





        #region Раздел с графикой (территория Родиона)


        Graphics g;  // Графическая переменная, изменения в ней изменяют pictureBox, далее "полотно"


        int bx = 1000; // запоминает положение стрелки по Х
        int by = 1000; // по У
        int x1 = 0, y1 = 0;// для рисования линии

       // bool bRet = false;
        //bool bLine = false;
        bool bDrawLine = false;
        //bool bTransfer = false;
        int lenght = 0, height = 0;
        byte typeEdit = 0;
       bool firstStart = true;

        static Color foneColor = Color.White;
        Pen blackPen = new Pen(Color.Black, 2);
      //  Pen rPen = new Pen(Color.Red, 2);
        Pen gPen = new Pen(Color.Green, 2);
        Pen clearPen = new Pen(foneColor, 2);
        //GraphicsPath gPath;
        GraphicsObject gObject;
        //List<GraphicsPath> listGPath = new List<GraphicsPath>(); // запоминает все фигуры, нарисованные на pictureBox
        List<GraphicsObject> listGObject = new List<GraphicsObject>(); // запоминает все фигуры, нарисованные на pictureBox
        
        public Form1()
        {
            InitializeComponent();
            this.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            this.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            imageBox.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 250;
            imageBox.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height - 200;
            panel3.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 630;
            DoubleBuffered = true;
            g = imageBox.CreateGraphics();
            g.Clear(foneColor);
            //  timer1.Interval = 400; //частота очистки экрана
           
        }



        #region Функции
        // перерисовывает все из коллекции на "полотно"
        private void drawCollection(List<GraphicsObject> lgObject, Pen pen)
        {

            foreach (GraphicsObject gobject in lgObject)
            {
                GraphicsPath gPath = gobject.gPath;
                g.DrawPath(pen, gPath);
            }
        }

        private GraphicsObject searchGpaphicsPath (List<GraphicsObject> lgObject, int x, int y)
        {
            foreach (GraphicsObject gobject in lgObject)
            {
                if(gobject.isVisiblePath(x, y))
                {
                    g.DrawPath(gPen, gobject.gPath);
                    timer1.Enabled = false;
                    return gobject;
                }
            }
            return null;
        }

        private void deleteGraphicsPath(GraphicsObject gObject)
        { 
            try {
                g.DrawPath(clearPen, gObject.gPath);
                listGObject.Remove(gObject);
                drawCollection(listGObject, blackPen);
            }
            catch (System.ArgumentNullException)
            {

            }
            catch (System.NullReferenceException)
            {
                //MessageBox.Show("Попытка вызова пустого класса?");
            }

        }

        private GraphicsObject drawLine(int x1, int y1, int x2, int y2, Pen pen)
        {
            gObject = new GraphicsObject("line");
            if (gObject.setLine(x1, y1, x2, y2))
            {
               // Debug1.setInfo(gObject.zoneX1, gObject.zoneX11, gObject.zoneY1, gObject.zoneY11);
                g.DrawPath(pen, gObject.gPath);
                return gObject;
            }
            else return null;
        } 



        private GraphicsObject drawRetangle (int x, int y, Pen pen)
        {
            gObject = new GraphicsObject("retangle");
            if (gObject.setRetangle(x, y, (int)retLength.Value, (int)retHeight.Value))
            {
               // Debug1.setInfo(gObject.zoneX1, gObject.zoneX11, gObject.zoneY1, gObject.zoneY11);
                g.DrawPath(pen, gObject.gPath);
                return gObject;
            }
            else return null;
        }

        #endregion

        #region События
        private void timer1_Tick(object sender, EventArgs e)
        {
           // g.Clear(foneColor);
            if (typeEdit ==2) drawRetangle(bx, by, blackPen);
           // if (typeEdit ==1)  drawLine(x1, y1, bx, by, rPen);           
            drawCollection(listGObject, blackPen);
        }

        private void retButton_Click(object sender, EventArgs e)
        {
            typeEdit = 2;
            timer1.Enabled = true; // запускаем авто-очистку
            bx = by = 1000;
        }


        private void editButton_Click(object sender, EventArgs e)
        {
            typeEdit = 3;
            timer1.Enabled = true;
            bx = by = 1000;
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            typeEdit = 1;
            timer1.Enabled = true; // запускаем авто-прорисовку
            bx = by = 1000;
        }

        private void imageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (typeEdit ==1)
            {          
                x1 = bx = e.X;
                by = y1 = e.Y;
                bDrawLine = true;
            }
        }

        private void imageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (bDrawLine)
            {

                g.Clear(foneColor); 
                if (x1 != e.X && y1 != e.Y) listGObject.Add(drawLine(x1, y1, e.X, e.Y, blackPen));
                drawCollection(listGObject, blackPen); // нарисовать элементы из коллекции.
                typeEdit = 0;
                bDrawLine = false;
            }
        }

        private void общиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings set = new Settings();
            set.ShowDialog();
        }

        private void imageBox_MouseClick(object sender, MouseEventArgs e)
        {
            try {
                if (typeEdit == 5)
                {
                    if (gObject.type == "line")
                    {
                        gObject.setLine(bx, by, bx + lenght, by + height);
                        g.DrawPath(clearPen, gObject.gPath);
                        gObject.setLine(e.X, e.Y, e.X + lenght, e.Y + height);
                        bx = e.X;
                        by = e.Y;
                        g.DrawPath(blackPen, gObject.gPath);
                        listGObject.Add(gObject);
                        typeEdit = 0;
                        timer1.Enabled = false;
                    }
                    if (gObject.type == "retangle")
                    {
                        gObject.setRetangle(bx, by, gObject.x2, gObject.y2);
                        g.DrawPath(clearPen, gObject.gPath);
                        gObject.setRetangle(e.X, e.Y, gObject.x2, gObject.y2);
                        bx = e.X;
                        by = e.Y;
                        g.DrawPath(blackPen, gObject.gPath);
                        listGObject.Add(gObject);
                        typeEdit = 0;
                        timer1.Enabled = false;
                    }


                }

                if (typeEdit == 3)
                {
                    if (checkBox1.Checked) deleteGraphicsPath(searchGpaphicsPath(listGObject, e.X, e.Y));
                    else
                    {
                        gObject = searchGpaphicsPath(listGObject, e.X, e.Y);
                        listGObject.Remove(gObject);
                        lenght = gObject.lenght();
                        height = gObject.height();
                        typeEdit = 5;
                        timer1.Enabled = true;
                    }

                    //  MessageBox.Show("тычок:  " + e.X + "-" + e.Y );
                }

            }
            catch (System.ArgumentNullException) { typeEdit = 3; }
            catch (System.NullReferenceException) { typeEdit = 3; }


            if (typeEdit == 2) // если фигура выбрана
                {


                    timer1.Enabled = false;
                    g.Clear(foneColor);
                    listGObject.Add(drawRetangle(e.X, e.Y, blackPen));
                    drawCollection(listGObject, blackPen); // рисуем фигуры

                    typeEdit = 0; // фигура поставлена
                }
            
          

        }


        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {
            try {
                if (typeEdit == 5)
                {
                    
                    if (gObject.type == "line")
                    {
                     

                        gObject.setLine(bx, by, bx + lenght, by + height);
                        g.DrawPath(clearPen, gObject.gPath);
                        gObject.setLine(e.X, e.Y, e.X + lenght, e.Y + height);
                        bx = e.X;
                        by = e.Y;
                        g.DrawPath(blackPen, gObject.gPath);
                    }

                    if (gObject.type == "retangle")
                    {
                        gObject.setRetangle(bx, by, gObject.x2, gObject.y2);
                        g.DrawPath(clearPen, gObject.gPath);
                        gObject.setRetangle(e.X, e.Y, gObject.x2, gObject.y2);
                        bx = e.X;
                        by = e.Y;
                        g.DrawPath(blackPen, gObject.gPath);
                    }

                }

                if (firstStart)
                {
                    g.Clear(foneColor);
                    firstStart = false;
                }
                if (typeEdit == 2)  // если фигура выбрана
                {
                    drawRetangle(bx, by, clearPen);
                    drawRetangle(e.X, e.Y, blackPen);
                    bx = e.X;
                    by = e.Y;

                }

                if (bDrawLine)
                {
                    drawLine(x1, y1, bx, by, clearPen);
                    drawLine(x1, y1, e.X, e.Y, blackPen);
                    bx = e.X;
                    by = e.Y;
                    drawCollection(listGObject, blackPen); // нарисовать элементы из коллекции.
                }
            }
            catch (System.NullReferenceException) { }
            
        }
        #endregion

        #endregion

    }
}
