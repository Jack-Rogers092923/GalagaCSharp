using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicControls
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // double buffer
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        // movement
        bool l = false;
        bool r = false;
        bool er = false;
        bool c = false;

        // location 
        int x = 0;
        int y = 0;

        // speed
        int speed = 5;
        int playerSpeed = 5;
        // player location on collision
        int px;
        int py;

        // ammo
        int quiver = 10;

        // create random event for enemy to fire projectile
        Random gen = new Random();

        // score
        int score = 0;
        int level = 0;
        // create series of labels
        List<Label> projectiles = new List<Label>();
        List<PictureBox> enemies = new List<PictureBox>();
        List<Label> enemyProjectiles = new List<Label>();
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // if space is pressed fire projectile
            if (e.KeyCode == Keys.Space)
            {
                CreateProjectile();
                System.IO.Stream str = Properties.Resources.pew;
                System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                snd.Play();
            }

            // move left and right
            if (e.KeyCode == Keys.D) { r = true; }
            if (e.KeyCode == Keys.A) { l = true; }
            if (e.KeyCode == Keys.C) { c = true; }
            if (e.KeyCode == Keys.R) { quiver = 10; progressBar1.Value = quiver; }
        }
        private void CreateProjectile()
        {
            if (quiver > 0) {
                // create the projectile
                Label p = new Label();
                p.Size = new Size(5, 10);
                p.Location = new Point(pbPlayer.Location.X + 27, pbPlayer.Location.Y);
                p.BackColor = Color.MediumVioletRed;
                Controls.Add(p);
                projectiles.Add(p);
                quiver--;
                progressBar1.Value = quiver;
            }
        }
        private void SpawnEnemy()
        {
            bool topRow = true;
            for (int i = 0; i < 14; i++)
            {
                PictureBox enemy = new PictureBox();
                enemy.Size = new Size(75, 75);
                if (topRow)
                {
                    enemy.Location = new Point(x, y);
                    y += 100; topRow = false;
                }
                else
                {
                    enemy.Location = new Point(x, y);
                    y -= 100; topRow = true; x += 90;
                }
                enemy.Image = Properties.Resources.enemy;
                enemy.SizeMode = PictureBoxSizeMode.Zoom;

                Controls.Add(enemy);
                enemies.Add(enemy);
               
         
            }
            // create the enemy

            
        }
        private void tmrMovement_Tick(object sender, EventArgs e)
        {
            // px & py equal players current location
            px = pbPlayer.Location.X;
            py = pbPlayer.Location.Y;

            // move left and right
            if (l)
            { pbPlayer.Left -= playerSpeed; }
            if (r)
            { pbPlayer.Left += playerSpeed; }
            if (c)
            { playerSpeed = 25; ;}

            // if player collides with the left wall bound player cannot move passed it
            if (pbPlayer.Bounds.IntersectsWith(pbLeft.Bounds))
            {pbPlayer.Location = new Point(px, py);}

            // if player collides with right wall bounds player cannot move passed it
            if (pbPlayer.Bounds.IntersectsWith(pbRight.Bounds))
            {pbPlayer.Location = new Point(px, py);}
        }

        private void tmrProjectile_Tick(object sender, EventArgs e)
        {
            // enemy will fire back every 2-4 seconds
            // 10/1000th second integer 
            // every second, timer runs 100x

           
            int r = gen.Next(200);
            if ( r == 75)
            {
                CreateEnemyProjectle();
            }

            // manage projectile movement
            for (int j = 0; j < projectiles.Count; j++)
            {
                projectiles[j].Top -= speed;
                if (projectiles[j].Location.Y < 0)
                {
                   // remove projectile if location is less than zero
                    Controls.Remove(projectiles[j]);
                    projectiles[j].Dispose();
                    projectiles.RemoveAt(j);
                    j--;
                }
            }
            for (int i = 0; i < enemyProjectiles.Count; i++)
            {
                enemyProjectiles[i].Top += speed;
                if (enemyProjectiles[i].Bounds.IntersectsWith(pbPlayer.Bounds))
                {
                   
                    tmrProjectile.Stop();
                    tmrMovement.Stop();
                    pbPlayer.Visible = false;
                    DialogResult dialogResult = MessageBox.Show("You lose with a score of " + score + " points" + " and made it to level " + level + ". Would you like to play again?", "Play Again?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Application.Restart();
                        //this.Close();
                        //this.Refresh();
                        this.InitializeComponent();

                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        this.Close();
                    }
                   
                }
                if (enemyProjectiles[i].Location.Y < 0)
                {
                    // remove projectile if location is less than zero
                    Controls.Remove(enemyProjectiles[i]);
                    enemyProjectiles[i].Dispose();
                    enemyProjectiles.RemoveAt(i);
                    i--;
                }
            }

            // Enemy movements
            // Iterate over the enemies
            // Create an int variable to track which enemy is intersecting with the bound
            // create boolean to make if all 7 are visible then they all start to move
            for (int i = 0; i < enemies.Count; i++)
              {
                // If there is an enemy intersecting with the bound, move it in the opposite direction
                //if (enemies[i].Bounds.IntersectsWith(pbLeft.Bounds)) { er = true;  break; /*lr = false;*/ }
               // if (enemies[i].Bounds.IntersectsWith(pbRight.Bounds)) { er = false; break; /*lr = true;*/ }
                if (er) { enemies[i].Left += speed; }
                else { enemies[i].Left -= speed; }
               
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Bounds.IntersectsWith(pbLeft.Bounds)) { er = true; break; /*lr = false;*/ }
                if (enemies[i].Bounds.IntersectsWith(pbRight.Bounds)) { er = false; break; /*lr = true;*/ }
               
            }
           
                
            
            
            
            // manage collisions
            try
            {
                for (int i = 0; i < projectiles.Count; i++)
                {
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        
                        if (enemies[j].Bounds.IntersectsWith(projectiles[i].Bounds))
                        {
                            System.IO.Stream str = Properties.Resources.kaboom;
                            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                            snd.Play();
                            // remove enemy if it collides with projectile
                            Controls.Remove(enemies[j]);
                            enemies[j].Dispose();
                            enemies.RemoveAt(j);
                            j--; 
                            Controls.Remove(projectiles[i]);
                            projectiles[i].Dispose();
                            projectiles.RemoveAt(i);
                            i--;
                            score += 100;
                            lblScore.Text = "Score: " + score.ToString();
                            
                            
                            // no enemies remain message box show that you win (Temp)
                        }
                       
                    }
                    
                }

            }
            catch
            {

            }
            if (enemies.Count <= 0)
            {

                tmrProjectile.Stop();
                tmrMovement.Stop();

                MessageBox.Show("Advancing");


                tmrProjectile.Start();
                tmrMovement.Start();
                this.projectiles.Clear();

                x = 0;
                y = 0;
                SpawnEnemy();
                level += 1;
                lblLevel.Text = "Level: " + level.ToString();
            } 
            // ends enemy movement
        }
    private void CreateEnemyProjectle()
        {
            Label ep = new Label();
            ep.Size = new Size(10, 10);
            int randship = gen.Next(enemies.Count - 1);
            int shipX = enemies[randship].Location.X + enemies[randship].Width / 2 - 5;
            int shipY = enemies[randship].Location.Y;
            ep.SendToBack();
            ep.Location = new Point(shipX, shipY);
            ep.BackColor = Color.Green;
            Controls.Add(ep);
            enemyProjectiles.Add(ep);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // stop moving if key goes up
            if (e.KeyCode == Keys.D) { r = false; }
            if (e.KeyCode == Keys.A) { l = false; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SpawnEnemy();
        }

        private void tmrEnemySpawn_Tick(object sender, EventArgs e)
        {
          
        }
    }
}
