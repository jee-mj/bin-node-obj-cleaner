using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

        Collection<string> BinDirsCollection = new();
        Collection<string> NodeDirsCollection = new();
        Collection<string> ObjDirsCollection = new();

        string node = "node_modules";
        string bin = "bin";
        string obj = "obj";

        // if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) -- should do this at the end (exclude Library)

        var AllDirectories = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
        var ObjBinDirs = AllDirectories.Where(d => !node.Any(s => d.Contains(s)));
        var NodeDirs = AllDirectories.Where(d => node.Any(s => d.Contains(s)));

        foreach (var dir in ObjBinDirs)
        { // This is O(n)

            foreach (var binDir in Directory.GetDirectories(dir, bin))
            { // This is O(n)
                if (Directory.GetDirectories(binDir).Count()>0)
                { // This is O(1)
                    BinDirsCollection.Add(binDir);
                }
            }

            foreach (var objDir in Directory.GetDirectories(dir, obj))
            { // This is O(n)
                if (Directory.GetDirectories(objDir).Count() > 0)
                { // This is O(1)
                    ObjDirsCollection.Add(objDir);
                }
            }
        }

        foreach (var dir in NodeDirs)
        {
            foreach (var nodeDir in Directory.GetDirectories(dir, node))
            { // This is O(n)
                if (!Regex.IsMatch(nodeDir, "node_modules."))
                { // This is O(1)
                    NodeDirsCollection.Add(nodeDir);
                }
            }
        }

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
        { // This is O(3)
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