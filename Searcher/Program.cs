using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Searcher
{
    class Index : IEquatable<Index>, IComparable<Index>
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Extention { get; set; }

        public Index() { }

        public int CompareTo(Index compareMatch)
        {
            if (compareMatch == null)
                return 1;
            else
                return this.FileName.CompareTo(compareMatch.FileName);
        }
        public bool Equals(Index other)
        {
            if (other == null)
                return false;
            else
                return (this.FileName.Equals(other.FileName));
        }
        public override string ToString()
        {
            return (FileName + Environment.NewLine + "\t" + FilePath).Replace(@"\\", @"\");
        }
        public string ToJSON()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
    class Position
    {
        public char Character { get; set; }
        public int PositionNr { get; set; }

        public Position() { }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string[] input = File.ReadAllLines(@"C:\Users\Andreas\Desktop\Index.json");
            List<Index> indices = new List<Index>();
            foreach (string item in input)
            {
                indices.Add((Index)System.Text.Json.JsonSerializer.Deserialize(item, typeof(Index)));
            }
            input = File.ReadAllLines(@"C:\Users\Andreas\Desktop\IndexPositions.json");
            List<Position> positions = new List<Position>();
            foreach (string item in input)
            {
                positions.Add((Position)System.Text.Json.JsonSerializer.Deserialize(item, typeof(Position)));
            }

            if (args.Count() == 0)
            {
                Console.Write("Search string: ");
                string searchString = Console.ReadLine();

                int i = 0;
                for (; i < positions.Count; i++) 
                {
                    if (char.ToUpper(searchString.First()) == positions[i].Character)
                    {
                        i = positions[i].PositionNr;
                        break;
                    }
                }
                
                for (; i < indices.Count; i++)
                {
                    if (searchString == indices[i].FileName)
                    {
                        Console.WriteLine("File is at: {0}", indices[i].FilePath);
                        Console.ReadLine();
                        break;
                    }
                }
            }
            
            
            return;
        }
    }
}
