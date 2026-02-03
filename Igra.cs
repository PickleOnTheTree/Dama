using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dama
{
    class Igra
    {
        private int dimenzijeKvadratka = 100;
        public List<Figura> Figure = new List<Figura> { };
        public int velikostPlosce = 8;
        Figura izbranaFigura = null;

        public Igra()
        {
            Figure = new List<Figura>();
            GenerirajFigure();
        }

        static void Meni()
        {

        }

        public void GenerirajFigure()
        {
            Figure.Clear();

            for (int j = 0; j < velikostPlosce; j++)
            {
                for (int i = 0; i < velikostPlosce; i++)
                {
                    if (j % 2 == 0)
                    {
                        if (i % 2 == 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (i % 2 == 1)
                        {
                            continue;
                        }
                    }

                    if (j < 3)
                    {
                        Figura f = new Figura(i, j, Color.Red);
                        Figure.Add(f);
                    }

                    else if (j > velikostPlosce - 4)
                    {
                        Figura f = new Figura(i, j, Color.Blue);
                        Figure.Add(f);
                    }
                }
            }
        }

        public bool handleClick(Point lokacija)
        {
            int clickXkvadratek = lokacija.X / dimenzijeKvadratka;
            int clickYkvadratek = lokacija.Y / dimenzijeKvadratka;

            //izberi figuro ce si kliknil nanjo 
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
            //premakini izbrano figuro
            else
            {
                if (JePremikOk(clickXkvadratek, clickYkvadratek))
                {
                    izbranaFigura.X = clickXkvadratek;
                    izbranaFigura.Y = clickYkvadratek;
                    izbranaFigura.IsSelected = false;
                    izbranaFigura = null;
                }


            }
            return false;
        }

        private bool JePremikOk(int clickXkvadratek, int clickYkvadratek)
        {
            foreach(Figura f in Figure)
            {

            }
            return false;
        }


    }
}
