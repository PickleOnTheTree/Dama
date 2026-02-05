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
        Figura izbranaFigura = null;

        public int velikostPlosce => Nastavitve.VelikostPlosce;

        public Igra() => GenerirajFigure();

        public void GenerirajFigure()
        {
            Figure.Clear();
            for (int j = 0; j < velikostPlosce; j++)
            {
                for (int i = 0; i < velikostPlosce; i++)
                {
                    if ((i + j) % 2 == 0) continue;

                    if (j < 3) Figure.Add(new NavadnaFigura(i, j, Color.Red));
                    else if (j > velikostPlosce - 4) Figure.Add(new NavadnaFigura(i, j, Color.Blue));
                }
            }
        }

        public Figura this[int x, int y] => Figure.FirstOrDefault(f => f.X == x && f.Y == y);

        public bool handleClick(Point lokacija)
        {
            int clickX = lokacija.X / Nastavitve.DimenzijaKvadratka;
            int clickY = lokacija.Y / Nastavitve.DimenzijaKvadratka;

            if (izbranaFigura == null)
            {
                foreach (var f in Figure)
                {
                    if (f.X == clickX && f.Y == clickY)
                    {
                        izbranaFigura = f;
                        izbranaFigura.IsSelected = true;
                        return true;
                    }
                }
            }
            else
            {
                if (izbranaFigura.X == clickX && izbranaFigura.Y == clickY)
                {
                    izbranaFigura.IsSelected = false;
                    izbranaFigura = null;
                    return true;
                }

                if (izbranaFigura.JePremikOk(clickX, clickY, Figure))
                {
                    izbranaFigura.Premakni(clickX, clickY);

                    // promocija
                    if (izbranaFigura is NavadnaFigura &&
                        ((izbranaFigura.Barva == Color.Red && izbranaFigura.Y == Nastavitve.VelikostPlosce - 1) ||
                         (izbranaFigura.Barva == Color.Blue && izbranaFigura.Y == 0)))
                    {
                        Color barva = izbranaFigura.Barva;
                        Figure.Remove(izbranaFigura);
                        izbranaFigura = new Kraljica(clickX, clickY, barva);
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
