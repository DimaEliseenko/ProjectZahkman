using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace zmodule
{
    class GraphicsObject
    {
        
        public string type = "Object";
        public string text = "good job!";
        public int x1, y1, x2, y2;
        //public int zoneX1 = -20 , zoneY1 = - 20, zoneX11 = - 20, zoneY11 = - 20;
        public GraphicsPath gPath;
        GraphicsPath zone;
        
        public GraphicsObject() { }

       public GraphicsObject (String type)
        {
            this.type = type;
        }
        public GraphicsObject(String type, int X1, int Y1, int X2, int Y2)
        {
            this.type = type;
            x1 = X1; x2 = X2; y1 = Y1; y2 = Y2;
        }

        public void setValues(int X1, int Y1, int X2, int Y2)
        {
            x1 = X1; x2 = X2; y1 = Y1; y2 = Y2;
        }

        public bool setLine(int X1, int Y1, int X2, int Y2)
        {
            if (type == "line")
            {
                x1 = X1; y1 = Y1;
                x2 = X2; y2 = Y2;
                
               // zoneX1 = X1 - 10; zoneY1 = Y1 - 10;
               // zoneX11 = X1 + 10; zoneY11 = Y1 + 10;
                gPath = new GraphicsPath();
                gPath.Reset(); //очистим коллекцию граф. объектов
                gPath.StartFigure();
                zone = new GraphicsPath();
                zone.Reset();
                zone.StartFigure();
                //создаем линию
                gPath.AddLine(x1, y1, x2, y2);
                
               zone.AddEllipse(x1-5, y1-5, 10, 10);
            //   zone.AddEllipse(x2-3, y2-3, 6, 6);
                gPath.CloseFigure();
                zone.CloseFigure();
                return true;
            }
            else return false;
        } 

        public bool setRetangle(int X, int Y, int lenght, int height)
        {
            if (type=="retangle")
            {
                x1 = X; y1 = Y;
                x2 = lenght; y2 = height;
                gPath = new GraphicsPath();
                gPath.Reset(); //очистим коллекцию граф. объектов
                gPath.StartFigure();
                //создаем прямоугольник
                Rectangle ret = new Rectangle(x1,y1,x2,y2);
                gPath.AddRectangle(ret);
                gPath.CloseFigure();
                return true;
            }
            else return false;
        }

        public bool Create()
        {
            
            switch (type)
            {
                case "retangle":
                    gPath = new GraphicsPath();
                    gPath.Reset(); //очистим коллекцию граф. объектов
                    gPath.StartFigure();
                    //создаем прямоугольник
                    Rectangle ret = new Rectangle(x1, y1, x2, y2);
                    gPath.AddRectangle(ret);
                    gPath.CloseFigure();
                    return true;

                case "line":
                    gPath = new GraphicsPath();
                    gPath.Reset(); //очистим коллекцию граф. объектов
                    gPath.StartFigure();
                    zone = new GraphicsPath();
                    zone.Reset();
                    zone.StartFigure();
                    //создаем линию
                    gPath.AddLine(x1, y1, x2, y2);

                    zone.AddEllipse(x1 - 6, y1 - 6, 12, 12);
                   // zone.AddEllipse(x2 - 3, y2 - 3, 6, 6);
                    gPath.CloseFigure();
                    zone.CloseFigure();
                    return true;
                case "text":
                    gPath = new GraphicsPath();
                    gPath.Reset(); //очистим коллекцию граф. объектов
                    gPath.StartFigure();
                    Point p1 = new Point(x1, y1);
                    gPath.AddString(text, FontFamily.GenericSerif, 50, 50, p1, StringFormat.GenericDefault);
                    gPath.CloseFigure();
                    return true;
                    
                case "lineAndText":
                    try {
                        gPath = new GraphicsPath();
                        gPath.Reset(); //очистим коллекцию граф. объектов
                        gPath.StartFigure();
                        zone = new GraphicsPath();
                        zone.Reset();
                        zone.StartFigure();
                        gPath.AddLine(x1, y1, x2, y2);
                        Point p2 = new Point(x1 + lenght() / 2, y1);
                        gPath.AddString(text, FontFamily.GenericSerif, 50, 50, p2, StringFormat.GenericDefault);
                        Rectangle rct = new Rectangle(x1 - 5, y1 - 5, lenght() + 5, height() + 5);
                        zone.AddRectangle(rct);
                        // zone.AddEllipse(x1 - 6, y1 - 6, 12, 12);
                        //zone.AddEllipse(x2 - 3, y2 - 3, 6, 6);
                        gPath.CloseFigure();
                        zone.CloseFigure();
                        return true;
                    }catch(System.NullReferenceException) { return false; };

                default:
                   return false;
            }

        
          
        }

        public bool isVisiblePath(int x, int y)
        {
            switch (type)
            {
                case "lineAndText":
                    if (gPath.IsVisible(x, y)) return true;  
                      if  (zone.IsVisible(x, y)) return true;
                    else return false;

                case "retangle":
                case "text":
                    if (gPath.IsVisible(x, y)) return true;
                    else return false;

                case "line":
                    if (zone.IsVisible(x, y)) return true;
                    else return false;
                //case "text":
                //    if (gPath.IsVisible(x, y)) return true;
                //    else return false;
                    
                default:
                    return false;

            }
    
        }



        public int lenght()
        {
            // return Math.Abs(x1 - x2);
            return x2 - x1;
        }
        public int height()
        {
            // return Math.Abs(y1 - y2);
            return y2 - y1;
        }



        //public bool isVisible (int x, int y)
        //{

        //    if (x >= zoneX1 && 
        //        x <= zoneX11 && 
        //        y >= zoneY1 && 
        //        y <= zoneY11)
        //       return true;
        //        else return false;
        //}

    }
}
