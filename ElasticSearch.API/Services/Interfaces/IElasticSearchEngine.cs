namespace ElasticSearch.API.Services.Interfaces
{
    public interface IElasticSearchEngine<TIndex>
    {
        public Task<List<TIndex>> Search(string keyword);
        public Task<bool> UpdateIndex(TIndex index, int id);
        public Task<bool> DeleteIndex(int indexId);
        public Task<bool> CreateIndex(TIndex index);
        public Task<bool> DeleteAllIndecies();
    }
}
