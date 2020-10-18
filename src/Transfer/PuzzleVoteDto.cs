using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Warhorn.Api.Transfer
{
    public class PuzzleVoteDTO
    {
        public string Id { get; set; }
        public string PuzzleId { get; set; }
        public int Vote { get; set; }
    }
}
