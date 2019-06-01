//namespace HexGrid
//{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    [System.Serializable]
    public struct Hex
    {
        public int q, r, s;

        public Hex(int q, int r, int s)
        {
            Debug.Assert(q + r + s == 0);

            this.q = q;
            this.r = r;
            this.s = s;
        }

        public Hex(int q, int r)
        {
            this.q = q;
            this.r = r;
            s = -q - r;
        }

        public void SetThirdAxis(ShapeGenerators.Axis axis1, ShapeGenerators.Axis axis2)
        {
            switch (axis1)
            {
                case ShapeGenerators.Axis.q:
                    switch (axis2)
                    {
                        case ShapeGenerators.Axis.q:
                            throw new System.ArgumentException("Both axis cannot be the same");
                        case ShapeGenerators.Axis.r:
                            s = -q - r;
                            return;
                        case ShapeGenerators.Axis.s:
                            r = -q - s;
                            return;
                        default:
                            throw new System.ArgumentException("Invalid axis.");
                    }
                case ShapeGenerators.Axis.r:
                    switch (axis2)
                    {
                        case ShapeGenerators.Axis.q:
                            s = -q - r;
                            return;
                        case ShapeGenerators.Axis.r:
                            throw new System.ArgumentException("Both axis cannot be the same");
                        case ShapeGenerators.Axis.s:
                            q = -r - s;
                            return;
                        default:
                            throw new System.ArgumentException("Invalid axis.");
                    }
                case ShapeGenerators.Axis.s:
                    switch (axis2)
                    {
                        case ShapeGenerators.Axis.q:
                            r = -q - s;
                            return;
                        case ShapeGenerators.Axis.r:
                            q = -r - s;
                            return;
                        case ShapeGenerators.Axis.s:
                            throw new System.ArgumentException("Both axis cannot be the same");
                        default:
                            throw new System.ArgumentException("Invalid axis.");
                    }
                default:
                    throw new System.ArgumentException("Invalid axis.");
            }
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1:
                        return q;
                    case 2:
                        return r;
                    case 3:
                        return s;
                    default:
                        throw new System.IndexOutOfRangeException("Hex coord index must be 0, 1, or 2.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        q = value;
                        break;
                    case 1:
                        r = value;
                        break;
                    case 2:
                        s = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException("Hex coord index must be 0, 1, or 2.");
                }
            }
        }

        public static bool operator ==(Hex a, Hex b)
        {
            return a.q == b.q && a.r == b.r && a.s == b.s;
        }

        public static bool operator !=(Hex a, Hex b)
        {
            return !(a == b);
        }

        public static Hex operator +(Hex a, Hex b)
        {
            return new Hex(a.q + b.q, a.r + b.r, a.s + b.s);
        }

        public static Hex operator -(Hex a, Hex b)
        {
            return new Hex(a.q - b.q, a.r - b.r, a.s - b.s);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Hex ({0}, {1}, {2})", q, r, s);
        }

        public static int Length(Hex hex)
        {
            return (int)((Mathf.Abs(hex.q) + Mathf.Abs(hex.r) + Mathf.Abs(hex.s)) / 2);
        }

        public static int Distance(Hex a, Hex b)
        {
            return Length(a - b);
        }

        readonly static Hex[] directions = {new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1),
                                        new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1) };

        public static Hex Direction(int direction)
        {
            //if (direction < 0 || direction > 5) throw new System.ArgumentOutOfRangeException("direction", "Direction must be between 0 and 5.");
            direction = direction % 6;
            return directions[direction];
        }

        public static Hex Direction(FlatDirection dir)
        {
            return Direction((int)dir);
        }

        public static Hex Direction(PointyDirection dir)
        {
            return Direction((int)dir);
        }

        public static Hex Neighbor(Hex hex, int direction)
        {
            return hex + Direction(direction);
        }

        public static Hex Neighbor(Hex hex, FlatDirection dir)
        {
            return Neighbor(hex, (int)dir);
        }

        public static Hex Neighbor(Hex hex, PointyDirection dir)
        {
            return Neighbor(hex, (int)dir);
        }

        public static IEnumerable<Hex> Neighbors(Hex hex)
        {
            for (int i = 0; i < 6; i++)
            {
                yield return Neighbor(hex, i);
            }
        }

        public static Hex Round(Vector3 fractionalHex)
        {
            int q = (int)(Mathf.Round(fractionalHex.x));
            int r = (int)(Mathf.Round(fractionalHex.y));
            int s = (int)(Mathf.Round(fractionalHex.z));
            float q_diff = Mathf.Abs(q - fractionalHex.x);
            float r_diff = Mathf.Abs(r - fractionalHex.y);
            float s_diff = Mathf.Abs(s - fractionalHex.z);
            if (q_diff > r_diff && q_diff > s_diff)
            {
                q = -r - s;
            }
            else if (r_diff > s_diff)
            {
                r = -q - s;
            }
            else
            {
                s = -q - r;
            }
            return new Hex(q, r, s);
        }

        static Vector3 hex_lerp(Hex a, Hex b, float t)
        {
            return new Vector3(a.q + (b.q - a.q) * t,
                                 a.r + (b.r - a.r) * t,
                                 a.s + (b.s - a.s) * t);
        }

        public static IEnumerable<Hex> DrawLine(Hex a, Hex b)
        {
            int N = Distance(a, b);
            float step = 1f / Mathf.Max(N, 1);

            for (int i = 0; i <= N; i++)
            {
                yield return Round(hex_lerp(a, b, step * i));
            }
        }
    }

    public enum PointyDirection
    {
        NorthEast = 5,
        East = 0,
        SouthEast = 1,
        SouthWest = 2,
        West = 3,
        NorthWest = 4,
    }

    public enum FlatDirection
    {
        NorthEast = 0,
        SouthEast = 1,
        South = 2,
        SouthWest = 3,
        NorthWest = 4,
        North = 5,
    }

    public class Orientation
    {
        public readonly float f0, f1, f2, f3;
        public readonly float b0, b1, b2, b3;
        public readonly float startAngle;

        private Orientation(float f0_, float f1_, float f2_, float f3_,
                            float b0_, float b1_, float b2_, float b3_,
                            float startAngle_)
        {
            f0 = f0_;
            f1 = f1_;
            f2 = f2_;
            f3 = f3_;
            b0 = b0_;
            b1 = b1_;
            b2 = b2_;
            b3 = b3_;
            startAngle = startAngle_;
        }

        public static readonly Orientation pointy = new Orientation(Mathf.Sqrt(3), Mathf.Sqrt(3) / 2, 0, 3f / 2f,
                                                                    Mathf.Sqrt(3) / 3, -1 / 3f, 0, 2f / 3f,
                                                                    .5f);

        public static readonly Orientation flat = new Orientation(3f / 2f, 0, Mathf.Sqrt(3) / 2, Mathf.Sqrt(3),
                                                                  2f / 3f, 0, -1f / 3f, Mathf.Sqrt(3) / 3f,
                                                                  0);
    }

    public class Layout
    {
        public readonly Orientation orientation;
        public readonly float size;
        public readonly Vector2 origin;

        private Vector2[] cornerCoords = new Vector2[6];

        public Layout(Orientation orientation, float size, Vector2 origin)
        {
            this.orientation = orientation;
            this.size = size;
            this.origin = origin;


            for (int i = 0; i < 6; i++)
            {
                float angle = 2 * Mathf.PI * (i + orientation.startAngle) / 6;

                var x = Mathf.Cos(angle);
                var y = Mathf.Sin(angle);

                cornerCoords[i] = new Vector2(size * x, size * y);
            }
        }

        public Vector2 HexToWorld(Hex h)
        {
            var M = orientation;

            float x = (M.f0 * h.q + M.f1 * h.r) * this.size;
            float y = (M.f2 * h.q + M.f3 * h.r) * this.size;
            return new Vector2(x + this.origin.x, y + this.origin.y);
        }

        public Vector3 WorldToHex(Vector2 p)
        {
            var M = this.orientation;
            Vector2 pt = new Vector2((p.x - this.origin.x) / this.size,
                             (p.y - this.origin.y) / this.size);
            float q = M.b0 * pt.x + M.b1 * pt.y;
            float r = M.b2 * pt.x + M.b3 * pt.y;
            return new Vector3(q, r, -q - r);
        }

        public Hex WorldToRoundHex(Vector2 p)
        {
            return Hex.Round(WorldToHex(p));
        }

        public Vector2 CornerOffset(int i)
        {
            return cornerCoords[i];
        }

        public Vector2[] HexCorners(Hex h)
        {
            var corners = new Vector2[6];
            var center = HexToWorld(h);

            for (int i = 0; i < cornerCoords.Length; i++)
            {
                corners[i] = cornerCoords[i] + center;
            }

            return corners;
        }

    }

    public class Grid
    {

        public readonly Layout layout;
        public readonly int tilesX, tilesY;

        private Vector2[] uvCorners = new Vector2[6];


        public Grid(Layout layout, int tilesX = 1, int tilesY = 1)
        {
            this.layout = layout;
            this.tilesX = tilesX;
            this.tilesY = tilesY;

            for (int i = 0; i < 6; i++)
            {
                //remove the size of the hexagon from the corner coordinates (as if it was size 1)
                //then convert from -1 to 1 into 0 to 1, then divide by number tiles so it's one corner tile
                //can add offsets later for other tiles
                uvCorners[i] = new Vector2(((layout.CornerOffset(i).x / layout.size) + 1) / 2 / tilesX, ((layout.CornerOffset(i).y / layout.size) + 1) / 2 / tilesY);
            }
        }

        private IEnumerable<Vector2> tileOffsets()
        {
            //outer loop on y so that tiles 'read' left-right first, Then bottom-top
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    yield return new Vector2((float)x / tilesX, (float)y / tilesY);
                }
            }
        }

        public Mesh MakeMesh(IEnumerable<Hex> hexes)
        {
            return MakeMesh(hexes, (h) => 0);
        }

        public Mesh MakeMesh(IEnumerable<Hex> hexes, System.Func<Hex, int> tileSelector, System.Func<Hex, Color> colorSelector = null)
        {
            //verts and uvs, easier to do at same time
            var verts = new List<Vector3>();
            var uvs = new List<Vector2>();

            var col = new List<Color>();

            if (colorSelector == null)
            {
                colorSelector = (h) => Color.white;
            }

            int hexCount = 0;
            var tiles = tileOffsets().ToList();

            foreach (var h in hexes)
            {
                var corners = layout.HexCorners(h);

                for (int j = 0; j < corners.Length; j++)
                {
                    verts.Add(corners[j]);
                    uvs.Add(uvCorners[j] + tiles[tileSelector(h)]);
                    col.Add(colorSelector(h));
                }
                hexCount++;
            }

            //tris
            int[] tris = new int[hexCount * 3 * 4];
            var t = 0;

            for (int i = 0; i < hexCount; i++)
            {
                int corner0 = i * 6;

                tris[t] = corner0 + 1;
                tris[t + 1] = corner0;
                tris[t + 2] = corner0 + 5;

                tris[t + 3] = corner0 + 2;
                tris[t + 4] = corner0 + 1;
                tris[t + 5] = corner0 + 5;

                tris[t + 6] = corner0 + 2;
                tris[t + 7] = corner0 + 5;
                tris[t + 8] = corner0 + 4;

                tris[t + 9] = corner0 + 2;
                tris[t + 10] = corner0 + 4;
                tris[t + 11] = corner0 + 3;

                t += 12;
            }

            // mesh

            Mesh m = new Mesh();
            m.vertices = verts.ToArray();
            m.triangles = tris;
            m.uv = uvs.ToArray();
            m.colors = col.ToArray();
            m.RecalculateNormals();

            return m;
        }

        public Mesh MakeMesh(IDictionary<Hex, int> map)
        {
            var hexList = map.Keys.ToList();

            return MakeMesh(hexList, (h) => map[h]);
        }
    }

    [System.Serializable]
    public struct OffsetCoord
    {
        public enum Type
        {
            Pointy = 1,
            Flat = 0
        }
        public enum Which
        {
            Even = 1,
            Odd = -1,
        }

        public int col, row;

        public OffsetCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        public OffsetCoord(Hex h, Type t, Which w)
        {
            switch (t)
            {
                case Type.Pointy:
                    col = h.q + (h.r + (int)w * (h.r & 1)) / 2; //not sure why they cast here?
                    row = h.r;
                    return;
                case Type.Flat:
                    col = h.q;
                    row = h.r + (h.q + (int)w * (h.q & 1)) / 2;
                    return;
                default:
                    throw new System.ArgumentException("Invalid grid type.");
            }
        }

        public Hex ToHex(Type t, Which w)
        {
            switch (t)
            {
                case Type.Pointy:
                    int q = col - (row + (int)w * (row & 1)) / 2;
                    int r = row;
                    return new Hex(q, r);
                case Type.Flat:
                    q = col;
                    r = row - (col + (int)w * (col & 1)) / 2;
                    return new Hex(q, r);
                default:
                    throw new System.ArgumentException("Invalid grid type.");
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", col, row);
        }
    }

    public static class ShapeGenerators
    {
        public enum Axis
        {
            q = 0,
            r = 1,
            s = 2
        }

        public static IEnumerable<Hex> Parallelogram(Axis axisX, Axis axisY, int x1, int x2, int y1, int y2)
        {
            var ax = (int)axisX;
            var ay = (int)axisY;

            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    var h = new Hex();
                    h[ax] = x;
                    h[ay] = y;
                    h.SetThirdAxis(axisX, axisY);
                    yield return h;
                }
            }
        }

        public static IEnumerable<Hex> Triangle(int size, bool vertical)
        {
            if (vertical)
            {
                for (int q = 0; q <= size; q++)
                {
                    for (int r = 0; r <= size - q; r++)
                    {
                        yield return new Hex(q, r);
                    }
                }
            }
            else
            {
                for (int q = 0; q <= size; q++)
                {
                    for (int r = size - q; r <= size; r++)
                    {
                        yield return new Hex(q, r);
                    }
                }
            }
        }

        public static IEnumerable<Hex> Hexagon(int radius)
        {
            for (int q = -radius; q <= radius; q++)
            {
                int r1 = Mathf.Max(-radius, -q - radius);
                int r2 = Mathf.Min(radius, -q + radius);
                for (int r = r1; r <= r2; r++)
                {
                    yield return new Hex(q, r, -q - r);
                }
            }
        }

        public static IEnumerable<Hex> Rectangle(Axis axisX, Axis axisY, int height, int width)
        {
            var ax = (int)axisX;
            var ay = (int)axisY;

            for (int y = 0; y < height; y++)
            {
                int y_offset = y >> 1;
                for (int x = -y_offset; x < width - y_offset; x++)
                {
                    var h = new Hex();
                    h[ax] = x;
                    h[ay] = y;
                    h.SetThirdAxis(axisX, axisY);
                    yield return h;
                }
            }
        }
    }
//}