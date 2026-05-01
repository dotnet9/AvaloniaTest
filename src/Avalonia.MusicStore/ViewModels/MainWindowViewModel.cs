using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.MusicStore.Models;
using ReactiveUI;

namespace Avalonia.MusicStore.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool _collectionEmpty = true;

    public MainWindowViewModel()
    {
        BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new MusicStoreViewModel();
            var result = await OpenStoreAsync(store);

            if (result != null)
            {
                Albums.Add(result);
                await result.SaveToDiskAsync();
                UpdateCollectionState();
            }
        });
    }

    public bool CollectionEmpty
    {
        get => _collectionEmpty;
        private set => SetProperty(ref _collectionEmpty, value);
    }

    public ObservableCollection<AlbumViewModel> Albums { get; } = [];

    public ReactiveCommand<Unit, Unit> BuyMusicCommand { get; }

    public int AlbumCount => Albums.Count;

    public string HeroTitle => CollectionEmpty ? "开始建立你的收藏" : "已保存的专辑收藏";

    public string HeroSubtitle => CollectionEmpty
        ? "从商店里搜索喜欢的音乐，把第一张专辑加入桌面收藏。"
        : $"当前已保存 {AlbumCount} 张专辑，可以继续扩充你的精选库。";

    public Func<MusicStoreViewModel, Task<AlbumViewModel?>> OpenStoreAsync { get; set; } =
        _ => Task.FromResult<AlbumViewModel?>(null);

    public async Task LoadAlbumsAsync()
    {
        var albums = (await Album.LoadCachedAsync()).Select(x => new AlbumViewModel(x));

        foreach (var album in albums)
        {
            Albums.Add(album);
        }

        foreach (var album in Albums.ToList())
        {
            await album.LoadCover();
        }

        UpdateCollectionState();
    }

    private void UpdateCollectionState()
    {
        CollectionEmpty = Albums.Count == 0;
        this.RaisePropertyChanged(nameof(AlbumCount));
        this.RaisePropertyChanged(nameof(HeroTitle));
        this.RaisePropertyChanged(nameof(HeroSubtitle));
    }
}
