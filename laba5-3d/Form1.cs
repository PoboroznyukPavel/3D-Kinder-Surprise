using System;
using System.Drawing;
using System.Windows.Forms;

namespace lab_5
{
    public partial class Form1 : Form
    {
        #region vars
        int N;
        Bitmap bmp;
        Graphics g;
        Pen pen = new Pen(Color.Green, 1);
        Point3D[,] alfabeta;
        Point3D[,] axonom;
        Point[,] screen;

        const double ANGLEJOY = Math.PI / 8;
        bool TIMERON;
        double ANGLECOUNTER;
        int r = 150;
        double f = -Math.PI / 2;
        double q = Math.PI / 2;
        double oX;
        double oY;
        double oZ;
        Color backgrColor = Color.Black;
        double kx;
        double ky;
        double kz;
        double dx;
        double dy;
        double dz;

        #endregion

        #region form
        public Form1()
        {
            InitializeComponent();
            N = 20;
            TIMERON = false;
            Restart();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        private void Restart()
        {
            alfabeta = new Point3D[N, N];
            axonom = new Point3D[N, N];
            screen = new Point[N, N];
            bmp = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            g = Graphics.FromImage(bmp);
            pictureBox2.Image = bmp;
            g.Clear(backgrColor);
            fillMatr3D();
            MatrixToAxon();
            fillScreenMatrix();
            Draw();
        }

        private void Redraw()
        {
            g.Clear(backgrColor);
            fillScreenMatrix();
            pictureBox2.Refresh();
            Draw();
            pictureBox2.Image = bmp;
        }
        #endregion

        #region backend
        private Point3D CalcCoord(double a, double b)
        {
            Point3D tmp = new Point3D();
            tmp.X = r * Math.Sin(a) * Math.Cos(b);
            tmp.Y = r * Math.Sin(a) * Math.Sin(b);
            tmp.Z = r * Math.Cos(a);
            if (r * Math.Cos(a) > 0)
                 tmp.Z += r;

            return tmp;
        }
        // Первод в аксонометрические координаты. 
        private Point3D CalcAxomCoord(Point3D old, double f, double q)
        {
            Point3D tmp = new Point3D();
            tmp.X = old.X * Math.Cos(f) + old.Y * Math.Sin(f);
            tmp.Y = -old.X * Math.Sin(f) * Math.Cos(q) + old.Y * Math.Cos(f) * Math.Cos(q) + old.Z * Math.Sin(q);
            tmp.Z = old.X * Math.Sin(f) * Math.Sin(q) - old.Y * Math.Cos(f) * Math.Sin(q) + old.Z * Math.Cos(q);
            return tmp;
        }
        // Перевод в экранные координаты.
        private Point CalcScreenCoord(Point3D old)
        {
            int x = pictureBox2.ClientSize.Width / 2;
            int y = pictureBox2.ClientSize.Height / 2;
            Point tmp = new Point();
            tmp.X = Convert.ToInt32(x + old.X);
            tmp.Y = Convert.ToInt32(y + old.Y);
            return tmp;
        }


        private void fillMatr3D()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    alfabeta[i, j] = CalcCoord((j*((2.0*Math.PI)/(double)N)), ((i * ((2.0 * Math.PI) / (double)N))));
                }
            }
        }

        // Заполнение матрицы аксонометрических координат.
        private void MatrixToAxon()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    axonom[i, j] = CalcAxomCoord(alfabeta[i, j], f, q);
                }
            }
        }

        private void fillScreenMatrix()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    screen[i, j] = CalcScreenCoord(axonom[i, j]);
                }
            }
        }

        private void Draw()
        {
            //Она больше не двоит - был выбран неверный шаг для вычисления точек.
            //правильный, это когда за N точек проходим всю окружность (360deg)
            //раньше стоял рандомный "ну типа круглый и зашибись"
            Point p1;
            Point p2;
            for (int i = 1; i < N; i++)
            {
                p1 = new Point(screen[N - 1, i].X, screen[N - 1, i].Y);
                p2 = new Point(screen[0, i].X, screen[0, i].Y);
                g.DrawLine(pen, p1, p2);
                for (int j = 1; j < N; j++)
                {
                p1 = new Point(screen[i-1, j].X, screen[i-1, j].Y);
                p2 = new Point(screen[i, j].X, screen[i, j].Y);
                g.DrawLine(pen, p1, p2);
                }
            }

            for (int i = 1; i < N; i++)
            {
                p1 = new Point(screen[0, i-1].X, screen[0, i-1].Y);
                p2 = new Point(screen[0, i].X, screen[0, i].Y);
                g.DrawLine(pen, p1, p2);
                for (int j = 1; j < N; j++)
                {
                    p1 = new Point(screen[i, j-1].X, screen[i, j - 1].Y);
                    p2 = new Point(screen[i, j].X, screen[i, j].Y);
                    g.DrawLine(pen, p1, p2);
                }
            }
        }

        #region turning

        private Point3D TurnOx(Point3D old)
        {
            Point3D tmp = new Point3D();
            tmp.X = old.X;
            tmp.Y = old.Y * Math.Cos(oX) - old.Z * Math.Sin(oX);
            tmp.Z = old.Y * Math.Sin(oX) + old.Z * Math.Cos(oX);
            return tmp;
        }
        private Point3D TurnOy(Point3D old)
        {
            Point3D tmp = new Point3D();
            tmp.X = old.X * Math.Cos(oY) + old.Z * Math.Sin(oY);
            tmp.Y = old.Y;
            tmp.Z = -old.X * Math.Sin(oY) + old.Z * Math.Cos(oY);
            return tmp;
        }
        private Point3D TurnOz(Point3D old)
        {
            Point3D tmp = new Point3D();
            tmp.X = old.X * Math.Cos(oZ) - old.Y * Math.Sin(oZ);
            tmp.Y = old.X * Math.Sin(oZ) + old.Y * Math.Cos(oZ);
            tmp.Z = old.Z;
            return tmp;
        }

        private void TurnAxomOx()
        {
            int i = 0;
            int j = 0;
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < N; j++)
                {

                    axonom[i, j] = TurnOx(axonom[i, j]);
                }
            }
        }
        private void TurnAxomOy()
        {
            int i = 0;
            int j = 0;
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < N; j++)
                {

                    axonom[i, j] = TurnOy(axonom[i, j]);
                }
            }
        }
        private void TurnAxomOz()
        {
            int i = 0;
            int j = 0;
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < N; j++)
                {

                    axonom[i, j] = TurnOz(axonom[i, j]);
                }
            }
        }
        #endregion

        #region scaling

        private Point3D Scaling(Point3D old)
        {
            Point3D tmp = new Point3D();
            tmp.X = old.X * kx;
            tmp.Y = old.Y * ky;
            tmp.Z = old.Z * kz;
            return tmp;
        }
        private void ScalingAxom()
        {
            int i = 0;
            int j = 0;
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < N; j++)
                {
                    axonom[i, j] = Scaling(axonom[i, j]);
                }
            }
        }
        #endregion

        #region moving

        private Point3D Shift(Point3D old)
        {
            Point3D tmp = new Point3D();
            tmp.X = old.X + dx;
            tmp.Y = old.Y + dy;
            tmp.Z = old.Z + dz;
            return tmp;
        }
        private void ShiftAxom()
        {
            int i = 0;
            int j = 0;
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < N; j++)
                {

                    axonom[i, j] = Shift(axonom[i, j]);
                }
            }
        }
        #endregion

        
       

        #endregion



        #region buttons
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (textBoxKX.Text != "") { kx = Convert.ToDouble(textBoxKX.Text); } else kx = 1;
            }
            catch (Exception inkx)
            {
                textBoxKX.Text = "1";
                kx = Convert.ToDouble(textBoxKX.Text);
            }
            try
            {
                if (textBoxKY.Text != "") { ky = Convert.ToDouble(textBoxKY.Text); } else ky = 1;
            }
            catch (Exception inky)
            {
                textBoxKY.Text = "1";
                ky = Convert.ToDouble(textBoxKY.Text);
            }
            try
            { 
                if (textBoxKZ.Text != "") { kz = Convert.ToDouble(textBoxKZ.Text); } else kz = 1;
            }
            catch (Exception inkz)
            {
                textBoxKZ.Text = "1";
                kz = Convert.ToDouble(textBoxKZ.Text);
            }
            ScalingAxom();
            Redraw();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (textBoxDX.Text != "") { dx = Convert.ToDouble(textBoxDX.Text); } else dx = 0;
            }
            catch (Exception indx)
            {
                textBoxDX.Text = "0";
                dx = Convert.ToDouble(textBoxDX.Text);
            }
            try
            { 
                if (textBoxDY.Text != "") { dy = Convert.ToDouble(textBoxDY.Text); } else dy = 0;
            }
            catch (Exception indy)
            {
                textBoxDY.Text = "0";
                dy = Convert.ToDouble(textBoxDY.Text);
            }
            try
            { 
                if (textBoxDZ.Text != "") { dz = Convert.ToDouble(textBoxDZ.Text); } else dz = 0;
            }
            catch (Exception indz)
            {
                textBoxDZ.Text = "0";
                dz = Convert.ToDouble(textBoxDZ.Text);
            }
            ShiftAxom();
            Redraw();
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (textBoxOX.Text != "") { oX = Convert.ToDouble(textBoxOX.Text) / (180.0 / Math.PI); TurnAxomOx(); } else oX = 0;
            }
            catch (Exception inox)
            {
                textBoxOX.Text = "0";
                oX = Convert.ToDouble(textBoxOX.Text);
            }
            try
            { 
                if (textBoxOY.Text != "") { oY = Convert.ToDouble(textBoxOY.Text) / (180.0 / Math.PI); TurnAxomOy(); } else oY = 0;
            }
            catch (Exception inoy)
            {
                textBoxOY.Text = "0";
                oY = Convert.ToDouble(textBoxOY.Text);
            }
            try
            {
            if (textBoxOZ.Text != "") { oZ = Convert.ToDouble(textBoxOZ.Text) / (180.0 / Math.PI); TurnAxomOz(); } else oZ = 0;
            }
            catch (Exception inoz)
            {
                textBoxOZ.Text = "0";
                oZ = Convert.ToDouble(textBoxOZ.Text);
            }
            Redraw();
        }


        #endregion

        #region joysticks

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBoxN.Text.Length > 0)
            {
                int v = 36;
                try
                {
                    v = Convert.ToInt32(textBoxN.Text);
                }
                catch(Exception inN)
                {
                    textBoxN.Text = "36";
                }
                N = v;
                Restart();
            }
        }

        private void buttonShrink_Click(object sender, EventArgs e)
        {
            kx = 0.5;
            ky = 0.5;
            kz = 0.5;
            ScalingAxom();
            Redraw();
            kx = 0;
            ky = 0;
            kz = 0;
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            g.Clear(backgrColor);
            kx = 2;
            ky = 2;
            kz = 2;
            ScalingAxom();
            Redraw();
            kx = 0;
            ky = 0;
            kz = 0;
        }


        private void buttonTopFromUs_Click(object sender, EventArgs e)
        {
           
            oX = ANGLEJOY;
            TurnAxomOx();
            Redraw();
            oX = 0;
        }
        private void buttonTopToUs_Click(object sender, EventArgs e)
        {
           
            oX = -ANGLEJOY;
            TurnAxomOx();
            Redraw();
            oX = 0;
        }
        private void buttonOy_Click(object sender, EventArgs e)
        {
           
            oY = ANGLEJOY;
            TurnAxomOy();
            Redraw();
            oY = 0;
        }
        private void buttonAntiOy_Click(object sender, EventArgs e)
        {
           
            oY = -ANGLEJOY;
            TurnAxomOy();
            Redraw();
            oY = 0;
        }
        private void button4_Click(object sender, EventArgs e)
        {
           
            oZ = -ANGLEJOY;
            TurnAxomOz();
            Redraw();
            oZ = 0;
        }
        private void buttonToUs_Click(object sender, EventArgs e)
        {
           
            oZ = ANGLEJOY;
            TurnAxomOz();
            Redraw();
            oZ = 0;
        }
        

        private void buttonUp_Click(object sender, EventArgs e)
        {
           
            dy = -pictureBox2.Height/50;
            ShiftAxom();
            Redraw();
            dy = 0;
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
           
            dx = 20;
            ShiftAxom();
            Redraw();
            dx = 0;
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
           
            dy = pictureBox2.Height/50;
            ShiftAxom();
            Redraw();
            dy = 0;
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
           
            dx = -20;
            ShiftAxom();
            Redraw();
            dx = 0;
        }


        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(ANGLECOUNTER <= Math.PI*2)
            {
                oY = ANGLEJOY / 10;
                TurnAxomOy();
                Redraw();
                oY = 0;
                ANGLECOUNTER+= ANGLEJOY / 10;
            }
            else
            {
                timer1.Stop();
                TIMERON = false;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (TIMERON)
            {
                timer1.Stop();
                TIMERON = false;
            }
            else
            {
                ANGLECOUNTER = 0;
                timer1.Start();
                TIMERON = true;
            }
        }
    }
}
