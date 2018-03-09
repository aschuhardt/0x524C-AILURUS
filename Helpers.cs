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
        public const string MAP_GEN_CONFIG_PATH = "config/map_generation.json";

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

        public static MapGenerationConfig GetMapGenerationConfig()
        {
            if (!File.Exists(MAP_GEN_CONFIG_PATH))
            {
                try
                {
                    var defaultConfig = new MapGenerationConfig();
                    var json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                    Directory.CreateDirectory(Path.GetDirectoryName(MAP_GEN_CONFIG_PATH));
                    File.WriteAllText(MAP_GEN_CONFIG_PATH, json);
                    return GetMapGenerationConfig();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write default configuration file to {MAP_GEN_CONFIG_PATH}");
                    Console.WriteLine(ex.Message);
                    return new MapGenerationConfig();
                }
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(MAP_GEN_CONFIG_PATH);
                    return JsonConvert.DeserializeObject<MapGenerationConfig>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read configuration file at {MAP_GEN_CONFIG_PATH}");
                    Console.WriteLine(ex.Message);
                    return new MapGenerationConfig();
                }
            }
        }
    }
}
