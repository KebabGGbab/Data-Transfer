namespace Data_Transfer.EF;

public partial class Process
{
    public uint Id { get; set; }

    public string Code { get; set; } = null!;

    public uint ProcessNameId { get; set; }

    public virtual ProcessName ProcessName { get; set; } = null!;

    public uint CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProcessDivision> ProcessesDivisions { get; set; } = [];
}