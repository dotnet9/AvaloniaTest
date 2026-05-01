using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Todo.Models;
using Todo.Services;

namespace Todo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _content;

    public MainWindowViewModel(Database db)
    {
        List = new TodoListViewModel(db.GetItems());
        _content = List;

        AddItemCommand = ReactiveCommand.Create(AddItem);
        ObserveItems();
        RefreshSummary();
    }

    public ViewModelBase Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    public TodoListViewModel List { get; }

    public ReactiveCommand<Unit, Unit> AddItemCommand { get; }

    public int TotalItems => List.Items.Count;

    public int CompletedItems => List.Items.Count(item => item.IsChecked);

    public int PendingItems => TotalItems - CompletedItems;

    public bool IsAddingItem => Content is AddItemViewModel;

    public string CurrentModeLabel => IsAddingItem ? "编辑中" : "概览";

    public string HeaderTitle => IsAddingItem ? "新建待办事项" : "今日待办";

    public string HeaderSubtitle => IsAddingItem
        ? "把接下来要做的事情快速记下来。"
        : PendingItems == 0
            ? "当前事项都已处理完成，节奏很好。"
            : $"还有 {PendingItems} 项待处理，继续保持专注。";

    private void AddItem()
    {
        var vm = new AddItemViewModel();

        vm.Ok
            .Take(1)
            .Subscribe(Observer.Create<TodoItem>(model =>
            {
                List.Items.Add(model);
                Content = List;
                RefreshSummary();
            }));

        vm.Cancel
            .Take(1)
            .Subscribe(Observer.Create<Unit>(_ =>
            {
                Content = List;
                RefreshSummary();
            }));

        Content = vm;
        RefreshSummary();
    }

    private void ObserveItems()
    {
        List.Items.CollectionChanged += OnItemsChanged;

        foreach (var item in List.Items)
        {
            item.PropertyChanged += OnTodoItemPropertyChanged;
        }
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (TodoItem item in e.NewItems)
            {
                item.PropertyChanged += OnTodoItemPropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (TodoItem item in e.OldItems)
            {
                item.PropertyChanged -= OnTodoItemPropertyChanged;
            }
        }

        RefreshSummary();
    }

    private void OnTodoItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TodoItem.IsChecked) or nameof(TodoItem.Description))
        {
            RefreshSummary();
        }
    }

    private void RefreshSummary()
    {
        this.RaisePropertyChanged(nameof(TotalItems));
        this.RaisePropertyChanged(nameof(CompletedItems));
        this.RaisePropertyChanged(nameof(PendingItems));
        this.RaisePropertyChanged(nameof(IsAddingItem));
        this.RaisePropertyChanged(nameof(CurrentModeLabel));
        this.RaisePropertyChanged(nameof(HeaderTitle));
        this.RaisePropertyChanged(nameof(HeaderSubtitle));
    }
}
