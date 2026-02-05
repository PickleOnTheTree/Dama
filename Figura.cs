using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dama
{
    public abstract class Figura
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
                Brush b1 = new SolidBrush(Color.White);
                g.FillEllipse(b1, X * dimenzijaKvadratka + 17 + border,
                                  Y * dimenzijaKvadratka + 17 + border,
                                  66, 66);
            }

            Brush b = new SolidBrush(Barva);
            g.FillEllipse(b, X * dimenzijaKvadratka + 20 + border,
                              Y * dimenzijaKvadratka + 20 + border,
                              60, 60);
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

            foreach (var f in figure)
                if (f.X == novX && f.Y == novY)
                    return false;

            return Math.Abs(novX - X) == 1 && Math.Abs(novY - Y) == 1;
        }
    }

    public class Kraljica : Figura
    {
        public Kraljica(int x, int y, Color barva) : base(x, y, barva) { }

        public override bool JePremikOk(int novX, int novY, List<Figura> figure)
        {
            if (novX < 0 || novY < 0 || novX >= Nastavitve.VelikostPlosce || novY >= Nastavitve.VelikostPlosce)
                return false;

            if ((novX + novY) % 2 == 0)
                return false;

            foreach (var f in figure)
                if (f.X == novX && f.Y == novY)
                    return false;

            return Math.Abs(novX - X) == Math.Abs(novY - Y);
        }

    }



}
