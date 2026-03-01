using Dama;
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
    public delegate void PremikDelegate();       // custom delegate za premik
    public delegate void PromocijaDelegate();    // custom delegate za promocijo

    //Event
    public class FiguraEventArgs : EventArgs
    {
        public Figura Figura { get; private set; }

        public FiguraEventArgs(Figura figura)
        {
            Figura = figura;
        }
    }

    // static razred (static člani, const, readonly)
    static class Nastavitve
    {   
        //  static
        public static int VelikostPlosce = 8;

        // readonly podatki (nastavljeni enkrat)
        public static readonly int DimenzijaKvadratka = 100;
        public static readonly int BorderDebelina = 10;
    }

    public class Igra
    {
        //kapslulcija seznam figur javno dostopen, vendar protected pred spreminjanjem (private set)
        public List<Figura> Figure { get; private set; } = new List<Figura>();

        //kapsulacija trenutno izbrana figura je skrita zunanjim razredom
        Figura izbranaFigura = null;

        // current player (used by MenjajIgralca) - start with Blue as requested (first blue then red)
        private Color trenutniIgralec = Color.Blue;

        //dogodki obvestijo UI o spremembah v igri
        public event EventHandler<FiguraEventArgs> FiguraPremaknjena;
        public event EventHandler<FiguraEventArgs> FiguraPromovirana;

        //lastnosti dostop do velikosti plošče preko Nastavitve
        public int velikostPlosce => Nastavitve.VelikostPlosce;

        //konstruktor inicializacija igre
        public Igra() => GenerirajFigure();

        //objektna metoda generira začetne figure
        public void GenerirajFigure()
        {
            Figure.Clear();

            for (int j = 0; j < velikostPlosce; j++)
            {
                for (int i = 0; i < velikostPlosce; i++)
                {
                    // logika črnih/belih polj
                    if ((i + j) % 2 == 0) continue;

                    // ustvarjanje objektov (NavadnaFigura)
                    if (j < 3)
                        Figure.Add(new NavadnaFigura(i, j, Color.Red));
                    else if (j > velikostPlosce - 4)
                        Figure.Add(new NavadnaFigura(i, j, Color.Blue));
                }
            }
        }

        //indekser omogoča dostop z igra[x,y]
        public Figura this[int x, int y]
            => Figure.FirstOrDefault(f => f.X == x && f.Y == y);

        //objektna metoda logika klika + izbire + premika
        public bool handleClick(Point lokacija)
        {
            int clickX = lokacija.X / Nastavitve.DimenzijaKvadratka;
            int clickY = lokacija.Y / Nastavitve.DimenzijaKvadratka;

            var kliknjenaFigura = this[clickX, clickY];

            // Če je multi-jump aktiven, dovolimo klik samo za trenutno figuro
            if (izbranaFigura != null && izbranaFigura.IsSelected)
            {
                if (kliknjenaFigura != null &&
                    kliknjenaFigura != izbranaFigura &&
                    kliknjenaFigura.Barva == trenutniIgralec)
                {
                    return false; // ne dovoli izbire druge figure
                }
            }

            // Če ni izbrane figure → izberi
            if (izbranaFigura == null)
            {
                if (kliknjenaFigura != null && kliknjenaFigura.Barva == trenutniIgralec)
                {
                    izbranaFigura = kliknjenaFigura;
                    izbranaFigura.IsSelected = true;
                    return true;
                }
            }
            else
            {
                // Klik na isto figuro vedno odznači
                if (izbranaFigura == kliknjenaFigura)
                {
                    izbranaFigura.IsSelected = false;
                    izbranaFigura = null;
                    return true;
                }

                // Poskus premika
                if (izbranaFigura.JePremikOk(clickX, clickY, Figure))
                {


                    Figura movedFigura = izbranaFigura;
                    IzvediPremik(movedFigura, clickX, clickY);

                    // PROMOCIJA
                    if (movedFigura is NavadnaFigura &&
                        ((movedFigura.Barva == Color.Red && movedFigura.Y == Nastavitve.VelikostPlosce - 1) ||
                         (movedFigura.Barva == Color.Blue && movedFigura.Y == 0)))
                    {
                        Color barva = movedFigura.Barva;

                        Figure.Remove(movedFigura);

                        var promoted = new Kraljica(movedFigura.X, movedFigura.Y, barva);

                        OnFiguraPromovirana(promoted);

                        Figure.Add(promoted);

                        izbranaFigura = promoted;
                        movedFigura = promoted;
                    }

                    OnFiguraPremaknjena(movedFigura);

                    // Če je multi-jump še aktiven ostani na isti figuri
                    if (izbranaFigura != null && izbranaFigura.IsSelected)
                        return true;

                    izbranaFigura = null;
                    return true;
                }
            }

            return false;
        }


        //METODE ZA SPROŽITEV EVENTOV
        protected virtual void OnFiguraPremaknjena(Figura figura)
        {
            // previrja da ni null
            if (figura == null) return;
            FiguraPremaknjena?.Invoke(this, new FiguraEventArgs(figura));
        }

        protected virtual void OnFiguraPromovirana(Figura figura)
        {
            if (figura == null) return;
            FiguraPromovirana?.Invoke(this, new FiguraEventArgs(figura));
        }

        private void IzvediPremik(Figura figura, int novX, int novY)
        {
            if (!figura.JePremikOk(novX, novY, Figure))
                return;

            Figura pojedenKos = figura.GetFiguraZaJemanje(novX, novY, Figure);

            // premakni figuro
            figura.Premakni(novX, novY);

            if (pojedenKos != null && pojedenKos.Barva != figura.Barva)
            {
                Figure.Remove(pojedenKos);

                // MULTI JUMP
                if (ImaMoznoJemanje(figura))
                {
                    izbranaFigura = figura;
                    figura.IsSelected = true;
                    return; // isti igralec nadaljuje
                }
            }

            // konec poteze
            figura.IsSelected = false;
            izbranaFigura = null;

            // preveri konec igre
            if (JeKonecIgre())
                return;

            MenjajIgralca();
        }

        // switch current player
        private void MenjajIgralca()
        {
            trenutniIgralec = (trenutniIgralec == Color.Red) ? Color.Blue : Color.Red;
        }

        //ce je prosto za figuro
        private bool ImaMoznoJemanje(Figura figura)
        {
            return ImaMoznoJemanjeRekurzivno(figura.X, figura.Y, figura.Barva, new HashSet<(int, int)>());
        }

        private bool ImaMoznoJemanjeRekurzivno(int x, int y, Color barva, HashSet<(int, int)> obiskane)
        {
            // vse štiri diagonalne smeri
            int[] dxSmeri = { -1, 1 };
            int[] dySmeri = { -1, 1 };

            bool najdenSkok = false;

            foreach (int dx in dxSmeri)
            {
                foreach (int dy in dySmeri)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    bool nasprotnikNaSredi = false;
                    int preskocenX = -1, preskocenY = -1;

                    while (nx >= 0 && ny >= 0 && nx < Nastavitve.VelikostPlosce && ny < Nastavitve.VelikostPlosce)
                    {
                        Figura f = Figure.FirstOrDefault(fig => fig.X == nx && fig.Y == ny);

                        if (f == null)
                        {
                            if (nasprotnikNaSredi)
                            {
                                // če smo preskočili nasprotnika, lahko izvedemo skok
                                if (!obiskane.Contains((nx, ny)))
                                {
                                    var novaObiskana = new HashSet<(int, int)>(obiskane);
                                    novaObiskana.Add((nx, ny));
                                    if (ImaMoznoJemanjeRekurzivno(nx, ny, barva, novaObiskana))
                                        return true;
                                    // tudi če ni nadaljnjega skoka, že en skok zadostuje
                                    najdenSkok = true;
                                }
                            }
                        }
                        else
                        {
                            if (f.Barva == barva || nasprotnikNaSredi)
                                break; // naša figura ali že preskočen nasprotnik → prekinemo
                            nasprotnikNaSredi = true;
                            preskocenX = nx;
                            preskocenY = ny;
                        }

                        nx += dx;
                        ny += dy;
                    }
                }
            }

            return najdenSkok;
        }
        //preveri ali je možna poteza
        private bool ImaMoznoPotezo(Color barva)
        {
            foreach (var f in Figure.Where(x => x.Barva == barva))
            {
                for (int x = 0; x < velikostPlosce; x++)
                {
                    for (int y = 0; y < velikostPlosce; y++)
                    {
                        if (f.JePremikOk(x, y, Figure))
                            return true;
                    }
                }
            }
            return false;
        }
        //preveri ali je zmankalo figur, oziroma če ni več možnih potez
        private bool JeKonecIgre()
        {
            bool imaRdece = Figure.Any(f => f.Barva == Color.Red);
            bool imaModre = Figure.Any(f => f.Barva == Color.Blue);

            if (!imaRdece || !imaModre)
            {
                string zmagovalec = imaRdece ? "Rdeči" : "Modri";
                MessageBox.Show($"Konec igre! Zmagal je {zmagovalec}.");
                return true;
            }

            if (!ImaMoznoPotezo(trenutniIgralec))
            {
                string zmagovalec = (trenutniIgralec == Color.Red) ? "Modri" : "Rdeči";
                MessageBox.Show($"Konec igre! {zmagovalec} zmaga (nasprotnik nima potez).");
                return true;
            }

            return false;
        }
    }
}

