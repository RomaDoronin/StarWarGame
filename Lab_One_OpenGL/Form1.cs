using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_One_OpenGL
{
    public partial class Form1 : Form
    {
        GLgraphics glgraphics = new GLgraphics();

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            glgraphics.Setup(glControl1.Width, glControl1.Height);
            Application.Idle += Application_Idle;
            
            int[] texID = new int[150];

            texID[0] = glgraphics.LoadTexture("logo_ITMM.png");
            glgraphics.texturesIDs.Add(texID[0]);

            texID[1] = glgraphics.LoadTexture("Sun.jpg");
            glgraphics.texturesIDs.Add(texID[1]);
            texID[2] = glgraphics.LoadTexture("Меркурий.png");
            glgraphics.texturesIDs.Add(texID[2]);
            texID[3] = glgraphics.LoadTexture("Венера.png");
            glgraphics.texturesIDs.Add(texID[3]);
            texID[4] = glgraphics.LoadTexture("Земля.jpg");
            glgraphics.texturesIDs.Add(texID[4]);
            texID[5] = glgraphics.LoadTexture("Атмосфера.png");
            glgraphics.texturesIDs.Add(texID[5]);
            texID[6] = glgraphics.LoadTexture("Луна.png");
            glgraphics.texturesIDs.Add(texID[6]);
            texID[7] = glgraphics.LoadTexture("Марс.png");
            glgraphics.texturesIDs.Add(texID[7]);
            texID[8] = glgraphics.LoadTexture("Юпитер.png");
            glgraphics.texturesIDs.Add(texID[8]);
            texID[9] = glgraphics.LoadTexture("Сатурн.png");
            glgraphics.texturesIDs.Add(texID[9]);
            
            texID[10] = glgraphics.LoadTexture("kosmos.jpg");
            glgraphics.texturesIDs.Add(texID[10]);

            texID[11] = glgraphics.LoadTexture("Звезда.jpg");
            glgraphics.texturesIDs.Add(texID[11]);
            texID[12] = glgraphics.LoadTexture("Angry.jpg");
            glgraphics.texturesIDs.Add(texID[12]);
            texID[13] = glgraphics.LoadTexture("Boom.jpg");
            glgraphics.texturesIDs.Add(texID[13]);

            // Комната
            texID[14] = glgraphics.LoadTexture("Door.png");
            glgraphics.texturesIDs.Add(texID[14]);
            texID[15] = glgraphics.LoadTexture("Sex.jpg");
            glgraphics.texturesIDs.Add(texID[15]);
            texID[16] = glgraphics.LoadTexture("flor.jpg");
            glgraphics.texturesIDs.Add(texID[16]);
            texID[17] = glgraphics.LoadTexture("Wall.jpg");
            glgraphics.texturesIDs.Add(texID[17]);
        }

             
       private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glgraphics.Update();
            glControl1.SwapBuffers();

            glgraphics.sum = 1;
            for (int i = 0; i < 8; i++)
                glgraphics.sum *= glgraphics.Game[i];
            glgraphics.sum += glgraphics.Game[8];

            if ((glgraphics.sum > 10) && (glgraphics.sum < 100))
                {
                    label5.Text = "YOU LOST";
                }
            else if (glgraphics.sum == 1)
                {
                    label5.Text = "YOU WIN";
                }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            float widthCoef = (-e.X + glControl1.Width * 0.5f) / (float)glControl1.Width;
            float heightCoef = (-e.Y + glControl1.Height * 0.5f) / (float)glControl1.Height;
            glgraphics.latitude = heightCoef * 180;
            glgraphics.longitude = widthCoef * 360;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
                glControl1.Refresh();
        }

        private void glControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (glgraphics.flag != 10)
            {
                glgraphics.rotateB[glgraphics.flag] = 0;
                glgraphics.flag += 1;
            }
            else
                label4.Text = "RECHARGE! Press R";

            String s = (10 - glgraphics.flag).ToString();

            label1.Text = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (glgraphics.Cheker > 1) glgraphics.Cheker--;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (glgraphics.Cheker < 2) glgraphics.Cheker++;
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            int num;
            num = (int)e.KeyCode;
            if (num == 83) // S
            {
                glgraphics.Straight(-0.05f);
            }
            if (num == 87) // W
            {
                glgraphics.Straight(0.05f);
            }
            if (num == 82) // R
            {
                label4.Text = "";
                glgraphics.flag = 0;
                label1.Text = "10";
            }
            if (num == 68) // D
            {
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
