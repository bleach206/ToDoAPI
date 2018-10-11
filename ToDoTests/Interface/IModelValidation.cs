using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoTests.Interface
{
    public interface IModelValidation
    {
        IEnumerable<ValidationResult> ValidateModels<T>(T model);
    }
}
