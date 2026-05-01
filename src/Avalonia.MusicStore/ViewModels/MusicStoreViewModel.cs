using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Threading;
using Avalonia.MusicStore.Models;
using ReactiveUI;

namespace Avalonia.MusicStore.ViewModels;

public class MusicStoreViewModel : ViewModelBase
{
    private bool _isBusy;
    private string? _searchText;
    private AlbumViewModel? _selectedAlbum;
    private CancellationTokenSource? _cancellationTokenSource;

    public MusicStoreViewModel()
    {
        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(400))
            .Subscribe(searchText =>
                Dispatcher.UIThread.Post(() => DoSearch(searchText ?? string.Empty)));

        var canBuy = this.WhenAnyValue(x => x.SelectedAlbum)
            .Select(selectedAlbum => selectedAlbum is not null);

        BuyMusicCommand = ReactiveCommand.Create(
            () => SelectedAlbum,
            canBuy);

        BuyMusicCommand.Subscribe(album => BuyRequested?.Invoke(album));
    }

    public event Action<AlbumViewModel?>? BuyRequested;

    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public AlbumViewModel? SelectedAlbum
    {
        get => _selectedAlbum;
        set => SetProperty(ref _selectedAlbum, value);
    }

    public ObservableCollection<AlbumViewModel> SearchResults { get; } = [];

    public ReactiveCommand<Unit, AlbumViewModel?> BuyMusicCommand { get; }

    public bool HasResults => SearchResults.Count > 0;

    public string SearchSummary => string.IsNullOrWhiteSpace(SearchText)
        ? "输入歌手名或专辑名开始搜索。"
        : IsBusy
            ? "正在检索在线专辑目录..."
            : HasResults
                ? $"共找到 {SearchResults.Count} 张专辑，可选择后加入收藏。"
                : "没有找到匹配结果，试试换一个关键词。";

    private async void DoSearch(string searchText)
    {
        IsBusy = true;
        SearchResults.Clear();
        RefreshSearchState();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var albums = await Album.SearchAsync(searchText);

            foreach (var album in albums)
            {
                SearchResults.Add(new AlbumViewModel(album));
            }

            RefreshSearchState();

            if (!cancellationToken.IsCancellationRequested)
            {
                LoadCovers(cancellationToken);
            }
        }

        IsBusy = false;
        RefreshSearchState();
    }

    private async void LoadCovers(CancellationToken cancellationToken)
    {
        foreach (var album in SearchResults.ToList())
        {
            await album.LoadCover();

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }

    private void RefreshSearchState()
    {
        this.RaisePropertyChanged(nameof(HasResults));
        this.RaisePropertyChanged(nameof(SearchSummary));
    }
}
