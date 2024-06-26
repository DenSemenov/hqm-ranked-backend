
namespace LinearAlgebra {
	public struct Vector4D { 
		public double X, Y, Z, W;

		//Oh my...
		public Vector3D XYZ { get => new Vector3D(X, Y, Z); set { X = value.X; Y = value.Y; Z = value.Z; } }
		public Vector3D XZY { get => new Vector3D(X, Z, Y); set { X = value.X; Z = value.Y; Y = value.Z; } }
		public Vector3D XYW { get => new Vector3D(X, Y, W); set { X = value.X; Y = value.Y; W = value.Z; } }
		public Vector3D XWY { get => new Vector3D(X, W, Y); set { X = value.X; W = value.Y; Y = value.Z; } }
		public Vector3D XZW { get => new Vector3D(X, Z, W); set { X = value.X; Z = value.Y; W = value.Z; } }
		public Vector3D XWZ { get => new Vector3D(X, W, Z); set { X = value.X; W = value.Y; Z = value.Z; } }



		public Vector3D YXZ { get => new Vector3D(Y, X, Z); set { Y = value.X; X = value.Y; Z = value.Z; } }
		public Vector3D YZX { get => new Vector3D(Y, Z, X); set { Y = value.X; Z = value.Y; X = value.Z; } }
		public Vector3D YXW { get => new Vector3D(Y, X, W); set { Y = value.X; X = value.Y; W = value.Z; } }
		public Vector3D YWX { get => new Vector3D(Y, W, X); set { Y = value.X; W = value.Y; X = value.Z; } }
		public Vector3D YZW { get => new Vector3D(Y, Z, W); set { Y = value.X; Z = value.Y; W = value.Z; } }
		public Vector3D YWZ { get => new Vector3D(Y, W, Z); set { Y = value.X; W = value.Y; Z = value.Z; } }
		


		public Vector3D ZXY { get => new Vector3D(Z, X, Y); set { Z = value.X; X = value.Y; Y = value.Z; } }
		public Vector3D ZYX { get => new Vector3D(Z, Y, X); set { Z = value.X; Y = value.Y; X = value.Z; } }
		public Vector3D ZXW { get => new Vector3D(Z, X, W); set { Z = value.X; X = value.Y; W = value.Z; } }
		public Vector3D ZWX { get => new Vector3D(Z, W, X); set { Z = value.X; W = value.Y; X = value.Z; } }
		public Vector3D ZYW { get => new Vector3D(Z, Y, W); set { Z = value.X; Y = value.Y; W = value.Z; } }
		public Vector3D ZWY { get => new Vector3D(Z, W, Y); set { Z = value.X; W = value.Y; Y = value.Z; } }



		public Vector3D WXY { get => new Vector3D(W, X, Y); set { W = value.X; X = value.Y; Y = value.Z; } }
		public Vector3D WYX { get => new Vector3D(W, Y, X); set { W = value.X; Y = value.Y; X = value.Z; } }
		public Vector3D WXZ { get => new Vector3D(W, X, Z); set { W = value.X; X = value.Y; Z = value.Z; } }
		public Vector3D WZX { get => new Vector3D(W, Z, X); set { W = value.X; Z = value.Y; X = value.Z; } }
		public Vector3D WYZ { get => new Vector3D(W, Y, Z); set { W = value.X; Y = value.Y; Z = value.Z; } }
		public Vector3D WZY { get => new Vector3D(W, Z, Y); set { W = value.X; Z = value.Y; Y = value.Z; } }




		public Vector2D XY { get => new Vector2D(X, Y); set { X = value.X; Y = value.Y; } }
		public Vector2D XZ { get => new Vector2D(X, Z); set { X = value.X; Z = value.Y; } }
		public Vector2D XW { get => new Vector2D(X, W); set { X = value.X; W = value.Y; } }


		public Vector2D YX { get => new Vector2D(Y, Y); set { Y = value.X; X = value.Y; } }
		public Vector2D YZ { get => new Vector2D(Y, Z); set { Y = value.X; Z = value.Y; } }
		public Vector2D YW { get => new Vector2D(Y, W); set { Y = value.X; W = value.Y; } }


		public Vector2D ZX { get => new Vector2D(Z, X); set { Z = value.X; X = value.Y; } }
		public Vector2D ZY { get => new Vector2D(Z, Y); set { Z = value.X; Y = value.Y; } }
		public Vector2D ZW { get => new Vector2D(Z, W); set { Z = value.X; W = value.Y; } }


		public Vector2D WX { get => new Vector2D(W, X); set { W = value.X; X = value.Y; } }
		public Vector2D WY { get => new Vector2D(W, Y); set { W = value.X; Y = value.Y; } }
		public Vector2D WZ { get => new Vector2D(W, Z); set { W = value.X; Z = value.Y; } }



		public double Length {
			get { return length(); }
			set { 
				double mul = value / Length;
				X *= mul;
				Y *= mul;
				Z *= mul;
				W *= mul;
			}
		}

		public Vector4D(Vector4D src) : this(src.X, src.Y, src.Z, src.W) {}

		public Vector4D(double x = 0.0f, double y = 0.0f, double z = 0.0f, double w = 1.0f) {
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		public static Vector4D operator+(Vector4D a, Vector4D b) {
			return new Vector4D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}

		public static Vector4D operator-(Vector4D a, Vector4D b) {
			return new Vector4D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
		}

		public static double operator*(Vector4D a, Vector4D b) {
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
		}

		public static Vector4D operator+(Vector4D a, double b) {
			return new Vector4D(a.X + b, a.Y + b, a.Z + b, a.W + b);
		}

		public static Vector4D operator-(Vector4D a, double b) {
			return new Vector4D(a.X - b, a.Y - b, a.Z - b, a.W - b);
		}

		public static Vector4D operator*(Vector4D a, double b) {
			return new Vector4D(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}

		public static Vector4D operator*(double a, Vector4D b) {
			return b * a;
		}

		public static Vector4D operator/(Vector4D a, double b) {
			return new Vector4D(a.X / b, a.Y / b, a.Z / b, a.W / b);
		}

		public static bool operator==(Vector4D a, Vector4D b) {
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
		}

		public static bool operator!=(Vector4D a, Vector4D b) {
			return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
		}

		public static double operator^(Vector4D a, Vector4D b) { 
			return Math.Acos((a * b) / (a.Length * b.Length));
		}

		public override bool Equals(object obj) {
			if(obj is Vector4D) {
				Vector4D b = (Vector4D)obj;
				return this.X == b.X && this.Y == b.Y && this.Z == b.Z && this.W == b.W;
			}
			return false;
		}

		public override int GetHashCode() {
			return (X, Y, Z, W).GetHashCode();
		}

		public double length() { 
			return Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
		}

		public double sqrLength() { 
			return X * X + Y * Y + Z * Z + W * W;
		}

		public Vector4D normalized() {
			return this / length();
		}
		public Vector4D clone() {
			return new Vector4D(this);
		}


		public static implicit operator Vector4D(Vector3D vec3) {
			return new Vector4D(vec3.X, vec3.Y, vec3.Z);
		}

		public static implicit operator Vector4D(Vector2D vec2) {
			return new Vector4D(vec2.X, vec2.Y);
		}

		public static explicit operator Vector4D((double x, double y, double z, double w) t) {
			return new Vector4D(t.x, t.y, t.z, t.w);
		}

		public static explicit operator (double x, double y, double z, double w)(Vector4D vec4) {
			return (vec4.X, vec4.Y, vec4.Z, vec4.Z);
		}

		public static Vector4D operator*(Matrix4 a, Vector4D b) {
			Vector4D result = new Vector4D();
			result.X = a.MatrixData[0, 0] * b.X + a.MatrixData[0, 1] * b.Y + a.MatrixData[0, 2] * b.Z + a.MatrixData[0, 3] * b.W;
			result.Y = a.MatrixData[1, 0] * b.X + a.MatrixData[1, 1] * b.Y + a.MatrixData[1, 2] * b.Z + a.MatrixData[1, 3] * b.W;
			result.Z = a.MatrixData[2, 0] * b.X + a.MatrixData[2, 1] * b.Y + a.MatrixData[2, 2] * b.Z + a.MatrixData[2, 3] * b.W;
			result.W = a.MatrixData[3, 0] * b.X + a.MatrixData[3, 1] * b.Y + a.MatrixData[3, 2] * b.Z + a.MatrixData[3, 3] * b.W;
			return result;
		}

		public override string ToString() {
			return $"({X}; {Y}; {Z}; {W})"; 
		}
	}
}
