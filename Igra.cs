using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dama
{
    static class Nastavitve
    {
        public const int VelikostPlosce = 8;
        public static readonly int DimenzijaKvadratka = 100;
        public static readonly int BorderDebelina = 10;
    }
    public class Igra
    {
        public List<Figura> Figure { get; private set; } = new List<Figura>();
        public int velikostPlosce = Nastavitve.VelikostPlosce;
        Figura izbranaFigura = null;

        public Igra()
        {
            GenerirajFigure();
        }

        public void GenerirajFigure()
        {
            Figure.Clear();

            for (int j = 0; j < velikostPlosce; j++)
            {
                for (int i = 0; i < velikostPlosce; i++)
                {
                    if ((i + j) % 2 == 0) continue;

                    if (j < 3)
                    {
                        Figure.Add(new NavadnaFigura(i, j, Color.Red));
                    }
                    else if (j > velikostPlosce - 4)
                    {
                        Figure.Add(new NavadnaFigura(i, j, Color.Blue));
                    }
                }
            }
        }

        public bool handleClick(Point lokacija)
        {
            int clickXkvadratek = lokacija.X / Nastavitve.DimenzijaKvadratka;
            int clickYkvadratek = lokacija.Y / Nastavitve.DimenzijaKvadratka;

            if (izbranaFigura == null)
            {
                foreach (Figura f in Figure)
                {
                    if (f.X == clickXkvadratek && f.Y == clickYkvadratek)
                    {
                        izbranaFigura = f;
                        izbranaFigura.IsSelected = true;
                        return true;
                    }
                }
            }
            else
            {
                if (izbranaFigura.X == clickXkvadratek &&
                    izbranaFigura.Y == clickYkvadratek)
                {
                    izbranaFigura.IsSelected = false;
                    izbranaFigura = null;
                    return true;
                }

                if (izbranaFigura.JePremikOk(clickXkvadratek, clickYkvadratek, Figure))
                {
                    izbranaFigura.Premakni(clickXkvadratek, clickYkvadratek);

                    if (izbranaFigura is NavadnaFigura && (izbranaFigura.Y == 0 || izbranaFigura.Y == velikostPlosce - 1))
                    {
                        Color barva = izbranaFigura.Barva;
                        Figure.Remove(izbranaFigura);
                        izbranaFigura = new Kraljica(clickXkvadratek, clickYkvadratek, barva);
                        Figure.Add(izbranaFigura);
                    }

                    izbranaFigura.IsSelected = false;
                    izbranaFigura = null;
                    return true;
                }
            }

            return false;
        }
    }

}
