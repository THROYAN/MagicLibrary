using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MagicLibrary.MathUtils.Graphs.Trees
{
    public interface ITree : IEnumerable
    {
        ITreeNode Root { get; }

        void AddTreeNode(object nodeValue, object parentNodeValue = null);

        void AddTree(ITree tree, object parentNodeValue = null);
    }
}
