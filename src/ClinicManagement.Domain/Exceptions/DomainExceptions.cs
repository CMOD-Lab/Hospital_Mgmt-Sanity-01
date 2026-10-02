namespace ClinicManagement.Domain.Exceptions;

/// <summary>Thrown when a requested entity is not found.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}

/// <summary>Thrown when a business rule is violated.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Thrown when a duplicate entity is detected.</summary>
public class DuplicateEntityException : Exception
{
    public DuplicateEntityException(string message) : base(message) { }
}
