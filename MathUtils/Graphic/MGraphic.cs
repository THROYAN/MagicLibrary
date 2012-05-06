using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MagicLibrary.MathUtils;

namespace MagicLibrary.Graphic
{
    public static class MGraphic
    {
        public static Matrix R(double angle)
        {
            double a = angle * Math.PI / 180;
            Matrix mR = new Matrix(new double[3, 3] {
                                                        {Math.Cos(a),-Math.Sin(a),0},
                                                        {Math.Sin(a),Math.Cos(a),0},
                                                        {0,0,1}
                                                    });
            return mR;
        }

        public static Matrix R(double angle, PointF p)
        {
            return T(p) * R(angle) * T(-p.X, -p.Y);
        }

        public static Matrix S(double sx, double sy)
        {
            Matrix mS = new Matrix(new double[3, 3]{
                                                        {sx,0,0},
                                                        {0,sy,0},
                                                        {0,0,1}
            });
            return mS;
        }

        public static Matrix T(PointF d)
        {
            Matrix mT = new Matrix(new double[3, 3]{
                                                        {1,0,d.X},
                                                        {0,1,d.Y},
                                                        {0,0,1}
            });
            return mT;
        }

        public static Matrix T(float X,float Y)
        {
            return T(new PointF(X, Y));
        }

        public static Matrix Pr(PointF O)
        {
            Matrix mPr = new Matrix(new double[3, 3]{
                                                        {1,0,0},
                                                        {0,1,0},
                                                        {-1/O.X,-1/O.Y,1}
            });
            return mPr;
        }
        public static PointF Rotation(PointF P, double angle)
        {
            double a = angle * Math.PI / 180;
            Matrix mP = new Matrix(new double[2, 1]{
                                                        {P.X},
                                                        {P.Y}
            });
            Matrix mR = new Matrix(new double[2, 2] {
                                                        {Math.Cos(a),-Math.Sin(a)},
                                                        {Math.Sin(a),Math.Cos(a)}
            });
            Matrix res = mR * mP;
            return res.ToPointF();
        }
        public static PointF Transpanation(PointF P, PointF d)
        {
            Matrix mP = new Matrix(new double[3, 1]{
                                                        {P.X},
                                                        {P.Y},
                                                        {1}
            });
            Matrix mT = new Matrix(new double[3, 3]{
                                                        {1,0,d.X},
                                                        {0,1,d.Y},
                                                        {0,0,1}
            });
            Matrix res = mT * mP;
            return res.ToPointF();
        }

        public static PointF Scale(PointF P, double sx, double sy)
        {
            Matrix mP = new Matrix(new double[2, 1]{
                                                        {P.X},
                                                        {P.Y}
            });
            Matrix mS = new Matrix(new double[2, 2]{
                                                        {sx,0},
                                                        {0,sy}
            });
            Matrix res = mS * mP;
            return res.ToPointF();
        }

        public static PointF Proekciya(PointF P, PointF O)
        {
            Matrix mP = new Matrix(new double[3, 1]{
                                                        {P.X},
                                                        {P.Y},
                                                        {1}
            });
            Matrix mPr = new Matrix(new double[3, 3]{
                                                        {1,0,0},
                                                        {0,1,0},
                                                        {1/O.X,1/O.Y,1}
            });
            Matrix res = mPr * mP;
            return res.ToPointF();
        }

        public static PointF Projection(PointF P, double a, double b)
        {
            return Proekciya(P, new PointF((float)a,(float)b));
        }

        public static PointF Transpanation(PointF P, double dx, double dy)
        {
            return Transpanation(P, new PointF((float)dx, (float)dy));
        }

        public static PointF UseMixedTransformation(PointF P, Matrix m)
        {
            return m * P;
        }

        public static PointF UseMixedTransformation(float X, float Y, Matrix m)
        {
            return m * (new PointF(X, Y));
        }

        public static PointF[] UseMixedTransformations(PointF[] Ps, Matrix m)
        {
            PointF[] nPs = new PointF[Ps.Length];
            for (int i = 0; i < Ps.Length; i++)
                nPs[i] = UseMixedTransformation(Ps[i], m);
            return nPs;
        }

        private static PointF[] SplineFunctionOo(PointF[] points, double t)
        {
            PointF[] result = new PointF[points.Length - 1];

            for (int i = 0; i < points.Length - 1; i++)
            {
                result[i] = new PointF(
                    (1 - (float)t) * points[i].X + (float)t * points[i + 1].X,
                    (1 - (float)t) * points[i].Y + (float)t * points[i + 1].Y
                    );
            }

            return result;
        }

        private static PointF SplinePoint(PointF[] points, double t)
        {
            PointF[] p = new PointF[points.Length];
            points.CopyTo(p, 0);

            while (p.Length > 1)
            {
                p = SplineFunctionOo(p, t);
            }
            return p[0];
        }

        private static PointF[] SplinePoints(PointF[] points, double eps)
        {
            List<PointF> ps = new List<PointF>();
            for (double t = 0; t <= 1; t += eps)
            {
                ps.Add(SplinePoint(points, t));
            }
            return ps.ToArray();
        }

        public static void DrawSpline(Graphics g, Pen p, PointF[] points, double eps = 0.001)
        {
            g.DrawLines(p, SplinePoints(points,eps));
        }

        public static void DrawSpline(Graphics g, Pen p, PointF[] points, Matrix m, double eps = 0.001)
        {
            DrawSpline(g, p, m * points, eps);
        }

        private static PointF[][] PointsForEllipse(float x, float y, float width, float height)
        {
            return new PointF[][]{
                new PointF[]{
                    new PointF(x,y + height/2), //  _
                    new PointF(x,y+height/4),
                    //new PointF(x,y),      //  /
                    new PointF(x +width/4,y),
                    new PointF(x + width/2,y),  
                },
                new PointF[]{
                    new PointF(x + width/2,y), //  _
                    new PointF(x+3*width/4,y),
                    //new PointF(x+w,y),     //   \
                    new PointF(x + width, y+height/4),
                    new PointF(x+width,y+height/2),
                },
                new PointF[]{
                    new PointF(x+width,y+height/2), 
                    new PointF(x+width,y+3*height/4),
                    //new PointF(x+w,y+h),//  _/
                    new PointF(x+3 * width / 4,y + height), 
                    new PointF(x+width/2,y+height),
                },
                new PointF[]{
                    new PointF(x+width/2,y+height),
                    new PointF(x+width/4,y+height), 
                    //new PointF(x,y+h), //  \_
                    new PointF(x,y+3*height/4), 
                    new PointF(x,y+height/2),
                },
            };
        }

        public static void DrawEllipse(Graphics g, Pen p, float x, float y, float width, float height, Matrix m = null, double eps = 0.001)
        {
            if (m == null)
            {
                PointsForEllipse(x, y, width, height).ToList().ForEach(pt => MGraphic.DrawSpline(g, p, pt, eps));
            }
            else
                PointsForEllipse(x, y, width, height).ToList().ForEach(pt => MGraphic.DrawSpline(g, p, m * pt, eps));
        }

        public static void DrawEllipse(Graphics g, Pen p, PointF pt, SizeF s, Matrix m = null, double eps = 0.001)
        {
            DrawEllipse(g, p, pt.X, pt.Y, s.Width, s.Height, m, eps);
        }

        public static void DrawEllipse(Graphics g, Pen p, RectangleF r, Matrix m = null, double eps = 0.001)
        {
            DrawEllipse(g,p,r.X,r.Y,r.Width,r.Height,m,eps);
        }

        public static void DrawFillEllipse(Graphics g, Brush b, float x, float y, float width, float height, Matrix m = null, double eps = 0.001)
        {
            List<PointF> ps = new List<PointF>();
            if (m == null)
                PointsForEllipse(x, y, width, height).ToList().ForEach(p => ps.AddRange(SplinePoints(p, eps)));
            else
                PointsForEllipse(x, y, width, height).ToList().ForEach(p => ps.AddRange(SplinePoints(m * p, eps)));
            g.FillClosedCurve(b, ps.ToArray());
        }

        public static void DrawFillEllipse(Graphics g, Brush b, PointF pt, SizeF s, Matrix m = null, double eps = 0.001)
        {
            DrawFillEllipse(g, b, pt.X, pt.Y, s.Width, s.Height, m, eps);
        }

        public static void DrawFillEllipse(Graphics g, Brush b, RectangleF r, Matrix m = null, double eps = 0.001)
        {
            DrawFillEllipse(g, b, r.X, r.Y, r.Width, r.Height, m, eps);
        }
    }
}
