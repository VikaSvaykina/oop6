using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace oop6
{
    public partial class Form1 : Form
    {
        public List<CFigure> FigureList = new List<CFigure>();
        public List<String> saveFigures = new List<String>();
        List<string> treeData = new List<string>();
        public string selectFigure = "Круг";
        public bool ctrl = false;
        public int rad = 20;
        public Color color = Color.Red;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectFigure == "Круг")
            {
                selectFigure = "Квадрат";
                button1.Text = "Квадрат";
            }
            else if (selectFigure == "Квадрат")
            {
                selectFigure = "Треугольник";
                button1.Text = "Треугольник";
            }
            else if (selectFigure == "Треугольник")
            {
                selectFigure = "Отрезок";
                button1.Text = "Отрезок";
            }
            else if (selectFigure == "Отрезок")
            {
                selectFigure = "Круг";
                button1.Text = "Круг";
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            
            if (!ctrl)
            {
                foreach (CFigure f in FigureList)
                {
                    f.SetHighlight(false);
                }
                CFigure figure = null;
                switch (selectFigure)
                {
                    case "Круг":
                        {
                            figure = new CCircle(e.X, e.Y, rad, color);
                            break;
                        }
                    case "Квадрат":
                        {
                            figure = new CSquare(e.X, e.Y, rad, color);
                            break;
                        }
                    case "Треугольник":
                        {
                            figure = new CTriangle(e.X, e.Y, rad, color);
                            break;
                        }
                    case "Отрезок":
                        {
                            figure = new CSection(e.X, e.Y, rad, color);
                            break;
                        }
                }
                FigureList.Add(figure);
                
            }
            else if (ctrl)
            {
                foreach (CFigure f in FigureList)
                {
                    if (f.CheckMouse(e))
                    {
                        f.SetHighlight(true);
                        break;
                    }

                }
            }
            Refresh();
            SyncLtoTree();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (CFigure f in FigureList)
            {
                f.DrawFigure(e.Graphics);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ctrl = checkBox1.Checked;
            foreach (CFigure f in FigureList)
            {
                f.SetCtrl(ctrl);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                checkBox1.Checked = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                for (int i = 0; i < FigureList.Count; i++)
                {
                    if (FigureList[i].highlight)
                    {
                        FigureList.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (e.KeyCode == Keys.Oemplus)
            {
                foreach (CFigure f in FigureList)
                {
                    if (f.highlight)
                    {
                        f.ChangeSize("plus", this);
                    }
                }
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                foreach (CFigure f in FigureList)
                {
                    if (f.highlight)
                    {
                        f.ChangeSize("minus", this);
                    }
                }
            }
            Refresh();
            SyncLtoTree();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            checkBox1.Checked = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            rad = int.Parse(numericUpDown1.Value.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (color == Color.Red)
            {
                color = Color.Blue;
            }
            else if (color == Color.Blue)
            {
                color = Color.Green;
            }
            else if (color == Color.Green)
            {
                color = Color.Purple;
            }
            else if (color == Color.Purple)
            {
                color = Color.Red;
            }
            button2.BackColor = color;
            foreach (CFigure f in FigureList)
            {
                if (f.highlight)
                    f.SetColor(color);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CGroup group = new CGroup();
            for (int i = 0; i < FigureList.Count; i++)
            { 
                if (FigureList[i].highlight)
                {
                    group.AddFigure(FigureList[i]);
                    FigureList.RemoveAt(i);
                    i--;
                }
            }
            FigureList.Add(group);
            Refresh();
            SyncLtoTree();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
            {
                foreach (CFigure f in FigureList)
                {
                    if (f.highlight && f.CanMove("up", this))
                    {
                        f.Move("up");
                    }
                }
            }
            else if (e.KeyChar == 's')
            {
                foreach (CFigure f in FigureList)
                {
                    if (f.highlight && f.CanMove("down", this))
                    {
                        f.Move("down");
                    }
                }
            }
            else if (e.KeyChar == 'd')
            {
                foreach (CFigure f in FigureList)
                {
                    if (f.highlight && f.CanMove("right", this))
                    {
                        f.Move("right");
                    }
                }
            }
            else if (e.KeyChar == 'a')
            {
                foreach (CFigure f in FigureList)
                {
                    if (f.highlight && f.CanMove("left", this))
                    {
                        f.Move("left");
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Save();
            File.Delete("D:\\figures.txt");
            File.WriteAllLines("D:\\figures.txt", saveFigures);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (CFigure figure in FigureList)
            {
                figure.SetHighlight(true);
            }
            for (int i = 0; i < FigureList.Count; i++)
            {
                if (FigureList[i].highlight)
                {
                    FigureList.RemoveAt(i);
                    i--;
                }
            }

            StreamReader sr = new StreamReader("D:\\figures.txt");

            while (!sr.EndOfStream)
            {
                FigureList.Add(read(sr));
            }
            sr.Close();
            Refresh();
            SyncLtoTree();
        }

        private void Save()
        {
            foreach (CFigure f in FigureList)
            {
                f.SaveFigures(saveFigures);
            }
        }

        CFigure read(StreamReader sr)
        {
            string line = sr.ReadLine();
            string[] data = line.Split(';');
            switch (data[0])
            {
                case "CGroup":
                    {
                        int count = int.Parse(data[1]);
                        CGroup newfigure = new CGroup();
                        for (int i = 0; i < count; i++)
                        {
                            newfigure.AddFigure(read(sr));
                        }
                        return newfigure;
                    }
                default:
                    {
                        int x = int.Parse(data[2]);
                        int y = int.Parse(data[3]);
                        int rad = int.Parse(data[4]);
                        bool selected = bool.Parse(data[5]);
                        Color color = Color.FromArgb(int.Parse(data[1]));
                        switch (data[0])
                        {
                            case "CCircle":
                                {
                                    CCircle newfigure = new CCircle(x, y, rad, color);
                                    newfigure.SetHighlight(selected);
                                    return newfigure;
                                }
                            case "CSquare":
                                {
                                    CSquare newfigure = new CSquare(x, y, rad, color);
                                    newfigure.SetHighlight(selected);
                                    return newfigure;
                                }
                            case "CTriangle":
                                {
                                    CTriangle newfigure = new CTriangle(x, y, rad, color);
                                    newfigure.SetHighlight(selected);
                                    return newfigure;
                                }
                            case "CSection":
                                {
                                    CSection newfigure = new CSection(x, y, rad, color);
                                    newfigure.SetHighlight(selected);
                                    return newfigure;
                                }
                        }
                        return null;
                    }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            foreach(CFigure f in FigureList)
            {
                f.SetHighlight(false);
            }
            Refresh();
            SyncLtoTree();
        }
        TreeNode readdata(StreamReader sr)
        {
            string line = sr.ReadLine();
            string[] data = line.Split(';');
            switch (data[0])
            {
                case "CGroup":
                    {
                        int count = int.Parse(data[1]);
                        TreeNode newnode = new TreeNode();
                        newnode.Text = data[0].ToString();

                        for (int i = 0; i < count; i++)
                        {
                            newnode.Nodes.Add(readdata(sr));
                        }
                        return newnode;
                    }
                default:
                    {
                        Color color = Color.FromArgb(int.Parse(data[1]));
                        TreeNode treeNode = new TreeNode();
                        treeNode.Text = data[0].ToString();
                        if (data[2] == "0")
                        {
                            treeNode.ForeColor = color;
                        }
                        else
                        {
                            treeNode.ForeColor = Color.Blue;
                        }
                        return treeNode;
                    }
            }
        }
        public void SyncLtoTree()
        {
            foreach (CFigure figure in FigureList)
            {
                figure.RetData(treeData);
            }
            File.WriteAllLines("D:\\tree.txt", treeData);
            treeView1.Nodes.Clear();

            StreamReader sr = new StreamReader("D:\\tree.txt");

            while (!sr.EndOfStream)
            {
                treeView1.Nodes.Add(readdata(sr));
            }
            sr.Close();
            treeData.Clear();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach (CFigure figure in FigureList)
            {
                figure.SetHighlight(false);
            }

            FigureList[e.Node.Index].SetHighlight(true);

            SyncLtoTree();

            e.Node.ForeColor = Color.Blue;
            Refresh();
            SyncLtoTree();
        }
    }

    public class CFigure
    {
        public int x, y, rad;
        public Color color;
        public bool highlight = false;
        public bool ctrl = false;
        public CFigure(int x, int y, int rad, Color color)
        {
            this.x = x;
            this.y = y;
            this.rad = rad;
            this.color = color;
        }
        public CFigure()
        {

        }
        public virtual void SetCtrl(bool ctrled)
        {
            ctrl = ctrled;
        }
        public virtual void SetHighlight(bool highlighted)
        {
            highlight = highlighted;
        }
        public virtual void SetColor(Color colored)
        {
            color = colored;
        }
        public virtual void ChangeSize(string change, Form form)
        {
            if (change == "plus" && rad<150)
            {
                if ((y + rad) < (int)form.ClientSize.Height && (x + rad) < (int)form.ClientSize.Width)
                {
                    rad += 5;
                }
            }
            else if (change == "minus" && rad > 5)
            {
                rad -= 5;
            }
        }
        public virtual bool CanMove(string direction, Form form)
        {
            if (direction == "up")
            {
                if ((y - rad) > 0)
                    return true;
                else
                    return false;
            }
            else if (direction == "down")
            {
                if ((y + rad) < (int)form.ClientSize.Height)
                    return true;
                else
                    return false;
            }
            else if (direction == "right")
            {
                if ((x + rad) < (int)form.ClientSize.Width)
                    return true;
                else
                    return false;
            }
            else if (direction == "left")
            {
                if ((x - rad) > 0)
                    return true;
                else
                    return false;
            }
            else return false;
        }
        public virtual void Move(string direction)
        {
            if (direction == "up")
            {
                y -= 1;
            }
            else if (direction == "down")
            {
                y += 1;
            }
            else if (direction == "right")
            {
                x += 1;
            }
            else if (direction == "left")
            {
                x -= 1;
            }
        }
        public virtual bool CheckMouse(MouseEventArgs mouse)
        {
            if (ctrl)
            {
                if (Math.Pow(mouse.X - x, 2) + Math.Pow(mouse.Y - y, 2) <= Math.Pow(rad, 2) && !highlight)
                {
                    highlight = true;
                    return true;
                }
            }
            return false;
        }
        public virtual void DrawFigure(Graphics g)
        {

        }
        public virtual void SaveFigures(List<String> saveFigures)
        {
            StringBuilder line = new StringBuilder();
            line.Append(ToString().Remove(0, 5)).Append(";");
            line.Append(color.ToArgb()).Append(";");
            line.Append(x.ToString()).Append(";");
            line.Append(y.ToString()).Append(";");
            line.Append(rad.ToString()).Append(";");
            line.Append(highlight.ToString()).Append(";");
            saveFigures.Add(line.ToString());
        }
        public virtual void RetData(List<string> treeData)
        {
            StringBuilder line = new StringBuilder();
            line.Append(ToString().Remove(0, 5)).Append(";");
            line.Append(color.ToArgb()).Append(";");
            if (highlight)
            {
                line.Append("1").Append(";");
            }
            else
            {
                line.Append("0").Append(";");
            }
            treeData.Add(line.ToString());
        }
    }
    public class CCircle : CFigure
    {
        public CCircle(int x, int y, int rad, Color color) : base(x, y, rad, color)
        {
        }

        public override void DrawFigure(Graphics g)
        {
            if (highlight)
                g.DrawEllipse(new Pen(Color.Black, 2), x - rad, y - rad, rad * 2, rad * 2);
            else
                g.DrawEllipse(new Pen(color, 2), x - rad, y - rad, rad * 2, rad * 2);
        }
    }
    public class CSquare : CFigure
    {
        public CSquare(int x, int y, int rad, Color color) : base(x, y, rad, color)
        {
        }
        public override void DrawFigure(Graphics g)
        {
            if (highlight)
                g.DrawRectangle(new Pen(Color.Black, 2), x - rad, y - rad, rad * 2, rad * 2);
            else
                g.DrawRectangle(new Pen(color, 2), x - rad, y - rad, rad * 2, rad * 2);
        }
    }
    public class CTriangle : CFigure
    {
        public CTriangle(int x, int y, int rad, Color color) : base(x, y, rad, color)
        {
        }
        public override void DrawFigure(Graphics g)
        {
            Point point1 = new Point(x, y - rad);
            Point point2 = new Point(x + rad, y + rad);
            Point point3 = new Point(x - rad, y + rad);
            Point[] Points = { point1, point2, point3 };

            if (highlight == true)
                g.DrawPolygon(new Pen(Color.Black, 2), Points);
            else
                g.DrawPolygon(new Pen(color, 2), Points);
        }
    }
    public class CSection : CFigure
    {
        public CSection(int x, int y, int rad, Color color) : base(x, y, rad, color)
        {
        }
        public override void DrawFigure(Graphics g)
        {
            if (highlight == true)
                g.DrawLine(new Pen(Color.Black, 2), x - rad, y, x + rad, y);
            else
                g.DrawLine(new Pen(color, 2), x - rad, y, x + rad, y);
        }
    }
    public class CGroup : CFigure
    {
        public List<CFigure> figures = new List<CFigure>();

        public CGroup()
        {
            highlight = true;
        }
        public void AddFigure(CFigure figure)
        {
            figures.Add(figure);
        }
        public override void SetCtrl(bool ctrled)
        {
            foreach (CFigure f in figures)
            {
                f.SetCtrl(ctrled);
            }
        }
        public override void SetHighlight(bool highlighted)
        {
            foreach (CFigure f in figures)
            {
                f.SetHighlight(highlighted);
            }
            highlight = highlighted;
        }
        public override void SetColor(Color colored)
        {
            foreach (CFigure f in figures)
            {
                f.SetColor(colored);
            }
        }
        public override void DrawFigure(Graphics g)
        {
            foreach (CFigure f in figures)
            {
                f.DrawFigure(g);
            }    
        }
        public override bool CanMove(string direction, Form form)
        {
            foreach (CFigure f in figures)
            {
                if (!f.CanMove(direction, form))
                {
                    return false;
                }
            }
            return true;
        }
        public override void Move(string direction)
        {
            foreach(CFigure f in figures)
            {
                f.Move(direction);
            }
        }
        public override void ChangeSize(string change, Form form)
        {
            foreach(CFigure f in figures)
            {
                f.ChangeSize(change, form);
            }
        }
        public override bool CheckMouse(MouseEventArgs mouse)
        {
            foreach (CFigure f in figures)
            {
                if (f.CheckMouse(mouse))
                    return true;
            }
            return false;
        }
        public override void SaveFigures(List<String> saveFigures)
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append(ToString().Remove(0, 5)).Append(";");
            tmp.Append(figures.Count().ToString()).Append(";");
            saveFigures.Add(tmp.ToString());
            foreach (CFigure f in figures)
            {
                f.SaveFigures(saveFigures);
            }
        }
        public override void RetData(List<string> treeData)
        {
            StringBuilder line = new StringBuilder();
            line.Append(ToString().Remove(0, 5)).Append(";");
            line.Append(figures.Count.ToString()).Append(";");
            treeData.Add(line.ToString());
            foreach (CFigure child in figures)
            {
                child.RetData(treeData);
            }
        }

        public class Line : CFigure
        {
            public List<CFigure> twofigs = new List<CFigure>();
            public bool f1s = false;
            public bool f2s = false;

            public void AddFigure(CFigure figure)
            {
                if (twofigs.Count < 2)
                {
                    figure.SetHighlight(false);
                    if (twofigs.Count == 0)
                        figure.SetColor(Color.ForestGreen);
                    else
                        figure.SetColor(Color.DarkOliveGreen);
                    twofigs.Add(figure);
                }
            }

            public override void SetCtrl(bool pressed)
            {
                foreach (CFigure component in twofigs)
                {
                    component.ctrl = pressed;
                }
                ctrl = pressed;
            }

            public override void SetHighlight(bool cond)
            {
                if (!highlight)
                {
                    twofigs[0].SetHighlight(f1s);
                    twofigs[1].SetHighlight(f2s);
                    highlight = cond;
                }
                else
                {
                    foreach (CFigure component in twofigs)
                    {
                        component.SetHighlight(cond);
                    }
                    highlight = cond;
                    f1s = cond;
                    f2s = cond;
                }
            }

            public override bool CheckMouse(MouseEventArgs e)
            {
                if (twofigs[0].CheckMouse(e))
                {
                    f1s = true;
                    return true;
                }
                else if (twofigs[1].CheckMouse(e))
                {
                    f2s = true;
                    return true;
                }
                else
                {
                    f1s = false;
                    f2s = false;
                    return false;
                }
            }
        }
    }
}
