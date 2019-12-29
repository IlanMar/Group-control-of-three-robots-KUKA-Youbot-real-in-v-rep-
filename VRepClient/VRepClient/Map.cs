using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace VRepClient
{
    public class ObstaclesPoint
    {
        public float X;
        public float Y;
        public float weight = 2f;
    }
    public class Map
    {

        public List<ObstaclesPoint> GlobalMapList = new List<ObstaclesPoint>();
        public float[] RobOdData; //только для вывода на форму картинкис роботом
        public bool invalidateform = false;
        public float Phi, Phi2, Phi3, Phi4;//Phi4 пеленг на рассматриваемую точку с ледара
        public void LedDataToList(float[] LedData, float[] OdomData, float[] OdomData2, float[] OdomData3)
        {
            if (LedData != null)
            {
                RobOdData = new float[OdomData.Length];
                RobOdData[0] = OdomData[0]; RobOdData[1] = OdomData[1]; RobOdData[2] = OdomData[2];//строчка только для вывода перемеще
                float[,] LedDataMass = new float[LedData.Length, 2];//массив с координатами точек относительно робота//убрать коменты
                float A = 180f / LedData.Length;//угол между данными с ледара
                float x; // угол между катетом и гипотенузой
                float c; //длина гипотенузы
                float u = 1;//для умножения X на -1 когда x становится больше 90 градусов


                #region Find Phi2 from robot to robot2
                float RobTwoX = OdomData2[0] + Xmax / 2;
                float RobTwoY = OdomData2[1] + Ymax / 2;
                Phi = 0; Phi2 = 0; Phi3 = 0;
                //определяем относительное направление цели цели
                float Xpel2 = RobTwoX - (OdomData[0] + Xmax / 2);
                float Ypel2 = RobTwoY - (OdomData[1] + Xmax / 2);
                float TargetDirection2 = (float)Math.Atan2(Xpel2, Ypel2);
                float DistToTarget2 = (float)Math.Sqrt(Xpel2 * Xpel2 + Ypel2 * Ypel2);
                if (TargetDirection2 - OdomData[2] < Math.PI && TargetDirection2 - OdomData[2] > -Math.PI)
                {
                    Phi2 = TargetDirection2 - OdomData[2];
                }
                else
                {
                    if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + TargetDirection2 - OdomData[2])))//если угол между точками больше двух ПИ
                    {
                        // Phi = (-1) * (float)(Math.PI * 2 + TargetDirection - RobA);
                        Phi2 = (float)(Math.PI * 2 + TargetDirection2 - OdomData[2]);
                    }
                    else
                    {
                        Phi2 = (TargetDirection2 - OdomData[2] - (float)(Math.PI * 2));
                    }
                }
                #endregion
                #region Find Phi3 from robot to robot3
                RobTwoX = OdomData3[0] + Xmax / 2;
                RobTwoY = OdomData3[1] + Ymax / 2;

                //определяем относительное направление цели цели
                Xpel2 = RobTwoX - (OdomData[0] + Xmax / 2);
                Ypel2 = RobTwoY - (OdomData[1] + Xmax / 2);
                float TargetDirection3 = (float)Math.Atan2(Xpel2, Ypel2);
                float DistToTarget3 = (float)Math.Sqrt(Xpel2 * Xpel2 + Ypel2 * Ypel2);
                if (TargetDirection3 - OdomData[2] < Math.PI && TargetDirection3 - OdomData[2] > -Math.PI)
                {
                    Phi3 = TargetDirection3 - OdomData[2];
                }
                else
                {
                    if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + TargetDirection3 - OdomData[2])))//если угол между точками больше двух ПИ
                    {
                        // Phi = (-1) * (float)(Math.PI * 2 + TargetDirection - RobA);
                        Phi3 = (float)(Math.PI * 2 + TargetDirection3 - OdomData[2]);
                    }
                    else
                    {
                        Phi3 = (TargetDirection3 - OdomData[2] - (float)(Math.PI * 2));
                    }
                }
                #endregion



                for (int i = 0; i < LedData.Length; i++)
                {
                    if (LedData[i] <2)
                    {
                        float Alpha = (float)Math.PI / LedData.Length;
                        float Angle = i * Alpha;
                        float aa = (float)(Math.Sin(Angle) * LedData[i]);//y
                        float cc = (float)(Math.Cos(Angle) * LedData[i]);//x
                        #region Find Phi4 from robot to laserpoint

                        //float temp = RobOdData[0] + Xmax / 2;
                        //float remp2 = RobOdData[1] + Ymax / 2; ;
                        RobTwoX = cc;//x
                        RobTwoY = aa;//y

                        //определяем относительное направление цели цели
                        Xpel2 = RobTwoX - (0);
                        Ypel2 = RobTwoY - (0);
                        float TargetDirection4 = (float)Math.Atan2(Xpel2, Ypel2);
                        float DistToTarget4 = (float)Math.Sqrt(Xpel2 * Xpel2 + Ypel2 * Ypel2);
                        if (TargetDirection4 - 0 < Math.PI && TargetDirection3 - 0 > -Math.PI)
                        {
                            Phi4 = TargetDirection4 - 0;
                        }
                        else
                        {
                            if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + TargetDirection4 - 0)))//если угол между точками больше двух ПИ
                            {
                                // Phi = (-1) * (float)(Math.PI * 2 + TargetDirection - RobA);
                                Phi4 = (float)(Math.PI * 2 + TargetDirection4 - 0);
                            }
                            else
                            {
                                Phi4 = (TargetDirection4 - 0 - (float)(Math.PI * 2));
                            }
                        }
                        #endregion


                        c = LedData[i];
                        x = i * A;
                        x = x * 0.0174533f - OdomData[2];
                        if (Phi4 + 1.4 < Phi2 || Phi4 - 1.4 > Phi2)
                        {
                            if (Phi4 + 1.4 < Phi3 || Phi4 - 1.4 > Phi3)
                            {

                                float LedRX = 0.344f * (float)Math.Cos(OdomData[2]);
                                float LedRY = 0.344f * (float)Math.Sin(OdomData[2]);
                                LedDataMass[i, 0] = ((float)Math.Cos(x) * c) + OdomData[0] + LedRY;//значение по оси Х
                                LedDataMass[i, 1] = ((float)Math.Sin(x) * c) + OdomData[1] + LedRX;// значение по оси Y   
                                u = 1;
                            }
                            if (DistToTarget2 > 2.4 && DistToTarget3 > 2.4)
                            {

                                float LedRX = 0.344f * (float)Math.Cos(OdomData[2]);
                                float LedRY = 0.344f * (float)Math.Sin(OdomData[2]);
                                //LedDataMass[i, 0] = ((float)Math.Cos(x) * c) + OdomData[0] + LedRY;//значение по оси Х
                                //LedDataMass[i, 1] = ((float)Math.Sin(x) * c) + OdomData[1] + LedRX;// значение по оси Y   
                                u = 1;
                            }
                        }
                    }
                }
                GlobalMapList.Add(new ObstaclesPoint { X = 0f, Y = 0f, weight = 1 });//задаю одну точку по умолчанию с обеих сторон от робота


                float DistBetweenPoints = 0;

                for (int g = 0; g < LedData.Length; g++)
                {
                    float radius = 0;
                    float h = 4;//для запомнания кратчайшей точки в цикле
                    for (int i = 0; i < GlobalMapList.Count; i++)
                    {

                        float Xpel = LedDataMass[g, 0] - GlobalMapList[i].X;
                        float Ypel = LedDataMass[g, 1] - GlobalMapList[i].Y;
                        DistBetweenPoints = (float)Math.Abs(Math.Sqrt(Xpel * Xpel + Ypel * Ypel));
                        // radius = (float)(Math.Sqrt(LedDataMass[g, 0] * LedDataMass[g, 0] + LedDataMass[g, 1] * LedDataMass[g, 1]));
                        radius = LedData[g];// для отсеивания точек дальше 4-ех метров
                        if (DistBetweenPoints < h)
                        {
                            h = DistBetweenPoints;
                        }
                        if (h > 0.03 && radius < 2 && i == GlobalMapList.Count - 1 && radius > 0.2)
                        {
                            //закоментено 09.06.2016 для того чтобы ледар не мешал тестам
                            GlobalMapList.Add(new ObstaclesPoint { X = LedDataMass[g, 0], Y = LedDataMass[g, 1], weight = 2 });


                        }



                    }
                }
                filterGlobalMapList(GlobalMapList, LedData, OdomData, LedDataMass);//отправка листа в функцию на отфильтровывание точек                  
            }
        }
        /*
        public void OtherRobotsToGraph(List<ObstaclesPoint> GlobalMapList, float[] OdomData, float[] OdomData2, float[] OdomData3)//те клетки накоторых стоит робот отмечаем как зоны с пониженной проходимосттью
        {

            GlobalMapList.RemoveAll(x => x.weight == 3f);

            for (int i = -7; i < 8; i++)
            {
                for (int k = -7; k < 8; k++)
                {
                    GlobalMapList.Add(new ObstaclesPoint { X = OdomData[0] + i * 0.1f, Y = OdomData[1] + k * 0.1f, weight = 3f });
                    GlobalMapList.Add(new ObstaclesPoint { X = OdomData3[0] + i * 0.1f, Y = OdomData3[1] + k * 0.1f, weight = 3f });
                    GlobalMapList.Add(new ObstaclesPoint { X = OdomData2[0] + i * 0.1f, Y = OdomData2[1] + k * 0.1f, weight = 3f });
                }
            }


            // GlobalMapList.Add(new ObstaclesPoint { X = OdomData3[0], Y = OdomData3[1], weight = 1.9f });
            // GlobalMapList.Add(new ObstaclesPoint { X = OdomData2[0], Y = OdomData2[1], weight = 1.9f });



        }
         * */
        void filterGlobalMapList(List<ObstaclesPoint> GlobalMapList, float[] LedData, float[] OdomData, float[,] LedDataMass)
        {
            /*
            
            for (int i = 0; i < GlobalMapList.Count; i++)
            {
                for (int g = 0; g < LedData.Length; g++)
                {
                    float Tg = (float)Math.Atan(GlobalMapList[i].X / GlobalMapList[i].Y);
                    float Tg2 = (float)Math.Atan(LedDataMass[g, 0] / LedDataMass[g, 1]);
                    float dx = Math.Abs(GlobalMapList[i].X) - Math.Abs(LedDataMass[g, 0]);
                    float dy = Math.Abs(GlobalMapList[i].Y) - Math.Abs(LedDataMass[g, 1]);
                    float da = (float)Math.Abs(Tg - Tg2);

                    float Xpel = GlobalMapList[i].X - OdomData[0];
                    float Ypel = GlobalMapList[i].Y - OdomData[1];
                    float TargetonPoint = (float)Math.Atan2(Xpel, Ypel);
                    float TT = (float)Math.Abs(TargetonPoint - OdomData[2]);

                    float XpelP = LedDataMass[g, 0] - GlobalMapList[i].X;//для рсчета расстояния между точками
                    float YpelP = LedDataMass[g, 1] - GlobalMapList[i].Y;
                    float DistBetweenPoints = (float)Math.Abs(Math.Sqrt(XpelP * XpelP + YpelP * YpelP));

                   if ( LedData[g] < 2 && DistBetweenPoints>0.1 && da < 0.005 && TT<1.5  ) //проверка если старая точка заслоняется новой то ее удалить
                    {
                          // GlobalMapList.RemoveAt(i);//закоментированно удаление несуществующих точек, временно
                         //   break;
                    }
                    
                    //вся эта функция неправльная, она должна быть привязана к точке листа i а не к g
                    if ( LedData[g] > 2.1 && da < 0.002 && TT < 1.2) //если в данном направвлении точек нет то существующие удалить
                    {
                      //   GlobalMapList.RemoveAt(i);//эта функция почемуто удаляет точки при вращении
                       //  break;
                    }
                }
            }
            */
        }

        // public float[,] graph;
        public float[, ,] graph;
        public int Ymax = 180;
        public int Xmax = 180;
        int oldXmax = 0;
        int oldX = 0; int oldY = 0;

        public void GlobListToGraph(List<ObstaclesPoint> GlobalMapList, float[] OdomData)//метод для перевода листа в матрицу
        {

            graph = new float[Xmax, Ymax, 2];//матрица которая является взвешенным графом
            int ymatrix = 0;
            int xmatrix = 0;
            bool key1 = false;
            bool key2 = false;
            for (int k = 0; k < Xmax; k++)
            {
                for (int k2 = 0; k2 < Ymax; k2++)
                {
                    graph[k, k2, 0] = 1;
                    graph[k, k2, 1] = 0;
                }
            }

            for (int i = 0; i < GlobalMapList.Count; i++)
            {
                float Tx = GlobalMapList[i].X * 10;
                float Ty = GlobalMapList[i].Y * 10;
                xmatrix = (int)Math.Floor(Tx);
                ymatrix = (int)Math.Floor(Ty);
                if (graph[xmatrix + Xmax / 2, ymatrix + Ymax / 2, 0] != 2)
                {
                    graph[xmatrix + Xmax / 2, ymatrix + Ymax / 2, 0] = GlobalMapList[i].weight;
                }

            }
            for (int u = 0; u < 3; u++)
            {
                for (int i = 3; i < Xmax - 3; i++)
                {
                    for (int k = 3; k < Xmax - 3; k++)
                    {



                        if (graph[i, k, 0] != 2)
                        {
                            for (int x = -1; x < 2; x++)
                            {
                                for (int y = -1; y < 2; y++)
                                {
                                    if (graph[i + x, k + y, 0] == 2)
                                    {
                                        graph[i, k, 1] = 1;
                                    }

                                }
                            }

                        }

                        if (graph[i, k, 0] != 2 && graph[i, k, 1] != 1)
                        {
                            for (int x = -1; x < 2; x++)
                            {
                                for (int y = -1; y < 2; y++)
                                {
                                    if (graph[i + x, k + y, 1] == 1)
                                    {
                                        graph[i, k, 1] = 2;
                                    }
                                }
                            }
                        }

                        if (graph[i, k, 0] != 2 && graph[i, k, 1] == 0)
                        {
                            for (int x = -1; x < 2; x++)
                            {
                                for (int y = -1; y < 2; y++)
                                {
                                    if (graph[i + x, k + y, 1] == 2)
                                    {
                                        graph[i, k, 1] = 3;
                                    }
                                }
                            }
                        }
                        
                        if (graph[i, k, 0] != 2 && graph[i, k, 1] == 0)
                        {
                            for (int x = -2; x < 3; x++)
                            {
                                for (int y = -2; y < 3; y++)
                                {
                                    if (graph[i + x, k + y, 1] == 3)
                                    {
                                        graph[i, k, 1] = 4;
                                    }
                                }
                            }
                        }

                    }
                }
            }

        }

    }
}