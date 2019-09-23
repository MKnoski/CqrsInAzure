namespace CqrsInAzure.Candidates.Storage
{
    public interface ICvStorage
    {
        string GetLink(string id);
    }
}