// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace DBC.Test
{
    public static class MockHelpers
    {
        public static StringBuilder LogMessage = new StringBuilder();
        private static string AnyString => It.IsAny<string>();
        public static Mock<SignInManager<TUser>> MockSignInManager2<TUser>() where TUser : class
        {
            var context = new Mock<HttpContext>();
            var manager = MockUserManager2<TUser>();
            return new Mock<SignInManager<TUser>>(manager.Object,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null, null)
            { CallBase = true };
        }

        public static Mock<UserManager<TUser>> MockUserManager2<TUser>() where TUser : class
        {
            IList<IUserValidator<TUser>> userValidators = new List<IUserValidator<TUser>>();
            IList<IPasswordValidator<TUser>> passwordValidators = new List<IPasswordValidator<TUser>>();

            var store = new Mock<IUserStore<TUser>>();
            userValidators.Add(new UserValidator<TUser>());
            passwordValidators.Add(new PasswordValidator<TUser>());
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, userValidators, passwordValidators, null, null, null, null, null);
            return mgr;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole> store = null) where TRole : class
        {
            store = store ?? new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>>();
            roles.Add(new RoleValidator<TRole>());
            return new Mock<RoleManager<TRole>>(store, roles, null, null, null, null);
        }


        //Mock UserManager
        public static UserManager<TUser> MockUserManager<TUser>() where TUser : class
        {
            return MockUserManager<TUser>(null);
        }
        public static UserManager<TUser> MockUserManager<TUser>(TUser user, string generateTwoFactorToken) where TUser : class
        {
            return MockUserManager<TUser>(user, null, false, generateTwoFactorToken);
        }
        public static UserManager<TUser> MockUserManager<TUser>(TUser user, IdentityResult confirmEmailAsync) where TUser : class
        {
            return MockUserManager<TUser>(user, confirmEmailAsync, false, null);
        }
        public static UserManager<TUser> MockUserManager<TUser>(TUser user, bool isEmailConfirmedAsync) where TUser : class
        {
            return MockUserManager<TUser>(user, null, isEmailConfirmedAsync, null);
        }
        public static UserManager<TUser> MockUserManager<TUser>(TUser user) where TUser : class
        {
            return MockUserManager<TUser>(user, null, false, null);
        }
        public static UserManager<TUser> MockUserManager<TUser>(TUser user, IdentityResult identityResult, bool isEmailConfirmedAsync, string generateTwoFactorToken) where TUser : class
        {
            IList<string> userFactors = new List<string>() { "purpose1", "purpose2" };
            IList<IUserValidator<TUser>> userValidators = new List<IUserValidator<TUser>>();
            IList<IPasswordValidator<TUser>> passwordValidators = new List<IPasswordValidator<TUser>>();
            var store = new Mock<IUserStore<TUser>>();
            userValidators.Add(new UserValidator<TUser>());
            passwordValidators.Add(new PasswordValidator<TUser>());
            var mockUserManager = new Mock<UserManager<TUser>>(store.Object, null, null, userValidators, passwordValidators, null, null, null, null, null);
            mockUserManager.Setup(m => m.FindByEmailAsync(AnyString)).Returns(Task.FromResult(user));
            mockUserManager.Setup(m => m.FindByIdAsync("userId")).Returns(Task.FromResult(user));
            mockUserManager.Setup(m => m.GetValidTwoFactorProvidersAsync(It.IsAny<TUser>())).Returns(Task.FromResult(result: userFactors));
            mockUserManager.Setup(m => m.ConfirmEmailAsync(It.IsAny<TUser>(), "code")).Returns(Task.FromResult(identityResult));
            mockUserManager.Setup(m => m.ResetPasswordAsync(It.IsAny<TUser>(), "token", "password")).Returns(Task.FromResult(identityResult));
            mockUserManager.Setup(m => m.IsEmailConfirmedAsync(It.IsAny<TUser>())).Returns(Task.FromResult(isEmailConfirmedAsync));
            mockUserManager.Setup(m => m.GeneratePasswordResetTokenAsync(It.IsAny<TUser>())).Returns(Task.FromResult("PasswordResetToken"));
            mockUserManager.Setup(m => m.GenerateTwoFactorTokenAsync(It.IsAny<TUser>(), "Email")).Returns(Task.FromResult(generateTwoFactorToken));
            mockUserManager.Setup(m => m.GetEmailAsync(It.IsAny<TUser>())).Returns(Task.FromResult("EmailAddress@"));


            return mockUserManager.Object;
        }

        //mock SignInManager
        public static SignInManager<TUser> MockSignInManager<TUser>() where TUser : class
        {
            return MockSignInManager<TUser>(SignInResult.Success);
        }
        public static SignInManager<TUser> MockSignInManager<TUser>(TUser user) where TUser : class
        {
            return MockSignInManager(user, SignInResult.Success, SignInResult.Success, null);
        }
        public static SignInManager<TUser> MockSignInManager<TUser>(SignInResult signInResult) where TUser : class
        {
            return MockSignInManager<TUser>(signInResult, null);
        }
        public static SignInManager<TUser> MockSignInManager<TUser>(SignInResult signInResult, ExternalLoginInfo info) where TUser : class
        {
            return MockSignInManager<TUser>(signInResult, signInResult, info);
        }
        public static SignInManager<TUser> MockSignInManager<TUser>(SignInResult signInResult1, SignInResult signInResult2, ExternalLoginInfo info) where TUser : class
        {
            return MockSignInManager<TUser>(null, signInResult1, signInResult2, info);
        }
        public static SignInManager<TUser> MockSignInManager<TUser>(TUser user, SignInResult signInAsync1, SignInResult signInAsync2, ExternalLoginInfo info) where TUser : class
        {
            var mockSignInManager = MockSignInManager2<TUser>();
            mockSignInManager.Setup(m => m.GetExternalLoginInfoAsync(It.IsAny<string>())).Returns(Task.FromResult<ExternalLoginInfo>(info));
            var tasks = new Queue<Task<SignInResult>>();
            tasks.Enqueue(Task.FromResult<SignInResult>(signInAsync1));
            tasks.Enqueue(Task.FromResult<SignInResult>(signInAsync2));
            mockSignInManager.Setup(m => m.PasswordSignInAsync(AnyString, AnyString, It.IsAny<bool>(), It.IsAny<bool>())).Returns(tasks.Dequeue);
            mockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<TUser>(), AnyString, It.IsAny<bool>(), It.IsAny<bool>())).Returns(tasks.Dequeue);
            mockSignInManager.Setup(m => m.ExternalLoginSignInAsync(AnyString, AnyString, It.IsAny<bool>())).Returns(tasks.Dequeue);
            mockSignInManager.Setup(m => m.GetTwoFactorAuthenticationUserAsync()).Returns(Task.FromResult(user));
            mockSignInManager.Setup(m => m.TwoFactorSignInAsync(AnyString, AnyString, It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(signInAsync1));
            return mockSignInManager.Object;
        }
        public static ExternalLoginInfo DefaultInfo()
        {
            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            mockClaimsPrincipal.Setup(m => m.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.Email, "emailaddress@"));
            return new Mock<ExternalLoginInfo>(mockClaimsPrincipal.Object, "google", "google", "google").Object;
        }
    }

}
