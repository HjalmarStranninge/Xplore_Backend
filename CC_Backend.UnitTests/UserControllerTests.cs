using CC_Backend.Controllers;
using CC_Backend.Models;
using CC_Backend.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;


namespace CC_Backend.UnitTests
{
    public class UserControllerTests
    {
        // This is the mock object that will be used to simulate the IUserRepo
        private readonly Mock<IUserRepo> _userRepoMock;

        public UserControllerTests()
        {
            // This is the setup for the mock object and MockBeghavior.Strict will throw an exception if a method is called that is not set up
            _userRepoMock = new Mock<IUserRepo>(MockBehavior.Strict);
        }
        // Fact is a test that does not take any parameters
        [Fact]
        // Trait is a way to categorize tests and useful for filtering tests in the test explorer
        [Trait("Category", "Unit")]
        public async Task GetAllUsers_should_return_all_users_when_called()
        {

            // Arrange
            // This is the setup for the GetAllUsersAsync method in the IUserRepo
            _userRepoMock.Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(new List<ApplicationUser>() { new ApplicationUser() { UserName = "TestUser" } });

            // Act
            // This is the creation of the UserController with the mock object. the controller instatiates the userRepoMock and result calls the method GetAllUsers
            var controller = new UserController(_userRepoMock.Object, null, null);
            var result = await controller.GetAllUsers();

            // Assert

            //// This is the assertion that the result is an OkObjectResult and that the value is a list of ApplicationUser
            //var okResult = result as OkObjectResult;
            //var item = Assert.IsType<List<ApplicationUser>>(okResult.Value);

            //// This is the assertion that the list contains one item
            //Assert.Equal(1, item.Count);

            // This Assert is with FluentAssertions
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUsers = okResult.Value.Should().BeAssignableTo<List<ApplicationUser>>().Subject;
            returnedUsers.Count.Should().Be(1);
            returnedUsers[0].UserName.Should().Be("TestUser");
            // this is the assertion that the GetAllUsersAsync method was called once
            _userRepoMock.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
        }

        //// Theory is a test that takes parameters
        //[Theory]
        //// InlineData is the data that will be passed to the test and is applied to one parameter
        //[InlineData("Alex")]
        //[InlineData("Fredrich")]
        //[InlineData("Hjalmar")]
        //[InlineData("Huan")]
        //[InlineData("Niklas")]

        ////ClassData is different from InlineData because it is applied to multiple parameters, isn't constant and can be reused
        //[ClassData(typeof(ClassData))]

        ////MemberData is different from ClassData because it uses a method to provide the data and it can be reused
        //[MemberData(nameof(Input)]

        //// Customattribute can also be used but have to be created first
        //[Input]

    }

    ////Customattribute example
    //public class Input : DataAttribute 
    //{ 
    //}

    ////Theorydata example that uses the ClassData
    //public class ClassData : TheoryData
    //{
    //    public ClassData()
    //    {
    //        AddRow("Alex");
    //        AddRow("Fredrich");
    //        AddRow("Hjalmar");
    //        AddRow("Huan");
    //        AddRow("Niklas");
    //    }
    //}

}
