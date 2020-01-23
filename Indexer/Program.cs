using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Indexer
{
    class Index : IEquatable<Index>, IComparable<Index>
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Extention { get; set; }

        public Index() {  }

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

        public Position() {  }
    }

    class Program
    {
        static List<Index> GetFiles(DirectoryInfo directoryInfo)
        {
            List<Index> indices = new List<Index>();
            DirectoryInfo[] subDirectoryInfo = directoryInfo.GetDirectories();

            /*FileIOPermission filePermission = new FileIOPermission(FileIOPermissionAccess.Read, directoryInfo.FullName);
            if (filePermission.AllFiles == FileIOPermissionAccess.NoAccess)
            {
                Console.WriteLine("Permission to: {0} denied", directoryInfo.FullName);
                subDirectoryInfo = null;
                //return indices;
            }
            else
            {
                Console.WriteLine("Permission to: {0} granted", directoryInfo.FullName);  
            }*/

            try
            {
                var files = directoryInfo.EnumerateFiles("*.exe", SearchOption.TopDirectoryOnly);
                foreach (var item in files)
                {
                    indices.Add(new Index()
                    {
                        FileName = item.Name,
                        FilePath = item.FullName,
                        Extention = item.Extension
                    });
                }

                if (subDirectoryInfo == null)
                {
                    return indices;
                }
                for (int i = 0; i < subDirectoryInfo.Count(); i++)
                {
                    indices.AddRange(GetFiles(subDirectoryInfo[i]));
                }
            }
            catch (UnauthorizedAccessException)
            {
                return indices;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            
            return indices;
        }

        static void Main(string[] args)
        {
            List<Index> indices = new List<Index>();
            List<string> directories = new List<string>() { @"C:\Program Files" , @"C:\Program Files (x86)" ,
                @"C:\Users\Andreas", @"C:\Steam\steamapps\common", @"D:\", @"E:\", @"F:\", @"G:\", @"H:\" };

            foreach (var item in directories)
            {
                Console.WriteLine("Directory: {0}", item);
                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                indices.AddRange(GetFiles(directoryInfo));
            }

            indices.Sort();
            char lastChar = char.ToUpper(indices[0].FileName.First());
            List<Position> positions = new List<Position>() { new Position() { Character = lastChar, PositionNr = 0 } };
            File.WriteAllText(@"C:\Users\Andreas\Desktop\IndexPositions.json", System.Text.Json.JsonSerializer.Serialize(positions) + Environment.NewLine);

            for (int i = 0; i < indices.Count; i++)
            {
                if (lastChar != char.ToUpper(indices[i].FileName.First()))
                {
                    lastChar = char.ToUpper(indices[i].FileName.First());
                    if (positions.Where(x => x.Character == lastChar).ToList().Count == 0)
                    {                        
                        positions.Add(new Position() { Character = lastChar, PositionNr = i });
                        File.AppendAllText(@"C:\Users\Andreas\Desktop\IndexPositions.json", System.Text.Json.JsonSerializer.Serialize(positions.Last()) + Environment.NewLine);
                    }                    
                }
                File.AppendAllText(@"C:\Users\Andreas\Desktop\Index.txt", indices[i].ToString() + Environment.NewLine);
                File.AppendAllText(@"C:\Users\Andreas\Desktop\Index.json", indices[i].ToJSON() + Environment.NewLine);
            }
            Console.Write("Done indexing");
            Console.ReadLine();
            return;
        }
    }
}
