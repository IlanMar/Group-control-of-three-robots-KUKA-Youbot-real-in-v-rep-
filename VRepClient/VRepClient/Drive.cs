using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRepClient
{
    public class RobPos //2019-01-31
    {
        public float RobX, RobY, RobA;
    }

    public class Drive
    {
        public float right, left, Phi, Phi2, Phi3, PhiR, Phi5, forwBackVel, leftRightVel, rotVel, ResultPhi, PhiObs;
        public float TargetDirection, TargetDirection2, TargetDirection3;
        public float RobotDirection, RobotDirection2, RobotDirection3;//переменная для вывода на форму чрез форму
        public float DistToTarget, DistToTarget2, DistToTarget3;
        float ResultOld = 0;
        float ResultOldTwo = 0;
        float KeyToMove = 0; //если робот стои больше н циклов то ехать
        public void GetDrive(RobPos p, RobPos p2, RobPos p3, float GoalPointX, float GoalPointY, float Xmax, float Ymax, Point CenterMass, float[] LedData, float[, ,] graph)
        {       
            Phi = 0;
            Phi2 = 0;
            Phi3 = 0;
            PhiR = 0;
            Phi5 = 0;
            PhiObs = 0;


            //2019-02-01
            DistToTarget=float.MaxValue;
            DistToTarget2=float.MaxValue;
            DistToTarget3= float.MaxValue;


            #region Find Phi from robot to goal
            GoalPointX = GoalPointX * 0.1f;
            GoalPointY = GoalPointY * 0.1f;
            Xmax = Xmax * 0.1f;
            Ymax = Ymax * 0.1f;
            p.RobX = p.RobX + Xmax / 2;
            p.RobY = p.RobY + Ymax / 2;



            RobotDirection = p.RobA;
            //float GoalPointX = -1;
            // float GoalPointY = 1;
            //определяем относительное направление цели цели
            float Xpel = GoalPointX - p.RobX;
            float Ypel = GoalPointY - p.RobY;
            TargetDirection = (float)Math.Atan2(Xpel, Ypel);//надо просто RobA-
            DistToTarget = (float)Math.Sqrt(Xpel * Xpel + Ypel * Ypel);

            // Phi = TargetDirection - RobA;

            if (TargetDirection - p.RobA < Math.PI && TargetDirection - p.RobA > -Math.PI)
            {
                Phi = TargetDirection - p.RobA;
            }
            else
            {
                if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + TargetDirection - p.RobA)))//если угол между точками больше двух ПИ
                {
                    // Phi = (-1) * (float)(Math.PI * 2 + TargetDirection - RobA);
                    Phi = (float)(Math.PI * 2 + TargetDirection - p.RobA);
                }
                else
                {
                    Phi = (TargetDirection - p.RobA - (float)(Math.PI * 2));
                }
            }
            #endregion
            float PhiCenterMass = 0;
            float CenterMassX = (CenterMass.X) * 0.1f; //для определения пеленга для центра масс
            float CenterMassY = (CenterMass.Y) * 0.1f; //для определения пеленга для центра масс
            float CenterMassXpel = GoalPointX - CenterMassX; //для определения пеленга для центра масс
            float CenterMassYpel = GoalPointY - CenterMassY; //для определения пеленга для центра масс
            float DistToTargetmass = (float)Math.Sqrt(CenterMassXpel * CenterMassXpel + CenterMassYpel * CenterMassYpel); //для определения пеленга для центра масс

            float CenterMassTargetDirection = (float)Math.Atan2(CenterMassXpel, CenterMassYpel);//надо просто RobA-

            if (CenterMassTargetDirection - p.RobA < Math.PI && CenterMassTargetDirection - p.RobA > -Math.PI)
            {
                PhiCenterMass = CenterMassTargetDirection - p.RobA;
            }
            else
            {
                if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + CenterMassTargetDirection - p.RobA)))//если угол между точками больше двух ПИ
                {
                    PhiCenterMass = (float)(Math.PI * 2 + CenterMassTargetDirection - p.RobA);
                }
                else
                {
                    PhiCenterMass = (CenterMassTargetDirection - p.RobA - (float)(Math.PI * 2));
                }
            }

            if (p2 != null)
            {
                #region Find Phi from robot to robot2
                float RobTwoX = p2.RobX + Xmax / 2;
                float RobTwoY = p2.RobY + Ymax / 2;
                //определяем относительное направление цели цели
                float Xpel2 = RobTwoX - p.RobX;
                float Ypel2 = RobTwoY - p.RobY;
                TargetDirection2 = (float)Math.Atan2(Xpel2, Ypel2);
                DistToTarget2 = (float)Math.Sqrt(Xpel2 * Xpel2 + Ypel2 * Ypel2);
                if (TargetDirection2 - p.RobA < Math.PI && TargetDirection2 - p.RobA > -Math.PI)
                {
                    Phi2 = TargetDirection2 - p.RobA;
                }
                else
                {
                    if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + TargetDirection2 - p.RobA)))//если угол между точками больше двух ПИ
                    {
                        // Phi = (-1) * (float)(Math.PI * 2 + TargetDirection - RobA);
                        Phi2 = (float)(Math.PI * 2 + TargetDirection2 - p.RobA);
                    }
                    else
                    {
                        Phi2 = (TargetDirection2 - p.RobA - (float)(Math.PI * 2));
                    }
                }
                #endregion
            }
            if (p3 != null)
            {
                #region Find Phi from robot to robot3
                float RobThreeX = p3.RobX + Xmax / 2;
                float RobThreeY = p3.RobY + Ymax / 2;
                //определяем относительное направление цели цели
                float Xpel3 = RobThreeX - p.RobX;
                float Ypel3 = RobThreeY - p.RobY;
                TargetDirection3 = (float)Math.Atan2(Xpel3, Ypel3);
                DistToTarget3 = (float)Math.Sqrt(Xpel3 * Xpel3 + Ypel3 * Ypel3);
                if (TargetDirection3 - p.RobA < Math.PI && TargetDirection3 - p.RobA > -Math.PI)
                {
                    Phi3 = TargetDirection3 - p.RobA;
                }
                else
                {
                    if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + TargetDirection3 - p.RobA)))//если угол между точками больше двух ПИ
                    {
                        // Phi = (-1) * (float)(Math.PI * 2 + TargetDirection - RobA);
                        Phi3 = (float)(Math.PI * 2 + TargetDirection3 - p.RobA);
                    }
                    else
                    {
                        Phi3 = (TargetDirection3 - p.RobA - (float)(Math.PI * 2));
                    }
                }
                #endregion
            }

            float fx2 = 0;
            float fx3 = 0;
            float fx = 0;
            float fxObs = 0;
            //float fx = (9f / DistToTarget* DistToTarget);
            // fx2 = Math.Abs(10f / DistToTarget2 * DistToTarget2);
            // fx3 = Math.Abs(10f / DistToTarget3 * DistToTarget3);
            // fx = Math.Abs(10f / (float)Math.Exp((DistToTargetmass*1)));
            fx = (float)Math.Log10(DistToTarget * 40);
            fx = fx * 2f;
            if (DistToTarget2 < 2.2) fx2 = Math.Abs(38f / (float)Math.Exp((DistToTarget2 * 3f)));
            if (DistToTarget3 < 2.2) fx3 = Math.Abs(38f / (float)Math.Exp((DistToTarget3 * 3f)));
            if (DistToTarget2 > 2.8) { fx2 = Math.Abs((float)Math.Exp(DistToTarget2 * 0.3f)); }
            if (DistToTarget3 > 2.8) { fx3 = Math.Abs((float)Math.Exp(DistToTarget3 * 0.3f)); }
            /*
            if (DistToTarget2 < 2)
            {
                fx2 = Math.Abs((float)Math.Exp(DistToTarget2 * 0.2f));
                if (DistToTarget3 > 2.5) { fx3 = Math.Abs((float)Math.Exp(DistToTarget3 * 0.2f)); }
            }
            if (DistToTarget3 < 2)
            {
                fx3 = Math.Abs((float)Math.Exp(DistToTarget3 * 0.2f));
                if (DistToTarget3 > 2.5) { fx3 = Math.Abs((float)Math.Exp(DistToTarget3 * 0.2f)); }
            }
             if (DistToTarget2 > 2 && DistToTarget3 > 2)
             {
                 fx2 = Math.Abs((float)Math.Exp(DistToTarget2 * 0.2f));
                 fx3 = Math.Abs((float)Math.Exp(DistToTarget3 * 0.2f));
             }
             if (DistToTarget2 < 1.2) fx2 = Math.Abs(20f / (float)Math.Exp((DistToTarget2 * 2f)));
             if (DistToTarget3 < 1.2) fx3 = Math.Abs(20f / (float)Math.Exp((DistToTarget3 * 2f)));
            //if (DistToTarget2 > 2.0) { fx2 = Math.Abs((float)Math.Exp(DistToTarget2 * 0.2f)); }
           // if (DistToTarget3 > 2.0) { fx3 = Math.Abs((float)Math.Exp(DistToTarget3 * 0.2f)); }
            */

            float OldX = 0, OldY = 0;
            float ObsResultX = 0;
            float ObsResultY = 0;
            for (int gx = 0; gx < 179; gx++) //для всех клеток графа в радиусе двух метров, строятся вектора силы и складываются
            {
                for (int gy = 0; gy < 179; gy++)
                {
                    if (graph[gx, gy, 0] == 2)
                    {
                        float Xobs = gx;
                        float Yobs = gy;

                        Xobs = gx - p.RobX * 10;
                        Yobs = gy - p.RobY * 10;
                        float DistToObs = (float)Math.Sqrt(Xobs * Xobs + Yobs * Yobs);
                        DistToObs = DistToObs * 0.1f;
                        if (DistToObs < 1)
                        {
                            float ObsDirection = (float)Math.Atan2(Xobs, Yobs);
                            fxObs = Math.Abs(80f / (float)Math.Exp((DistToObs * 4f)));//сила отталкивания от препатствия

                            #region Find PhiObs
                            if (ObsDirection - p.RobA < Math.PI && ObsDirection - p.RobA > -Math.PI)
                            {
                                PhiObs = ObsDirection - p.RobA;
                            }
                            else
                            {
                                if ((Math.PI * 2) > Math.Abs((float)(Math.PI * 2 + ObsDirection - p.RobA)))//если угол между точками больше двух ПИ
                                {

                                    PhiObs = (float)(Math.PI * 2 + ObsDirection - p.RobA);
                                }
                                else
                                {
                                    PhiObs = (ObsDirection - p.RobA - (float)(Math.PI * 2));
                                }
                            }
                            #endregion
                            if (PhiObs < 0)
                            {
                                PhiObs = (PhiObs + (float)Math.PI);
                            }
                            else
                                PhiObs = (PhiObs - (float)Math.PI);

                            float PhiObsX = (float)Math.Sin(PhiObs) * fxObs;
                            float PhiObsY = (float)Math.Cos(PhiObs) * fxObs;
                            ObsResultX = PhiObsX + OldX;
                            ObsResultY = PhiObsY + OldY;
                            OldX = PhiObsX;
                            OldY = PhiObsY;
                        }
                    }
                }
            }
            // if (DistToTarget2 > 2 || DistToTarget3 > 2) { fx = 0; fx2 = Math.Abs(6f / DistToTarget2 * DistToTarget2);
            // fx3 = Math.Abs(6f / DistToTarget3 * DistToTarget3);}
            //fx = 0;


            if (DistToTarget2 < 1.2)//добавляем ПИ чтобы сила была направлена не к роботу соседу а от него
            {
                if (Phi2 < 0)
                {
                    Phi2 = (Phi2 + (float)Math.PI);
                }
                else
                    Phi2 = (Phi2 - (float)Math.PI);
            }
            if (DistToTarget3 < 1.2)
            {
                if (Phi3 < 0)
                {
                    Phi3 = (Phi3 + (float)Math.PI);
                }
                else
                    Phi3 = (Phi3 - (float)Math.PI);
            }


           
            //складываем вектора, длины векторов задаются как силы fx
            float c = 1;
            float a = (float)Math.Sin(Phi) * fx;
            float b = (float)Math.Cos(Phi) * fx;

            if (p2 == null) fx2 = 0;
            float Phi2a = (float)Math.Sin(Phi2) * fx2;
            float Phi2b = (float)Math.Cos(Phi2) * fx2;

            if (p3 == null) fx3 = 0;
            float Phi3a = (float)Math.Sin(Phi3) * fx3;
            float Phi3b = (float)Math.Cos(Phi3) * fx3;

            float PhiSumA = Phi2a + Phi3a + a + ObsResultX;
            float PhiSumB = Phi2b + Phi3b + b + ObsResultY;

            ResultPhi = (float)Math.Atan2(PhiSumA, PhiSumB);
            ResultPhi = ResultPhi * -1;//раскоментить для работы с настоящей кукой

            if (ResultOldTwo > ResultPhi && ResultOldTwo - ResultPhi > 1.5)
            {
                if (ResultOld > ResultPhi && ResultOld - ResultPhi > 0.5) { ResultPhi = ResultOld - 0.5f; }
                if (ResultOld < ResultPhi && ResultPhi - ResultOld > 0.5) { ResultPhi = ResultOld + 0.5f; }
                ResultOld = ResultPhi;
            }
            if (ResultOldTwo < ResultPhi && ResultPhi - ResultOldTwo > 1.5)
            {
                if (ResultOld > ResultPhi && ResultOld - ResultPhi > 0.5) { ResultPhi = ResultOld - 0.5f; }
                if (ResultOld < ResultPhi && ResultPhi - ResultOld > 0.5) { ResultPhi = ResultOld + 0.5f; }
                ResultOld = ResultPhi;
            }


            

            c = (float)Math.Sqrt(PhiSumA * PhiSumA + PhiSumB * PhiSumB) / 1.5f;
            a = (float)Math.Sin(ResultPhi) * c;
            b = (float)Math.Cos(ResultPhi) * c;
            
            //2019-02-01
            if (DistToTargetmass < 1f) //создаем зону в которой роботы успокаиваются
            {
                b = 0; a = 0; PhiCenterMass = 0;
                //fx2 = 0;         // fx3 = 0;
            }

            if (DistToTarget < 0.5f) { b = 0; a = 0; Phi = 0; }//защита
            if (DistToTarget2 < 0.5f) { b = 0; a = 0; Phi = 0; }
            if (DistToTarget3 < 0.5f) { b = 0; a = 0; Phi = 0; }

            if (ResultOldTwo > ResultPhi && ResultOldTwo - ResultPhi > 1.5)
            {
                b = 0; a = 0; KeyToMove++;
            }
            if (ResultOldTwo < ResultPhi && ResultPhi - ResultOldTwo > 1.5)
            {
                b = 0; a = 0; KeyToMove++;
            }
            else
            {
                forwBackVel = b;
                leftRightVel = a;
                rotVel = 0;// PhiCenterMass;// ResultPhi;
                KeyToMove = 0;
                ResultOldTwo = ResultPhi;
            }



            if (ResultPhi < -1.5 && ResultPhi > 1.5 && KeyToMove > 100)//если прошло больше 10 циклов а роот стоит то ехать в нужную сторону
            {
                forwBackVel = b;
                leftRightVel = a;
                rotVel = 0;// PhiCenterMass;// ResultPhi;
                KeyToMove = 0;
                ResultOldTwo = ResultPhi;
            }
            if (ResultPhi > -1.5 && ResultPhi < 1.5 && KeyToMove > 10)
            {
                forwBackVel = b;
                leftRightVel = a;
                rotVel = 0;// PhiCenterMass;// ResultPhi;
                KeyToMove = 0;
                ResultOldTwo = ResultPhi;
            }


            //forwBackVel = 0;
            //leftRightVel = 0;
            //rotVel = 0;

           
        }

    }
}
