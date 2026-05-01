using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Todo.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        this.RaiseAndSetIfChanged(ref field, value, propertyName);
        return true;
    }
}
