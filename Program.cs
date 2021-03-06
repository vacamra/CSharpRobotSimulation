﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using AntisocialRobots;

namespace RobotTest
{
    class Program
    {
        //Settings:
        static int stepCount = 50;
        static int roomCellsSize = 100;
        static int robotCount = 250;
        static int unmovableDenominator = 10; // approximately  1 / unmovableDenominator robots will be unmovable


        static void Main(string[] args)
        {
            RobotSimulation sim = init();

            sim.UpdateMode = RobotSimulation.FrameUpdateMode.Sequential;
            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < stepCount; i++)
            {
                sim.PerformFrameUpdate();
                if (sim.CheckInvariant())
                {
                    Console.WriteLine("!!BUG!!, Sequential, step {0}, location {1}", i, sim.ConflictLocation);
                }
                Console.WriteLine("Sequential, step {0}, meanDist {1:0.0000}", i, GetMeanDist(sim.Robots));
            }
            sw.Stop();
            TimeSpan sequentialSpan = sw.Elapsed;

            sim = init();
            sim.UpdateMode = RobotSimulation.FrameUpdateMode.Parallel;

            sw.Restart();
            for (int i = 0; i < stepCount; i++)
            {
                sim.PerformFrameUpdate();
                if (sim.CheckInvariant())
                {
                    Console.WriteLine("!!BUG!!, Parallel, step {0}, location {1}", i, sim.ConflictLocation);
                }
                Console.WriteLine("Parallel, step {0}, meanDist {1:0.0000}", i, GetMeanDist(sim.Robots));
            }
            sw.Stop();

            Console.WriteLine("Sequential took {0}", sequentialSpan);
            Console.WriteLine("Parallel took {0}", sw.Elapsed);
        }

        static RobotSimulation init()
        {
            int step = (stepCount * stepCount) / robotCount;
            RobotSimulation sim = new RobotSimulation(roomCellsSize);

            Random r = new Random();
            for (int i = 0; i < robotCount; i++)
            {
                bool movable = r.Next(unmovableDenominator) != 0;
                int x = (i * step) / roomCellsSize;
                int y = (i * step) % roomCellsSize;
                sim.CreateRobot(x, y, movable);
            }

            return sim;
        }

        static double GetMeanDist(List<Robot> robots)
        {
            double sum = 0;
            foreach (Robot robot in robots)
            {
                double sumInner = 0;
                foreach (Robot r2 in robots)
                {                    
                    sumInner += r2.Location.DistanceTo(robot.Location);
                }
                sum += sumInner / robots.Count;
            }
            return sum / robots.Count;
        }
    }
}
