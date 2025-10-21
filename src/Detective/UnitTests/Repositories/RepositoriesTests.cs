using TestBase.Repositories;

namespace UnitTests.Repositories;


public class RelationshipRepositoryInMemoryTests : RelationshipRepositoryTests<InMemoryDatabaseFixture>;

public class PersonRepositoryInMemoryTests : PersonRepositoryTests<InMemoryDatabaseFixture>;

public class ContactRepositoryInMemoryTests : ContactRepositoryTests<InMemoryDatabaseFixture>;

public class CharacteristicRepositoryInMemoryTests : CharacteristicRepositoryTests<InMemoryDatabaseFixture>;

public class DocumentRepositoryInMemoryTests : DocumentRepositoryTests<InMemoryDatabaseFixture>;

public class PropertyRepositoryInMemoryTests : PropertyRepositoryTests<InMemoryDatabaseFixture>;

public class UserRepositoryInMemoryTests : UserRepositoryTests<InMemoryDatabaseFixture>;