using Microsoft.Xna.Framework;
using System;

namespace ailurus.Map
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DecorationAttribute : Attribute
    {
        readonly DecorationType _decoration;
        readonly float _frequency;
        readonly Color _color;

        public DecorationAttribute(DecorationType decoration, float frequency, int r, int g, int b)
        {
            _decoration = decoration;
            _frequency = frequency;
            _color = new Color(r, g, b);
        }

        public DecorationType DecorationType => _decoration;
        public float Frequency => _frequency;
        public Color Color => _color;
    }
}
