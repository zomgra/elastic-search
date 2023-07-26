using Elasticsearch.Net;
using Nest;
using System.Text.Json.Serialization;

namespace ElasticSearch.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quality { get; set; }
        public int Price { get; set; }
        /// <summary>
        /// Product shops
        /// </summary>
        [Nested]
        public ICollection<Shop> Shops { get; set; } = new List<Shop>();

        /// <summary>
        /// Update props product CAN`T UPDATE SHOPS
        /// </summary>
        /// <param name="product">Updated products, with out shops</param>
        public void Update(Product product)
        {
            Name = product.Name;
            Quality = product.Quality;
            Description = product.Description;
            Price = product.Price;
        }
    }
}