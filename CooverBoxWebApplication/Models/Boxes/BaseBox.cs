using PDFCore;
using PDFCore.Graphic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data;
using PdfSharpCore.Pdf;
using OfficeOpenXml;
using System.Drawing;
using Point = PDFCore.Point;
using OfficeOpenXml.Style;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CooverBoxWebApplication.Infrastructure;
using CooverBoxWebApplication.Models.Boxes.Helpers;
using Graphics = PDFCore.Graphic.Graphics;
using Image = PDFCore.Image;
using CooverBoxWebApplication.InputValues.InputValuesInfo;

namespace CooverBoxWebApplication.Models.Boxes
{
    //базовый класс коробочек
    [ModelBinder(BinderType = typeof(InputValuesModelBinder))]
    abstract public class BaseBox
    {
        public string Id { get; set; } = null;
        public string UserName { get; set; } = null;
        //список для рабочих областей
        //каждая область имеет площаль и обрабатывается каким-то набором действий
        public List<WorkPlace> WorkPlaces { get; set; } = new List<WorkPlace>();
        //список всех коробочек
        static public BaseBox Box(string Type) => Type switch
        {
            ("TopBotton") => new TopBotton(),
            ("BookValve") => new BookValve(),
            ("OptimusPride") => new OptimusPride(),
            ("BookWithString") => new BookWithString(),
            ("Casket") => new Casket(),
            ("SliderMDF") => new SliderMDF(),
            ("Cub") => new Cub(),
            ("Slider") => new Slider(),
            ("Bumblebee") => new Bumblebee(),
            ("CraftTopBottonOptimus") => new CraftTopBottonOptimus(),
            ("BookValveMDF") => new BookValveMDF(),
            ("BookWithStringMDF") => new BookWithStringMDF(),
            ("CircleTopBotton") => new CircleTopBotton(),
            ("Mercedes") => new Mercedes(),
            ("CraftWithEars") => new CraftWithEars(),
            ("GofraTopBottonOptimus") => new GofraTopBottonOptimus(),
            ("GofraWithEars") => new GofraWithEars(),
            ("CatHouse") => new CatHouse(),
            _ => null,
            
        };

        public abstract string TypeBoxe { get; }
        public abstract string Name { get; set; }
        public abstract double X { get; set; }
        public abstract double Y { get; set; }
        public abstract double H { get; set; }
        public abstract int N { get; set; }

        protected enum ResultFincPart
        {
            Graphics,
            Group
        }


        public enum EnumAKnifPlotter
        {
            Автоматически, Нож, Плоттер
        }
        static public AreaInputValue MiddleInputValues { get; } = new AreaInputValue()
        {
            Items = new List<InputValues.Base.BaseInputValue>() {
                new AreaInputValue(){Card=true,Header="Ложемент",Items=new List<InputValues.Base.BaseInputValue>{
                    new ListIsolonInputValue(typeof(BaseBox).GetProperty("Isolon"),"Ложемент изолон/поролон") {AddOption="Не выбран",Count=4 },
                    new PaperInputValue(typeof(BaseBox).GetProperty("IsolonPaper"),"Изолон/поролон, покрытие") {AddOption="Не выбран",UserInput=true,TypeUsing=PaperTypeUsing.CoverIsolon },
                    new ClothInputValue(typeof(BaseBox).GetProperty("Drape"), "Ложемент с драпировкой ткани"){AddOption="Не выбран" },
                    new PaperInputValue(typeof(BaseBox).GetProperty("DesignerCarton"),"Ложемент диз картон") {AddOption="Не выбран",TypeUsing=PaperTypeUsing.DisCarton },
                    new NumberInputValue(typeof(BaseBox).GetProperty("DesignerCartonH"),"Высота ложемента из диз картона") {Required=false,Max=400 },
                    new EnumDropListInputValue<EnumAKnifPlotter>(typeof(BaseBox).GetProperty("LogementMachineType"),"Ложемент Machine-Type"){AlwaysDefault=true,Roll="admin",DisplayNullVisible = true },
                    }
                },
                new AreaInputValue(){Card=true,Header="Наполнители",Items=new List<InputValues.Base.BaseInputValue>{
                    new TissuePaperInputValue(typeof(BaseBox).GetProperty("TissuePaper"),"Бумага тишью"){ AddOption="Не выбран" },
                    new ArtHayInputValue(typeof(BaseBox).GetProperty("ArtHay"),"Арт-сено"){ AddOption="Не выбран" },
                    new FringePaperInputValue(typeof(BaseBox).GetProperty("FringePaper"),"Бумажная бахрома"){ AddOption="Не выбран" },
                    }
                }
            }
        };


        public int SilkWork { get; set; } = 0;
        public int ClicheWork { get; set; } = 0;


        public string DisplayLabel1 { get; } = "Ложемент";

        public List<Isolon> Isolon { get; set; }
        
        public DesignPaper IsolonPaper { get; set; }

        //если покрытие из полноцвет понтон то добавка из Картон GC2 magistr
        public DesignPaper IsolonPaperMagicPonton { get; set; }

        public Cloth Drape { get; set; }

        public DesignPaper DesignerCarton { get; set; }

        public double DesignerCartonH { get; set; } = 0;

        public EnumAKnifPlotter LogementMachineType { get; set; }

        public TissuePaper TissuePaper { get; set; }
        public ArtHay ArtHay { get; set; }

        public FringePaper FringePaper { get; set; }
                
        //Дата отгрузки
        public abstract DateTime DateOut { get; set; }

        //Дата отгрузки
        public abstract string BitrixLink { get; set; }

        //Крышка заворот, мм
        public abstract double Defect_percent { get; set; }

        //Отступ при раскрое, мм
        public abstract double Otstup { get; set; }

        public abstract string FullName { get; }

        //почтовый алгоритм
        public PostLiker2 PostLiker2 { get; private set; }
        public abstract double[] OuterSize { get; }

        public double Weight { get; set; } = 0;

        //валидация
        public List<ValidationResult> BaseValidate(ValidationContext validationContext, DBAppContext _dbcontext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            //покрытие изолона без изолона не возможно
            if (Isolon == null || Isolon.Count == 0) IsolonPaper = null;
            if (Isolon != null && Isolon.Any(i => i.Type != Isolon[0].Type)) errors.Add(new ValidationResult("Ложемент должен быть однотипный(например только изолон)"));
            if (H < Isolon?.Sum(iu => iu.Thickness)) errors.Add(new ValidationResult("Суммарная высота изолона/поролона больше высоты коробки"));
            //если покрытие полноцветной печатью, то печать идет на обычной бумаги которая кашируеться на крафтовый картон
            if(IsolonPaper != null && IsolonPaper.Name.Contains("Ponton"))
            {
                IsolonPaperMagicPonton = _dbcontext.DesignPapers.FirstOrDefault(p=>p.Id == 7 && p.Name.Contains("magistr"));
                IsolonPaper = _dbcontext.DesignPapers.FirstOrDefault(p => p.Name == IsolonPaper.Name && p.Density == 150);
            }
            //ложемент из диз картона
            if (DesignerCarton == null) DesignerCartonH = 0;
            if (DesignerCartonH == 0) DesignerCarton = null;
            if (H < DesignerCartonH) errors.Add(new ValidationResult("Высота ложемента больше высоты коробки"));

            if(LogementMachineType == EnumAKnifPlotter.Автоматически && (Isolon != null && Isolon.Count > 0) || (DesignerCarton != null && DesignerCartonH > 0))
            {
                if (N > 100)
                    LogementMachineType = EnumAKnifPlotter.Нож;
                else
                    LogementMachineType = EnumAKnifPlotter.Плоттер;
            }

            return errors;
        }


        //добавка по стоимости
        public abstract void AddTableCostViewChild();

        //хранит список стоимостей
        public List<DataTableLiker> TableCostView { get; private set; } = new List<DataTableLiker>();


        public List<InputValues.Base.BaseInputValue> GetInputValueInfo(List<InputValues.Base.BaseInputValue> inputValues = null)
        {
            if (inputValues == null)
            {
                Type type = this.GetType();
                inputValues = (List<InputValues.Base.BaseInputValue>)type.GetProperty("InputValues").GetValue(null);
            }
            List<InputValues.Base.BaseInputValue> result = new List<InputValues.Base.BaseInputValue>();
            foreach(var info in inputValues)
            {
                if(info.GetType() != typeof(AreaInputValue))
                {
                    result.Add(info);
                }
                else
                {
                    result.AddRange(GetInputValueInfo(((AreaInputValue)info).Items));
                }
            }
            return result;
        }

        //раскрой
        protected abstract BoxListRaskroy ListRaskroy { get; set; }
        //работы
        protected abstract WorkCostFormat WorkCostFormat { get; set; }
        //куски основного чертежа
        protected abstract List<ViewPDFGroup> ViewGroupsPDF { get; set; }
        //основная функция, добаление контуров, работ и тд 
        protected abstract void Start();
        //функция которая создает раскрой для дизайнеров
        public abstract void CreateDisignPDF(string PathConst,string PathFile);

        //добать контура до раскроя и стоимости работы
        //контур может быть любой обьект типа Graphics
        //материал - имя материала уже хранящегося в ListRaskroy
        //назначение контура - его имя
        //имя области работ хранящееся в WorkCostFormat
        //развернуть ли контур по умолчанию
        //имя связки разворотов (работа с волокном картона бумаги)
        protected void AddPart(Graphics graphic, string MaterialName, string Way, string WorkName, bool? Rotate = null, string RotateName = null)
        {
            ListRaskroy.AddPart(graphic, MaterialName, Way, Rotate, RotateName);
            WorkCostFormat.AddWorkWay(WorkName, Way, graphic.Squeare);
        }

        public void Create(string Path = null)
        {
            
            TableCostView = new List<DataTableLiker>();
            double Defect_percent = 1 + this.Defect_percent / 100;
            Start();
            //добавляем покрытие изолона на раскрой
            if (Isolon != null && Isolon.Count > 0 && IsolonPaper != null)
            {
                ListRaskroy.AddMaterial("IsolonPaper", IsolonPaper);
                ListRaskroy.AddPart(Contur.Rectangle(0, 0, X + 30, Y + 30), "IsolonPaper", "ложемент покрытие");
            }
            //раскрой
            List<BoxListRaskroy.DiffPage> raskroy = ListRaskroy.Work();
            //переносим стоимость листов в таблицу
            foreach (var kroy in raskroy)
            {
                AddCostView(Name: kroy.MaterialName, Type: kroy.MaterialType, Coment: $"{kroy.MaterialSpace.Width}x{kroy.MaterialSpace.Height}", Way: kroy.ContursWay, ComentLine: ((kroy.DefectKroy) ? "не удалось разложить" : ""), Count: kroy.Count, ValueType: "листов", Price: kroy.Price);
            }
            //добавляем если надо картон для покрытия пантонон
            if(IsolonPaperMagicPonton != null && raskroy.Any(r=>r.ContursWay.Contains("ложемент покрытие")))
            {
                Contur contur = Contur.Rectangle(0, 0, X + 30, Y + 30);
                double numberX1 = Math.Truncate(IsolonPaperMagicPonton.X / contur.Height) * Math.Truncate(IsolonPaperMagicPonton.Y / (contur.Width));
                double numberY1 = Math.Truncate(IsolonPaperMagicPonton.X / (contur.Width)) * Math.Truncate(IsolonPaperMagicPonton.Y / contur.Height);
                double numbers1 = Math.Max(numberX1, numberY1);
                double cn = Math.Ceiling(Math.Ceiling((double)(N) / numbers1) * Defect_percent);
                AddCostView(Name: IsolonPaperMagicPonton.Name, Type: "картон", Way: "ложемент покрытие", Count: (int)cn, ValueType: "листов", Price: cn * IsolonPaperMagicPonton.Price);
            }
            //плюсуем вес
            Weight = ListRaskroy.Weight;
            //считаем изолон
            if (Isolon != null && Isolon.Count > 0)
            {
                bool isolonpocrytie = IsolonPaper != null;
                Contur contur = Contur.Rectangle(0, 0, X + 30, Y + 30);
                foreach (Isolon var in Isolon)
                {
                    IsolonUsing(var, contur, N * Defect_percent, out double cn, out double weight);
                    Weight += weight;
                    double CostCartonPapertemp = cn * var.Price;
                    AddCostView($"{var.Name} {var.Color} {var.Thickness}", "ложемент", Count: cn, ValueType: (var.Roll) ? "метр погонный" : "лист", Price: CostCartonPapertemp);
                    if(isolonpocrytie) {
                        WorkCostFormat.AddWorkPlace("Ложемент c покрытие", $"{var.Name} {var.Color} {var.Thickness}", $"покрытие {IsolonPaper}", contur.Squeare, isolonplus: 1);
                        isolonpocrytie = false;
                    }
                    else { 
                        WorkCostFormat.AddWorkPlace("Ложемент", $"{var.Name} {var.Color} {var.Thickness}", "", contur.Squeare, isolon: 1);
                    }
                }
                if (Isolon.Any(i => i.Type == "FoamRubber") && IsolonPaper != null) AddCostView("Скотч", Count: contur.Squeare * (N * Defect_percent) / 1000000, ValueType: "м2", Way: "для склейки паралона", Price: contur.Squeare * (N * Defect_percent) * GetParameter("Base", "TapeRed") / 1000000);
                AddCostView("Работы", Way: "Высечка ложемент", Count: Isolon.Count, Price: Isolon.Count * (GetParameter("Base", "CuttingWorkPLog") + (N * Defect_percent) * (((N * Defect_percent) < 1000) ? GetParameter("Base", "CuttingWork_b1000") : GetParameter("Base", "CuttingWork_a1000"))));
            }
            {
                //работа шелк и тиснение
                
                int SilkWork = GetInputValueInfo().Where(v => v.ValueType == typeof(PaperWithPrint) && v.GetValue(this) != null ).Sum(v => v.GetValue(this).SilkWork);
                int ClicheWork = GetInputValueInfo().Where(v => v.ValueType == typeof(PaperWithPrint) && v.GetValue(this) != null ).Sum(v => v.GetValue(this).ClicheWork);
                //if (SilkWork > 0)
                //{
                //    AddCostView("Работы", Way: "Шелк", Count: SilkWork, Price: SilkWork * (GetParameter("Base", "SilkWorkP") + (N * Defect_percent) * (((N * Defect_percent) < 50) ? 0 : GetParameter("Base", "SilkWork"))));
                //}
                //стоимость клише на тиснение
                foreach(var value in GetInputValueInfo().Where(v=> v.ValueType == typeof(PaperWithPrint) && v.GetValue(this)?.ClicheX > 0 && v.GetValue(this)?.ClicheY > 0))
                {
                    AddCostView("Клише", Way: $"{value.GetValue(this).ClicheX}x{value.GetValue(this).ClicheY}", Count: 1, Price: (50 + (value.GetValue(this).ClicheX / 10 + 2) * (value.GetValue(this).ClicheY / 10 + 2) * GetParameter("Base", "ClicheCost")));
                }

                //if (ClicheWork > 0)
                //{
                //    AddCostView("Работы", Way: "Тиснение", Count: ClicheWork, Price: ClicheWork * (GetParameter("Base", "ClicheWorkP") + (N * Defect_percent) * (((N * Defect_percent) < 50) ? 0 : GetParameter("Base", "ClicheWork"))));
                //}
            }
            if (ArtHay != null)
            {
                Weight += 20;
                AddCostView(Way: "Наполнитель", Type: "Арт-сено", Name: $"{ArtHay.Name}", Count: Math.Round(0.020 * (N * Defect_percent), 3), ValueType: "кг", Price: Math.Round((0.020 * (N * Defect_percent)) * ArtHay.Price, 2));
            }
            if (TissuePaper != null)
            {
                Weight += 20;
                AddCostView(Type: "Бумага тишью", Way: "Наполнитель", Name: $"{TissuePaper.Name} {TissuePaper.Color}", Count: Math.Round(N * Defect_percent, 2), ValueType: "шт", Price: Math.Round(((N * Defect_percent)) * TissuePaper.Price, 2));
            }
            //ложемент драпировкой
            if (Drape != null)
            {
                Contur contur = Contur.Rectangle(0, 0, 3 * (X + 10), 3 * (Y + 10));
                if (Drape.Roll)
                {
                    double number1 = Math.Truncate(Drape.X / contur.Width);
                    double number2 = Math.Truncate(Drape.X / contur.Height);
                    double cn;
                    double temp;
                    if ((Drape.X - number1 * contur.Height) < (Drape.X - number2 * contur.Width))
                    {
                        temp = Math.Ceiling(N * Defect_percent / number1);
                        cn = (contur.Height) * temp;
                    }
                    else
                    {
                        temp = Math.Ceiling(N * Defect_percent / number2);
                        cn = contur.Width * temp;
                    }
                    cn /= 1000;
                    double CostCartonPapertemp = cn * Drape.Price;
                    AddCostView($"{Drape.Name}", "ткань", Way: "ложемент с драпировкой ткани", Count: cn, ValueType: "метр погонный", Price: CostCartonPapertemp);
                }
                else
                {
                    double numberX1 = Math.Truncate(Drape.X / contur.Height) * Math.Truncate(Drape.Y / (contur.Width));
                    double numberY1 = Math.Truncate(Drape.X / (contur.Width)) * Math.Truncate(Drape.Y / contur.Height);
                    double numbers1 = Math.Max(numberX1, numberY1);
                    double cn = Math.Ceiling((double)(N) / numbers1);
                    cn += Math.Ceiling(cn * Defect_percent / 100);
                    double CostCartonPapertemp = cn * Drape.Price;
                    AddCostView($"{Drape.Name}", "ткань", Way: "ложемент с драпировкой ткани", Count: cn, ValueType: "лист", Price: CostCartonPapertemp);
                }
                Weight += contur.Squeare * Drape.Density / 1000000;
                Weight += (X + 10) * (Y + 10) * 650 / 1000000;
                WorkCostFormat.AddWorkPlace("Ложемент с драпировкой ткани", $"{Drape.Name}", "", contur.Squeare, drap: 1);
            }
            if (FringePaper != null)
            {
                Weight += 20;
                double mass = 0.020 * (N * Defect_percent);
                double masslist = (FringePaper.X / 1000) * (FringePaper.Y / 1000) * FringePaper.Density;
                double lists = Math.Ceiling((mass * 1000) / masslist);
                AddCostView(Type: "Бумажная бахрома", Way: "Наполнитель", Name: $"{FringePaper.Name} {FringePaper.Color}", Count: Math.Round(mass, 3), ValueType: "кг", Price: Math.Round(lists * FringePaper.Price, 2));
            }
            if (DesignerCarton != null && DesignerCartonH > 0)
            {
                double w = X + 2 * DesignerCartonH + 20;
                double h = Y + 2 * DesignerCartonH + 20;
                double number_X = Math.Truncate(DesignerCarton.X / (w)) * Math.Truncate(DesignerCarton.Y / (h));
                double number_Y = Math.Truncate(DesignerCarton.X / (h)) * Math.Truncate(DesignerCarton.Y / (w));
                double lists = Math.Ceiling(Math.Ceiling(N / Math.Max(number_X, number_Y)) * Defect_percent);
                double Costtemp = lists * DesignerCarton.Price;
                AddCostView(Way: "Наполнитель", Type: "Ложемент дис картон", Name: $"{DesignerCarton.Name} {DesignerCarton.Color} {DesignerCarton.Density} г/м2", Count: lists, ValueType: "листов", Price: Math.Round(Costtemp, 2));
                Weight += w * h * DesignerCarton.Density / 1000000;
                WorkCostFormat.AddWorkPlace($"Ложемент дис картон", $"{DesignerCarton}", $"H{DesignerCartonH}", w*h, discarton: 1);
            }
            //AddCostView("Работы", Way: "Упаковка", Count: N, Price: (GetParameter($"{this.GetType().Name}", "PackageWork") + ((Isolon != null) ? GetParameter($"{this.GetType().Name}", "IsolonWork") : 0)) * (N * Defect_percent));
            //AddCostView("Работы", Way: "Оклейка", Count: N, Price: GetParameter($"{this.GetType().Name}", "FaiWork") * (N * Defect_percent));
            //AddCostView("Админы", Count: N, Price: GetParameter("Base", "Admin") * N);
            //добовляем стоимости которые индивидуальны для каждой коробочки
            AddTableCostViewChild();
            //считаем упаковку
            PostLiker2 = new PostLiker2(OuterSize[0], OuterSize[1], OuterSize[2], Weight, N);
            PostLiker2.Work(FullName,Path);
            //список работ и их стоимость
            WorkPlaces = WorkCostFormat.ToSheeldByName();
            {
                if(WorkPlaces.Any(w => w.Cuting > 0))
                {
                    AddCostView("Работы", Way: "Высечка", Count: WorkPlaces.Sum(w=>w.Cuting), Price: WorkPlaces.Sum(w => w.Cuting) * (GetParameter("Work", "CuttingP") + ((N > 50) ? N * GetParameter("Work", "Cutting") : 0)));
                }
                if (WorkPlaces.Any(w => w.Silk > 0))
                {
                    AddCostView("Работы", Way: "Шелк", Count: WorkPlaces.Sum(w => w.Silk), Price: WorkPlaces.Sum(w => w.Silk) * (GetParameter("Work", "SilkP") + ((N > 50) ? N * GetParameter("Work", "Silk") :0)));
                }
                if (WorkPlaces.Any(w => w.Cliche > 0))
                {
                    AddCostView("Работы", Way: "Тиснение", Count: WorkPlaces.Sum(w => w.Cliche), Price: WorkPlaces.Sum(w => w.Cliche) * (GetParameter("Work", "ClicheP") + ((N > 50) ? N * GetParameter("Work", "Cliche") : 0)));
                }
                if (WorkPlaces.Any(w => w.Fairy > 0))
                {
                    AddCostView("Работы", Way: "Оклейка", Count: 1, Price: N * GetParameter($"{this.GetType().Name}", "Fairy"));
                }
                if (WorkPlaces.Any(w => w.Saw > 0))
                {
                    AddCostView("Работы", Way: "Распиловка", Count: 1, Price: N * GetParameter($"Work", "Saw"));
                }
                if (!WorkPlaces.Any(w => w.Isolon > 0 || w.IsolonPlus > 0))
                {
                    AddCostView("Работы", Way: "Упаковка", Count: 1, Price: N * GetParameter("Work", "Package"));
                }
                else
                {
                    AddCostView("Работы", Way: "Упаковка", Count: 1, Price: N * GetParameter("Work", "IsolonPackage"));
                }
                AddCostView("Админы", Count: 1, Price: N * GetParameter($"{this.GetType().Name}", "Admin"));
            }
            //еслиуказан путь для папки, создаем файлы
            if (string.IsNullOrEmpty(Path)) return;
            {
                
                {
                    TextFrame shapka = new TextFrame(width: 190, strokeWidth: 18);
                    shapka.Content = $"Стандартный срок производства\nТираж: {N} шт";
                    shapka.MinimizeHeight();
                    shapka.SetLocation(10, 10);

                    TextFrame telo = new TextFrame(width: 190, strokeWidth: 14);
                    //telo.Content = $"Название проекта: {FullName}";
                    foreach (var cost in TableCostView.Where(t => !("Работы Админы Клише Нож".Contains(t.Name))))
                    {
                        telo.Content += $"\n{cost.Name} {(string.IsNullOrEmpty(cost.Coment) ? "" : cost.Coment)}: {cost.dataLines?.Sum(d => d.Count)} {cost.dataLines?.First().ValueType}";
                    }
                    telo.MinimizeHeight();
                    telo.SetLocation(10, shapka.Down + 5);

                    TextFrame data = new TextFrame(width: 190, strokeWidth: 18);
                    data.Content = $"Дата отгрузки: {DateOut:dd/MM/yyyy}";
                    data.MinimizeHeight();
                    data.SetLocation(10, telo.Down + 5);

                    Page page = new Page();
                    page.Add(shapka);
                    page.Add(telo);
                    page.Add(data);
                    page.AddQRCode((string.IsNullOrEmpty(BitrixLink) ? FullName : BitrixLink),0.2);
                    PdfDocument pdfDocument1 = new PdfDocument();
                    page.ToPDFSharp(pdfDocument1);
                    pdfDocument1.Save($"{Path}\\Без ТЗ ХЗ {FullName}.pdf");
                    pdfDocument1.Close();
                }
                {
                    Page page = new Page();
                    {
                        Group group = new Group();
                        Image image = Image.CreateQRCode((string.IsNullOrEmpty(BitrixLink) ? FullName : BitrixLink));
                        image.Scale = 120 / image.Width;
                        group.Add(image);
                        ViewGroupsPDF.Add(new ViewPDFGroup(-1,group));
                    }
                    if ((Isolon != null && Isolon.Count > 0) || (DesignerCarton != null && DesignerCartonH > 0))
                    {
                        Group group = new Group();
                        TextFrame shtamp = new TextFrame(width: 350, strokeWidth: 40);
                        shtamp.Content = $"Ложемент {LogementMachineType}";
                        shtamp.SetLocation(10, 10);
                        group.Add(shtamp);
                        group.SetLocation(10, 10);
                        ViewGroupsPDF.Add(new ViewPDFGroup(0, group));
                    }
                    int i = 1;
                    int prioritybefore = -1;
                    foreach (var view in ViewGroupsPDF.OrderBy(v => v.Prioriry))
                    {
                        if (page.Count == 0)
                        {
                            view.Group.SetLocation(10, 10);
                            page.Add(view.Group);
                            i = 1;
                        }
                        else if (i++ % 4 != 0 && prioritybefore == view.Prioriry)
                        {
                            view.Group.SetLocation(page.GetObject(page.Count - 1).Right + 10, page.GetObject(page.Count - 1).Top);
                            page.Add(view.Group);
                        }
                        else
                        {
                            view.Group.SetLocation(10, page.Down + ((prioritybefore == view.Prioriry) ? 20 : 10));
                            page.Add(view.Group);
                            i = 1;
                        }
                        prioritybefore = view.Prioriry;
                    }
                    PdfDocument pdfDocument1 = new PdfDocument();
                    page.ToPDFSharp(pdfDocument1);
                    pdfDocument1.Save($"{Path}\\Чертеж {FullName}.pdf");
                    pdfDocument1.Close();
                }
                {
                    PdfDocument pdfDocument1 = new PdfDocument();
                    foreach (var kroy in raskroy)
                    {
                        kroy.Page.AddQRCode((string.IsNullOrEmpty(BitrixLink) ? FullName : BitrixLink), 0.15);
                        kroy.Page.ToPDFSharp(pdfDocument1);
                    }
                    pdfDocument1.Save($"{Path}\\Раскрой {FullName}.pdf");
                    pdfDocument1.Close();
                }
            }
            using ExcelPackage excelPackage = new ExcelPackage();
            {
                //create a WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Column(1).Width = 20;
                worksheet.Column(1).Style.WrapText = true;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 45;
                worksheet.Column(5).Width = 20;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 50;
                worksheet.Cells.Style.Font.Size = 14;
                worksheet.Cells.Style.Font.Name = "Times New Roman";
                int intejerlines = 1;
                worksheet.Cells[$"A{intejerlines}:C{intejerlines}"].Merge = true;
                worksheet.Cells[$"A{intejerlines}:E{intejerlines}"].Style.Font.Bold = true;
                worksheet.Cells[$"A{intejerlines}:E{intejerlines}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"A{intejerlines}:E{intejerlines}"].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                worksheet.Cells[$"A{intejerlines}"].Value = $"{FullName}";
                worksheet.Cells[$"D{intejerlines}"].Value = $"{TypeBoxe}";
                worksheet.Cells[$"E{intejerlines}"].Value = $"Тираж {N}";
                intejerlines++;
                worksheet.Cells[$"A{intejerlines}:H{intejerlines}"].Style.Font.Bold = true;
                worksheet.Cells[$"A{intejerlines}:H{intejerlines}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"A{intejerlines}:H{intejerlines}"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                worksheet.Cells[$"A{intejerlines}:B{intejerlines}"].Merge = true;
                worksheet.Cells[$"A{intejerlines}"].Value = "Название";
                worksheet.Cells[$"C{intejerlines}"].Value = "Тип";
                worksheet.Cells[$"D{intejerlines}"].Value = "Направление";
                worksheet.Cells[$"E{intejerlines}"].Value = "Количество";
                worksheet.Cells[$"F{intejerlines}"].Value = "Ед. измерения";
                worksheet.Cells[$"G{intejerlines}"].Value = "Цена";
                worksheet.Cells[$"H{intejerlines}"].Value = "Коментарий";
                intejerlines++;
                List<int> tempint = new List<int>();
                foreach (DataTableLiker var in TableCostView.Where(t => !"Клише Нож".Contains(t.Name)))
                {
                    worksheet.Cells[$"A{intejerlines}:B{intejerlines + 1}"].Merge = true;
                    worksheet.Cells[$"A{intejerlines}"].Value = var.Name;
                    worksheet.Cells[$"A{intejerlines}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{intejerlines}:C{intejerlines + 1}"].Merge = true;
                    worksheet.Cells[$"C{intejerlines}"].Value = var.Type;
                    worksheet.Cells[$"C{intejerlines}"].Style.Font.Bold = true;
                    intejerlines++;
                    int i = intejerlines;
                    foreach (DataLineLiker line in var.dataLines)
                    {
                        worksheet.Cells[$"D{intejerlines}"].Value = line.Way;
                        worksheet.Cells[$"E{intejerlines}"].Value = line.Count;
                        worksheet.Cells[$"F{intejerlines}"].Value = line.ValueType;
                        worksheet.Cells[$"G{intejerlines}"].Value = line.Price;
                        worksheet.Cells[$"H{intejerlines}"].Value = line.Coment;
                        intejerlines++;
                    }
                    worksheet.Cells[$"D{intejerlines}"].Value = "СУММА";
                    worksheet.Cells[$"E{intejerlines}"].Formula = $" = SUM(E{i}: E{intejerlines - 1})";
                    worksheet.Cells[$"G{intejerlines}"].Formula = $" = SUM(G{i}: G{intejerlines - 1})";
                    tempint.Add(intejerlines);
                    worksheet.Cells[$"D{intejerlines}:G{intejerlines}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[$"D{intejerlines}:G{intejerlines}"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    intejerlines++;
                }
                worksheet.Cells[$"A{intejerlines}:B{intejerlines}"].Merge = true;
                worksheet.Cells[$"A{intejerlines}"].Value = "СУММА";
                string tempG = "";
                foreach (int var in tempint)
                {
                    if (string.IsNullOrEmpty(tempG))
                    {
                        tempG += $"G{var}";
                    }
                    else
                    {
                        tempG += $", G{var}";
                    }
                }
                worksheet.Cells[$"G{intejerlines}"].Formula = $" = SUM({tempG})";
                worksheet.Cells[$"A{intejerlines}:G{intejerlines}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"A{intejerlines}:G{intejerlines}"].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                intejerlines += 2;
                foreach (DataTableLiker var in TableCostView.Where(t => "Клише Нож".Contains(t.Name)))
                {
                    worksheet.Cells[$"A{intejerlines}:B{intejerlines + 1}"].Merge = true;
                    worksheet.Cells[$"A{intejerlines}"].Value = var.Name;
                    worksheet.Cells[$"A{intejerlines}"].Style.Font.Bold = true;
                    worksheet.Cells[$"C{intejerlines}:C{intejerlines + 1}"].Merge = true;
                    worksheet.Cells[$"C{intejerlines}"].Value = var.Type;
                    worksheet.Cells[$"C{intejerlines}"].Style.Font.Bold = true;
                    intejerlines++;
                    int i = intejerlines;
                    foreach (DataLineLiker line in var.dataLines)
                    {
                        worksheet.Cells[$"D{intejerlines}"].Value = line.Way;
                        worksheet.Cells[$"E{intejerlines}"].Value = line.Count;
                        worksheet.Cells[$"F{intejerlines}"].Value = line.ValueType;
                        worksheet.Cells[$"G{intejerlines}"].Value = line.Price;
                        worksheet.Cells[$"H{intejerlines}"].Value = line.Coment;
                        intejerlines++;
                    }
                    intejerlines++;
                }
                intejerlines += 3;
                worksheet.Cells[$"A{intejerlines}"].Value = "ВЕС";
                worksheet.Cells[$"B{intejerlines}"].Value = $"{Math.Round(Weight * Defect_percent, 3)}";
                worksheet.Cells[$"C{intejerlines}"].Value = $"гр";
                intejerlines += 2;
                worksheet.Cells[$"A{intejerlines}"].Value = "Размеры";
                worksheet.Cells[$"B{intejerlines}"].Value = $"{Math.Round(OuterSize[0], 2)}x{Math.Round(OuterSize[1], 2)}x{Math.Round(OuterSize[2], 2)}";
                worksheet.Cells[$"C{intejerlines}"].Value = $"{Math.Round((OuterSize[0] / 1000) * (OuterSize[1] / 1000) * (OuterSize[2] / 1000), 10)} м3";
            }
            //{
            //    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Работы");

            //    int intejerlines = 1;
            //    int lineindex = 1;
            //    worksheet.Cells[intejerlines, lineindex++].Value = $"Название";
            //    worksheet.Cells[intejerlines, lineindex++].Value = $"Материал";
            //    worksheet.Cells[intejerlines, lineindex++].Value = $"Куда";
            //    worksheet.Cells[intejerlines, lineindex++].Value = $"Формат";
            //    foreach(var type in new WorkCostFormat.WorkPlace().works.Keys)
            //    {
            //        worksheet.Cells[intejerlines, lineindex++].Value = type;
            //    }
            //    intejerlines++;
            //    foreach (var line in WorkCostFormat.ToSheeldByName())
            //    {
            //        lineindex = 1;
            //        worksheet.Cells[intejerlines, lineindex++].Value = line.Name;
            //        worksheet.Cells[intejerlines, lineindex++].Value = line.Material;
            //        worksheet.Cells[intejerlines, lineindex++].Value = line.Way;
            //        worksheet.Cells[intejerlines, lineindex++].Value = line.Format;
            //        foreach (var type in line.works.Keys)
            //        {
            //            worksheet.Cells[intejerlines, lineindex++].Value = line.works[type];
            //        }
            //        intejerlines++;
            //    }
            //}
            excelPackage.SaveAs(new FileInfo($"{Path}\\{FullName}.xlsx"));
        }

        //получить параметры из файла Parameters
        public static double GetParameter(string NameSection,string Name)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("Parameters.json",true,true);
            string temp = builder.Build().GetSection(NameSection).GetSection(Name).Value;
            //если не удалось найти параметр ищим в разделе Default
            if (string.IsNullOrEmpty(temp))
                return double.Parse(builder.Build().GetSection("Default").GetSection(Name).Value.Replace(",", "."));
            else
                return double.Parse(builder.Build().GetSection(NameSection).GetSection(Name).Value.Replace(",", "."));
        }
        //добавить поле в таблицу стоимостей
        protected void AddCostView(string Name,string Type = "",string Coment = "", string Way = "",string ComentLine = "",double Count = 0,string ValueType = "",double Price = 0)
        {
            //если такой раздел уже есть то добавляем в него строки
            int index = TableCostView.FindIndex(t => t.Name == Name && (string.IsNullOrEmpty(Type) || t.Type == Type) && (string.IsNullOrEmpty(Type) || t.Coment == Coment));
            if(index >= 0)
            {
                TableCostView[index].dataLines.Add(new DataLineLiker() {Way = Way,Coment = ComentLine, Count = Math.Round(Count, 5), ValueType = ValueType,Price = Math.Round(Price,3) });
            }
            else
            {
                TableCostView.Add(new DataTableLiker() { Name = Name, Type = Type,Coment = Coment, dataLines = new List<DataLineLiker>() { new DataLineLiker() { Way = Way, Coment = ComentLine, Count = Math.Round(Count,5), ValueType = ValueType, Price = Math.Round(Price, 3) } } });
            }
        }
        //установить переменную через отражения
        public bool SetValue(string Name, dynamic Value)
        {
            try
            {
                this.GetType().GetProperty(Name)?.SetValue(this, Value);
                return true;
            }
            catch { }
            return false;
        }
        //раскрой изолона
        static public void IsolonUsing(Isolon isolon,Graphics contur,double N,out double cn,out double weight)
        {
            //если изолон поставляеться рулоном
            if (isolon.Roll)
            {
                double number1 = Math.Truncate(isolon.X / contur.Width);
                double number2 = Math.Truncate(isolon.X / contur.Height);
                double temp;
                if ((isolon.X - number1 * contur.Height) < (isolon.X - number2 * contur.Width))
                {
                    temp = Math.Ceiling(N  / number1);
                    cn = (contur.Height) * temp;
                }
                else
                {
                    temp = Math.Ceiling(N / number2);
                    cn = contur.Width * temp;
                }
                cn /= 1000;
            }
            else
            {
                double numberX1 = Math.Truncate(isolon.X / contur.Height) * Math.Truncate(isolon.Y / (contur.Width));
                double numberY1 = Math.Truncate(isolon.X / (contur.Width)) * Math.Truncate(isolon.Y / contur.Height);
                double numbers1 = Math.Max(numberX1, numberY1);
                cn = Math.Ceiling((double)(N) / numbers1);
            }
            //добавка веса
            if (isolon.Thickness <= 20 && isolon.Type == "Isolon")
                weight = contur.Squeare * 300 / 1000000;
            else if (isolon.Thickness > 20 && isolon.Type == "Isolon")
                weight = contur.Squeare * 500 / 1000000;
            else if (isolon.Thickness <= 20 && isolon.Type == "FoamRubber")
                weight = contur.Squeare * 32 / 1000000;
            else if (isolon.Thickness > 20 && isolon.Type == "FoamRubber")
                weight = contur.Squeare * 62 / 1000000;
            else if (isolon.Type == "Rubber")
                weight = contur.Squeare * 300 / 1000000;
            else weight = 0;
        }
    }
}
