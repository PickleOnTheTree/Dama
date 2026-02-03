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
            NarisiPlosco(e.Graphics);
            NarisiFigure(e.Graphics, igra.Figure);
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


        Point firstPoint;
        Boolean haveFirstPoint;


        void Form1_MouseDownDrawing(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (haveFirstPoint)
            {
                Graphics g = this.CreateGraphics();
                g.DrawLine(Pens.Black, firstPoint, e.Location);
                haveFirstPoint = false;
            }
            else
            {
                firstPoint = e.Location;
                haveFirstPoint = true;
            }
        }

    }
}
