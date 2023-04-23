using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace pong
{
    public partial class Form1 : Form
    {
        //KEYDOWN/KEYUP LISTS:
        bool[] WSADUpDownLeftRight = new bool[] { false, false, false, false, false, false, false, false };
        Keys[] keysToCheck = new Keys[] { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Up, Keys.Down, Keys.Left, Keys.Right };

        //The first column refers to spaces on the WASDUpDownLeftRight bool[]. The second column refers to the oppoite key to column one, for example [0][0] = Up boolian value, [0][1] = Down boolian value.
        //The final column refers to the space in the "objectDirectionsXY int[]" that should be changed depending on the values in the first two columns.
        //This means I can repeat the same code for each set of keys using a for loop that checks the values in each row and finally outputs a direction.
        int[][] determineDirectionsList = new int[][]
          {
      new int[]{  0,1,4},
      new int[]{  2,3,1},
      new int[]{  4,5,5},
      new int[]{  6,7,2},
          };

        Stopwatch stopwatch = new Stopwatch();

        //BRUSHES & PENS
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        Pen whitePen = new Pen(Color.White, 5);


        //GAME OBJECTS
        Rectangle[] gameObjects = new Rectangle[] //0 = ball, 1 = player1, 2 = player2
        { new Rectangle(189, 424, 65, 65), new Rectangle(171, 0, 110, 110), new Rectangle(171, 747, 110, 110)};

        //Amount of time between each stopwatch update
        int stopwatchInterval = 20;
        int updatePositionInterval = 4;

        //TRACKING INFORMATION ON ONLY THE TWO PADDLES:
        Point[] previousLocations = new Point[] { new Point(10, 10), new Point(10, 10) };
        double[] objectVelocitiesXY = new double[] { 0, 0, 0, 0 };
        //TRACKING OBJECT INFORMATION: 0 = BALL 1 = PLAYER1 2 = PLAYER2
        int[] objectDirectionsXY = new int[] { -1, 0, 0, -1, 0, 0 };
        double[] objectSpeedsXY = new double[] { 0, 7, 7, 0, 7, 7 };
        public Form1()
        {
            InitializeComponent();
            stopwatch.Start();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds % stopwatchInterval == 0)
            {
                if (objectSpeedsXY[0] > 0.5) { objectSpeedsXY[0] -= 0.5; }
                if (objectSpeedsXY[0 + 3] > 0.5) { objectSpeedsXY[0 + 3] -= 0.5; }
            }

            if (stopwatch.ElapsedMilliseconds % updatePositionInterval == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    objectVelocitiesXY[i] = Math.Abs(gameObjects[i + 1].X - previousLocations[i].X);
                    objectVelocitiesXY[i + 2] = Math.Abs(gameObjects[i + 1].Y - previousLocations[i].Y);
                }
            }

            for (int i = 0; i < 2; i++)
            {

                previousLocations[i] = gameObjects[i + 1].Location;
            }

            //Determening all player directions depending on what keys are pressed:
            for (int i = 0; i < determineDirectionsList.Length; i++)
            {
                if (WSADUpDownLeftRight[determineDirectionsList[i][0]] == true && WSADUpDownLeftRight[determineDirectionsList[i][1]] == false) { objectDirectionsXY[determineDirectionsList[i][2]] = -1; }
                else if (WSADUpDownLeftRight[determineDirectionsList[i][1]] == true && WSADUpDownLeftRight[determineDirectionsList[i][0]] == false) { objectDirectionsXY[determineDirectionsList[i][2]] = 1; }
                else { objectDirectionsXY[determineDirectionsList[i][2]] = 0; }
            }

            for (int i = 0; i <= 2; i++)
            {
                //IF ANY OBJECTS ARE HITTING THE WALLS; PUSH THEM BACK
                //TOP WALL
                if (gameObjects[i].Y <= 0)
                {
                    if (i == 0) { objectDirectionsXY[0 + 3] *= -1; }
                    gameObjects[i].Y = 0 + 1;
                }
                //BOTTOM WALL
                if (gameObjects[i].Y >= this.Height - gameObjects[i].Height)
                {
                    if (i == 0) { objectDirectionsXY[0 + 3] *= -1; }
                    gameObjects[i].Y = this.Height - gameObjects[i].Height - 1;
                }
                //LEFT WALL
                if (gameObjects[i].X < 0)
                {
                    if (i == 0) { objectDirectionsXY[0] *= -1; }
                    gameObjects[i].X = 0 + 1;
                }
                //RIGHT WALL
                if (gameObjects[i].X > this.Width - gameObjects[i].Width)
                {
                    if (i == 0) { objectDirectionsXY[0] *= -1; }
                    gameObjects[i].X = this.Width - gameObjects[i].Width - 1;
                }

                //OBJECTS INTERACTING WITH EACHOTHER; I wont use .Intersects with because these are circles;
                //--instead I want to compare the position of the ball and the other circles by drawing a line between each circle and
                //--looking at if it is smaller than the sum of their radius's. Math.Sprt((x1-x2)^2 + (y1-y2)^2) <= r1+r2
                if (i != 0 && GetLength(gameObjects[0], gameObjects[i]) <= Convert.ToDouble((gameObjects[0].Width / 2) + (gameObjects[i].Width / 2)))
                {
                    if (gameObjects[0].X > gameObjects[i].X + gameObjects[0].Width)
                    {
                        objectDirectionsXY[0] = 1;
                    }
                    if (gameObjects[0].X < gameObjects[i].X)
                    {
                        objectDirectionsXY[0] = -1;
                    }
                    if (gameObjects[0].Y > gameObjects[i].Y + gameObjects[0].Width)
                    {
                        objectDirectionsXY[0 + 3] = 1;
                    }
                    if (gameObjects[0].Y < gameObjects[i].Y)
                    {
                        objectDirectionsXY[0 + 3] = -1;
                    }
                    objectSpeedsXY[0] = objectSpeedsXY[0] / 2 + objectVelocitiesXY[i - 1];
                    objectSpeedsXY[0 + 3] = objectSpeedsXY[0 + 3] / 2 + objectVelocitiesXY[i + 2 - 1];
                }

                //UPDATE OBJECT POSITIONS
                gameObjects[i].X += Convert.ToInt32(objectDirectionsXY[i] * objectSpeedsXY[i]);
                gameObjects[i].Y += Convert.ToInt32(objectDirectionsXY[i + 3] * objectSpeedsXY[i + 3]);
            }



            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(whiteBrush, gameObjects[0]);
            e.Graphics.FillEllipse(redBrush, gameObjects[1]);
            e.Graphics.FillEllipse(blueBrush, gameObjects[2]);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            checkKey(true, e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            checkKey(false, e);
        }


        double GetLength(Rectangle rectangle1, Rectangle rectangle2)
        {

            double x1 = rectangle1.X + (rectangle1.Width / 2);
            double x2 = rectangle2.X + (rectangle2.Width / 2);
            double y1 = rectangle1.Y + (rectangle1.Height / 2);
            double y2 = rectangle2.Y + (rectangle2.Height / 2);

            //A^2 = B^2 + C^2
            double length = Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
            return length;
        }
        void checkKey(bool trueOrFalse, KeyEventArgs e)
        {
            for (int i = 0; i < keysToCheck.Length; i++)
            {
                if (e.KeyCode == keysToCheck[i])
                {
                    WSADUpDownLeftRight[i] = trueOrFalse;
                }
            }
        }
    }
}
