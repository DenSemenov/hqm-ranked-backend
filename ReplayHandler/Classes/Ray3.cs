

namespace LinearAlgebra{
	public struct Ray3 {
		public Vector3D Dir;
		public Vector3D StartPoint;
		public bool IsLine;

		public Ray2 XYProj { get => new Ray2(StartPoint.XY, Dir.XY, IsLine); }
		public Ray2 YZProj { get => new Ray2(StartPoint.YZ, Dir.YZ, IsLine); }
		public Ray2 ZXProj { get => new Ray2(StartPoint.ZX, Dir.ZX, IsLine); }

		public Ray2 YXProj { get => new Ray2(StartPoint.YX, Dir.YX, IsLine); }
		public Ray2 ZYProj { get => new Ray2(StartPoint.ZY, Dir.ZY, IsLine); }
		public Ray2 XZProj { get => new Ray2(StartPoint.XZ, Dir.XZ, IsLine); }



		public bool IsAxisXDirected { get { return Dir.X != 0.0; } }
		public bool IsAxisYDirected { get { return Dir.Y != 0.0; } }
		public bool IsAxisZDirected { get { return Dir.Z != 0.0; } }

		public Vector3D ShiftYZ { get { return IsAxisXDirected ? StartPoint - Dir * (StartPoint.X / Dir.X) : throw new Exception($"The ray {this} isn't directed by axis x"); } }
		public Vector3D ShiftXZ { get { return IsAxisYDirected ? StartPoint - Dir * (StartPoint.Y / Dir.Y) : throw new Exception($"The ray {this} isn't directed by axis y"); } }
		public Vector3D ShiftXY { get { return IsAxisZDirected ? StartPoint - Dir * (StartPoint.Z / Dir.Z) : throw new Exception($"The ray {this} isn't directed by axis z"); } }

		public Ray3(Vector3D point, Vector3D dir, bool isLine = false) { 
			this.Dir = dir;
			this.StartPoint = point;
			this.IsLine = isLine;
		}

		public static Ray3 FromTwoPoints(Vector3D from, Vector3D to, bool isLine = false) {
			return new Ray3(from, to - from, isLine);
		}

		

		public bool GetPointWithX(double x, out Vector3D result) {
			if(Dir.X != 0.0) {
				result = Dir * (x / Dir.X) + ShiftYZ;
				return true;
			} else {
				result = default;
				return false;
			}
		}
		public bool GetPointWithY(double y, out Vector3D result) { 
			if(Dir.Y != 0.0) {
				result = Dir * (y / Dir.Y) + ShiftXZ;
				return true;
			} else {
				result = default;
				return false;
			}
		}
		public bool GetPointWithZ(double z, out Vector3D result) { 
			if(Dir.Z != 0.0) {
				result = Dir * (z / Dir.Z) + ShiftXY;
				return true;
			} else {
				result = default;
				return false;
			}
		}

		public override string ToString() {
			return $"{StartPoint} - {Dir}";
		}

		public bool IsXOnRay(double x) {
			return (x == StartPoint.X) || (IsAxisXDirected && (IsLine || (x > StartPoint.X ? Dir.X > 0 : Dir.X < 0)));
		}

		public bool IsYOnRay(double y) {
			return (y == StartPoint.Y) || (IsAxisYDirected && (IsLine || (y > StartPoint.Y ? Dir.Y > 0 : Dir.Y < 0)));
		}

		public bool IsZOnRay(double z) {
			return (z == StartPoint.Z) || (IsAxisZDirected && (IsLine || (z > StartPoint.Z ? Dir.Z > 0 : Dir.Z < 0)));
		}
	};
}
