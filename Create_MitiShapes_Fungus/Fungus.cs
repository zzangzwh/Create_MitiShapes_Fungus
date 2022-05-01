using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using GDIDrawer;

namespace Create_MitiShapes_Fungus
{
    public class Fungus
    {
        //static ConcurrentDictionary<Point, Color> - shared by all Fungus instances
        private static ConcurrentDictionary<Point, Color> dicFung = new ConcurrentDictionary<Point, Color>();
        //count
        public static int count = 0;
        //current location
        private Point fLocation;
        //color fungus
        private Color fClr;
        private static CDrawer fGdi;
        //fungus thread
        Thread fThread;
        private static Random rnd = new Random();

        /// <summary>
        /// add a public manual property called NextPoint . It will marshall/lock access creating and
        /// //returning a random point within the CDrawer. lock() this operation to protect thread corruption
        //returning out of the lock() is fine, as the lock will be released upon return.
        /// </summary>
        public Point NextPoint
        {
            get
            {
                // returning out of the lock() is fine, as the lock will be released upon return.
                // using substitute it protects the object
                Point pt = new Point(Substitute(fGdi.ScaledWidth), Substitute(fGdi.ScaledHeight));
                return pt;

            }
        }
        /// <summary>
        ///   public static int Substitute(int num)
        /// add a public static method as a substitute for(Max), which will lock() then generate and
        //return the random value in the range required, again protecting the Random object.
        /// </summary>
        /// <param name="num">integer num</param>
        /// <returns></returns>
        public static int Substitute(int max)
        {
            int val;
            //lock() then generate and
            //return the random value in the range required, again protecting the Random object
            lock (rnd)
            {
                val = rnd.Next(0, max + 1);
            }
            return val;
        }
        /// <summary>
        /// ctor
        ///  public Fungus(CDrawer gdi, Color c)
        /// </summary>
        /// <param name="gdi">cdrawer</param>
        /// <param name="c">color</param>
        public Fungus(CDrawer gdi, Color c)
        {
            fGdi = gdi;
            fClr = c;
            //thread
            fThread = new Thread(StartFungusThread);
            fThread.IsBackground = true;
            fThread.Start();

            // get random new point
            fLocation = NextPoint;
            // increment fungus counter
            count++;
        }


        /// <summary>
        /// thread
        ///  public void StartFungusThread()
        /// </summary>
        public void StartFungusThread()
        {
            bool flag = true;
            int backtrack = 100_000;
            /* If the dictionary indicates the CDrawer is full job done, reset the CDrawer background to Black
             and clear the dictionary -starting from scratch.*/
            while (flag)
            {
                //If the dictionary indicates the CDrawer is full job done, reset the CDrawer background to Black
                //and clear the dictionary -starting from scratch.
                if (dicFung.Count >= fGdi.ScaledWidth * fGdi.ScaledHeight)
                {
                    fGdi.BBColour = Color.Black;
                    fGdi.Clear();
                }
                //  Generate a list of the possible adjacent points to the current location(there should be eight of
                //them, and you could write a helper method to produce this list).

                List<Point> adj = GetAdj(fLocation).Where(p => p.X > 0 && p.X < fGdi.ScaledWidth && p.Y > 0 && p.Y < fGdi.ScaledHeight).ToList();
                adj.RemoveAll(p => dicFung.ContainsKey(p) && dicFung[p] != fClr && dicFung[p] != Color.Black);
                // create lists for visited location and unvisited location
                List<Point> notVisited;
                List<Point> visited;
                // An optional optimization at this step would be to separate the potential points into 2
                //groups – unvisited and visited, shuffle both and combine them back together in “best - toworst” order – the result will to use shuffle unvisited before using any back-tracking

                lock (dicFung)
                {
                    notVisited = Shuffle(adj.Where(p => !dicFung.ContainsKey(p))).ToList();
                    visited = Shuffle(adj.Where(p => dicFung.ContainsKey(p))).ToList();

                }

                adj = notVisited.Concat(visited).ToList();
                //sleep
                Thread.Sleep(0);
                // between filtering our points and shuffling, our point collection may not be valid, so we need
                //to try each point to see if it is a valid move candidate, if it is (open or our color), take it( if open)
                //    and set our current point.

                // using foreach loop to determine if it is a valid move candidate
                foreach (Point p in adj)
                {
                    // check if the candidate is valid
                    if (dicFung.TryAdd(p, fClr))
                    {
                        // update new current location
                        fLocation = p;
                        // set back-buffer pixel
                        fGdi.SetBBScaledPixel(fLocation.X, fLocation.Y, fClr);
                        // jump out of a foreach
                        break;
                    }
                    // show message on the output panel
                    // if any spot we check is currently occupied by another fungus
                    if (dicFung.ContainsKey(p) && dicFung[p] != fClr)
                        Trace.WriteLine($"Fungus[{fClr.Name}] : {p} - candidate spot taken ...");
                    // if the spot we took was already our color,
                    // reduce backtracking count by one
                    else if (dicFung.ContainsKey(p) && dicFung[p].Equals(fClr))
                        backtrack--;

                    // update new current location
                    fLocation = p;
                }
                if (backtrack < 1)
                {
                    count--;
                    Trace.WriteLine($"FUNGUS[{fClr.Name}] : terminated, too much backtracking...");
                    flag = false;

                }

            }

        }
        /// <summary>
        ///   public List<Point> GetAdj(Point cPoint)
        /// </summary>
        /// <param name="cPoint"> point</param>
        /// <returns></returns>
        public List<Point> GetAdj(Point cPoint)
        {
            List<Point> _lPoint = new List<Point>();
            //left
            _lPoint.Add(new Point(cPoint.X - 1, cPoint.Y));
            //top left
            _lPoint.Add(new Point(cPoint.X - 1, cPoint.Y - 1));
            //top 
            _lPoint.Add(new Point(cPoint.X, cPoint.Y - 1));
            //top right
            _lPoint.Add(new Point(cPoint.X + 1, cPoint.Y - 1));
            //right
            _lPoint.Add(new Point(cPoint.X + 1, cPoint.Y));
            //bottom right;
            _lPoint.Add(new Point(cPoint.X + 1, cPoint.Y + 1));
            //bottom
            _lPoint.Add(new Point(cPoint.X, cPoint.Y + 1));
            //bottom left
            _lPoint.Add(new Point(cPoint.X - 1, cPoint.Y + 1));

            return _lPoint;
        }
        //- returns an IEnumerable<Point>, accepting an IEnumerable<Point>
        public static IEnumerable<Point> Shuffle(IEnumerable<Point> iP)
        {
            //It will return a new shuffled collection of the invoking collection Fisher-Yates implementation
            List<Point> temp = iP.ToList();
            for (int i = 0; i < temp.Count(); i++)
            {
                int j = rnd.Next(i, temp.Count);
                Point p = temp[i];
                temp[i] = temp[j];
                temp[j] = p;

            }
            return temp;
        }
    }
}
