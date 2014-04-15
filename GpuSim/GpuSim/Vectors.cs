using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FragSharpFramework
{
    [Hlsl("float2")]
    public partial struct vec2
    {
        [Hlsl("float2")]
        public vec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        [Hlsl("x")]
        public float x;

        [Hlsl("y")]
        public float y;

        public static vec2 operator *(float a, vec2 v)
        {
            return new vec2(a * v.x, a * v.y);
        }

        public static vec2 operator *(vec2 v, float a)
        {
            return new vec2(a * v.x, a * v.y);
        }

        public static vec2 operator /(float a, vec2 v)
        {
            return new vec2(a / v.x, a / v.y);
        }

        public static vec2 operator /(vec2 v, float a)
        {
            return new vec2(v.x / a, v.y / a);
        }

        public static vec2 operator +(vec2 v, vec2 w)
        {
            return new vec2(v.x + w.x, v.y + w.y);
        }

        public static vec2 operator -(vec2 v, vec2 w)
        {
            return new vec2(v.x - w.x, v.y - w.y);
        }

        public static vec2 operator *(vec2 v, vec2 w)
        {
            return new vec2(v.x * w.x, v.y * w.y);
        }

        public static vec2 operator /(vec2 v, vec2 w)
        {
            return new vec2(v.x / w.x, v.y / w.y);
        }

        public static implicit operator Vector2(vec2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static explicit operator vec2(Vector2 v)
        {
            return new vec2(v.X, v.Y);
        }

        public static readonly vec2 Zero = new vec2(0, 0);
    }

    [Hlsl("float3")]
    public partial struct vec3
    {
        [Hlsl("float3")]
        public vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        [Hlsl("x")]
        public float x;

        [Hlsl("y")]
        public float y;

        [Hlsl("z")]
        public float z;

        public vec2 xy { get { return new vec2(x, y); } set { x = value.x; y = value.y; } }

        public static implicit operator Vector3(vec3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static explicit operator vec3(Vector3 v)
        {
            return new vec3(v.X, v.Y, v.Z);
        }

        public static readonly vec3 Zero = new vec3(0, 0, 0);
    }

    [Hlsl("float4")]
    public partial struct vec4
    {
        [Hlsl("float4")]
        public vec4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        [Hlsl("x")]
        public float x;

        [Hlsl("y")]
        public float y;

        [Hlsl("z")]
        public float z;

        [Hlsl("w")]
        public float w;

        public vec2 xy { get { return new vec2(x, y); } set { x = value.x; y = value.y; } }
        public vec3 xyz { get { return new vec3(x, y, z); } set { x = value.x; y = value.y; z = value.z; } }

        [Hlsl("r")]
        public float r { get { return x; } set { x = value; } }

        [Hlsl("g")]
        public float g { get { return y; } set { y = value; } }

        [Hlsl("b")]
        public float b { get { return z; } set { z = value; } }

        [Hlsl("a")]
        public float a { get { return w; } set { w = value; } }

        public vec3 rgb { get { return xyz; } set { xyz = value; } }

        public static vec4 operator *(float a, vec4 v)
        {
            return new vec4(a * v.x, a * v.y, a * v.z, a * v.w);
        }

        public static vec4 operator *(vec4 v, float a)
        {
            return new vec4(a * v.x, a * v.y, a * v.z, a * v.w);
        }

        public static vec4 operator /(float a, vec4 v)
        {
            return new vec4(a / v.x, a / v.y, a / v.z, a / v.w);
        }

        public static vec4 operator /(vec4 v, float a)
        {
            return new vec4(v.x / a, v.y / a, v.z / a, v.w / a);
        }

        public static vec4 operator +(vec4 v, vec4 w)
        {
            return new vec4(v.x + w.x, v.y + w.y, v.z + w.z, v.w + w.w);
        }

        public static vec4 operator -(vec4 v, vec4 w)
        {
            return new vec4(v.x - w.x, v.y - w.y, v.z - w.z, v.w - w.w);
        }

        public static vec4 operator *(vec4 v, vec4 w)
        {
            return new vec4(v.x * w.x, v.y * w.y, v.z * w.z, v.w * w.w);
        }

        public static vec4 operator /(vec4 v, vec4 w)
        {
            return new vec4(v.x / w.x, v.y / w.y, v.z / w.z, v.w / w.w);
        }

        public static implicit operator Vector4(vec4 v)
        {
            return new Vector4(v.x, v.y, v.z, v.w);
        }

        public static explicit operator vec4(Vector4 v)
        {
            return new vec4(v.X, v.Y, v.Z, v.W);
        }

        public static readonly vec4 Zero = new vec4(0, 0, 0, 0);
    }

    [Copy(typeof(vec4))]
    public partial struct color
    {
        public static readonly color TransparentBlack = new color(0, 0, 0, 0);

        public static explicit operator color(Color v)
        {
            return new color(v.R, v.G, v.B, v.A);
        }
    }

    [Copy(typeof(vec4))]
    public partial struct unit
    {
        [Hlsl("r")]
        public float direction { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float change { get { return r; } set { r = value; } }
        
        public static readonly unit Nothing = new unit(0, 0, 0, 0);
    }
}