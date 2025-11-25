using Domain.Enum;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Service;
using Domain.Models;
using Logic.Validators;
using Microsoft.Extensions.Logging;

namespace Logic.Services;

public class PersonService(IPersonRepository personRepository,
    IContactRepository contactRepository,
    IDocumentRepository documentRepository,
    IPropertyRepository propertyRepository,
    IRelationshipRepository relationshipRepository,
    ICharacteristicRepository characteristicRepository,
    ILogger<PersonService> logger) : IPersonService
{
    private readonly DocumentValidator _documentValidator = new DocumentValidator();
    private readonly ContactValidator _contactValidator = new ContactValidator();
    private readonly PropertyValidator _propertyValidator = new PropertyValidator();
    private readonly PersonValidator _personValidator = new PersonValidator();

    public async Task<List<Person>> GetPersons(Sex? sex, string? fullName, DateOnly? minBirthDate, DateOnly? maxBirthDate, int? limit = null, int? skip = null)
    {
        var persons = await personRepository.GetPersons(sex, fullName, minBirthDate, maxBirthDate, limit, skip);

        return persons;
    }

    public async Task<PersonFull> GetPerson(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(id));

        try
        {
            var person = await personRepository.GetPerson(id);
            var contacts = await contactRepository.GetPersonContacts(id);
            var documents = await documentRepository.GetPersonDocuments(id);
            var properties = await propertyRepository.GetPersonProperties(id);
            var characteristics = await characteristicRepository.GetPersonsCharacteristics(id);

            return new PersonFull(person, contacts, documents, properties, characteristics);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id}", id);
            throw new PersonNotFoundException(id);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task DeletePerson(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(id));

        try
        {
            await personRepository.DeletePerson(id);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id}", id);
            throw new PersonNotFoundException(id);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<Guid> CreatePerson(Sex? sex, string? fullName, DateOnly? birthDate)
    {
        (await _personValidator.ValidateAsync((sex, fullName, birthDate))).ThrowIfInvalid();

        try
        {
            var person = await personRepository.CreatePerson(Guid.NewGuid(), sex, fullName, birthDate);
            return person.Id;
        }
        catch (PersonAlreadyExistsRepositoryException)
        {
            throw new PersonServiceException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task ChangePersonGeneralInfo(Guid id, Sex? sex, string? fullName, DateOnly? birthDate)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(id));

        (await _personValidator.ValidateAsync((sex, fullName, birthDate))).ThrowIfInvalid();

        try
        {
            await personRepository.UpdatePerson(id, sex, fullName, birthDate);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id}", id);
            throw new PersonNotFoundException(id);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<List<Relationship>> GetPersonRelationships(Guid personId, uint depth = 1, List<RelationshipType>? allowedTypes = null)
    {
        if (depth == 0)
            throw new ArgumentException("Depth can't be 0", nameof(depth));

        try
        {
            var result = new List<Relationship>();
            var visitedPersons = new HashSet<Guid> { personId };
            var currentLevelPersons = new Queue<Guid>();
            currentLevelPersons.Enqueue(personId);

            var addedRelationships = new HashSet<(Guid, Guid)>();

            for (int currentDepth = 0; currentDepth < depth; currentDepth++)
            {
                var nextLevelPersons = new Queue<Guid>();

                while (currentLevelPersons.Count > 0)
                {
                    var currentPersonId = currentLevelPersons.Dequeue();
                    var relationships = await relationshipRepository.GetPersonRelationships(currentPersonId).ConfigureAwait(true);

                    foreach (var relationship in relationships)
                    {
                        var relatedPersonId = relationship.PersonId2;

                        if (allowedTypes is not null && allowedTypes.Contains(relationship.Type))
                            continue;

                        if (!addedRelationships.Contains((relationship.PersonId2, relationship.PersonId1)))
                        {
                            result.Add(relationship);
                            addedRelationships.Add((relationship.PersonId1, relationship.PersonId2));
                        }

                        if (visitedPersons.Add(relatedPersonId))
                        {
                            nextLevelPersons.Enqueue(relatedPersonId);
                        }
                    }
                }

                currentLevelPersons = nextLevelPersons;

                if (currentLevelPersons.Count == 0)
                    break;
            }

            return result;
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id}", personId);
            throw new PersonNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task SetPersonsRelationship(Guid id1, Guid id2, RelationshipType type)
    {
        if (id1 == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(id1));

        if (id2 == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(id2));

        try
        {
            await relationshipRepository.SetRelationship(id1, id2, type);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id1} or {id2}", id1, id2);
            throw new PersonNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task DeleteRelationship(Guid id1, Guid id2)
    {
        if (id1 == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(id1));

        if (id2 == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(id2));

        try
        {
            await relationshipRepository.SetRelationship(id1, id2, null);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id1} or {id2}", id1, id2);
            throw new PersonNotFoundException();
        }
        catch (RelationshipNotFoundRepositoryException)
        {
            logger.LogError("Relationship not found between {id1} and {id2}", id1, id2);
            throw new RelationshipNotFoundException(id1, id2);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task AddPersonDocument(Guid personId, DocumentType type, string info)
    {
        if (personId == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(personId));

        (await _documentValidator.ValidateAsync((type, info))).ThrowIfInvalid();

        try
        {
            await documentRepository.CreateDocument(Guid.NewGuid(), personId, type, info);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id}", personId);
            throw new PersonNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task AddPersonContact(Guid personId, ContactType type, string info)
    {
        if (personId == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(personId));

        (await _contactValidator.ValidateAsync((type, info))).ThrowIfInvalid();

        try
        {
            await contactRepository.CreateContact(Guid.NewGuid(), personId, type, info);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id}", personId);
            throw new PersonNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task AddPersonProperty(Guid personId, string name, int? cost)
    {
        if (personId == Guid.Empty)
            throw new ArgumentException("Person Id cant be default Guid", nameof(personId));

        (await _propertyValidator.ValidateAsync((name, cost))).ThrowIfInvalid();

        try
        {
            await propertyRepository.CreateProperty(Guid.NewGuid(), personId, name, cost);
        }
        catch (PersonNotFoundRepositoryException)
        {
            logger.LogError("Person not found with id = {id}", personId);
            throw new PersonNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task DeletePersonContact(Guid contactId)
    {
        if (contactId == Guid.Empty)
            throw new ArgumentException("Contact Id cant be default Guid", nameof(contactId));

        try
        {
            await contactRepository.DeleteContact(contactId);
        }
        catch (ContactNotFoundRepositoryException)
        {
            logger.LogError("Contact not found with id = {id}", contactId);
            throw new ContactNotFoundException(contactId);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task DeletePersonDocument(Guid documentId)
    {
        if (documentId == Guid.Empty)
            throw new ArgumentException("Document Id cant be default Guid", nameof(documentId));

        try
        {
            await documentRepository.DeleteDocument(documentId);
        }
        catch (DocumentNotFoundRepositoryException)
        {
            throw new DocumentNotFoundException(documentId);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task DeletePersonProperty(Guid propertyId)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property Id cant be default Guid", nameof(propertyId));

        try
        {
            await propertyRepository.DeleteProperty(propertyId);
        }
        catch (PropertyNotFoundRepositoryException)
        {
            throw new PropertyNotFoundException(propertyId);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<Person> GetPersonByContact(ContactType contactType, string info)
    {
        (await _contactValidator.ValidateAsync((contactType, info))).ThrowIfInvalid();

        try
        {
            var personId = await contactRepository.GetPersonByContact(contactType, info);
            return personId;
        }
        catch (PersonNotFoundRepositoryException)
        {
            throw new PersonNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<Characteristic> GetCharacteristicById(Guid id)
    {
        try
        {
            var characteristic = await characteristicRepository.GetCharacteristicById(id);
            return characteristic;
        }
        catch (CharacteristicNotFoundRepositoryException)
        {
            throw new CharacteristicNotFoundException(id);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<IEnumerable<Characteristic>> GetPersonsCharacteristics(Guid personId)
    {
        try
        {
            var characteristics = await characteristicRepository.GetPersonsCharacteristics(personId);
            return characteristics;
        }
        catch (CharacteristicNotFoundRepositoryException)
        {
            throw new CharacteristicNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<Guid> CreateCharacteristic(Guid personId, string username, string appearance, string personality, string medicalConditions)
    {
        try
        {
            var id = Guid.NewGuid();
            await characteristicRepository.CreateCharacteristic(id,
                personId,
                username,
                appearance,
                personality,
                medicalConditions);
            return id;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task DeleteCharacteristic(Guid id)
    {
        try
        {
            await characteristicRepository.DeleteCharacteristic(id);
        }
        catch (CharacteristicNotFoundRepositoryException)
        {
            throw new CharacteristicNotFoundException();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }
}