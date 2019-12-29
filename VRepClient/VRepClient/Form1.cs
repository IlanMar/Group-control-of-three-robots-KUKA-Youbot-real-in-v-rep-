//Margolin Ilan. Kuka Youbot Controller. 2016
using System;
using System.Drawing;
using System.Windows.Forms;
using VRepAdapter;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace VRepClient
{
    public partial class Form1 : Form

    {
        public MapForm f2 = new MapForm();
        public Form1()
        {
            InitializeComponent();
            f1 = this;
            
            //f2.Show();


            int MapWidth =180;
            int MapHeight = 180;
            int CellSize = 4;
            // Размер карты в пискелях.
            int mapWidthPxls = MapWidth * (CellSize + 1) + 1,
                mapHeightPxls = MapHeight * (CellSize + 1) + 1;
            Bitmap mapImg = new Bitmap(mapWidthPxls, mapHeightPxls);
            Graphics g = Graphics.FromImage(mapImg);

            // Заливаем весь битмап:
            g.Clear(Color.White);

            // Рисуем сетку:
            for (int x = 0; x <= MapWidth; x++)
                g.DrawLine(Pens.LightGray, x * (CellSize + 1), 0, x * (CellSize + 1), mapHeightPxls);
            for (int y = 0; y <= MapHeight; y++)
                g.DrawLine(Pens.LightGray, 0, y * (CellSize + 1), mapWidthPxls, y * (CellSize + 1));
            PictureBox p = pictureBox1;
            if (p.Image != null)
                p.Image.Dispose();

            pictureBox1.Image = mapImg;
            g.Dispose();

      
            
        }
            
        public RobotAdapter ra,ra2,ra3; //экземпляр класса ra - robot adapter

        public Drive RobDrive, RobDrive2, RobDrive3;
        public Drive GetRobDrive(int ind)
        {
            if (ind == 0) return RobDrive;
            if (ind == 1) return RobDrive2;
            if (ind == 2) return RobDrive3;

            return null;
        }


        public SequencePoints SQ, SQ2, SQ3;//объект класса sequencePoints
       public Map map,map2,map3;//объект класса Map
       public SearchInGraph SiG, SiG2, SiG3;//объект класса поиска по графу
        //все что в абзаце ниже, удалить
       // public tacticalLevel TactLevel = new tacticalLevel();
       /// <summary>
       /// 
       /// </summary>
 //public PotField PotFiel = new PotField();
    //    public KukaPotField KukaPotField = new KukaPotField();
      //  public int PotfieldButtonA = 0;//если кнопка нажате то методм PotField доступен
    //    public int KukaPotButtonB = 0;//если кнопка нажата то работает метод кука.
      //  public Bitmap Rob = new Bitmap(@"C:\Users\Илан\Pictures\Robot.jpg");
        public List<Point> ListPoints = new List<Point>();
        public List<Point> ListPoints2 = new List<Point>();
        public List<Point> ListPoints3 = new List<Point>();

        public List<Point> RobotsPoint1 = new List<Point>();//списки чтобы рисовать траекторию движения робота
        public List<Point> RobotsPoint2 = new List<Point>();
        public List<Point> RobotsPoint3 = new List<Point>();

        /*enum ErrorCodes
        {
            simx_error_noerror = 0x000000,
            simx_error_novalue_flag = 0x000001,		// input buffer doesn't contain the specified command 
            simx_error_timeout_flag = 0x000002,		//command reply not received in time for simx_opmode_oneshot_wait operation mode 
            simx_error_illegal_opmode_flag = 0x000004,		//command doesn't support the specified operation mode
            simx_error_remote_error_flag = 0x000008,		// command caused an error on the server side 
            simx_error_split_progress_flag = 0x000010,		// previous similar command not yet fully processed (applies to simx_opmode_oneshot_split operation modes) 
            simx_error_local_error_flag = 0x000020,		// command caused an error on the client side //
            simx_error_initialize_error_flag = 0x000040		// simxStart was not yet called //
        };*/
        
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < robots.Length; i++) //2019-01-30
            {
                if(robots[i]!=null) robots[i].Init(i+1);
            }
        }
      
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void VrepAdapterButton_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(tb_ip.Text)) ra = new VrepAdapter();
            if (!string.IsNullOrWhiteSpace(tb_ip2.Text)) ra2 = new VrepAdapter();
            if (!string.IsNullOrWhiteSpace(tb_ip3.Text)) ra3 = new VrepAdapter();

            robots = new VRepClient.RobotAdapter[] { ra, ra2, ra3 }; //2019-01-30

        }
        public RobotAdapter[] robots;
        string get_tb_ip(int ind)
        {
            if (ind == 0) return tb_ip.Text;
            if (ind == 1) return tb_ip2.Text;
            if (ind == 2) return tb_ip3.Text;
            return null;
        }
        RobPos get_rob_pos(int ind)
        {
            float[] rod = null;

            var ra = robots[ind];
            if (ra == null) return null;
            rod = ra.RobotOdomData;
            return new RobPos { RobX = ra.RobotOdomData[0], RobY = ra.RobotOdomData[1], RobA = ra.RobotOdomData[2] };

        }


        public class RobInfo
        {
            public string ip;
            public float x, y;
        }

        public List<RobInfo> rob_infos=new List<RobInfo>();//2019-02-01
        private void YoubotAdapter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tb_ip.Text)) ra = new YoubotAdapter();
            if (!string.IsNullOrWhiteSpace(tb_ip2.Text)) ra2 = new YoubotAdapter();
            if (!string.IsNullOrWhiteSpace(tb_ip3.Text)) ra3 = new YoubotAdapter();
            
            robots = new VRepClient.RobotAdapter[] { ra, ra2, ra3 }; //2019-01-30

            if(ra!=null) rob_infos.Add(getRobInfo(tb_ip.Text, tb_shift1.Text));
            if(ra2!=null) rob_infos.Add(getRobInfo(tb_ip2.Text, tb_shift2.Text));
            if(ra3!=null) rob_infos.Add(getRobInfo(tb_ip3.Text, tb_shift3.Text));

           

            //initial_poses.Sort((x1, x2) => x1.x.CompareTo(x2.x));
        }

        RobInfo getRobInfo(string ip, string xy)
        {
            var res = new RobInfo();
            res.ip = ip;

            var arr = xy.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            res.x = float.Parse(arr[0], CultureInfo.InvariantCulture);
            res.y = float.Parse(arr[1], CultureInfo.InvariantCulture);
            return res;
        }

        public RobInfo findRobInfo(string ip)
        {
            for (int i = 0; i < rob_infos.Count; i++)
            {
                if (rob_infos[i].ip == ip) return rob_infos[i];
            }
            return null;
        }

        float[] fixOdomArray(RobotAdapter ra)
        {
            if (ra == null) return new float[] { 0, 0, 0 };
            else return ra.RobotOdomData;
        }
        float[] fixLedArray(RobotAdapter ra)
        {
            if (ra == null) return null;
            else return ra.RobotLedData;
        }
        Point CenterMass=Point.Empty;
        Point CenterMass0 = Point.Empty;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (robots == null) return;
                 //pictureBox1.Invalidate();


                 //   ra.RobotOdomData[0] = ra.RobotOdomData[0] + 1.8f;//заплатка вместо камер
                 //   ra3.RobotOdomData[0] = ra3.RobotOdomData[0] - 1.8f;

                 PaintEventArgs p = new PaintEventArgs(pictureBox1.CreateGraphics(), pictureBox1.Bounds); //Компонент на котором нужно рисовать и область на которой нужно рисовать
            pictureBox1_Paint(sender, p);
           // pictureBox1_Paint();
                 // this.Invalidate();
            f2.Invalidate();

            bool rob_drive_ok = false;

            for (int i = 0; i < robots.Length; i++)
            {
                var ra = robots[i]; if (ra == null) continue;
                if (ra is VrepAdapter)
                {

                    var suffix = "";
                    if (i == 1) suffix = "2";
                    if (i == 2) suffix = "3";

                    var vrep = ra as VrepAdapter;
                    float Xdelta = 0, Ydelta = 0;
                    string Lidar = VRepFunctions.GetStringSignal(VrepAdapter.clientID, "Lidar"+ suffix);//str-данные с ледара
                    string RobPos = VRepFunctions.GetStringSignal(VrepAdapter.clientID, "RobPos"+ suffix);//получение координат робота на сцене Врепа
                    vrep.ReceiveLedData(Lidar);
                    vrep.ReceiveOdomData(RobPos, Xdelta, Ydelta);
                }

                var rd = GetRobDrive(i);
                if (rd != null) rob_drive_ok = true;
                ra.Send(rd);
            }

            if (rob_drive_ok && robots != null && SQ != null)//отправка одометрии в экземпляр класса drive
            {

                map.GlobListToGraph(map.GlobalMapList, map.RobOdData);
                float GoalPointX = Convert.ToSingle(textBox8.Text);
                float GoalPointY = Convert.ToSingle(textBox9.Text);
                float GoalPointX2 = Convert.ToSingle(textBox11.Text);
                float GoalPointY2 = Convert.ToSingle(textBox10.Text);
                float GoalPointX3 = Convert.ToSingle(textBox13.Text);
                float GoalPointY3 = Convert.ToSingle(textBox12.Text);

                Point goal = new Point((int)GoalPointX * 10 + map.Xmax / 2, (int)GoalPointY * 10 + map.Ymax / 2);
                Point goal2 = new Point((int)GoalPointX2 * 10 + map.Xmax / 2, (int)GoalPointY2 * 10 + map.Ymax / 2);
                Point goal3 = new Point((int)GoalPointX3 * 10 + map.Xmax / 2, (int)GoalPointY3 * 10 + map.Ymax / 2);

                Point start=Point.Empty, start2 = Point.Empty, start3 = Point.Empty;
                if(ra!=null) start = new Point((int)(ra.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra.RobotOdomData[1] * 10 + map.Ymax / 2));
                if (ra2 != null) start2 = new Point((int)(ra2.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra2.RobotOdomData[1] * 10 + map.Ymax / 2));
                if (ra3!= null) start3 = new Point((int)(ra3.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra3.RobotOdomData[1] * 10 + map.Ymax / 2));

                //2019-01-30
                float[] XXX = new[] { GoalPointX, GoalPointX2, GoalPointX3 };
                float[] YYY = new[] { GoalPointY, GoalPointY2, GoalPointY3 };
                Point[] GOALS = new[] { goal, goal2, goal3 };
                Point[] STARTS = new[] { start, start2, start3 };


                CenterMass = new Point();

                int cnt_robots = 0;
                for (int i = 0; i < robots.Length; i++)
                {
                    var ra = robots[i]; if (ra == null) continue;

                    CenterMass.X += STARTS[i].X;
                    CenterMass.Y += STARTS[i].Y;
                    cnt_robots++;
                }

                CenterMass.X = (int)(CenterMass.X / (float)cnt_robots);
                CenterMass.Y = (int)(CenterMass.Y / (float)cnt_robots);
                //  Point goal2 = new Point(50, 50);
                //  Point goal3 = new Point(150, 130);
                // ListPoints = null;

                for (int x = -3; x < 4; x++)//попытка сделать проезжаемыми приепятствия для точки отсчета пути
                {
                    for (int y = -3; y < 4; y++)
                    {
                        if (map.graph[CenterMass.X + x, CenterMass.Y + y, 1] > 0)
                        {
                            map.graph[CenterMass.X + x, CenterMass.Y + y, 0] = 1;
                            map.graph[CenterMass.X + x, CenterMass.Y + y, 1] = 0;
                        }
                        if (map.graph[CenterMass.X + x, CenterMass.Y + y, 0] > 1)
                        {
                            map.graph[CenterMass.X + x, CenterMass.Y + y, 0] = 1;
                            map.graph[CenterMass.X + x, CenterMass.Y + y, 1] = 0;
                        }

                    }
                }

                if(SiG!=null) ListPoints = SiG.FindPath(map.graph, CenterMass, goal); //SearchInGraph.FindPath(map.graph, start, goal);
                if (SiG2 != null) ListPoints2 = SiG2.FindPath(map.graph, CenterMass, goal2);
                if (SiG3 != null) ListPoints3 = SiG3.FindPath(map.graph, CenterMass, goal3);
                var LPs = new[] { ListPoints, ListPoints2, ListPoints3 };


#warning непонятно что если ra3==null
                map.LedDataToList(fixLedArray(ra), fixOdomArray(ra), fixOdomArray(ra2), fixOdomArray(ra3));

#warning непонятно
                if (RobDrive != null)
                {
                    //  textBox6.Text = map.Phi.ToString();
                    //   textBox6.Invalidate();
                    // textBox3.Text = map.Phi2.ToString();
                    // textBox3.Invalidate();
                    textBox4.Text = map.Phi3.ToString();
                    textBox4.Invalidate();
                }

                map.LedDataToList(fixLedArray(ra2), fixOdomArray(ra2), fixOdomArray(ra), fixOdomArray(ra3));
                map.LedDataToList(fixLedArray(ra3), fixOdomArray(ra3), fixOdomArray(ra), fixOdomArray(ra2));

                if (ListPoints != null)//2019-01-31
                {
                    RobPos[] RPs = new RobPos[] { get_rob_pos(0), get_rob_pos(1), get_rob_pos(2) };
                    for (int i = 0; i < robots.Length; i++)
                    {
                        var ra = robots[i]; if (ra == null) continue;
                        var SQ = SQs[i];

                        if (i == 0)
                        {
                            SQ.GetNextPoint(LPs[i], CenterMass, map.Xmax, map.Ymax);// ra.RobotOdomData[0], ra.RobotOdomData[1], ra.RobotOdomData[2], map.Xmax, map.Ymax);
                            RobDrive.GetDrive(get_rob_pos(0), get_rob_pos(1), get_rob_pos(2), SQ.CurrentPointX, SQ.CurrentPointY, map.Xmax, map.Ymax, CenterMass, ra.RobotLedData, map.graph);
                            ra.Send(RobDrive);
                        }

                        if (i == 1)
                        {
                            SQ2.GetNextPoint(ListPoints2, CenterMass, map.Xmax, map.Ymax);//ra2.RobotOdomData[0], ra2.RobotOdomData[1], ra2.RobotOdomData[2], map.Xmax, map.Ymax);
                            RobDrive2.GetDrive(get_rob_pos(1), get_rob_pos(0), get_rob_pos(2), SQ2.CurrentPointX, SQ2.CurrentPointY, map.Xmax, map.Ymax, CenterMass, ra2.RobotLedData, map.graph);
                            ra2.Send(RobDrive2);
                        }

                        if (i == 2)
                        {
                            SQ3.GetNextPoint(ListPoints3, CenterMass, map.Xmax, map.Ymax);// ra3.RobotOdomData[0], ra3.RobotOdomData[1], ra3.RobotOdomData[2], map.Xmax, map.Ymax);
                            RobDrive3.GetDrive(get_rob_pos(2), get_rob_pos(0), get_rob_pos(1), SQ3.CurrentPointX, SQ3.CurrentPointY, map.Xmax, map.Ymax, CenterMass, ra3.RobotLedData, map.graph);
                            ra3.Send(RobDrive3);
                        }
                    }

                }
            }

            if (ra != null & RobDrive != null)//вывод переменных из Робот Адаптера на форму
         {
             string OutLedData="";
             string OutOdomData = "";
             for (int i = 0; i < ra.RobotLedData.Length; i++) 
             {
                 OutLedData = OutLedData + ra.RobotLedData[i]+"; ";
             }
             for (int i = 0; i < ra.RobotOdomData.Length; i++)
             {
                 OutOdomData = OutOdomData + ra.RobotOdomData[i]+"; ";
             }

            // richTextBox1.Text = OutLedData;//закоменчен вывод данных одометрии
             richTextBox2.Text = OutOdomData;
             if (RobDrive != null)
             {
                    if (RobDrive2 != null)
                    {
                        textBox3.Text = RobDrive2.Phi2.ToString();
                        textBox3.Invalidate();
                        //  textBox4.Text = map.Phi3.ToString();
                        //  textBox4.Invalidate();
                        textBox5.Text = RobDrive2.DistToTarget3.ToString();
                        textBox5.Invalidate();
                        textBox6.Text = RobDrive2.Phi.ToString();
                        textBox6.Invalidate();
                        textBox7.Text = RobDrive2.DistToTarget2.ToString();
                        textBox7.Invalidate();
                        textBox20.Text = RobDrive2.ResultPhi.ToString();
                        textBox20.Invalidate();
                    }
                    if (RobDrive != null)
                    {
                        textBox14.Text = RobDrive.Phi2.ToString();
                        textBox14.Invalidate();
                        textBox15.Text = RobDrive.Phi3.ToString();
                        textBox15.Invalidate();
                        textBox16.Text = RobDrive.Phi.ToString();
                        textBox16.Invalidate();
                        textBox21.Text = RobDrive.ResultPhi.ToString();
                        textBox21.Invalidate();
                    }
                    if (RobDrive3 != null)
                    {
                        textBox2.Text = RobDrive3.Phi.ToString();
                        textBox2.Invalidate();

                        textBox17.Text = RobDrive3.Phi2.ToString();
                        textBox17.Invalidate();
                        textBox18.Text = RobDrive3.Phi3.ToString();
                        textBox18.Invalidate();
                        textBox1.Text = RobDrive3.ResultPhi.ToString();
                        textBox1.Invalidate();
                        textBox19.Text = RobDrive3.DistToTarget2.ToString();
                        textBox19.Invalidate();
                    }
             }
         }
            
           
            
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ra != null)
            {
                ra.Deactivate();
              //  ra2.Deactivate();
              //  ra3.Deactivate();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            //PotfieldButtonA = 1;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static Form1 f1;

        private void bt_tcp_test_Click(object sender, EventArgs e)
        {//2019-01-30
            bool ok = false;
            for (int i = 0; i < robots.Length; i++)
            {
                var ra = robots[i];
                if(ra!=null)
                {
                    var ya = ra as YoubotAdapter;
                    var str = get_tb_ip(i);
                    ya.TCPconnect(str, "7777", true);

                    ok = true;
                }
            }
            
            if(!ok) { MessageBox.Show("вы работаете с Врепом, а не с реальным роботом!"); }
        }

        public void ShowLedData(string s)
        {
            rtb_tcp.Invoke(new Action(() => rtb_tcp.Text = s));
        }
        public void ShowOdomData(string s)
        {
            rtb_tcp2.Invoke(new Action(() => rtb_tcp2.Text = s));
        }

        private void btsend_Click(object sender, EventArgs e)
        {
       //     if (tc == null) 
        //    {

        //        return;
       //     }
       //     tc.Send(rtb_send.Text); 
        }

        private void rtb_send_TextChanged(object sender, EventArgs e)
        {

        }

        private void KukaPotButton_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// /////////////тут была инициализация объектов класса ра для куки и врепа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        SequencePoints[] SQs;
        private void Drive_Click(object sender, EventArgs e)
        {
            if (ra != null)
            {
                SQ = new SequencePoints();
                RobDrive = new Drive();
                map = new Map();
                SiG = new SearchInGraph();
            }

            if (ra2 != null)
            {
                SQ2 = new SequencePoints();
                RobDrive2 = new Drive();
                map2 = new Map();
                SiG2 = new SearchInGraph();
            }

            if (ra3 != null)
            {
                SQ3 = new SequencePoints();
                RobDrive3 = new Drive();
                map3 = new Map();
                SiG3 = new SearchInGraph();
            }


            SQs = new SequencePoints[] { SQ, SQ2, SQ3 };

            //2019-02-01
            for (int i = 0; i < robots.Length; i++)
            {
                var ra = robots[i]; if (ra == null) continue;
                var ya = ra as YoubotAdapter;
                if (ya == null) continue;

                var info = rob_infos[i];

                ya.tc.Send(string.Format(CultureInfo.InvariantCulture, "LUA_UpdateScreenPose({0}, {1}, {2}, 1)", 0,0,0));
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void rtb_tcp_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (ra != null && SQ != null && Drive != null && map != null && map.graph != null)
         {          

            int yy = 0;
            int xx=0;
          //  this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
          //  if (10==timer1.Tick) { }               
               /*
              
                    for (int l = 0; l < map.Ymax + 1; l++)//отрисовываем сетку
                    {
                        e.Graphics.DrawLine(new Pen(Color.Black), 0, yy, pictureBox1.Width, yy);
                        // xx = xx + 50;
                        yy = yy + pictureBox1.Height / map.Ymax;
                    }
                    for (int l = 0; l < map.Xmax + 1; l++)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Black), xx, 0, xx, pictureBox1.Height);
                        xx = xx + pictureBox1.Width / map.Xmax;
                    }
                    System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
             
           */
            
                int MapWidth = map.Xmax;
                int MapHeight = map.Ymax;
                int CellSize = 4;
                // Размер карты в пискелях.
                int mapWidthPxls = MapWidth * (CellSize + 1) + 1,
                    mapHeightPxls = MapHeight * (CellSize + 1) + 1;
                Bitmap mapImg = new Bitmap(mapWidthPxls, mapHeightPxls);
                Graphics g = Graphics.FromImage(mapImg);

                // Заливаем весь битмап:
                g.Clear(Color.White);

                // Рисуем сетку:
                for (int x = 0; x <= MapWidth; x++)
                    g.DrawLine(Pens.LightGray, x * (CellSize + 1), 0, x * (CellSize + 1), mapHeightPxls);
                for (int y = 0; y <= MapHeight; y++)
                    g.DrawLine(Pens.LightGray, 0, y * (CellSize + 1), mapWidthPxls, y * (CellSize + 1));
                PictureBox p = pictureBox1;
                if (p.Image != null)
                    p.Image.Dispose();

                pictureBox1.Image = mapImg;
                g.Dispose();
            
            
         for (int i = 0; i < map.Xmax; i++) //закрашиваем ячейки с препядствиями
         {
             for (int k = 0; k < map.Ymax; k++)
             {
                 if (map.graph[i, k,0] ==2)
                 {
                     int H = CellSize+1;//(int)(pictureBox1.Height / map.Ymax);
                     int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);
                     
                     SolidBrush blueBrush = new SolidBrush(Color.Blue);                   
                     Rectangle rect = new Rectangle((i) * W, pictureBox1.Height + ((-1) * k * H) , W, H);                  
                     e.Graphics.FillRectangle(blueBrush, rect);                  
                 }

                 /*
                 if (map.graph[i, k, 1] == 1)//закрашиваем еденички, ячейки соседствующие со стеной
                 {
                     int H = CellSize + 1;//(int)(pictureBox1.Height / map.Ymax);
                     int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                     SolidBrush blueBrush = new SolidBrush(Color.Aquamarine);
                     Rectangle rect = new Rectangle((i) * W, pictureBox1.Height + ((-1) * k * H), W, H);
                     e.Graphics.FillRectangle(blueBrush, rect);
                 }

                 if (map.graph[i, k, 1] == 2)//закрашиваем еденички, ячейки соседствующие со стеной
                 {
                     int H = CellSize + 1;//(int)(pictureBox1.Height / map.Ymax);
                     int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                     SolidBrush blueBrush = new SolidBrush(Color.BlanchedAlmond);
                     Rectangle rect = new Rectangle((i) * W, pictureBox1.Height + ((-1) * k * H), W, H);
                     e.Graphics.FillRectangle(blueBrush, rect);
                 }

                 if (map.graph[i, k, 1] == 3)//закрашиваем еденички, ячейки соседствующие со стеной
                 {
                     int H = CellSize + 1;//(int)(pictureBox1.Height / map.Ymax);
                     int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                     SolidBrush blueBrush = new SolidBrush(Color.BlueViolet);
                     Rectangle rect = new Rectangle((i) * W, pictureBox1.Height + ((-1) * k * H), W, H);
                     e.Graphics.FillRectangle(blueBrush, rect);
                 }
              */
                 /*
                 if (map.graph[i, k] ==0 )//закрашиваем пустые ячейки прозрачным цветом
                 {
                     int H = (int)(pictureBox1.Height / map.Ymax);
                     int W = (int)(pictureBox1.Width / map.Xmax);
                    
                     Color brushColor = Color.FromArgb(250 / 100 * 0, 255, 0, 0);
                     SolidBrush blueBrush = new SolidBrush(brushColor);
                     // Create rectangle.//ниже путаница со знаками, по Иксу двигается а по У нет
                     Rectangle rect = new Rectangle((i) * W, pictureBox1.Height + ((-1) * k * H), W, H);

                     // Fill rectangle to screen.
                     e.Graphics.FillRectangle(blueBrush, rect);

                 }*/
                 
             }
         }
         if (ListPoints != null)//ресуем получившийся маршрут
         {
             for (int i = 0; i < ListPoints.Count; i++)
             {
                 int H =CellSize+1; //(int)(pictureBox1.Height / map.Ymax);
                 int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                 SolidBrush blueBrush = new SolidBrush(Color.Black);
                 SolidBrush greenBrush = new SolidBrush(Color.Green);
                 Rectangle rect = new Rectangle((ListPoints[i].X) * W, pictureBox1.Height + ((-1) * ListPoints[i].Y * H), W, H);
                 Rectangle rectCurrentPoint = new Rectangle(((int)SQ.CurrentPointX) * W, pictureBox1.Height + ((-1) * (int)SQ.CurrentPointY * H), W, H);
                 
                 e.Graphics.FillRectangle(blueBrush, rect);
                 e.Graphics.FillRectangle(greenBrush, rectCurrentPoint);
             }

         }
                /*
         if (ListPoints2 != null)//ресуем получившийся маршрут второго робота
         {
             for (int i = 0; i < ListPoints2.Count; i++)
             {
                 int H = CellSize + 1; //(int)(pictureBox1.Height / map.Ymax);
                 int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                 SolidBrush blueBrush = new SolidBrush(Color.Black);
                 SolidBrush greenBrush = new SolidBrush(Color.Green);
                 Rectangle rect = new Rectangle((ListPoints2[i].X) * W, pictureBox1.Height + ((-1) * ListPoints2[i].Y * H), W, H);
                 Rectangle rectCurrentPoint = new Rectangle(((int)SQ.CurrentPointX) * W, pictureBox1.Height + ((-1) * (int)SQ.CurrentPointY * H), W, H);

                 e.Graphics.FillRectangle(blueBrush, rect);
                 e.Graphics.FillRectangle(greenBrush, rectCurrentPoint);
             }

         }
         if (ListPoints3 != null)//ресуем получившийся маршрут третьего робота
         {
             for (int i = 0; i < ListPoints3.Count; i++)
             {
                 int H = CellSize + 1; //(int)(pictureBox1.Height / map.Ymax);
                 int W = CellSize + 1;//(int)(pictureBox1.Width / map.Xmax);

                 SolidBrush blueBrush = new SolidBrush(Color.Bisque);
                 SolidBrush greenBrush = new SolidBrush(Color.Green);
                 Rectangle rect = new Rectangle((ListPoints3[i].X) * W, pictureBox1.Height + ((-1) * ListPoints3[i].Y * H), W, H);
                 Rectangle rectCurrentPoint = new Rectangle(((int)SQ.CurrentPointX) * W, pictureBox1.Height + ((-1) * (int)SQ.CurrentPointY * H), W, H);

                 e.Graphics.FillRectangle(blueBrush, rect);
                 e.Graphics.FillRectangle(greenBrush, rectCurrentPoint);
             }

         }*/
         Brush Green = new SolidBrush(Color.Green);
         Brush Yellow = new SolidBrush(Color.Yellow);
        Brush Red = new SolidBrush(Color.Red);

         Pen GreenPen = new Pen(Color.Green, 2);
         Pen YellowPen = new Pen(Color.Yellow, 2);
         Pen RedPen = new Pen(Color.Red, 2);

         int H2 =CellSize+1;// (int)(pictureBox1.Height / map.Ymax);
         int W2 = CellSize + 1; //(int)(pictureBox1.Width / map.Xmax);

                Point start=Point.Empty, start2 = Point.Empty, start3 = Point.Empty;
                if (ra != null) //2019-02-01
                {
                    start = new Point((int)(ra.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra.RobotOdomData[1] * 10 + map.Ymax / 2));//отрисовки местоположений роботов
                    e.Graphics.FillEllipse(Green, (int)start.X * W2 - 2 * W2 - 0, pictureBox1.Height + ((-1) * (int)start.Y) * H2 - 2 * W2 - 0, 30, 30);
                    RobotsPoint1.Add(new Point((int)start.X * W2 - 2 * W2, pictureBox1.Height + ((-1) * (int)start.Y) * H2 - 2 * W2));//задаю точку где был робот

                }

                if (ra2 != null)
                {
                    start2 = new Point((int)(ra2.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra2.RobotOdomData[1] * 10 + map.Ymax / 2));
                    e.Graphics.FillEllipse(Yellow, (int)start2.X * W2 - 2 * W2 - 0, pictureBox1.Height + ((-1) * (int)start2.Y) * H2 - 2 * W2 - 0, 30, 30);
                    RobotsPoint2.Add(new Point((int)start2.X * W2 - 2 * W2, pictureBox1.Height + ((-1) * (int)start2.Y) * H2 - 2 * W2));//задаю точку где был робот

                }

                if (ra3 != null)
                {
                    start3 = new Point((int)(ra3.RobotOdomData[0] * 10 + map.Xmax / 2), (int)(ra3.RobotOdomData[1] * 10 + map.Ymax / 2));
                    e.Graphics.FillEllipse(Red, (int)start3.X * W2 - 2 * W2 - 0, pictureBox1.Height + ((-1) * (int)start3.Y) * H2 - 2 * W2 - 0, 30, 30);
                    RobotsPoint3.Add(new Point((int)start3.X * W2 - 2 * W2, pictureBox1.Height + ((-1) * (int)start3.Y) * H2 - 2 * W2));//задаю точку где был робот
                }

                //2019-02-01
                //draw goal
                {

                    if (CenterMass0 == Point.Empty) CenterMass0 = CenterMass;

                    float GoalPointX = Convert.ToSingle(textBox8.Text);
                    float GoalPointY = Convert.ToSingle(textBox9.Text);

                    Point goal = new Point((int)GoalPointX * 10 + map.Xmax / 2, (int)GoalPointY * 10 + map.Ymax / 2);


                    var x =  goal.X * W2;
                    var y = pictureBox1.Height - (goal.Y * H2);
                    e.Graphics.FillEllipse(Brushes.Violet, x, y, 6, 6);

                }


                if (checkBox1.Checked)
         {
             for (int i = 5; i < Form1.f1.RobotsPoint1.Count - 17; i++)
             {
                 Pen blackPen = new Pen(Color.Green, 2);
                 e.Graphics.DrawLine(blackPen, (int)Form1.f1.RobotsPoint1[i].X, (int)Form1.f1.RobotsPoint1[i].Y, (int)Form1.f1.RobotsPoint1[i + 17].X, (int)Form1.f1.RobotsPoint1[i + 17].Y);
                 e.Graphics.DrawLine(YellowPen, (int)Form1.f1.RobotsPoint2[i].X, (int)Form1.f1.RobotsPoint2[i].Y, (int)Form1.f1.RobotsPoint2[i + 17].X, (int)Form1.f1.RobotsPoint2[i + 17].Y);
                 e.Graphics.DrawLine(RedPen, (int)Form1.f1.RobotsPoint3[i].X, (int)Form1.f1.RobotsPoint3[i].Y, (int)Form1.f1.RobotsPoint3[i + 17].X, (int)Form1.f1.RobotsPoint3[i + 17].Y);
                 i = i + 16;
             }
         }

         
     }
          //  e.Graphics.Clear(Color.Teal);
           // e.Graphics.Clear();
            if (map != null && map.invalidateform == true)//обновляем форму
            {
               pictureBox1.Invalidate();//вызов отрисовки на пикчербоксе перенести в более логичное мето
            }

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
         

        }

        Point Point = new Point();
        int X, Y;

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
           
              //  Point = Control.MousePosition;
             // Point.X = Point.X - (X);
               // Point.Y = Point.Y - (Y);
              //  base.Location = Point;
            
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
              //  X = Control.MousePosition.X - base.Location.X;
              //  Y = Control.MousePosition.Y - base.Location.Y;
            }
        }
        void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (map != null&& map.graph!=null&&e.Button == MouseButtons.Left)
            {
                Point startpoint = e.Location;
                int mapWidthPxls = map.Xmax * (4 + 1)+1,
                    mapHeightPxls = map.Ymax * (4 + 1)+1  ;
                int Xmouse = startpoint.X;
                int Ymouse = mapHeightPxls-startpoint.Y+4;
               float NodeGraphX = (Xmouse / 5);
               float NodeGraphY = (Ymouse / 5);          
               map.GlobalMapList.Add(new ObstaclesPoint { X = (NodeGraphX  - map.Xmax / 2)/10, Y = (NodeGraphY - map.Xmax / 2)/10, weight = 2 });
            }
            if (e.Button == MouseButtons.Left) 
            {/*
                Point startpoint = e.Location;
                int mapWidthPxls = map.Xmax * (4 + 1) + 1,
                    mapHeightPxls = map.Ymax * (4 + 1) + 1;
                int Xmouse = startpoint.X;
                int Ymouse = mapHeightPxls - startpoint.Y + 4;
                float NodeGraphX = (Xmouse / 5);
                float NodeGraphY = (Ymouse / 5);  
                //map.GlobalMapList.Remove(  (int)(NodeGraphX - map.Xmax / 2) / 10,  (int)(NodeGraphY - map.Xmax / 2) / 10);
            */
              }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void tb_ip_TextChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void bt_restart_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

      public  YoubotAdapter yatccam;
        private void button3_Click(object sender, EventArgs e)
        {
            yatccam = new YoubotAdapter();//новая переменная для доступа к камере

            yatccam.TCPconnect("192.168.88.236", "8888", false);//"192.168.88.2", "8888", false);//отправляем запрос на сервер камер //"10.0.48.244"
            // 192.168.88.236 "10.0.48.49"
        }

        private void rtb_cam_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label21_Click_1(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void tb_ip2_TextChanged(object sender, EventArgs e)
        {

        }
        /*
        private void pictureBox1_MouseClick(Object sender, MouseEventArgs e)
        {

            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "Button", e.Button);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Clicks", e.Clicks);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "X", e.X);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Y", e.Y);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Delta", e.Delta);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Location", e.Location);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "MouseClick Event");
        }*/
   
    }

  

}
