using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data_Transfer
{
    /// <summary>
    /// Базовый функционал для всех Model и ViewModel.
    /// </summary>
    public abstract class BaseMVVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие, оповещающие при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Имя свойства, значение которого было изменено.</param>
        private protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
