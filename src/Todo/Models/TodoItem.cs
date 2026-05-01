using ReactiveUI;

namespace Todo.Models;

public class TodoItem : ReactiveObject
{
    private string _description = string.Empty;
    private bool _isChecked;

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            this.RaiseAndSetIfChanged(ref _isChecked, value);
            this.RaisePropertyChanged(nameof(StatusText));
        }
    }

    public string StatusText => IsChecked ? "已完成" : "待处理";
}
