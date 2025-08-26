#if ENABLED_UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace YooX {
	static public class MathfExtension {
		#region Min

#if ENABLED_UNITY_MATHEMATICS
        public static half Min(half a, half b) {
            return (a < b) ? a : b;
        }

        public static half Min(params half[] values) {
            var num = values.Length;
            if (num == 0) {
                return (half) 0;
            }

            var num2 = values[0];
            for (var i = 1; i < num; i++) {
                if (values[i] < num2) {
                    num2 = values[i];
                }
            }

            return num2;
        }
#endif

		static public double Min(double a, double b) =>
			a < b
				? a
				: b;

		static public double Min(params double[] values) {
			int num = values.Length;

			if (num == 0) {
				return 0f;
			}

			double num2 = values[0];

			for (var i = 1; i < num; i++) {
				if (values[i] < num2) {
					num2 = values[i];
				}
			}

			return num2;
		}

		#endregion

		#region Max

#if ENABLED_UNITY_MATHEMATICS
        public static half Max(half a, half b) {
            return (a > b) ? a : b;
        }

        public static half Max(params half[] values) {
            var num = values.Length;
            if (num == 0) {
                return (half) 0;
            }

            var num2 = values[0];
            for (int i = 1; i < num; i++) {
                if (values[i] > num2) {
                    num2 = values[i];
                }
            }

            return num2;
        }
#endif

		static public double Max(double a, double b) =>
			a > b
				? a
				: b;

		static public double Max(params double[] values) {
			int num = values.Length;

			if (num == 0) {
				return 0f;
			}

			double num2 = values[0];

			for (var i = 1; i < num; i++) {
				if (values[i] > num2) {
					num2 = values[i];
				}
			}

			return num2;
		}

		#endregion
	}
}