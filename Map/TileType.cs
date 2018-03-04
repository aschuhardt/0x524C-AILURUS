using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ailurus.Map
{
    public enum TileType
    {
        [Texture("tiles/dots_1", 0)]
        [Texture("tiles/dots_2", 1)]
        [Texture("tiles/dots_3", 2)]
        [Decoration(DecorationType.Flower, 0.2f, 20, 69, 141)]
        [Decoration(DecorationType.Flower, 0.4f, 214, 39, 12)]
        [Decoration(DecorationType.Flower, 0.6f, 238, 219, 57)]
        Grass,

        [Texture("tiles/splatter")]
        [Decoration(DecorationType.LargeStone, 0.2f, 74, 60, 58)]
        [Decoration(DecorationType.MediumStone, 0.4f, 95, 101, 109)]
        [Decoration(DecorationType.SmallStone, 0.6f, 61, 58, 33)]
        Dirt
    }
}
