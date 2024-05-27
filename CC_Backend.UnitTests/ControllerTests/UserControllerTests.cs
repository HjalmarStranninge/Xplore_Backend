using CC_Backend.Models;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using CC_Backend.Controllers;
using CC_Backend.Repositories.UserRepo;
using CC_Backend.Repositories.FriendsRepo;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Repositories.CommentRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Moq;
using FluentAssertions;
using AutoFixture;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CC_Backend.Models.DTOs;


namespace CC_Backend.UnitTests.ControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepo> _userRepoMock;
        private readonly Mock<IFriendsRepo> _friendsRepoMock;
        private readonly Mock<IStampsRepo> _stampsRepoMock;
        private readonly Mock<ICommentRepo> _commentRepoMock;
        private readonly Mock<ILikeRepo> _likeRepoMock;
        private readonly Mock<ISearchUserService> _searchUserServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly UserController _userControllerMock;
        private readonly Fixture _fixture;

        public UserControllerTests()
        {
            _userRepoMock = new Mock<IUserRepo>();
            _friendsRepoMock = new Mock<IFriendsRepo>();
            _stampsRepoMock = new Mock<IStampsRepo>();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _searchUserServiceMock = new Mock<ISearchUserService>();
            _commentRepoMock = new Mock<ICommentRepo>();
            _likeRepoMock = new Mock<ILikeRepo>();
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _userControllerMock = new UserController(
                _userRepoMock.Object,
                _friendsRepoMock.Object,
                _stampsRepoMock.Object,
                _userManagerMock.Object,
                _commentRepoMock.Object,
                _likeRepoMock.Object,
                _searchUserServiceMock.Object
                );
        }

        [Fact]
        [Trait("Category", "succeed")]
        public async Task GetAllUsers_should_return_all_users_when_called()
        {

            // Arrange
            var expectedUsers = _fixture.CreateMany<ApplicationUser>(1).ToList();
            _userRepoMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _userControllerMock.GetAllUsers();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUsers = okResult.Value.Should().BeAssignableTo<List<GetAllUsersViewModel>>().Subject;
            returnedUsers.Count.Should().Be(1);
            _userRepoMock.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
        }

        [Theory]
        [Trait("Category", "succeed")]
        [InlineData("b", 3)]
        [InlineData("f", 1)]
        [InlineData("c", 2)]
        public async Task SearchUser_should_return_count_of_searchResults(string searchString, int expectedCount)
        {
            // Arrange
            var names = new[] { "Abel", "Bonnie", "Bert", "Felicia", "Caesar" };
            var users = names.Select(name => _fixture.Build<ApplicationUser>()
                .With(u => u.DisplayName, name)
                .Create())
                .ToList();
            var viewModels = users.Select(u => new SearchUserViewModel
            {
                DisplayName = u.DisplayName,
                ProfilePicture = u.ProfilePicture
            })
            .ToList();

            _userRepoMock.Setup(repo => repo.SearchUserAsync(It.IsAny<string>()))
                .ReturnsAsync((string query) => users.Where(u => u.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList());

            _searchUserServiceMock.Setup(service => service.GetSearchUserViewModels(It.IsAny<List<ApplicationUser>>(), It.IsAny<string>()))
                .Returns((List<ApplicationUser> users, string query) => viewModels.Where(vm => vm.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList());


            // Act
            var result = await _userControllerMock.SearchUser(searchString);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeAssignableTo<List<SearchUserViewModel>>().Subject;
            returnValue.Count.Should().Be(expectedCount);
        }

        [Fact]
        [Trait("Category", "succeed")]
        public async Task GetUserProfile_should_return_userProfile_when_called()
        {
            // Arrange
            var userId = "testUserId";
            var userProfile = _fixture.Build<ApplicationUser>()
                          .With(u => u.StampsCollected, _fixture.CreateMany<StampCollected>().ToList())
                          .Create();
            var friends = _fixture.CreateMany<FriendViewModel>(3).ToList();
            var stamps = _fixture.CreateMany<StampViewModel>().ToList();

            SetupUserMock(userId, userProfile);
            SetupFriendsMock(friends);
            _stampsRepoMock.Setup(repo => repo.GetStampsFromUserAsync(It.IsAny<string>())).ReturnsAsync(stamps);

            //// Mock User property of the controller
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId"),
            }, "mock"));
            _userControllerMock.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _userControllerMock.GetUserProfile();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeAssignableTo<GetUserProfileViewmodel>().Subject;
            returnValue.DisplayName.Should().Be(userProfile.DisplayName);
            returnValue.ProfilePicture.Should().NotBeEmpty();
            returnValue.StampsCollectedTotalCount.Should().Be(userProfile.StampsCollected.Count);
            returnValue.FriendsCount.Should().Be(friends.Count);
            returnValue.Friends.Should().BeEquivalentTo(friends);
            returnValue.StampCollectedTotal.Should().BeEquivalentTo(stamps); ;
        }

        [Fact]
        [Trait("Category", "succeed")]
        public async Task GetUserFeed_should_return_UserFeed_when_called()
        {
            // Arrange
            var userId = "testUserId";
            var userProfile = _fixture.Build<ApplicationUser>()
                          .With(u => u.StampsCollected, _fixture.CreateMany<StampCollected>().ToList())
                          .Create();
            var friends = _fixture.CreateMany<FriendViewModel>(3).ToList();
            var stampsCollected = _fixture.CreateMany<StampCollected>().ToList();
            var comments = _fixture.CreateMany<CommentViewModel>().ToList();
            var likes = _fixture.CreateMany<LikeViewModel>().ToList();

            SetupUserMock(userId, userProfile);
            SetupFriendsMock(friends);
            SetupStampsMock(stampsCollected);
            SetupCommentsAndLikesMock(comments, likes);

            // Act
            var result = await _userControllerMock.GetUserFeed();
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedFeed = okResult.Value.Should().BeAssignableTo<IEnumerable<UserFeedViewmodel>>().Subject;
            returnedFeed.Should().NotBeEmpty();
        }

        [Theory]
        [Trait("Category", "succeed")]
        [InlineData("test")]
        [InlineData("test1")]
        public async Task GetUserProfileByDisplayname_should_return_UserProfile_when_called(string displayName)
        {
            // Arrange
            var userId = "testUserId";
            var userProfile = _fixture.Build<ApplicationUser>()
                          .With(u => u.StampsCollected, _fixture.CreateMany<StampCollected>().ToList())
                          .With(u => u.DisplayName, displayName)
                          .Create();
            var friends = _fixture.CreateMany<FriendViewModel>(3).ToList();
            var stampsCollected = _fixture.CreateMany<StampCollected>().ToList();
            var dto = new GetUserProfileByDisplaynameDTO { DisplayName = userProfile.DisplayName };

            SetupUserMock(userId, userProfile);
            SetupFriendsMock(friends);
            SetupStampsMock(stampsCollected);

            // Act
            var result = await _userControllerMock.GetUserProfileByDisplayname(dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUserProfile = okResult.Value.Should().BeAssignableTo<GetUserProfileViewmodel>().Subject;
            returnedUserProfile.Should().NotBeNull();
            returnedUserProfile.DisplayName.Should().Be(displayName);
            returnedUserProfile.ProfilePicture.Should().NotBeNull();
        }

        [Fact]
        [Trait("Category", "succeed")]
        public async Task SetProfilePicture_should_set_a_byteArray_as_profilePicture_when_called()
        {
            // Arrange
            var userId = "testUserId";
            var userProfile = _fixture.Build<ApplicationUser>()
                          .With(u => u.StampsCollected, _fixture.CreateMany<StampCollected>().ToList())
                          .Create();

            var dto = new SetProfilePictureDTO { ProfilePicture = userProfile.ProfilePicture };
            dto.ProfilePicture.Should().BeOfType<byte[]>();

            SetupUserMock(userId, userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId"),
            }, "mock"));
            _userControllerMock.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _userRepoMock.Setup(repo => repo.SetProfilePicAsync(It.IsAny<string>(), It.IsAny<byte[]>())).ReturnsAsync(true);


            // Act
            var result = await _userControllerMock.SetProfilePicture(dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeOfType<bool>().Which.Should().BeTrue();
        }

        private void SetupUserMock(string userId, ApplicationUser userProfile)
        {
            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(userProfile);
            _userRepoMock.Setup(repo => repo.GetUserByDisplayNameAsync(It.IsAny<string>())).ReturnsAsync(userProfile);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId"),
            },
            "mock"));
            _userControllerMock.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
        private void SetupFriendsMock(List<FriendViewModel> friends)
        {
            _friendsRepoMock.Setup(repo => repo.GetFriendsAsync(It.IsAny<string>())).ReturnsAsync(friends);
        }

        private void SetupStampsMock(List<StampCollected> stampsCollected)
        {
            _stampsRepoMock.Setup(repo => repo.GetStampsCollectedFromUserAsync(It.IsAny<string>())).ReturnsAsync(stampsCollected);
            _stampsRepoMock.Setup(repo => repo.GetCategoryFromStampAsync(It.IsAny<int>())).ReturnsAsync(new CategoryViewModel());
        }

        private void SetupCommentsAndLikesMock(List<CommentViewModel> comments, List<LikeViewModel> likes)
        {
            _commentRepoMock.Setup(repo => repo.GetCommentsFromStampCollectedAsync(It.IsAny<int>())).ReturnsAsync(comments);
            _likeRepoMock.Setup(repo => repo.GetLikesFromStampCollected(It.IsAny<int>())).ReturnsAsync(likes);
        }

    }
}
