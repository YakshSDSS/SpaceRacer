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
        new List<Rectangle> obstacleList1 = new List<Rectangle>();
        new List<int> speedList1 = new List<int>();
        new List<int> sizeList1 = new List<int>();

        bool upPressed = false;
        bool downPressed = false;
        bool WPressed = false;
        bool SPressed = false;

        //Declare colours to draw players and obstacles.
        SolidBrush blueBrush = new SolidBrush(Color.Cyan);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        Pen goldPen = new Pen(Color.Gold, 10);

        Random randGen = new Random();
        int randValue = 0;

        //Set game variables.
        int player1Score = 0;
        int player2Score = 0;
        int time = 400;

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

        private void SpaceRace_Paint(object sender, PaintEventArgs e)
        { 
            //Draw obstacles.
            for (int i = 0; i < obstacleList1.Count; i++)
            {
                e.Graphics.FillEllipse(whiteBrush, obstacleList1[i]);
            }
            
            //Draw players
            e.Graphics.FillRectangle(blueBrush, player1);
            e.Graphics.FillRectangle(redBrush, player2);

            //Update score display.
            player1ScoreLabel.Text = $"{player1Score}";
            player2ScoreLabel.Text = $"{player2Score}";

            //Draw time line.
            e.Graphics.DrawLine(goldPen, 395, 400, 395, this.Height - time);
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
                obstacleList1.Add(obstacle);
                speedList1.Add(randGen.Next(5, 15));
                sizeList1.Add(randGen.Next(5, 15));
            }
            if (randValue < 16)
            {
                randValue = randGen.Next(0, this.Height - 50);
                Rectangle obstacle = new Rectangle(this.Width - 15, randValue, 0, 0);
                obstacleList1.Add(obstacle);
                speedList1.Add(randGen.Next(-15, -5));
                sizeList1.Add(randGen.Next(5, 15));
            }
            //Remove obstacles which go out of form.
            for (int i = 0; i < obstacleList1.Count; i++)
            {
                if (obstacleList1[i].X > this.Width - 15 || obstacleList1[i].X < 0)
                {
                    obstacleList1.RemoveAt(i);
                    speedList1.RemoveAt(i);
                    sizeList1.RemoveAt(i);
                }
            }
           
            //Reset players if they come in contact with a obstacle.
            for (int i = 0; i < obstacleList1.Count; i++)
            {
                if (player1.IntersectsWith(obstacleList1[i]))
                {
                    player1.X = 200;
                    player1.Y = 360;
                    explosion.Play();
                }
                if (player2.IntersectsWith(obstacleList1[i]))
                {
                    player2.X = 600;
                    player2.Y = 360;
                    explosion.Play();
                }
            }

            //Make obstacles move.
            for (int i = 0; i < obstacleList1.Count; i++)
            {
                int x = obstacleList1[i].X + speedList1[i];
                obstacleList1[i] = new Rectangle(x, obstacleList1[i].Y, sizeList1[i], sizeList1[i]);
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
            //End game once time runs out and declare the winner or if it is a tie.
            if (time == 0 && player1Score > player2Score)
            {
                winnerLabel.Text = "Player 1 Wins";
                gameOver.Play();
                gameTimer.Stop();
            }
            else if (time < 0 && player1Score < player2Score)
            {
                winnerLabel.Text = "Player 2 Wins";
                gameTimer.Stop();
            }
            else if (time < 0 && player1Score == player2Score)
            {
                winnerLabel.Text = "Tie";
                gameTimer.Stop();
            }

            Refresh();
        }
    }
}
