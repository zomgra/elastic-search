using ElasticSearch.API.Models;

namespace ElasticSearch.API.Services
{
    /// <summary>
    /// Imitated db service
    /// </summary>
    public class ProductService
    {
        private List<Product> _products;
        public ProductService()
        {
            _products = new();
        }

        public List<Product> GetProducts()
        {
            return _products;
        }

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }
        public Product GetProductById(int id)
        {
            var product = _products.FirstOrDefault(x => x.Id == id);
            if (product == null) throw new ArgumentException();
            return product;
        }
        public Product GetProductByShop(Shop shop)
        {
            var product = _products.FirstOrDefault(x => x.Shops.Contains(shop));
            if (product == null) throw new ArgumentException();
            return product;
        }
        public Shop GetShopByShopId(int id)
        {
            var shop = _products.SelectMany(product => product.Shops)
                        .FirstOrDefault(shop => shop.Id == id);
             if(shop == null) throw new ArgumentException();
             return shop;
        }
        public bool DeleteProductById(int id) 
        { 
            var product = GetProductById(id);
            _products.Remove(product);
            return true;
        }
        public Product DeleteShopById(int id)
        {
            var shop = GetShopByShopId(id);
            var product = GetProductByShop(shop);
            product.Shops.Remove(shop);
            return product;
        }
        public Product UpdateProduct(int id, Product newProduct)
        {
            var product = GetProductById(id);
            product.Update(newProduct);
            return product;
        }
    }
}
