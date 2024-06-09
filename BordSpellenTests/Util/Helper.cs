using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BordSpellenTests.Util;

public static class Helper
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

        mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
        mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, _) => ls.Add(x));
        mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

        return mgr;
    }

    public static void ValidateModel(this Controller controller, object viewModel)
    {
        controller.ModelState.Clear();


        var validationContext = new ValidationContext(viewModel, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(viewModel, validationContext, validationResults,true);

        foreach (var result in validationResults)
        {
            foreach (var name in result.MemberNames)
            {
                controller.ModelState.AddModelError(name, result.ErrorMessage);
            }
        }
    }
}