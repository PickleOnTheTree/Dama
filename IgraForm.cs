using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dama
{
    public partial class IgraForm : Form
    {
        Point? debugCircle = null;
        Igra igra;
        private int dimenzijeKvadratka = 100;
        private int borderDebelina = 10;

        public IgraForm()
        {
            InitializeComponent();
            igra = new Igra();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.DoubleBuffered = true;
            base.OnPaint(e);
            NarisiPlosco(e.Graphics);
            NarisiFigure(e.Graphics, igra.Figure);

            if (debugCircle != null)
            {
                Color shadow = Color.FromArgb(100, Color.Black);
                e.Graphics.FillEllipse(
                    new SolidBrush(shadow),
                    debugCircle.Value.X - 37 / 2 + 3,
                    debugCircle.Value.Y - 37 / 2 + 3,
                    37,
                    37
                );
                e.Graphics.FillEllipse(
                    Brushes.Black,
                    debugCircle.Value.X - 37/2,
                    debugCircle.Value.Y - 37 / 2,
                    37,
                    37
                );
                e.Graphics.FillEllipse(
                    Brushes.Green,
                    debugCircle.Value.X - 32 / 2,
                    debugCircle.Value.Y - 32 / 2,
                    32,
                    32
                );
            }
        }

        private void NarisiPlosco(Graphics g)
        {
            g.FillRectangle(
                new SolidBrush(Color.Green),
                0,
                0,
                igra.velikostPlosce * dimenzijeKvadratka + 2*borderDebelina,
                igra.velikostPlosce * dimenzijeKvadratka + 2*borderDebelina
                );

            for (int i = 0; i < igra.velikostPlosce; i++)
                for (int j = 0; j < igra.velikostPlosce; j++)
                {
                    Color color = Color.White;
                    if ((i + j) % 2 == 0)
                    {
                        color = Color.White;
                    }
                    else
                    {
                        color = Color.Black;
                    }

                    g.FillRectangle(
                        new SolidBrush(color),
                        i * dimenzijeKvadratka + borderDebelina,
                        j * dimenzijeKvadratka + borderDebelina,
                        dimenzijeKvadratka,
                        dimenzijeKvadratka
                        );
                }
        }

        public void NarisiFigure(Graphics g, List<Figura> figure)
        {
            foreach (Figura fig in figure)
            {
                Brush b = new SolidBrush(fig.Barva);
                int px = fig.X * dimenzijeKvadratka + 20 + borderDebelina;
                int py = fig.Y * dimenzijeKvadratka + 20 + borderDebelina;
                g.FillEllipse(b, px, py, 60, 60);
            }
        }

        void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Point locationBrezBorderja = new Point(e.X - borderDebelina, e.Y - borderDebelina);
            igra.handleClick(locationBrezBorderja);
            //System.Diagnostics.Debug.WriteLine("loc" + e.Location);  // run in Debug mode (F5)
            debugCircle = e.Location;
            Invalidate();
        }

    }
}
