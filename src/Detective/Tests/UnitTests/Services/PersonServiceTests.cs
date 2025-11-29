using System.Text.Json;
using Domain.Enum;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services;
using Domain.Interfaces.Repository;
using Domain.Models;
using FluentValidation;
using Logic.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace UnitTests.Services;

public class PersonServiceTests
{
    public abstract class PersonServiceTestsBase
    {
        protected readonly Mock<IPersonRepository> PersonRepositoryMock;
        protected readonly Mock<IContactRepository> ContactRepositoryMock;
        protected readonly Mock<IDocumentRepository> DocumentRepositoryMock;
        protected readonly Mock<IPropertyRepository> PropertyRepositoryMock;
        protected readonly Mock<IRelationshipRepository> RelationshipRepositoryMock;
        protected readonly Mock<ICharacteristicRepository> CharacteristicRepositoryMock;
        protected readonly PersonService PersonService;

        protected PersonServiceTestsBase()
        {
            PersonRepositoryMock = new Mock<IPersonRepository>();
            ContactRepositoryMock = new Mock<IContactRepository>();
            DocumentRepositoryMock = new Mock<IDocumentRepository>();
            PropertyRepositoryMock = new Mock<IPropertyRepository>();
            RelationshipRepositoryMock = new Mock<IRelationshipRepository>();
            CharacteristicRepositoryMock = new Mock<ICharacteristicRepository>();

            PersonService = new PersonService(
                PersonRepositoryMock.Object,
                ContactRepositoryMock.Object,
                DocumentRepositoryMock.Object,
                PropertyRepositoryMock.Object,
                RelationshipRepositoryMock.Object,
                CharacteristicRepositoryMock.Object,
                NullLogger<PersonService>.Instance);
        }
    }

    public class GetPersonRelationshipsTests : PersonServiceTestsBase
    {

        [Fact]
        public async Task GetPersonRelationships_WhenDepthIsZero_ThrowsArgumentException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            uint depth = 0;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => PersonService.GetPersonRelationships(personId, depth));
        }

        [Fact]
        public async Task GetPersonRelationships_WhenPersonDontExists_ThrowsException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            uint depth = 1;

            RelationshipRepositoryMock.Setup(x => x.GetPersonRelationships(personId))
                .Throws(new PersonNotFoundRepositoryException(personId));

            // Act & Assert
            await Assert.ThrowsAsync<PersonNotFoundException>(() => PersonService.GetPersonRelationships(personId, depth));
        }

        [Fact]
        public async Task GetPersonRelationships_WhenNoRelationships_WithDepth1_ReturnsEmpty()
        {
            // Arrange
            var personId = Guid.NewGuid();
            uint depth = 1;

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(personId))
                .ReturnsAsync([]);

            var result = await PersonService.GetPersonRelationships(personId, depth);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPersonRelationships_WithDepth1_ReturnsDirectRelationships()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var relatedPerson1 = Guid.NewGuid();
            var relatedPerson2 = Guid.NewGuid();
            uint depth = 1;

            var expectedRelationships = new List<Relationship>
            {
                new Relationship(personId1: personId, personId2: relatedPerson1, type: RelationshipType.Friend),
                new Relationship(personId1: personId, personId2: relatedPerson2, type: RelationshipType.Colleague)
            };

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(personId))
                .ReturnsAsync(expectedRelationships);

            // Act
            var result = await PersonService.GetPersonRelationships(personId, depth);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.PersonId2 == relatedPerson1);
            Assert.Contains(result, r => r.PersonId2 == relatedPerson2);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(personId), Times.Once);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetPersonRelationships_WithDepth2_ReturnsMultiLevelRelationships1()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var level1Person = Guid.NewGuid();
            var level2Person = Guid.NewGuid();
            uint depth = 2;

            var level1Relationships = new List<Relationship>
            {
                new Relationship(personId1: personId, personId2: level1Person, type: RelationshipType.Friend)
            };

            var level2Relationships = new List<Relationship>
            {
                new Relationship(personId1: level1Person, personId2: level2Person, type: RelationshipType.Colleague)
            };

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(personId))
                .ReturnsAsync(level1Relationships);

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(level1Person))
                .ReturnsAsync(level2Relationships);

            // Act
            var result = await PersonService.GetPersonRelationships(personId, depth);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.PersonId1 == personId && r.PersonId2 == level1Person);
            Assert.Contains(result, r => r.PersonId1 == level1Person && r.PersonId2 == level2Person);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(personId), Times.Once);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(level1Person), Times.Once);
        }

        [Fact]
        public async Task GetPersonRelationships_WhenNoFurtherRelationships_StopsEarly()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var relatedPerson = Guid.NewGuid();
            uint depth = 3;

            var personRelationships = new List<Relationship>
            {
                new Relationship(personId1: personId, personId2: relatedPerson, type: RelationshipType.Friend)
            };

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(personId))
                .ReturnsAsync(personRelationships);

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(relatedPerson))
                .ReturnsAsync(new List<Relationship>());

            // Act
            var result = await PersonService.GetPersonRelationships(personId, depth);

            // Assert
            Assert.Single(result);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(personId), Times.Once);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(relatedPerson), Times.Once);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(It.IsAny<Guid>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetPersonRelationships_WithDepth2_ReturnsMultiLevelRelationships2()
        {
            // Arrange
            var person1Id = Guid.NewGuid();
            var person2Id = Guid.NewGuid();
            var person3Id = Guid.NewGuid();
            var person4Id = Guid.NewGuid();
            var person5Id = Guid.NewGuid();
            uint depth = 2;

            var person1Relationships = new List<Relationship>
            {
                new Relationship(person1Id, person2Id, RelationshipType.Friend),
                new Relationship(person1Id, person3Id, RelationshipType.Friend)
            };

            var person2Relationships = new List<Relationship>
            {
                new Relationship(person2Id, person1Id, RelationshipType.Friend),
                new Relationship(person2Id, person4Id, RelationshipType.Friend),
                new Relationship(person2Id, person5Id, RelationshipType.Friend),
            };

            var person3Relationships = new List<Relationship>
            {
                new Relationship(person3Id, person1Id, RelationshipType.Friend),
                new Relationship(person3Id, person5Id, RelationshipType.Friend),
            };

            var person4Relationships = new List<Relationship>
            {
                new Relationship(person4Id, person2Id, RelationshipType.Friend),
            };

            var person5Relationships = new List<Relationship>
            {
                new Relationship(person5Id, person2Id, RelationshipType.Friend),
                new Relationship(person5Id, person3Id, RelationshipType.Friend),
            };


            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(person1Id))
                .ReturnsAsync(person1Relationships);


            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(person2Id))
                .ReturnsAsync(person2Relationships);

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(person3Id))
                .ReturnsAsync(person3Relationships);

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(person4Id))
                .ReturnsAsync(person4Relationships);

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(person5Id))
                .ReturnsAsync(person5Relationships);

            // Act
            var result = await PersonService.GetPersonRelationships(person1Id, depth);

            var expectedResult = new List<Relationship>
            {
                new Relationship(person1Id, person2Id, RelationshipType.Friend),
                new Relationship(person1Id, person3Id, RelationshipType.Friend),
                new Relationship(person2Id, person4Id, RelationshipType.Friend),
                new Relationship(person2Id, person5Id, RelationshipType.Friend),
                new Relationship(person3Id, person5Id, RelationshipType.Friend),
            };

            // Assert
            foreach (var expectedRelationship in expectedResult)
            {
                var oppositeRelationship = expectedRelationship.Opposite();

                Assert.True(
                    result.Contains(expectedRelationship) ||
                    result.Contains(oppositeRelationship),
                    $"Neither relationship {expectedRelationship} nor its opposite was found in the results");
            }
        }


        [Fact]
        public async Task GetPersonRelationships_WithCircularRelationship_ReturnsRelationshipsWithoutDuplicates()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var relatedPerson = Guid.NewGuid();
            uint depth = 2;

            var personRelationships = new List<Relationship>
            {
                new Relationship(personId1: personId, personId2: relatedPerson, type: RelationshipType.Friend)
            };

            var relatedPersonRelationships = new List<Relationship>
            {
                new Relationship(personId1: relatedPerson, personId2: personId, type: RelationshipType.Friend)
            };

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(personId))
                .ReturnsAsync(personRelationships);

            RelationshipRepositoryMock
                .Setup(x => x.GetPersonRelationships(relatedPerson))
                .ReturnsAsync(relatedPersonRelationships);

            // Act
            var result = await PersonService.GetPersonRelationships(personId, depth);

            // Assert
            Assert.Single(result);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(personId), Times.Once);
            RelationshipRepositoryMock.Verify(x => x.GetPersonRelationships(relatedPerson), Times.Once);
        }
    }

    public class GetPersonsTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task GetPersons_ReturnsFilteredPersons()
        {
            // Arrange
            var expectedPersons = new List<Person>
            {
                new Person(Guid.NewGuid(), Sex.Male, "John Doe", new DateOnly(1990, 1, 1))
            };

            PersonRepositoryMock.Setup(x =>
                    x.GetPersons(It.IsAny<Sex?>(), It.IsAny<string>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(expectedPersons);

            // Act
            var result = await PersonService.GetPersons(Sex.Male, "John", null, null, 10);

            // Assert
            Assert.Equal(expectedPersons, result);
            PersonRepositoryMock.Verify(x => x.GetPersons(Sex.Male, "John", null, null, 10, null), Times.Once);
        }
    }

    public class GetPersonTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task GetPerson_WithDefaultId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => PersonService.GetPerson(Guid.Empty));
        }

        [Fact]
        public async Task GetPerson_WhenPersonNotFound_ThrowsPersonNotFoundException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            PersonRepositoryMock.Setup(x => x.GetPerson(personId)).Throws<PersonNotFoundRepositoryException>();

            // Act & Assert
            await Assert.ThrowsAsync<PersonNotFoundException>(() => PersonService.GetPerson(personId));
        }

        [Fact]
        public async Task GetPerson_WithValidId_ReturnsPersonFull()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var person = new Person(personId, Sex.Female, "Jane Doe", new DateOnly(1995, 5, 5));
            var contacts = new List<Contact> { new(Guid.NewGuid(), personId, ContactType.Phone, "+79991112233") };
            var documents = new List<Document>();
            var properties = new List<Property>();

            PersonRepositoryMock.Setup(x => x.GetPerson(personId)).ReturnsAsync(person);
            ContactRepositoryMock.Setup(x => x.GetPersonContacts(personId, null)).ReturnsAsync(contacts);
            DocumentRepositoryMock.Setup(x => x.GetPersonDocuments(personId)).ReturnsAsync(documents);
            PropertyRepositoryMock.Setup(x => x.GetPersonProperties(personId)).ReturnsAsync(properties);

            // Act
            var result = await PersonService.GetPerson(personId);

            // Assert
            Assert.Equal(person.Id, result.Id);
            Assert.Equal(person.Sex, result.Sex);
            Assert.Equal(person.BirthDate, result.BirthDate);
            Assert.Equal(person.FullName, result.FullName);

            Assert.Equal(contacts, result.Contacts);
            Assert.Equal(documents, result.Documents);
            Assert.Equal(properties, result.Properties);
        }
    }

    public class DeletePersonTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task DeletePerson_WithValidId_CallsRepository()
        {
            // Arrange
            var personId = Guid.NewGuid();

            // Act
            await PersonService.DeletePerson(personId);

            // Assert
            PersonRepositoryMock.Verify(x => x.DeletePerson(personId), Times.Once);
        }

        [Fact]
        public async Task DeletePerson_WithDefaultId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => PersonService.DeletePerson(Guid.Empty));
        }
    }

    public class CreatePersonTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task CreatePerson_WithValidData_ReturnsGuid()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var person = new Person(expectedId, Sex.Male, "John Doe", new DateOnly(1990, 1, 1));

            PersonRepositoryMock.Setup(x => x.CreatePerson(It.IsAny<Guid>(), It.IsAny<Sex?>(), It.IsAny<string>(), It.IsAny<DateOnly?>()))
                .ReturnsAsync(person);

            // Act
            var result = await PersonService.CreatePerson(Sex.Male, "John Doe", new DateOnly(1990, 1, 1));

            // Assert
            Assert.Equal(expectedId, result);
        }

        [Fact]
        public async Task CreatePerson_WhenPersonExists_ThrowsPersonAlreadyExistsException()
        {
            // Arrange
            PersonRepositoryMock.Setup(x => x.CreatePerson(It.IsAny<Guid>(), It.IsAny<Sex?>(), It.IsAny<string>(), It.IsAny<DateOnly?>()))
                .Throws(new PersonAlreadyExistsRepositoryException(Guid.NewGuid()));

            // Act & Assert
            await Assert.ThrowsAsync<PersonServiceException>(() =>
                PersonService.CreatePerson(Sex.Male, "John Doe", new DateOnly(1990, 1, 1)));
        }
    }

    public class ChangePersonGeneralInfoTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task ChangePersonGeneralInfo_WithValidData_CallsRepository()
        {
            // Arrange
            var personId = Guid.NewGuid();

            // Act
            await PersonService.ChangePersonGeneralInfo(personId, Sex.Female, "Jane Smith", new DateOnly(1995, 5, 5));

            // Assert
            PersonRepositoryMock.Verify(x => x.UpdatePerson(personId, Sex.Female, "Jane Smith", new DateOnly(1995, 5, 5)), Times.Once);
        }
    }

    public class SetPersonsRelationshipTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task SetPersonsRelationship_WithValidIds_CallsRepository()
        {
            // Arrange
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            // Act
            await PersonService.SetPersonsRelationship(id1, id2, RelationshipType.Child);

            // Assert
            RelationshipRepositoryMock.Verify(x => x.SetRelationship(id1, id2, RelationshipType.Child), Times.Once);
        }
    }


    // Группа тестов для метода AddPersonDocument
    public class AddPersonDocumentTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task WithValidPassport_CallsRepository()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var passportPayload = JsonSerializer.Serialize(new PassportPayload { Series = "1234", Number = "123456" });

            // Act
            await PersonService.AddPersonDocument(personId,
                DocumentType.Passport,
                passportPayload);

            // Assert
            DocumentRepositoryMock.Verify(x => x.CreateDocument(It.IsAny<Guid>(), personId, DocumentType.Passport, passportPayload), Times.Once);
        }

        [Fact]
        public async Task WithInvalidPassportJson_ThrowsValidationException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var passportPayload = "1234 567890";

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => PersonService.AddPersonDocument(personId,
                DocumentType.Passport,
                passportPayload));

            DocumentRepositoryMock.Verify(x => x.CreateDocument(It.IsAny<Guid>(), personId, DocumentType.Passport, passportPayload), Times.Never);
        }

        [Fact]
        public async Task WithInvalidPassport_ThrowsValidationException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var passportPayload = JsonSerializer.Serialize(new PassportPayload { Series = "123456", Number = "12345678" });

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => PersonService.AddPersonDocument(personId,
                DocumentType.Passport,
                passportPayload));

            DocumentRepositoryMock.Verify(x => x.CreateDocument(It.IsAny<Guid>(), personId, DocumentType.Passport, passportPayload), Times.Never);
        }
    }

    // Группа тестов для метода AddPersonContact
    public class AddPersonContactTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task WithUnknownContactType_ThrowsValidationException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var unknownType = (ContactType)999;

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                PersonService.AddPersonContact(personId, unknownType, "any-value"));
        }

        [Fact]
        public async Task WithValidEmail_CallsRepository()
        {
            // Arrange
            var personId = Guid.NewGuid();

            // Act
            await PersonService.AddPersonContact(personId, ContactType.Email, "test@example.com");

            // Assert
            ContactRepositoryMock.Verify(x => x.CreateContact(It.IsAny<Guid>(), personId, ContactType.Email, "test@example.com"), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("invalid-email")]
        [InlineData("test@.com")]
        [InlineData("@example.com")]
        public async Task WithInvalidEmail_ThrowsValidationException(string invalidEmail)
        {
            // Arrange
            var personId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                PersonService.AddPersonContact(personId, ContactType.Email, invalidEmail));
        }

        [Fact]
        public async Task WithValidPhone_CallsRepository()
        {
            // Arrange
            var personId = Guid.NewGuid();

            // Act
            await PersonService.AddPersonContact(personId, ContactType.Phone, "+78005553535");

            // Assert
            ContactRepositoryMock.Verify(x => x.CreateContact(It.IsAny<Guid>(), personId, ContactType.Phone, "+78005553535"), Times.Once);
        }

        [Fact]
        public async Task WithInvalidPhone_ThrowsValidationException()
        {
            // Arrange
            var personId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                PersonService.AddPersonContact(personId, ContactType.Phone, "12345"));
        }
    }

    // Группа тестов для метода AddPersonProperty
    public class AddPersonPropertyTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task WithValidData_CallsRepository()
        {
            // Arrange
            var personId = Guid.NewGuid();

            // Act
            await PersonService.AddPersonProperty(personId, "House", 100000);

            // Assert
            PropertyRepositoryMock.Verify(x => x.CreateProperty(It.IsAny<Guid>(), personId, "House", 100000), Times.Once);
        }
    }

    // Группа тестов для метода DeletePersonContact
    public class DeletePersonContactTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task WithValidId_CallsRepository()
        {
            // Arrange
            var contactId = Guid.NewGuid();

            // Act
            await PersonService.DeletePersonContact(contactId);

            // Assert
            ContactRepositoryMock.Verify(x => x.DeleteContact(contactId), Times.Once);
        }
    }

    // Группа тестов для метода DeletePersonDocument
    public class DeletePersonDocumentTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task WithValidId_CallsRepository()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            // Act
            await PersonService.DeletePersonDocument(documentId);

            // Assert
            DocumentRepositoryMock.Verify(x => x.DeleteDocument(documentId), Times.Once);
        }
    }

    // Группа тестов для метода DeletePersonProperty
    public class DeletePersonPropertyTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task WithValidId_CallsRepository()
        {
            // Arrange
            var propertyId = Guid.NewGuid();

            // Act
            await PersonService.DeletePersonProperty(propertyId);

            // Assert
            PropertyRepositoryMock.Verify(x => x.DeleteProperty(propertyId), Times.Once);
        }
    }

    // Группа тестов для метода GetPersonIdByContact
    public class GetPersonIdByContactTests : PersonServiceTestsBase
    {
        [Fact]
        public async Task WithInvalidPhone_ThrowsValidationException()
        {
            // Act & Assert

            await Assert.ThrowsAsync<ValidationException>(() =>
                PersonService.GetPersonByContact(ContactType.Phone, "+7999555667788"));
        }

        [Fact]
        public async Task WithValidPhone_ReturnsPersonId()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var person = new Person(Guid.NewGuid(), Sex.Male, "John Doe", new DateOnly(1990, 1, 1));
            ContactRepositoryMock.Setup(x => x.GetPersonByContact(It.IsAny<ContactType>(), It.IsAny<string>()))
                .ReturnsAsync(person);

            // Act
            var result = await PersonService.GetPersonByContact(ContactType.Phone, "+79995556677");

            // Assert
            Assert.Equal(person, result);
        }
    }
}