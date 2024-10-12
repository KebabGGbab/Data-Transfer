using System.Windows;
using System.Windows.Input;

namespace Data_Transfer.ViewModels
{
    /// <summary>
    /// Базовая ViewModel для всех ViewModel, в которых требуется сохранять данные.
    /// </summary>
    public abstract class BaseViewModel : BaseMVVM
    {
        private bool isSavingData;

        /// <summary>
        /// Указывает, были ли изменены данные.
        /// </summary>
        public bool IsDataModified { get; private protected set; } = false;

        /// <summary>
        /// Указывает, происходит ли в данный момент сохранение данных в базу данных.
        /// </summary>
        public bool IsSavingData
        {
            get 
            {
                return isSavingData;
            }
            private protected set
            {
                if (isSavingData != value)
                {
                    isSavingData = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Спросить у пользователя о необходимости сохранения данных.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool? AskUserNeedSaveData(string message)
        {
            MessageBoxResult msResult = MessageBox.Show(message, "Требуется подтверждение", MessageBoxButton.YesNoCancel);
            if (msResult == MessageBoxResult.Yes)
            {
                return true;
            }
            else if (msResult == MessageBoxResult.Cancel)
            {
                return null;
            }
            return false;
        }

        /// <summary>
        /// Сообщить пользователю об ошибке
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        private protected static void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
