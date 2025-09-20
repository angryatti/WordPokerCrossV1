﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace WordPokerCrossV1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";

}