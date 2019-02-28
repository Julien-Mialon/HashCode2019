using System;
using System.Collections.Generic;
using System.Linq;

namespace HashCode
{
    class Process
    {
        public static List<Slide> Run(List<Picture> pictures)
        {
            List<Slide> slides = new List<Slide>();

            HashSet<Picture> horizontal = pictures.Where(x => x.Orientation == Orientation.Horizontal).ToHashSet();
            HashSet<Picture> vertical = pictures.Where(x => x.Orientation == Orientation.Vertical).ToHashSet();

            Picture first = horizontal.First();
            horizontal.Remove(first);
            Slide slide = new Slide(first);
            slides.Add(slide);

            while (horizontal.Count > 0 || vertical.Count > 0)
            {
                
            }
            
            return slides;
        }

        public static int Score(HashSet<string> left, HashSet<string> right)
        {
            int leftNotRight = 0;
            int leftAndRight = 0;
            int rightNotLeft = 0;
            
            foreach (string s in left)
            {
                if (right.Contains(s))
                {
                    leftAndRight++;
                }
                else
                {
                    leftNotRight++;
                }
            }

            rightNotLeft = right.Count - leftAndRight;

            return Math.Min(leftNotRight, Math.Min(leftAndRight, rightNotLeft));
        }
        
        public static int Score(HashSet<string> left, HashSet<string> right1, HashSet<string> right2)
        {
            int leftNotRight = 0;
            int leftAndRight = 0;a
            int rightNotLeft = 0;
            
            foreach (string s in left)
            {
                if (right1.Contains(s) || right2.Contains(s))
                {
                    leftAndRight++;
                }
                else
                {
                    leftNotRight++;
                }
            }

            int unionCount = right1.Count(right2.Contains);
            
            
            rightNotLeft = right1.Count + right2.Count - leftAndRight - unionCount;

            return Math.Min(leftNotRight, Math.Min(leftAndRight, rightNotLeft));
        }
    }
}