using ReplayHandler.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Classes
{
    public class ReplayTickWithVectors
    {
        public uint Packet { get; set; }
        public List<ObjectWithVectors> Objects { get; set; } = new List<ObjectWithVectors>();
    }

    public class ObjectWithVectors
    {
        public int Index { get; set; }
        public ObjectType Type { get; set;}
        public Vector Pos { get; set; }
        public Vector Rot { get; set; }
        public Vector PosVelocity { get; set; }
        public Vector RotVelocity { get; set; }
        public Vector StickPos { get; set; }
        public Vector StickRot { get; set; }
        public Vector StickPosVelocity { get; set; }
        public Vector StickRotVelocity { get; set; }
        public int? TouchedBy { get; set; }
    }

    public enum ObjectType
    {
        Player,
        Puck
    }

    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector Subtract(Vector other)
        {
            return new Vector(X - other.X, Y - other.Y, Z - other.Z);
        }

        public double Magnitude()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) - Math.Pow(Z, 2));
        }

        public static Vector CalcVector(Vector? v1, Vector v2)
        {
            if (v1 != null)
            {
                var vx = v2.X - v1.X;
                var vy = v2.Y - v1.Y;
                var vz = v2.Z - v1.Z;

                return new Vector(vx, vy, vz);
            }
            else
            {
                return new Vector(0, 0, 0);
            }
        }
    }
}
