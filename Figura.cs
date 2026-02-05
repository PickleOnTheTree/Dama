using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dama
{
    //vmesnik
    public interface IRisljiva
    {
        void Narisi(Graphics g, int dimenzijaKvadratka, int border);
    }

    public interface IPremik
    {
        bool JePremikOk(int novX, int novY, List<Figura> figure);
        void Premakni(int novX, int novY);
    }
    //abstrakten razred
    //polimorfizem
    public abstract class Figura : IRisljiva, IPremik
    {
        //kapsulacija
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public Color Barva { get; protected set; }
        public bool IsSelected { get; set; }
        
        //konstruktor
        public Figura(int x, int y, Color barva)
        {
            X = x;
            Y = y;
            Barva = barva;
            IsSelected = false;
        }

        //objektna metoda
        public void Premakni(int novX, int novY)
        {
            X = novX;
            Y = novY;
        }

        //abstraktna metoda
        public abstract bool JePremikOk(int novX, int novY, List<Figura> figure);


        //virtualna metoda
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

            //risanje figure
            using (Brush b = new SolidBrush(Barva))
            {
                g.FillEllipse(b, X * dimenzijaKvadratka + 20 + border,
                                  Y * dimenzijaKvadratka + 20 + border,
                                  60, 60);
            }
        }
    }

    //dedovanje
    public class NavadnaFigura : Figura
    {
        public NavadnaFigura(int x, int y, Color barva) : base(x, y, barva) { }

        public override bool JePremikOk(int novX, int novY, List<Figura> figure)
        {
            //meje
            if (novX < 0 || novY < 0 || novX >= Nastavitve.VelikostPlosce || novY >= Nastavitve.VelikostPlosce)
                return false;

            //ne po belih
            if ((novX + novY) % 2 == 0)
                return false;

            //ne po drugih figurah
            if (figure.Any(f => f.X == novX && f.Y == novY))
                return false;

            int deltaX = novX - X;
            int deltaY = novY - Y;

            //premik navadne figure
            if (deltaX != 1 && deltaX != -1)
                return false;

            //preprečitev premika nazaj navadni figuri
            if (Barva == Color.Red && deltaY != 1)
                return false;
            if (Barva == Color.Blue && deltaY != -1)
                return false;

            return true;
        }
    }

    //dedovanje

    public class Kraljica : Figura
    {
        //konstruktor
        public Kraljica(int x, int y, Color barvaOriginal) : base(x, y, barvaOriginal)
        {
            Barva = (barvaOriginal == Color.Red) ? Color.Orange : Color.Green;
        }

        public override bool JePremikOk(int novX, int novY, List<Figura> figure)
        {
            // preveri meje
            if (novX < 0 || novY < 0 || novX >= Nastavitve.VelikostPlosce || novY >= Nastavitve.VelikostPlosce)
                return false;

            // samo na črnih
            if ((novX + novY) % 2 == 0)
                return false;

            int premikX = novX - X;
            int premikY = novY - Y;

            // diagonalni premik
            if (!((premikX == premikY) || (premikX == -premikY)))
                return false;

            int korakiX;
            if (premikX > 0)
                korakiX = 1;
            else
                korakiX = -1;

            int korakiY;
            if (premikY > 0)
                korakiY = 1;
            else
                korakiY = -1;

            int diagonalaX = X + korakiX;
            int diagonalaY = Y + korakiY;

            // ali je prosto
            while (diagonalaX != novX && diagonalaY != novY)
            {
                if (figure.Any(f => f.X == diagonalaX && f.Y == diagonalaY))
                    return false;

                diagonalaX += korakiX;
                diagonalaY += korakiY;
            }

            // preveri cilj
            if (figure.Any(f => f.X == novX && f.Y == novY))
                return false;

            return true;
        }

        public override void Narisi(Graphics g, int dimenzijaKvadratka, int border)
        {
            //izbran
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
