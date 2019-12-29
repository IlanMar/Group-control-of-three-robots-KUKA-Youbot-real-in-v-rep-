//Margolin Ilan 2016
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using VRepAdapter;

namespace VRepClient
{
    public class RobotAdapter
    {
        public virtual void Init(int robotnumber) { }
        public virtual void Deactivate(){}
        public virtual void Send(Drive RobDrive) { }//отправка управляющих команд
        public virtual void ReceiveLedData(string LedarData) { } /*получение данных ледара и закидывание их в массив*/
        public virtual void ReceiveOdomData(string OdometryData, float Xdelta,float Ydelta ) { }//получение данных одометрии и закидывание их в массив
        //Xdelta и Ydelta это смещение робота относительно цетра карты при инициализации 
        public float[] RobotLedData;//сюда заносятся данные с ледара Врепа
        public float[] RobotOdomData;//сюда заносятся данные одометри Врепа
        public string RobotCamOdomData;//сюда заносятся данные о местоположения роботов через камеры

      //  public float right;
      //  public float left;
        public float forwBackVel, leftRightVel, rotVel;
        public float Xrob1, Yrob1, Arob1, Xrob2, Yrob2, Arob2, Xrob3, Yrob3, Arob3;//переменные в которые заносится одометрия с камер
               
    }

    public class VrepAdapter : RobotAdapter //наследный класс для работы с Vrep
    {
        public static int clientID = -1;
        int leftMotorHandle, rightMotorHandle, sensorHandle, leftMotorHandleA, rightMotorHandleA;
        float driveBackStartTime = -99000;
        float[] motorSpeeds = new float[4];

      
        public override void Init(int robotnumber) 
        {

            if (clientID == -1)
            {
                clientID = VRepFunctions.Start("127.0.0.1", 7777);
            }
            if (clientID != -1)
            {
                string rn=null;//rn - robot number прибавляем номер робота
                if (robotnumber == 1) rn = null;
                if (robotnumber == 2) rn = "#0";
                if (robotnumber == 3) rn="#1";
                VRepFunctions.GetObjectHandle(clientID, "rollingJoint_fl"+rn, out leftMotorHandle);
                VRepFunctions.GetObjectHandle(clientID, "rollingJoint_rl"+rn, out leftMotorHandleA);
                VRepFunctions.GetObjectHandle(clientID, "rollingJoint_rr"+rn, out rightMotorHandle);
                VRepFunctions.GetObjectHandle(clientID, "rollingJoint_fr"+rn, out rightMotorHandleA);
               // VRepFunctions.GetObjectHandle(clientID, "Proximity_sensor", out sensorHandle);

            }
        }
        public override void Send(Drive RobDrive)
        { /*youbot_connection.send(ToString(data));*/
            if (RobDrive != null)
            {
               // right = RobDrive.right * (-5f);//20.02 удалет тип var//*(-2.5f);
               //  left = RobDrive.left * (-5f);
                forwBackVel = RobDrive.forwBackVel * (5);
                leftRightVel = RobDrive.leftRightVel * (5);//0.5f
                rotVel = RobDrive.rotVel * (5);//0.5f
                 //right = 0;//для проверки
              //  left = 0;
            }
            if (VRepFunctions.GetConnectionId(clientID) == -1) return;

            Byte sensorTrigger = (Byte)0;
            
                   
            int simulationTime = VRepFunctions.GetLastCmdTime(clientID);//??????
            if (simulationTime - driveBackStartTime < 3000)
                driveBackStartTime = simulationTime;
            {
                VRepFunctions.SetJointTargetVelocity(clientID, leftMotorHandle, -forwBackVel- leftRightVel- rotVel);
                VRepFunctions.SetJointTargetVelocity(clientID, leftMotorHandleA, -forwBackVel+ leftRightVel- rotVel);
                VRepFunctions.SetJointTargetVelocity(clientID, rightMotorHandle,-forwBackVel- leftRightVel+ rotVel);
                VRepFunctions.SetJointTargetVelocity(clientID, rightMotorHandleA, -forwBackVel+ leftRightVel+ rotVel);
            }
        }
        public override void ReceiveLedData(string LedarData)
        { /* парсинг строки из Vrep "0.3, 0.1, -0.7" *//* base.Recieve("0.3, 0.1, +0.7");*/
         RobotLedData = new float[518];//более 1412 это бесконечность
         float[,] LaserDatatemporaryVrep;//временный массив с координатами видимых препядствий
         string g = LedarData;
         LaserDatatemporaryVrep = new float[684, 3];
         if (g != "")
         {
             string someString = LedarData;
             string[] words = someString.Split(new char[] { ';' });// words
             int h = 0;//вспомогательная переменная для преодразоания str в массиив
             
             for (int i = 0; i < 684; i++)//записываем данные с ледара в двухмерный массив 683 на 3, в сроках x y z
             {
                 for (int j = 0; j < 3; j++)
                 {
                     LaserDatatemporaryVrep[i, j] = float.Parse(words[h], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                     if (LaserDatatemporaryVrep[i, j] == 0) { LaserDatatemporaryVrep[i, j] = 500; }
                     h++;
                 }
             }

             int d=0;
             for (int i = 83; i < 601; i++) //в массив LaserDataVrep, длиной 516 высчитываем и добавляем расстояния до объектов
             {
                 RobotLedData[d] = (float)(Math.Sqrt(LaserDatatemporaryVrep[i, 0] * LaserDatatemporaryVrep[i, 0] + LaserDatatemporaryVrep[i, 1] * LaserDatatemporaryVrep[i, 1]));
                 d++;
             }
                       

         }
            
        }
        public override void ReceiveOdomData(string OdometryData, float Xdelta, float Ydelta) 
        {
            RobotOdomData = new float[3];
            if (OdometryData != "")
            {
                // string someString = RobPos;
                string[] words = OdometryData.Split(new char[] { ';' });//парсим строку в массив words
                for (int i = 0; i < 3; i++)
                {
                    RobotOdomData[i] = float.Parse(words[i], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

                }
              
                //textBox2.Text = h;
                //    RobLocDataKuka[2] = RobLocDataKuka[2] * -1;//
            }
        
        }

        public override void Deactivate()
        {
            VRepFunctions.Finish(clientID);
        }
    }

    public class YoubotAdapter : RobotAdapter //наследный класс для работы с Kuka Youbot
    {
        public TcpConnection tc;
        
        public void TCPconnect(string ip,string process, bool is_robot) 
        {
            tc = new TcpConnection(0, Form1.f1,
                   str => MessageBox.Show("Connected!"),
                   str =>
                   {
                       try
                       {
                           ProcessTCP(str, ip);
                       }
                       catch { }
                   },
                   str => MessageBox.Show("Disconnected!"));

            tc.Connect(ip, process, is_robot);
        }
        private void ProcessTCP(string str, string ip)
        {
            if (str.StartsWith(".laser#"))
            {
                int ind = str.IndexOf("#");
                var LedData = str.Substring(ind + 1);

                Form1.f1.ShowLedData(LedData);
                //KukaPotField.LedDataKuka(s);//отправляем данные с лудара куки
                ReceiveLedData(LedData); //отправляем данные с ледара
            }
           // float Xrob1, Yrob1, Arob1, Xrob2, Yrob2, Arob2, Xrob3, Yrob3, Arob3;
            if (str.Contains("pt id="))
            {
                Form1.f1.rtb_cam.Invoke(new Action(() => Form1.f1.rtb_cam.Text = str));
                             
                string Firstrobot= str.Substring(55, 17);
                string Secondrobot = str.Substring(112, 17);
                string Thirdrobot = str.Substring(169, 17);

              //  CultureInfo culture = new CultureInfo("ru");
                Xrob3 = float.Parse(Firstrobot.Substring(0, 3), new CultureInfo("en-US"));//раньше это был Хроб1
                Yrob3 = float.Parse(Firstrobot.Substring(6, 4), new CultureInfo("en-US"));
            //    Arob1 = float.Parse(Firstrobot.Substring(10, 4), new CultureInfo("en-US"));

                Xrob2 = float.Parse(Secondrobot.Substring(0, 3), new CultureInfo("en-US"));
                Yrob2 = float.Parse(Secondrobot.Substring(6, 4), new CultureInfo("en-US"));
             ///   Arob2 = float.Parse(Secondrobot.Substring(10, 4), new CultureInfo("en-US"));

                Xrob1 = float.Parse(Thirdrobot.Substring(0, 3), new CultureInfo("en-US"));
                Yrob1 = float.Parse(Thirdrobot.Substring(6, 4), new CultureInfo("en-US"));
              //  Arob3 = float.Parse(Thirdrobot.Substring(10, 4), new CultureInfo("en-US"));
             
                //или просто по символам, отсчитать символы для чисел в первой второй и третей строчке


            }
                     


            if (str.Contains(".odom#"))
            {
                int ind = str.IndexOf("#");
                var OdometryData = str.Substring(ind + 1);
                //  KukaPotField.RodLocReceivingKuka(s);//отпровляем "s" данные одометрии
                Form1.f1.ShowOdomData(OdometryData);
                float Xdelta = 0, Ydelta = 0;

                //2019-02-01
                var info = Form1.f1.findRobInfo(ip);
                if (info!=null)
                {
                    Xdelta = info.x; Ydelta = info.y;
                 //   Xdelta = Form1.f1.yatccam.Xrob1; Ydelta = 6 - Form1.f1.yatccam.Yrob1;
                   // Xdelta = Form1.f1.yatccam.Xrob1 - Form1.f1.yatccam.Xrob2; Ydelta = Form1.f1.yatccam.Yrob2 - Form1.f1.yatccam.Yrob1;
                   
                    //Xdelta = Xdelta * -1; Ydelta = Ydelta * -1;
                    //2019-02-01
                    Ydelta = Ydelta * -1;
                }
               
                ReceiveOdomData(OdometryData,   Xdelta, Ydelta); //отправляем данные одометрии
            }
            //ra.Receive(LedData, OdometryData); 
            if (tc == null)
            {

                return;
            }

        }
        string ExtractSubsection(string firststr, int startnum, int endnum)
        {
            //string a =   name;
            
            return firststr.Substring(startnum, startnum - endnum);
       

        }

        string last_str; //2019-02-01
        public override void Send(Drive RobDrive) 
        {
            if (RobDrive != null)
            {
                forwBackVel = RobDrive.forwBackVel;
                leftRightVel = RobDrive.leftRightVel;
                rotVel = RobDrive.rotVel;
            }

            if (tc != null)// здесь происходит отправка задающих команд на куку
            {
                string control_str;
                 control_str = string.Format( "LUA_Base({0}, {1}, {2})", 0, 0, 0);
                

                    float Vrob = forwBackVel; //(right + left) / 2;
                    float LRrob = leftRightVel;
                    float Wrob = rotVel;//(right - left) / 1;
                    var speed = 0.1f;
                    //var k_slow = 0.1f;
                    var arg1 = Vrob; arg1 = (float)Math.Max(-speed, Math.Min(arg1, speed));//надо переделать эти выводы для адекватного вывода
                    var arg2 = LRrob; arg2 = (float)Math.Max(-speed, Math.Min(arg2, speed));
                    var arg3 = Wrob; arg3 = Math.Max(-speed, Math.Min(arg3, speed));//возможно(left-right)
                    control_str = string.Format(CultureInfo.InvariantCulture, "LUA_Base({0}, {1}, {2})", arg1, arg2, arg3);

                // var b = KukaPotField.ObstDistKuka(KukaPotField.LaserDataKuka, KukaPotField.RobLocDataKuka);
                //if (b)
                //{

                    if (control_str != null && control_str!=last_str)
                        tc.Send(control_str);//отправляем команду на сервер куки

                last_str = control_str;
            }
            /*youbot_connection.send(ToString(data));*/
        }
        public override void ReceiveLedData(string LedarData) 
        { /* парсинг строки из Vrep "0.3, 0.1, -0.7" *//*base.Recieve("0.3, 0.1, +0.7"); */
                        
            string g = LedarData;
            
           // float[] LaserDataKuka;
            if (g != "")
            {
                string someString = LedarData;
                string[] words = someString.Split(new char[] { ';' });// words
                int h = 0;//вспомогательная переменная для преобразоания str в массиив

               // LaserDataKuka = new float[words.Length];
                RobotLedData = new float[words.Length];
                for (int i = 0; i < words.Length; i++)//записываем данные с ледара в массив, в сроках x y z
                {
                    RobotLedData[i] = float.Parse(words[h], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    h++;
                }
             }

        }
        public override void ReceiveOdomData(string OdometryData, float Xdelta, float Ydelta) 
        {
            RobotOdomData = new float[3];
            if (OdometryData != "")
            {
                // string someString = RobPos;
                string[] words = OdometryData.Split(new char[] { ';' });//парсим строку в массив words
                for (int i = 0; i < 3; i++)
                {
                    RobotOdomData[i] = float.Parse(words[i], System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    
                }
                float alpha = RobotOdomData[0];
                
                 RobotOdomData[0] = RobotOdomData[1];
                 RobotOdomData[1] = alpha;
                 RobotOdomData[1] = RobotOdomData[1] + Ydelta;
                 RobotOdomData[0] = RobotOdomData[0] + Xdelta;
            }
        }
        public override void Deactivate()
        {

            if (tc != null) tc.Disconnect("form closing", false);
        }
    }

}
