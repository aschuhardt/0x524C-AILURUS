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

        public static double Distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public static T GetConfiguration<T>(string path)
        {
            if (!File.Exists(path))
            {
                try
                {
                    var defaultConfig = Activator.CreateInstance<T>();
                    var json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, json);
                    return GetConfiguration<T>(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write default configuration file to {path}");
                    Console.WriteLine(ex.Message);
                    return Activator.CreateInstance<T>();
                }
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read configuration file at {path}");
                    Console.WriteLine(ex.Message);
                    return Activator.CreateInstance<T>();
                }
            }
        }
    }
}
