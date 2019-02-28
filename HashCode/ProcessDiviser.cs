using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HashCode
{
    class ProcessDiviser
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

            

            int count = 32;

            int itemCount = slidePool.Length / count;

            int offset = 0;
            for (int i = 0; i < count; ++i)
            {
                Console.WriteLine($"Iteration: {i}");
                List<Slide> group = new List<Slide>();

                if (i == count - 1)
                {
                    group.AddRange(slidePool.Skip(offset));
                }
                else
                {
                    group.AddRange(slidePool.Skip(offset).Take(itemCount));
                }

                Console.WriteLine("graph");
                var graph = BuildGraph(group.ToArray());
                Console.WriteLine("process"         );
                var result = Parcours(graph);
                
                slides.AddRange(result.result);
                
                offset += itemCount;
            }
            
            /*
            Node[] nodes = BuildGraph(slidePool);
            
            return Parcours(nodes);
            */
            return (slides, 100);
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
            //*
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
            // */
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
            // */
            
            #endregion

            Console.WriteLine($"Graph build: {watcher.ElapsedMilliseconds}ms");

            return nodes;
        }

        private static HashSet<Slide> AssembleVerticals(HashSet<Picture> pictures)
        {
            HashSet<Slide> slides = new HashSet<Slide>(pictures.Count / 2);
            List<Picture> pictureList = new List<Picture>(pictures);

            for (int i = 0; i < pictureList.Count; i += 2)
            {
                slides.Add(new Slide(pictureList[i], pictureList[i + 1]));
            }
            
            /*
            HashSet<Slide> slides = new HashSet<Slide>(pictures.Count / 2);

            List<Picture> pictureList = new List<Picture>(pictures);
            bool[] used = new bool[pictures.Count];
            for (var i = 0; i < used.Length; i++)
            {
                used[i] = false;
            }

            int remainingCount = pictures.Count;
            int nextOffset = 0;

            while (remainingCount > 1)
            {
                for (; used[nextOffset]; nextOffset++) ;

                Console.WriteLine(remainingCount);
                Picture current = pictureList[nextOffset];
                used[nextOffset] = true;
                nextOffset++;
                
                int minScore = int.MaxValue;
                Picture bestPicture = null;
                int bestIndex = 0;
                
                for (var i = nextOffset; i < pictureList.Count; i++)
                {
                    Picture picture = pictureList[i];
                    int score = ScoreVerticalAssociation(current.Tags, picture.Tags);

                    if (score < minScore)
                    {
                        minScore = score;
                        bestPicture = picture;
                        bestIndex = i;
                    }
                }

                used[bestIndex] = true;
                slides.Add(new Slide(current, bestPicture));
                
                remainingCount -= 2;
            }
            */
            /*
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
            */
            return slides;
        }

        public static int ScoreVerticalAssociation(HashSet<string> left, HashSet<string> right)
        {
            int res = 0;
            foreach (var s in left)
            {
                if (right.Contains(s))
                {
                    res++;
                }
            }

            return res;
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
}