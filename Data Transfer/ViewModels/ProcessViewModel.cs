using Data_Transfer.EF;
using Data_Transfer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace Data_Transfer.ViewModels
{
    public class ProcessViewModel : BaseViewModel
    {
        private ObservableCollection<ProcessModel> processModels = [];
        private string? openFile = null;
        private string? encodingFile;
        private string? separator;
        private RelayCommand? openFileCommand;
        private RelayCommand? saveDataInDataBaseCommand;

        /// <summary>
        /// Коллекция моделей, хранящих информацию о процессах.
        /// </summary>
        public ObservableCollection<ProcessModel> ProcessModels
        {
            get
            {
                return processModels;
            }
            private set
            {
                processModels = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Абсолютный путь к прочитанному файлу.
        /// </summary>
        public string? OpenFile
        {
            get
            {
                return openFile;
            }
            set
            {
                openFile = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Кодировка, в которой прочитан файл.
        /// </summary>
        public string? EncodingFile
        {
            get
            {
                return encodingFile;
            }
            set
            {
                encodingFile = value;
                OnPropertyChanged();
            }
        }
       
        /// <summary>
        /// Разделитель, который следует использовать для различения столбцов в .csv и .txt файлах
        /// </summary>
        public string? Separator
        {
            get
            {
                return separator;
            }
            set
            {
                separator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Команда для чтения дданных из файла.
        /// </summary>
        public RelayCommand ReadFileCommand
        {
            get
            {
                return openFileCommand ??= new RelayCommand(ExecuteReadFileCommand, CanExecuteOpenFileCommand);
            }
        }

        /// <summary>
        /// Команда для сохранения прочитанных данных из файла в базу данных.
        /// </summary>
        public RelayCommand SaveDataInDataBaseCommand
        {
            get
            {
                return saveDataInDataBaseCommand ??= new RelayCommand(ExecuteSaveDataInDataBaseCommandAsync, CanExecuteSaveDataInDataBaseCommand);
            }
        }

        /// <summary>
        /// Инициализирует класс с данными из файла конфигурации.
        /// </summary>
        public ProcessViewModel()
        {
            IConfigurationSection config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetRequiredSection("AppSettings");
            encodingFile = config.GetRequiredSection("DefaultEncodingFileOpen").Value;
            separator = config.GetRequiredSection("DefaultSeparatorColumn").Value;
        }

        /// <summary>
        /// Вызывает команду ReadFileCommand
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteReadFileCommand(object? parameter)
        {
            if (IsDataModified)
            {
                bool? isNeedSave = AskUserNeedSaveData("Программа содержит несохраненные данные. Сохранить перед загрузкой данных?");
                if (isNeedSave == true)
                {
                    SaveDataInDataBaseCommand?.Execute(null);
                }
                else if (isNeedSave == null)
                {
                    return;
                }
            }
            OpenFileDialog dialog = new()
            {
                Filter = "Текстовые файлы (*.csv; *.txt)|*.csv;*.txt|Все файлы (*.*)|*.*"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                EncodingFile = "windows-1251";
                OpenFile = dialog.FileName;
                try
                {
                    ProcessModels = new ObservableCollection<ProcessModel>(ProcessModel.GetProcess(OpenFile, EncodingFile, ";") ?? []);
                    IsDataModified = true;
                }
                catch(Exception error)
                {
                    if (error.Message.Contains("Файл не найден."))
                    {
                        ShowError($"Не удалось найти файл: {OpenFile}");
                    }
                    else if (error.Message.Contains("Первая строка не должна быть пустой, она должна содержать имена столбцов.") ||
                             error.Message.Contains("Не найдено соответствующее свойство для столбца"))
                    {
                        ShowError("Ошибка со столбцами файла.");
                    }
                    else if (error.Message.Contains("Расширение файла должно быть '.csv'"))
                    {
                        ShowError("Расширение файла должно быть '.csv'");
                    }
                    else if (error.Message.Contains("The process cannot access the file") && error.Message.Contains("because it is being used by another process."))
                    {
                        ShowError("Файл не может быть открыт, так как используется другим процессом.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет, может ли быть быть вызвана команда ReadFileCommand
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CanExecuteOpenFileCommand(object? parameter)
        {
            return EncodingFile != null && Separator != null && !IsSavingData;
        }

        /// <summary>
        /// Вызывает команду SaveDataInDataBaseCommand
        /// </summary>
        /// <param name="parameter"></param>
        private async void ExecuteSaveDataInDataBaseCommandAsync(object? parameter)
        {
            IsDataModified = false;
            IsSavingData = true;
            try
            {
                using ProcessContext context = new();
                using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    foreach (ProcessModel processModel in processModels)
                    {
                        ProcessName? processName = await context.ProcessNames.FirstOrDefaultAsync(pn => pn.Name == processModel.Name);
                        if (processName == null)
                        {
                            processName = new ProcessName { Name = processModel.Name };
                            await context.ProcessNames.AddAsync(processName);
                            await context.SaveChangesAsync();
                        }
                        Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Name == processModel.Category);
                        if (category == null)
                        {
                            category = new Category { Name = processModel.Category };
                            await context.Categories.AddAsync(category);
                            await context.SaveChangesAsync();
                        }
                        Divisione? division = null;
                        if (!string.IsNullOrEmpty(processModel.Division))
                        {
                            division = await context.Divisiones.FirstOrDefaultAsync(d => d.Name == processModel.Division);
                            if (division == null)
                            {
                                division = new Divisione { Name = processModel.Division };
                                await context.Divisiones.AddAsync(division);
                                await context.SaveChangesAsync();
                            }
                        }
                        Process? process = await context.Processes.FirstOrDefaultAsync(p => p.Code == processModel.Code);
                        if (process == null)
                        {
                            process = new Process
                            {
                                Code = processModel.Code,
                                ProcessNameId = processName.Id,
                                CategoryId = category.Id
                            };
                            await context.Processes.AddAsync(process);
                            await context.SaveChangesAsync();
                        }
                        if (division != null)
                        {
                            ProcessDivision processDivision = new()
                            {
                                ProcessId = process.Id,
                                DivisionId = division.Id
                            };
                            await context.ProcessesDivisions.AddAsync(processDivision);
                            await context.SaveChangesAsync();
                        }
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ShowError($"Данные не были сохранены в базе данных.\nОшибка: {ex.Message}{(ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : string.Empty)}");
                }
            }
            catch (Exception e)
            {
                ShowError($"Не удалось подключиться к базе данных:\n{e.Message}");
            }
            finally
            {
                IsSavingData = false;
            }
        }

        /// <summary>
        /// Проверяет, может ли быть вызвана команда SaveDataInDataBaseCommand
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CanExecuteSaveDataInDataBaseCommand(object? parameter)
        {
            return IsDataModified;
        }
    }
}
