using System.Numerics;

namespace ReplayHandler.Classes
{
    internal class HQMParse
    {
        public static Vector3 ConvertMatrixFromNetwork(byte b, uint v1, uint v2)
        {
            Vector3 vector = ConvertRotColumnFromNetwork(b, v1);
            Vector3 vector2 = ConvertRotColumnFromNetwork(b, v2);
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            float[,] array = new float[3, 3];
            array[0, 0] = -vector3.X;
            array[0, 1] = -vector.X;
            array[0, 2] = -vector2.X;
            array[1, 0] = vector3.Y;
            array[1, 1] = vector.Y;
            array[1, 2] = vector2.Y;
            array[2, 0] = vector3.Z;
            array[2, 1] = vector.Z;
            array[2, 2] = vector2.Z;
            float[,] array2 = array;
            Matrix4x4 matrix4x = new Matrix4x4();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix4x[i, j] = array2[i, j];
                }
            }
            matrix4x[3, 3] = 1f;
            return ConvertMatrixToEulerAngles(matrix4x);
        }

        public static Vector3 GetEulerAnglesFromQuaternion(Quaternion quaternion)
        {
            Matrix4x4 matrix = Matrix4x4.CreateFromQuaternion(quaternion);

            // Extract Euler angles
            float pitch, yaw, roll;
            if (Math.Abs(matrix.M13) < 0.9999999)
            {
                pitch = (float)Math.Asin(-matrix.M23);
                yaw = (float)Math.Atan2(matrix.M13, matrix.M33);
                roll = (float)Math.Atan2(matrix.M21, matrix.M22);
            }
            else
            {
                pitch = (float)Math.PI / 2 * Math.Sign(-matrix.M23);
                yaw = (float)-Math.Atan2(-matrix.M12, matrix.M11);
                roll = 0;
            }

            return new Vector3(pitch, yaw, roll);
        }

        public static Vector3 ConvertRotColumnFromNetwork(byte b, uint v)
        {
            Vector3 vector = new Vector3(1f, 0f, 0f);
            Vector3 vector2 = new Vector3(-1f, 0f, 0f);
            Vector3 vector3 = new Vector3(0f, 1f, 0f);
            Vector3 vector4 = new Vector3(0f, -1f, 0f);
            Vector3 vector5 = new Vector3(0f, 0f, 1f);
            Vector3 vector6 = new Vector3(0f, 0f, -1f);
            Vector3[,] array = new Vector3[8, 3];
            array[0, 0] = vector3;
            array[0, 1] = vector;
            array[0, 2] = vector5;
            array[1, 0] = vector3;
            array[1, 1] = vector5;
            array[1, 2] = vector2;
            array[2, 0] = vector3;
            array[2, 1] = vector6;
            array[2, 2] = vector;
            array[3, 0] = vector3;
            array[3, 1] = vector2;
            array[3, 2] = vector6;
            array[4, 0] = vector5;
            array[4, 1] = vector;
            array[4, 2] = vector4;
            array[5, 0] = vector2;
            array[5, 1] = vector5;
            array[5, 2] = vector4;
            array[6, 0] = vector;
            array[6, 1] = vector6;
            array[6, 2] = vector4;
            array[7, 0] = vector6;
            array[7, 1] = vector2;
            array[7, 2] = vector4;
            uint num = v & 7U;
            Vector3 vector7 = array[(int)num, 0];
            Vector3 vector8 = array[(int)num, 1];
            Vector3 vector9 = array[(int)num, 2];
            for (byte b2 = 3; b2 < b; b2 += 2)
            {
                uint num2 = v >> (int)b2 & 3U;
                Vector3 vector10 = vector7 + vector8;
                vector10 = Vector3.Normalize(vector10);
                Vector3 vector11 = vector8 + vector9;
                vector11 = Vector3.Normalize(vector11);
                Vector3 vector12 = vector9 + vector7;
                vector12 = Vector3.Normalize(vector12);
                switch (num2)
                {
                    case 0U:
                        vector8 = vector10;
                        vector9 = vector12;
                        break;
                    case 1U:
                        vector7 = vector10;
                        vector9 = vector11;
                        break;
                    case 2U:
                        vector7 = vector12;
                        vector8 = vector11;
                        break;
                    case 3U:
                        vector7 = vector10;
                        vector8 = vector11;
                        vector9 = vector12;
                        break;
                }
            }
            Vector3 result = vector7 + vector8 + vector9;
            return result;
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
    }
}
