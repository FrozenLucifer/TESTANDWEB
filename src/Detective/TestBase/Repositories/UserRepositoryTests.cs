using DataAccess;
using DataAccess.Models;
using DataAccess.Repository;
using Domain.Enum;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Xunit;

namespace TestBase.Repositories;

public class UserRepositoryTests<TFixture>
    where TFixture : DatabaseFixtureBase, new()
{
    private readonly Context _dbContext;
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests()
    {
        var fixture = new TFixture();
        fixture.InitializeAsync().GetAwaiter().GetResult();
        _dbContext = fixture.DbContext;
        _userRepository = new UserRepository(_dbContext);

        _dbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task CreateUser_ShouldCreateSuccessfully()
    {
        // Act
        await _userRepository.CreateUser("user1", "pass1", UserType.Admin);

        // Assert
        var loaded = await _userRepository.GetUser("user1");
        Assert.Equal("user1", loaded.Username);
        Assert.Equal("pass1", loaded.Password);
        Assert.Equal(UserType.Admin, loaded.Type);
    }

    [Fact]
    public async Task CreateUser_ShouldThrow_WhenUserAlreadyExists()
    {
        var user = new UserBuilder()
            .WithUsername("user2")
            .Build();

        _dbContext.Users.Add(user);

        await Assert.ThrowsAsync<UserAlreadyExistsRepositoryException>(() =>
            _userRepository.CreateUser("user2", "pass2", UserType.Admin));
    }

    [Fact]
    public async Task GetUser_ShouldThrow_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<UserNotFoundRepositoryException>(() =>
            _userRepository.GetUser("nonexistent"));
    }

    [Fact]
    public async Task ChangePassword_ShouldUpdatePassword()
    {
        var user = new UserBuilder()
            .WithUsername("user3")
            .WithPassword("oldpass")
            .Build();

        _dbContext.Users.Add(user);

        await _userRepository.ChangePassword(user.Username, "newpass");

        var loaded = await _userRepository.GetUser(user.Username);
        Assert.Equal("newpass", loaded.Password);
    }

    [Fact]
    public async Task ChangePassword_ShouldThrow_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<UserNotFoundRepositoryException>(() =>
            _userRepository.ChangePassword("nonexistent", "pass"));
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteSuccessfully()
    {
        var user = new UserBuilder()
            .WithUsername("user4")
            .Build();

        _dbContext.Users.Add(user);

        await _userRepository.DeleteUser(user.Username);

        await Assert.ThrowsAsync<UserNotFoundRepositoryException>(() =>
            _userRepository.GetUser(user.Username));
    }

    [Fact]
    public async Task DeleteUser_ShouldThrow_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<UserNotFoundRepositoryException>(() =>
            _userRepository.DeleteUser("nonexistent"));
    }

    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers()
    {
        var user1 = new UserBuilder()
            .WithUsername("user5")
            .Build();

        var user2 = new UserBuilder()
            .WithUsername("user6")
            .Build();

        _dbContext.Users.Add(user1);
        _dbContext.Users.Add(user2);
        await _dbContext.SaveChangesAsync();

        var users = await _userRepository.GetUsers();
        Assert.Contains(users, u => u.Username == user1.Username);
        Assert.Contains(users, u => u.Username == user2.Username);
        Assert.Equal(2, users.Count);
    }
}

public class UserBuilder
{
    private string _username = "defaultUser";
    private string _password = "defaultPass";
    private UserType _type = UserType.Admin;

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public UserBuilder WithType(UserType type)
    {
        _type = type;
        return this;
    }

    public UserDb Build()
    {
        var user = new UserDb(username: _username, password: _password, type: _type);

        return user;
    }
}