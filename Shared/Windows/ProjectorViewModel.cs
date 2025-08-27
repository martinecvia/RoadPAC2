using System;
using System.Collections.Generic;
using System.Text;

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
                            Name = "XHB 1",
                            Children =
                            {
                                new TreeItem
                                {
                                    Name = "V43",
                                    Children =
                                    {
                                        new TreeItem
                                        {
                                            Name = "Vytyčení",
                                            Children =
                                            {
                                                new TreeItem { Name = "V47x: \"Test\"" },
                                                new TreeItem { Name = "V47x: \"Test2\"" }
                                            }
                                        },
                                        new TreeItem
                                        {
                                            Name = "Spojené příčné řezy",
                                            Children =
                                            {
                                                new TreeItem { Name = "V91: \"Test\"" },
                                                new TreeItem { Name = "V91: \"Test2\"" }
                                            }
                                        },
                                                                            new TreeItem
                                        {
                                            Name = "Podklady pro IFC",
                                            Children =
                                            {
                                                new TreeItem { Name = "V94: \"Test\"" },
                                                new TreeItem { Name = "V94: \"Test2\"" }
                                            }
                                        }
                                    }
                                },
                                new TreeItem
                                {
                                    Name = "V51"
                                },
                                new TreeItem
                                {
                                    Name = "XNI"
                                },
                            }
                        },
                        new TreeItem
                        {
                            Name = "XHB 2",
                            Children =
                            {
                                new TreeItem
                                {
                                    Name = "V43",
                                    Children =
                                    {
                                        new TreeItem
                                        {
                                            Name = "Vytyčení",
                                            Children =
                                            {
                                                new TreeItem { Name = "V47x: \"Test\"" },
                                                new TreeItem { Name = "V47x: \"Test2\"" }
                                            }
                                        },
                                        new TreeItem
                                        {
                                            Name = "Spojené příčné řezy",
                                            Children =
                                            {
                                                new TreeItem { Name = "V91: \"Test\"" },
                                                new TreeItem { Name = "V91: \"Test2\"" }
                                            }
                                        },
                                                                            new TreeItem
                                        {
                                            Name = "Podklady pro IFC",
                                            Children =
                                            {
                                                new TreeItem { Name = "V94: \"Test\"" },
                                                new TreeItem { Name = "V94: \"Test2\"" }
                                            }
                                        }
                                    }
                                },
                                new TreeItem
                                {
                                    Name = "V51"
                                },
                                new TreeItem
                                {
                                    Name = "XNI"
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
