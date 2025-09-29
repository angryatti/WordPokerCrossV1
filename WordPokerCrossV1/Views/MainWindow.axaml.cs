using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace WordPokerCrossV1.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainView mainView = new MainView();


        mainView.Img1.Width = 400;
        mainView.Img1.Height = 800;

    
        this.Content = mainView;
    }
}