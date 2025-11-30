using Domain.Enums;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Logic.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace UnitTests.Services;

public class UserServiceTests
{
    // private readonly Mock<IUserRepository> _userRepositoryMock;
    // private readonly Mock<IPasswordHasher> _passwordHasherMock;
    // private readonly UserService _userService;
    //
    // public UserServiceTests()
    // {
    //     _userRepositoryMock = new Mock<IUserRepository>();
    //     _passwordHasherMock = new Mock<IPasswordHasher>();
    //     _userService = new UserService(_userRepositoryMock.Object, _passwordHasherMock.Object, NullLogger<UserService>.Instance);
    // }
    //
    // [Fact]
    // public async Task CreateUser_ShouldGeneratePasswordAndCallRepository()
    // {
    //     const string username = "testUser";
    //     const UserType userType = UserType.Employee;
    //     const string expectedHash = "hashedPassword";
    //
    //     _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
    //         .Returns(expectedHash);
    //
    //     // Act
    //     var result = await _userService.CreateUser(username, userType);
    //
    //     // Assert
    //     Assert.NotNull(result);
    //     _userRepositoryMock.Verify(x => x.CreateUser(username, expectedHash, userType), Times.Once);
    //     _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);
    // }
    //
    // [Fact]
    // public async Task CreateUser_ShouldThrowUserAlreadyExistsException_WhenUserExists()
    // {
    //     // Arrange
    //     const string username = "existingUser";
    //     const UserType userType = UserType.Employee;
    //
    //     _userRepositoryMock.Setup(x => x.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserType>()))
    //         .Throws(new UserAlreadyExistsRepositoryException(username));
    //
    //     // Act & Assert
    //     await Assert.ThrowsAsync<UserAlreadyExistsException>(() => _userService.CreateUser(username, userType));
    // }
    //
    // [Fact]
    // public async Task DeleteUser_ShouldCallRepository()
    // {
    //     // Arrange
    //     const string username = "userToDelete";
    //
    //     // Act
    //     await _userService.DeleteUser(username);
    //
    //     // Assert
    //     _userRepositoryMock.Verify(x => x.DeleteUser(username), Times.Once);
    // }
    //
    // [Fact]
    // public async Task DeleteUser_ShouldThrowUserNotFoundException_WhenUserNotExists()
    // {
    //     // Arrange
    //     const string username = "nonExistingUser";
    //
    //     _userRepositoryMock.Setup(x => x.DeleteUser(It.IsAny<string>()))
    //         .Throws(new UserNotFoundRepositoryException(username));
    //
    //     // Act & Assert
    //     await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.DeleteUser(username));
    // }
}