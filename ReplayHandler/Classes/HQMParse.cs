using LinearAlgebra;

namespace ReplayHandler.Classes
{
    public class HQMParse
    {
        static readonly Vector3D UXP = new Vector3D(1.0, 0.0, 0.0);
        static readonly Vector3D UXN = new Vector3D(-1.0, 0.0, 0.0);
        static readonly Vector3D UYP = new Vector3D(0.0, 1.0, 0.0);
        static readonly Vector3D UYN = new Vector3D(0.0, -1.0, 0.0);
        static readonly Vector3D UZP = new Vector3D(0.0, 0.0, 1.0);
        static readonly Vector3D UZN = new Vector3D(0.0, 0.0, -1.0);
        static readonly Vector3D[][] TABLE = new Vector3D[][]
       {
            new Vector3D[] { UYP, UXP, UZP },
            new Vector3D[] { UYP, UZP, UXN },
            new Vector3D[] { UYP, UZN, UXP },
            new Vector3D[] { UYP, UXN, UZN },
            new Vector3D[] { UZP, UXP, UYN },
            new Vector3D[] { UXN, UZP, UYN },
            new Vector3D[] { UXP, UZN, UYN },
            new Vector3D[] { UZN, UXN, UYN },
       };

        public static Vector3D Cross(Vector3D left, Vector3D right)
        {
            return new Vector3D
            (
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.Y * right.Z,
                left.X * right.Y - left.Y * right.X
            );
        }

        public static Vector3D ConvertMatrixFromNetwork(byte b, uint v1, uint v2)
        {
            var r1 = ConvertRotColumnFromNetwork(b, v1);
            var r2 = ConvertRotColumnFromNetwork(b, v2);
            var r0 = Cross(r1, r2);

            var m2 = new Matrix4();
            m2[0, 0] = r0.X;
            m2[1, 0] = r0.Y;
            m2[2, 0] = r0.Z;
            m2[0, 1] = r1.X;
            m2[1, 1] = r1.Y;
            m2[2, 1] = r1.Z;
            m2[0, 2] = r2.X;
            m2[1, 2] = r2.Y;
            m2[2, 2] = r2.Z;

            return m2.ToVector();
        }

        public static Vector3D ConvertRotColumnFromNetwork(byte b, uint v)
        {
            int start = (int)(v & 7);
            var temp1 = TABLE[start][0];
            var temp2 = TABLE[start][1];
            var temp3 = TABLE[start][2];
            int pos = 3;
            while (pos < b)
            {
                int step = (int)((v >> pos) & 3);
                var c1 = (temp1 + temp2).Normalized();
                var c2 = (temp2 + temp3).Normalized();
                var c3 = (temp1 + temp3).Normalized();
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
                }
                pos += 2;
            }
            return (temp1 + temp2 + temp3).Normalized();
        }
    }
}
