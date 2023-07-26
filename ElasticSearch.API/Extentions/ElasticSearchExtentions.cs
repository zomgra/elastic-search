using Elasticsearch.Net;
using ElasticSearch.API.Models;
using Nest;

namespace ElasticSearch.API.Extentions
{
    public static class ElasticSearchExtentions
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ElasticConfiguration:Uri"];
            var defaultIndex = configuration["ElasticConfiguration:Index"];

            var settings = new ConnectionSettings(new Uri(url)).PrettyJson().DefaultIndex(defaultIndex);
            AddDefaultMapping(settings);

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMapping(ConnectionSettings settings)
        {
            settings.DefaultMappingFor<Product>(p =>
                p.Ignore(x => x.Id)
            );
        }
        private static void CreateIndex(IElasticClient client,string indexName)
        {
          var responce = client.Indices.Create(indexName, index => index.Map<Product>(x => x.AutoMap()));

        }
    }
}
