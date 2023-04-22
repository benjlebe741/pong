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

namespace pong
{
    public partial class Form1 : Form
    {
        //KEYDOWN/KEYUP LISTS:
        bool[] WSADUpDownLeftRight = new bool[] { false, false, false, false, false, false, false, false };
        Keys[] keysToCheck = new Keys[] { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Up, Keys.Down, Keys.Left, Keys.Right };

        int[][] determineDirectionsList = new int[][] { };

        //BRUSHES & PENS
        new SolidBrush blueBrush = new SolidBrush(Color.LightBlue);
        new SolidBrush whiteBrush = new SolidBrush(Color.White);
        new Pen whitePen = new Pen(Color.White, 5);


        //GAME OBJECTS
        Rectangle[] gameObjects = new Rectangle[] //0 = ball, 1 = player1, 2 = player2
        {
        new Rectangle(480, 240, 20, 20),
        new Rectangle(0, 320, 20, 80),
        new Rectangle(0, 80, 20, 80),
        };

        Point[] locationList = new Point[] { 
            new Point(480, 240),
            new Point(0, 320),  
            new Point(0, 80),
        };

        //TRACKING OBJECT INFORMATION: 0 = BALL 1 = PLAYER1 2 = PLAYER2
        int[] objectDirectionsY = new int[] { -1, 0, 0 };
        int[] objectDirectionsX = new int[] { -1, 0, 0 };
        int[] objectSpeeds = new int[] { 3, 3, 3 };
        int[] playerScores = new int[] { 0, 0 };
        int currentPlayer = 2;
        public Form1()
        {
            InitializeComponent();
            determineDirectionsList = new int[][]
          {
      new int[]{  0,1,objectDirectionsY[1]},
      new int[]{  2,3,objectDirectionsX[1]},
      new int[]{  4,5,objectDirectionsY[2]},
      new int[]{  6,7,objectDirectionsX[2]},
          };
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++) {
                if (playerScores[i] > 2) 
                {
                    winLabel.Text = $"PLAYER {i + 1} IS THE VICTOR!";
                    gameTimer.Enabled = false; 
                }
            }


            //Determening all player directions:
            for (int i = 0; i < determineDirectionsList.Length; i++)
            {
                if (WSADUpDownLeftRight[determineDirectionsList[i][0]] == true && WSADUpDownLeftRight[determineDirectionsList[i][1]] == false) { determineDirectionsList[i][2] = -1; }
                else if (WSADUpDownLeftRight[determineDirectionsList[i][1]] == true && WSADUpDownLeftRight[determineDirectionsList[i][0]] == false) { determineDirectionsList[i][2] = 1; }
                else { determineDirectionsList[i][2] = 0; }
            }

            objectDirectionsY[1] = determineDirectionsList[0][2];
            objectDirectionsX[1] = determineDirectionsList[1][2];
            objectDirectionsY[2] = determineDirectionsList[2][2];
            objectDirectionsX[2] = determineDirectionsList[3][2];

            for (int i = 0; i <= 2; i++)
            {
                //IF ANY OBJECTS ARE HITTING THE WALLS; PUSH THEM BACK
                //TOP WALL
                if (gameObjects[i].Y <= 0)
                {
                    if (i == 0) { objectDirectionsY[0] *= -1; }
                    gameObjects[i].Y = 0 + 1;
                }
                //BOTTOM WALL
                if (gameObjects[i].Y >= this.Height - gameObjects[i].Height)
                {
                    if (i == 0) { objectDirectionsY[0] *= -1; }
                    gameObjects[i].Y = this.Height - gameObjects[i].Height - 1;
                }
                //LEFT WALL
                if (gameObjects[i].X < 0)
                {
                    if (i == 0)
                    {
                        updateScore(currentPlayer - 1);
                        //increase the speed of the ball
                        objectSpeeds[0] = 3;

                        for (int j = 0; j < 3; j++) { gameObjects[j].Location = locationList[j]; }
                    }
                    else
                    {
                        gameObjects[i].X = 0 + 1;
                    }
                }
                //RIGHT WALL
                if (gameObjects[i].X > this.Width - gameObjects[0].Width)
                {
                    if (i == 0) { objectDirectionsX[0] *= -1; }
                    gameObjects[i].X = this.Width - gameObjects[0].Width - 1;
                }

                //OBJECTS INTERACTING WITH EACHOTHER
                if (gameObjects[0].IntersectsWith(gameObjects[i]) && i == currentPlayer && objectDirectionsX[0] == -1)
                {
                    objectDirectionsX[0] *= -1;
                    gameObjects[0].X = gameObjects[i].X + gameObjects[i].Width;
                    objectSpeeds[0]++;
                    if (i == 1) { currentPlayer = 2; }
                    else { currentPlayer = 1; }
                }
                //UPDATE OBJECT POSITIONS
                gameObjects[i].X += objectDirectionsX[i] * objectSpeeds[i];
                gameObjects[i].Y += objectDirectionsY[i] * objectSpeeds[i];
            }



            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(whiteBrush, gameObjects[0]);
            e.Graphics.FillRectangle(blueBrush, gameObjects[1]);
            e.Graphics.FillRectangle(blueBrush, gameObjects[2]);
            e.Graphics.DrawRectangle(whitePen, gameObjects[currentPlayer]);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            checkKey(true, e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            checkKey(false, e);
        }

        void updateScore(int player)
        {
            playerScores[player]++;
            player1ScoreLabel.Text = $"PLAYER 1:  {playerScores[1]}";
            player2ScoreLabel.Text = $"PLAYER 2:  {playerScores[0]}";
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
