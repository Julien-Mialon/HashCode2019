using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HashCode
{
    class Process
    {
        public static (List<Slide> result, int score) Run(List<Picture> pictures)
        {
            List<Slide> slides = new List<Slide>();

            Console.WriteLine("Construct slides");
            Slide[] slidePool = pictures
                .Where(x => x.Orientation == Orientation.Horizontal)
                .Select(x => new Slide(x))
                .Concat(AssembleVerticals(pictures.Where(x => x.Orientation == Orientation.Vertical).ToHashSet()))
                .ToArray();

            /*
            bool[] used = new bool[slidePool.Length];
            for (var i = 0; i < slidePool.Length; i++)
            {
                used[i] = false;
            }

            Slide current = slidePool.First();
            slides.Add(current);
            used[0] = true;

            int remainingCount = used.Length - 1;
            */
            Node[] nodes = BuildGraph(slidePool);
            
            /*
            Console.WriteLine("Find slides");
            while (remainingCount > 0)
            {
                int maxScore = -1;
                int maxIndex = -1;

                Console.WriteLine($"Remaining: {remainingCount}");
                for (var i = 0; i < slidePool.Length; i++)
                {
                    if (used[i])
                    {
                        continue;
                    }
                    
                    int score = Score(current.Tags, slidePool[i].Tags);
                    //Console.WriteLine($"Score: {score}");

                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxIndex = i;
                    }
                }
                
                slides.Add(slidePool[maxIndex]);
                used[maxIndex] = true;
                remainingCount--;
            }
            */

            return Parcours(nodes);
        }

        private static (List<Slide> result, int score) Parcours(Node[] nodes)
        {
            List<Slide> result = new List<Slide>();
            int expectedScore = 0;
            
            Random random = new Random((int)DateTime.Now.Ticks);
            
            Node current = nodes[random.Next(nodes.Length)];
            current.Slide.Used = true;
            result.Add(current.Slide);

            int remainingCount = nodes.Length - 1;

            while (remainingCount > 0)
            {
                int max = -1;
                Node selectedNext = null;
                foreach (var (next, score) in current.Edges)
                {
                    if (next.Used)
                    {
                        continue;
                    }

                    if (score > max)
                    {
                        max = score;
                        selectedNext = next;
                    }
                }

                if (selectedNext != null)
                {
                    expectedScore += max;
                }

                if (selectedNext is null)
                {
                    for (var i = 0; i < 100; ++i)
                    {
                        int index = random.Next(nodes.Length);
                        if (!nodes[index].Used)
                        {
                            selectedNext = nodes[index];
                            break;
                        }
                    }

                    if (selectedNext is null)
                    {
                        for (var i = 0; i < nodes.Length; i++)
                        {
                            if (!nodes[i].Used)
                            {
                                selectedNext = nodes[i];
                                break;
                            }
                        }
                    }
                }

                if (selectedNext is null)
                {
                    break;
                }

                selectedNext.Slide.Used = true;
                result.Add(selectedNext.Slide);

                current = selectedNext;
                
                remainingCount--;
            }

            Console.WriteLine($"Score: {expectedScore}");
            return (result, expectedScore);
        }

        private static Node[] BuildGraph(Slide[] slides)
        {
            Stopwatch watcher = Stopwatch.StartNew();
            Node[] nodes = slides.Select(x => new Node(x)).ToArray();
            
            
            
            #region implem B
            
            Dictionary<Slide, Node> nodeDictionary = nodes.ToDictionary(x => x.Slide, x => x);
            Dictionary<string, List<Node>> result = slides
                .SelectMany(slide => slide.Tags.Select(tag => (slide, tag)))
                .GroupBy(x => x.Item2)
                .ToDictionary(x => x.Key, x => x.Select(y => nodeDictionary[y.Item1]).ToList());
            
            for(int i = 0 ; i < nodes.Length ; ++i)
            {
                Node source = nodes[i];
             
                HashSet<Node> used = new HashSet<Node>();
                foreach (var tag in source.Slide.Tags)
                {
                    foreach (Node dest in result[tag])
                    {
                        if (used.Contains(dest))
                        {
                            continue;
                        }

                        used.Add(dest);
                        
                        int score = Score(source.Slide.Tags, dest.Slide.Tags);

                        if (score > 0)
                        {
                            source.Edges.Add((dest, score));
                        }
                    }
                }
            }
            
            #endregion
            
            #region implem A / C
            
            /*
            for(int i = 0 ; i < nodes.Length ; ++i)
            {
                //Console.WriteLine($"Assemble: {i}");
                Node source = nodes[i];
                
                for (var j = i+1; j < nodes.Length; j++)
                {
                    Node dest = nodes[j];

                    int score = Score(source.Slide.Tags, dest.Slide.Tags);

                    if (score > 0)
                    {
                        source.Edges.Add((dest, score));
                        dest.Edges.Add((source, score));
                    }
                }
            }
            */
            
            #endregion

            Console.WriteLine($"Graph build: {watcher.ElapsedMilliseconds}ms");

            return nodes;
        }

        private static HashSet<Slide> AssembleVerticals(HashSet<Picture> pictures)
        {
            HashSet<Slide> slides = new HashSet<Slide>(pictures.Count / 2);

            while (pictures.Count > 1)
            {
                var p1 = pictures.First();

                pictures.Remove(p1);

                int minScore = int.MaxValue;
                Picture bestPicture = null;
                foreach (Picture picture in pictures)
                {
                    int score = ScoreVerticalAssociation(p1.Tags, picture.Tags);

                    if (score < minScore)
                    {
                        minScore = score;
                        bestPicture = picture;
                    }
                }

                pictures.Remove(bestPicture);

                slides.Add(new Slide(p1, bestPicture));
            }

            return slides;
        }

        public static int ScoreVerticalAssociation(HashSet<string> left, HashSet<string> right)
        {
            return left.Count(right.Contains);
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
    }

    class Node
    {
        public Slide Slide { get; set; }

        public bool Used => Slide.Used;

        public HashSet<Node> EdgeNodes { get; set; }

        public List<(Node next, int score)> Edges { get; set; }

        public Node(Slide slide)
        {
            Slide = slide;
            Edges = new List<(Node next, int score)>();
            EdgeNodes = new HashSet<Node>();
        }
    }
}