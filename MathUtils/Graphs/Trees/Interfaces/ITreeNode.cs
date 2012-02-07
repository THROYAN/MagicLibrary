using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs.Trees
{
    public interface ITreeNode
    {
        List<ITreeNode> Children { get; set; }

        ITreeNode Parent { get; set; }

        ITree Tree { get; }

        ITreeNode GetParent();

        ITreeNode[] GetChildren();

        void AddChild(ITreeNode node);

        void AddTree(ITree tree);

        bool IsChild(ITreeNode node);

        bool IsRoot();

        bool IsLeaf();

        int Degree { get; }

        int Level { get; }

        string ToStringWithChildren();
    }
}
