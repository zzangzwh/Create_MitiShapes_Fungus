using GDIDrawer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create_MitiShapes_Fungus
{
    //interface Irender
    public interface IRender
    {
        // render instance to the supplied drawer
        void Render(CDrawer dr);
    }
    //interface Ianimate
    public interface IAnimate
    {
        // cause per-tick state changes to instance (movement, animation, etc...)
        void Tick();
    }
    /* Before you tackle the code, note that your client has provided a test method which holds a framework of
    conditional tests relevant for each completion step.This code also provides the CTOR signatures that you
    must implement to use the framework.The client is expecting the test harness to not require any
    modifications, so review the code before you start defining your classes.*/
    public abstract class Shape : IRender
    {
        /*    This abstract class defines the basics of all shapes.It should contain a PointF for the position of the shape, a
            *    color, and a Shape object reference for the ‘parent’ of this instance.If the shape has no parent, this
                 reference will be null.*/
        protected PointF sPosition;
        protected Color sClr;
        protected Color lineClr = Color.White;
        protected Shape parentShape;

        //CTOR accepts poitnf color and shape
        public Shape(PointF p, Color c, Shape sP)
        {
            sPosition = p;
            sClr = c;
            parentShape = sP;
        }
        public PointF getPotint
        {
            get
            {
                return sPosition;
            }
        }

        /*   This class will support the IRender interface. Using NVI, this Render method must be non-virtual, and
               invoke your protected virtual polymorphic sister method(vRender? ). All shapes will draw a white line
               between the instance and its parent(if it exists). This behavior is common to all shapes.This must be
               done either prior-to or after invoking vRender() - what impact does this have? Is one better than the
               other?
               */
        public void Render(CDrawer dr)
        {
            vRender(dr);

        }
        /// <summary>
        ///   protected virtual void vRender(CDrawer dr)
        /// </summary>
        /// <param name="dr"> Gdrawer </param>
        protected virtual void vRender(CDrawer dr)
        {
            if (parentShape != null)
            {
                dr.AddLine((int)sPosition.X, (int)sPosition.Y, (int)parentShape.sPosition.X, (int)parentShape.sPosition.Y, lineClr);
            }

        }

    }
    /* The FixedSquare class will allow the rendering of 20 x 20 pixel solid colored squares centered at their
         position.This class will derive from Shape.
         Add a constructor that is able to satisfy the base class.
         Add any other fields and method implementations that are required to complete this class.*/
    public class FixedSquare : Shape
    {
        protected int squareSize;
        //Add a constructor that is able to satisfy the base class.
        //Add any other fields and method implementations that are required to complete this class
        public FixedSquare(PointF p, Color c, Shape parent, int iSize = 20) : base(p, c, parent)
        {
            squareSize = iSize;
        }

        protected override void vRender(CDrawer dr)
        {
            base.vRender(dr);
            dr.AddCenteredRectangle((int)sPosition.X, (int)sPosition.Y, squareSize, squareSize, sClr, 2, Color.Black);
        }


    }
    /*The abstract AniShape class defines the basics of all animating shapes.It should contain a double for the
sequence value that controls the animation, and a double for the sequence delta (how much the
sequence value changes by each tick). The sequence value may be interpreted differently by derived
classes, but in most cases it will be fed into a Sin or Cos call to control position or rotation.
*/
    public abstract class AniShape : Shape, IAnimate
    {
        protected double sequenceValue;
        protected double delta;
        //protected double animation;
        /*      This class supports the IAnimate interface. With NVI in mind, implement this requirement.With this class
               *      acting as the foundation of Animated classes, implement the non-virtual (Tick ) and a virtual sister
            method to allow derived overrides.Unlike abstract vRender, provide an actual implementation that will
            sum the sequence delta to the sequence value.This provides a basic functional foundation, but one that
            can be replaced/overridden in derived objects.*/
        public AniShape(PointF p, Color c, Shape parent = null, double dAniIncrement = 0, double dAniValue = 0) : base(p, c, parent)
        {
            sequenceValue = dAniValue;
            delta = dAniIncrement;
        }
        public void Tick()
        {
            vTick();
        }
        //increaed by delta
        protected virtual void vTick()
        {
            sequenceValue += delta;
        }
    }
    /// <summary>
    /// The AniPoly class derives from AniShape and provides very basic polygon animation.
    // This class will need a field for the side count of the polygon.
    /// </summary>
    public class AniPoly : AniShape
    {
        protected int side;
        protected int radius;
        /*Add a suitable constructor that is able to initialize this class and the base. If the side count provided is less
        than 3 throw an ArgumentException describing the problem.*/
        public AniPoly(PointF p, Color c, int sides, Shape parent = null, double dAniIncrement = 0, double dAniValue = 0, int iRadius = 25) : base(p, c, parent, dAniIncrement, dAniValue)
        {
            side = sides;
            radius = iRadius;
            if (side < 3) throw new ArgumentException("count provided is less than 3");


        }
        /// <summary>
        ///  protected override void vRender(CDrawer dr)

        /// this class will render the polygon with the CDrawer AddPolygon method. Use a value of 25 for the vertex
        //radius.Use the sequence value for rotation.
        /// </summary>
        /// <param name="dr"> cdrawer</param>
        protected override void vRender(CDrawer dr)
        {
            base.vRender(dr);
            dr.AddPolygon((int)this.sPosition.X, (int)this.sPosition.Y, radius, side, sequenceValue, sClr, 3, Color.Red);
        }


    }
    /// <summary>
    /// The abstract AniChild class derives from AniShape and is the foundation for all shapes that require a
    // parent.
    /// </summary>
    public abstract class AniChild : AniShape
    {
        protected double distance;
        /* This type will require a double field to hold the distance of this shape from its parent.
             Add a suitable constructor that is able to initialize this class and its base. If the parent provided is null,
             throw an ArgumentException that describes the problem.
             Typically the position field for classes that derive from this type will be set in the Tick method, as the
             position is calculated using the parent position and calculating an offset using the current sequence
             value.The constructor should therefore call Tick to ensure that the position is given a value.
             Add any other fields and method implementations that are required to complete this class*/
        public AniChild(Color c, double dDistToParent, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(new PointF(parent.getPotint.X, parent.getPotint.Y), c, parent, dAniIncrement, dAniValue)
        {
            if (parent is null) throw new ArgumentException("Parent is null Exception happend!");
            distance = dDistToParent;
            this.Tick();
        }


    }
    /// <summary>
    /// The AniHighlight class will derive from AniChild. This is a specialized class that will be used to show
    //highlighting of the parent through some animation.This class is odd as the position will be the same as
    //the parent, and will not be modified by the Tick method.In the demo the distance determines the
    //distance of the animation oscillates from the center of the parent.
    //Add a suitable constructor to initialize this class and the base.
    /// </summary>
    public class AniHighlight : AniChild
    {
        protected double radius = 50;
        protected int temp;
        /*   Add any other fields and method implementations that are required to complete this class. Think.Note the
               rendering that you do in this class must draw attention to the parent(with animation) but must not
               obscure the parent.The demo uses a translucent growing/shrinking “shield” emanating from the parent.*/

        public AniHighlight(Color c, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c, 0, parent, dAniIncrement, dAniValue)
        {
            delta = dAniIncrement;
        }
        /// <summary>
        ///   protected override void vRender(CDrawer dr)
        /// </summary>
        /// <param name="dr"> Cdrawer</param>
        protected override void vRender(CDrawer dr)
        {

            temp = (int)(radius + radius / 2 * Math.Cos(sequenceValue));
            dr.AddCenteredEllipse((int)parentShape.getPotint.X, (int)parentShape.getPotint.Y,
                                    Math.Max(temp, 1), Math.Max(temp, 1), Color.FromArgb(150, sClr));
            base.vRender(dr);

        }
    }
    /// <summary>
    /// The abstract AniBall class derives from AniChild and is the foundation for ball rendering children. All
    //derived instances will be rendered in the same way – a 20 x 20 centered ellipse.Place the code to do this
    //in your virtual Render support override.
    //Add a suitable constructor to initialize this class and its base.
    //Add any other fields and method implementations that are required to complete this class
    /// </summary>
    public abstract class AniBall : AniChild
    {
        public AniBall(Color c, double dDistToParent, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c, dDistToParent, parent, dAniIncrement, dAniValue)
        {

        }
        /// <summary>
        ///  protected override void vRender(CDrawer dr)
        /// </summary>
        /// <param name="dr"> cdrawer</param>
        protected override void vRender(CDrawer dr)
        {
            dr.AddCenteredEllipse((int)this.getPotint.X, (int)this.getPotint.Y, 20, 20, sClr);
            base.vRender(dr);
        }
    }
    /// <summary>
    /// The VWobbleBall class derives from AniBall and represents a ball that vertically wobbles on its parent. This
    //will require implementation in the override for Tick.Use of a trig or related expression using the
    //animation sequence value is required for the remaining classes.Do NOT use a linear boundary to
    //boundary adjustment, sin() and cos() are your friends here.
    /// </summary>
    public class VWobbleBall : AniBall
    {
        public VWobbleBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c, dParentDistance, parent, dAniIncrement, dAniValue)
        {
        }

        protected override void vTick()
        {
            base.vTick();
            this.sPosition.X = (float)(this.parentShape.getPotint.X);
            this.sPosition.Y = (float)(this.parentShape.getPotint.Y + distance * Math.Cos(sequenceValue));
        }

    }
    //The HWobbleBall class derives from AniBall and represents a ball that horizontally wobbles on its parent.
    //This will require implementation in the override for Tick.
    public class HWobbleBall : AniBall
    {

        public HWobbleBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0) : base(c, dParentDistance, parent, dAniIncrement, dAniValue)
        {

        }
        ////This will require implementation in the override for Tick.
        protected override void vTick()
        {
            base.vTick();
            this.sPosition.X = (float)(this.parentShape.getPotint.X + distance * Math.Sin(sequenceValue));
            this.sPosition.Y = (float)(this.parentShape.getPotint.Y);
        }


    }
    /// <summary>
    /// The OrbitBall class derives from AniBall and represents a ball that ‘orbits’ its parent. This will require
    //    implementation in the override for Tick.
    //As the notion of orbit has an X, Y component, add an additional member of double representing the ratio
    //of the distance with respect to Y(ie.ratio of 1 = circle, whereas ratio of 2 gives a Y orbit twice as big ) In
    //the constructor argument list provide a default ratio value of 1. OrbitBall Tick is really the combination of
    //VWobble and HWobble processing, creating a 2D path rather than a 1D path.

    /// </summary>
    public class OrbitBall : AniBall
    {
        protected double ratio;
        public OrbitBall(Color c, double dParentDistance, Shape parent, double dAniIncrement = 0, double dAniValue = 0, double dWHRatio = 1) : base(c, dParentDistance, parent, dAniIncrement, dAniValue)
        {
            ratio = dWHRatio;
            if (ratio != 1)
                lineClr = Color.Transparent;
        }
        /// <summary>
        ///   protected override void vTick()
        /// As the notion of orbit has an X, Y component, add an additional member of double representing the ratio
        //of the distance with respect to Y(ie.ratio of 1 = circle, whereas ratio of 2 gives a Y orbit twice as big ) In
        //the constructor argument list provide a default ratio value of 1. OrbitBall Tick is really the combination of
        //VWobble and HWobble processing, creating a 2D path rather than a 1D path.

        /// </summary>
        protected override void vTick()
        {
            base.vTick();
            this.sPosition.X = (float)(this.parentShape.getPotint.X + distance * Math.Sin(sequenceValue));
            this.sPosition.Y = (float)(this.parentShape.getPotint.Y + distance * Math.Cos(sequenceValue) * ratio);
        }
    }
}

