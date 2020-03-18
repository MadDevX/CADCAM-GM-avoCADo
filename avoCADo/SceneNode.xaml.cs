﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for SceneNode.xaml
    /// </summary>
    public partial class SceneNode : UserControl
    {
        public Node Node { get; set; }
        public SceneNode()
        {
            InitializeComponent();
            DataContext = Node;
        }
    }
}
