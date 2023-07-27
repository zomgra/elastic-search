using ElasticSearch.API.Models;
using ElasticSearch.API.Services;
using ElasticSearch.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace ElasticSearch.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;
        private readonly IElasticSearchEngine<Product> _searchEngine;

        public ProductController(ILogger<ProductController> logger, ProductService productService, IElasticSearchEngine<Product> searchEngine)
        {
            _logger = logger;
            _productService = productService;
            _searchEngine = searchEngine;
        }

        [HttpGet("GetProductsIndex")]
        public async Task<IActionResult> GetProductsIndex(string keyword)
        {
            var results = await _searchEngine.Search(keyword);

            _logger.LogInformation($"Searched {results.Count} products index");

            return Ok(results);
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var result = _productService.GetProducts();
            _logger.LogInformation($"Returned {result.Count} products");
            return Ok(result);
        }
        [HttpPost("AddShop")]
        public async Task<IActionResult> CreateShop(int productId, Shop newShop)
        {
            var product = _productService.GetProductById(productId);
            product.Shops.Add(newShop);

            var response = await _searchEngine.UpdateIndex(product, productId);
            _logger.LogInformation($"Created new {newShop.Id} shop");

            return Ok();
        }
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            _productService.AddProduct(product);

            var res = await _searchEngine.CreateIndex(product);

            _logger.LogInformation($"Created new {product.Id} product");

            return Ok();
        }
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            var updatedProduct = _productService.UpdateProduct(id, product);
            await _searchEngine.UpdateIndex(updatedProduct, id);

            _logger.LogInformation($"Updated {id} product");

            return Ok(updatedProduct);
        }
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = _productService.DeleteProductById(productId);

            if (result)
            {
                var deleteRes = await _searchEngine.DeleteIndex(productId);
                return deleteRes ? Ok() : BadRequest();
            }
            return BadRequest("Invalid Id");
        }
        [HttpDelete("DeleteShop")]
        public async Task<IActionResult> DeleteShop(int shopId)
        {
            var shopProduct = _productService.DeleteShopById(shopId);

            if (shopProduct != null)
            {
                var deleteRes = await _searchEngine.UpdateIndex(shopProduct, shopProduct.Id);
                return deleteRes ? Ok() : BadRequest();
            }
            return BadRequest("Invalid Id");
        }

    }
}
