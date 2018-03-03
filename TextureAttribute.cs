using System;

namespace ailurus
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class TextureAttribute : Attribute
    {
        readonly string _contentPath;
        readonly int _index;

        public TextureAttribute(string contentPath)
        {
            this._contentPath = contentPath;
            _index = 0;
        }

        public TextureAttribute(string contentPath, int index) : this(contentPath)
        {
            _index = index;
        }

        public string ContentPath => _contentPath;
        public int Index => _index;
    }
}
