using ClinicWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ClinicWebApi.Packages
{
    public interface IPKG_Employee
    {
        public void save_emp(EmployeeModel employee, byte[] pic, byte[] cv);
        public List<EmployeeModel>  get_emp();
        public EmployeeModel get_emp_by_id(int emp_id);
        public List<EmployeeModel> filter_emp_by_fullname(string fullName);
        public List<EmployeeModel> filter_emp_by_category(string category);
        public void delete_emp(int id);
        public void update_emp(EmployeeModel employee);
        public List<EmployeeModel> get_emp_lazy_load(int last_person_id);
        public void update_view_count(int user_id);
        public EmployeeModel get_emp_by_emp_id(int emp_id);
    }
    public class PKG_Employee : PKG_Base, IPKG_Employee
    {
        IConfiguration configuration;
        public PKG_Employee(IConfiguration configuration) : base(configuration) { }

        public void save_emp(EmployeeModel employee, byte[] pic, byte[] cv)
        {
            using (var oracleConnection = new OracleConnection(ConnectionStr))
            {
                oracleConnection.Open();

                using (var cmd = new OracleCommand("olerning.pkg_lo_clinic_emp.save_emp", oracleConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = employee.First_Name;
                    cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = employee.Last_Name;
                    cmd.Parameters.Add("p_birthday", OracleDbType.Date).Value = employee.Birthday;
                    cmd.Parameters.Add("p_gender", OracleDbType.Int32).Value = employee.Gender;
                    cmd.Parameters.Add("P_idcard_id", OracleDbType.Varchar2).Value = employee.Id_card;
                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = employee.Email;
                    cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = employee.Phone;
                    cmd.Parameters.Add("p_role_id", OracleDbType.Int32).Value = employee.Role_id;
                    cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = employee.Password;

                    if (pic != null)
                    {
                        cmd.Parameters.Add("p_picture", OracleDbType.Blob).Value = pic;
                    }
                    else
                    {
                        cmd.Parameters.Add("p_picture", OracleDbType.Blob).Value = DBNull.Value;
                    }

                    cmd.Parameters.Add("p_category_id", OracleDbType.Int32).Value = employee.Category_id > 1 ? employee.Category_id : (object)DBNull.Value;

                    if (cv != null)
                    {
                        cmd.Parameters.Add("p_cv", OracleDbType.Blob).Value = cv;
                    }
                    else
                    {
                        cmd.Parameters.Add("p_cv", OracleDbType.Blob).Value = DBNull.Value;
                    }

                    cmd.ExecuteNonQuery();
                }
                oracleConnection.Close();
            }
        }

        public List<EmployeeModel> get_emp()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            OracleConnection oracleconnection = new OracleConnection();
            oracleconnection.ConnectionString = ConnectionStr;
            oracleconnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleconnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.get_doctors";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string fullName = reader["full_name"] != DBNull.Value ? Convert.ToString(reader["full_name"]) : string.Empty;
                string[] nameParts = fullName.Split(' ');

                string firstname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                string lastname = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                EmployeeModel emp = new EmployeeModel();
                emp.Id = int.Parse(reader["id"].ToString());
                emp.First_Name = firstname;
                emp.Last_Name = lastname;


                emp.Birthday = Convert.ToDateTime(reader["birthday"]);
                emp.Gender_str = reader["gender"] != DBNull.Value ? Convert.ToString(reader["gender"]) : string.Empty;

                emp.Phone = reader["phone"] != DBNull.Value ? Convert.ToString(reader["phone"]) : string.Empty; 
                emp.Role_id = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0;
                emp.Review_count = reader["avg_score"] != DBNull.Value ? Convert.ToDouble(reader["avg_score"].ToString()) : 5;
                emp.Category_name = reader["category_name"] != DBNull.Value ? Convert.ToString(reader["category_name"]) : string.Empty;
                emp.View_count = reader["view_count"] != DBNull.Value ? int.Parse(Convert.ToString(reader["view_count"])) : 0;

                var pictureData = reader["picture"] as byte[];

                if (pictureData != null)
                {
                    emp.PicData = Convert.ToBase64String(pictureData);
                }
                else
                {
                    emp.PicData = null;
                }

                employees.Add(emp);


            }
            oracleconnection.Close();
            return employees;
        }
        public EmployeeModel get_emp_by_id(int emp_id)
        {
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.get_emp_by_id";
            cmd.CommandType =  CommandType.StoredProcedure;
            
            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = emp_id;  

            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                EmployeeModel emp = new EmployeeModel();

                string fname = reader["full_name"] != DBNull.Value ? Convert.ToString(reader["full_name"]) : string.Empty;
                string[] nameParts = fname.Split(' ');

                string firstname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                string lastname = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                emp.Id = int.Parse(reader["ID"].ToString());
                emp.EmployeeId = int.Parse(reader["T_EMP_ID"].ToString());
                emp.First_Name = firstname;
                emp.Last_Name = lastname;
                emp.Birthday = Convert.ToDateTime(reader["BIRTHDAY"].ToString());
                emp.Id_card = reader["IDCARD_ID"].ToString();
                emp.Gender_str = reader["GENDER"].ToString();
                emp.Email = reader["EMAIL"].ToString();
                emp.Password = reader["PASSWORD"].ToString();

                var picData = reader["PICTURE"] as byte[];

                emp.PicData = picData != null ? Convert.ToBase64String(picData) : null;

                emp.Role_id =  int.Parse(reader["ROLE_ID"].ToString());
                emp.Category_id = reader["CAT_ID"] != DBNull.Value ? int.Parse(reader["CAT_ID"].ToString()) : null;

                emp.Role_name = emp.Role_id > 0 && emp.Category_id > 0 ? "Doctor" : reader["ROLE_NAME"].ToString();

                emp.Review_count = reader["avg_score"] != DBNull.Value ? Convert.ToDouble(reader["avg_score"].ToString()) : 5;
                emp.Category_name = Convert.ToString(reader["CATEGORY_NAME"]).Length > 1 ? reader["CATEGORY_NAME"].ToString() : null;
                emp.View_count = reader["VIEW_COUNT"] != DBNull.Value ? int.Parse(reader["VIEW_COUNT"].ToString()) : null;

                /*
                */
                oracleConnection.Close();

                return emp;
            }
            return null;
        }
        public List<EmployeeModel> filter_emp_by_fullname(string fullName)
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.filter_emp_by_fullname";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_fullname", OracleDbType.Varchar2).Value = fullName;

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string fullNameFromDb = reader["full_name"] != DBNull.Value ? Convert.ToString(reader["full_name"]) : string.Empty;
                string[] nameParts = fullNameFromDb.Split(' ');

                string firstname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                string lastname = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                EmployeeModel emp = new EmployeeModel();
                emp.Id = int.Parse(reader["id"].ToString());
                emp.First_Name = firstname;
                emp.Last_Name = lastname;


                emp.Birthday = Convert.ToDateTime(reader["birthday"]);
                emp.Gender_str = reader["gender"] != DBNull.Value ? Convert.ToString(reader["gender"]) : string.Empty;

                emp.Phone = reader["phone"] != DBNull.Value ? Convert.ToString(reader["phone"]) : string.Empty;
                emp.Role_id = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0;
                emp.Review_count = reader["avg_score"] != DBNull.Value ? Convert.ToDouble(reader["avg_score"].ToString()) : 5;
                emp.Category_name = reader["category_name"] != DBNull.Value ? Convert.ToString(reader["category_name"]) : string.Empty;
                emp.View_count = reader["view_count"] != DBNull.Value ? int.Parse(Convert.ToString(reader["view_count"])) : 0;

                var pictureData = reader["picture"] as byte[];

                if (pictureData != null)
                {
                    emp.PicData = Convert.ToBase64String(pictureData);
                }
                else
                {
                    emp.PicData = null;
                }

                employees.Add(emp);
            }
            oracleConnection.Close();
            return employees;
        }
        public List<EmployeeModel> filter_emp_by_category(string category)
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.filter_emp_by_category";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_category", OracleDbType.Varchar2).Value = category;

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string fullName = reader["full_name"] != DBNull.Value ? Convert.ToString(reader["full_name"]) : string.Empty;
                string[] nameParts = fullName.Split(' ');

                string firstname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                string lastname = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                EmployeeModel emp = new EmployeeModel();
                emp.Id = int.Parse(reader["id"].ToString());
                emp.First_Name = firstname;
                emp.Last_Name = lastname;


                emp.Birthday = Convert.ToDateTime(reader["birthday"]);
                emp.Gender_str = reader["gender"] != DBNull.Value ? Convert.ToString(reader["gender"]) : string.Empty;

                emp.Phone = reader["phone"] != DBNull.Value ? Convert.ToString(reader["phone"]) : string.Empty;
                emp.Role_id = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0;
                emp.Review_count = reader["avg_score"] != DBNull.Value ? Convert.ToDouble(reader["avg_score"].ToString()) : 5;
                emp.Category_name = reader["category_name"] != DBNull.Value ? Convert.ToString(reader["category_name"]) : string.Empty;
                emp.View_count = reader["view_count"] != DBNull.Value ? int.Parse(Convert.ToString(reader["view_count"])) : 0;

                var pictureData = reader["picture"] as byte[];

                if (pictureData != null)
                {
                    emp.PicData = Convert.ToBase64String(pictureData);
                }
                else
                {
                    emp.PicData = null;
                }

                employees.Add(emp);
            }
            oracleConnection.Close();
            return employees;
        }
        public void delete_emp(int id) 
        {
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.delete_emp_by_id";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
            cmd.ExecuteNonQuery();
            oracleConnection.Close();  
        }
        public void update_emp(EmployeeModel employee)
        {
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "sys.pkg_lo_clinic_emp.update_emp";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = employee.Id;
            cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = employee.First_Name;
            cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = employee.Last_Name;
            cmd.Parameters.Add("p_birthday", OracleDbType.Date).Value = employee.Birthday;
            cmd.Parameters.Add("p_gender", OracleDbType.Int16).Value = employee.Gender;
            cmd.Parameters.Add("p_idcard_id", OracleDbType.Varchar2).Value = employee.Id_card;
            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = employee.Email;
            cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = employee.Phone;
            cmd.Parameters.Add("p_role_id", OracleDbType.Int32).Value = employee.Role_id;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = employee.Password;
            // cmd.Parameters.Add("p_picture", OracleDbType.).Value = employee;
            cmd.Parameters.Add("p_category_id", OracleDbType.Int16).Value = employee.Category_id > 0 ? employee.Category_id : null;
            cmd.Parameters.Add("p_review_count", OracleDbType.Int32).Value = employee.Review_count;
            cmd.Parameters.Add("p_view_count", OracleDbType.Int32).Value = employee.View_count;
            // cmd.Parameters.Add("p_cv", OracleDbType.).Value = employee;

            cmd.ExecuteNonQuery();

            oracleConnection.Close();
        }
        public List<EmployeeModel> get_emp_lazy_load(int last_person_id)
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.get_emp_new";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_last_emp_id", OracleDbType.Int32).Value = last_person_id;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) 
            {
                string fullName = reader["full_name"] != DBNull.Value ? Convert.ToString(reader["full_name"]) : string.Empty;
                string[] nameParts = fullName.Split(' ');

                string firstname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                string lastname = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                EmployeeModel emp = new EmployeeModel();
                emp.Id = int.Parse(reader["id"].ToString());
                emp.First_Name = firstname;
                emp.Last_Name = lastname;


                emp.Birthday = Convert.ToDateTime(reader["birthday"]);
                emp.Gender_str = reader["gender"] != DBNull.Value ? Convert.ToString(reader["gender"]) : string.Empty;

                emp.Phone = reader["phone"] != DBNull.Value ? Convert.ToString(reader["phone"]) : string.Empty;
                emp.Role_id = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0;
                emp.Review_count = reader["avg_score"] != DBNull.Value ? Convert.ToDouble(reader["avg_score"].ToString()) : 5;
                emp.Category_name = reader["category_name"] != DBNull.Value ? Convert.ToString(reader["category_name"]) : string.Empty;
                emp.View_count = reader["view_count"] != DBNull.Value ? int.Parse(Convert.ToString(reader["view_count"])) : 0;

                var pictureData = reader["picture"] as byte[];

                if (pictureData != null)
                {
                    emp.PicData = Convert.ToBase64String(pictureData);
                }
                else
                {
                    emp.PicData = null;
                }

                employees.Add(emp);
            }

            oracleConnection.Close();

            return employees;
        }
        public void update_view_count(int user_id)
        {
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.update_view_count";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = user_id;
            
            cmd.ExecuteNonQuery();

            oracleConnection.Close();
        }

        public EmployeeModel get_emp_by_emp_id(int emp_id)
        {
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_emp.get_emp_by_emp_id";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = emp_id;

            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                EmployeeModel emp = new EmployeeModel();

                string fname = reader["full_name"] != DBNull.Value ? Convert.ToString(reader["full_name"]) : string.Empty;
                string[] nameParts = fname.Split(' ');

                string firstname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                string lastname = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                emp.Id = int.Parse(reader["ID"].ToString());
                //emp.EmployeeId = int.Parse(reader["T_EMP_ID"].ToString()); 
                emp.First_Name = firstname;
                emp.Last_Name = lastname;
                //emp.Birthday = Convert.ToDateTime(reader["BIRTHDAY"].ToString());
                //emp.Id_card = reader["IDCARD_ID"].ToString();
                //emp.Gender_str = reader["GENDER"].ToString();
                emp.Email = reader["EMAIL"].ToString();
                //emp.Password = reader["PASSWORD"].ToString();

                //var picData = reader["PICTURE"] as byte[];

                //emp.PicData = picData != null ? Convert.ToBase64String(picData) : null;

                //emp.Role_id = int.Parse(reader["ROLE_ID"].ToString());
                //emp.Category_id = reader["CAT_ID"] != DBNull.Value ? int.Parse(reader["CAT_ID"].ToString()) : null;

                //emp.Role_name = emp.Role_id > 0 && emp.Category_id > 0 ? "Doctor" : reader["ROLE_NAME"].ToString();

                emp.Review_count = reader["avg_score"] != DBNull.Value ? Convert.ToDouble(reader["avg_score"].ToString()) : 5;
                emp.Category_name = Convert.ToString(reader["CATEGORY_NAME"]).Length > 1 ? reader["CATEGORY_NAME"].ToString() : null;
                emp.View_count = reader["VIEW_COUNT"] != DBNull.Value ? int.Parse(reader["VIEW_COUNT"].ToString()) : null;

                /*
                */
                oracleConnection.Close();

                return emp;
            }
            return null;
        }

    }
}
