using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.MusicStore.ViewModels;

namespace Avalonia.MusicStore.Views;

public partial class MusicStoreWindow : Window
{
    private MusicStoreViewModel? _viewModel;

    public MusicStoreWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.BuyRequested -= OnBuyRequested;
        }

        _viewModel = DataContext as MusicStoreViewModel;
        if (_viewModel != null)
        {
            _viewModel.BuyRequested += OnBuyRequested;
        }
    }

    private void OnBuyRequested(AlbumViewModel? album)
    {
        Close(album);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
