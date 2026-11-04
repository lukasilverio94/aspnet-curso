using Xunit;
using Microsoft.AspNetCore.Mvc;
using TDDExample.Controllers;

public class UserRegistrationTests
{
    [Fact]
    public void RegisterUser_WithValidEmail_ReturnsOkResult()
    {
        // Arrange
        var controller = new UserController();
        var email = "test@example.com";
        // Act
        var result = controller.RegisterUser(email);
        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void RegisterUser_InvalidEmail_ThrowsBadRequest()
    {
        // Arrange
        var controller = new UserController();
        // Act
        var result = controller.RegisterUser("invalid-email");
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void RegisterUser_EmptyEmail_ThrowsBadRequest()
    {
        // Arrange
        var controller = new UserController();
        // Act
        var result = controller.RegisterUser("");
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}