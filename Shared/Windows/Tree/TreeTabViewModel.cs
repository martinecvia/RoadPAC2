using System.Collections.Generic;

namespace Shared.Windows.Tree
{
    public class TreeTabViewModel
    {
        public string Name { get; set; } = string.Empty;
        public List<TreeItem> Item { get; set; }
    }
}
