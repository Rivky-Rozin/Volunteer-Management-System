namespace BO;

[Serializable]

// This exception is thrown when the requested object does not exist in the system.
//החריגה תיזרק למשל כאשר BL מנסה, דרך פניה ל DAL, לעדכן אובייקט עם מספר מזהה שלא קיים ברשימת האובייקטים מסוג כלשהו. ואשר בעקבותיו, DAL זרקה חריגה משלה. בתגובה BL, זורק חריגה מקבילה כלפי מעלה ל PL.

public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}


[Serializable]
// This exception is thrown when the requested object already exists in the system.
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

[Serializable]
public class FormatException : Exception
{
    public FormatException(string? message) : base(message) { }
}