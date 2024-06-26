

namespace LinearAlgebra{
	public class Plane {
		public Vector3D Normal;
		public double D = 0.0;

		public enum ParameterType { STANDART_D, NORMAL_SHIFT };

		public Plane(Vector3D normal, double parameter = 0.0, ParameterType parameterType = ParameterType.STANDART_D) {
			this.Normal = normal;
			switch (parameterType) {
			case ParameterType.STANDART_D:
				D = parameter;
				break;
			case ParameterType.NORMAL_SHIFT:
				SetNormalShift(parameter);
				break;
			}
		}

		public static Plane FromPoints(Vector3D point1, Vector3D point2, Vector3D point3) { 
			Vector3D vec1 = point2 - point1;
			Vector3D vec2 = point3 - point1;
			Vector3D normal = Geometry.CrossProduct(vec1, vec2);
			return new Plane(normal, -normal * point1, ParameterType.STANDART_D);
		}

		public void SetNormalShift(double normalLength) {
			D = -Normal.SqrLength() * normalLength;
		}

		public override string ToString() {
			return $"(N = {Normal}; D = {D})";
		}
	};
}
