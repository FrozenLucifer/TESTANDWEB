using Domain.Models;
using Domain.Exceptions.Repositories;

namespace Domain.Interfaces.Repository;

public interface IPropertyRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="personId"></param>
    /// <param name="name"></param>
    /// <param name="cost">Стоимость в рублях</param>
    public Task CreateProperty(Guid id, Guid personId, string name, int? cost);

    public Task<List<Property>> GetPersonProperties(Guid personId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="ContactNotFoundRepositoryException"></exception>
    public Task DeleteProperty(Guid id);
}