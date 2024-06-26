

namespace LinearAlgebra {
	public struct Vector2D { 
		public double X, Y;

		public double Tan { get => Y / X; }

		public double Length {
			get { return GetLength(); }
			set { 
				double mul = value / Length;
				X *= mul;
				Y *= mul;
			}
		}

		public Vector2D(Vector2D src) : this(src.X, src.Y) {}

		public Vector2D(double x = 0.0f, double y = 0.0f) {
			this.X = x;
			this.Y = y;
		}

		public static Vector2D operator+(Vector2D a, Vector2D b) {
			return new Vector2D(a.X + b.X, a.Y + b.Y);
		}

		public static Vector2D operator-(Vector2D a, Vector2D b) {
			return new Vector2D(a.X - b.X, a.Y - b.Y);
		}

		public static double operator*(Vector2D a, Vector2D b) {
			return a.X * b.X + a.Y * b.Y;
		}

		public static Vector2D operator-(Vector2D vec) {
			return new Vector2D(-vec.X, -vec.Y);
		}

		public static Vector2D operator+(Vector2D a, double b) {
			return new Vector2D(a.X + b, a.Y + b);
		}

		public static Vector2D operator-(Vector2D a, double b) {
			return new Vector2D(a.X - b, a.Y - b);
		}

		public static Vector2D operator*(Vector2D a, double b) {
			return new Vector2D(a.X * b, a.Y * b);
		}

		public static Vector2D operator*(double a, Vector2D b) {
			return b * a;
		}

		public static Vector2D operator/(Vector2D a, double b) {
			return new Vector2D(a.X / b, a.Y / b);
		}

		public static bool operator==(Vector2D a, Vector2D b) {
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator!=(Vector2D a, Vector2D b) {
			return a.X != b.X || a.Y != b.Y;
		}

		public static double operator^(Vector2D a, Vector2D b) { 
			return a.GetAngle(b);
		}

		public override bool Equals(object obj) {
			if(obj is Vector2D) {
				Vector2D b = (Vector2D)obj;
				return this.X == b.X && this.Y == b.Y;
			}
			return false;
		}

		public override int GetHashCode() {
			return (X, Y).GetHashCode();
		}

		public double GetLength() { 
			return Math.Sqrt(X * X + Y * Y);
		}

		public double SqrLength() { 
			return X * X + Y * Y;
		}

		public Vector2D Normalized() {
			return this / GetLength();
		}

		public double GetAngle(bool isNormalized = false, AngleMeasure measure = AngleMeasure.RADIANS) {
			Vector2D n = isNormalized ? this : this.Normalized();
			double result = n.Y > 0 ? Math.Acos(n.X) : -Math.Acos(n.X);
			if(measure == AngleMeasure.DEGREES)
				result = Geometry.ToDegrees(result);
			return result;
		}

		public double GetAngle(Vector2D b, AngleMeasure measure = AngleMeasure.RADIANS) {
			double result = Math.Acos(this * b / (this.GetLength() * b.GetLength()));
			if(new Vector2D(-Y, X) * b < 0)
				result = -result;
			if(measure == AngleMeasure.DEGREES)
				result = Geometry.ToDegrees(result);
			return result;
		}

		public Vector2D Clone() {
			return new Vector2D(this);
		}

		public static implicit operator Vector2D(Vector4D vec4) {
			return new Vector2D(vec4.X, vec4.Y);
		}

		public static implicit operator Vector2D(Vector3D vec3) {
			return new Vector2D(vec3.X, vec3.Y);
		}

		public static explicit operator Vector2D((double x, double y) t) {
			return new Vector2D(t.x, t.y);
		}

		public static explicit operator (double x, double y)(Vector2D vec2) {
			return (vec2.X, vec2.Y);
		}

		public override string ToString() {
			return $"({X}; {Y})"; 
		}
	}
}
