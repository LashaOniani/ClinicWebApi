using ClinicWebApi.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace ClinicWebApi.Packages
{
    public interface IPKG_User 
    {
        public void save_user(UserModel user, byte[] picture);
        public List<UserModel> get_all_user();
        public UserModel get_user_id(LoginModel login);
        public UserModel get_user(int id);
    }
    public class PKG_User : PKG_Base, IPKG_User
    {
        IConfiguration configuration;
        public PKG_User(IConfiguration configuration) : base(configuration) { }
        public void save_user(UserModel user, byte[] picture)
        {
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_user.save_user";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = user.First_Name;
            cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = user.Last_Name;
            cmd.Parameters.Add("p_birthday", OracleDbType.Date).Value = user.Birthday;
            cmd.Parameters.Add("p_gender", OracleDbType.Int32).Value = user.Gender;
            cmd.Parameters.Add("p_idcard_id", OracleDbType.Varchar2).Value = user.Id_card;
            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = user.Email;
            cmd.Parameters.Add("p_phone", OracleDbType.Varchar2).Value = user.Phone;
            cmd.Parameters.Add("p_role_id", OracleDbType.Int32).Value = user.Role_id;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = user.Password;
            cmd.Parameters.Add("p_picture", OracleDbType.Blob).Value = picture;

            cmd.ExecuteNonQuery();
            oracleConnection.Close();
        }
        public List<UserModel> get_all_user()
        {
            List<UserModel> userList = new List<UserModel>();

            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_user.get_all_user";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) 
            {
                //string fullName = reader["full_name"] != DBNull.Value ? Convert.ToString(reader["full_name"]) : string.Empty;
                //string[] nameParts = fullName.Split(' ');

                //string firstname = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                //string lastname = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                UserModel eachUser = new UserModel();
                eachUser.First_Name = reader["first_name"].ToString();
                eachUser.Last_Name = reader["last_name"].ToString();
                eachUser.Birthday = Convert.ToDateTime(reader["birthday"].ToString());
                eachUser.Gender_str = reader["gender"].ToString();
                eachUser.Id_card = reader["idcard_id"].ToString();
                eachUser.Phone =reader["phone"].ToString();
                eachUser.Email = reader["email"].ToString();
                eachUser.Password = reader["password"].ToString();
                userList.Add(eachUser);
            }

            oracleConnection.Close();
            return userList;
        }

        public UserModel get_user_id(LoginModel login)
        {
            UserModel user = null;

            OracleConnection connection = new OracleConnection();
            connection.ConnectionString = ConnectionStr;
            connection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "olerning.pkg_lo_clinic_user.get_user";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_user", OracleDbType.Varchar2).Value = login.Username;
            cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = login.Password;

            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                user = new UserModel();
                user.Id = int.Parse(reader["id"].ToString());
                user.Role_id = int.Parse(reader["role_id"].ToString());

                /* user.Fname = reader["fname"].ToString();
                user.Lname = reader["Lname"].ToString();
                user.Email = reader["Email"].ToString();
                user.Password = reader["Password"].ToString();
                user.Pnumber = reader["Pnumber"].ToString(); */
            }

            connection.Close();
            return user;
        }

        public UserModel get_user(int id)
        {
            UserModel user = new UserModel();

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = ConnectionStr;
            conn.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_lo_clinic_user.get_user_by_id";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction= ParameterDirection.Output;
            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

            OracleDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user.Id = int.Parse(reader["ID"].ToString());
                user.UserId = int.Parse(reader["T_USER_ID"].ToString());
                user.First_Name = reader["FIRST_NAME"].ToString();
                user.Last_Name = reader["LAST_NAME"].ToString();
                user.Birthday = Convert.ToDateTime(reader["BIRTHDAY"].ToString());
                user.Id_card = reader["IDCARD_ID"].ToString();
                user.Gender_str = reader["GENDER"].ToString();
                user.Email = reader["EMAIL"].ToString();
                user.Phone = reader["PHONE"].ToString();    
                user.Role_name = reader["ROLE_NAME"].ToString();

                byte[] pictureData = reader["PICTURE"] as byte[];

                if( pictureData != null )
                {
                    user.PicData = Convert.ToBase64String(pictureData);
                }
                else
                {
                    user.PicData = null;
                }

                return user;
                /* user.Role_name = reader["ROLE_NAME"].ToString();
                user.Password = reader["PASSWORD"].ToString();
                user.PicData = reader["PICTURE"].ToString(); */
            }
            else
            {
                user = null;
            }

            conn.Close();
            return user;
        }
    }
}
