using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using System.Windows.Input;

namespace GOLD.Builder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string grammar;
        public string Grammar
        {
            get => grammar;
            set => this.RaiseAndSetIfChanged(ref grammar, value);
        }


        public MainWindowViewModel()
        {
        }

        public void OnContinueClickCommand()
        {
            BuilderUI.GrammarText = grammar;
            BuilderUI.PerformNextAction();
        }
    }
}
