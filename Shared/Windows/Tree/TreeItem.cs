using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Windows.Tree
{
    public class TreeItem
    {
        public string Name { get; set; }
        public List<TreeItem> Children { get; set; } = new List<TreeItem>();
    }
}
