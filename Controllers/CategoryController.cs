using EcommerceMVCAdo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceMVCAdo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IConfiguration _configuration;
        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateCategory(int? CategoryId)
        {
            Console.WriteLine(CategoryId);
            CategoryFormModel model = new CategoryFormModel();
            model.CatModel = new CategoryModel();
            model.Categories = new List<CategoryModel>();
            string ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string CatListQuery = "Select* from Category";
                    using (SqlCommand cmd = new SqlCommand(CatListQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                               // Console.Write(reader["CategoryName"]);
                                model.Categories.Add(new CategoryModel
                                {
                                    CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                    Name = reader["CategoryName"].ToString()
                                });
                            }
                        }
                    }
                    string currentCat = "SELECT CategoryId, CategoryName FROM Category WHERE CategoryId = @CategoryId";
                    if (CategoryId != null)
                    {
                        using (SqlCommand cmd = new SqlCommand(currentCat, con))
                        {
                            cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Console.WriteLine("readid" + reader["CategoryId"]);
                                    Console.WriteLine("readname" + reader["CategoryName"]);
                                    model.CatModel.CategoryId = Convert.ToInt32(reader["CategoryId"]);

                                    model.CatModel.Name = reader["CategoryName"].ToString();
                                }
                            }
                        }
                    }

                }
                return View("~/Views/Category/CategoryForm.cshtml",model);
            }
            catch(Exception ex)
            {
                return View("~/Views/Category/CategoryForm.cshtml", model);
            }
         
            
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory(CategoryFormModel model)
        {
            if (!ModelState.IsValid)
            {
                //foreach (var error in ModelState.Values
                //             .SelectMany(v => v.Errors))
                //{
                //    Console.WriteLine(error.ErrorMessage);
                //}
                string connectionstring = _configuration.GetConnectionString("DefaultConnection");
                using(SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    string query = @"Select* from Category";
                    using(SqlCommand cmd = new SqlCommand(query , con))
                    {
                        using(SqlDataReader reader = cmd.ExecuteReader())
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

                return View("~/Views/Category/CategoryForm.cshtml", model);
            }
            int? CatId = null;
            try
            {
                
                string ConnectionString = _configuration.GetConnectionString("DefaultConnection");
                if (model.CatModel.CategoryId > 0)
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    {
                        con.Open();
                        string Query = @"Update Category set CategoryName=@CategoryName where CategoryId=@CategoryId";
                        using(SqlCommand cmd = new SqlCommand(Query,con ))
                        {
                            cmd.Parameters.Add(
                                "@CategoryName",
                                SqlDbType.VarChar,
                                50
                                ).Value = model.CatModel.Name;
                            cmd.Parameters.Add(
                                "@CategoryId",
                                SqlDbType.Int
                                ).Value = model.CatModel.CategoryId;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction("CreateCategory", new { CategoryId = model.CatModel.CategoryId });
                }
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string query = @"Insert into Category(CategoryName)
                                       output Inserted.CategoryId
                                        values (@CategoryName)";
                    using (SqlCommand cmd = new SqlCommand(query,con)) 
                    {
                        cmd.Parameters.Add(
                            "@CategoryName",
                            SqlDbType.VarChar,
                            50
                        ).Value = model.CatModel.Name;
                        int CategoryId = Convert.ToInt32(cmd.ExecuteScalar());
                        CatId = CategoryId;
                    }
                }
                //Console.WriteLine("gothere1");
                return RedirectToAction("CreateCategory", new { CategoryId = CatId });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something Went Wrong!";
                return RedirectToAction("CreateCategory");
                
            }
            

        }

        public async Task<IActionResult> DeleteCategory(int? CategoryId)
        {
            if (CategoryId != null)
            {
                string connectionstring = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    string query = @"Delete from Category where CategoryId=@CategoryId";
                    using(SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@CategoryId",
                            SqlDbType.Int
                        ).Value = CategoryId;
                        cmd.ExecuteNonQuery();
                    }
                }
                
            }
            TempData["Message"] = "Category Deleted";
            return RedirectToAction("CreateCategory");
        }
    }
}
