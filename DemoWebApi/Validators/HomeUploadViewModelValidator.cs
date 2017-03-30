using DemoWebApi.Models;
using FluentValidation;

namespace DemoWebApi.Validators
{
    public class HomeUploadViewModelValidator : AbstractValidator<HomeUploadViewModel>
    {
        public HomeUploadViewModelValidator()
        {
            RuleFor(x => x.Imagefile).NotNull().WithMessage("Please choose a file");
            RuleFor(x => x.Imagefile).SetValidator(new ImagefileValidator());
        }
    }
}