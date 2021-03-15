using System;
using System.Collections.Generic;

namespace SkynetDota2Mods
{
    [Serializable]
    public class Hero
    {
        public string Name { get; set; }
        public string Taunt { get; set; }
        public List<Items> Bundles { get; set; } = new List<Items>();

    }
}