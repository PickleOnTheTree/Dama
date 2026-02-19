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

        // Provide a default implementation so callers using the base type can ask which piece would be captured.
        // Derived pieces (like Kraljica) can override this when their capture logic differs.
        public virtual Figura GetFiguraZaJemanje(int novX, int novY, List<Figura> figure)
        {
            int deltaX = novX - X;
            int deltaY = novY - Y;

            if (Math.Abs(deltaX) == 2 && Math.Abs(deltaY) == 2)
            {
                int srednjiX = X + deltaX / 2;
                int srednjiY = Y + deltaY / 2;

                return figure.FirstOrDefault(f => f.X == srednjiX && f.Y == srednjiY);
            }

            return null;
        }

        // Visual color separate from owner color — override in subclasses if needed
        protected virtual Color VizualnaBarva => Barva;

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

            //risanje figure using visual color
            using (Brush b = new SolidBrush(VizualnaBarva))
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
            // meje
            if (novX < 0 || novY < 0 || novX >= Nastavitve.VelikostPlosce || novY >= Nastavitve.VelikostPlosce)
                return false;

            // ne po belih
            if ((novX + novY) % 2 == 0)
                return false;

            // ne po drugih figurah
            if (figure.Any(f => f.X == novX && f.Y == novY))
                return false;

            int deltaX = novX - X;
            int deltaY = novY - Y;

            //JEMANJE (skok za 2)
            if (Math.Abs(deltaX) == 2 && Math.Abs(deltaY) == 2)
            {
                int srednjiX = X + deltaX / 2;
                int srednjiY = Y + deltaY / 2;

                Figura figuraNaSredini = figure
                    .FirstOrDefault(f => f.X == srednjiX && f.Y == srednjiY);

                // mora biti nasprotnik
                if (figuraNaSredini != null && figuraNaSredini.Barva != Barva)
                {
                    // odstrani pojeden kos
                    return true;
                }

                return false;
            }

            //NAVADEN PREMIK

            // premik za 1 diagonalno
            if (Math.Abs(deltaX) != 1 || Math.Abs(deltaY) != 1)
                return false;

            // preprečitev premika nazaj navadni figuri
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
        private Color vizualnaBarva;

        // konstruktor
        public Kraljica(int x, int y, Color barvaOriginal) : base(x, y, barvaOriginal)
        {
            // nastavitev vizualne barve glede na lastnika
            if (barvaOriginal == Color.Blue)
                vizualnaBarva = Color.Green;
            else if (barvaOriginal == Color.Red)
                vizualnaBarva = Color.Orange;
            else
                vizualnaBarva = barvaOriginal;

        }

        // override vizualna barva
        protected override Color VizualnaBarva => vizualnaBarva;

        public override bool JePremikOk(int novX, int novY, List<Figura> figure)
        {
            // preveri meje
            if (novX < 0 || novY < 0 || novX >= Nastavitve.VelikostPlosce || novY >= Nastavitve.VelikostPlosce)
                return false;

            // samo črna polja
            if ((novX + novY) % 2 == 0)
                return false;

            int premikX = novX - X;
            int premikY = novY - Y;

            if (Math.Abs(premikX) != Math.Abs(premikY) || premikX == 0)
                return false;

            int stepX = (premikX > 0) ? 1 : -1;
            int stepY = (premikY > 0) ? 1 : -1;

            int cx = X + stepX;
            int cy = Y + stepY;

            int encountered = 0;
            Figura encounteredFigura = null;

            while (cx != novX || cy != novY)
            {
                var f = figure.FirstOrDefault(fr => fr.X == cx && fr.Y == cy);
                if (f != null)
                {
                    encountered++;
                    encounteredFigura = f;
                    if (encountered > 1)
                        return false; // ne more preskočiti več kot ene figure
                }

                cx += stepX;
                cy += stepY;
            }

            // cilj mora biti prazen
            if (figure.Any(f => f.X == novX && f.Y == novY))
                return false;

            if (encountered == 0) // običajen premik
                return true;

            // če je ena nasprotnikova figura → jemanje
            return encountered == 1 && encounteredFigura != null && encounteredFigura.Barva != Barva;
        }

        public override Figura GetFiguraZaJemanje(int novX, int novY, List<Figura> figure)
        {
            int deltaX = novX - X;
            int deltaY = novY - Y;

            if (Math.Abs(deltaX) != Math.Abs(deltaY) || deltaX == 0)
                return null;

            int stepX = (deltaX > 0) ? 1 : -1;
            int stepY = (deltaY > 0) ? 1 : -1;

            int cx = X + stepX;
            int cy = Y + stepY;

            Figura encounteredFigura = null;
            int encountered = 0;

            while (cx != novX || cy != novY)
            {
                var f = figure.FirstOrDefault(fr => fr.X == cx && fr.Y == cy);
                if (f != null)
                {
                    encountered++;
                    encounteredFigura = f;
                    if (encountered > 1)
                        return null; // neveljavno
                }

                cx += stepX;
                cy += stepY;
            }

            if (encountered == 1 && encounteredFigura != null && encounteredFigura.Barva != Barva)
                return encounteredFigura;

            return null;
        }

        public override void Narisi(Graphics g, int dimenzijaKvadratka, int border)
        {
            base.Narisi(g, dimenzijaKvadratka, border);
        }
    }

}
