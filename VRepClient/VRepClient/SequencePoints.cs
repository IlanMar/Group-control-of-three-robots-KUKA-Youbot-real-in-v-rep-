﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRepClient
{
    public class SequencePoints
    {
        public float CurrentPointX=0;
        public float CurrentPointY=0;

        public float GoalPointX;
        public float GoalPointY;
        public float GoalPointX2;
        public float GoalPointY2; 
        public float GoalPointX3;
        public float GoalPointY3;
        float DistToTarget;
        int NumberOfGoalPoint=0;
        float LastDist = 1;
        public bool KeyForSearchInGraph=true;
        public void GetNextPoint(List<Point> ListPoints, Point CenterMass, int Xmax, int Ymax)  //float RobX, float RobY, float RobA, int Xmax, int Ymax)
        {
         //   if(ListPoints!=null){
           // ListPoints[0].X = 1;
          //  ListPoints[0].Y = 1;}

            int RobLocX = (int)(CenterMass.X ) ; //положение робота в клетках графа
            int RobLocY = (int)(CenterMass.Y ) ;
          //  float Xpel = CurrentPointX - RobLocX;
            //float Ypel = CurrentPointY - RobLocY;
          //  DistToTarget = (float)Math.Sqrt(Xpel * Xpel + Ypel * Ypel);
            if (ListPoints != null)
            {
                for (int i = 1; i < ListPoints.Count - 8; i++)//было 4 точки а не 6
                {
                    if (Math.Abs(ListPoints[i].X - RobLocX) < 3 && Math.Abs(ListPoints[i].Y - RobLocY) < 3)
                    {
                        CurrentPointX = ListPoints[i + 8].X;
                        CurrentPointY = ListPoints[i + 8].Y;
                        //KeyForSearchInGraph = true;
                        //  break;
                    }

                }
            }
            /*
            if (NumberOfGoalPoint == 0)
            {
                CurrentPointX = GoalPointX;
                CurrentPointY = GoalPointY;
            }
            float Xpel = CurrentPointX - RobX;
            float Ypel = CurrentPointY - RobY;
                    
            DistToTarget = (float)Math.Sqrt(Xpel * Xpel + Ypel * Ypel);
            if (DistToTarget < 0.3 && LastDist>0.3) { NumberOfGoalPoint++;  LastDist = DistToTarget; }
            if(NumberOfGoalPoint==0)
            {
                CurrentPointX = GoalPointX;
                CurrentPointY = GoalPointY;
                DistToTarget = (float)Math.Sqrt(Xpel * Xpel + Ypel * Ypel);
            }
            if (DistToTarget > 0.5) LastDist = DistToTarget;
            if (NumberOfGoalPoint == 1)
            {
                CurrentPointX = GoalPointX2;
                CurrentPointY = GoalPointY2;
                DistToTarget = (float)Math.Sqrt(Xpel * Xpel + Ypel * Ypel);
                
            }
            if (NumberOfGoalPoint == 2)
            {
                CurrentPointX = GoalPointX3;
                CurrentPointY = GoalPointY3;
                DistToTarget = (float)Math.Sqrt(Xpel * Xpel + Ypel * Ypel);
            }
            */
        }
    }
}
