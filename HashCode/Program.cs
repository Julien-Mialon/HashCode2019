#define LOOP

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HashCode
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Process.Score(new HashSet<string> {"A", "B"}, new HashSet<string> {"C"});
            Process.Score(new HashSet<string> {"A", "B"}, new HashSet<string> {"B", "C"});
            Process.Score(new HashSet<string> {"A", "B"}, new HashSet<string> {"B", "C", "D"});
            Process.Score(new HashSet<string> {"A", "B"}, new HashSet<string> {"A"});
            Process.Score(new HashSet<string> {"A", "B", "E"}, new HashSet<string> {"C", "E"});
            Process.Score(new HashSet<string> {"A", "B", "E"}, new HashSet<string> {"C", "E", "B"});
            */

            List<string> files = new List<string>
            {
                //"../a_example.txt",
                //"../b_lovely_landscapes.txt",
                //"../c_memorable_moments.txt",
                //"../d_pet_pictures.txt",
                //"../e_shiny_selfies.txt"
            };

            foreach (string file in files)
            {
                var pictures = Read(file);

                //*
                int maxScore = 0;
                for (int i = 0; i < 100; ++i)
                {
                    Console.WriteLine($"Processing: {file}");
                    (List<Slide> result, int score) result = Process.Run(pictures);

                    if (result.score > maxScore)
                    {
                        maxScore = result.score;

                        Console.WriteLine($"Write: {file}");
                        Write(result.result, file);
                    }
                }
                // */

                /*
                Console.WriteLine($"Processing: {file}");
                (List<Slide> result, int score) result = Process.Run(pictures);

                Console.WriteLine($"Write: {file}");
                Write(result.result, file);
                // */
            }
            
            List<string> files2 = new List<string>
            {
                //"../a_example.txt",
                //"../b_lovely_landscapes.txt",
                //"../c_memorable_moments.txt",
                "../d_pet_pictures.txt",
                "../e_shiny_selfies.txt"
            };
            
            foreach (string file in files2)
            {
                var pictures = Read(file);

                /*
                int maxScore = 0;
                for (int i = 0; i < 100; ++i)
                {
                    Console.WriteLine($"Processing: {file}");
                    (List<Slide> result, int score) result = Process.Run(pictures);

                    if (result.score > maxScore)
                    {
                        maxScore = result.score;

                        Console.WriteLine($"Write: {file}");
                        Write(result.result, file);
                    }
                }
                // */

                //*
                Console.WriteLine($"Processing: {file}");
                (List<Slide> result, int score) result = ProcessDiviser.Run(pictures);

                Console.WriteLine($"Write: {file}");
                Write(result.result, file);
                // */
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