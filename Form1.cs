using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using Dapper;


namespace DapperCRUD
{

    public partial class Form1 : Form
    {
      
        Bitmap playerImg, fadeImg, shplatImg, midplatImg, wideplatImg, sqplatImg, winImg, null_fade_img;
        Bitmap[] rightimg, leftimg;
        bool goleft = false;
        bool goright = false;
        bool jumping = false;
        bool onGround = false;
        static int jumpHeight = 15, fade_offset_x = 16, fade_offset_y = 0;
        int jumpspeed = jumpHeight;
        int currFrame = 0;
        uint score = 0;
        uint maxScore = 0;
        int windowsHeight, windowsWidth, win1_4, hei1_4;
        int[] Side = new int[2];
        int[] AbsSide = new int[2];
        int[] SignSide = new int[2];


        double temp;

        WindowsMediaPlayer bg_sound = new WindowsMediaPlayer();
        WindowsMediaPlayer win_sound = new WindowsMediaPlayer();
        WindowsMediaPlayer grass_sound = new WindowsMediaPlayer();
        WindowsMediaPlayer jump_sound = new WindowsMediaPlayer();
        WindowsMediaPlayer star_sound = new WindowsMediaPlayer();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public Form1()
        { 
            InitializeComponent();
            fadeImg = new Bitmap("fade.png");
            playerImg = new Bitmap("player.png");
            shplatImg = new Bitmap("sh_platform.png");
            midplatImg = new Bitmap("mid_platform.png");
            wideplatImg = new Bitmap("wide_platform.png");
            sqplatImg = new Bitmap("sq_platform.png");
            null_fade_img = new Bitmap("null_fade.png");
            platform1.Image = midplatImg;
            platform2.Image = midplatImg;
            platform3.Image = midplatImg;
            platform4.Image = midplatImg;
            platform5.Image = sqplatImg;
            platform6.Image = sqplatImg;
            platform7.Image = sqplatImg;
            platform8.Image = midplatImg;
            platform9.Image = midplatImg;
            platform10.Image = midplatImg;
            platform11.Image = midplatImg;
            sh_platform1.Image = shplatImg;
            sh_platform2.Image = shplatImg;
            sh_platform3.Image = shplatImg;
            wide_platform_1.Image = wideplatImg;
            wide_platform_2.Image = wideplatImg;
            winImg = new Bitmap("win.png");

            bg_sound.URL = "Worldmap Theme.wav";
            bg_sound.controls.stop();

            win_sound.URL = "Iceland Theme.wav";
            win_sound.controls.stop();

            grass_sound.URL = "grass.wav";
            grass_sound.controls.stop();
            grass_sound.settings.volume = 40;

            jump_sound.URL = "jump.wav";
            jump_sound.controls.stop();
            jump_sound.settings.volume = 50;

            star_sound.URL = "star.wav";
            star_sound.controls.stop();


            bg_sound.settings.volume = 30;
            bg_sound.settings.setMode("loop", true);
            bg_sound.controls.play();

            winBox.Visible = false;

            rightimg = new Bitmap[6];
            leftimg = new Bitmap[6];
            for (int i = 0; i < 6; ++i)
            {
                rightimg[i] = playerImg.Clone(new Rectangle(new Point(32 * i + 4, 0), new Size(26, 26)), playerImg.PixelFormat);
                leftimg[i] = playerImg.Clone(new Rectangle(new Point(32 * i + 4, 26), new Size(26, 26)), playerImg.PixelFormat);
            }

            drawPlayer(1);

            foreach(Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag == "star")
                    maxScore++;
                
            }
            windowsHeight = Size.Height;
            windowsWidth = Size.Width;
            win1_4 =  windowsWidth / 4;
            hei1_4 =  windowsHeight / 4;


        }



        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) goleft = true;
            if (e.KeyCode == Keys.D) goright = true;
            if (e.KeyCode == Keys.W && !jumping && onGround)
            {
                jump_sound.controls.play();
                jumpspeed = 10;
                jumping = true;
                onGround = false;
                grass_sound.controls.stop();
            }


        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) goleft = false;
            if (e.KeyCode == Keys.D) goright = false;
            if (e.KeyCode == Keys.W && jumping)
            {
                jumping = false;
            }
        }


        private void drawFade(int ch)
        {
            if (ch == 1)
            {
                Image part = new Bitmap(32, 20);
                Graphics g = Graphics.FromImage(part);
                g.DrawImage(fadeImg, 0, 0, new Rectangle(new Point(32 * currFrame, 0), new Size(32, 26)), GraphicsUnit.Pixel);
                fade.Size = new Size(32, 20);
                fade.Visible = true;
                fade.Image = part;
            }
            if (ch == 2)
            {
                Image part = new Bitmap(32, 20);
                Graphics g = Graphics.FromImage(part);
                g.DrawImage(fadeImg, 0, 0, new Rectangle(new Point(32 * currFrame, 26), new Size(32, 26)), GraphicsUnit.Pixel);
                fade.Size = new Size(32, 20);
                fade.Visible = true;
                fade.Image = part;
            }
            if (ch == 0) { 
                fade.Visible = false; }
        }
        private void drawPlayer(int direction)
        {
          
            if (direction == 1)
            {
                player.Image = rightimg[currFrame];
            }
            else
            {
                player.Image = leftimg[currFrame];
            }
        }
       

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (player.Top + player.Height > wide_platform_1.Top) player.Top = wide_platform_1.Top - player.Height - 1; 

            if (jumping) {
                jumping = false;
                jumpspeed = jumpHeight;
                drawFade(0);
               
            }
            player.Top -= jumpspeed;
            if (jumpspeed > -1.5*jumpHeight) jumpspeed--;
            if (!goright && !goleft)
            { currFrame = 0; drawFade(0); }
            else 
            {
                if (goleft) { player.Left -= 5; fade_offset_x = -8; fade_offset_y = 1; drawPlayer(0); currFrame++; }
                else { player.Left += 5; fade_offset_x = 8; fade_offset_y = 1; drawPlayer(1); currFrame++;  }
            }
            if (currFrame > 5) currFrame = 0;
            Side[0] = (player.Left + (player.Width - windowsWidth) / 2);
            Side[1] = (player.Top - windowsHeight / 2);
            AbsSide[0] = Math.Abs(Side[0]);
            AbsSide[1] = Math.Abs(Side[1]);
            SignSide[0] = Math.Sign(Side[0]);
            SignSide[1] = Math.Sign(Side[1]);

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag != "cloud" || x is TransparentControl && x.Tag != "fade")
                {                  
                    if (AbsSide[0] > win1_4)
                    {
                        if (x.Tag != "player") x.Left -= 6 * SignSide[0];
                        else {
                            temp = (AbsSide[0] * 1.0) / (win1_4 * 1.0) - 1.0; x.Left -= SignSide[0]*(Convert.ToInt32(20.0*(temp))+2); }
                    }
                   
                    if (AbsSide[1] > hei1_4)
                    {
                        if (x.Tag != "player") x.Top -= 6 * SignSide[1];
                        else
                            { temp = (AbsSide[1] * 1.0) / (hei1_4 * 1.0) - 1.0; x.Top -= SignSide[1] * (Convert.ToInt32(10.0 * (temp)) + 2); }
                    }

                    if (x.Tag == "platform")
                    {
                        //drawPlatform(x);
                        Rectangle marioRect = player.Bounds;
                        Rectangle blockRect = x.Bounds;
                        Rectangle intersection = Rectangle.Intersect(marioRect, blockRect);

                        if (intersection.Height > intersection.Width)
                        {
                            if (marioRect.Right > blockRect.Left && marioRect.Right < blockRect.Right)
                            {
                                player.Left -= intersection.Width; //левая
                            }
                            else
                            {
                                player.Left += intersection.Width; //правая
                            }
                        }
                        else if (intersection.Height < intersection.Width)
                        {
                            if (marioRect.Bottom > blockRect.Top && marioRect.Bottom < blockRect.Bottom)
                            {
                                if (!jumping) //вехрняя
                                {
                                    onGround = true;
                                    
                                    jumpspeed = 0; //поставь 10 и он будет сам прыгать от поверхностей
                                    if ((goleft || goright))
                                        grass_sound.controls.play();
                                    if (!goright && !goleft) { 
                                        drawFade(0);
                                        grass_sound.controls.stop(); 
                                        }
                                    else
                                    {
                                        if (goleft) { drawFade(2);}
                                        else { drawFade(1);  }
                                    }
                                        fade.Top = player.Top - fade_offset_y;
                                    fade.Left = player.Left - fade_offset_x;
                                }
                                player.Top -= intersection.Height;
                            }
                            if (marioRect.Top > blockRect.Top && marioRect.Top < blockRect.Bottom)
                            {
                                player.Top = blockRect.Bottom;
                                jumpspeed -= jumpHeight;
                            }

                        }
                    }
                    
                }

                if (x is PictureBox && x.Tag == "door")
                {
                    if(player.Bounds.IntersectsWith(x.Bounds) && score == maxScore)
                    {
                        
                        bg_sound.controls.stop();
                        grass_sound.controls.stop();

                        winBox.Left = (/*winBox.Left +*/ (windowsWidth - winBox.Width) / 2);
                        winBox.Top = winBox.Top - windowsHeight / 2;
                        winBox.Visible = true;

                       
                        win_sound.controls.play();

                        timer1.Stop();
                        player.Visible = false;
                        fade.Visible = false;
                        
                    }
                }

                if (x is PictureBox && x.Tag == "star")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        
                       
                        star_sound.controls.play();
                        score++;
                        Controls.Remove(x);
                        ScoreBox.Text = $"Счет: {score}";
                    }
                }
            }
        }
    }
}
