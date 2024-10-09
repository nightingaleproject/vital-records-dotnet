namespace VRDR.Interfaces
{
    public interface ICommonMessage
    {
        string MessageId { get; set; }
        string MessageType { get; set; }
        string MessageSource { get; set; }
        string MessageDestination { get; set; }


    }
}