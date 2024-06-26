using System.Numerics;

namespace LinearAlgebra
{
    public struct Vector3D
    {
        public double X, Y, Z;

        public Vector2D XY { get => new Vector2D(X, Y); set { X = value.X; Y = value.Y; } }
        public Vector2D YZ { get => new Vector2D(Y, Z); set { Y = value.X; Z = value.Y; } }
        public Vector2D ZX { get => new Vector2D(Z, X); set { Z = value.X; X = value.Y; } }

        public Vector2D YX { get => new Vector2D(Y, X); set { Y = value.X; X = value.Y; } }
        public Vector2D ZY { get => new Vector2D(Z, Y); set { Z = value.X; Y = value.Y; } }
        public Vector2D XZ { get => new Vector2D(X, Z); set { X = value.X; Z = value.Y; } }

        public double Yaw
        {
            get { return X; }
            set { X = value; }
        }
        public double Pitch
        {
            get { return Y; }
            set { Y = value; }
        }
        public double Roll
        {
            get { return Z; }
            set { Z = value; }
        }

        public double Length
        {
            get { return GetLength(); }
            set
            {
                double mul = value / Length;
                X *= mul;
                Y *= mul;
                Z *= mul;
            }
        }

        public Vector3D(Vector3D src) : this(src.X, src.Y, src.Z) { }

        public Vector3D(double x = 0.0f, double y = 0.0f, double z = 0.0f)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D XAxis()
        {
            return new Vector3D(1, 0, 0);
        }

        public static Vector3D YAxis()
        {
            return new Vector3D(0, 1, 0);
        }

        public static Vector3D ZAxis()
        {
            return new Vector3D(0, 0, 1);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static double operator *(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3D operator -(Vector3D vec)
        {
            return new Vector3D(-vec.X, -vec.Y, -vec.Z);
        }

        public static Vector3D operator +(Vector3D a, double b)
        {
            return new Vector3D(a.X + b, a.Y + b, a.Z + b);
        }

        public static Vector3D operator -(Vector3D a, double b)
        {
            return new Vector3D(a.X - b, a.Y - b, a.Z - b);
        }

        public static Vector3D operator *(Vector3D a, double b)
        {
            return new Vector3D(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3D operator *(double a, Vector3D b)
        {
            return b * a;
        }

        public static Vector3D operator /(Vector3D a, double b)
        {
            return new Vector3D(a.X / b, a.Y / b, a.Z / b);
        }

        public static bool operator ==(Vector3D a, Vector3D b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3D a, Vector3D b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public static double operator ^(Vector3D a, Vector3D b)
        {
            return Math.Acos((a * b) / (a.Length * b.Length));
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3D)
            {
                Vector3D b = (Vector3D)obj;
                return this.X == b.X && this.Y == b.Y && this.Z == b.Z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (X, Y, Z).GetHashCode();
        }

        public double GetLength()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3D ComponentMul(Vector3D mul)
        {
            return new Vector3D(this.X * mul.X, this.Y * mul.Y, this.Z * mul.Z);
        }

        public double GetAngle(bool isNormalized = false, AngleMeasure measure = AngleMeasure.RADIANS)
        {
            Vector2D n = isNormalized ? this : this.Normalized();
            double result = n.Y > 0 ? Math.Acos(n.X) : -Math.Acos(n.X);
            if (measure == AngleMeasure.DEGREES)
                result = Geometry.ToDegrees(result);
            return result;
        }

        public double GetAngle(Vector3D b, AngleMeasure measure = AngleMeasure.RADIANS)
        {
            double result = Math.Acos(this * b / (this.Length * b.Length));
            if (measure == AngleMeasure.DEGREES)
                result = Geometry.ToDegrees(result);
            return result;
        }

        public double SqrLength()
        {
            return X * X + Y * Y + Z * Z;
        }

        public Vector3D Normalized()
        {
            return this / GetLength();
        }

        public double Norm()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3D Multiply(Vector3D v)
        {
            var c = new Quaternion(this);
            var b = new Quaternion(v);
            return (c * b);
        }

        public Vector3D ScaleBy(double scaleFactor)
        {
            return new Vector3D(this.X * scaleFactor, this.Y * scaleFactor, this.Z * scaleFactor);
        }

        public double DotProduct(Vector3D v)
        {
            var c = new Quaternion(this);
            var b = new Quaternion(v);

            return c.Dot(b);
        }

        public Vector3D MuliplyAxis(Vector3D v)
        {
            var m =this.ToMatrix();

            if (v.X != 0)
            {
                return new Vector3D(
                    m[0,0] * v.X,
                    m[1,0] * v.X,
                    m[2,0]* v.X
                );
            }else if (v.Y != 0)
            {
                return new Vector3D(
                    m[0, 1] * v.Y,
                    m[1, 1] * v.Y,
                    m[2, 1] * v.Y
                );
            }else
            {
                return new Vector3D(
                    m[0, 2] * v.Z,
                    m[1, 2] * v.Z,
                    m[2, 2] * v.Z
                );
            }
            
        }

        public Matrix4 ToMatrix()
        {
            var yaw = this.X ;
            var pitch = this.Y;
            var roll = this.Z;

            var m = new Matrix4();
            var r_yaw = new double[] {
           Math.Cos(yaw), -Math.Sin(yaw), 0,
           Math.Sin(yaw),  Math.Cos(yaw),   0,
            0,         0,          1,
       };

            var r_pitch = new double[] {
           Math.Cos(pitch),  0,  Math.Sin(pitch),
          0,           1,  0          ,
          -Math.Sin(pitch), 0,  Math.Cos(pitch)
       };

            var r_roll = new double[] {
           1, 0,           0 ,
          0, Math.Cos(roll),  -Math.Sin(roll),
          0, Math.Sin(roll),  Math.Cos(roll)
       };

            var temp1 = MultiplyMatrices(r_yaw, r_pitch);
            var temp2 = MultiplyMatrices(temp1, r_roll);

            m[0, 0] = temp2[8];
            m[0, 1] = temp2[6];
            m[0, 2] = temp2[2];
            m[1, 0] = temp2[7];
            m[1, 1] = temp2[4];
            m[1, 2] = temp2[1];
            m[2, 0] = temp2[6];
            m[2, 1] = temp2[3];
            m[2, 2] = temp2[0];

            return m;
        }

        public static double[] MultiplyMatrices(double[] matrix1, double[] matrix2)
        {
            double[] result = new double[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        sum += matrix1[i * 3 + k] * matrix2[k * 3 + j];
                    }
                    result[i * 3 + j] = sum;
                }
            }

            return result;
        }

        public void GetYawPitch(out double yaw, out double pitch, bool isNormalized = false, AngleMeasure measure = AngleMeasure.RADIANS)
        {
            Vector3D n = isNormalized ? this : this.Normalized();
            pitch = Math.Asin(n.Y);
            Vector2D xzDir = new Vector2D(X, Z).Normalized();
            yaw = xzDir.GetAngle();

            if (measure == AngleMeasure.DEGREES)
            {
                pitch = Geometry.ToDegrees(pitch);
                yaw = Geometry.ToDegrees(yaw);
            }

        }

        public static implicit operator Vector3D(Vector4D vec4)
        {
            return new Vector3D(vec4.X, vec4.Y, vec4.Z);
        }

        public static implicit operator Vector3D(Vector2D vec2)
        {
            return new Vector3D(vec2.X, vec2.Y);
        }

        public static implicit operator Vector3D(Quaternion q)
        {
            return new Vector3D(q.X, q.Y, q.Z);
        }

        public static explicit operator Vector3D((double x, double y, double z) t)
        {
            return new Vector3D(t.x, t.y, t.z);
        }

        public static explicit operator (double x, double y, double z)(Vector3D vec3)
        {
            return (vec3.X, vec3.Y, vec3.Z);
        }

        public override string ToString()
        {
            return $"({X}; {Y}; {Z})";
        }
    }
}
