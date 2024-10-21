namespace VRDR.Interfaces
{
    public interface ICommonMessage
    {
        string MessageId { get; set; }
        string MessageType { get; set; }
        string MessageSource { get; set; }
        string MessageDestination { get; set; }
        string ToXML(bool prettyPrint = false);
        string ToJSON(bool prettyPrint = false);
        string ToXml(bool prettyPrint = false);
        string ToJson(bool prettyPrint = false);


    }
}