using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dama
{
    class Igra
    {
        public List<Figura> Figure = new List<Figura> { };
        public int velikostPlosce = 8;

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
                        if(i % 2 == 0)
                        {
                            continue;
                        } 
                    }
                    else
                    {
                        if(i % 2 == 1)
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
        

    }
}
