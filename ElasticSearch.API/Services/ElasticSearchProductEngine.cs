using ElasticSearch.API.Models;
using ElasticSearch.API.Services.Interfaces;
using Nest;

namespace ElasticSearch.API.Services
{
    public class ElasticSearchProductEngine : IElasticSearchEngine<Product>
    {
        private readonly IElasticClient _elasticClient;

        public ElasticSearchProductEngine(IElasticClient client)
        {
            _elasticClient = client;
        }

        public async Task<bool> CreateIndex(Product index)
        {
            var result = await _elasticClient.IndexDocumentAsync(index);
            return result.IsValid;
        }

        public async Task<bool> DeleteAllIndecies()
        {
            var deleteResult = await _elasticClient.DeleteByQueryAsync<Product>(d=>d.Query(
                q=>q.MatchAll()));
            return deleteResult.IsValid;
        }

        public async Task<bool> DeleteIndex(int indexId)
        {
            var deleteResult = await _elasticClient.DeleteAsync<Product>(indexId);
            return deleteResult.IsValid;
        }

        public async Task<List<Product>> Search(string keyword)
        {
            var result = await _elasticClient.SearchAsync<Product>(
                s => s.Query(sq =>
                    sq.MultiMatch(mm => mm
                        .Query(keyword)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return result.Documents.ToList();
        }

        public async Task<bool> UpdateIndex(Product index, int productId)
        {
           var updateResult = await _elasticClient.UpdateAsync<Product>(productId, u => u
                .Doc(index)
             );
            return updateResult.IsValid;
        }
    }
}
