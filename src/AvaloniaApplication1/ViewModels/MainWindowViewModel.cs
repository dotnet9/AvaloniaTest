using ReactiveUI;
using System;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _greeting = "Welcome to Avalonia!";

    public string Greeting
    {
        get => _greeting;
        set
        {
            this._greeting = value;
            this.RaisePropertyChanged();
        }
    }

    public void ButtonClicked() => this.Greeting = DateTime.Now.ToString();
}