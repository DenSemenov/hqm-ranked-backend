

namespace LinearAlgebra{

	public enum AngleMeasure { RADIANS, DEGREES };

	public static partial class Geometry {

		public static double ToRadians(double angle) {
			return angle / 180.0 * Math.PI;
		}

		public static double ToDegrees(double rads) {
			return rads / Math.PI * 180.0;
		}

		public static Vector3D ToRadians(Vector3D ypr) {
			return ypr / 180.0 * Math.PI;
		}

		public static Vector3D ToDegrees(Vector3D ypr) {
			return ypr / Math.PI * 180.0;
		}

		public static Vector3D CrossProduct(Vector3D vec1, Vector3D vec2) { 
			return new Vector3D(vec1.Y * vec2.Z - vec2.Y * vec1.Z, vec1.Z * vec2.X - vec1.X * vec2.Z, vec1.X * vec2.Y - vec2.X * vec1.Y);
		}

		//Cross points

		public static bool GetCrossPoint(Ray3 ray, Plane plane, out Vector3D result) { 
			return GetCrossPoint(ray, plane.Normal, plane.D, out result);
		}

		
		public static bool GetCrossPoint(Ray3 ray, Vector3D normal, double d, out Vector3D result) {
			//Перпендикулярность вектора. Вероятность пересечения крайне мала
			if(ray.Dir.X * normal.X + ray.Dir.Y * normal.Y + ray.Dir.Z * normal.Z == 0) {
				result = default;
				return false;
			}

			//normal.x * x + normal.y * (line.dir.y * (x / line.dir.x) + line.shiftYZ.y) + normal.z * (line.dir.z * (x / line.dir.x) + line.shiftYZ.z) + d = 0
			//normal.x * x + normal.y * line.dir.y * x / line.dir.x + normal.dir.y * line.shiftYZ.y + normal.z * line.dir.z * x / line.dir.x + normal.z * line.shiftYZ.z + d = 0
			//normal.x * x + normal.y * line.dir.y * x / line.dir.x + normal.z * line.dir.z * x / line.dir.x = -(normal.y * line.shiftYZ.y + normal.z * line.shiftYZ.z + d)
			//x * (normal.x + normal.y * line.dir.y / line.dir.x + normal.z * line.dir.z / line.dir.x) = -(normal.y * line.shiftYZ.y + normal.z * line.shiftYZ.z + d)
			//x = -(normal.y * line.shiftYZ.y + normal.z * line.shiftYZ.z + d) / (normal.x + normal.y * line.dir.y / line.dir.x + normal.z * line.dir.z / line.dir.x)

			if(ray.IsAxisXDirected) {
				Vector3D shiftYZ = ray.ShiftYZ;
				double x = -(normal.Y * shiftYZ.Y + normal.Z * shiftYZ.Z + d) / (normal.X + normal.Y * ray.Dir.Y / ray.Dir.X + normal.Z * ray.Dir.Z / ray.Dir.X);
				if(ray.IsXOnRay(x))
					return ray.GetPointWithX(x, out result);
			} else if(ray.IsAxisYDirected) {
				Vector3D shiftXZ = ray.ShiftXZ;
				double y = -(normal.X * shiftXZ.X + normal.Z * shiftXZ.Z + d) / (normal.Y + normal.X * ray.Dir.X / ray.Dir.Y + normal.Z * ray.Dir.Z / ray.Dir.Y);
				if(ray.IsYOnRay(y))
					return ray.GetPointWithY(y, out result);
			} else if(ray.IsAxisZDirected) {
				Vector3D shiftXY = ray.ShiftXY;
				double z = -(normal.Y * shiftXY.Y + normal.X * shiftXY.X + d) / (normal.Z + normal.Y * ray.Dir.Y / ray.Dir.Z + normal.X * ray.Dir.X / ray.Dir.Z);
				if(ray.IsZOnRay(z))
					return ray.GetPointWithZ(z, out result);
			}

			result = default;
			return false;
		}
		public static bool GetXPlaneCrossPoint(Ray3 ray, double planeX, Vector3D crossPoint, out Vector3D result) {
			return ray.GetPointWithX(planeX, out result);
		}

		public static bool GetYPlaneCrossPoint(Ray3 ray, double planeY, Vector3D crossPoint, out Vector3D result) {
			return ray.GetPointWithY(planeY, out result);
		}

		public static bool GetZPlaneCrossPoint(Ray3 ray, double planeZ, Vector3D crossPoint, out Vector3D result) {
			return ray.GetPointWithZ(planeZ, out result);
		}

		//In checkers
		public static bool CheckIn(double a, double value, double b) { 
			return (a <= value) && (value <= b);
		}

		public static bool CheckIn(Vector2D a, Vector2D value, Vector2D b) { 
			//Optimized without top level CheckIn call
			return (a.X <= value.X) && (value.X <= b.X) && (a.Y <= value.Y) && (value.Y <= b.Y);
		}

		public static bool CheckIn(Vector3D a, Vector3D value, Vector3D b) { 
			//Optimized without top level CheckIn call
			return (a.X <= value.X) && (value.X <= b.X) && (a.Y <= value.Y) && (value.Y <= b.Y) && (a.Z <= value.Z) && (value.Z <= b.Z);
		}


		//Interpolation

		public static Vector2D Interpolate(Vector2D a, Vector2D b, double percent) {
			return a * (1.0 - percent) + b * percent;
		}

		public static Vector3D Interpolate(Vector3D a, Vector3D b, double percent) {
			return a * (1.0 - percent) + b * percent;
		}

		public static Vector4D Interpolate(Vector4D a, Vector4D b, double percent) {
			return a * (1.0 - percent) + b * percent;
		}

		public static Quaternion Interpolate(Quaternion a, Quaternion b, double percent) {
			return a * (1.0 - percent) + b * percent;
		}

		//Quaternions

		public static double ScalarMultiply(Quaternion a, Quaternion b) {
			return a.W * b.W + a.X * b.X + a.Y * b.Y * a.Z * b.Z;
		}

		public static Vector3D Rotate(this Vector3D vec, Quaternion quaternion) {
			return  quaternion * new Quaternion(vec) * (-quaternion);
		}

		public static void Rotate(ref Vector3D vec, Quaternion quaternion) {
			vec = quaternion * new Quaternion(vec) * (-quaternion);
		}

		public static Ray3 RotateFull(this Ray3 ray, Quaternion quaternion) {
			return Ray3.FromTwoPoints(ray.StartPoint.Rotate(quaternion), (ray.StartPoint + ray.Dir).Rotate(quaternion), ray.IsLine);
		}

		public static Ray3 RotateDir(this Ray3 ray, Quaternion quaternion) {
			return new Ray3(ray.StartPoint, ray.Dir.Rotate(quaternion), ray.IsLine);
		}

		//Shifts

		public static Vector2D Shift(this Vector2D vec, Vector2D shiftVec) { return vec + shiftVec; }

		public static Vector3D Shift(this Vector3D vec, Vector3D shiftVec) { return vec + shiftVec; }

		public static Vector4D Shift(this Vector4D vec, Vector4D shiftVec) { return vec + shiftVec; }

		public static Ray3 Shift(this Ray3 ray, Vector3D shiftVec) { return new Ray3(ray.StartPoint + shiftVec, ray.Dir, ray.IsLine); }

	}
}
