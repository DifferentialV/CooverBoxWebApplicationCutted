using PDFCore;
using PDFCore.Graphic;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models.Boxes.Helpers
{
    //разкрой для бумаги и картона
    public class BoxListRaskroy
    {
        //материалы
        private List<BoxMaterial> BoxMaterials;
        //контуры
        private List<BoxPart> Parts;
        //связь повората контура, если надо сохранить перекрест волокна
        private List<BoxRotate> BoxRotates;
        public int N { get; set; }
        public double Defect_percent { get; set; }
        public double Otstup { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        //вес
        public double Weight { get { return Parts.Sum(p => p.Weight); } }
        public BoxListRaskroy()
        {
            BoxMaterials = new List<BoxMaterial>();
            Parts = new List<BoxPart>();
            BoxRotates = new List<BoxRotate>();
        }
        public void AddMaterial(string Name, dynamic Material,string DisplayName = null)
        {
            if(! (new[] { typeof(CoverCarton), typeof(DesignPaper) }.Contains((Type)Material.GetType()))) throw new System.FormatException("9595");
            if (string.IsNullOrEmpty(DisplayName)) DisplayName = Material.ToString();
            if (BoxMaterials.Any(m => m.Name == Name)) throw new System.FormatException("9595");
            BoxMaterials.Add(new BoxMaterial(BoxMaterials.Count, this) { Name = Name, Material = Material,DisplayName = DisplayName });
        }
        public void AddPart(Graphics Graphics, string MaterialName, string Way, bool? Rotate = null, string RotateName = null)
        {
            int? matrialid = BoxMaterials.FirstOrDefault(m => m.Name == MaterialName)?.Id;
            if (matrialid == null) throw new System.FormatException("9595");
            int rotateid = -1;
            if (!string.IsNullOrEmpty(RotateName))
            {
                int? rotateid1 = BoxRotates.FirstOrDefault(m => m.Name == RotateName)?.Id;
                if (rotateid1 == null) throw new System.FormatException("9595");
                rotateid = rotateid1.Value;
            }
            Parts.Add(new BoxPart(Parts.Count, this) {Rotate = Rotate, MaterialId = matrialid.Value, Graphics = Graphics.Clone(), Way = Way, RotateId = rotateid });
        }
        public void AddRotate(string Name)
        {
            if (BoxRotates.Any(r => r.Name == Name)) throw new System.FormatException("9595");
            BoxRotates.Add(new BoxRotate(BoxRotates.Count, this, Name));
        }

        //выбор формата полноцветной печати для всего тиража коробок
        private string Fullcolor()
        {
            //по умолчанию индиго
            string Fullcolorresult = "NONOfset";
            foreach (var material in BoxMaterials.Where(m => m.Material.GetType() == typeof(DesignPaper) && m.Material.FullColor))
            {
                //если Понтон то всегда формат А1
                if(material.Material.Name.Contains("Ponton"))
                    Fullcolorresult = "A1";
                foreach (var part in material.Parts)
                {
                    double width = part.Graphics.Width + Otstup;
                    double height = part.Graphics.Height + Otstup;
                    var temp = (part.Rotate) switch
                    {
                        (null) => Math.Max(Math.Floor(480 / (height) * Math.Floor(680 / (width))), Math.Floor(480 / (width) * Math.Floor(680 / (height)))),
                        (true) => Math.Floor(480 / (height) * Math.Floor(680 / (width))),
                        (false) => Math.Floor(480 / (width) * Math.Floor(680 / (height))),
                    };
                    double lists = Math.Ceiling(N / temp);
                    if (Fullcolorresult == "NONOfset")
                    {
                        if (temp == 0 || lists > 300)
                        {
                            Fullcolorresult = "A2";
                        }
                    }
                    if (Fullcolorresult == "A2")
                    {

                        if (temp == 0 || lists > 1000)
                        {
                            Fullcolorresult = "A1";
                        }
                    }
                    if (Fullcolorresult == "A1") continue;
                }
                if (Fullcolorresult == "A1") continue;
            }
            return Fullcolorresult;
        }
        public List<DiffPage> Work()
        {
            //список возможных сочитаний поворотов
            List<List<bool>> rotatesqueue = new List<List<bool>>();
            {
                rotatesqueue.Add(new List<bool>());
                for (int j = 0; j < BoxRotates.Count; j++)
                {
                    rotatesqueue[^1].Add(false);
                }
                int ineer = 1;
                for (int i = 1; i < Math.Pow(2, BoxRotates.Count); i++)
                {
                    rotatesqueue.Add(new List<bool>());
                    int ineer1 = 1;
                    for (int j = 0; j < BoxRotates.Count; j++)
                    {
                        rotatesqueue[^1].Add(!((ineer & ineer1) == 0));
                        ineer1 += (int)Math.Pow(2, j);
                    }
                    ineer++;
                }
            }
            List<BoxMaterial> materialsQueue = new List<BoxMaterial>();
            //cпосок материалов без посторения
            foreach (var material in BoxMaterials)
            {
                if (materialsQueue.All(m => (m.Material.Id != material.Material.Id && m.Material.Name != material.Material.Name) || m.Material.GetType() != material.Material.GetType()))
                    materialsQueue.Add(material);
            }
            //список листов с разкроем
            List<DiffPage> pages=null;
            //проходим по всем комбинациям поворотов
            foreach (var rotates in rotatesqueue)
            {
                List<DiffPage> pagestemp = new List<DiffPage>();
                for (int i = 0; i < rotates.Count; i++)
                {
                    BoxRotates[i].Rotate = rotates[i];
                }
                string FullColorFormat = Fullcolor();
                foreach(var material in materialsQueue)
                {
                    string name = material.DisplayName;
                    string type = "картон";
                    if (material.Material.GetType() == typeof(DesignPaper)) {type = (material.Material.Sticker)? "бумага":"";}
                    if (material.Material.GetType() == typeof(DesignPaper) && material.Material.FullColor)
                    {
                        name = (FullColorFormat) switch
                        {
                            ("NONOfset") => $"Индиго {name}",
                            ("A2") => $"Офсет A2 {name}",
                            ("A1") => $"Офсет A1 {name}",
                            _ => $"{name}"
                        };
                    }
                    //область раскрой, для полноцвета константа
                    DiffContur recta = new DiffContur() { Width = material.Material.X, Height = material.Material.Y };
                    if (material.Material.GetType() == typeof(DesignPaper) && material.Material.FullColor)
                    {
                        recta = (FullColorFormat) switch
                        {
                            ("NONOfset") => new DiffContur() { Width = 480, Height = 680},
                            ("A2") => new DiffContur() { Width = 480, Height = 680},
                            ("A1") => new DiffContur() { Width = 680, Height = 980},
                            _ => new DiffContur() {Width = material.Material.X, Height = material.Material.Y }
                        };
                    }
                    //отступ для картона минимум 10 мм
                    double Otstup = this.Otstup;/////////////////////////////////////////////////////////////
                    if(type == "картон") {Otstup = 10;}
                    ////////////////////////////////////////////////////////////////////////////////////////
                    //собираем пакет контуров на раскрой
                    List<DiffContur> diffConturs = new List<DiffContur>();
                    {
                        foreach(var materi in BoxMaterials.Where(m => m.Material.Id == material.Material.Id))
                        {
                            //добавлям отступ
                            foreach(var part in materi.Parts)
                            {
                                Graphics contur = part.Graphics.Clone();
                                double W = part.Graphics.Width + Otstup;
                                double H = part.Graphics.Height + Otstup;
                                switch (part.Rotate)
                                {
                                    case true:double temp = W;W = H;H = temp; contur.Rotate(Math.PI / 2); break;
                                    case false: break;
                                    case null:if((Math.Floor((recta.Width-Otstup)/W)* Math.Floor((recta.Height - Otstup) / H) )< (Math.Floor((recta.Width - Otstup) / H) * Math.Floor((recta.Height - Otstup) / W))){ double temp1 = W; W = H; H = temp1; contur.Rotate(Math.PI / 2); } break;
                                }
                                diffConturs.Add(new DiffContur() { Width = W, Height = H, Way = part.Way, Graphics = contur, X = 0, Y = 0 });
                            }
                        }
                    }
                    //сам раскрой
                    Raskroy(recta, diffConturs,Otstup, out List<DiffPage> outpages);
                    int countA1Off = (material.Material.GetType() == typeof(DesignPaper) && material.Material.FullColor) ? outpages.Sum(o => o.Count) : 0;
                    int countsum = outpages.Sum(p => (int)Math.Ceiling(p.Count * Defect_percent));
                    //установка цены, на полноцвет свои приколы
                    for (int i=0;i<outpages.Count;i++)
                    {
                        int Count = (int)Math.Ceiling(outpages[i].Count * Defect_percent);
                        double Price = Count * material.Material.Price;
                        if (material.Material.GetType() == typeof(DesignPaper) && material.Material.FullColor)
                        {
                            string ofsettype = (material.DisplayName.Contains("Ponton")) ? "Ponton" : "CMYK";
                            if (i == 0)
                            {
                                Price = (FullColorFormat) switch
                                {
                                    ("NONOfset") => BaseBox.GetParameter("FullColor", $"NONOfsetP") +  countsum * BaseBox.GetParameter("FullColor", $"NONOfset"),
                                    ("A2") => Math.Max(countsum * BaseBox.GetParameter("FullColor", $"A2_{ofsettype}"), BaseBox.GetParameter("FullColor", $"A2_{ofsettype}_min")),
                                    ("A1") => BaseBox.GetParameter("FullColor", $"A1_b500_{ofsettype}") + ((countsum > 500) ? (countsum - 500) * BaseBox.GetParameter("FullColor", $"A1_a500_{ofsettype}") : 0),
                                    _ => countsum * material.Material.Price
                                };
                                Price += countA1Off * material.Material.Price;
                            }
                            else
                            {
                                Price = 0;
                            }
                        }
                        //Price = Math.Round(Price * Defect_percent, 3);

                        outpages[i].Count = Count;
                        outpages[i].Price = Price;
                        outpages[i].MaterialName = name;
                        outpages[i].MaterialSpace = recta.Clone();
                        outpages[i].MaterialType = type;
                        string conturway = " ";
                        foreach(var level in outpages[i].Levels)
                        {
                            foreach(var contur in level.conturs)
                            {
                                if (!conturway.Contains(contur.Way))
                                    conturway += $"{contur.Way}/";
                            }
                        }
                        conturway = conturway.Remove(conturway.Length - 1).Trim();
                        outpages[i].ContursWay = conturway;
                        pagestemp.Add(outpages[i]);
                    }
                }

                //выбор лучшего раскроя
                //тот который не вышел за пределы листа
                //и по цене
                if (pages==null)
                {
                    pages = new List<DiffPage>();
                    foreach(var page in pagestemp)
                    {
                        pages.Add(page);
                    }
                }
                else if(pages.Any(p=>p.DefectKroy) && pagestemp.All(p => !p.DefectKroy))
                {
                    pages = new List<DiffPage>();
                    foreach (var page in pagestemp)
                    {
                        pages.Add(page);
                    }
                }
                else if(pages.All(p => !p.DefectKroy) && pagestemp.Any(p => p.DefectKroy))
                {
                    int yhyu = 0;
                }
                else if(pages.Sum(p=>p.Price) > pagestemp.Sum(p=>p.Price))
                {
                    pages = new List<DiffPage>();
                    foreach (var page in pagestemp)
                    {
                        pages.Add(page);
                    }
                }
                else if(pages.Sum(p => p.Price) == pagestemp.Sum(p => p.Price) && pages.Sum(p => p.Count) > pagestemp.Sum(p => p.Count))
                {
                    pages = new List<DiffPage>();
                    foreach (var page in pagestemp)
                    {
                        pages.Add(page);
                    }
                }
            }
            List<PdfSharpCore.Drawing.XColor> colorslist = new List<PdfSharpCore.Drawing.XColor> {
            PdfSharpCore.Drawing.XColors.Blue,
            PdfSharpCore.Drawing.XColors.LightBlue,
            PdfSharpCore.Drawing.XColors.BlueViolet,
            PdfSharpCore.Drawing.XColors.DarkBlue,
            PdfSharpCore.Drawing.XColors.LightSteelBlue,
            PdfSharpCore.Drawing.XColors.DarkBlue,
            PdfSharpCore.Drawing.XColors.Red,
            PdfSharpCore.Drawing.XColors.Yellow,
            PdfSharpCore.Drawing.XColors.Green,
            PdfSharpCore.Drawing.XColors.GreenYellow,
            PdfSharpCore.Drawing.XColors.LightYellow,
            };
            //создание PDF страниц
            for (int i=0;i<pages.Count;i++)
            {
                double Otstup = this.Otstup;
                if(pages[i].MaterialType == "картон"){Otstup = 10;}
                Page addpage = new Page();
                addpage.Add(Contur.Rectangle(0, 0, pages[i].MaterialSpace.Width, pages[i].MaterialSpace.Height));
                string way_temp = "";
                foreach (var level in pages[i].Levels)
                {
                    foreach (var contur in level.conturs)
                    {
                        Graphics temp = contur.Graphics.Clone();
                        if(temp.GetType() ==typeof(Contur)) ((Contur)temp).Color = colorslist[i%colorslist.Count];
                        temp.SetLocation(contur.Left + Otstup, contur.Top + Otstup);
                        addpage.Add(temp);
                    }
                    if(level.conturs.Count > 0)
                    {
                        if (way_temp != level.conturs[0].Way)
                            addpage.Add(Tools.AddLineDimension(addpage.GetObject(addpage.Count - 1), -15, offline: true));
                        way_temp = level.conturs[0].Way;
                    }
                }
                List<string> temp_raskroy = new List<string>() {""};
                foreach(string var in $"Расклад: {pages[i].MaterialType} {pages[i].ContursWay}".Split("/"))
                {
                    if (temp_raskroy[^1].Length + var.Length > 60)
                        temp_raskroy.Add(var);
                    else
                        temp_raskroy[^1] += $"{(temp_raskroy[^1].Length > 0 ? "/": " ")}{var}";
                }
                addpage.Add(Tools.AddLabel(addpage, $"Материал: {pages[i].MaterialName}", otstup: 2));
                addpage.Add(Tools.AddLabel(addpage, $"Лист: {pages[i].MaterialSpace.Width}x{pages[i].MaterialSpace.Height} {pages[i].Count} листов", otstup: 0));
                addpage.Add(Tools.AddLabel(addpage, $"Тираж: {N}", otstup: 0));

                temp_raskroy.Reverse();
                foreach(string var in temp_raskroy)
                    addpage.Add(Tools.AddLabel(addpage, var, otstup: 0));

                addpage.Add(Tools.AddLabel(addpage, $"{ProjectType}", otstup: 0));
                addpage.Add(Tools.AddLabel(addpage, $"Название проекта: {ProjectName}", otstup: 0));
                addpage.SetLocation(10, 10);
                pages[i].Page = addpage;
            }
            return pages;
        }
        //алгоритм раскроя
        private void Raskroy(DiffContur _recta,List<DiffContur> conturs,double Otstup, out List<DiffPage> diffPages)
        {
            DiffContur Recta = new DiffContur() { Width = _recta.Width-Otstup, Height = _recta.Height-Otstup };
            //сортируем по убыванию высоты
            conturs.Sort(new ConturComparer());
            //очередь на раскрой
            List<DiffLevel> queues = new List<DiffLevel>();
            //одинаковые по высоте детали идут вместе (рядом)
            foreach(var contur in conturs)
            {
                int integer = -1;
                for(int i=0;i<queues.Count;i++)
                {
                    if (queues[i].conturs.Count != 0 && queues[i].conturs[^1].Height == contur.Height && queues[i].conturs.Sum(c => c.Width) + contur.Width < Recta.Width)
                    {
                        integer = i;
                        break;
                    }
                }
                if (integer >= 0)
                {
                    queues[integer].Add(contur.Clone());
                }
                else
                {
                    queues.Add(new DiffLevel());
                    queues[^1].Add(contur.Clone());
                }
            }
            //копируем, чтобы занять ширину листа по максимуму
            for(int i=0;i<queues.Count;i++)
            {
                double w = queues[i].conturs.Sum(c => c.Width);
                int count = (int)Math.Truncate(Recta.Width / w);
                if(count>=2)
                {
                    List<DiffContur> temp = new List<DiffContur>();
                    foreach (var contur in queues[i].conturs)
                    {
                        temp.Add(contur.Clone());
                    }
                    for (int j=0;j<count-1;j++)
                    {
                        foreach (var contur in temp)
                        {
                            queues[i].conturs.Add(contur.Clone());
                        }
                    }
                }
                queues[i].Count = count;
            }
            //список раскроев
            diffPages = new List<DiffPage>();
            foreach(var queue in queues)
            {
                double w = queue.conturs.Sum(c => c.Width);
                //отступ чтобы поставить раскрой по центру 
                double addotstup = 0;
                if(Recta.Width - w > 0)
                {
                    addotstup = Math.Round((Recta.Width - w) / (queue.conturs.Count), 1);
                }
                //сколько надо разложить
                int N_temp = N;
                //обходим все готовые ракрои, пытаемся впихнуть снизу текущую очередь
                foreach(var page in diffPages)
                {
                    if (page.Levels.Max(l => l.Right) > Recta.Width || page.Levels.Max(l => l.Down) > Recta.Height) continue;
                    if(Recta.Height - page.Levels.Sum(l=>l.Height) >= queue.Height && Recta.Width - w > 0)
                    {
                        int countH = (int)Math.Floor((Recta.Height - page.Levels.Sum(l => l.Height)) / queue.Height);
                        for (int i = 0; i < countH; i++)
                        {
                            page.Levels.Add(new DiffLevel());
                            foreach (var contur in queue.conturs)
                            {
                                DiffContur temp = contur.Clone();
                                if (page.Levels[^1].conturs.Count == 0)
                                    temp.SetLocation(addotstup / 2, page.Levels.Max(l => l.Down));
                                else
                                    temp.SetLocation(page.Levels[^1].Right + addotstup, page.Levels[^1].Top);
                                page.Levels[^1].Add(temp);
                            }
                            N_temp -= queue.Count * page.Count;
                        }
                        if (N_temp <= 0) break;
                    }
                }
                //если все не влезло, то создаем новую страницу раскроя
                if (N_temp > 0)
                {
                    diffPages.Add(new DiffPage());
                    int countH = (int)Math.Floor(Recta.Height / queue.Height);
                    {
                        diffPages[^1].Levels.Add(new DiffLevel());
                        foreach (var contur in queue.conturs)
                        {
                            DiffContur temp = contur.Clone();
                            if (diffPages[^1].Levels[^1].conturs.Count == 0)
                                temp.SetLocation(addotstup / 2, diffPages[^1].Levels.Max(l => l.Down));
                            else
                                temp.SetLocation(diffPages[^1].Levels[^1].Right + addotstup, diffPages[^1].Levels[^1].Top);
                            diffPages[^1].Levels[^1].Add(temp);
                        }
                    }
                    for (int i = 1; i < countH; i++)
                    {
                        diffPages[^1].Levels.Add(new DiffLevel());
                        foreach (var contur in queue.conturs)
                        {
                            DiffContur temp = contur.Clone();
                            if (diffPages[^1].Levels[^1].conturs.Count == 0)
                                temp.SetLocation(addotstup / 2, diffPages[^1].Levels.Max(l => l.Down));
                            else
                                temp.SetLocation(diffPages[^1].Levels[^1].Right + addotstup, diffPages[^1].Levels[^1].Top);
                            diffPages[^1].Levels[^1].Add(temp);
                        }
                    }
                    //считаем сколько надо листов, и ставим DefectKroy если вылезли за пределы
                    if (queue.Count != 0 && countH != 0)
                        diffPages[^1].Count = (int)Math.Ceiling((double)N_temp / queue.Count / countH);
                    if (queue.Count == 0 && countH != 0)
                    { diffPages[^1].Count = (int)Math.Ceiling(1.5 * (double)N_temp / countH); diffPages[^1].DefectKroy = true; }
                    if (queue.Count != 0 && countH == 0)
                    { diffPages[^1].Count = (int)Math.Ceiling(1.5 * (double)N_temp / queue.Count); diffPages[^1].DefectKroy = true; }
                    if (queue.Count == 0 && countH == 0)
                    { diffPages[^1].Count = (int)Math.Ceiling(1.5 * (double)N_temp); diffPages[^1].DefectKroy = true; }
                }
            }
        }
        //сортировка по возрастанию высоты
        class ConturComparer : IComparer<DiffContur>
        {
            public int Compare(DiffContur c1, DiffContur c2)
            {
                if (c1.Height < c2.Height)
                    return 1;
                else if (c1.Height > c2.Height)
                    return -1;
                else
                    return 0;
            }
        }
        //один уровень раскроя с контурами
        public class DiffLevel
        {
            public readonly List<DiffContur> conturs = new List<DiffContur>();
            public int Count { get; set; } = 1;
            public void Add(DiffContur contur)
            {
                conturs.Add(contur);
            }
            public double Left { get { if (conturs.Count == 0) return 0; return conturs.Min(c => c.Left); } }
            public double Right { get { if (conturs.Count == 0) return 0; return conturs.Max(c => c.Right); } }
            public double Top { get { if (conturs.Count == 0) return 0; return conturs.Min(c => c.Top); } }
            public double Down { get { if (conturs.Count == 0) return 0; return conturs.Max(c => c.Down); } }
            public double Height { get { if (conturs.Count == 0) return 0; return Down - Top; } }
            public double Width { get { if (conturs.Count == 0) return 0; return Right - Left; } }
        }
        //обертка над контуром
        public class DiffContur
        {
            public double X { get; set; } = 0;
            public double Y { get; set; } = 0;
            public double Height { get; set; } = 0;
            public double Width { get; set; } = 0;
            public void SetLocation(double x, double y)
            {
                X = x;
                Y = y;
            }
            public double Left { get { return X; } }
            public double Right { get { return X + Width; } }
            public double Top { get { return Y; } }
            public double Down { get { return Y + Height; } }
            public void Rotate()
            {
                double temp = Height;
                Height = Width;
                Width = temp;
            }
            public DiffContur Clone() { return new DiffContur() { X = X, Y = Y, Height = Height, Width = Width, Graphics = Graphics?.Clone(), Way = Way }; }
            public Graphics Graphics { get; set; }
            public string Way { get; set; }
        }

        public class DiffPage
        {
            public List<DiffLevel> Levels { get; set; } = new List<DiffLevel>();
            public int Count { get; set; }
            public double Price { get; set; }
            public string MaterialName { get; set; }
            public DiffContur MaterialSpace { get; set; }
            public string MaterialType { get; set; }
            public string ContursWay { get; set; }
            public Page Page { get; set; }
            public bool DefectKroy { get; set; } = false;
        }


        class BoxPart
        {
            public int Id { get; private set; }
            readonly BoxListRaskroy detail;
            public BoxPart(int Id, BoxListRaskroy boxDetail)
            {
                this.Id = Id;
                detail = boxDetail;
            }
            public string Way { get; set; }
            //короче, если надо повернуть по "приказу" Области поворота
            public bool? Rotate { get { if (_rotate == null) return null; else if (BoxRotate == null) return _rotate; else if (BoxRotate.Rotate) return !_rotate; else return _rotate; } set { _rotate = value; } }
            //надо повернуть по дефолту
            private bool? _rotate;
            public Graphics Graphics { get; set; }
            public int MaterialId { get; set; }
            public int RotateId { get; set; } = -1;
            public BoxMaterial Material { get { return detail.BoxMaterials[MaterialId]; } }
            public BoxRotate BoxRotate { get { return detail.BoxRotates.FirstOrDefault(r => r.Id == RotateId); } }
            public double Weight { get { return Graphics.Squeare * Material.Material.Density / 1000000 + Graphics.Squeare * (60/1000000); } }
        }
        class BoxMaterial
        {
            public int Id { get; private set; }
            readonly BoxListRaskroy detail;
            public BoxMaterial(int Id, BoxListRaskroy boxDetail)
            {
                this.Id = Id;
                detail = boxDetail;
            }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public dynamic Material { get; set; }
            public List<BoxPart> Parts { get { return detail.Parts.Where(p => p.MaterialId == Id).ToList(); } }
        }
        class BoxRotate
        {
            public int Id { get; private set; }
            public string Name { get; set; }
            readonly BoxListRaskroy detail;
            public BoxRotate(int Id, BoxListRaskroy boxDetail, string Name)
            {
                this.Id = Id;
                this.Name = Name;
                detail = boxDetail;
            }
            public bool Rotate { get; set; } = false;
            public List<BoxPart> Parts { get { return detail.Parts.Where(p => p.RotateId == Id).ToList(); } }
        }
    }
}
