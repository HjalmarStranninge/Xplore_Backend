using CC_Backend.Controllers;
using CC_Backend.Models;
using CC_Backend.Services;
using CC_Backend.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using AutoFixture;
using CC_Backend.Repositories.Friends;
using CC_Backend.Repositories.Stamps;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Models.Viewmodels;


namespace CC_Backend.UnitTests.ControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepo> _userRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IFriendsRepo> _friendsRepoMock;
        private readonly Mock<IStampsRepo> _stampsRepoMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly UserController _userController;
        private readonly Fixture _fixture;

        public UserControllerTests()
        {
            // This is the setup for the mock object and MockBeghavior.Strict will throw an exception if a method is called that is not set up
            _userRepoMock = new Mock<IUserRepo>(MockBehavior.Strict);
            _emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            _friendsRepoMock = new Mock<IFriendsRepo>(MockBehavior.Strict);
            _stampsRepoMock = new Mock<IStampsRepo>(MockBehavior.Strict);
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _userController = new UserController(_userRepoMock.Object, _emailServiceMock.Object, _friendsRepoMock.Object, _stampsRepoMock.Object, _userManagerMock.Object);
        }
        [Fact]
        // Trait is a way to categorize tests and useful for filtering tests in the test explorer
        [Trait("Category", "succeed")]
        [Trait("Category", "Action")]
        public async Task GetAllUsers_should_return_all_users_when_called()
        {

            // Arrange

            // This is the setup for the GetAllUsersAsync method in the IUserRepo
            //var expectedUsers = new List<ApplicationUser>() { new ApplicationUser() { UserName = "TestUser" } };
            var expectedUsers = _fixture.CreateMany<ApplicationUser>(1).ToList();
            _userRepoMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(expectedUsers);

            // Act

            var result = await _userController.GetAllUsers();

            // Assert

            //// This is the assertion that the result is an OkObjectResult and that the value is a list of ApplicationUser
            //var okResult = result as OkObjectResult;
            //var item = Assert.IsType<List<ApplicationUser>>(okResult.Value);

            //// This is the assertion that the list contains one item
            //Assert.Equal(1, item.Count);

            // This Assert is with FluentAssertions
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUsers = okResult.Value.Should().BeAssignableTo<List<GetAllUsersViewModel>>().Subject;
            returnedUsers.Count.Should().Be(1);
            // this is the assertion that the GetAllUsersAsync method was called once
            _userRepoMock.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
        }

        //[Fact]
        //[Trait("Category", "fail")]
        //[Trait("Category", "Action")]
        //public async Task GetAllUsers_should_not_return_all_users_when_incorrectly_called()
        //{
        //    // Arrange

        //    _userRepoMock.Setup(x => x.GetAllUsersAsync())
        //        .ReturnsAsync(new List<ApplicationUser>() { new ApplicationUser() { UserName = "TestUser" } });

        //    // Act

        //    var controller = new UserController(_userRepoMock.Object, null, null);
        //    var result = await controller.GetAllUsers();

        //    // Assert

        //    //// This is the assertion that the result is an OkObjectResult and that the value is a list of ApplicationUser
        //    ///            //var okResult = result as OkObjectResult;
        //    ///                       //var item = Assert.IsType<List<ApplicationUser>>(okResult.Value);
        //    ///                       
        //    //// This is the assertion that the list contains one item
        //    ///            //Assert.Equal(1, item.Count);
        //    ///            
        //    // This Assert is with FluentAssertions
        //    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        //    var returnedUsers = okResult.Value.Should().BeAssignableTo<List<ApplicationUser>>().Subject;
        //    returnedUsers.Count.Should().Be(1);
        //    returnedUsers[0].UserName.Should().Be("TestUser");
        //    // this is the assertion that the GetAllUsersAsync method was called once
        //    _userRepoMock.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
        //}

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

        //[Fact]
        //[Trait("Category", "Unit")]
        //public async Task SndPasswordResetToken_should_send_ResetToken_when_called()
        //{
        //    // Arrange

        //}
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
