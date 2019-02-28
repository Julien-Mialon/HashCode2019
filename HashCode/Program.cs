using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> files = new List<string>
            {
                "../a_example.txt",
                "../c_memorable_moments.txt"
            };

            foreach (string file in files)
            {
                var pictures = Read(file);

                var slides = Process.Run(pictures);

                Write(slides, file);
            }
        }

        static void Write(List<Slide> slides, string name)
        {
            List<string> result = new List<string>
            {
                slides.Count.ToString()
            };


            result.AddRange(slides.Select(x =>
            {
                if (x.Id2 >= 0)
                {
                    return $"{x.Id1} {x.Id2}";
                }

                return x.Id1.ToString();
            }));

            File.WriteAllLines($"{name}.out", result);
        }
        
        static List<Picture> Read(string file)
        {
            string[] lines = File.ReadAllLines(file);
            List<Picture> result = new List<Picture>(100_000);

            for (int i = 1; i < lines.Length; ++i)
            {
                var columns = lines[i].Split(' ');

                var orientation = columns[0] == "H" ? Orientation.Horizontal : Orientation.Vertical;

                Picture p = new Picture
                {
                    Id = i - 1,
                    Orientation = orientation,
                    Tags = columns.Skip(2).ToHashSet(),
                };

                result.Add(p);
            }

            return result;
        }
    }
}