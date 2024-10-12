using Data_Transfer.ViewModels;
using System.Windows;
namespace Data_Transfer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ProcessViewModel();
        }

        /// <summary>
        /// Обработчик события закрытия окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProcessViewModel processViewModel = (ProcessViewModel)btnSaveDocumentInDB.DataContext;
            if (processViewModel.IsDataModified)
            {
                bool? isNeedSave = BaseViewModel.AskUserNeedSaveData("Программа содержит несохраненные данные. Сохранить перед закрытием?");
                if (isNeedSave == true)
                {
                    processViewModel.SaveDataInDataBaseCommand.Execute(null);
                }
                else if (isNeedSave == null)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}