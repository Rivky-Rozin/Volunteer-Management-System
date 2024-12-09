
namespace DO;

//האובייקט לא קיים
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}



//האובייקט כבר קיים
[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}

//היינו צריכות להוסיף שגיאה בשביל המקומות שבהם אסור לאובייקט להיות NULL
[Serializable]
public class DalObjectCanNotBeNull : Exception
{
    public DalObjectCanNotBeNull(string? message) : base(message) { }
}

//השתמשנו בשגיאה זו במקומות שהיינו צריכים להתריע על קלט שלא מתאים לENUM
[Serializable]
public class DalInputDoesNotMatchEnum : Exception
{
    public DalInputDoesNotMatchEnum(string? message) : base(message) { }
}

//השתמשנו בשגיאה זו במקומות שהיה צריך לזרוק שגיאה בעקבות קלט לא חוקי. לדוגמה אחרי TRYPARSE
public class DalInvalidInput : Exception
{
    public DalInvalidInput(string? message) : base(message) { }
}


