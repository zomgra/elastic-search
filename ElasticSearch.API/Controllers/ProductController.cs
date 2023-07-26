using ElasticSearch.API.Models;
using ElasticSearch.API.Services;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearch.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;

        public ProductController(IElasticClient elasticClient, ILogger<ProductController> logger, ProductService productService)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("GetProductsIndex")]
        public async Task<IActionResult> GetProductsIndex(string keyword)
        {
            var results = await _elasticClient.SearchAsync<Product>(
                s => s.Query(sq =>
                    sq.MultiMatch(mm => mm
                        .Query(keyword)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );
            return Ok(results.Documents.ToList());
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var result = _productService.GetProducts();
            return Ok(result);
        }
        [HttpPost("AddShop")]
        public async Task<IActionResult> CreateShop(int productId, Shop newShop)
        {
            var product = _productService.GetProductById(productId);

            product.Shops.Add(newShop);

            var response = await _elasticClient.UpdateAsync<Product, object>(productId, u => u
                .Doc(new { Shops = product.Shops })
             );

            return Ok();
        }
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = _productService.DeleteProductById(productId);

            if (result)
            {
                var deleteRes = await _elasticClient.DeleteAsync<Product>(productId);
                return deleteRes.IsValid ? Ok() : BadRequest(deleteRes.Result.ToString());
            }
            return BadRequest("Invalid Id");
        }
        [HttpDelete("DeleteShop")]
        public async Task<IActionResult> DeleteShop(int shopId)
        {
            var shopProduct = _productService.DeleteShopById(shopId);

            if (shopProduct != null)
            {
                var deleteRes = await _elasticClient.UpdateAsync<Product>(shopProduct.Id, u=>u.Doc(shopProduct));
                return deleteRes.IsValid ? Ok() : BadRequest(deleteRes.Result.ToString());
            }
            return BadRequest("Invalid Id");
        }
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            _productService.AddProduct(product);

            var res = await _elasticClient.IndexDocumentAsync(product);
            
            return Ok();
        }
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            var updatedProduct = _productService.UpdateProduct(id, product);
                await _elasticClient.UpdateAsync<Product>(id, u=>
            u.Doc(updatedProduct)
            );
            return Ok(updatedProduct);
        }
    }
}
