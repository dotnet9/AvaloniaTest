using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Todo.Models;

namespace Todo.ViewModels;

internal class AddItemViewModel : ViewModelBase
{
    private string _description = string.Empty;

    public AddItemViewModel()
    {
        var okEnabled = this.WhenAnyValue(
            x => x.Description,
            description => !string.IsNullOrWhiteSpace(description));

        Ok = ReactiveCommand.Create(
            () => new TodoItem { Description = Description },
            okEnabled);

        Cancel = ReactiveCommand.Create(() => Unit.Default);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public ReactiveCommand<Unit, TodoItem> Ok { get; }

    public ReactiveCommand<Unit, Unit> Cancel { get; }
}
