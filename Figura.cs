using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dama
{
    public interface IRisljiva
    {
        void Narisi(Graphics g, int dimenzijaKvadratka, int border);
    }

    public interface IPremik
    {
        bool JePremikOk(int novX, int novY, List<Figura> figure);
        void Premakni(int novX, int novY);
    }
    public abstract class Figura : IRisljiva, IPremik
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public Color Barva { get; protected set; }
        public bool IsSelected { get; set; }

        public Figura(int x, int y, Color barva)
        {
            X = x;
            Y = y;
            Barva = barva;
            IsSelected = false;
        }

        public void Premakni(int novX, int novY)
        {
            X = novX;
            Y = novY;
        }

        public abstract bool JePremikOk(int novX, int novY, List<Figura> figure);

        public virtual void Narisi(Graphics g, int dimenzijaKvadratka, int border)
        {
            if (IsSelected)
            {
                using (Brush b1 = new SolidBrush(Color.White))
                {
                    g.FillEllipse(b1, X * dimenzijaKvadratka + 17 + border,
                                      Y * dimenzijaKvadratka + 17 + border,
                                      66, 66);
                }
            }

            using (Brush b = new SolidBrush(Barva))
            {
                g.FillEllipse(b, X * dimenzijaKvadratka + 20 + border,
                                  Y * dimenzijaKvadratka + 20 + border,
                                  60, 60);
            }
        }
    }
    public class NavadnaFigura : Figura
    {
        public NavadnaFigura(int x, int y, Color barva) : base(x, y, barva) { }

        public override bool JePremikOk(int novX, int novY, List<Figura> figure)
        {
            if (novX < 0 || novY < 0 || novX >= Nastavitve.VelikostPlosce || novY >= Nastavitve.VelikostPlosce)
                return false;

            if ((novX + novY) % 2 == 0)
                return false;

            if (figure.Any(f => f.X == novX && f.Y == novY))
                return false;

            int deltaX = novX - X;
            int deltaY = novY - Y;

            if (deltaX != 1 && deltaX != -1)
                return false;

            if (Barva == Color.Red && deltaY != 1)
                return false;
            if (Barva == Color.Blue && deltaY != -1)
                return false;

            return true;
        }
    }

    public class Kraljica : Figura
    {
        public Kraljica(int x, int y, Color barvaOriginal) : base(x, y, barvaOriginal)
        {
            Barva = (barvaOriginal == Color.Red) ? Color.Orange : Color.Green;
        }

        public override bool JePremikOk(int novX, int novY, List<Figura> figure)
        {
            if (novX < 0 || novY < 0 || novX >= Nastavitve.VelikostPlosce || novY >= Nastavitve.VelikostPlosce)
                return false;

            if ((novX + novY) % 2 == 0)
                return false;

            int premikX = novX - X;
            int premikY = novY - Y;

            if (!((premikX == premikY) || (premikX == -premikY)))
                return false;

            int korakiX = (premikX > 0) ? 1 : -1; 
            int korakiY = (premikY > 0) ? 1 : -1; 

            int diagonalaX = X + korakiX;
            int diagonalaY = Y + korakiY;
            
            while (diagonalaX != novX && diagonalaY != novY)
            {
                if (figure.Any(f => f.X == diagonalaX && f.Y == diagonalaY))
                    return false;

                diagonalaX += korakiX;
                diagonalaY += korakiY;
            }

            if (figure.Any(f => f.X == novX && f.Y == novY))
                return false;

            return true;
        }

        public override void Narisi(Graphics g, int dimenzijaKvadratka, int border)
        {
            //ozačen ce izbran
            if (IsSelected)
            {
                using (Brush b1 = new SolidBrush(Color.White))
                {
                    g.FillEllipse(b1, X * dimenzijaKvadratka + 17 + border,
                                      Y * dimenzijaKvadratka + 17 + border,
                                      66, 66);
                }
            }
            //prebarva ob "promociji"
            using (Brush b = new SolidBrush(Barva))
            {
                g.FillEllipse(b, X * dimenzijaKvadratka + 20 + border,
                              Y * dimenzijaKvadratka + 20 + border,
                              60, 60);
            }

            Color kronaBarva = Color.Yellow; 
            using (Brush crownBrush = new SolidBrush(kronaBarva))
            {
                g.FillEllipse(crownBrush, X * dimenzijaKvadratka + 25 + border,
                                          Y * dimenzijaKvadratka + 25 + border,
                                          50, 50);
            }
        }
    }



}
