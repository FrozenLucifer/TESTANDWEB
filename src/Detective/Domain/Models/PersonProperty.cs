namespace Domain.Models;

public class PersonProperty(Guid id,
    Guid personId,
    string type,
    int? cost)
{
    public Guid Id { get; }= id;
    public Guid PersonId { get; }= personId;
    public string Type { get; }= type;
    public int? Cost { get; }= cost;
}