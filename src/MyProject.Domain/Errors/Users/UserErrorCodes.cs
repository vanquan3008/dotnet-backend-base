namespace MyProject.Domain.Errors.Users;

public static class UserErrorCodes
{
    public const string IdRequired = "user.id_required";
    public const string EmailRequired = "user.email_required";
    public const string EmailInvalid = "user.email_invalid";
    public const string EmailTooLong = "user.email_too_long";
    public const string CodeRequired = "user.code_required";
    public const string CodeTooLong = "user.code_too_long";
    public const string CodeInvalidFormat = "user.code_invalid_format";
    public const string NameRequired = "user.name_required";
    public const string NameTooShort = "user.name_too_short";
    public const string NameTooLong = "user.name_too_long";
    public const string CodeAlreadyExists = "user.code_already_exists";
    public const string EmailAlreadyExists = "user.email_already_exists";
    public const string NotActive = "user.not_active";
    public const string Deleted = "user.deleted";
}
