using ClinicWebApi.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ClinicWebApi.Packages
{
    public interface IPKG_Category
    {
        public List<CategoriesModel> get_all_categories();
    }
    public class PKG_Category : PKG_Base, IPKG_Category
    {
        IConfiguration configuration;
        public PKG_Category(IConfiguration configuration) : base(configuration) { }

        public List<CategoriesModel> get_all_categories()
        {
            List<CategoriesModel> all_categories = new List<CategoriesModel>();
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_cat.get_categories";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                CategoriesModel category = new CategoriesModel();
                category.CategoryId = int.Parse(reader["id"].ToString());
                category.CategoryName = reader["category_name"].ToString();
                category.CategoryDescription = reader["category_description"].ToString();
                category.CategoryDoctorsCount = int.Parse(reader["doctors_count"].ToString());
                all_categories.Add(category);
            }

            oracleConnection.Close();
            return all_categories;
        }
    }
}
