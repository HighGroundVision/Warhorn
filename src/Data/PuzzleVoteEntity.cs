using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Warhorn.Api.Data
{
    public class PuzzleVoteEntity
    {
        public string Id { get; set; }
        public string PuzzleId { get; set; }

        public string UserId { get; set; }
    
        public int Vote { get; set; }

        public DateTime Date { get; set; }
    }
}
