using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace Lab_One_OpenGL
{
    class GLgraphics
    {
        public int flag = 0; // Колличесто выпущенных снаярдов
        public float DistanceCentrel = 0.1f; // Расстояние корабля до центра
        public const int MaxShots = 10; // Размер "магазина" коробля
        public float[] rotateB = new float[MaxShots]; // Счетчик для выстрела
        public float rotateAngle;
        public double[] rotateD = new double[25]/* = 0.03*/; // Счетчик для взрыва
        public List<int> texturesIDs = new List<int>(); // Список текстур
        public int sum = 1;

        public int[] Game = {0, 0, 0, 0, 0, 0, 0, 0, 0};

        // Массивы, хранящие определители координат снарядов
        public float[] pX = new float[MaxShots];
        public float[] pY = new float[MaxShots];
        public float[] pZ = new float[MaxShots];

        // Массивы, хранящие определители координат кораблей
        public float[] psX = new float[25];
        public float[] psY = new float[25];
        public float[] psZ = new float[25];

        //private int[] ChekTarget = new int[25]; // Идентификатор попадания
        private int[] ChekTarget = { 1, 1, 1, 1, 1, 1, 1, 1,
                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        Vector3 cameraPosition = new Vector3(2, 2, 1.2f); //Позиция камеры
        Vector3 cameraDirecton = new Vector3(2, 3, 1); //Направление камеры
        Vector3 cameraUp = new Vector3(0, 0, 1);

        //!!!!!!!!!!!!!!!!!!!!
        Vector3 ShipPosition = new Vector3(2, 2, 2); // Позиция камеры
        Vector3 ShotPosition = new Vector3(2, 2, 2); // Позиция снаряда
        Vector3 SightPosition = new Vector3(2, 2, 2); // Позиция прицела
        //Vector3 ShipDirecton = new Vector3(0, 0, 0); //Направление корабля


        public float latitude = 47.98f;
        public float longitude = 60.41f;
        public float radius = 15.385f; // Радиус по которому перемещается Direction

        public void Setup(int width, int height)
        {
            GL.ClearColor(Color.DarkGray); //Заливает буфер экрана одним цветом
            GL.ShadeModel(ShadingModel.Smooth); //Устанавливает тип отрисовки полигонов с оттенками
            GL.Enable(EnableCap.DepthTest); //Включает буфер глубины

            Matrix4 perspectiveMat = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                width / (float)height, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspectiveMat);
            SetupLightning();
        }

        public void Update()
        {
            rotateAngle += 0.1f;

            //Функция отчищает буферы
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Управление камерой
            Matrix4 viewMat = Matrix4.LookAt(cameraPosition, cameraDirecton, cameraUp);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref viewMat);
            Render();

            /*cameraPosition*/ cameraDirecton = new Vector3(
                (float)(radius * Math.Cos(Math.PI / 180.0f * latitude) * Math.Cos(Math.PI / 180.0f * longitude)),
                (float)(radius * Math.Cos(Math.PI / 180.0f * latitude) * Math.Sin(Math.PI / 180.0f * longitude)),
                (float)(radius * Math.Sin(Math.PI / 180.0f * latitude)));

            //cameraDirecton = ShipDirecton;
        }

        // Функция для подсчета координат движения
        public void Straight(float l)
        {
            // Координаты направленного вектора
            float pX = cameraDirecton.X - cameraPosition.X;
            float pY = cameraDirecton.Y - cameraPosition.Y;
            float pZ = cameraDirecton.Z - cameraPosition.Z;

            // Вычисление позиции Камеры
            cameraPosition.X += (l * pX) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ);
            cameraPosition.Y += (l * pY) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ);
            cameraPosition.Z += (l * pZ) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ);

            // Вычисление позиции, куда камера смотрит
            cameraDirecton.X += (l * pX) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ);
            cameraDirecton.X += (l * pY) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ);
            cameraDirecton.X += (l * pZ) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ);
        }

        public void Render()
        {
                // Солнечная система
                DrawSolSystem();
                // Фон для SolSystem
                drawSphere(40, 20, 20, 10);
                CallSpaceShip();
                DrawSight();

                // Рисование Космических корабля!
                if ((DistanceCentrel > -1) && (DistanceCentrel < 45) /*|| (DistanceCentrel > 50)*/)
                {
                    AddShip(4.3f + 35, 4.6f, 7.1f, 0, DistanceCentrel);
                    AddShip(9.9f + 42, -7.3f, 2.1f, 1, DistanceCentrel); //AddShip(9 + xe, 5.6f + ye, 4.9f + ze, 10, DistanceCentrel);
                    AddShip(1 - 47, 4.9f, 7.5f, 2, DistanceCentrel); //AddShip(5 + xe, 1.4f + ye, 1.8f + ze, 11, DistanceCentrel);
                    AddShip(6.9f - 37, -3.9f, 6.5f, 3, DistanceCentrel); //AddShip(4.6f + xe, 4.2f + ye, 1 + ze, 12, DistanceCentrel);
                    AddShip(5, 9.3f + 54, 7.8f, 4, DistanceCentrel); //AddShip(2.8f + xe, 6.2f + ye, 3.9f + ze, 13, DistanceCentrel);
                    AddShip(-5.6f, 9.7f + 36, 1.5f, 5, DistanceCentrel); //AddShip(9.9f + xe, 8.5f + ye, 1.7f + ze, 14, DistanceCentrel);
                    AddShip(-1.6f, 8.9f - 66, 1.6f, 6, DistanceCentrel); //AddShip(6.8f + xe, 2.6f + ye, 9.6f + ze, 15, DistanceCentrel);
                    AddShip(9.8f, 7.2f - 49, 2.4f, 7, DistanceCentrel); //AddShip(2.3f + xe, 6.9f + ye, 2.4f + ze, 16, DistanceCentrel);
                    //AddShip(4.7f + xe, (-1) * (10 + ye), 2.7f + ze, 8, DistanceCentrel); //AddShip(2.5f + xe, 2.9f + ye, 5.1f + ze, 17, DistanceCentrel);
                    //AddShip(9.1f + xe, (-1) * (5.3f + ye), 1.1f + ze, 9, DistanceCentrel); //AddShip(5.4f + xe, 4.7f + ye, 5.2f + ze, 18, DistanceCentrel);
                    DistanceCentrel += 0.02f;
                }
                else if (sum != 1) Game[8] += 10;
        }

        // Функция добавляющая корабль
        private void AddShip(float xTarget, float yTarget, float zTarget, int iTarget, float l)
        {
            if (l == 0.1f)
            {
                psX[iTarget] = 0 - xTarget;
                psY[iTarget] = 0 - yTarget;
                psZ[iTarget] = 0 - zTarget;
            }

                xTarget += ((l + 2) * psX[iTarget]) / (float)Math.Sqrt(psX[iTarget] * psX[iTarget] + psY[iTarget] * psY[iTarget] + psZ[iTarget] * psZ[iTarget]);
                yTarget += ((l + 2) * psY[iTarget]) / (float)Math.Sqrt(psX[iTarget] * psX[iTarget] + psY[iTarget] * psY[iTarget] + psZ[iTarget] * psZ[iTarget]);
                zTarget += ((l + 2) * psZ[iTarget]) / (float)Math.Sqrt(psX[iTarget] * psX[iTarget] + psY[iTarget] * psY[iTarget] + psZ[iTarget] * psZ[iTarget]);

            HitTheTarget(xTarget, yTarget, zTarget,
            1.5, 0.4, 0.8, iTarget);

            if ((ChekTarget[iTarget] == 1) || (ChekTarget[iTarget] == 2))
            {
                    GL.PushMatrix();
                    GL.Translate(xTarget, yTarget, zTarget);

                    if (iTarget < 2)
                        GL.Rotate(180, Vector3.UnitZ);
                    else if ((iTarget < 4) && (iTarget >= 2))
                        GL.Rotate(90, Vector3.UnitZ);
                    else if ((iTarget < 6) && (iTarget >= 4))
                        GL.Rotate(180, Vector3.UnitZ);
                    else if ((iTarget < 8) && (iTarget >= 6))
                        GL.Rotate(90, Vector3.UnitZ);

                    DrawSpaceShip();
                    GL.PopMatrix();
            }


        }

        // Функция отслеживающая попадания снарядов
        // ROD - The Radius Of Destruction
        private void HitTheTarget(double xTarget, double yTarget, double zTarget,
            double xROD, double yROD, double zROD, int iTarget)
        {
            //if (ChekTarget[iTarget] == 1)
            //{
                for (int i = 0; i < flag; i++)
                {
                    rotateB[i] += 0.05f;
                    if (rotateB[i] >= 30) // (30) - Дальность полета 
                    { }
                    else
                        Shot(rotateB[i], i);

                    // Проверка на попадание снаряда в корабль
                    if ((ShotPosition.X < (xTarget + xROD + 0.05)) && (ShotPosition.X > (xTarget - xROD - 0.05)) &&
                        (ShotPosition.Y < (yTarget + yROD + 0.05)) && (ShotPosition.Y > (yTarget - yROD - 0.05)) &&
                        (ShotPosition.Z < (zTarget + zROD + 0.05)) && (ShotPosition.Z > (zTarget - zROD - 0.05)))
                    {
                        rotateB[i] = 30;
                        ChekTarget[iTarget] = 2;
                    }
                }
            //}
            /*else*/ if (ChekTarget[iTarget] == 2)
            {
                GL.PushMatrix();
                GL.Translate(xTarget, yTarget, zTarget);
                Boom(1.1, iTarget);
                GL.PopMatrix();
                Game[iTarget] = 1;
            }
        }

        // Функция загружающая текстуры
        public int LoadTexture(string filePath)
        {
            try
            {
                Bitmap image = new Bitmap(filePath);
                int texID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texID);
                BitmapData data = image.LockBits(
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0,
                    PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                image.UnlockBits(data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                return texID;
            }
            catch (System.IO.FileNotFoundException е)
            {
                return -1;
            }
        }

        // Функция рисующая квадрат с текстурами
        private void drawTexturedQuad()
        {
            // Включение наложения текстур
            GL.Enable(EnableCap.Texture2D);
            // Указание, какую текстуру берем
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[8]);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Blue);
            // Задание Текстурных координат
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Color3(Color.Red);
            // Задание Текстурных координат
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Color3(Color.White);
            // Задание Текстурных координат
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Color3(Color.Green);
            // Задание Текстурных координат
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.End();

            // Выключение наложения текстур
            GL.Disable(EnableCap.Texture2D);
        }

        // Функция настройки Света
        private void SetupLightning()
        {
            // Включение Освещения
            GL.Enable(EnableCap.Lighting);
            // Включение нулевого источника света
            GL.Enable(EnableCap.Light0);

            //GL.Enable(EnableCap.Light1); // Включение первого истчника света

            // Включение освещение цветных вершин
            GL.Enable(EnableCap.ColorMaterial);

            // Установка позиции источника света
            Vector4 lightPosition = new Vector4(0.0f, 0.0f, 5.0f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);

            /*Vector4 lightPosition1 = new Vector4(-1.0f, -1.0f, 4.5f, 0.0f);
            GL.Light(LightName.Light1, LightParameter.Position, lightPosition1);*/

            // Установка цвета, который будет иметь объект, не освещенный источником
            Vector4 ambientColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Ambient, ambientColor);

            /*Vector4 ambientColor1 = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
            GL.Light(LightName.Light1, LightParameter.Ambient, ambientColor1);*/

            // Установка цвета, который будет иметь объект, освещенный источником
            Vector4 diffuseColor = new Vector4(0.6f, 0.6f, 1.0f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuseColor);

            /*Vector4 diffuseColor1 = new Vector4(0.6f, 0.6f, 1.0f, 1.0f);
            GL.Light(LightName.Light1, LightParameter.Diffuse, diffuseColor1);*/

            // Установка материалам зеркальной состовляющей
            Vector4 materialSpecular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, materialSpecular);
            float materialShininess = 100;
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, materialShininess);
        }

        // Функция рисующая Сферу
        private void drawSphere(double r, int nx, int ny, int NumPlanet)
        {
            // r - радиус сферы
            // nx * ny - колличество полигонов(четырехугольников) из которых будет собрана сфера
            // ColorPlanet - цвет сферы
            // NumPlanet - номер текстуры для накладывания на сферу
            
            int ix, iy;
            double x, y, z;
            for (iy = 0; iy < ny; ++iy)
            {
                if (NumPlanet != -1)
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, texturesIDs[NumPlanet]);
                }
                GL.Begin(PrimitiveType.QuadStrip);
                GL.Color3(Color.Transparent);

                for (ix = 0; ix <= nx; ++ix)
                {
                    
                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos(iy * Math.PI / ny); 

                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)iy / (double)ny);
                    GL.Vertex3(x, y, z);
                   

                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos((iy + 1) * Math.PI / ny);

                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)(iy + 1) / (double)ny);
                    GL.Vertex3(x, y, z);
                    
                }
                GL.End();
                if (NumPlanet != -1)
                    GL.Disable(EnableCap.Texture2D);
            }
        }

        // Функция рисующая объемную трапецию
        private void GetTrap(double x, double y, double x1, double y1, double x2, double h1, double h2)
        {
            // Трапеция
            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(Color.Black);
            GL.Vertex3(x, y1, h2);
            GL.Vertex3(x, y, h2);
            GL.Vertex3(x2, y1, h2);
            GL.Vertex3(x1, y, h2);
            GL.Color3(Color.Red);
            GL.Vertex3(x1, y, h1);
            GL.Vertex3(x2, y1, h2);
            GL.Vertex3(x2, y1, h1);
            GL.Vertex3(x, y1, h2);
            GL.Vertex3(x, y1, h1);
            GL.Vertex3(x, y, h2);
            GL.Vertex3(x, y, h1);
            GL.Vertex3(x1, y, h2);
            GL.Color3(Color.White);
            GL.Vertex3(x1, y, h1);
            GL.Vertex3(x, y, h1);
            GL.Vertex3(x2, y1, h1);
            GL.Vertex3(x, y1, h1);
            GL.End();
        }

        // Функция рисующая двигатель
        private void GetMotor(int sgn)
        {
            GetTrap(sgn * 0.2, 0.06, sgn * 0.31, 0.54, sgn * 0.26, 0.05, 0);
            GetTrap(sgn * 0.2, -0.19, sgn * 0.31, 0.06, sgn * 0.31, 0.05, 0);
            GetTrap(sgn * 0.2, -0.37, sgn * 0.31, -0.19, sgn * 0.31, 0.06, 0);
            GetTrap(sgn * 0.2, -0.37, sgn * 0.31, -0.19, sgn * 0.31, -0.01, 0);
            GetTrap(sgn * 0.22, -0.42, sgn * 0.29, -0.16, sgn * 0.29, 0.06, 0);
            GetTrap(sgn * 0.22, -0.42, sgn * 0.29, -0.16, sgn * 0.29, -0.01, 0);
            GetTrap(sgn * 0.2, -0.62, sgn * 0.225, -0.39, sgn * 0.225, 0.05, 0);
            GetTrap(sgn * 0.285, -0.62, sgn * 0.31, -0.39, sgn * 0.31, 0.05, 0);
            GetTrap(sgn * 0.22, -0.62, sgn * 0.285, -0.56, sgn * 0.285, 0.01, -0.01);
            GetTrap(sgn * 0.22, -0.62, sgn * 0.285, -0.56, sgn * 0.285, 0.06, 0.04);
        }

        // Функция рисующая космический корабль
        private void DrawSpaceShip()
        {
            // Основание
            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(Color.White);
            GL.Vertex3(-0.15, 0, 0);
            GL.Vertex3(-0.025, 1.5, 0);
            GL.Vertex3(-0.05, -0.5, 0);
            GL.Vertex3(0.025, 1.5, 0);
            GL.Vertex3(0.05, -0.5, 0);
            GL.Vertex3(0.15, 0, 0);

            GL.Color3(Color.Red);
            GL.Vertex3(0.1, 0, 0.13);
            GL.Vertex3(0.025, 1.5, 0);
            GL.Vertex3(0.025, 1.5, 0.01);
            GL.Vertex3(-0.025, 1.5, 0.01);
            GL.Vertex3(-0.025, 1.5, 0);
            GL.Vertex3(-0.1, 0, 0.13);
            GL.Vertex3(-0.15, 0, 0);
            GL.Vertex3(-0.05, -0.5, 0.01);
            GL.Vertex3(-0.05, -0.5, 0);
            GL.Vertex3(0.05, -0.5, 0);
            GL.Vertex3(0.05, -0.5, 0.01);
            GL.Vertex3(0.1, 0, 0.13);

            GL.Color3(Color.SlateGray);
            GL.Vertex3(-0.05, -0.5, 0.01);
            GL.Vertex3(-0.1, 0, 0.13);
            GL.Vertex3(0.1, 0, 0.13);
            GL.Vertex3(-0.025, 1.5, 0.01);
            GL.Vertex3(0.025, 1.5, 0.01);

            GL.End();

            // Крылья
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(Color.Yellow);
            GL.Vertex3(0.08, 0.4, 0.04);
            GL.Vertex3(0.08, -0.24, 0.04);
            GL.Vertex3(0.5, -0.24, -0.09);
            GL.End();

            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(Color.Yellow);
            GL.Vertex3(-0.08, 0.4, 0.04);
            GL.Vertex3(-0.08, -0.24, 0.04);
            GL.Vertex3(-0.5, -0.24, -0.09);
            GL.End();

            // Двигатели
            GL.PushMatrix();
            GL.Translate(0, 0, 0.06);
            GL.Rotate(22, Vector3.UnitY);
            GetMotor(1);
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(0, 0, 0.06);
            GL.Rotate(-22, Vector3.UnitY);
            GetMotor(-1);
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(0.22, -0.24, -0.02);
            GL.Scale(1, 0.8f, 0.8f);
            GL.Rotate(22, Vector3.UnitY);
            GetMotor(1);
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(-0.22, -0.24, -0.02);
            GL.Scale(1, 0.8f, 0.8f);
            GL.Rotate(-22, Vector3.UnitY);
            GetMotor(-1);
            GL.PopMatrix();

        }

        // Функия считающая Угол поворота корабля
        private float CalcAlf(int index)
        {
            float result;

            double a2, b2, c2; // Длины сторон треугольника по которому вычисляем угол

            if (index == 1)
            {
                a2 = (cameraDirecton.X - (ShipPosition.X + 1)) * (cameraDirecton.X - (ShipPosition.X + 1)) +
                    (cameraDirecton.Z - (ShipPosition.Z + 1)) * (cameraDirecton.Z - (ShipPosition.Z + 1));
                b2 = (cameraDirecton.X - ShipPosition.X) * (cameraDirecton.X - ShipPosition.X) +
                    (cameraDirecton.Z - ShipPosition.Z) * (cameraDirecton.Z - ShipPosition.Z);
                c2 = 1;

                result = (float)Math.Acos((b2 + c2 - a2) / (2 * Math.Sqrt(b2) * Math.Sqrt(c2)));
            }
            else
            {
                a2 = (ShipPosition.Y - cameraDirecton.Y) * (ShipPosition.Y - cameraDirecton.Y);
                b2 = (ShipPosition.X - cameraDirecton.X) * (ShipPosition.X - cameraDirecton.X) +
                    (ShipPosition.Z - cameraDirecton.Z) * (ShipPosition.Z - cameraDirecton.Z);
                c2 = (ShipPosition.X - cameraDirecton.X) * (ShipPosition.X - cameraDirecton.X) +
                    (ShipPosition.Y - cameraDirecton.Y) * (ShipPosition.Y - cameraDirecton.Y) +
                    (ShipPosition.Z - cameraDirecton.Z) * (ShipPosition.Z - cameraDirecton.Z);

                result = (float)Math.Acos((b2 + c2 - a2) / (2 * Math.Sqrt(b2) * Math.Sqrt(c2)));
            }

            return (result);
        }

        // Функция задает координаты корабля
        private void CallSpaceShip()
        {
            float pX = cameraDirecton.X - cameraPosition.X;
            float pY = cameraDirecton.Y - cameraPosition.Y;
            float pZ = cameraDirecton.Z - cameraPosition.Z;

            ShipPosition.X = (1.5f * pX) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ) + cameraPosition.X;
            ShipPosition.Y = (1.5f * pY) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ) + cameraPosition.Y;
            ShipPosition.Z = (1.5f * pZ) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ) + cameraPosition.Z - 0.3f;
            
            GL.PushMatrix();
            GL.Translate(ShipPosition.X, ShipPosition.Y, ShipPosition.Z);

            /*GL.Rotate(CalcAlf(1), Vector3.UnitX); // Разворот по Y
            GL.Rotate(CalcAlf(2), Vector3.UnitY); // Разворот по Z*/
            
            GL.Scale(0.8, 0.8, 0.8);
            //DrawSpaceShip();
            drawSphere(0.2 , 40, 40, 11);
            GL.PopMatrix();
        }

        // Функция рисующая выстрел
        public void Shot(float l, int ii)
        {
            if (l == 0.1f)
            {
                pX[ii] = cameraDirecton.X - cameraPosition.X;
                pY[ii] = cameraDirecton.Y  - cameraPosition.Y;
                pZ[ii] = cameraDirecton.Z - cameraPosition.Z;
            }

            ShotPosition.X = ((l + 2) * pX[ii]) / (float)Math.Sqrt(pX[ii] * pX[ii] + pY[ii] * pY[ii] + pZ[ii] * pZ[ii]) + cameraPosition.X;
            ShotPosition.Y = ((l + 2) * pY[ii]) / (float)Math.Sqrt(pX[ii] * pX[ii] + pY[ii] * pY[ii] + pZ[ii] * pZ[ii]) + cameraPosition.Y;
            ShotPosition.Z = ((l + 2) * pZ[ii]) / (float)Math.Sqrt(pX[ii] * pX[ii] + pY[ii] * pY[ii] + pZ[ii] * pZ[ii]) + cameraPosition.Z - (0.4f / (rotateB[ii] + 1));

            GL.PushMatrix();
            GL.Translate(ShotPosition.X, ShotPosition.Y, ShotPosition.Z);
            drawSphere(0.05, 20, 20, 12);
            GL.PopMatrix();
        }

        // Функция рисующая прицел
        private void DrawSight()
        {
            float pX = cameraDirecton.X - cameraPosition.X;
            float pY = cameraDirecton.Y - cameraPosition.Y;
            float pZ = cameraDirecton.Z - cameraPosition.Z;

            SightPosition.X = (5f * pX) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ) + cameraPosition.X;
            SightPosition.Y = (5f * pY) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ) + cameraPosition.Y;
            SightPosition.Z = (5f * pZ) / (float)Math.Sqrt(pX * pX + pY * pY + pZ * pZ) + cameraPosition.Z;

            GL.PushMatrix();

            GL.Translate(SightPosition.X, SightPosition.Y, SightPosition.Z);
            GL.Rotate(90, Vector3.UnitX);

            {
                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(0, -0.15, 0);
                GL.Vertex3(0.04, -0.2, 0);
                GL.Vertex3(-0.04, -0.2, 0);
                GL.End();

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(0, 0.15, 0);
                GL.Vertex3(0.04, 0.2, 0);
                GL.Vertex3(-0.04, 0.2, 0);
                GL.End();

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(-0.15, 0, 0);
                GL.Vertex3(-0.2, 0.04, 0);
                GL.Vertex3(-0.2,-0.04, 0);
                GL.End();

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(0.15, 0, 0);
                GL.Vertex3(0.2, 0.04, 0);
                GL.Vertex3(0.2, -0.04, 0);
                GL.End();

                //--------------------------------

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(0, -0.15, 0);
                GL.Vertex3(0, -0.2, 0.04);
                GL.Vertex3(0, -0.2, -0.04);
                GL.End();

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(0, 0.15, 0);
                GL.Vertex3(0, 0.2, 0.04);
                GL.Vertex3(0, 0.2, -0.04);
                GL.End();

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(0, 0, -0.15);
                GL.Vertex3(0, 0.04, -0.2);
                GL.Vertex3(0, -0.04, -0.2);
                GL.End();

                GL.Begin(PrimitiveType.Triangles);
                GL.Color3(Color.White);
                GL.Vertex3(0, 0, 0.15);
                GL.Vertex3(0, 0.04, 0.2);
                GL.Vertex3(0, -0.04, 0.2);
                GL.End();
            }

            /*drawLine(0.15, 0, 0);
            drawLine(0, 0.15, 0);
            drawLine(-0.15, 0, 0);
            drawLine(0, -0.15, 0);
            drawLine(0, 0, 0.15);
            drawLine(0, 0, -0.15);*/
            drawCircle(0.1, Color.White, false);

            GL.PopMatrix();

            GL.PushMatrix();
            GL.Translate(SightPosition.X, SightPosition.Y, SightPosition.Z);
            GL.Rotate(90, Vector3.UnitY);
            drawCircle(0.1, Color.White, false);
            GL.PopMatrix();
        }

        // Функция рисующая Взрыв
        private void Boom(double RadiusBoom, int iTarget)
        {
            if (rotateD[iTarget] <= RadiusBoom)
            {
                rotateD[iTarget] += 0.003;
                GL.Scale(rotateD[iTarget], rotateD[iTarget], rotateD[iTarget]);
                drawSphere(RadiusBoom, 20, 20, 13);
            }
            else
            {
                ChekTarget[iTarget] = 0;
            }
        }

        // Функция рисующая Линию
        private void drawLine(double x, double y, double z)
        {
            // Задание ширины/толщины линии
            GL.LineWidth(5);

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.White);
            GL.Vertex3(0, 0, 0);
            GL.Color3(Color.White);
            GL.Vertex3(x, y, z);
            GL.End();
        }

        // Функция рисующая Шестиугольник
        private void drawTriangleStrip()
        {
            // TriangleStrip - рисует треугольники, беря вершины по тройкам в последовательности
         
            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(Color.White);
            GL.Vertex3(-1.7f, -1.0f, -1.0f); // 1
            GL.Color3(Color.White);
            GL.Vertex3(-1.7f, 1.0f, -1.0f);  // 1,2
            GL.Color3(Color.Blue);
            GL.Vertex3(0.0f, -2.0f, -1.0f);  // 1,2,3
            GL.Color3(Color.Blue);
            GL.Vertex3(0.0f, 2.0f, -1.0f);   // 2,3,4
            GL.Color3(Color.Red);
            GL.Vertex3(1.7f, -1.0f, -1.0f);  // 3,4
            GL.Color3(Color.Red);
            GL.Vertex3(1.7f, 1.0f, -1.0f);   // 4
            GL.End();
        }

        // Функция рисующая Четырехугольную пирамиду
        private void drawTriangleFun()
        {
            // TriangleFan - рисует треугольтики, беря как вершины первую и две последние

            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(Color.Red);
            GL.Vertex3(0.0f, 0.0f, 1.0f);    // 1,2,3,4
            GL.Color3(Color.Orange);
            GL.Vertex3(1.0f, 1.0f, -1.0f);   // 1,4
            GL.Color3(Color.Yellow);
            GL.Vertex3(1.0f, -1.0f, -1.0f);  // 1,2
            GL.Color3(Color.Green);
            GL.Vertex3(-1.0f, -1.0f, -1.0f); // 2,3
            GL.Color3(Color.Blue);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);  // 3,4
            // Вершина повторяется, чтобы замкнуть пирамиду
            GL.Color3(Color.Orange);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.End();
        }

        // Функция рисующая Куб с текстурами
        private void DrawCube()
        {
            // Первая Грань
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[0]);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Red);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Color3(Color.Red);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Color3(Color.Red);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Color3(Color.Red);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);

            // Вторая Грань
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[1]);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Color3(Color.White);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Color3(Color.White);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Color3(Color.White);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);

            // Третья Грань
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[2]);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Blue);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Color3(Color.Blue);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Color3(Color.Blue);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Color3(Color.Blue);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);

            // Четвертая Грань
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[3]);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Yellow);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Color3(Color.Yellow);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Color3(Color.Yellow);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Color3(Color.Yellow);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);

            // Пятая Грань
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[4]);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Green);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Color3(Color.Green);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Color3(Color.Green);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Color3(Color.Green);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);

            // Шестая Грань
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[5]);
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Orange);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Color3(Color.Orange);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Color3(Color.Orange);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Color3(Color.Orange);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);
        }

        // Функция рисующая Солнечную Систему
        private void DrawSolSystem()
        {
            // Солнце
            GL.PushMatrix();
            GL.Rotate((rotateAngle * 24.47f / 50), Vector3.UnitZ);
            drawSphere(1.0f, 20, 20, 1);
            GL.PopMatrix();

            Color CCorcle = Color.White;

            drawCircle(0.387 * 3, CCorcle, false);
            drawCircle(0.733 * 3, CCorcle, false);
            drawCircle(3, CCorcle, false);
            drawCircle(1.52 * 3, CCorcle, false);
            drawCircle(5.19 * 3, CCorcle, false);
            drawCircle(9.53 * 3, CCorcle, false);

            // Меркурий

            // Координаты положения планеты:
            // Math.Sin(rotateAngle / <скорость вращения>) * <радиус вращения> * 3
            // Math.Cos(rotateAngle / <скорость вращения>) * <радиус вращения> * 3
            GL.PushMatrix();
            GL.Translate(
                Math.Sin(rotateAngle / 7.23) * 0.387 * 3,
                Math.Cos(rotateAngle / 7.23) * 0.387 * 3,
                0);

            // GL.Rotate(rotateAngle * <скорость вращения вокруг своей оси> / 50, Vector3.UnitZ);
            GL.Rotate(rotateAngle * 88 / 50, Vector3.UnitZ);
            drawSphere(0.111f / 3, 20, 20, 2);
            GL.PopMatrix();
            
            // Венера
            GL.PushMatrix();
            GL.Translate(
                Math.Sin(rotateAngle / 18.46) * 0.733 * 3,
                Math.Cos(rotateAngle / 18.46) * 0.733 * 3,
                0);
            GL.Rotate(rotateAngle * 200 / 50, Vector3.UnitZ);
            drawSphere(0.291f / 3, 20, 20, 3);
            GL.PopMatrix();

            // Земля
            GL.PushMatrix();
            GL.Translate(
                Math.Sin(rotateAngle / 30) * 3,
                Math.Cos(rotateAngle / 30) * 3,
                0);
            GL.Rotate(rotateAngle / 50, Vector3.UnitZ);
            drawSphere(0.3f / 3, 20, 20, 4);
            GL.PopMatrix();

            //Луна
            GL.PushMatrix();
            GL.Translate(
                Math.Sin(rotateAngle / 2.3) * 0.05 * 3 + Math.Sin(rotateAngle / 30) * 3,
                Math.Cos(rotateAngle / 2.3) * 0.05 * 3 + Math.Cos(rotateAngle / 30) * 3,
                0);
            drawSphere(0.081f / 3, 20, 20, 6);
            GL.PopMatrix();

            //Марс
            /*HitTheTarget((Math.Sin(rotateAngle / 56.46) * 1.52 * 3),
                (Math.Cos(rotateAngle / 56.46) * 1.52 * 3),
                0,
                0.152, 0.152, 0.152, 2);

            if ((ChekTarget[2] == 1) || (ChekTarget[2] == 2))
            {*/
                GL.PushMatrix();
                GL.Translate(
                    Math.Sin(rotateAngle / 56.46) * 1.52 * 3,
                    Math.Cos(rotateAngle / 56.46) * 1.52 * 3,
                    0);
                GL.Rotate(rotateAngle * 1.025f / 50, Vector3.UnitZ);
                drawSphere(0.456f / 3, 20, 20, 7);
                GL.PopMatrix();
            //}

            //Юпитер
            GL.PushMatrix();
            GL.Translate(
                Math.Sin(rotateAngle / 355.8) * 5.19 * 3,
                Math.Cos(rotateAngle / 355.8) * 5.19 * 3,
                0);
            GL.Rotate(rotateAngle * 9.92f / 50, Vector3.UnitZ);
            drawSphere(3.384f / 3, 20, 20, 8);
            GL.PopMatrix();

            //Сатурн
            GL.PushMatrix();
            GL.Translate(
                Math.Sin(rotateAngle / 883.81) * 9.53 * 3,
                Math.Cos(rotateAngle / 883.81) * 9.53 * 3,
                0);
            GL.Rotate(rotateAngle * 10.23f / 50, Vector3.UnitZ);
            drawSphere(2.835f / 3, 20, 20, 9); 

            //Кольца Сатурна
            double i = 1.1;
            while (i < 1.6)
            {
                drawCircle(i, Color.DarkOrange, true);
                i += 0.05;
            }

            i = 1.7;
            while (i < 2.11)
            {
                drawCircle(i, Color.Orange, true);
                i += 0.05;
            }
            
            GL.PopMatrix();
        }

        // Функция рисующая Окружность
        private void drawCircle(double Radius, Color CircleColor, bool round)
        {
            //Переменна round нужна для наклона колец Cатурна

            GL.PointSize(1);

            GL.Begin(PrimitiveType.Points);
            double i = 0;
            double alf;

            while (i < 10)
            {
                if (round) 
                    alf = Math.Sin(i) * Radius;
                else
                    alf = 0;
                GL.Color3(CircleColor);
                GL.Vertex3(Math.Sin(i) * Radius, Math.Cos(i) * Radius, alf);
                i += 0.01;
            }
            GL.End();
        }
    }
}
