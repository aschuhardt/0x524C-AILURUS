using ailurus.Map;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ailurus
{
    public static class Helpers
    {
        public static TextureMap<T> LoadTextures<T>(ContentManager content)
        {
            var loaded = new TextureMap<T>();
            foreach (var t in typeof(T).GetMembers())
            {
                var attrs = t.GetCustomAttributes(typeof(TextureAttribute), false);
                if (attrs != null && attrs.Any())
                {
                    T tileType = (T)Enum.Parse(typeof(T), t.Name);
                    loaded.Add(tileType, new List<Texture2D>());
                    foreach (var texture in attrs.Cast<TextureAttribute>().OrderBy(x => x.Index))
                    {
                        loaded[tileType].Add(content.Load<Texture2D>(texture.ContentPath));
                    }
                }
            }
            return loaded;
        }
    }
}
