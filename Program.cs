using System;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;

namespace sd2.module1
{
    class DirectoryDetail
    {
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
        public string DirectoryName { get; set; }

        public DirectoryDetail(int count, long size, string name)
        {
            FileCount = count;
            TotalSize = size;
            DirectoryName = name;
        }
    }

    class TreeNode
    {
        private readonly DirectoryDetail _info;
        private readonly List<TreeNode> _children = new List<TreeNode>();

        public TreeNode(DirectoryDetail dir)
        {
            _info = dir;
        }

        public TreeNode this[int i]
        {
            get { return _children[i]; }
        }

        public TreeNode? Parent { get; private set; }
        public DirectoryDetail Info { get { return _info; } }

        public ReadOnlyCollection<TreeNode> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode AddChild(DirectoryDetail value)
        {
            var node = new TreeNode(value) { Parent = this };
            _children.Add(node);
            return node;
        }
    }

    class Program
    {
        static string GetStartingDirectory()
        {
            string? dir = "";
            while (String.IsNullOrEmpty(dir))
            {
                Console.Write("Enter a directory: ");
                dir = Console.ReadLine();

                if (String.IsNullOrEmpty(dir))
                {
                    continue;
                }

                if (Directory.Exists(dir))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("{0} is not a valid directory.", dir);
                }

                dir = "";
            }
            return dir;
        }

        static long GetDirectorySize(FileInfo[] files)
        {
            long totalFileSize = 0;

            foreach (FileInfo file in files)
            {
                totalFileSize += file.Length;
            }

            return totalFileSize;
        }

        static DirectoryDetail GetDirectoryDetail(DirectoryInfo di)
        {
            FileInfo[] files = di.GetFiles();
            return new DirectoryDetail(
                files.Length,
                GetDirectorySize(files),
                di.Name
            );
        }

        static string SpacingByLevel(int level)
        {
            return new String('-', level * 2);
        }

        static TreeNode BuildDirectoryTree(DirectoryInfo di, int level = 0, TreeNode? fileTree = null)
        {
            TreeNode tree;

            // Create root node
            if (fileTree is null)
            {
                tree = new TreeNode(GetDirectoryDetail(di));
            }
            else
            {
                tree = fileTree;
            }

            // Create children nodes
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                TreeNode child = tree.AddChild(GetDirectoryDetail(dir));
                BuildDirectoryTree(dir, level + 1, child);
            }

            return tree;
        }

        static void PrintTree(TreeNode tree, int level = 0)
        {
            Console.WriteLine(
                "{0} {1}, {2} files, {3} Bytes",
                SpacingByLevel(level),
                tree.Info.DirectoryName,
                tree.Info.FileCount,
                tree.Info.TotalSize
            );

            foreach (TreeNode child in tree.Children)
            {
                PrintTree(child, level + 1);
            }
        }

        static void Main(string[] args)
        {
            string startingDirectory = GetStartingDirectory();
            DirectoryInfo rootInfo = new DirectoryInfo(startingDirectory);
            TreeNode directoryTree = BuildDirectoryTree(rootInfo);
            PrintTree(directoryTree);
        }
    }
}