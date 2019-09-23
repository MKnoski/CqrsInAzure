namespace CqrsInAzure.Candidates.Storage
{
    public interface IPhotosStorage
    {
        string GetLink(string id);
    }
}