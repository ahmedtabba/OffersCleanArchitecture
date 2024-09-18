using System.Text;
using FluentValidation.Results;

namespace Offers.CleanArchitecture.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }

    public override string Message
    {
        get
        {
            var builder = new StringBuilder();
            foreach (var error in Errors)
            {
                // Append the property name followed by its associated errors
                builder.AppendLine($"{error.Key}:");
                foreach (var message in error.Value)
                {
                    builder.AppendLine($"  - {message}");
                }
            }
            return builder.ToString().TrimEnd('\r', '\n');
        }
    }

}
