namespace Data_Transfer.Models
{
    /// <summary>
    /// Model, содержащая информацию о процессе.
    /// </summary>
    public class ProcessModel : BaseMVVM
    {
        /// <summary>
        /// Категория процесса.
        /// </summary>
        [ColumnName("Категория процесса")]
        public string Category { get; set; } = null!;

        /// <summary>
        /// Код процесса.
        /// </summary>
        [ColumnName("Код процесса")]
        public string Code { get; set; } = null!;

        /// <summary>
        /// Имя процесса.
        /// </summary>
        [ColumnName("Наименование процесса")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Подразделение-владелец процесса.
        /// </summary>
        [ColumnName("Подразделение-владелец процесса")]
        public string? Division { get; set; }

        /// <summary>
        /// ПОлучить информацию о процессах из файлы.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="encoding">Кодировка, в которой следует открыть файл.</param>
        /// <param name="separator">Строка, которой разделены столбцы.</param>
        /// <returns></returns>
        public static ICollection<ProcessModel> GetProcess(string filePath, string encoding, string separator)
        {
            return CSVFileConverterInCSharpObject<ProcessModel>.GetObjects(filePath, encoding, separator);
        }
    }
}
