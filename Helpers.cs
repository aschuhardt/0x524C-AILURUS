using ailurus.Map;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                    T variant = (T)Enum.Parse(typeof(T), t.Name);
                    loaded.Add(variant, new List<Texture2D>());
                    foreach (var texture in attrs.Cast<TextureAttribute>().OrderBy(x => x.Index))
                    {
                        loaded[variant].Add(content.Load<Texture2D>(texture.ContentPath));
                    }
                }
            }
            return loaded;
        }

        public static Config LoadConfig(string path)
        {
            path = Path.GetFullPath(path);

            var cfg = new Config()
            {
                WindowWidth = Config.DEFAULT_WIDTH,
                WindowHeight = Config.DEFAULT_HEIGHT,
                MapWidth = Config.DEFAULT_MAP_SIZE,
                MapHeight = Config.DEFAULT_MAP_SIZE
            };

            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(cfg, Formatting.Indented));
                return cfg;
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
                }
                catch (Exception)
                {
                    return cfg;
                }
            }
        }

        public static void SaveConfig(Config cfg, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(cfg, Formatting.Indented));
        }

        public static Decorations GetDecorations()
        {
            var decorations = new Decorations();
            foreach (var t in typeof(TileType).GetMembers())
            {
                var attrs = t.GetCustomAttributes(typeof(DecorationAttribute), false);
                if (attrs != null && attrs.Any())
                {
                    var variant = (TileType)Enum.Parse(typeof(TileType), t.Name);
                    decorations.Add(variant, new List<DecorationAttribute>());
                    foreach (var attr in attrs.Cast<DecorationAttribute>())
                        decorations[variant].Add(attr);
                }
            }
            return decorations;
        }
    }
}
