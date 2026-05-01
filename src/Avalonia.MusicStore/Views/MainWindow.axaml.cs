using Avalonia.Controls;
using Avalonia.MusicStore.ViewModels;
using System.Threading.Tasks;

namespace Avalonia.MusicStore.Views;

public partial class MainWindow : Window
{
    private bool _loaded;

    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, System.EventArgs e)
    {
        if (_loaded || DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        _loaded = true;
        viewModel.OpenStoreAsync = OpenStoreWindowAsync;
        await viewModel.LoadAlbumsAsync();
    }

    private async Task<AlbumViewModel?> OpenStoreWindowAsync(MusicStoreViewModel storeViewModel)
    {
        var dialog = new MusicStoreWindow
        {
            DataContext = storeViewModel
        };

        return await dialog.ShowDialog<AlbumViewModel?>(this);
    }
}
