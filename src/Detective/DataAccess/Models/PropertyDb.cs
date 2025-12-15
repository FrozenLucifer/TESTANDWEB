namespace DataAccess.Models;

public class PropertyDb(Guid id, Guid personId, string name, int? cost)
{
    public Guid Id { get; set; }= id;
    public Guid PersonId { get; set; }= personId;
    public string Name { get; set; }= name;
    public int? Cost { get; set; }= cost;
    public virtual PersonDb Person { get; set; }
}