using System.Collections.Generic;
using System.Linq;

namespace HashCode
{
    class Slide
    {
        public int Id1 { get; set; } = -1;

        public int Id2 { get; set; } = -2;

        public HashSet<string> Tags { get; set; }
        
        public Slide(Picture picture)
        {
            Id1 = picture.Id;

            Tags = picture.Tags;
        }

        public Slide(Picture picture1, Picture picture2)
        {
            Id1 = picture1.Id;
            Id2 = picture2.Id;

            Tags = picture1.Tags.Concat(picture2.Tags).ToHashSet();
        }
    }
}