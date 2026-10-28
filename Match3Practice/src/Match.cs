using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Practice.src
{
    public class Match
    {
        public Match()
        {
            Tiles = new List<Tile>();
        }

        public List<Tile> Tiles { get; set; }

        public int MatchValue { get; set; }
    }
}
