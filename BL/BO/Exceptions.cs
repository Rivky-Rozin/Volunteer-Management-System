namespace BO;

//שגיאות חדשות לBO
[Serializable]
public class InvalidActionException : Exception
{
    public InvalidActionException(string? message) : base(message) { }
}

[Serializable]
public class ErrorAddingObject : Exception
{
    public ErrorAddingObject(string? message) : base(message) { }
    public ErrorAddingObject(string message, Exception innerException)
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
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
    public BlFormatException(string message, Exception innerException)
        : base(message, innerException) { }
}
