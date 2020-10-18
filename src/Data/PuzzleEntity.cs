using System;
using System.Collections.Generic;
using System.Text;

namespace HGV.Warhorn.Api.Data
{
    public class PuzzleEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }

        public string Name { get; set; }
        public string CreatedBy { get; set; }

        public int UpVotes { get; set; }
        public int DownVotes { get; set; }

        public DateTime CreatedWhen { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
