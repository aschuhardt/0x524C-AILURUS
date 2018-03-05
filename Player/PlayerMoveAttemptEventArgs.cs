using Microsoft.Xna.Framework;
using System;

namespace ailurus.Player
{
    public class PlayerMoveAttemptEventArgs : EventArgs
    {
        public Point Destination { get; }
        public bool Cancel { get; set; }

        public PlayerMoveAttemptEventArgs(Point destination)
        {
            Destination = destination;
            Cancel = false;
        }
    }
}
