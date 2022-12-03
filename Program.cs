using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
internal class Program
{
    private static void Main(string[] args)
    {
        string projectDirectory = DirectoryDetails(args[0]);

        RemoveBinAndObj(FindBinAndObj(projectDirectory));
        Console.WriteLine("\n");
        Console.WriteLine("The operation has completed successfully.");
        Console.WriteLine();
    }
    private static string DirectoryDetails(string directory)
    {
        try
        {
            Console.WriteLine();
            Console.WriteLine($"Project root directory:          {directory}");
            Console.WriteLine();
        }
        catch (System.Exception)
        {
            
            throw;
        }
        return directory;
    }
    private static Collection<Collection<string>> FindBinAndObj(string root)
    {
        var AllDirectories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);

        #region bin_dirs
        Collection<string> BinDirsCollection = new();
        string[] node = new string[] {"node_modules"};
        foreach (var dir in AllDirectories.Where(d => !node.Any(s => d.Contains(s))))
        {
            foreach (var binDir in Directory.GetDirectories(dir, @"bin"))
            {
                if (Directory.GetDirectories(binDir).Count()>0)
                {
                    BinDirsCollection.Add(binDir);
                }
            }
        }
        #endregion

        #region node_modules
        Collection<string> NodeDirsCollection = new();
        Regex test = new Regex("node_modules.");
        foreach (var dir in AllDirectories)
        {
            var containsNode = Directory.GetDirectories(dir, "node_modules");
            foreach (var item in containsNode)
            {
                if(!Regex.IsMatch(item, "node_modules."))
                {
                    NodeDirsCollection.Add(item);
                }
            }
        }
        #endregion

        #region obj_dirs
        Collection<string> ObjDirsCollection = new();
        foreach (var dir in AllDirectories)
        {
            foreach (var objDir in Directory.GetDirectories(dir, @"obj"))
            {
                if (Directory.GetDirectories(objDir).Count()>0)
                {
                    ObjDirsCollection.Add(objDir);
                }
            }
        }
        #endregion

        #region return
        Collection<Collection<string>> AllDirsCollection = new()
        {
            BinDirsCollection, NodeDirsCollection, ObjDirsCollection
        };
        return AllDirsCollection;
        #endregion
    }
    private static void RemoveBinAndObj(Collection<Collection<string>> dirCollection)
    {
        Collection<string> dirTypes = new() {"bin", "node", "obj"};
        for (int i = 1; i <= 3; i++)
        {
            foreach (var collection in dirCollection[i-1])
            {
                foreach (var delete in Directory.GetFiles(collection, "*", SearchOption.AllDirectories))
                {
                    try
                        {
                            File.Delete(delete); // delete all files
                        }
                        catch (Exception)
                        {
                            System.Console.WriteLine(@$"Failed Deleting: {delete}");
                            throw;
                        }
                }

                try
                    {
                        Directory.Delete(collection, true); // recursively delete all directories
                    }
                    catch (Exception)
                    {
                        System.Console.WriteLine(@$"Failed Deleting: {collection}");
                        throw;
                    }
            }
            System.Console.WriteLine(@$"Successfully deleted all {dirTypes[i-1]} folders.");
        }
    }
}