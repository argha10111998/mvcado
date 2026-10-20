using EcommerceMVCAdo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceMVCAdo.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public ProductController(IConfiguration configuration) 
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateProduct(int? ProductId)
        {
            try
            {
                ProductFormModel model = new ProductFormModel();
                if (ProductId != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        string query = @"Select* from Product where ProductId=@ProductId";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.Add(
                                "@ProductId",
                                SqlDbType.Int
                                ).Value = ProductId;

                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                if (reader.Read())
                                {
                                    model.ProductModel.ProductId = Convert.ToInt32(reader["ProductId"]);
                                    model.ProductModel.ProductName = (reader["ProductName"]).ToString();
                                    model.ProductModel.ProductPrice = Convert.ToDecimal(reader["ProductPrice"]);
                                    model.ProductModel.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                                    model.ProductModel.Stock = Convert.ToInt32(reader["Stock"]);
                                }
                            }
                        }
                    }
                }
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        string query = @"Select* from Category";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    model.Categories.Add(new CategoryModel
                                    {
                                        CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                        Name = (reader["CategoryName"]).ToString()
                                    });
                                }
                            }
                        }
                    }
                    
                
                return View("~/Views/Product/ProductForm.cshtml", model);
            }
            catch (Exception ex)
            {
                ProductFormModel model = new ProductFormModel();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    string query = @"Select* from Category";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                model.Categories.Add(new CategoryModel
                                {
                                    CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                    Name = (reader["CategoryName"]).ToString()
                                });
                            }
                        }
                    }
                }
                return View("~/Views/Product/ProductForm.cshtml", model);
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Product/ProductForm.cshtml",model);
            }

            try
            {
                int productId = 0;
                if (model.ProductModel.ProductId==0)
                { 
                
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    string query = @"Insert into Product(ProductName,ProductPrice,Stock,CategoryId)
                                    Output Inserted.*
                                    values(@ProductName,@ProductPrice,@Stock,@CategoryId);";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@ProductName",
                            SqlDbType.NVarChar,
                            50
                            ).Value = model.ProductModel.ProductName;
                        cmd.Parameters.Add(
                            "@ProductPrice",
                            SqlDbType.NVarChar,
                            10
                            ).Value = model.ProductModel.ProductPrice;
                        cmd.Parameters.Add(
                            "@CategoryId",
                            SqlDbType.Int
                            ).Value = model.ProductModel.CategoryId;
                        cmd.Parameters.Add(
                            "@Stock",
                            SqlDbType.Int
                            ).Value = model.ProductModel.Stock;
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                productId = Convert.ToInt32(reader["ProductId"]);
                            }
                        }
                    }
                }
                TempData["Message"] = "Product Saved Successfully!";
                }
                else
                {
                    using(SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        Console.WriteLine(model.ProductModel.ProductId);
                        Console.WriteLine(model.ProductModel.ProductName);
                        string query = @"Update Product set ProductName=@ProductName,ProductPrice=@ProductPrice,CategoryId=@CategoryId where ProductId=@ProductId";
                        using(SqlCommand cmd = new SqlCommand(query,con))
                        {
                            cmd.Parameters.Add(
                                "@ProductName",
                                SqlDbType.NVarChar,
                                50
                            ).Value = model.ProductModel.ProductName;
                            cmd.Parameters.Add(
                                "@ProductId",
                                SqlDbType.Int
                                
                            ).Value = model.ProductModel.ProductId;
                            cmd.Parameters.Add(
                                "@ProductPrice",
                                SqlDbType.NVarChar,
                                10

                            ).Value = model.ProductModel.ProductPrice;
                            cmd.Parameters.Add(
                                "@CategoryId",
                                SqlDbType.Int

                            ).Value = model.ProductModel.CategoryId;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    productId = model.ProductModel.ProductId;
                    TempData["Message"] = "Product Updated Successfully!";
                }
                return RedirectToAction("CreateProduct", new {ProductId = productId });
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
                TempData["Message"] = "Somethinh Went Wrong!";
                return RedirectToAction("CreateProduct");
            }
        }
    }
}
