using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Rage.GSRAsset.SignalProcessor
{
    public class InflectionLine
    {
        private double x1;
        private double x2;
        private double y1;
        private double y2;
        private double length;
        private InflectionLineDirection direction;

        public InflectionLine()
        {
            //super();
        }

        public InflectionLine(double x1, double y1, double x2, double y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            this.length = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            this.direction = y2 > y1 ? InflectionLineDirection.Positive : InflectionLineDirection.Negative;
        }

        public double InflectionPointX1
        {
            get
            {
                return x1;
            }
            set
            {
                x1 = value;
            }
        }

        public double InflectionPointX2
        {
            get
            {
                return x2;
            }
            set
            {
                x2 = value;
            }
        }

        public double InflectionPointY1
        {
            get
            {
                return y1;
            }
            set
            {
                y1 = value;
            }
        }

        public double InflectionPointY2
        {
            get
            {
                return y2;
            }
            set
            {
                y2 = value;
            }
        }

        public double Length
        {
            get
            {
                return length;
            }
        }

        public InflectionLineDirection Direction
        {
            get
            {
                return direction;
            }
        }

        public double GetLineLength(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public InflectionLineDirection GetDirection(double x1, double y1, double x2, double y2)
        {
            return y2.CompareTo(y1) > 0 ? InflectionLineDirection.Positive : (y1.CompareTo(y2) == 0 ? InflectionLineDirection.Neutral : InflectionLineDirection.Negative);
        }

        public List<InflectionPoint> GetInflectionPoints(List<InflectionPoint> signalCoordinatePoints)
        {
            List<InflectionPoint> inflectionPoints = new List<InflectionPoint>();
            double lastInflectionPointY = -100;
            int candidateInflectionPoint = -1;
            for (int i = 0; i < signalCoordinatePoints.Count - 1; i++)
            {
                double currentPointY = signalCoordinatePoints[i].CoordinateY;
                ExtremaType extremumType = IsPointInflection(signalCoordinatePoints, i);
                signalCoordinatePoints[i].ExtremaType = extremumType;
                if (!extremumType.Equals(ExtremaType.None))
                {
                    if (currentPointY.CompareTo(lastInflectionPointY) != 0)
                    {
                        if (i > 0 && inflectionPoints.Count > 0)
                        {
                            if ((candidateInflectionPoint + 1) == i && inflectionPoints[inflectionPoints.Count - 1].CoordinateY.Equals(lastInflectionPointY))
                            {
                                inflectionPoints.Add(signalCoordinatePoints[candidateInflectionPoint]);
                            }
                        }
                        
                        inflectionPoints.Add(signalCoordinatePoints[i]);
                    }
                    else
                    {
                        candidateInflectionPoint = i;
                    }
                    lastInflectionPointY = currentPointY;

                }
                else
                {
                    if (i > 0 && inflectionPoints.Count > 0)
                    {
                        if ((candidateInflectionPoint + 1) == i && inflectionPoints[inflectionPoints.Count - 1].CoordinateY.Equals(signalCoordinatePoints[i - 1].CoordinateY))
                        {
                            inflectionPoints.Add(signalCoordinatePoints[candidateInflectionPoint]);
                        }
                    }
                }
            }

            return inflectionPoints;
        }

        private static ExtremaType IsPointInflection(List<InflectionPoint> points, int i)
        {
            if (i == 0)
            {
                return (points[0].CoordinateY.CompareTo(points[1].CoordinateY) >= 0) ? ExtremaType.Maximum : ExtremaType.Minimum;
            }
            if (points[i].CoordinateY.CompareTo(points[i - 1].CoordinateY) >= 0 &&
                                   points[i].CoordinateY.CompareTo(points[i + 1].CoordinateY) >= 0)
            {
                return ExtremaType.Maximum;
            }
            else if (points[i].CoordinateY.CompareTo(points[i - 1].CoordinateY) <= 0 &&
                                    points[i].CoordinateY.CompareTo(points[i + 1].CoordinateY) <= 0)
            {
                return ExtremaType.Minimum;
            }

            return ExtremaType.None;
        }
    }
}
