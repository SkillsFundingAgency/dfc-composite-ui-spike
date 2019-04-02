using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Courses.Models
{
    public class BaseEditViewModel
    {
        protected const string FieldLengthValidationError = "'{0}' must be {1} characters or less";
        protected const string FieldMandatoryhValidationError = "Enter {0}";
    }
}
