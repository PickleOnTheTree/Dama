using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dama
{
    public class Figura
    {
        public int X;
        public int Y;
        public Color Barva;
        private bool IsKralj;

        public Figura(int x, int y, Color ploscek)
        {
            this.X = x;
            this.Y = y;
            this.Barva = ploscek;
            this.IsKralj = false;
        }
        private void premik(int novX, int novY)
        {
            
        }
        private void premikKralja(int novX, int novY)
        {

        }
        private static void Promotion()
        {
            //this.isKralj = true;
        }
    }
}
