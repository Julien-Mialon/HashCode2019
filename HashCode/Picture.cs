using System.Collections.Generic;

namespace HashCode
{
    class Picture
    {
        public int Id { get; set; }
        
        public Orientation Orientation { get; set; }

        public HashSet<string> Tags { get; set; }
    }
}