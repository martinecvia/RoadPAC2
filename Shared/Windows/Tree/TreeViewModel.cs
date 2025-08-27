using System.Collections.Generic;

namespace Shared.Windows.Tree
{
    public class TabViewModel
    {
        public string Header { get; set; } = string.Empty;
        public List<TreeItem> Items { get; set; }
    }
}
