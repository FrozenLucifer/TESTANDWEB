namespace DataAccess.Models;

public class PropertyDb(Guid id, Guid personId, string name, int? cost)
{
    public Guid Id = id;
    public Guid PersonId = personId;
    public string Name = name;
    public int? Cost = cost;
    public virtual PersonDb Person { get; set; }
}