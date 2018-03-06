﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ailurus
{
    public class Config
    {
        public const int DEFAULT_WIDTH = 800;
        public const int DEFAULT_HEIGHT = 480;
        public const int DEFAULT_MAP_SIZE = 512;

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
    }
}
