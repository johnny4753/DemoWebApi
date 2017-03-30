using System.Web;
using DemoWebApi.Validators;
using FluentValidation.Attributes;

namespace DemoWebApi.Models
{
    [Validator(typeof(HomeUploadViewModelValidator))]
    public class HomeUploadViewModel
    {
        public HttpPostedFileBase Imagefile { get; set; }
    }
}