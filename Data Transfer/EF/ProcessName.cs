namespace Data_Transfer.EF
{
    public partial class ProcessName
    {
        public uint Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Process> Processes { get; set; } = [];
    }
}