using IntegrationTests.DataAccess;
using TestBase.Repositories;

namespace IntegrationTests.Repositories;

public class RelationshipRepositoryPostgresTests : RelationshipRepositoryTests<PostgresDatabaseFixture>;

public class PersonRepositoryPostgresTests : PersonRepositoryTests<PostgresDatabaseFixture>;

public class ContactRepositoryPostgresTests : ContactRepositoryTests<PostgresDatabaseFixture>;

public class CharacteristicRepositoryPostgresTests : CharacteristicRepositoryTests<PostgresDatabaseFixture>;

public class DocumentRepositoryPostgresTests : DocumentRepositoryTests<PostgresDatabaseFixture>;

public class PropertyRepositoryPostgresTests : PropertyRepositoryTests<PostgresDatabaseFixture>;

public class UserRepositoryPostgresTests : UserRepositoryTests<PostgresDatabaseFixture>;