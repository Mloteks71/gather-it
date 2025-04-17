namespace Application.Interfaces;
public interface IDocumentSimilarityService
{
    bool[,] CalculateSimilarity(IList<string> stringList1, IList<string> stringList2);
}
