using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Windows.Tree
{
    public class TreeItem
    {
        public string Name { get; set; } = string.Empty;
        public List<TreeItem> Item { get; set; } = new List<TreeItem>();
    }
}