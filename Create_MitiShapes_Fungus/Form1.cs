/*
 * Name :Wonhyuk Cho
 * 
 * Description : Creating fungus and multi shpaes in Graphic Drawer           
 * 
 *

 */
using GDIDrawer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WCho_lib;

namespace Create_MitiShapes_Fungus
{
    public partial class Form1 : Form
    {
        //Timer for Tick event 
        Timer _timer = new Timer();
        //Creating fungus from fungus class
        List<Fungus> _fList = new List<Fungus>();
        //GDIDrawer 
        PosiDrawer _gdi;
        //Creating multishpaes from Shape class
        List<Shape> _shapes = new List<Shape>();
        public Form1()
        {
            InitializeComponent();
            this.Text = "Wonhyuk Cho";
            _timer.Enabled = true;
            _timer.Tick += _timer_Tick;
            _timer.Interval = 50;
            _gdi = new PosiDrawer(1000, 1000, this, PosiDrawer.EPosition.eRight);
            _gdi.BBColour = Color.Black;
            Shown += Form1_Shown;
            _gdi.ContinuousUpdate = false;
            Test();

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            _gdi.SetPosition(this, PosiDrawer.EPosition.eRight);
            Activate();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _gdi.Clear();
            //count fungus let than 8
            if (Fungus.count < 8)
                new Fungus(_gdi, RandColor.GetColor());

            //iterate through shape and render and tick
            for (int i = 0; i < _shapes.Count; ++i)
            {
                if (_shapes[i] is IAnimate)
                    (_shapes[i] as IAnimate).Tick();
                _shapes[i].Render(_gdi);

            }
            _gdi.Render();
        }
        private void Test()
        {
            // This code assumes you have a List<Shape> called _shapes
            // If you do not, make one and after this code completes, add it to your
            // actual collection used by your timer

            // CTOR Declarations used for this test code
            //public FixedSquare( PointF p, Color c, Shape parent = null, int iSize = 20 )
            //public AniShape( PointF p, Color c, Shape parent = null, double dAniIncrement = 0, double dAniValue = 0 )
            //public AniPoly( PointF p, Color c, int sides, Shape parent = null, double dAniIncrement = 0, double dAniValue = 0,  int iRadius = 25 )
            //public AniChild(Color c, double dDistToParent, Shape parent, double dAniIncrement = 0, double dAniValue = 0)
            //public AniHighlight(Color c, Shape parent, double dAniIncrement = 0, double dAniValue = 0)
            //public OrbitBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0, double dWHRatio = 1 )
            //public VWobbleBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0)
            //public HWobbleBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0)


            int DebugLevel = 9;
            // FixedSquare
            // center solid anchors
            if (DebugLevel > 0)
            { // Bottoms Up
                _shapes.Add(new FixedSquare(new Point(50, 50), Color.Orange, null));
                _shapes.Add(new FixedSquare(new Point(50, 100), Color.Orange, _shapes.Last()));
                _shapes.Add(new FixedSquare(new Point(100, 100), Color.Orange, _shapes.Last()));
                _shapes.Add(new FixedSquare(new Point(100, 50), Color.Orange, _shapes.Last()));
            }

            // AniPoly Only
            // show animated polygons (interlocking triangles) 
            if (DebugLevel > 1)
            {
                _shapes.Add(new AniPoly(new PointF(200, 100), Color.Wheat, 3, null, 0.1));
                _shapes.Add(new AniPoly(new PointF(235, 100), Color.Wheat, 3, null, -0.1, 1));
                _shapes.Add(new AniPoly(new PointF(270, 100), Color.Wheat, 3, null, 0.1));

                _shapes.Add(new AniPoly(new PointF(400, 50), Color.Wheat, 3, null, 0.1, 1, 50));
                _shapes.Add(new AniPoly(new PointF(470, 25), Color.Wheat, 4, null, -0.1, 1));
                _shapes.Add(new AniPoly(new PointF(500, 50), Color.Wheat, 5, null, 0.1, 0, 50));
            }

            // FixedSquare, AniHighlight
            // show highlight on a fixed square 
            if (DebugLevel > 2)
            {
                List<Shape> local = new List<Shape>();
                local.Add(new FixedSquare(new PointF(700, 100), Color.LightCoral, null));
                local.Add(new AniHighlight(Color.Yellow, local.Last(), -0.1));
                local.Add(new FixedSquare(new PointF(800, 100), Color.LightCoral, null, 10));
                local.Add(new AniHighlight(Color.Yellow, local.Last(), 0.2));
                _shapes.AddRange(local);
            }

            // FixedSquare, HWobble
            // show string of adjacent relative horizontal wobble balls 
            if (DebugLevel > 3)
            {
                List<Shape> local = new List<Shape>();
                local.Add(new FixedSquare(new PointF(500, 20), Color.GreenYellow, null));
                for (int i = 1; i < 15; ++i)
                    local.Add(new HWobbleBall(Color.OrangeRed, 30, local.Last(), 0.15));
                local.Add(new FixedSquare(new PointF(500, 980), Color.GreenYellow, null));
                for (int i = 1; i < 15; ++i)
                    local.Add(new HWobbleBall(Color.OrangeRed, 30, local.Last(), 0.15, Math.PI));
                _shapes.AddRange(local);
            }

            // FixedSquere, VWobble
            // show the top row of solid blocks with incremental offset animated vwballs 
            if (DebugLevel > 5)
            {
                double aniVal = 0;
                List<Shape> local = new List<Shape>();
                for (int i = 50; i < 1000; i += 50)
                {
                    local.Add(new FixedSquare(new PointF(i, 800), Color.Fuchsia, null));
                    local.Add(new VWobbleBall(Color.LimeGreen, 15 * aniVal, local.Last(), 0.1, aniVal += 0.7));
                }
                _shapes.AddRange(local);
            }

            // OrbitBall
            // ccw orbit chain 
            if (DebugLevel > 6)
            {
                List<Shape> local = new List<Shape>();
                local.Add(new FixedSquare(new PointF(250, 350), Color.GreenYellow, null));
                local.Add(new OrbitBall(Color.Yellow, 100, local.Last(), -0.1, Math.PI));
                local.Add(new OrbitBall(Color.Turquoise, 75, local.Last(), -0.15, Math.PI));
                local.Add(new OrbitBall(Color.Blue, 50, local.Last(), -0.2, Math.PI));
                local.Add(new OrbitBall(Color.Green, 25, local.Last(), -0.25, Math.PI));

                // cw orbit chain 
                local.Add(new FixedSquare(new PointF(750, 350), Color.GreenYellow, null));
                local.Add(new OrbitBall(Color.Yellow, 50, local.Last(), 0.05));
                local.Add(new OrbitBall(Color.Pink, 50, local.Last(), 0.075));
                local.Add(new OrbitBall(Color.Blue, 50, local.Last(), 0.1));
                local.Add(new OrbitBall(Color.Green, 50, local.Last(), 0.125));
                _shapes.AddRange(local);
            }


            // FixedSquare, OrbitBall
            //…continues… // show 3-tier cloud of quad balls orbiting the same block 
            if (DebugLevel > 6)
            {
                List<Shape> local = new List<Shape>();
                local.Add(new FixedSquare(new PointF(200, 650), Color.GreenYellow, null));
                local.Add(new OrbitBall(Color.Yellow, 30, local.First(), 0.1, 0));
                local.Add(new OrbitBall(Color.Yellow, 30, local.First(), 0.1, Math.PI / 2));
                local.Add(new OrbitBall(Color.Yellow, 30, local.First(), 0.1, Math.PI));
                local.Add(new OrbitBall(Color.Yellow, 30, local.First(), 0.1, 3 * Math.PI / 2));
                local.Add(new OrbitBall(Color.Yellow, 60, local.First(), -0.05, 0));
                local.Add(new OrbitBall(Color.Yellow, 60, local.First(), -0.05, Math.PI / 2));
                local.Add(new OrbitBall(Color.Yellow, 60, local.First(), -0.05, 3 * Math.PI));
                local.Add(new OrbitBall(Color.Yellow, 60, local.First(), -0.05, 3 * Math.PI / 2));
                local.Add(new OrbitBall(Color.Yellow, 90, local.First(), 0.025, 0));
                local.Add(new OrbitBall(Color.Yellow, 90, local.First(), 0.025, Math.PI / 2));
                local.Add(new OrbitBall(Color.Yellow, 90, local.First(), 0.025, Math.PI));
                local.Add(new OrbitBall(Color.Blue, 90, local.First(), 0.025, 3 * Math.PI / 2));
                _shapes.AddRange(local);
            }

            // FixedSquare OrbitBall
            // show OrbitBall with Ratio value, Elliptical orbits
            if (DebugLevel > 7)
            {
                List<Shape> local = new List<Shape>();
                local.Add(new FixedSquare(new PointF(500, 400), Color.Violet, null));
                local.Add(new OrbitBall(Color.HotPink, 50, local.First(), 0.1, 0, 2));
                local.Add(new OrbitBall(Color.HotPink, 50, local.First(), 0.1, Math.PI / 2, 2));
                local.Add(new OrbitBall(Color.HotPink, 50, local.First(), 0.1, Math.PI, 2));
                local.Add(new OrbitBall(Color.HotPink, 50, local.First(), 0.1, 3 * Math.PI / 2, 2));
                local.Add(new OrbitBall(Color.YellowGreen, 300, local.First(), -0.05, 0, 0.5));
                local.Add(new OrbitBall(Color.YellowGreen, 300, local.First(), -0.05, Math.PI / 2, 0.5));
                local.Add(new OrbitBall(Color.YellowGreen, 300, local.First(), -0.05, 3 * Math.PI, 0.5));
                local.Add(new OrbitBall(Color.YellowGreen, 300, local.First(), -0.05, 3 * Math.PI / 2, 0.5));
                _shapes.AddRange(local);
            }

            // FixedSquare, OrbitBall, VWobble, HWobble
            if (DebugLevel > 8)
            {
                List<Shape> local = new List<Shape>();
                local.Add(new FixedSquare(new PointF(800, 550), Color.Cyan, null));
                local.Add(new HWobbleBall(Color.Red, 100, local.Last(), 0.1));
                local.Add(new VWobbleBall(Color.Red, 100, local.Last(), 0.15));
                local.Add(new OrbitBall(Color.LightBlue, 25, local.Last(), -0.2));
                _shapes.AddRange(local);
            }
        }

        }
    }
