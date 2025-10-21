namespace Domain.Models;

public class Property(Guid id,
    Guid personId,
    string type,
    int? cost)
{
    public Guid Id = id;
    public Guid PersonId = personId;
    public string Type = type;
    public int? Cost = cost;
}