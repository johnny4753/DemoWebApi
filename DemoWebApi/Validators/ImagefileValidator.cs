using System.Web;
using ByteSizeLib;
using FluentValidation;

namespace DemoWebApi.Validators
{
    public class ImagefileValidator : AbstractValidator<HttpPostedFileBase>
    {
        private const int LimitSizeInKiloBytes = 50;
        public ImagefileValidator()
        {
            RuleFor(x => ByteSize.FromBytes(x.ContentLength).KiloBytes)
                .LessThanOrEqualTo(LimitSizeInKiloBytes)
                .WithMessage($"file size must less than {LimitSizeInKiloBytes} KB");
            RuleFor(x => x.FileName)
                .Must(x => x.EndsWith("jpg")|| x.EndsWith("png") )
                .WithMessage("must be *.jpg or *.png");
        }
    }
}