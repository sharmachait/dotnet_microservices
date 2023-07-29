using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Migrations;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/ProductAPI")]
    [ApiController]

    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _response;
        private IMapper _mapper;
        public ProductAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDTO();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDTO Get()
        {
            try
            {
                IEnumerable<Product> objList = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDTO>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO Get(int id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public ResponseDTO Post([FromForm] ProductDTO productDTO)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDTO);
                _db.Products.Add(product);
                _db.SaveChanges();

                if (productDTO.Image != null)
                {
                    string fileName=product.ProductId+Path.GetExtension(productDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDir = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using(var fileStream=new FileStream(filePathDir,FileMode.Create))
                    {
                        productDTO.Image.CopyTo(fileStream);
                    }
                    
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageLocalPath = filePath;
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                }
                else
                {
                    productDTO.ImageUrl = "https://placehold.co/600x400";
                }
                _db.Products.Update(product);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        public ResponseDTO Put([FromForm] ProductDTO productDTO)
        {
            try
            {
                var to_change = _db.Products.Any(u => u.ProductId == productDTO.ProductId);
                if (to_change == false) {
                    throw new Exception(message: "Product does not exist");
                }
                Product product = _mapper.Map<Product>(productDTO);

                if (productDTO.Image != null)
                {
                    string localPath = product.ImageLocalPath;
                    if (!string.IsNullOrEmpty(localPath))
                    {
                        var dir = Path.Combine(Directory.GetCurrentDirectory(), localPath);
                        FileInfo file = new FileInfo(dir);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDir = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDir, FileMode.Create))
                    {
                        productDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageLocalPath = filePath;
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                }
                else
                {
                    productDTO.ImageUrl = "https://placehold.co/600x400";
                }

                _db.Products.Update(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDTO Delete(int id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                string localPath = obj.ImageLocalPath;
                if (!string.IsNullOrEmpty(localPath))
                {
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), localPath);
                    FileInfo file = new FileInfo(dir);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                _db.Products.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
