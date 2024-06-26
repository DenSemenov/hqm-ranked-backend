

namespace LinearAlgebra {
	public class Matrix4 { 
		public double[,] MatrixData = new double[4, 4];
		public Matrix4 Transpossed { get => GetTransposed(); }

		public Matrix4(bool identity = true) {
			for(int i = 0; i < 4; i++)
				for(int j = 0; j < 4; j++)
					if(identity)
						MatrixData[i, j] = i != j ? 0.0 : 1.0;
					else
						MatrixData[i, j] = 0.0;
		}

		public static Matrix4 operator+(Matrix4 a, Matrix4 b) {
			Matrix4 result = new Matrix4();
			result.MatrixData[0, 0] = a.MatrixData[0, 0] + b.MatrixData[0, 0];
			result.MatrixData[0, 1] = a.MatrixData[0, 1] + b.MatrixData[0, 1];
			result.MatrixData[0, 2] = a.MatrixData[0, 2] + b.MatrixData[0, 2];
			result.MatrixData[0, 3] = a.MatrixData[0, 3] + b.MatrixData[0, 3];
			result.MatrixData[1, 0] = a.MatrixData[1, 0] + b.MatrixData[1, 0];
			result.MatrixData[1, 1] = a.MatrixData[1, 1] + b.MatrixData[1, 1];
			result.MatrixData[1, 2] = a.MatrixData[1, 2] + b.MatrixData[1, 2];
			result.MatrixData[1, 3] = a.MatrixData[1, 3] + b.MatrixData[1, 3];
			result.MatrixData[2, 0] = a.MatrixData[2, 0] + b.MatrixData[2, 0];
			result.MatrixData[2, 1] = a.MatrixData[2, 1] + b.MatrixData[2, 1];
			result.MatrixData[2, 2] = a.MatrixData[2, 2] + b.MatrixData[2, 2];
			result.MatrixData[2, 3] = a.MatrixData[2, 3] + b.MatrixData[2, 3];
			result.MatrixData[3, 0] = a.MatrixData[3, 0] + b.MatrixData[3, 0];
			result.MatrixData[3, 1] = a.MatrixData[3, 1] + b.MatrixData[3, 1];
			result.MatrixData[3, 2] = a.MatrixData[3, 2] + b.MatrixData[3, 2];
			result.MatrixData[3, 3] = a.MatrixData[3, 3] + b.MatrixData[3, 3];
			return result;
		}

		public static Matrix4 operator-(Matrix4 a, Matrix4 b) {
			Matrix4 result = new Matrix4();
			result.MatrixData[0, 0] = a.MatrixData[0, 0] - b.MatrixData[0, 0];
			result.MatrixData[0, 1] = a.MatrixData[0, 1] - b.MatrixData[0, 1];
			result.MatrixData[0, 2] = a.MatrixData[0, 2] - b.MatrixData[0, 2];
			result.MatrixData[0, 3] = a.MatrixData[0, 3] - b.MatrixData[0, 3];
			result.MatrixData[1, 0] = a.MatrixData[1, 0] - b.MatrixData[1, 0];
			result.MatrixData[1, 1] = a.MatrixData[1, 1] - b.MatrixData[1, 1];
			result.MatrixData[1, 2] = a.MatrixData[1, 2] - b.MatrixData[1, 2];
			result.MatrixData[1, 3] = a.MatrixData[1, 3] - b.MatrixData[1, 3];
			result.MatrixData[2, 0] = a.MatrixData[2, 0] - b.MatrixData[2, 0];
			result.MatrixData[2, 1] = a.MatrixData[2, 1] - b.MatrixData[2, 1];
			result.MatrixData[2, 2] = a.MatrixData[2, 2] - b.MatrixData[2, 2];
			result.MatrixData[2, 3] = a.MatrixData[2, 3] - b.MatrixData[2, 3];
			result.MatrixData[3, 0] = a.MatrixData[3, 0] - b.MatrixData[3, 0];
			result.MatrixData[3, 1] = a.MatrixData[3, 1] - b.MatrixData[3, 1];
			result.MatrixData[3, 2] = a.MatrixData[3, 2] - b.MatrixData[3, 2];
			result.MatrixData[3, 3] = a.MatrixData[3, 3] - b.MatrixData[3, 3];
			return result;
		}

		public static Matrix4 operator*(Matrix4 a, Matrix4 b) {
			Matrix4 result = new Matrix4();
			result.MatrixData[0, 0] = a.MatrixData[0, 0] * b.MatrixData[0, 0] + a.MatrixData[0, 1] * b.MatrixData[1, 0] + a.MatrixData[0, 2] * b.MatrixData[2, 0] + a.MatrixData[0, 3] * b.MatrixData[3, 0];
			result.MatrixData[0, 1] = a.MatrixData[0, 0] * b.MatrixData[0, 1] + a.MatrixData[0, 1] * b.MatrixData[1, 1] + a.MatrixData[0, 2] * b.MatrixData[2, 1] + a.MatrixData[0, 3] * b.MatrixData[3, 1];
			result.MatrixData[0, 2] = a.MatrixData[0, 0] * b.MatrixData[0, 2] + a.MatrixData[0, 1] * b.MatrixData[1, 2] + a.MatrixData[0, 2] * b.MatrixData[2, 2] + a.MatrixData[0, 3] * b.MatrixData[3, 2];
			result.MatrixData[0, 3] = a.MatrixData[0, 0] * b.MatrixData[0, 3] + a.MatrixData[0, 1] * b.MatrixData[1, 3] + a.MatrixData[0, 2] * b.MatrixData[2, 3] + a.MatrixData[0, 3] * b.MatrixData[3, 3];
				
			result.MatrixData[1, 0] = a.MatrixData[1, 0] * b.MatrixData[0, 0] + a.MatrixData[1, 1] * b.MatrixData[1, 0] + a.MatrixData[1, 2] * b.MatrixData[2, 0] + a.MatrixData[1, 3] * b.MatrixData[3, 0];
			result.MatrixData[1, 1] = a.MatrixData[1, 0] * b.MatrixData[0, 1] + a.MatrixData[1, 1] * b.MatrixData[1, 1] + a.MatrixData[1, 2] * b.MatrixData[2, 1] + a.MatrixData[1, 3] * b.MatrixData[3, 1];
			result.MatrixData[1, 2] = a.MatrixData[1, 0] * b.MatrixData[0, 2] + a.MatrixData[1, 1] * b.MatrixData[1, 2] + a.MatrixData[1, 2] * b.MatrixData[2, 2] + a.MatrixData[1, 3] * b.MatrixData[3, 2];
			result.MatrixData[1, 3] = a.MatrixData[1, 0] * b.MatrixData[0, 3] + a.MatrixData[1, 1] * b.MatrixData[1, 3] + a.MatrixData[1, 2] * b.MatrixData[2, 3] + a.MatrixData[1, 3] * b.MatrixData[3, 3];

			result.MatrixData[2, 0] = a.MatrixData[2, 0] * b.MatrixData[0, 0] + a.MatrixData[2, 1] * b.MatrixData[1, 0] + a.MatrixData[2, 2] * b.MatrixData[2, 0] + a.MatrixData[2, 3] * b.MatrixData[3, 0];
			result.MatrixData[2, 1] = a.MatrixData[2, 0] * b.MatrixData[0, 1] + a.MatrixData[2, 1] * b.MatrixData[1, 1] + a.MatrixData[2, 2] * b.MatrixData[2, 1] + a.MatrixData[2, 3] * b.MatrixData[3, 1];
			result.MatrixData[2, 2] = a.MatrixData[2, 0] * b.MatrixData[0, 2] + a.MatrixData[2, 1] * b.MatrixData[1, 2] + a.MatrixData[2, 2] * b.MatrixData[2, 2] + a.MatrixData[2, 3] * b.MatrixData[3, 2];
			result.MatrixData[2, 3] = a.MatrixData[2, 0] * b.MatrixData[0, 3] + a.MatrixData[2, 1] * b.MatrixData[1, 3] + a.MatrixData[2, 2] * b.MatrixData[2, 3] + a.MatrixData[2, 3] * b.MatrixData[3, 3];

			result.MatrixData[3, 0] = a.MatrixData[3, 0] * b.MatrixData[0, 0] + a.MatrixData[3, 1] * b.MatrixData[1, 0] + a.MatrixData[3, 2] * b.MatrixData[2, 0] + a.MatrixData[3, 3] * b.MatrixData[3, 0];
			result.MatrixData[3, 1] = a.MatrixData[3, 0] * b.MatrixData[0, 1] + a.MatrixData[3, 1] * b.MatrixData[1, 1] + a.MatrixData[3, 2] * b.MatrixData[2, 1] + a.MatrixData[3, 3] * b.MatrixData[3, 1];
			result.MatrixData[3, 2] = a.MatrixData[3, 0] * b.MatrixData[0, 2] + a.MatrixData[3, 1] * b.MatrixData[1, 2] + a.MatrixData[3, 2] * b.MatrixData[2, 2] + a.MatrixData[3, 3] * b.MatrixData[3, 2];
			result.MatrixData[3, 3] = a.MatrixData[3, 0] * b.MatrixData[0, 3] + a.MatrixData[3, 1] * b.MatrixData[1, 3] + a.MatrixData[3, 2] * b.MatrixData[2, 3] + a.MatrixData[3, 3] * b.MatrixData[3, 3];
			return result;
		}

		public static Matrix4 operator*(Matrix4 a, double b) {
			Matrix4 result = new Matrix4();
			result.MatrixData[0, 0] = a.MatrixData[0, 0] * b;
			result.MatrixData[0, 1] = a.MatrixData[0, 1] * b;
			result.MatrixData[0, 2] = a.MatrixData[0, 2] * b;
			result.MatrixData[0, 3] = a.MatrixData[0, 3] * b;
			result.MatrixData[1, 0] = a.MatrixData[1, 0] * b;
			result.MatrixData[1, 1] = a.MatrixData[1, 1] * b;
			result.MatrixData[1, 2] = a.MatrixData[1, 2] * b;
			result.MatrixData[1, 3] = a.MatrixData[1, 3] * b;
			result.MatrixData[2, 0] = a.MatrixData[2, 0] * b;
			result.MatrixData[2, 1] = a.MatrixData[2, 1] * b;
			result.MatrixData[2, 2] = a.MatrixData[2, 2] * b;
			result.MatrixData[2, 3] = a.MatrixData[2, 3] * b;
			result.MatrixData[3, 0] = a.MatrixData[3, 0] * b;
			result.MatrixData[3, 1] = a.MatrixData[3, 1] * b;
			result.MatrixData[3, 2] = a.MatrixData[3, 2] * b;
			result.MatrixData[3, 3] = a.MatrixData[3, 3] * b;
			return result;
		}

		public static Matrix4 operator/(Matrix4 a, double b) {
			Matrix4 result = new Matrix4();
			result.MatrixData[0, 0] = a.MatrixData[0, 0] / b;
			result.MatrixData[0, 1] = a.MatrixData[0, 1] / b;
			result.MatrixData[0, 2] = a.MatrixData[0, 2] / b;
			result.MatrixData[0, 3] = a.MatrixData[0, 3] / b;
			result.MatrixData[1, 0] = a.MatrixData[1, 0] / b;
			result.MatrixData[1, 1] = a.MatrixData[1, 1] / b;
			result.MatrixData[1, 2] = a.MatrixData[1, 2] / b;
			result.MatrixData[1, 3] = a.MatrixData[1, 3] / b;
			result.MatrixData[2, 0] = a.MatrixData[2, 0] / b;
			result.MatrixData[2, 1] = a.MatrixData[2, 1] / b;
			result.MatrixData[2, 2] = a.MatrixData[2, 2] / b;
			result.MatrixData[2, 3] = a.MatrixData[2, 3] / b;
			result.MatrixData[3, 0] = a.MatrixData[3, 0] / b;
			result.MatrixData[3, 1] = a.MatrixData[3, 1] / b;
			result.MatrixData[3, 2] = a.MatrixData[3, 2] / b;
			result.MatrixData[3, 3] = a.MatrixData[3, 3] / b;
			return result;
		}

		public Matrix4 GetTransposed() { 
			Matrix4 result = new Matrix4(false);
			result.MatrixData[0, 0] = MatrixData[0, 0];
			result.MatrixData[0, 1] = MatrixData[1, 0];
			result.MatrixData[0, 2] = MatrixData[2, 0];
			result.MatrixData[0, 3] = MatrixData[3, 0];

			result.MatrixData[1, 0] = MatrixData[0, 1];
			result.MatrixData[1, 1] = MatrixData[1, 1];
			result.MatrixData[1, 2] = MatrixData[2, 1];
			result.MatrixData[1, 3] = MatrixData[3, 1];

			result.MatrixData[2, 0] = MatrixData[0, 2];
			result.MatrixData[2, 1] = MatrixData[1, 2];
			result.MatrixData[2, 2] = MatrixData[2, 2];
			result.MatrixData[2, 3] = MatrixData[3, 2];

			result.MatrixData[3, 0] = MatrixData[0, 3];
			result.MatrixData[3, 1] = MatrixData[1, 3];
			result.MatrixData[3, 2] = MatrixData[2, 3];
			result.MatrixData[3, 3] = MatrixData[3, 3];
			return result;
		}
        public Vector3D Column(int n)
        {
			return new Vector3D(this[n, 0], this[n, 1], this[n, 2]);
        }

        public Vector3D ToVector()
        {
            var sy = Math.Sqrt(this[0, 0] * this[0, 0] + this[1, 0] * this[1, 0]);
            var singular = sy < 1e-6;

            double x, y, z;

            if (!singular)
            {
                x = Math.Atan2(this[2, 1], this[2, 2]);
                y = Math.Atan2(-this[2, 0], sy);
                z = Math.Atan2(this[1, 0], this[0, 0]);
            }
            else
            {
                x = Math.Atan2(-this[1, 2], this[1, 1]);
                y = Math.Atan2(-this[2, 0], sy);
                z = 0;
            }

            return new Vector3D(x, y, z);
        }

        public static Matrix4 Identity { get => new Matrix4();}

		public float[] ToFloatArray(float[] arr = null, bool transposed = false) {
			if(arr == null)
				arr = new float[16];
			if(transposed) { 
				arr[00] = (float)MatrixData[0, 0];
				arr[01] = (float)MatrixData[1, 0];
				arr[02] = (float)MatrixData[2, 0];
				arr[03] = (float)MatrixData[3, 0];
				arr[04] = (float)MatrixData[0, 1];
				arr[05] = (float)MatrixData[1, 1];
				arr[06] = (float)MatrixData[2, 1];
				arr[07] = (float)MatrixData[3, 1];
				arr[08] = (float)MatrixData[0, 2];
				arr[09] = (float)MatrixData[1, 2];
				arr[10] = (float)MatrixData[2, 2];
				arr[11] = (float)MatrixData[3, 2];
				arr[12] = (float)MatrixData[0, 3];
				arr[13] = (float)MatrixData[1, 3];
				arr[14] = (float)MatrixData[2, 3];
				arr[15] = (float)MatrixData[3, 3];
			} else {
				arr[00] = (float)MatrixData[0, 0];
				arr[01] = (float)MatrixData[0, 1];
				arr[02] = (float)MatrixData[0, 2];
				arr[03] = (float)MatrixData[0, 3];
				arr[04] = (float)MatrixData[1, 0];
				arr[05] = (float)MatrixData[1, 1];
				arr[06] = (float)MatrixData[1, 2];
				arr[07] = (float)MatrixData[1, 3];
				arr[08] = (float)MatrixData[2, 0];
				arr[09] = (float)MatrixData[2, 1];
				arr[10] = (float)MatrixData[2, 2];
				arr[11] = (float)MatrixData[2, 3];
				arr[12] = (float)MatrixData[3, 0];
				arr[13] = (float)MatrixData[3, 1];
				arr[14] = (float)MatrixData[3, 2];
				arr[15] = (float)MatrixData[3, 3];
			}
			return arr;
		}

		public static Matrix4 RotationX(double angle, AngleMeasure measure = AngleMeasure.RADIANS) { 
			if(measure == AngleMeasure.DEGREES)
				angle = Geometry.ToRadians(angle);
			Matrix4 result = new Matrix4(false);
			result.MatrixData[0, 0] = 1.0;
			double cosA = Math.Cos(angle);
			double sinA = Math.Sin(angle);
			result.MatrixData[0, 0] = 1.0;
			result.MatrixData[1, 1] =  cosA;
			result.MatrixData[1, 2] = -sinA;
			result.MatrixData[2, 1] =  sinA;
			result.MatrixData[2, 2] =  cosA;
			result.MatrixData[3, 3] = 1.0;
			return result;
		}

		public static Matrix4 RotationY(double angle, AngleMeasure measure = AngleMeasure.RADIANS) {
			if(measure == AngleMeasure.DEGREES)
				angle = Geometry.ToRadians(angle);
			Matrix4 result = new Matrix4(false);
			result.MatrixData[0, 0] = 1.0;
			double cosA = Math.Cos(angle);
			double sinA = Math.Sin(angle);
			result.MatrixData[1, 1] = 1.0;
			result.MatrixData[0, 0] =  cosA;
			result.MatrixData[0, 2] =  sinA;
			result.MatrixData[2, 0] =  -sinA;
			result.MatrixData[2, 2] =  cosA;
			result.MatrixData[3, 3] = 1.0;
			return result;
		}

		public static Matrix4 RotationZ(double angle, AngleMeasure measure = AngleMeasure.RADIANS) {
			if(measure == AngleMeasure.DEGREES)
				angle = Geometry.ToRadians(angle);
			Matrix4 result = new Matrix4(false);
			double cosA = Math.Cos(angle);
			double sinA = Math.Sin(angle);
			
			result.MatrixData[0, 0] =  cosA;
			result.MatrixData[0, 1] = -sinA;
			result.MatrixData[1, 0] =  sinA;
			result.MatrixData[1, 1] =  cosA;
			result.MatrixData[2, 2] = 1.0;
			result.MatrixData[3, 3] = 1.0;
			return result;
		}

		public static Matrix4 Scale(double scaleX, double scaleY, double scaleZ) {
			Matrix4 result = new Matrix4(true);
			result.MatrixData[0, 0] = scaleX;
			result.MatrixData[1, 1] = scaleY;
			result.MatrixData[2, 2] = scaleZ;
			return result;
		}

		public static Matrix4 Translation(double x, double y, double z) { 
			Matrix4 result = Matrix4.Identity;
			result.MatrixData[0, 3] = x;
			result.MatrixData[1, 3] = y;
			result.MatrixData[2, 3] = z;
			return result;
		} 

		public static Matrix4 Projection(double fov, double aspect, double nearDist, double farDist) {
			
			Matrix4 result = new Matrix4();
			double frustumDepth = farDist - nearDist;
			double oneOverDepth = 1 / frustumDepth;

			result.MatrixData[1, 1] = 1 / Math.Tan(0.5f * fov);
			result.MatrixData[0, 0] = -result.MatrixData[1, 1] / aspect;
			result.MatrixData[2, 2] = farDist * oneOverDepth;
			result.MatrixData[2, 3] = (-farDist * nearDist) * oneOverDepth;
			result.MatrixData[3, 2] = 1.0;
			result.MatrixData[3, 3] = 0;
			return result;
		}

		public double this[int i, int j] {
			get { return MatrixData[i, j]; }
			set { MatrixData[i, j] = value; }
		}

		public override string ToString() {
			string result = "";
			for(int i = 0; i < 4; i++) {
				for(int j = 0; j < 4; j++)
					result += MatrixData[i, j] + " ";
				result += "\n";
			}
			return result;
		}

	}
}
