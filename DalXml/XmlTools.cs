//namespace Dal;
////xx
//using DO;
//using System.Xml;
//using System.Xml.Linq;
//using System.Xml.Serialization;

//static class XMLTools
//{
//    private static readonly object s_callsLock = new object();
//    private static readonly object s_volunteersLock = new object();
//    //const string s_xmlDir = @"C:\Users\bashy\source\repos\dotNet5785_1652_9845\xml"; 
//    const string s_xmlDir=@"..\xml\"; 
//    static XMLTools()
//    {
//        if (!Directory.Exists(s_xmlDir))
//            Directory.CreateDirectory(s_xmlDir);
//    }

//    #region SaveLoadWithXMLSerializer
//    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
//    {
//        string xmlFilePath = s_xmlDir + xmlFileName;

//        try
//        {
//            //DO.CallType c = CallType.Food;
//            using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
//            var ser = new XmlSerializer(typeof(List<T>));
//            ser.Serialize(file,list );
//            //new XmlSerializer(typeof(List<T>)).Serialize(file, list);
//        }
//        catch (Exception ex)
//        {
//            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
//        }
//    }
//    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
//    {
//        string xmlFilePath = s_xmlDir + xmlFileName;

//        try
//        {
//            if (!File.Exists(xmlFilePath)) return new();
//            using FileStream file = new(xmlFilePath, FileMode.Open);
//            XmlSerializer x = new(typeof(List<T>));
//            return x.Deserialize(file) as List<T> ?? new();
//        }
//        catch (Exception ex)
//        {
//            throw new DalXMLFileLoadCreateException($"fail to load xml file: {xmlFilePath}, {ex.Message}");
//        }
//    }
//    #endregion

//    #region SaveLoadWithXElement
//    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
//    {
//        string xmlFilePath = s_xmlDir + xmlFileName;

//        try
//        {
//            rootElem.Save(xmlFilePath);
//        }
//        catch (Exception ex)
//        {
//            throw new DalXMLFileLoadCreateException($"fail to create xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
//        }
//    }
//    public static XElement LoadListFromXMLElement(string xmlFileName)
//    {
//        string xmlFilePath = s_xmlDir + xmlFileName;

//        try
//        {
//            if (File.Exists(xmlFilePath))
//                return XElement.Load(xmlFilePath);
//            XElement rootElem = new(xmlFileName);
//            rootElem.Save(xmlFilePath);
//            return rootElem;
//        }
//        catch (Exception ex)
//        {
//            throw new DalXMLFileLoadCreateException($"fail to load xml file: {s_xmlDir + xmlFilePath}, {ex.Message}");
//        }
//    }
//    #endregion

//    #region XmlConfig
//    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
//        int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
//        root.Element(elemName)?.SetValue((nextId + 1).ToString());
//        XMLTools.SaveListToXMLElement(root, xmlFileName);
//        return nextId;
//    }
//    public static int GetConfigIntVal(string xmlFileName, string elemName)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
//        int num = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
//        return num;
//    }
//    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
//        //Console.WriteLine(root);
//        DateTime dt = root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
//        return dt;
//    }
//    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
//        root.Element(elemName)?.SetValue((elemVal).ToString());
//        XMLTools.SaveListToXMLElement(root, xmlFileName);
//    }
//    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
//        root.Element(elemName)?.SetValue((elemVal).ToString());
//        XMLTools.SaveListToXMLElement(root, xmlFileName);
//    }

//    public static TimeSpan GetConfigTimeSpanVal(string xmlFileName, string elemName)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
//        TimeSpan ts = root.ToTimeSpanNullable(elemName) ?? throw new FormatException($"can't convert:  {xmlFileName}, {elemName}");
//        return ts;
//    }

//    public static void SetConfigTimeSpanVal(string xmlFileName, string elemName, TimeSpan elemVal)
//    {
//        XElement root = XMLTools.LoadListFromXMLElement(xmlFileName);
//        root.Element(elemName)?.SetValue(elemVal.ToString());
//        XMLTools.SaveListToXMLElement(root, xmlFileName);
//    }

//    #endregion


//    #region ExtensionFuctions
//    public static TimeSpan? ToTimeSpanNullable(this XElement element, string name)
//    {
//        string val = element.Element(name)?.Value;
//        if (TimeSpan.TryParse(val, out TimeSpan result))
//            return result;
//        return null;
//    }

//    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
//        Enum.TryParse<T>((string?)element.Element(name), out var result) ? (T?)result : null;
//    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
//        DateTime.TryParse((string?)element.Element(name), out var result) ? (DateTime?)result : null;
//    public static double? ToDoubleNullable(this XElement element, string name) =>
//        double.TryParse((string?)element.Element(name), out var result) ? (double?)result : null;
//    public static int? ToIntNullable(this XElement element, string name) =>
//        int.TryParse((string?)element.Element(name), out var result) ? (int?)result : null;
//    #endregion

//}
namespace Dal;

using DO;
using System.Collections.Concurrent;
using System.Xml.Linq;
using System.Xml.Serialization;

static class XMLTools
{
    private static readonly ConcurrentDictionary<string, object> s_fileLocks = new();
    private static object GetLockForFile(string fileName) => s_fileLocks.GetOrAdd(fileName.ToLower(), _ => new object());

    const string s_xmlDir = @"..\xml\";

    static XMLTools()
    {
        if (!Directory.Exists(s_xmlDir))
            Directory.CreateDirectory(s_xmlDir);
    }

    #region SaveLoadWithXMLSerializer
    public static void SaveListToXMLSerializer<T>(List<T> list, string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);

        lock (fileLock)
        {
            try
            {
                using FileStream file = new(xmlFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                var ser = new XmlSerializer(typeof(List<T>));
                ser.Serialize(file, list);
            }
            catch (Exception ex)
            {
                throw new DalXMLFileLoadCreateException($"Failed to save XML file '{xmlFilePath}'.", ex);
            }
        }
    }

    public static List<T> LoadListFromXMLSerializer<T>(string xmlFileName) where T : class
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);

        lock (fileLock)
        {
            try
            {
                if (!File.Exists(xmlFilePath)) return new();
                using FileStream file = new(xmlFilePath, FileMode.Open);
                XmlSerializer x = new(typeof(List<T>));
                return x.Deserialize(file) as List<T> ?? new();
            }
            catch (Exception ex)
            {
                throw new DalXMLFileLoadCreateException($"Failed to load XML file '{xmlFilePath}'.", ex);
            }
        }
    }
    #endregion

    #region SaveLoadWithXElement
    public static void SaveListToXMLElement(XElement rootElem, string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);

        lock (fileLock)
        {
            try
            {
                rootElem.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                throw new DalXMLFileLoadCreateException($"Failed to save XElement to '{xmlFilePath}'.", ex);
            }
        }
    }

    public static XElement LoadListFromXMLElement(string xmlFileName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);

        lock (fileLock)
        {
            try
            {
                if (File.Exists(xmlFilePath))
                    return XElement.Load(xmlFilePath);

                XElement rootElem = new(xmlFileName.Replace(".xml", ""));
                rootElem.Save(xmlFilePath);
                return rootElem;
            }
            catch (Exception ex)
            {
                throw new DalXMLFileLoadCreateException($"Failed to load XElement from '{xmlFilePath}'.", ex);
            }
        }
    }
    #endregion

    #region XmlConfig (Thread-Safe)

    // כל פונקציית Get/Set צריכה להיות נעולה כדי למנוע קריאה וכתיבה בו-זמנית מאותו קובץ הגדרות.
    // פונקציות שמבצעות "קריאה-שינוי-כתיבה" צריכות לנעול את כל הפעולה.

    public static int GetAndIncreaseConfigIntVal(string xmlFileName, string elemName)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);

        lock (fileLock) // נועלים את כל הפעולה כדי להבטיח אטומיות
        {
            XElement root = XElement.Load(xmlFilePath);
            int nextId = root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert: {xmlFileName}, {elemName}");
            root.Element(elemName)?.SetValue((nextId + 1).ToString());
            root.Save(xmlFilePath);
            return nextId;
        }
    }

    public static int GetConfigIntVal(string xmlFileName, string elemName)
    {
        XElement root = LoadListFromXMLElement(xmlFileName); // הפונקציה הזו כבר נעולה
        return root.ToIntNullable(elemName) ?? throw new FormatException($"can't convert: {xmlFileName}, {elemName}");
    }

    public static DateTime GetConfigDateVal(string xmlFileName, string elemName)
    {
        XElement root = LoadListFromXMLElement(xmlFileName); // הפונקציה הזו כבר נעולה
        return root.ToDateTimeNullable(elemName) ?? throw new FormatException($"can't convert: {xmlFileName}, {elemName}");
    }

    public static TimeSpan GetConfigTimeSpanVal(string xmlFileName, string elemName)
    {
        XElement root = LoadListFromXMLElement(xmlFileName); // הפונקציה הזו כבר נעולה
        return root.ToTimeSpanNullable(elemName) ?? throw new FormatException($"can't convert: {xmlFileName}, {elemName}");
    }

    // כל פונקציות ה-Set הן "קריאה-שינוי-כתיבה" וחייבות להיות נעולות כמקשה אחת.
    public static void SetConfigIntVal(string xmlFileName, string elemName, int elemVal)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);
        lock (fileLock)
        {
            XElement root = XElement.Load(xmlFilePath);
            root.Element(elemName)?.SetValue(elemVal.ToString());
            root.Save(xmlFilePath);
        }
    }

    public static void SetConfigDateVal(string xmlFileName, string elemName, DateTime elemVal)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);
        lock (fileLock)
        {
            XElement root = XElement.Load(xmlFilePath);
            root.Element(elemName)?.SetValue(elemVal.ToString("o")); // Using ISO 8601 format for robustness
            root.Save(xmlFilePath);
        }
    }

    public static void SetConfigTimeSpanVal(string xmlFileName, string elemName, TimeSpan elemVal)
    {
        string xmlFilePath = s_xmlDir + xmlFileName;
        object fileLock = GetLockForFile(xmlFilePath);
        lock (fileLock)
        {
            XElement root = XElement.Load(xmlFilePath);
            root.Element(elemName)?.SetValue(elemVal.ToString());
            root.Save(xmlFilePath);
        }
    }

    #endregion

    #region ExtensionFunctions
    public static TimeSpan? ToTimeSpanNullable(this XElement element, string name)
    {
        string? val = element.Element(name)?.Value;
        if (TimeSpan.TryParse(val, out TimeSpan result))
            return result;
        return null;
    }

    public static T? ToEnumNullable<T>(this XElement element, string name) where T : struct, Enum =>
        Enum.TryParse<T>(element.Element(name)?.Value, out var result) ? (T?)result : null;
    public static DateTime? ToDateTimeNullable(this XElement element, string name) =>
        DateTime.TryParse(element.Element(name)?.Value, out var result) ? (DateTime?)result : null;
    public static double? ToDoubleNullable(this XElement element, string name) =>
        double.TryParse(element.Element(name)?.Value, out var result) ? (double?)result : null;
    public static int? ToIntNullable(this XElement element, string name) =>
        int.TryParse(element.Element(name)?.Value, out var result) ? (int?)result : null;
    #endregion
}