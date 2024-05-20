using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WasHere.Utils
{
    public class Status : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _optimizationInProgress;
        private bool _isAdmin;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool OptimizationInProgress
        {
            get { return _optimizationInProgress; }
            set
            {
                if (_optimizationInProgress != value)
                {
                    _optimizationInProgress = value;
                    OnPropertyChanged(nameof(OptimizationInProgress));
                }
            }
        }

        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    OnPropertyChanged(nameof(IsAdmin));
                }
            }
        }

        public async Task StartOptimizationProcess()
        {
            OptimizationInProgress = true;
            await Task.Delay(5000);
            OptimizationInProgress = false;
        }
    }
}
