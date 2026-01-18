using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Profiles;
using HITSBlazor.Services.Tests;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.ShowUserModal
{

    //TODO: Это модальное окно многофункциональное, в том плане что вызывается в разных частях сайта, я не конкретной части
    //TODO: путь будет "profile/userId", указать этот путь каждой странице не могу, значит надо как то через срвис навигации вызывать компонент
    //TODO: Если такое реализовать, то модальное окно идей также нужно реализовать
    public partial class ShowUserModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProfileService ProfileService { get; set; } = null!;

        [Inject]
        private ITestService TestService { get; set; } = null!;

        [Parameter]
        public Guid UserId { get; set; }

        private bool isLoading = true;

        private Profile? Profile { get; set; }
        private RoleType? CurrentUserRole { get; set; }
        private TestResult? BelbinTestResult { get; set; }
        private TestResult? TemperTestResult { get; set; }
        private TestResult? MindTestResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            Profile = await ProfileService.GetUserProifleAsync(UserId);
            if (Profile is null) return;
            
            BelbinTestResult = await TestService.GetTestResultAsync(UserId, TestService.BelbinTestName);
            TemperTestResult = await TestService.GetTestResultAsync(UserId, TestService.TemperTestName);
            MindTestResult = await TestService.GetTestResultAsync(UserId, TestService.MindTestName);

            var currentUser = AuthService.CurrentUser;
            if (UserId == currentUser?.Id)
                CurrentUserRole = currentUser.Role;

            isLoading = false;
        }
    }
}
