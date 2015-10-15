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
using System.Runtime.Serialization;


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
        private void общиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings set = new Settings();
            set.ShowDialog();
        }
        public void Correct_Work() // модуль проверки корректной работы программы при старте. Проверяет наличие необходимых папок и файлов
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string firstRUN = Properties.Settings.Default.First_Run;
            if (firstRUN != "TRUE")
            {
                setup_settings ss = new setup_settings();
                ss.ShowDialog();
                this.Hide();
            }
            if (User != null)
            {
                authorization aut = new authorization();
                aut.ShowDialog();
            }
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
        String typeEdit = null;

        bool firstStart = true;
        bool timeStop = false;
   //     int tick = 0;

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
            imageBox.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 200;
            imageBox.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height - 200;
            panel3.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 195;
            
            DoubleBuffered = true;
            g = imageBox.CreateGraphics();
            g.Clear(foneColor);
            //  timer1.Interval = 400; //частота очистки экрана
           
        }



        #region Функции
        // перерисовывает все из коллекции на "полотно"
        private void drawCollection(List<GraphicsObject> lgObject, Pen pen)
        {
            try {
                foreach (GraphicsObject gobject in lgObject)
                {
                    g.DrawPath(pen, gobject.gPath);
                }
            }
            catch (System.NullReferenceException) { MessageBox.Show("Проблемы при чтении и прорисовке коллекции"); }
        }

        private GraphicsObject searchGpaphicsPath (List<GraphicsObject> lgObject, int x, int y)
        {
            foreach (GraphicsObject gobject in lgObject)
            {
                if(gobject.isVisiblePath(x, y))
                {
                  // g.DrawPath(gPen, gobject.gPath);
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
            gObject = new GraphicsObject("line", x1 ,y1, x2, y2);
           // gObject.x1 = x1; gObject.x2 = x2; gObject.y1 = y1; gObject.y2 = y2;
            if (gObject.Create())
            {  
                g.DrawPath(pen, gObject.gPath);
                return gObject;
            }
            else return null;
        } 



        private GraphicsObject drawRetangle (int x, int y, Pen pen)
        {
            gObject = new GraphicsObject("retangle", x, y, (int)retLength.Value, (int)retHeight.Value);
            if (gObject.Create())
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
      
            if (firstStart)
            {
                g.Clear(foneColor);
                firstStart = false;
            }
            if (timeStop)
            {
                    drawCollection(listGObject, blackPen);
                    timeStop = false;
                    timer1.Enabled = false;
            }
            // g.Clear(foneColor);
            
           // if (typeEdit ==1)  drawLine(x1, y1, bx, by, rPen);           
            drawCollection(listGObject, blackPen);
        }

        private void retButton_Click(object sender, EventArgs e)
        {
            typeEdit = "retangle";
            timer1.Enabled = true; // запускаем авто-очистку
          //  bx = by = 1000;
        }


        private void editButton_Click(object sender, EventArgs e)
        {
            typeEdit = "edit";
            timer1.Enabled = true;
           // bx = by = 1000;
        }

        private void DrawTextButton_Click(object sender, EventArgs e)
        {
            typeEdit = "drawText";
            timer1.Enabled = true;
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            typeEdit = "line";
            timer1.Enabled = true; // запускаем авто-прорисовку
          //  bx = by = 1000;
        }

        private void imageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (typeEdit =="line")
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
                typeEdit = null;
                bDrawLine = false;
            }
        }

   

        private void imageBox_MouseClick(object sender, MouseEventArgs e)
        {
            try {
                switch (typeEdit)
                {
                    case "editDrag":
                      //  firstStart = true;
                        switch (gObject.type)
                        {
                            case "line":
                                gObject.setLine(bx, by, bx + lenght, by + height);
                                g.DrawPath(clearPen, gObject.gPath);
                                gObject.setLine(e.X, e.Y, e.X + lenght, e.Y + height);
                                bx = e.X;
                                by = e.Y;
                                g.DrawPath(blackPen, gObject.gPath);
                                listGObject.Add(gObject);
                                typeEdit = null;
                                timeStop = true;
                                drawCollection(listGObject, blackPen);
                                break;
                            case "retangle":

                                gObject.setRetangle(bx, by, gObject.x2, gObject.y2);
                                g.DrawPath(clearPen, gObject.gPath);
                                gObject.setRetangle(e.X, e.Y, gObject.x2, gObject.y2);
                                bx = e.X;
                                by = e.Y;
                                g.DrawPath(blackPen, gObject.gPath);
                                listGObject.Add(gObject);
                                typeEdit = null;
                                timeStop = true;
                                drawCollection(listGObject, blackPen);
                                break;

                            default:
                                return;
                                

                        }
                        break;

                    case "edit":
                        if (checkBox1.Checked) deleteGraphicsPath(searchGpaphicsPath(listGObject, e.X, e.Y));
                        else
                        {
                            gObject = searchGpaphicsPath(listGObject, e.X, e.Y);
                            listGObject.Remove(gObject);
                            g.DrawPath(clearPen, gObject.gPath);
                            lenght = gObject.lenght();
                            height = gObject.height();
                            typeEdit = "editDrag";
                            timer1.Enabled = true;
                        }
                        break;

                    case "retangle":
                        timer1.Enabled = false;
                        g.Clear(foneColor);
                        listGObject.Add(drawRetangle(e.X, e.Y, blackPen));
                        drawCollection(listGObject, blackPen); // рисуем фигуры

                        typeEdit = null; // фигура поставлена
                        break;

                    case "drawText":
                        timer1.Enabled = false;
                        gObject.type = "text";
                        gObject.x1 = e.X; gObject.y1 = e.Y;
                      if (!gObject.Create()) MessageBox.Show ("Не удалось создать текст");
                        g.DrawPath(blackPen, gObject.gPath);
                        break;

                    default:
                        return;
                        
                }

            }
            catch (System.ArgumentNullException) { typeEdit = "edit"; }
            catch (System.NullReferenceException) { typeEdit = "edit"; }

           

        }


        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {
            try {
                switch (typeEdit)
                {
                    case "editDrag":
                        switch (gObject.type)
                        {
                            case "line":
                                gObject.setLine(bx, by, bx + lenght, by + height);
                                g.DrawPath(clearPen, gObject.gPath);
                                gObject.setLine(e.X, e.Y, e.X + lenght, e.Y + height);
                                bx = e.X;
                                by = e.Y;
                                g.DrawPath(blackPen, gObject.gPath);
                                break;

                            case "retangle":
                                gObject.setRetangle(bx, by, gObject.x2, gObject.y2);
                                g.DrawPath(clearPen, gObject.gPath);
                                gObject.setRetangle(e.X, e.Y, gObject.x2, gObject.y2);
                                bx = e.X;
                                by = e.Y;
                                g.DrawPath(blackPen, gObject.gPath);
                                break;
                        }
                        break;

                    case "retangle":
                        drawRetangle(bx, by, clearPen);
                        drawRetangle(e.X, e.Y, blackPen);
                        bx = e.X;
                        by = e.Y;

                        break;

                        
                }
                if (firstStart)
                {
                    g.Clear(foneColor);
                    firstStart = false;
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


        #region Раздел сохранения и загрузки



        private void saveButton_Click(object sender, EventArgs e) // сохранение нового проекта
        {
            if (comboBox1.Text == "Выберите модель")
            {
                MessageBox.Show("Вы должны выбрать модель перед сохранением!");
            }
            else
            {

                string fileNameDefault = "Project_1";
                SaveFileDialog svd = new SaveFileDialog();
                svd.Title = "Выберите путь сохранения файла";
                svd.FileName = fileNameDefault;
                svd.Filter = "Zakhman Graphics Editor (*.zge) |*.zge";
                if (svd.ShowDialog() == DialogResult.OK)
                {
                    string typeModel = comboBox1.SelectedItem.ToString();
                    try
                    {
                        binarySave(svd.FileName, typeModel);
                    }
                    catch (System.Runtime.Serialization.SerializationException) { MessageBox.Show("Ошибка сериализации!"); }
                }
                else
                {
                    svd.FileName = "";
                }
                //string FN = @"E:\test\save12";
                //string typeModel = "model1";
                //try
                //{
                //    binarySave(FN, typeModel);
                //}
                //catch (System.Runtime.Serialization.SerializationException) { MessageBox.Show("Ошибка сериализации!"); }
            }
        }

        private void openButton_Click(object sender, EventArgs e) // открытие и загрузка проекта
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Title = "Выберите файл для загрузки в Редактор";
            opf.Filter = "Zakhman Graphics Editor (*.zge) |*.zge";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                string FN = opf.FileName;
               comboBox1.Text = binaryLoad(FN);
                this.Text = "Zakhman Graphics Editor - [ " + Path.GetFileName(FN) + " ]";
                toolStripStatusLabel1.Text = FN + " | ";
                toolStripStatusLabel2.Text = Path.GetFileName(FN) + " | ";
                toolStripStatusLabel3.Text = FN.Length / 1024 + " Кб";
            } 
            try
            {
              
              
            }
            catch (System.Runtime.Serialization.SerializationException) { MessageBox.Show("Ошибка сериализации!"); }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            listGObject.Clear();
            g.Clear(foneColor);
        }

     

        private void binarySave(string fileName, string mType) // сохранение модели
        {
            
            FileStream myStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter BW = new BinaryWriter(myStream);

            BW.Write(mType);
            foreach (GraphicsObject go in listGObject)
            {
                BW.Write(go.type);
                BW.Write(go.x1);
                BW.Write(go.x2);
                BW.Write(go.y1);
                BW.Write(go.y2);
            }
            BW.Close();
            myStream.Close();
        }

        private string binaryLoad(string fileName) // загрузчик моделей
        {

                        
            FileStream myStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader BR = new BinaryReader(myStream);
            GraphicsObject sgo;
            listGObject.Clear();
            string typeModel = BR.ReadString(); // читает тип модели
            while(BR.PeekChar() != - 1)
            {
                sgo = new GraphicsObject();
                sgo.type = BR.ReadString();
                sgo.x1 = BR.ReadInt32();
                sgo.x2 = BR.ReadInt32();
                sgo.y1 = BR.ReadInt32();
                sgo.y2 = BR.ReadInt32(); 
                sgo.Create();
                listGObject.Add(sgo);

            }

            BR.Close();
            myStream.Close();
            g.Clear(foneColor);
            drawCollection(listGObject, blackPen);
            return typeModel;

        }




        #endregion

    }
}
