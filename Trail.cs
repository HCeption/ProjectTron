﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTron
{
    class Trail : GameObject
    {
        public override void Reset()
        {
            Tron.removeObjects.Add(this);
        }
    }
}
