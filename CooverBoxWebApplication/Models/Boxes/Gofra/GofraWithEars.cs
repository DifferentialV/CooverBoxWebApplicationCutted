using PDFCore;
using PDFCore.Graphic;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CooverBoxWebApplication.Models.Boxes.Helpers;
using CooverBoxWebApplication.Infrastructure;
using CooverBoxWebApplication.InputValues.Base;
using CooverBoxWebApplication.InputValues.InputValuesInfo;

namespace CooverBoxWebApplication.Models.Boxes
{
    public class GofraWithEars : BaseBox, IValidatableObject
    {
        static public List<BaseInputValue> InputValues { get; }
        static GofraWithEars()
        {
            Type thistype = typeof(GofraWithEars);
            System.Reflection.PropertyInfo Prop(string name)
            {
                return thistype.GetProperty(name);
            }
            InputValues = new List<BaseInputValue>()
            {
            new AreaInputValue(){Items = new List<BaseInputValue>{
                new TextInputValue(Prop("Name"),"Имя проекта"){LengthMin=5,RegularExpression=@"^[a-zA-Z0-9\s_\\.\-\(\):\u0400-\u04FF]+$" },
                new NumberInputValue(Prop("X"),"Длина, мм"){Min=1},
                new NumberInputValue(Prop("Y"),"Ширина, мм"){Min=1 },
                new NumberInputValue(Prop("H"),"Высота, мм"){Min=1 },
                new NumberInputValue(Prop("N"),"Тираж"){Min=1},
                new CartonInputValue(Prop("Carton"),"Гофрокартон") {TypeUsing = CartonTypeUsing.CorrugatedBoard },
                new PaperWithPrintInputValue(Prop("Paper"),"Кашировка") {TypeUsing = PaperTypeUsing.Sticker,UserInput=true,AddOption="Не выбран" },
            } },
            MiddleInputValues,
            new AreaInputValue(){Roll="admin",Items=new List<BaseInputValue>{
                new NumberInputValue(Prop("Defect_percent"),"Заложенный процент на брак"){Min=0,Max = 100},
                new NumberInputValue(Prop("Otstup"),"Отступ при раскрое, мм"){Min=0,Max = 100},
                new EnumDropListInputValue<EnumAKnifPlotter>(Prop("MachineType"),"Machine-Type"){AlwaysDefault=true },
                new DateTimeInputValue(Prop("DateOut"),"Дата отгрузки"){DateTimeType = "date" },
                new TextInputValue(Prop("BitrixLink"),"Ссылка"){RegularExpression=@"^([^\s]*)?(bitrix24)([^\s]*)$",Required=false },
            } }
            };
        }

        public override string TypeBoxe { get { return "Гофра с ушами"; } }
        public override string FullName { get { return $"{Name} {X}x{Y}x{H} - {N}"; } }

        public override string Name { get; set; } = null;
        public override double X { get; set; } = 150;
        public override double Y { get; set; } = 100;
        public override double H { get; set; } = 75;
        public override int N { get; set; } = 40;

        public CoverCarton Carton { get; set; }
        public PaperWithPrint Paper { get; set; }

        public override double Defect_percent { get; set; } = 10;
        public override double Otstup { get; set; } = 3;
        public EnumAKnifPlotter MachineType { get; set; }
        public override DateTime DateOut { get; set; } = DateTime.Now.AddWorkDays(21);
        public override string BitrixLink { get; set; }

        public override void AddTableCostViewChild()
        {
            if (Paper != null)
                AddCostView("Работы", Way: "Кашировка", Count: 1, Price: 4 * N);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DBAppContext _dbcontext = validationContext.GetService<DBAppContext>();
            List<ValidationResult> errors = base.BaseValidate(validationContext, _dbcontext);
            Graphics path = DrawCoverTypical1Contur(X, Y, H);
            if ((Carton.X / path.Width < 1 || Carton.Y / path.Height < 1) && (Carton.Y / path.Width < 1 || Carton.X / path.Height < 1))
                errors.Add(new ValidationResult("Мы пытались оно не влезло"));
            if (H < 25) errors.Add(new ValidationResult("Высота коробки дожна быть не меньше 25"));

            if (MachineType == EnumAKnifPlotter.Автоматически)
            {
                if (N > 100)
                    MachineType = EnumAKnifPlotter.Нож;
                else
                    MachineType = EnumAKnifPlotter.Плоттер;
            }

            return errors;
        }
        //наружные размеры
        public override double[] OuterSize
        {
            get
            {

            }
        }

        protected override BoxListRaskroy ListRaskroy { get; set; }
        protected override WorkCostFormat WorkCostFormat { get; set; }
        protected override List<ViewPDFGroup> ViewGroupsPDF { get; set; }
        protected override void Start()
        {
            double Defect_percent = 1 + this.Defect_percent / 100;
            ViewGroupsPDF = new List<ViewPDFGroup>();
            {
                WorkCostFormat = new WorkCostFormat((int)Math.Ceiling(N * Defect_percent));
                WorkCostFormat.AddWorkPlace("Гофра", $"{Carton}", cuting: MachineType == EnumAKnifPlotter.Нож ? 1 : 0, plotter: MachineType == EnumAKnifPlotter.Плоттер ? 1 : 0);
                if(Paper != null)
                    WorkCostFormat.AddWorkPlace("Бумага", $"{Paper.Paper}",  silk: Paper.SilkWork, cliche: Paper.ClicheWork, fairy: 1);
            }
            ListRaskroy = new BoxListRaskroy()
            {
                Otstup = Otstup,
                N = N,
                Defect_percent = Defect_percent,
                ProjectName = FullName,
                ProjectType = TypeBoxe
            };
            ListRaskroy.AddMaterial("Gofra", Carton, $"{Carton}");
            if(Paper != null)
                ListRaskroy.AddMaterial("Paper", Paper.Paper, $"{Paper.Paper} {Paper.Paper.Thickness}мм");

            /*------------*/
            {
                Graphics path = DrawCoverTypical1Contur(X, Y, H);
                AddPart(path, "Gofra", $"Дно", "Гофра");
                if (Paper != null)
                    AddPart(Contur.Rectangle(0, 0, path.Width, path.Height), "Paper", $"Дно кашировка", "Бумага");
            }


            ViewGroupsPDF.Add(new ViewPDFGroup(1, DrawCoverTypical1(X, Y, H, $"Крой {MachineType}")));

            {
                Group group = new Group();
                TextFrame shtamp = new TextFrame(width: 350, strokeWidth: 40);
                shtamp.Content = $"Название проекта: {FullName}";
                int SilkWork = WorkCostFormat.ToSheeldByName().Sum(q => q.Silk);
                int ClicheWork = WorkCostFormat.ToSheeldByName().Sum(q => q.Cliche);
                if (Paper?.Paper.FullColor is true || SilkWork > 0 || ClicheWork > 0)
                {
                    shtamp.Content += $"\nПЕЧАТЬ";
                    if (Paper?.Paper.FullColor is true)
                    {
                        shtamp.Content += $"\nПолноцвет";
                    }
                    if (SilkWork > 0)
                    {
                        shtamp.Content += $"\nШёлк: приладок {SilkWork}";
                    }
                    if (ClicheWork > 0)
                    {
                        shtamp.Content += $"\nТиснение: приладок {ClicheWork}";
                    }
                }
                shtamp.SetLocation(10, 10);
                group.Add(shtamp);
                group.SetLocation(10, 10);
                ViewGroupsPDF.Add(new ViewPDFGroup(0, group));
            }
        }


        static private GraphicGroup DrawCoverTypical1Contur(double x, double y, double h)
        {
            double angl15 = Math.Tan(15 * Math.PI / 180);
            double angl13 = 13 * Math.PI / 180;
            Contur contur = new Contur();
            Contur temp = new Contur();
            {
                contur.Add(0, 0); temp.Add(contur.EndPoint);
                contur.AddPoint(-1, -1);

                Contur clonetemp = temp.Clone();
                clone.MirrorY();
                clonetemp.MirrorY();
                clone.SetLocation(x - 2, 0, 0);
                clonetemp.SetLocation(x - 2, 0, 0);
                contur.Add(clone, 'r');
                temp.Add(clonetemp, 'r');

            }
            GraphicGroup group = new GraphicGroup();
            Point[] ps = temp.GetPoints();

            List<Contur> lines = new List<Contur>
            {
                Contur.Line(ps[0],ps[1]),

                Contur.Line(new Point(ps[11].X,ps[11].Y-2.58),ps[10]),
                Contur.Line(new Point(ps[6].X+0.71,ps[6].Y-2),new Point(ps[11].X-0.71,ps[11].Y-2)),
                Contur.Line(ps[8],ps[9]),
            };
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Color = XColors.DarkBlue;
                lines[i].Stroke = 0.5;
                group.Add(lines[i]);
            }


            group.Add(contur);
            return group;
        }
        static private Group DrawCoverTypical1(double x, double y, double h, string Label)
        {
            GraphicGroup contur = DrawCoverTypical1Contur(x, y, h);
            Group group = new Group();
            group.Add(contur);
            group.Add(Tools.AddLineDimension(contur));
            group.Add(Tools.AddLabel(group, Label));
            group.SetLocation(0, 0);
            return group;
        }

        public override void CreateDisignPDF(string PathConst, string PathFile)
        {
            Group group = CreateDisign(X, Y, H);
            Image image = null;
            try
            {
                image = new Image(PathConst);
            }
            catch { }
            Page page = new Page();
            if (image != null)
            {
                image.SetLocation(10, 10);
                group.SetLocation(image.Left + (image.Width - group.Width) / 2, image.Height + 20);
                page.Add(image);
                page.Add(group);
                page.SetLocation(10, 10);
            }
            else
            {
                group.SetLocation(10, 10);
                page.Add(group);
                page.SetLocation(10, 10);
            }
            PdfDocument document = new PdfDocument();
            document.Info.Title = TypeBoxe;
            page.ToPDFSharp(document);
            document.Save(PathFile);
        }
        private static Group CreateDisign(double x, double y, double h)
        {
            double StrokeSize = (x > 80 && y > 80 && h > 50) ? 50 : 30;
            Group group1 = new Group();
            {
                List<Contur> conturs = new List<Contur> {
                     Contur.Rectangle(0,0, x,h,0.5),
                     Contur.Rectangle(0,0, x,y,0.5),
                     Contur.Rectangle(0,0, x,h,0.5),
                     Contur.Rectangle(0,0, x,y,0.5),
                     Contur.Rectangle(0,0, x,h-1,0.5),
                };
                group1.Add(conturs[0]);
                for (int i = 1; i < conturs.Count; i++)
                {
                    conturs[i].SetCenter(conturs[i - 1].Center.X, conturs[i - 1].Down + conturs[i].Height / 2);
                    group1.Add(conturs[i]);
                }
                Contur rectleft = Contur.Rectangle(0, 0, h, y, 0.5);
                rectleft.SetCenter(conturs[1].Left - rectleft.Width / 2, conturs[1].Center.Y);
                Contur rectright = Contur.Rectangle(0, 0, h, y, 0.5);
                rectright.SetCenter(conturs[1].Right + rectright.Width / 2, conturs[1].Center.Y);
                group1.Add(rectleft);
                group1.Add(rectright);
                Contur redline = new Contur()
                {
                    Stroke = 0.5,
                    Color = PdfSharpCore.Drawing.XColors.Red
                };
                {
                    redline.Add(conturs[1].Left - 15, conturs[0].Top - 15);
                    redline.Add(conturs[1].Left - 15, conturs[1].Top - 15);
                    redline.Add(rectleft.Left - 15, conturs[1].Top - 15);
                    redline.Add(rectleft.Left - 15, conturs[1].Down + 15);
                    redline.Add(conturs[1].Left - 15, conturs[1].Down + 15);
                    redline.Add(conturs[4].Left - 15, conturs[4].Down + 15);
                    {
                        Contur clone = redline.Clone();
                        clone.MirrorY();
                        clone.SetLocation(conturs[1].Right + 15, conturs[0].Top - 15, 0);
                        redline.Add(clone, 'r');
                    }
                }
                group1.Add(redline);
                group1.Add(Tools.AddStringCenter(conturs[4], "Клапан", StrokeSize, "Comic Sans MS"));
                group1.Add(Tools.AddStringCenter(conturs[3], "Лицо", StrokeSize, "Comic Sans MS"));
                group1.Add(Tools.AddStringCenter(conturs[2], "Борт 3", StrokeSize, "Comic Sans MS"));
                group1.Add(Tools.AddStringCenter(conturs[1], "Дно", StrokeSize, "Comic Sans MS"));
                group1.Add(Tools.AddStringCenter(conturs[0], "Борт 4", StrokeSize, "Comic Sans MS"));
                group1.Add(Tools.AddStringCenter(rectleft, "Борт 1", StrokeSize, "Comic Sans MS"));
                group1.Add(Tools.AddStringCenter(rectright, "Борт 2", StrokeSize, "Comic Sans MS"));
                group1.Add(Tools.AddLabel(redline, "Наружные области", StrokeSize, "Comic Sans MS"));
            }
            group1.SetLocation(10, 20);
            Group group2 = new Group();
            {
                List<Contur> conturs = new List<Contur> {
                     Contur.Rectangle(0,0, x,h,0.5),
                     Contur.Rectangle(0,0, x,y,0.5),
                     Contur.Rectangle(0,0, x,h,0.5),
                     Contur.Rectangle(0,0, x,y,0.5),
                     Contur.Rectangle(0,0, x,h-1,0.5),
                };
                group2.Add(conturs[0]);
                for (int i = 1; i < conturs.Count; i++)
                {
                    conturs[i].SetCenter(conturs[i - 1].Center.X, conturs[i - 1].Down + conturs[i].Height / 2);
                    group2.Add(conturs[i]);
                }
                Contur rectleft = Contur.Rectangle(0, 0, h, y, 0.5);
                rectleft.SetCenter(conturs[1].Left - rectleft.Width / 2, conturs[1].Center.Y);
                Contur rectright = Contur.Rectangle(0, 0, h, y, 0.5);
                rectright.SetCenter(conturs[1].Right + rectright.Width / 2, conturs[1].Center.Y);
                group2.Add(rectleft);
                group2.Add(rectright);
                Contur redline = new Contur()
                {
                    Stroke = 0.5,
                    Color = PdfSharpCore.Drawing.XColors.Red
                };
                {
                    redline.Add(conturs[1].Left - 15, conturs[0].Top - 15);
                    redline.Add(conturs[1].Left - 15, conturs[1].Top - 15);
                    redline.Add(rectleft.Left - 15, conturs[1].Top - 15);
                    redline.Add(rectleft.Left - 15, conturs[1].Down + 15);
                    redline.Add(conturs[1].Left - 15, conturs[1].Down + 15);
                    redline.Add(conturs[4].Left - 15, conturs[4].Down + 15);
                    {
                        Contur clone = redline.Clone();
                        clone.MirrorY();
                        clone.SetLocation(conturs[1].Right + 15, conturs[0].Top - 15, 0);
                        redline.Add(clone, 'r');
                    }
                }
                group2.Add(redline);
                group2.Add(Tools.AddStringCenter(conturs[4], "Клапан", StrokeSize, "Comic Sans MS"));
                group2.Add(Tools.AddStringCenter(conturs[3], "Лицо", StrokeSize, "Comic Sans MS"));
                group2.Add(Tools.AddStringCenter(conturs[2], "Борт 3", StrokeSize, "Comic Sans MS"));
                group2.Add(Tools.AddStringCenter(conturs[1], "Дно", StrokeSize, "Comic Sans MS"));
                group2.Add(Tools.AddStringCenter(conturs[0], "Борт 4", StrokeSize, "Comic Sans MS"));
                group2.Add(Tools.AddStringCenter(rectleft, "Борт 1", StrokeSize, "Comic Sans MS"));
                group2.Add(Tools.AddStringCenter(rectright, "Борт 2", StrokeSize, "Comic Sans MS"));
                group2.Add(Tools.AddLabel(redline, "Внутренние области", StrokeSize, "Comic Sans MS"));
            }
            group2.SetLocation(group1.Right + 20, group1.Top);

            Group group = new Group();
            group.Add(group1);
            group.Add(group2);
            return group;
        }
    }
}
