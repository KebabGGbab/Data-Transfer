namespace Data_Transfer.EF;

public partial class ProcessDivision
{
    public uint Id { get; set; }

    public uint ProcessId { get; set; }

    public uint? DivisionId { get; set; }

    public virtual Divisione? Division { get; set; }

    public virtual Process Process { get; set; } = null!;
}