using System;
using System.Collections.Generic;
using System.Text;

using ZwSoft.ZwCAD.Windows;

using System.Windows;
using Shared.Windows.Tree;

namespace Shared.Windows
{
    public class ProjectorViewModel
    {
        public List<TabViewModel> Tabs { get; set; }

        public ProjectorViewModel()
        {
            Tabs = new List<TabViewModel>
            {
                new TabViewModel
                {
                    Header = "Tab1",
                    Items = new List<TreeItem>
                    {
                        new TreeItem
                        {
                            Name = "Trasa A",
                            Children =
                            {
                                new TreeItem
                                {
                                    Name = "Koridor A1",
                                    Children =
                                    {
                                        new TreeItem
                                        {
                                            Name = "Vytyčení",
                                            Children =
                                            {
                                                new TreeItem { Name = "Vytyčení1: \"Test\"" },
                                                new TreeItem { Name = "Vytyčení2: \"Test2\"" }
                                            }
                                        },
                                        new TreeItem
                                        {
                                            Name = "Spojené příčné řezy",
                                            Children =
                                            {
                                                new TreeItem { Name = "SPR1: \"Test\"" },
                                                new TreeItem { Name = "SPR2: \"Test2\"" }
                                            }
                                        },
                                                                            new TreeItem
                                        {
                                            Name = "Podklady pro IFC",
                                            Children =
                                            {
                                                new TreeItem { Name = "IFC1: \"Test\"" },
                                                new TreeItem { Name = "IFC2: \"Test2\"" }
                                            }
                                        }
                                    }
                                },
                                new TreeItem
                                {
                                    Name = "Příčné řezy"
                                },
                                new TreeItem
                                {
                                    Name = "Niveleta"
                                },
                            }
                        },
                        new TreeItem
                        {
                            Name = "Trasa B",
                            Children =
                            {
                                new TreeItem
                                {
                                    Name = "Koridor B1",
                                    Children =
                                    {
                                        new TreeItem
                                        {
                                            Name = "Vytyčení",
                                            Children =
                                            {
                                                new TreeItem { Name = "Vytyčení1: \"Foo\"" },
                                                new TreeItem { Name = "Vytyčení2: \"Bar\"" }
                                            }
                                        },
                                        new TreeItem
                                        {
                                            Name = "Spojené příčné řezy",
                                            Children =
                                            {
                                                new TreeItem { Name = "SPR1: \"Test\"" },
                                                new TreeItem { Name = "SPR2: \"Test2\"" }
                                            }
                                        },
                                        new TreeItem
                                        {
                                            Name = "Podklady pro IFC",
                                            Children =
                                            {
                                                new TreeItem { Name = "IFC1: \"Test\"" },
                                                new TreeItem { Name = "IFC2: \"Test2\"" }
                                            }
                                        }
                                    }
                                },
                                new TreeItem
                                {
                                    Name = "Příčné řezy"
                                },
                                new TreeItem
                                {
                                    Name = "Niveleta"
                                },
                            }
                        }
                    }
                },
                new TabViewModel
                {
                    Header = "Tab2",
                    Items = new List<TreeItem>
                    {
                        new TreeItem { Name = "Další kořenový uzel" }
                    }
                }
            };
        }
    }
}
