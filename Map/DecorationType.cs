using System;
using System.Collections.Generic;
using System.Text;

namespace ailurus.Map
{
    public enum DecorationType
    {
        None,

        [Texture("tiles/flower_1", 0)]
        [Texture("tiles/flower_2", 1)]
        [Texture("tiles/flower_3", 2)]
        [Texture("tiles/flower_4", 3)]
        Flower,

        [Texture("tiles/stone_small")]
        SmallStone,

        [Texture("tiles/stone_medium")]
        MediumStone,

        [Texture("tiles/stone_large")]
        LargeStone,
    }
}
