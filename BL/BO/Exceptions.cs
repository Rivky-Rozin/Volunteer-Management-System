namespace BO;

//שגיאות חדשות לBO
[Serializable]
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
    public BlFormatException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidActionException : Exception
{
    public BlInvalidActionException(string? message) : base(message) { }
    public BlInvalidActionException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlValidationException : Exception
{
    public BlValidationException(string? message) : base(message) { }
    public BlValidationException(string message, Exception innerException)
        : base(message, innerException)
    { }
}

[Serializable]
public class BlPermissionException : Exception
{
    public BlPermissionException(string? message) : base(message) { }
    public BlPermissionException(string message, Exception innerException)
        : base(message, innerException)
    { }
}

[Serializable]
public class BlOperationNotAllowedException : Exception
{
    public BlOperationNotAllowedException(string? message) : base(message) { }
    public BlOperationNotAllowedException(string message, Exception innerException)
        : base(message, innerException)
    { }
}

[Serializable]
public class BlFailedToCreate : Exception
{
    public BlFailedToCreate(string? message) : base(message) { }
    public BlFailedToCreate(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlExpired : Exception
{
    public BlExpired(string? message) : base(message) { }
    public BlExpired(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyInTreatment : Exception
{
    public BlAlreadyInTreatment(string? message) : base(message) { }
    public BlAlreadyInTreatment(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlErrorAddingObject : Exception
{
    public BlErrorAddingObject(string? message) : base(message) { }
    public BlErrorAddingObject(string message, Exception innerException)
        : base(message, innerException)
    { }
}

[Serializable]
// האובייקט לא קיים
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
// האובייקט כבר קיים
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
// אסור שהאובייקט יהיה null
public class BlObjectCanNotBeNullException : Exception
{
    public BlObjectCanNotBeNullException(string? message) : base(message) { }
    public BlObjectCanNotBeNullException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
// ערך לא תואם לאנום
public class BlInputDoesNotMatchEnumException : Exception
{
    public BlInputDoesNotMatchEnumException(string? message) : base(message) { }
    public BlInputDoesNotMatchEnumException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
// קלט לא חוקי (למשל כשל ב-TryParse)
public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string? message) : base(message) { }
    public BlInvalidInputException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
// שגיאה בטעינה / יצירה של קובץ XML
public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }
    public BlXMLFileLoadCreateException(string message, Exception innerException)
        : base(message, innerException) { }
}

// שגיאת פורמט כללית (אם אתן משתמשות בזה בנפרד)
[Serializable]
public class BlGeneralException : Exception
{
    public BlGeneralException(string? message) : base(message) { }
    public BlGeneralException(string message, Exception innerException)
        : base(message, innerException) { }
}

//אין הרשאה לבטל טיפול"
[Serializable]
public class BlAuthorizationException : Exception
{
    public BlAuthorizationException(string? message) : base(message) { }
    public BlAuthorizationException(string message, Exception innerException)
        : base(message, innerException) { }
}


//"לא ניתן לבטל טיפול שכבר הסתיים"
[Serializable]
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }
    public BlInvalidOperationException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlCannotDeleteException : Exception
{
    public BlCannotDeleteException(string? message) : base(message) { }
    public BlCannotDeleteException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlArgumentException : Exception
{
    public BlArgumentException(string? message) : base(message) { }
    public BlArgumentException(string message, Exception innerException)
        : base(message, innerException) { }
}
[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException()
    {
    }

    public BLTemporaryNotAvailableException(string? message) : base(message)
    {
    }

    public BLTemporaryNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}


[Serializable]
public class BlFailedToCreateException : Exception
{
    public BlFailedToCreateException(string? message) : base(message) { }
    public BlFailedToCreateException(string message, Exception innerException)
        : base(message, innerException) { }
}

