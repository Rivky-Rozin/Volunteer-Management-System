namespace BO;

//שגיאות חדשות לBO
[Serializable]
public class BlInvalidActionException : Exception
{
    public BlInvalidActionException(string? message) : base(message) { }
}

[Serializable]
public class BlErrorAddingObject : Exception
{
    public BlErrorAddingObject(string? message) : base(message) { }
    public BlErrorAddingObject(string message, Exception innerException)
        : base(message, innerException)
    { }
    }

//מDO
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

