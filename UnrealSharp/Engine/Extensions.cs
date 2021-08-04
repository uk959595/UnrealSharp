﻿using System;
using System.Numerics;

namespace UnrealSharp
{
    public static class Extensions
    {
        public static SharpDX.Color ToSharpDx(this System.Drawing.Color color)
        {
            return new SharpDX.Color(color.R, color.G, color.B, color.A);
        }
        public static SharpDX.Vector3 ToSharpDx(this Vector3 v3)
        {
            return new SharpDX.Vector3(v3.X, v3.Y, v3.Z);
        }
        public static SharpDX.Vector2 ToSharpDx(this Vector2 v2)
        {
            return new SharpDX.Vector2(v2.X, v2.Y);
        }
        public static Vector3 ToNative(this SharpDX.Vector3 v3)
        {
            return new Vector3(v3.X, v3.Y, v3.Z);
        }
        public static Vector2 ToNative(this SharpDX.Vector2 v2)
        {
            return new Vector2(v2.X, v2.Y);
        }
        public static Single Mult(this Vector3 v, Vector3 s) => v.X * s.X + v.Y * s.Y + v.Z * s.Z;
        public static void GetAxes(this Vector3 v, out Vector3 x, out Vector3 y, out Vector3 z)
        {
            var m = v.ToMatrix();

            x = new Vector3(m[0, 0], m[0, 1], m[0, 2]);
            y = new Vector3(m[1, 0], m[1, 1], m[1, 2]);
            z = new Vector3(m[2, 0], m[2, 1], m[2, 2]);
        }
        public static Vector3 FromRotator(this Vector3 v)
        {
            float radPitch = (float)(v.X * Math.PI / 180f);
            float radYaw = (float)(v.Y * Math.PI / 180f);
            float SP = (float)Math.Sin(radPitch);
            float CP = (float)Math.Cos(radPitch);
            float SY = (float)Math.Sin(radYaw);
            float CY = (float)Math.Cos(radYaw);
            return new Vector3(CP * CY, CP * SY, SP);
        }
        public static Single[,] ToMatrix(this Vector3 v, Vector3 origin = default(Vector3))
        {
            if (origin == default)
                origin = default;
            var radPitch = (Single)(v.X * Math.PI / 180f);
            var radYaw = (Single)(v.Y * Math.PI / 180f);
            var radRoll = (Single)(v.Z * Math.PI / 180f);

            var SP = (Single)Math.Sin(radPitch);
            var CP = (Single)Math.Cos(radPitch);
            var SY = (Single)Math.Sin(radYaw);
            var CY = (Single)Math.Cos(radYaw);
            var SR = (Single)Math.Sin(radRoll);
            var CR = (Single)Math.Cos(radRoll);

            var m = new Single[4, 4];
            m[0, 0] = CP * CY;
            m[0, 1] = CP * SY;
            m[0, 2] = SP;
            m[0, 3] = 0f;

            m[1, 0] = SR * SP * CY - CR * SY;
            m[1, 1] = SR * SP * SY + CR * CY;
            m[1, 2] = -SR * CP;
            m[1, 3] = 0f;

            m[2, 0] = -(CR * SP * CY + SR * SY);
            m[2, 1] = CY * SR - CR * SP * SY;
            m[2, 2] = CR * CP;
            m[2, 3] = 0f;

            m[3, 0] = origin.X;
            m[3, 1] = origin.Y;
            m[3, 2] = origin.Z;
            m[3, 3] = 1f;
            return m;
        }
        public struct Transform
        {
            public Vector3 Rotation;
            public Single RotationW;
            public Vector3 Translation;
            public Single TranslationW;
            public Vector3 Scale;
            public Single ScaleW;
            public Matrix4x4 ToMatrixWithScale()
            {
                float x2 = Rotation.X + Rotation.X;
                float y2 = Rotation.Y + Rotation.Y;
                float z2 = Rotation.Z + Rotation.Z;

                float xx2 = Rotation.X * x2;
                float yy2 = Rotation.Y * y2;
                float zz2 = Rotation.Z * z2;

                float yz2 = Rotation.Y * z2;
                float wx2 = RotationW * x2;

                float xy2 = Rotation.X * y2;
                float wz2 = RotationW * z2;

                float xz2 = Rotation.X * z2;
                float wy2 = RotationW * y2;

                var m = new Matrix4x4
                {
                    M41 = Translation.X,
                    M42 = Translation.Y,
                    M43 = Translation.Z,
                    M11 = (1.0f - (yy2 + zz2)) * Scale.X,
                    M22 = (1.0f - (xx2 + zz2)) * Scale.Y,
                    M33 = (1.0f - (xx2 + yy2)) * Scale.Z,
                    M32 = (yz2 - wx2) * Scale.Z,
                    M23 = (yz2 + wx2) * Scale.Y,
                    M21 = (xy2 - wz2) * Scale.Y,
                    M12 = (xy2 + wz2) * Scale.X,
                    M31 = (xz2 + wy2) * Scale.Z,
                    M13 = (xz2 - wy2) * Scale.X,
                    M14 = 0.0f,
                    M24 = 0.0f,
                    M34 = 0.0f,
                    M44 = 1.0f
                };
                return m;
            }
            public Single[,] ToMatrixWithScale2()
            {
                var m = new Single[4, 4];

                m[3, 0] = Translation.X;
                m[3, 1] = Translation.Y;
                m[3, 2] = Translation.Z;

                var x2 = Rotation.X * 2;
                var y2 = Rotation.Y * 2;
                var z2 = Rotation.Z * 2;

                var xx2 = Rotation.X * x2;
                var yy2 = Rotation.Y * y2;
                var zz2 = Rotation.Z * z2;
                m[0, 0] = (1.0f - (yy2 + zz2)) * Scale.X;
                m[1, 1] = (1.0f - (xx2 + zz2)) * Scale.Y;
                m[2, 2] = (1.0f - (xx2 + yy2)) * Scale.Z;

                var yz2 = Rotation.Y * z2;
                var wx2 = RotationW * x2;
                m[2, 1] = (yz2 - wx2) * Scale.Z;
                m[1, 2] = (yz2 + wx2) * Scale.Y;

                var xy2 = Rotation.X * y2;
                var wz2 = RotationW * z2;
                m[1, 0] = (xy2 - wz2) * Scale.Y;
                m[0, 1] = (xy2 + wz2) * Scale.X;

                var xz2 = Rotation.X * z2;
                var wy2 = RotationW * y2;
                m[2, 0] = (xz2 + wy2) * Scale.Z;
                m[0, 2] = (xz2 - wy2) * Scale.X;

                m[0, 3] = 0.0f;
                m[1, 3] = 0.0f;
                m[2, 3] = 0.0f;
                m[3, 3] = 1.0f;

                return m;
            }
        }
        public static Single[,] MultiplyMatrix(Single[,] a, Single[,] b)
        {
            var c = new Single[4, 4];
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    c[i, j] = 0;
                    for (var k = 0; k < 4; k++) c[i, j] += a[i, k] * b[k, j];
                }
            }
            return c;
        }
        public static Vector3 CalcRotation(this Vector3 source, Vector3 destination, Vector3 origAngles, Single smooth)
        {
            var angles = new Vector3();
            var diff = source - destination;
            var hyp = Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
            angles.Y = (Single)Math.Atan(diff.Y / diff.X) * 57.295779513082f;
            angles.X = -(Single)Math.Atan(diff.Z / hyp) * 57.295779513082f;
            angles.Z = 0.0f;
            if (diff.X >= 0.0)
            {
                if (angles.Y > 0)
                    angles.Y -= 180.0f;
                else
                    angles.Y += 180.0f;
            }
            if (smooth > 0 && Math.Abs(angles.Y - origAngles.Y) < 180.0f)
                angles -= ((angles - origAngles) * smooth);
            return angles;
        }
    }
}