using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace SpaceRacer
{
    public partial class SpaceRace : Form
    {
        //Set variables for players.
        Rectangle player1 = new Rectangle(200, 360, 20, 20);
        Rectangle player2 = new Rectangle(600, 360, 20, 20);
        int playerSpeed = 4;

        //Lists required for obstacles going left to right.
        new List<Rectangle> obstacleList = new List<Rectangle>();
        new List<int> speedList = new List<int>();
        new List<int> sizeList = new List<int>();

        bool upPressed = false;
        bool downPressed = false;
        bool WPressed = false;
        bool SPressed = false;

        //Declare colours to draw players and obstacles.
        SolidBrush orangeBrush = new SolidBrush(Color.OrangeRed);
        SolidBrush greenBrush = new SolidBrush(Color.Lime);
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        Pen goldPen = new Pen(Color.Gold, 10);

        Random randGen = new Random();
        int randValue = 0;

        //Set game variables.
        int player1Score = 0;
        int player2Score = 0;
        int time = 400;

        //Put winner in a string to display in paint method.
        string winner;

        //Set sounds.
        SoundPlayer pointEarned = new SoundPlayer(Properties.Resources.NewPointSound);
        SoundPlayer explosion = new SoundPlayer(Properties.Resources.Explosion);
        SoundPlayer gameOver = new SoundPlayer(Properties.Resources.game_over);

        public SpaceRace()
        {  
            InitializeComponent();
        }

        private void SpaceRace_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    upPressed = true;
                    break;
                case Keys.Down:
                    downPressed = true;
                    break;
                case Keys.W:
                    WPressed = true;
                    break;
                case Keys.S:
                    SPressed = true;
                    break;
                case Keys.Space:
                    if (gameTimer.Enabled == false)
                    {
                        InitializeGame();
                    }
                    break;
                case Keys.Escape:
                    if (gameTimer.Enabled == false)
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void SpaceRace_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    upPressed = false;
                    break;
                case Keys.Down:
                    downPressed = false;
                    break;
                case Keys.W:
                    WPressed = false;
                    break;
                case Keys.S:
                    SPressed = false;
                    break;
            }
        }

        public void InitializeGame()
        {
            //Reset everything.
            titleLabel.Text = "";
            infoLabel.Text = "";

            gameTimer.Enabled = true;

            time = 400;
            player1Score = 0;
            player2Score = 0;

            obstacleList.Clear();
            sizeList.Clear();
            speedList.Clear();

            player1 = new Rectangle(200, 360, 20, 20);
            player2 = new Rectangle(600, 360, 20, 20);
        }

        private void SpaceRace_Paint(object sender, PaintEventArgs e)
        {
            if (gameTimer.Enabled == false && time > 0)
            {
                //Draw start screen.
                titleLabel.Text = "SPACE RACER";
                infoLabel.Text = "Press Space to Start or Esc to Exit";
            }
            else if (gameTimer.Enabled == true)
            {
                //Update score display.
                player1ScoreLabel.Text = $"{player1Score}";
                player2ScoreLabel.Text = $"{player2Score}";

                //Draw players
                e.Graphics.FillRectangle(greenBrush, player1);
                e.Graphics.FillRectangle(orangeBrush, player2);

                //Draw time line.
                e.Graphics.DrawLine(goldPen, 395, 400, 395, this.Height - time);

                //Draw obstacles.
                for (int i = 0; i < obstacleList.Count; i++)
                {
                    e.Graphics.FillEllipse(whiteBrush, obstacleList[i]);
                }
            }
            else
            {
                //Clear scores and display winner.
                player1ScoreLabel.Text = "";
                player2ScoreLabel.Text = "";

                titleLabel.Text = $"{winner}";
                infoLabel.Text = "Press Space to Start or Esc to Exit";
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //Subtract time to make game stop.
            time--;

            //Move Player 1 up and down. Also set the boundaries.
            if (WPressed == true && player1.Y > 0)
            {
                player1.Y = player1.Y - playerSpeed;
            }
            if (SPressed == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y = player1.Y + playerSpeed;
            }
            //Move Player 2 up and down. Also set the boundaries.
            if (upPressed == true && player2.Y > 0)
            {
                player2.Y = player2.Y - playerSpeed;
            }
            if (downPressed == true && player2.Y < this.Height - player2.Height)
            {
                player2.Y = player2.Y + playerSpeed;
            }

            //Add random balls, half going right to left and other half going left to right.
            //Randomize the size and speed of the balls too.
            randValue = randGen.Next(0, 101);
            if (randValue < 8)
            {
                randValue = randGen.Next(0, this.Height - 50);
                Rectangle obstacle = new Rectangle(0, randValue, 0, 0);
                obstacleList.Add(obstacle);
                speedList.Add(randGen.Next(5, 15));
                sizeList.Add(randGen.Next(5, 15));
            }
            else if (randValue < 16)
            {
                randValue = randGen.Next(0, this.Height - 50);
                Rectangle obstacle = new Rectangle(this.Width - 15, randValue, 0, 0);
                obstacleList.Add(obstacle);
                speedList.Add(randGen.Next(-15, -5));
                sizeList.Add(randGen.Next(5, 15));
            }
            //Remove obstacles which go out of form.
            for (int i = 0; i < obstacleList.Count; i++)
            {
                if (obstacleList[i].X > this.Width - 15 || obstacleList[i].X < 0)
                {
                    obstacleList.RemoveAt(i);
                    speedList.RemoveAt(i);
                    sizeList.RemoveAt(i);
                }
            }
           
            //Reset players if they come in contact with a obstacle.
            for (int i = 0; i < obstacleList.Count; i++)
            {
                if (player1.IntersectsWith(obstacleList[i]))
                {
                    player1.X = 200;
                    player1.Y = 360;
                    explosion.Play();
                    obstacleList.RemoveAt(i);
                    speedList.RemoveAt(i);
                    sizeList.RemoveAt(i);
                }
                if (player2.IntersectsWith(obstacleList[i]))
                {
                    player2.X = 600;
                    player2.Y = 360;
                    explosion.Play();
                    obstacleList.RemoveAt(i);
                    sizeList.RemoveAt(i);
                    speedList.RemoveAt(i);
                }
            }

            //Make obstacles move.
            for (int i = 0; i < obstacleList.Count; i++)
            {
                int x = obstacleList[i].X + speedList[i];
                obstacleList[i] = new Rectangle(x, obstacleList[i].Y, sizeList[i], sizeList[i]);
            }
           
            //Give points to player 1.
            if (player1.Y == 0)
            {
                player1Score++;
                player1.X = 200;
                player1.Y = 360;
                pointEarned.Play();
            }
            //Give points to player 2.
            if (player2.Y == 0)
            {
                player2Score++;
                player2.X = 600;
                player2.Y = 360;
                pointEarned.Play();
            }
            //End game once time runs out and store winner or a tie in a string.
            if (time == 0 && player1Score > player2Score)
            {
                winner = "Player 1 Wins!";
                gameOver.Play();
                gameTimer.Stop();
            }
            else if (time < 0 && player1Score < player2Score)
            {
                winner = "Player 2 Wins";
                gameTimer.Stop();
            }
            else if (time < 0 && player1Score == player2Score)
            {
                winner = "Tie";
                gameTimer.Stop();
            }

            Refresh();
        }
    }
}
