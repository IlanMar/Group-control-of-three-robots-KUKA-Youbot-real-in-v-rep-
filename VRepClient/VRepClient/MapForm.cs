using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;


namespace VRepClient
{
    public partial class MapForm : Form
    {
        public MapForm()
        {
            
            InitializeComponent();            
       
        }
        List<Point> RobotsPoint1 = new List<Point>();//списки чтобы рисовать траекторию движения робота
        List<Point> RobotsPoint2 = new List<Point>();
        List<Point> RobotsPoint3 = new List<Point>();
        protected override void OnPaint(PaintEventArgs e)
        {
           
            /*
            base.OnPaint(e);

            if (Form1.f1.RobDrive != null & Form1.f1.ra != null)
            {
              
                for (int i = 0; i < Form1.f1.map.GlobalMapList.Count; i++)
                {
                    int g = 50;//изменить для смена масштаба
                    //Y умножается на -1 чтобы изначально карта смотрела вертикально
                    e.Graphics.DrawLine(new Pen(Color.Black), Form1.f1.map.GlobalMapList[i].X * g + 250, Form1.f1.map.GlobalMapList[i].Y * (-1) * g + 300, Form1.f1.map.GlobalMapList[i].X * g + 251, Form1.f1.map.GlobalMapList[i].Y * (-1) * g + 301);
                   
                    e.Graphics.DrawImage(Form1.f1.Rob, Form1.f1.map.RobOdData[0]*g + 240, Form1.f1.map.RobOdData[1]*g*(-1) + 290);
                    //вызов функции ниже должен быть не здесь а в классе ScreachonGraph                
                    

                    int f = 0;             
                }

           }
              */
            if (Form1.f1.ra != null && Form1.f1.SQ != null &&  Form1.f1.map != null && Form1.f1.map.graph != null)
            {
                int H2 = 5;// (int)(pictureBox1.Height / map.Ymax);
                int W2 = 5; //(int)(pictureBox1.Width / map.Xmax);
              //  Point start = new Point((int)(Form1.f1.ra.RobotOdomData[0] * 10 + Form1.f1.map.Xmax / 2), (int)(Form1.f1.ra.RobotOdomData[1] * 10 + Form1.f1.map.Ymax / 2));//отрисовки местоположений роботов
                //e.Graphics.FillEllipse(Green, (int)start.X * W2 - 2 * W2 - 10, pictureBox1.Height + ((-1) * (int)start.Y) * H2 - 2 * W2 - 10, 20, 20);
//
             //   Point start2 = new Point((int)(Form1.f1.ra2.RobotOdomData[0] * 10 + Form1.f1.map.Xmax / 2), (int)(Form1.f1.ra2.RobotOdomData[1] * 10 + Form1.f1.map.Ymax / 2));
               // e.Graphics.FillEllipse(Yellow, (int)start2.X * W2 - 2 * W2 - 10, pictureBox1.Height + ((-1) * (int)start2.Y) * H2 - 2 * W2 - 10, 20, 20);

              //  Point start3 = new Point((int)(Form1.f1.ra3.RobotOdomData[0] * 10 + Form1.f1.map.Xmax / 2), (int)(Form1.f1.ra3.RobotOdomData[1] * 10 + Form1.f1.map.Ymax / 2));
               // e.Graphics.FillEllipse(Red, (int)start3.X * W2 - 2 * W2 - 10, pictureBox1.Height + ((-1) * (int)start3.Y) * H2 - 2 * W2 - 10, 20, 20);

              //  Brush Green = new SolidBrush(Color.Green);
                //Brush Yellow = new SolidBrush(Color.Yellow);
               // Brush Red = new SolidBrush(Color.Red);

                Pen  GreenPen = new Pen(Color.Green, 2);
                Pen YellowPen = new Pen(Color.Yellow, 2);
                Pen RedPen = new Pen(Color.Red, 2);

                SolidBrush greenBrush = new SolidBrush(Color.Green);
          //      RobotsPoint1.Add(new Point((int)start.X * W2 - 2 * W2, pictureBox1.Height + ((-1) * (int)start.Y) * H2 - 2 * W2));//задаю точку где был робот
           //     RobotsPoint2.Add(new Point((int)start2.X * W2 - 2 * W2, pictureBox1.Height + ((-1) * (int)start2.Y) * H2 - 2 * W2));//задаю точку где был робот
           //     RobotsPoint3.Add(new Point((int)start3.X * W2 - 2 * W2, pictureBox1.Height + ((-1) * (int)start3.Y) * H2 - 2 * W2));//задаю точку где был робот
                if (checkBox1.Checked)
                {
                    for (int i = 5; i < Form1.f1.RobotsPoint1.Count - 17; i++)
                    {
                        Pen blackPen = new Pen(Color.Green, 2);
                        e.Graphics.DrawLine(blackPen, (int)Form1.f1.RobotsPoint1[i].X, (int)Form1.f1.RobotsPoint1[i].Y, (int)Form1.f1.RobotsPoint1[i + 17].X, (int)Form1.f1.RobotsPoint1[i + 17].Y);
                        e.Graphics.DrawLine(YellowPen, (int)Form1.f1.RobotsPoint2[i].X, (int)Form1.f1.RobotsPoint2[i].Y, (int)Form1.f1.RobotsPoint2[i + 17].X, (int)Form1.f1.RobotsPoint2[i + 17].Y );
                        e.Graphics.DrawLine(RedPen, (int)Form1.f1.RobotsPoint3[i].X, (int)Form1.f1.RobotsPoint3[i].Y, (int)Form1.f1.RobotsPoint3[i + 17].X, (int)Form1.f1.RobotsPoint3[i + 17].Y);
                        i = i + 16;
                    }
                }
            }
        }

        private void DrawLine(Pen pen, Point point1, Point point2)
        {
         
        }
        private void MapForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          

        }
    }
}
