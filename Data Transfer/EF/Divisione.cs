namespace Data_Transfer.EF;

public partial class Divisione
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProcessDivision> ProcessesDivisions { get; set; } = [];
}