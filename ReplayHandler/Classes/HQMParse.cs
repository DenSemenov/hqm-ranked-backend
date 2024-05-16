using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using DLA = MathNet.Numerics.LinearAlgebra.Double;

namespace ReplayHandler.Classes
{
    internal class HQMParse
    {
        static readonly DLA.Vector UXP = (DLA.Vector)DLA.Vector.Build.DenseOfArray(new double[] { 1.0, 0.0, 0.0 });
        static readonly DLA.Vector UXN = (DLA.Vector)DLA.Vector.Build.DenseOfArray(new double[] { -1.0, 0.0, 0.0 });
        static readonly DLA.Vector UYP = (DLA.Vector)DLA.Vector.Build.DenseOfArray(new double[] { 0.0, 1.0, 0.0 });
        static readonly DLA.Vector UYN = (DLA.Vector)DLA.Vector.Build.DenseOfArray(new double[] { 0.0, -1.0, 0.0 });
        static readonly DLA.Vector UZP = (DLA.Vector)DLA.Vector.Build.DenseOfArray(new double[] { 0.0, 0.0, 1.0 });
        static readonly DLA.Vector UZN = (DLA.Vector)DLA.Vector.Build.DenseOfArray(new double[] { 0.0, 0.0, -1.0 });

        static DLA.Vector[][] TABLE = new DLA.Vector[][]
        {
            new DLA.Vector[] { UYP, UXP, UZP },
            new DLA.Vector[] { UYP, UZP, UXN },
            new DLA.Vector[] { UYP, UZN, UXP },
            new DLA.Vector[] { UYP, UXN, UZN },
            new DLA.Vector[] { UZP, UXP, UYN },
            new DLA.Vector[] { UXN, UZP, UYN },
            new DLA.Vector[] { UXP, UZN, UYN },
            new DLA.Vector[] { UZN, UXN, UYN },
        };
        public static DLA.Vector Cross(MathNet.Numerics.LinearAlgebra.Vector<double> left, MathNet.Numerics.LinearAlgebra.Vector<double> right)
        {
            if ((left.Count != 3 || right.Count != 3))
            {
                string message = "Vectors must have a length of 3.";
                throw new Exception(message);
            }
            DLA.Vector result = new DLA.DenseVector(3);
            result[0] = left[1] * right[2] - left[2] * right[1];
            result[1] = -left[0] * right[2] + left[2] * right[0];
            result[2] = left[0] * right[1] - left[1] * right[0];

            return result;
        }
        public static (float x, float y, float z) ConvertMatrixFromNetwork(byte b, uint v1, uint v2)
        {
            DLA.Vector r1 = ConvertRotColumnFromNetwork(b, v1);
            DLA.Vector r2 = ConvertRotColumnFromNetwork(b, v2);
            DLA.Vector r0 = Cross(r1, r2);
            var m = Matrix<double>.Build.DenseOfColumnVectors(r0, r1, r2);

            Matrix4x4 matrix4x4 = new Matrix4x4(
                (float)m[0, 0], (float)m[0, 1], (float)m[0, 2], 0.0f,
                 (float)m[1, 0], (float)m[1, 1], (float)m[1, 2], 0.0f,
                 (float)m[2, 0], (float)m[2, 1], (float)m[2, 2], 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f
                );

            var r = ConvertMatrixToEulerAngles(matrix4x4);
            return (r.X, r.Y, r.Z);
        }

        public static Vector3 ConvertMatrixToEulerAngles(Matrix4x4 rotationMatrix)
        {
            double sy = Math.Sqrt(rotationMatrix.M11 * rotationMatrix.M11 + rotationMatrix.M21 * rotationMatrix.M21);

            bool singular = sy < 1e-6;

            double x, y, z;
            if (!singular)
            {
                x = Math.Atan2(rotationMatrix.M32, rotationMatrix.M33);
                y = Math.Atan2(-rotationMatrix.M31, sy);
                z = Math.Atan2(rotationMatrix.M21, rotationMatrix.M11);
            }
            else
            {
                x = Math.Atan2(-rotationMatrix.M23, rotationMatrix.M22);
                y = Math.Atan2(-rotationMatrix.M31, sy);
                z = 0;
            }

            return new Vector3((float)x, (float)y, (float)z);
        }

        public static DLA.Vector ConvertRotColumnFromNetwork(byte b, uint v)
        {
            int start = (int)(v & 7);
            DLA.Vector temp1 = TABLE[start][0];
            DLA.Vector temp2 = TABLE[start][1];
            DLA.Vector temp3 = TABLE[start][2];
            int pos = 3;
            while (pos < b)
            {
                int step = (int)((v >> pos) & 3);
                DLA.Vector c1 = NormalizeVector((DLA.Vector)temp1.Add(temp2));
                DLA.Vector c2 = NormalizeVector((DLA.Vector)temp2.Add(temp3));
                DLA.Vector c3 = NormalizeVector((DLA.Vector)temp1.Add(temp3));
                switch (step)
                {
                    case 0:
                        temp2 = c1;
                        temp3 = c3;
                        break;
                    case 1:
                        temp1 = c1;
                        temp3 = c2;
                        break;
                    case 2:
                        temp1 = c3;
                        temp2 = c2;
                        break;
                    case 3:
                        temp1 = c1;
                        temp2 = c2;
                        temp3 = c3;
                        break;
                    default:
                        throw new Exception();
                }
                pos += 2;
            }
            return NormalizeVector((DLA.Vector)temp1.Add(temp2).Add(temp3));
        }

        public static DLA.Vector NormalizeVector(DLA.Vector v)
        {
            var vector = new Vector3((float)v[0], (float)v[1], (float)v[2]);
            var normalizedVector = Vector3.Normalize(vector);

            return (DLA.Vector)DLA.Vector.Build.DenseOfArray(new double[] { normalizedVector.X, normalizedVector.Y, normalizedVector.Z });
        }
    }
}
