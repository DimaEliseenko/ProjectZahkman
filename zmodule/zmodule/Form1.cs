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

        Родион, v0.124 , 15.10.2015.
        добвлен текст и линии с тектом. Они почти полностью являются полноценными графическими
        объектами

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
        // Загрузка параметров
        string Repository = null;
        public string User = null; // получаем юзера
        public string WorkFold = null; // его рабочую папку
        public string NumberGroup = null; // его номер группы
        string Provider = null;
        //----------------------------------------
        string Author = "Dima";
        string typeModel = null;
        string Group = "IT1301";
        string Lastedit = DateTime.Now.ToString();
        Settings set = new Settings();

        private string Load_Settings(string PathFile)
        {
            string loadParams = null;
            FileStream myStream = new FileStream(PathFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readSet = new BinaryReader(myStream);
            loadParams = readSet.ReadString();
            return loadParams;
        }
        public void LOAD_PARAMS() // загрузка начальных параметров
        {
           Repository = Load_Settings(Application.StartupPath + "\\Configurations\\repository.zgec");
           Provider = Load_Settings(Application.StartupPath + "\\Configurations\\provider.zgec");
           set.RepositoryNow = Repository;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string firstRUN = Application.StartupPath + "\\Configurations\\fr.zgec";
            FileInfo fr = new FileInfo(firstRUN);
            if (!fr.Exists)
            {
                setup_settings ss = new setup_settings();
                ss.ShowDialog();
                this.Hide();
            }
            else
            {
                User = null; // сам юзер
                WorkFold = null; // папка с проектами текущего профиля
                LOAD_PARAMS();
                set.RepositoryNow = Repository;
                authorization aut = new authorization();
                aut.ShowDialog();
            }

        }
        #endregion



        #region Раздел с графикой (территория Родиона)


        Graphics g;  // Графическая переменная, изменения в ней изменяют pictureBox, далее "полотно"
       // Bitmap bmp; 
         
        int bx = 1000; // запоминает положение стрелки по Х
        int by = 1000; // по У
        int x1 = 0, y1 = 0;// для рисования линии

      //  bool bDrawLine = false;

        int lenght = 0, height = 0;
        string typeEdit = null;

       bool firstStart = true;
        bool timerStop = false;
        

        setTextForm stf = new setTextForm();
        string text = "_";

        static Color foneColor = Color.White;
        Pen blackPen = new Pen(Color.Black, 2);
      //  Pen rPen = new Pen(Color.Red, 2);
        Pen gPen = new Pen(Color.Green, 2);
        Pen clearPen = new Pen(foneColor, 2);

        GraphicsObject gObject;
        List<GraphicsObject> listGObject = new List<GraphicsObject>(); // запоминает все фигуры, нарисованные на pictureBox
        
        public Form1()
        {
            InitializeComponent();
            this.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            this.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            imageBox.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 200;
            imageBox.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height - 200;
            panel3.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 195;
            imagePanel.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height - 180;
            imagePanel.Height = imageBox.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width - 180;
            imagePanel.AutoSize = true;
            imagePanel.AutoScroll = true;

            DoubleBuffered = true;

            g = imageBox.CreateGraphics();
            g.Clear(foneColor);

            //// Тест
            //bmp = new Bitmap(imageBox.Width, imageBox.Height);
            //g = Graphics.FromImage(bmp);
            //g.Clear(foneColor);
            //g.FillRectangle(new SolidBrush(Color.Gainsboro), 0, 0, bmp.Width, bmp.Height);
            //g.DrawLine(blackPen, 0, 0, bmp.Width, bmp.Height);
            //g.DrawLine(blackPen, 0, bmp.Height, bmp.Width, 0);
            //g.Dispose();

            //this.imageBox.Image = bmp;
            //this.imageBox.Size = bmp.Size;
            //this.vScrollBar1.Maximum = bmp.Height + this.ClientSize.Height;
            //this.hScrollBar1.Maximum = bmp.Width + this.ClientSize.Width;
            //this.hScrollBar1.Value = 0;

            //this.Text = "x = 0, y = 0";

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

        private GraphicsObject drawLineAndText(int x1, int y1, int x2, int y2, string text, Pen pen)
        {
            gObject = new GraphicsObject("lineAndText");
            gObject.x1 = x1; gObject.y1 = y1; gObject.x2 = x2; gObject.y2 = y2; gObject.text = text;
            if (gObject.Create())
            {
                g.DrawPath(pen, gObject.gPath);
                return gObject;
            }
            else
            {
                MessageBox.Show("Не указан текст!");
                typeEdit = null;
                return null;
            }
        }

        private GraphicsObject drawText(int x, int y, string text, Pen pen)
        {
            gObject = new GraphicsObject("text");
            gObject.x1 = x; gObject.y1 = y; gObject.text = text;
            if (gObject.Create())
            {
                g.DrawPath(pen, gObject.gPath);
                return gObject;
            }
            else
            {
                MessageBox.Show("Не указан текст!");
                typeEdit = null;
                return null;
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
            if (firstStart)
            {
                g.Clear(foneColor);
                firstStart = false;
            }
            if (timerStop)
            {
                drawCollection(listGObject, blackPen);
                timerStop = false;
                timer1.Enabled = false;
            }
            

            // g.Clear(foneColor);
            if (typeEdit =="drawRetangle") drawRetangle(bx, by, blackPen);
           // if (typeEdit ==1)  drawLine(x1, y1, bx, by, rPen);           
            drawCollection(listGObject, blackPen);
        }

        private void retButton_Click(object sender, EventArgs e)
        {
            typeEdit = "drawRetangle";
            timer1.Enabled = true; // запускаем авто-очистку
            bx = by = 1000;
        }


        private void drawTextButton_Click(object sender, EventArgs e)
        {
            try
            {
            typeEdit = "drawText";
            stf.Show();
            timer1.Enabled = true;
            }
            catch (System.ObjectDisposedException) { stf = new setTextForm(); stf.Show(); }
        }

        private void lineAndTextButton_Click(object sender, EventArgs e)
        {
            try {
                typeEdit = "drawLineAndText";
                stf.Show();
                timer1.Enabled = true;
            } catch(System.ObjectDisposedException) { stf = new setTextForm(); stf.Show(); }

        }

        private void editButton_Click(object sender, EventArgs e)
        {
            try
            {
                typeEdit = "edit";
            stf.Show();
            timer1.Enabled = true;
            bx = by = 1000;
            }
            catch (System.ObjectDisposedException) { stf = new setTextForm(); stf.Show(); }
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            typeEdit = "drawLine";
            timer1.Enabled = true; // запускаем авто-прорисовку
            bx = by = 1000;
        }

        private void imageBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (typeEdit)
            {
                case "drawLineAndText":
                    bx = x1 = e.X;
                    by = y1 = e.Y;
                    typeEdit = "drawLineAndTextMove";
                    timer1.Enabled = true;
                    break;

                case "drawLine":
                    x1 = bx = e.X;
                    by = y1 = e.Y;
                    typeEdit = "drawLineMove";
                    break;
            }

        }

        private void imageBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch (typeEdit)
            {
                case "drawLineAndTextMove":
                    
                    drawLineAndText(x1, y1, bx, y1, stf.text, clearPen);
                    listGObject.Add(drawLineAndText(x1, y1, e.X, y1, stf.text, blackPen));
                    timerStop = true;
                    typeEdit = null;
                    break;
                case "drawLineMove":
                    g.Clear(foneColor);
                    if (x1 != e.X && y1 != e.Y) listGObject.Add(drawLine(x1, y1, e.X, e.Y, blackPen));
                    drawCollection(listGObject, blackPen); // нарисовать элементы из коллекции.
                    typeEdit = null;
                    break;
            }


        }

        private void imageBox_MouseClick(object sender, MouseEventArgs e)
        {
            try {
                switch (typeEdit)
                {
                    case "drawText":
                        timer1.Enabled = false;
                        listGObject.Add(drawText(e.X, e.Y, stf.text, blackPen ));
                        drawCollection(listGObject, blackPen);
                       // typeEdit = null;
                        break;


                    case "drawRetangle":
                        timer1.Enabled = false;
                        g.Clear(foneColor);
                        listGObject.Add(drawRetangle(e.X, e.Y, blackPen));
                        drawCollection(listGObject, blackPen); // рисуем фигуры

                        typeEdit = null; // фигура поставлена
                        break;

                    case "edit":
                        if (checkBox1.Checked) deleteGraphicsPath(searchGpaphicsPath(listGObject, e.X, e.Y));
                        else
                        {
                            gObject = searchGpaphicsPath(listGObject, e.X, e.Y);
                            listGObject.Remove(gObject);
                            lenght = gObject.lenght();
                            height = gObject.height();
                            stf.text = gObject.text;
                            typeEdit = "editMove";
                            timer1.Enabled = true;
                        }
                        break;

                    case "editMove":
                        firstStart = true;
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
                                typeEdit = "edit";
                                timerStop = true;
                                break;

                            case "retangle":
                                gObject.setRetangle(bx, by, gObject.x2, gObject.y2);
                                g.DrawPath(clearPen, gObject.gPath);
                                gObject.setRetangle(e.X, e.Y, gObject.x2, gObject.y2);
                                bx = e.X;
                                by = e.Y;
                                g.DrawPath(blackPen, gObject.gPath);
                                listGObject.Add(gObject);
                                typeEdit = "edit";
                                timerStop = true;
                                break;

                            case "text":
                                drawText(bx, by, stf.text, clearPen);
                               listGObject.Add(drawText(e.X, e.Y, stf.text, clearPen));                              
                                typeEdit = "edit";
                                timerStop = true;
                                break;
                            case "lineAndText":
                                drawLineAndText(bx, by, bx + lenght, by + height, stf.text, clearPen);
                                listGObject.Add(drawLineAndText(e.X, e.Y, x1 + lenght, by + height, stf.text, blackPen));
                                typeEdit = "edit";
                                timerStop = true;
                                break;

                        }
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
                    case "drawLineAndTextMove":
                        //bool firstS = true;
                        //bool horizontally = false;
                        //bool vertically = false;

                        //if (firstS)
                        //{
                        //    if (e.X > x1 + 20)
                        //    { horizontally = true; firstStart = false; }
                        //    if (e.X < x1 - 20)
                        //    {
                        //        horizontally = true; firstStart = false;
                        //    }
                        //    if (e.Y > y1 + 20 || e.Y < y1 - 20)
                        //    { vertically = true; firstStart = false; }
                        //}

                        //else
                        {
                            //if (horizontally)
                            {
                                drawLineAndText(x1, y1, bx, y1, stf.text, clearPen);
                                drawLineAndText(x1, y1, e.X, y1, stf.text, blackPen);
                                bx = e.X;
                            }
                            //if (vertically)
                            //{
                            //    drawLineAndText(x1, y1, x1, by, text, clearPen);
                            //    drawLineAndText(x1, y1, x1, e.Y, text, blackPen);
                            //    by = e.Y;
                            //}
                        }
                        break;

                    case "drawLineMove":
                        drawLine(x1, y1, bx, by, clearPen);
                        drawLine(x1, y1, e.X, e.Y, blackPen);
                        bx = e.X;
                        by = e.Y;
                        drawCollection(listGObject, blackPen); // нарисовать элементы из коллекции.
                        break;

                    case "drawRetangle":
                        drawRetangle(bx, by, clearPen);
                        drawRetangle(e.X, e.Y, blackPen);
                        bx = e.X;
                        by = e.Y;
                        break;

                    case "editMove":

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
                            case "text":
                                gObject.x1 = bx; gObject.y1 = by;
                                gObject.Create();
                                g.DrawPath(clearPen, gObject.gPath);
                                gObject.x1 = bx = e.X; gObject.y1 = by = e.Y;
                                gObject.Create();
                                g.DrawPath(blackPen, gObject.gPath);
                                break;

                            case "lineAndText":
                                drawLineAndText(bx, by, bx + lenght , by + height, stf.text, clearPen);
                                drawLineAndText(e.X, e.Y, x1 + lenght, by + height, stf.text, blackPen);
                                bx = e.X; by = e.Y;
                                
                                break;
                        }

                        break;
                }

                if (firstStart)
                {
                    try
                    {
                        g.Clear(foneColor);
                        firstStart = false;
                    }
                    catch (System.ArgumentException) { MessageBox.Show("Проблемы с первичной очистной"); }
                }



                //if (typeEdit == "drawLineMove")
                //{
                //    drawLine(x1, y1, bx, by, clearPen);
                //    drawLine(x1, y1, e.X, e.Y, blackPen);
                //    bx = e.X;
                //    by = e.Y;
                //    drawCollection(listGObject, blackPen); // нарисовать элементы из коллекции.
                //}
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
                        binarySave(svd.FileName, typeModel, Author, Group, Lastedit);
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
            opf.Title = "Выберите файл проекта для загрузки";
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

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            this.imagePanel.Top = -this.vScrollBar1.Value;

      
            this.Text = "x = " + this.panel1.Location.X + ", y = " + this.panel1.Location.Y;
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            this.imagePanel.Left = -this.hScrollBar1.Value;
            // Display the current values in the title bar.
            this.Text = "x = " + this.panel1.Location.X + ", y = " + this.panel1.Location.Y;
        }

        private void binarySave(string fileName, string mType, string Author, string Group, string LastEdit) // сохранение модели
        {
            
            FileStream myStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter BW = new BinaryWriter(myStream);

            BW.Write(mType); // тип модели
            BW.Write(Author); // автор (текущий пользователь)
            BW.Write(Group); // номер его группы, например ИТ1301
            BW.Write(LastEdit); // дата последнего изменения проекта
            foreach (GraphicsObject go in listGObject)
            {
                BW.Write(go.type);
                BW.Write(go.text);
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
            label2.Text = BR.ReadString();
            label3.Text = BR.ReadString();
            label4.Text = BR.ReadString();
            while(BR.PeekChar() != - 1)
            {
                sgo = new GraphicsObject();
                sgo.type = BR.ReadString();
                sgo.text = BR.ReadString();
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
          //  MessageBox.Show(typeModel);
            return typeModel;

        }




        #endregion

    }
}
