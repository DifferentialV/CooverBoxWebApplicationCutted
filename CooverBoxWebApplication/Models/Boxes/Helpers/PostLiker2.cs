using PDFCore;
using PDFCore.Graphic;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CooverBoxWebApplication.Models.Boxes.Helpers
{
    //алгоритм для упаковки коробочек
    public class PostLiker2
    {
        public PostLiker2(double boxX, double boxY, double boxH, double weight, int n)
        {
            Box = new Cub() { X = boxX, Y = boxY, H = boxH };
            Weight = weight;
            N = n;
        }
        //толщина стенок коробки (сумма противоположных)
        const double Otstup = 30;
        //толщина изолона (сумма противоположных)
        const double Isolon = 60;

        //тираж
        public readonly int N;
        //вес
        public readonly double Weight;
        public readonly Cub Box;

        //бандеролька
        public Package UKRPOST_B { get; set; }
        public Package UKRPOST_Badd { get; set; }

        //ящик Херсон до 20
        public Package Herson_G20 { get; set; }
        public Package Herson_G20add { get; set; }

        //ящик Херсон до 30
        public Package Herson_G30 { get; set; }
        public Package Herson_G30add { get; set; }

        //ящик Херсон до 20
        public Package Koktebel_G20 { get; set; }
        public Package Koktebel_G20add { get; set; }

        //ящик Херсон до 30
        public Package Koktebel_G30 { get; set; }
        public Package Koktebel_G30add { get; set; }


        //гофро ящик
        public Package Gofra { get; set; }
        public Package Gofraadd { get; set; }

        public void Work(string fullName = null, string path = null)
        {
            UKRPOST_B = UKRPOST("Бандеролька", BaseBox.GetParameter("Post", "bander_size"), BaseBox.GetParameter("Post", "bander_sizesum"), BaseBox.GetParameter("Post", "bander_weight"));
            if(UKRPOST_B!=null)
            {
                int n = N - UKRPOST_B.Count * UKRPOST_B.BoxInPackageCount;
                if (n > 0) UKRPOST_Badd = UKRPOST("Бандеролька+", BaseBox.GetParameter("Post", "bander_size"), BaseBox.GetParameter("Post", "bander_sizesum"), BaseBox.GetParameter("Post", "bander_weight"), n);
            }

            Cub herson = new Cub() { X = BaseBox.GetParameter("Post", "herson_x"), Y = BaseBox.GetParameter("Post", "herson_y"), H = BaseBox.GetParameter("Post", "herson_h") };
            double W20 = BaseBox.GetParameter("Post", "UKRG_20");
            double W30 = BaseBox.GetParameter("Post", "UKRG_30");

            Herson_G30 = Boxes(@"Ящик Херсон", new Cub() {X=herson.X,Y=herson.Y,H=herson.H }, W30);
            if(Herson_G30 != null)
            {
                int n = N - Herson_G30.Count * Herson_G30.BoxInPackageCount;
                //здача, если нужен не полный ящик
                if (n > 0) Herson_G30add = Boxes(@"Ящик Херсон+", new Cub() { X = herson.X, Y = herson.Y, H = herson.H }, W30, n);
            }
            if (Herson_G30 != null && Herson_G30.Weight > W20)
            {
                Herson_G20 = Boxes(@"Ящик Херсон", new Cub() { X = herson.X, Y = herson.Y, H = herson.H }, W20);
                if (Herson_G20 != null)
                {
                    int n = N - Herson_G20.Count * Herson_G20.BoxInPackageCount;
                    if (n > 0) Herson_G20add = Boxes(@"Ящик Херсон+", new Cub() { X = herson.X, Y = herson.Y, H = herson.H }, W20, n);
                }
            }


            Cub kotebl = new Cub() { X = BaseBox.GetParameter("Post", "koktebel_x"), Y = BaseBox.GetParameter("Post", "koktebel_y"), H = BaseBox.GetParameter("Post", "koktebel_h") };


            Koktebel_G30 = Boxes(@"Ящик Коктебель", new Cub() { X = kotebl.X, Y = kotebl.Y, H = kotebl.H }, W30);
            if (Koktebel_G30 != null)
            {
                int n = N - Koktebel_G30.Count * Koktebel_G30.BoxInPackageCount;
                if (n > 0) Koktebel_G30add = Boxes(@"Ящик Коктебель+", new Cub() { X = kotebl.X, Y = kotebl.Y, H = kotebl.H }, W30, n);
            }
            if (Koktebel_G30 != null && Koktebel_G30.Weight > W20)
            {
                Koktebel_G20 = Boxes(@"Ящик Коктебель", new Cub() { X = kotebl.X, Y = kotebl.Y, H = kotebl.H }, W20);
                if (Koktebel_G20 != null)
                {
                    int n = N - Koktebel_G20.Count * Koktebel_G20.BoxInPackageCount;
                    if (n > 0) Koktebel_G20add = Boxes(@"Ящик Коктебель+", new Cub() { X = kotebl.X, Y = kotebl.Y, H = kotebl.H }, W20, n);
                }
            }


            Gofra = GofraRaskroy("Гофра", BaseBox.GetParameter("Post", "gofra_x"), BaseBox.GetParameter("Post", "gofra_y"));
            if (Gofra != null)
            {
                int n = N - Gofra.Count * Gofra.BoxInPackageCount;
                if (n > 0) Gofraadd = GofraRaskroy("Гофра+", BaseBox.GetParameter("Post", "gofra_x"), BaseBox.GetParameter("Post", "gofra_y"), n);
            }


            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(path)) return;
            PdfDocument document = new PdfDocument();
            addpage(UKRPOST_B, UKRPOST_Badd);
            addpage(Herson_G20, Herson_G20add);
            addpage(Herson_G30, Herson_G30add);
            addpage(Koktebel_G20, Koktebel_G20add);
            addpage(Koktebel_G30, Koktebel_G30add);
            addpage(Gofra, Gofraadd);
            if(Gofra != null)
            {
                Page page = new Page();
                page.Add(KroyGroupGofra(Gofra.Size.X, Gofra.Size.Y, Gofra.Size.H));
                page.Add(Tools.AddLabel(page, "Гофра крой"));
                page.SetLocation(10, 10);
                page.ToPDFSharp(document);
            }
            if (Gofraadd != null)
            {
                Page page = new Page();
                page.Add(KroyGroupGofra(Gofraadd.Size.X, Gofraadd.Size.Y, Gofraadd.Size.H));
                page.Add(Tools.AddLabel(page, "Гофра крой+"));
                page.SetLocation(10, 10);
                page.ToPDFSharp(document);
            }
            document.Save($"{path}\\Упаковка {fullName}.pdf");

            void addpage(Package package, Package add)
            {
                if (package != null)
                {
                    Page page = new Page();
                    Group group = package.ToPDFSharp();
                    group.SetLocation(10, 10);
                    page.Add(group);
                    if (add != null)
                    {
                        Group group1 = add.ToPDFSharp();
                        group1.SetLocation(group.Right + 20, 10);
                        page.Add(group1);
                    }
                    page.SetLocation(10, 10);
                    page.ToPDFSharp(document);
                }
            }
        }



        //упаковка в ящик
        //name имя упаковки
        //maxweight максимально допустимый вес
        //N сколько разложить, если нулл то весь тираж
        Package Boxes(string name, Cub size, double maxweight, int? N = null)
        {
            //уменьшаем на толщину стенок ящика
            size.X -= Otstup;
            size.Y -= Otstup;
            size.H -= Otstup;
            //запас по весу
            maxweight /= 1.1;
            if (N == null) N = this.N;
            Package package = new Package();
            List<BoxInPackage> WS = new List<BoxInPackage>() { new BoxInPackage() };
            WS[0].positionX = WS[0].positionY = WS[0].positionH = Otstup / 2;
            WS[0].Size = new Cub() { X = size.X, Y = size.Y, H = size.H };
            WS[0].Xn = WS[0].Yn = WS[0].Hn = 1;
            do
            {
                //удалить области в которые не влезит ни одна коробка, как никрути
                WS.RemoveAll(w => Box.Variats().All(b => b.X > w.Length || b.Y > w.Width || b.H > w.Height));
                //если некуда ложить коробку то все
                if (WS.Count == 0) break;
                //если есть области которые можно обьединить
                List<BoxInPackage> temp;
                for (int i = 0; i < WS.Count; i++)
                {
                    temp = new List<BoxInPackage>() { new BoxInPackage() };
                    foreach (var block in WS.Where(s => s != WS[i] && s.positionY == WS[i].positionY && s.positionH == WS[i].positionH && s.Width == WS[i].Width && s.Height == WS[i].Height))
                    {
                        WS[i].Size.X += block.Length;
                        temp.Add(block);
                    }
                    foreach (var item in temp)
                        WS.Remove(item);
                    temp = new List<BoxInPackage>() { new BoxInPackage() };
                    foreach (var block in WS.Where(s => s != WS[i] && s.positionX == WS[i].positionX && s.positionH == WS[i].positionH && s.Length == WS[i].Length && s.Height == WS[i].Height))
                    {
                        WS[i].Size.Y += block.Width;
                        temp.Add(block);
                    }
                    foreach (var item in temp)
                        WS.Remove(item);
                    temp = new List<BoxInPackage>() { new BoxInPackage() };
                    foreach (var block in WS.Where(s => s != WS[i] && s.positionX == WS[i].positionX && s.positionY == WS[i].positionY && s.Length == WS[i].Length && s.Width == WS[i].Width))
                    {
                        WS[i].Size.H += block.Height;
                        temp.Add(block);
                    }
                    foreach (var item in temp)
                        WS.Remove(item);
                    temp = new List<BoxInPackage>() { new BoxInPackage() };
                }
                //выбераем область которая минимум по Высоте по Длине по Ширине (сортировка в таком порядке)
                WS.Sort(new CubUseComparer());

                //набор возможных упаковок, крутим коробочку как можем
                List<BoxInPackage> ncubs = new List<BoxInPackage>();
                foreach (var box in Box.Variats())
                {
                    ncubs.AddRange(Kroy(box, WS[0].Size, maxweight - Weight * package.BoxInPackageCount));
                }
                //выбераем Упаковку, котора ближе всего к N, чтобы не упаковать лишние
                //если таких нету то выбираем ту в которой больше всего коробочек
                BoxInPackage nCub = ncubs.Where(b => b.N >= N - package.BoxInPackageCount).OrderBy(b => b.N).FirstOrDefault();
                if (nCub == null) nCub = ncubs.OrderByDescending(b => b.N).FirstOrDefault();
                if (nCub == null) break;
                nCub.positionX = WS[0].positionX;
                nCub.positionY = WS[0].positionY;
                nCub.positionH = WS[0].positionH;
                //добавляем Упаковку
                package.Boxes.Add(nCub);
                if (package.BoxInPackageCount >= N) break;

                //создаем новые области, после Упаковки в данную область создаются области с остатка по высоте ширине и длине
                WS.Add(new BoxInPackage() { Xn = 1, Yn = 1, Hn = 1, positionX = WS[0].positionX, positionY = nCub.positionY1, positionH = WS[0].positionH, Size = new Cub() { X = WS[0].Size.X, Y = WS[0].Size.Y - nCub.Width, H = WS[0].Size.H} });
                WS.Add(new BoxInPackage() { Xn = 1, Yn = 1, Hn = 1, positionX = nCub.positionX1, positionY = WS[0].positionY, positionH = WS[0].positionH, Size = new Cub() { X = WS[0].Size.X- nCub.Length, Y = WS[0].Size.Y, H = WS[0].Size.H} });
                WS.Add(new BoxInPackage() { Xn = 1, Yn = 1, Hn = 1, positionX = WS[0].positionX, positionY = WS[0].positionY, positionH = nCub.positionH1, Size = new Cub() { X = WS[0].Size.X, Y = WS[0].Size.Y, H = WS[0].Size.H - nCub.Height } });
                //удаляем тикущую область из списка
                WS.RemoveAt(0);
            }
            while (true);
            //ничего не влезло
            if (package.Boxes.Count == 0 || package.BoxInPackageCount == 0) return null;
            //размер коробки, добавляем толщину картона и изолон
            package.Size = new Cub() { X = size.X + Otstup + Isolon, Y = size.Y + Otstup + Isolon, H = size.H + Otstup + Isolon };
            package.Weight = Math.Min(package.BoxInPackageCount,N.Value) * Weight * 1.1;
            package.Volume = (package.Size.X / 1000) * (package.Size.Y / 1000) * (package.Size.H / 1000) * 1.1;
            package.Count = (int)Math.Max(Math.Floor((double)N / package.BoxInPackageCount), 1);
            package.Name = name;
            return package;
        }
        //бандеролька
        //name имя упаковки
        //maxweight максимально допустимый вес
        //N сколько разложить, если нулл то весь тираж
        //sizemax максимум для одной стороны
        //maxweight максимум для суммы всех сторон
        Package UKRPOST(string name, double sizemax, double sizesum, double maxweight, int? N = null)
        {
            //берем запас для изолона
            sizemax -= Isolon;
            sizesum -= 3 * Isolon;
            maxweight /= 1.1;

            if (N == null) N = this.N;
            //делаем крой
            //при этом коробку не крутив как хочется
            List<BoxInPackage> ncubs = Kroy(Box, sizemax, sizemax, sizemax, maxweight,sizesum);
            //выбираем
            //сначала: чтобы высота не была больше ширины и длины, количество упакованых больше или равно N и минимальное количество (чтобы не упаковать лишние)
            //потом: тоже самое только максимум коробок в посылке
            //потом: коробки в один ряд, с минимум возмодного
            //потом: коробки в один ряд, по максимуму
            BoxInPackage nCub = ncubs.Where(b => b.Length >= b.Height && b.Width >= b.Height).Where(b => b.N >= N).OrderBy(b => Math.Abs(b.Hn - b.Yn)).FirstOrDefault();
            if (nCub == null) nCub = ncubs.Where(b => b.Length >= b.Height && b.Width >= b.Height).OrderBy(b => (int)N / b.N).ThenBy(b => Math.Abs(b.Hn - b.Yn)).FirstOrDefault();
            if (nCub == null) nCub = ncubs.Where(b => b.Hn == 1).Where(b => b.N >= N).OrderBy(b => b.N).FirstOrDefault();
            if (nCub == null) nCub = ncubs.Where(b => b.Hn == 1).OrderBy(b => (int)N / b.N).ThenBy(b => Math.Abs(b.Hn - b.Yn)).FirstOrDefault();
            if (nCub == null) return null;
            Package package = new Package();
            nCub.positionX = nCub.positionY = nCub.positionH = Isolon / 2;
            package.Boxes.Add(nCub);

            //добрасываем на верх (сообще не надо, но для бандерольки не страшно
            if (package.BoxInPackageCount < N)
            {
                ncubs = new List<BoxInPackage>();
                foreach (var box in Box.Variats())
                {
                    ncubs.AddRange(Kroy(box, package.Boxes[0].Length, package.Boxes[0].Width, sizemax - package.Boxes[0].Height, maxweight - package.Boxes[0].N * Weight, sizesum - package.Boxes[0].Length - package.Boxes[0].Width - package.Boxes[0].Height));
                }

                nCub = ncubs.Where(b=>b.Length >= b.Height && b.Width >= b.Height).Where(b => b.N >= (N - package.BoxInPackageCount)).OrderBy(b => b.N).ThenBy(b => Math.Abs(b.Xn - b.Yn)).FirstOrDefault();
                if (nCub == null) nCub = ncubs.Where(b => b.Length >= b.Height && b.Width >= b.Height).OrderByDescending(b => b.N).ThenBy(b => Math.Abs(b.Xn - b.Yn)).FirstOrDefault();
                if (nCub != null)
                {
                    nCub.positionX = nCub.positionY = nCub.positionH = Isolon / 2;
                    nCub.positionH = package.Boxes[0].positionH1;
                    package.Boxes.Add(nCub);
                }

            }
            package.Size = new Cub() { X = package.Boxes.Max(b => b.positionX1) + Isolon / 2, Y = package.Boxes.Max(b => b.positionY1) + Isolon / 2, H = package.Boxes.Max(b => b.positionH1) + Isolon / 2 };
            package.Weight = package.BoxInPackageCount * Weight * 1.1;
            package.Volume = (package.Size.X / 1000) * (package.Size.Y / 1000) * (package.Size.H / 1000) * 1.1;
            package.Count = (int)Math.Max(Math.Floor((double)N / package.BoxInPackageCount), 1);
            package.Name = name;
            return package;
        }
        //раскрой для гофры
        //sizeX длина гофролиста
        //sizeY ширина гофролиста
        Package GofraRaskroy(string name, double sizeX, double sizeY, int? N = null)
        {
            //запас на толщину картона
            sizeX -= 40 + Otstup*4;
            sizeY -= Otstup;
            if (N == null) N = this.N;

            //поворачиваем коробку как хочется
            //подбираем все возможные раскроии, с учетов того влезит ли раскрой гофрокорбки на лист 
            List<BoxInPackage> ncubs = new List<BoxInPackage>();
            foreach (var box in Box.Variats())
            {
                int Xn = (int)Math.Floor((sizeX - 2 * box.Y) / (2 * box.X));
                int Yn = (int)Math.Floor((sizeX - 2 * box.X) / (2 * box.Y));
                int Hn = (int)Math.Floor((sizeY - box.Y) / (box.H));

                for (int i = 1; i <= Xn; i++)
                {
                    for (int ii = 1; ii <= Yn; ii++)
                    {
                        for (int iii = 1; iii <= Hn; iii++)
                        {
                            if (2 * i * box.X + 2 * ii * box.Y <= sizeX && iii * box.H + ii * box.Y <= sizeY)
                                ncubs.Add(new BoxInPackage() { Xn = i, Yn = ii, Hn = iii, Size = box });
                            else break;
                        }
                    }
                }
            }
            //логика как в бандеролке
            BoxInPackage nCub = ncubs.Where(b => b.Length >= b.Height && b.Width >= b.Height).Where(b => b.N >= N).OrderBy(b => b.N).FirstOrDefault();
            if (nCub == null) nCub = ncubs.Where(b => b.Length >= b.Height && b.Width >= b.Height).OrderByDescending(b => b.N).FirstOrDefault();
            if (nCub == null) nCub = ncubs.Where(b => b.Hn == 1).Where(b => b.N >= N).OrderBy(b => b.N).FirstOrDefault();
            if (nCub == null) nCub = ncubs.Where(b => b.Hn == 1).OrderByDescending(b => b.N).FirstOrDefault();
            if (nCub == null) return null;

            Package package = new Package();
            nCub.positionX = nCub.positionY = nCub.positionH = Otstup / 2;
            package.Boxes.Add(nCub);

            package.Size = new Cub() { X = package.Boxes.Max(b => b.positionX1) + Otstup / 2 + Isolon, Y = package.Boxes.Max(b => b.positionY1) + Otstup / 2 + Isolon, H = package.Boxes.Max(b => b.positionH1) + Otstup / 2 + Isolon };
            package.Weight = package.BoxInPackageCount * Weight * 1.1;
            package.Volume = (package.Size.X / 1000) * (package.Size.Y / 1000) * (package.Size.H / 1000) * 1.1;
            package.Count = (int)Math.Max(Math.Floor((double)N / package.BoxInPackageCount), 1);
            package.Name = name;
            return package;
        }

        List<BoxInPackage> Kroy(Cub box, double sizeX, double sizeY, double sizeH, double maxweight, double sizesum = double.MaxValue)
        {
            return Kroy(box, new Cub() { X = sizeX, Y = sizeY, H = sizeH }, maxweight, sizesum);
        }
        //возращает все возможные Упаковки которые взазять в пределы
        List<BoxInPackage> Kroy(Cub box, Cub size, double maxweight, double sizesum = double.MaxValue)
        {
            int Xn = (int)Math.Floor(size.X / box.X);
            int Yn = (int)Math.Floor(size.Y / box.Y);
            int Hn = (int)Math.Floor(size.H / box.H);
            int Mn = (int)Math.Floor(maxweight / Weight);

            List<BoxInPackage> ncubs = new List<BoxInPackage>();
            for (int i = 1; i <= Xn; i++)
            {
                for (int ii = 1; ii <= Yn; ii++)
                {
                    for (int iii = 1; iii <= Hn; iii++)
                    {
                        if (i * box.X + ii * box.Y + iii * box.H <= sizesum && i * ii * iii <= Mn)
                            ncubs.Add(new BoxInPackage() { Xn = i, Yn = ii, Hn = iii, Size = box });
                        else break;
                    }
                }
            }
            return ncubs;
        }

        //грой гофрокоробки для PDF файла
        Group KroyGroupGofra(double X, double Y, double H)
        {
            double b = Y / 2;
            Group group = new Group();

            Contur contur = new Contur();
            contur.Add(0, 0);
            contur.AddPoint(0, b);
            contur.AddPoint(X - 2.5, 0);
            contur.AddPoint(0, -b);
            contur.AddPoint(5, 0);
            contur.AddPoint(0, b);
            contur.AddPoint(Y - 5, 0);
            contur.AddPoint(0, -b);
            contur.AddPoint(5, 0);
            contur.AddPoint(0, b);
            contur.AddPoint(X - 5, 0);
            contur.AddPoint(0, -b);
            contur.AddPoint(5, 0);
            contur.AddPoint(0, b);
            contur.AddPoint(Y - 5, 0);
            contur.AddPoint(0, -b);
            contur.AddPoint(40, -40 * Math.Tan(Math.PI/10));
            {
                Contur clone = contur.Clone();
                clone.MirrorX();
                clone.SetLocation(0, -H, 0);
                contur.Add(clone, 'r');
            }
            group.Add(contur);
            {
                group.Add(Contur.Line(0, 0, 2 * X + 2 * Y - 2.5, 0));
                group.Add(Contur.Line(X, 0, X, -H));
                group.Add(Contur.Line(X + Y, 0, X + Y, -H));
                group.Add(Contur.Line(2 * X + Y, 0, 2 * X + Y, -H));
                group.Add(Contur.Line(2 * X + 2 * Y - 2.5, 0, 2 * X + 2 * Y - 2.5, -H));
                group.Add(Contur.Line(0, -H, 2 * X + 2 * Y - 2.5, -H));
            }
            for (int i = 1; i < 7; i++)
            {
                ((Contur)group.GetObject(i)).Color = PdfSharpCore.Drawing.XColors.Red;
            }
            //group.Add(Models.Boxes.BaseBox.AddLineDimension(contur));
            group.SetLocation(0, 0);
            return group;
        }


        class CubUseComparer : IComparer<BoxInPackage>
        {
            public int Compare(BoxInPackage c1, BoxInPackage c2)
            {
                if (c1.positionH < c2.positionH)
                    return 1;
                else if (c1.positionH > c2.positionH)
                    return -1;
                else if (c1.positionX < c2.positionX)
                    return 1;
                else if (c1.positionX > c2.positionX)
                    return -1;
                else if (c1.positionY < c2.positionY)
                    return 1;
                else if (c1.positionY > c2.positionY)
                    return -1;
                else
                    return 0;
            }
        }

    }


    public class Cub
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double H { get; set; }

        public List<Cub> Variats()
        {
            List<Cub> cubs = new List<Cub>()
            {
                new Cub(){X=X, Y=Y, H=H},
                new Cub(){X=X, Y=H, H=Y},
                new Cub(){X=Y, Y=X, H=H},
                new Cub(){X=Y, Y=H, H=X},
                new Cub(){X=H, Y=X, H=Y},
                new Cub(){X=H, Y=Y, H=X},
            };
            return cubs;
        }
    }

    public class Package
    {
        public Package()
        {
            Boxes = new List<BoxInPackage>();
        }
        public Cub Size { get; set; }
        public double Volume { get; set; }
        public double Weight { get; set; }
        public int Count { get; set; }
        public int BoxInPackageCount { get { return Boxes.Sum(b => b.N); } }

        public List<BoxInPackage> Boxes { get; set; }

        public string Name { get; set; }

        public Group ToPDFSharp()
        {
            Group group = new Group();
            group.Add(Contur.Line25D(0, 0, 0, 0, 0, Size.H+50));
            group.Add(Contur.Line25D(0, 0, 0, 0, Size.Y+50, 0));
            group.Add(Contur.Line25D(0, 0, 0, Size.X+50, 0, 0));
            group.Add(Contur.Line25D(Size.X, 0, 0, Size.X, 0, Size.H));
            group.Add(Contur.Line25D(0, 0, Size.H, Size.X, 0, Size.H));
            group.Add(Contur.Line25D(0, Size.Y, 0, 0, Size.Y, Size.H));
            group.Add(Contur.Line25D(0, 0, Size.H, 0, Size.Y, Size.H));
            group.Add(Contur.Line25D(Size.X, 0, 0, Size.X, Size.Y, 0));
            group.Add(Contur.Line25D(0, Size.Y, 0, Size.X, Size.Y, 0));
            for (int i = 0; i < Boxes.Count; i++)
            {
                group.Add(Boxes[i].ToPDFSharp(xColors[i % xColors.Count]));
            }
            group.Add(Tools.AddLabel(group, $"кробочек в посылке {BoxInPackageCount} Посылок {Count} Общий объем {Math.Round(Volume * Count, 3)} м3"));
            group.Add(Tools.AddLabel(group, $"{Name} {Math.Round(Size.X, 1)}x{Math.Round(Size.Y, 1)}x{Math.Round(Size.H, 1)} {Math.Round(Weight / 1000, 3)} кг Объем {Math.Round(Volume, 6)} м3"));
            group.SetLocation(10, 10);

            return group;
        }



        static readonly List<XColor> xColors = new List<XColor> { XColors.DarkRed, XColors.Blue, XColors.Green, XColors.Orange, XColors.Brown,
            XColors.BlueViolet, XColors.Gold, XColors.DarkBlue, XColors.DarkGoldenrod, XColors.OrangeRed, XColors.DarkGreen, XColors.Magenta,
            XColors.Red, XColors.DarkGreen, XColors.CadetBlue, XColors.DarkMagenta, XColors.LimeGreen, XColors.OliveDrab, XColors.DeepPink, XColors.Turquoise };
    }

    public class BoxInPackage
    {
        public double positionX { get; set; }
        public double positionY { get; set; }
        public double positionH { get; set; }
        public Cub Size { get; set; }
        
        public int Xn { get; set; }
        public int Yn { get; set; }
        public int Hn { get; set; }

        public double Length { get { return Size.X * Xn; } }
        public double Width { get { return Size.Y * Yn; } }
        public double Height { get { return Size.H * Hn; } }

        public double positionX1 { get { return positionX + Length; } }
        public double positionY1 { get { return positionY + Width; } }
        public double positionH1 { get { return positionH + Height; } }

        public int N  {get{ return Xn * Yn * Hn; } }

        public Group ToPDFSharp(XColor color)
        {
            Group group = new Group();
            group.Add(Contur.Line25D(positionX1, positionY, positionH, positionX1, positionY1, positionH, 0.5));
            group.Add(Contur.Line25D(positionX1, positionY1, positionH, positionX, positionY1, positionH, 0.5));
            group.Add(Contur.Line25D(positionX, positionY, positionH1, positionX1, positionY, positionH1, 0.5));
            group.Add(Contur.Line25D(positionX, positionY, positionH1, positionX, positionY1, positionH1, 0.5));
            group.Add(Contur.Line25D(positionX1, positionY, positionH1, positionX1, positionY1, positionH1, 0.5));
            group.Add(Contur.Line25D(positionX1, positionY1, positionH1, positionX, positionY1, positionH1, 0.5));
            group.Add(Contur.Line25D(positionX1, positionY, positionH, positionX1, positionY, positionH1, 0.5));
            group.Add(Contur.Line25D(positionX, positionY1, positionH, positionX, positionY1, positionH1, 0.5));
            group.Add(Contur.Line25D(positionX1, positionY1, positionH, positionX1, positionY1, positionH1, 0.5));
            for (int i = 0; i < group.Count; i++)
            {
                ((Contur)group.GetObject(i)).Color = color;
            }
            GraphicGroup conturs = new GraphicGroup();
            conturs.Add(Contur.Line25D(Size.X, 0, 0, Size.X, Size.Y, 0));
            conturs.Add(Contur.Line25D(Size.X, Size.Y, 0, 0, Size.Y, 0));

            conturs.Add(Contur.Line25D(0, 0, Size.H, Size.X, 0, Size.H));
            conturs.Add(Contur.Line25D(0, 0, Size.H, 0, Size.Y, Size.H));
            conturs.Add(Contur.Line25D(Size.X, 0, Size.H, Size.X, Size.Y, Size.H));
            conturs.Add(Contur.Line25D(Size.X, Size.Y, Size.H, 0, Size.Y, Size.H));

            conturs.Add(Contur.Line25D(Size.X, 0, 0, Size.X, 0, Size.H));
            conturs.Add(Contur.Line25D(0, Size.Y, 0, 0, Size.Y, Size.H));
            conturs.Add(Contur.Line25D(Size.X, Size.Y, 0, Size.X, Size.Y, Size.H));
            for(int i=0;i<conturs.Count;i++)
            {
                ((Contur)conturs.GetObject(i)).Color = color;
            }
            for (int i = 0; i < Xn; i++)
            {
                Contur range = Contur.Line25D(0, 0, 0, positionX + i * Size.X, positionY, positionH);
                GraphicGroup clone = conturs.Clone();
                clone.Move(range.GetPoint(1).X, range.GetPoint(1).Y);
                group.Add(clone);
            }
            for (int i = 1; i < Yn; i++)
            {
                Contur range = Contur.Line25D(0, 0, 0, positionX, positionY + i * Size.Y, positionH);
                GraphicGroup clone = conturs.Clone();
                clone.Move(range.GetPoint(1).X, range.GetPoint(1).Y);
                group.Add(clone);
            }

            return group;
        }
    }


}
