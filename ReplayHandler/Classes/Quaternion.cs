

namespace LinearAlgebra{
	public struct Quaternion {
		public double W, X, Y, Z;

		public static readonly Quaternion Identity = FromYawPitchRoll(0, 0, 0);

		public Quaternion(double w, double x, double y, double z, bool normalize = true) {
			this.W = w;
			this.X = x;
			this.Y = y;
			this.Z = z;
			
			if(normalize)
				this.Normalize();
		}

		public Quaternion(Vector3D vec) {
			W = 0;
			X = vec.X;
			Y = vec.Y;
			Z = vec.Z;
		}

		public Quaternion(Vector4D vec) {
			W = vec.W;
			X = vec.X;
			Y = vec.Y;
			Z = vec.Z;
		}

		public static Quaternion FromAxisRotation(double angle, Vector3D Axis, AngleMeasure measure = AngleMeasure.RADIANS) => FromAxisRotation(angle, Axis.X, Axis.Y, Axis.Z, measure);

		public static Quaternion FromAxisRotation(double angle, double axisX, double axisY, double axisZ, AngleMeasure measure = AngleMeasure.RADIANS) { 
			if(measure == AngleMeasure.DEGREES)
				angle = Geometry.ToRadians(angle);
			double sind2 = Math.Sin(angle / 2.0);
			return new Quaternion(Math.Cos(angle / 2.0), axisX * sind2, axisY * sind2, axisZ * sind2);
		}

		public static Quaternion FromYaw(double yaw, AngleMeasure angleMeasure = AngleMeasure.RADIANS) {
			return Quaternion.FromAxisRotation(yaw, 0.0, 1.0, 0.0, angleMeasure).Normalized();
		}

		public static Quaternion FromYawPitch(double yaw, double pitch, AngleMeasure angleMeasure = AngleMeasure.RADIANS) {
			return (Quaternion.FromAxisRotation(yaw, 0.0, 1.0, 0.0, angleMeasure) * Quaternion.FromAxisRotation(pitch, 0.0, 0.0, 1.0, angleMeasure)).Normalized();
		}

		public static Quaternion FromYawPitchRoll(Vector3D ypr, AngleMeasure angleMeasure = AngleMeasure.RADIANS) => FromYawPitchRoll(ypr.Yaw, ypr.Pitch, ypr.Roll);

		public static Quaternion FromYawPitchRoll(double yaw, double pitch, double roll, AngleMeasure angleMeasure = AngleMeasure.RADIANS) {
			return (Quaternion.FromAxisRotation(yaw, 0.0, 1.0, 0.0, angleMeasure) * Quaternion.FromAxisRotation(pitch, 0.0, 0.0, 1.0, angleMeasure) * Quaternion.FromAxisRotation(roll, 1.0, 0.0, 0.0, angleMeasure)).Normalized();
		}

		public Vector3D GetYawPitchRoll(AngleMeasure measure = AngleMeasure.RADIANS) {
			return measure == AngleMeasure.RADIANS ? new Vector3D(
				Math.Atan2(2*Y*W - 2*X*Z, 1 - 2*Y*Y - 2*Z*Z),
				Math.Asin(Math.Max(Math.Min(2*X*Y + 2*Z*W, 1.0), -1.0)),
				Math.Atan2(2*X*W - 2*Y*Z, 1 - 2*X*X - 2*Z*Z)
			) : new Vector3D(
				Geometry.ToDegrees(Math.Atan2(2*Y*W - 2*X*Z, 1 - 2*Y*Y - 2*Z*Z)),
				Geometry.ToDegrees(Math.Asin(Math.Max(Math.Min(2*X*Y + 2*Z*W, 1.0), -1.0))),
				Geometry.ToDegrees(Math.Atan2(2*X*W - 2*Y*Z, 1 - 2*X*X - 2*Z*Z))
			);
		}

		public static Quaternion operator+(Quaternion a, Quaternion b) {
			return new Quaternion(a.W + b.W, a.X + b.X, a.Y + b.Y, a.Z + b.Z, false);
		}

		public static Quaternion operator-(Quaternion a, Quaternion b) {
			return new Quaternion(a.W - b.W, a.X - b.X, a.Y - b.Y, a.Z - b.Z, false);
		}

		public static Quaternion operator-(Quaternion a) {
			return new Quaternion(a.W, -a.X, -a.Y, -a.Z, false);
		}

		
		public static Quaternion operator*(Quaternion a, double mul) {
			return new Quaternion(a.W * mul, a.X * mul , a.Y * mul, a.Z * mul, false);
		}

		public static Quaternion operator/(Quaternion a, double div) {
			return new Quaternion(a.W / div, a.X / div , a.Y / div, a.Z / div, false);
		}

		//a * b дают кватернион, равносильный повороту сначала b потом a
		public static Quaternion operator*(Quaternion a, Quaternion b) {
			return new Quaternion(
				a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z, 
				a.X * b.W + a.W * b.X + a.Y * b.Z - a.Z * b.Y,
				a.Y * b.W + a.W * b.Y + a.Z * b.X - a.X * b.Z, 
				a.Z * b.W + a.W * b.Z + a.X * b.Y - a.Y * b.X, false);
		}

		public static double operator^(Quaternion a, Quaternion b) { 
			return Dot(a, b) / (a.GetLength() * b.GetLength());
		}

		public static double Dot(Quaternion a, Quaternion b) { 
			return a.W * b.W + a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public double Dot(Quaternion b) { 
			return Dot(this, b);
		}


		public double GetLength() {
			return Math.Sqrt(W * W + X * X + Y * Y + Z * Z);
		}

		public double GetSqrLength() {
			return W * W + X * X + Y * Y + Z * Z;
		}

		public Quaternion Reverse() { 
			return (-this) / GetSqrLength();
		}

        public double Norm()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public void Normalize() {
			double l = GetLength();
			W /= l;
			X /= l;
			Y /= l;
			Z /= l;
		}

        public Quaternion ScaleBy(double scaleFactor)
        {
            return new Vector3D(this.X * scaleFactor, this.Y * scaleFactor, this.Z * scaleFactor);
        }

        public Quaternion Normalized() {
			return new Quaternion(W, X, Y, Z, true);
		}

		public Vector3D Rotate(Vector3D vec) {
			return this * new Quaternion(vec) * (-this);
		}


		public static implicit operator Quaternion(Vector3D vec) {
			return new Quaternion(vec);
		}

		public static implicit operator Quaternion(Vector4D vec) {
			return new Quaternion(vec);
		}

		public static implicit operator Vector4D(Quaternion vec) {
			return new Vector4D(vec);
		}

		public override string ToString() {
			 return $"(W = {W}; X = {X}; Y = {Y}; Z = {Z})";
		}


	};
}
