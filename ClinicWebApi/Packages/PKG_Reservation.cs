using ClinicWebApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ClinicWebApi.Packages
{
    public interface IPKG_Reservation
    {
        public void add_reservation(ReservationModel reservation);
        public List<ReservationModel> get_user_reservation_list(int id);

        public List<ReservationModel> get_doctor_reservation_list(int id);

        public List<ReservationModel> get_doctor_reservation_list_public(int id, int? userId);

        public void delete_reservation(int reservation_id, int user_person_id, int emp_person_id);
    }
    public class PKG_Reservation : PKG_Base, IPKG_Reservation
    {
        IConfiguration configuration;
        public PKG_Reservation(IConfiguration configuration) : base(configuration) { }

        public void add_reservation(ReservationModel reservation) 
        {
            OracleConnection oracleConnection = new OracleConnection();
            oracleConnection.ConnectionString = ConnectionStr;
            oracleConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = oracleConnection;
            cmd.CommandText = "olerning.pkg_lo_clinic_reservation.add_reservation";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_employee_id", OracleDbType.Int32).Value = reservation.Employee_Id;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = reservation.User_Id;
            cmd.Parameters.Add("p_start_date", OracleDbType.Date).Value = reservation.Start_Date;
            cmd.Parameters.Add("p_end_date", OracleDbType.Date).Value = reservation.Start_Date.AddHours(1);
            cmd.Parameters.Add("p_reservation_description", OracleDbType.Varchar2).Value = reservation.Reservation_Description;

            cmd.ExecuteNonQuery();
            oracleConnection.Close();
        }

        public List<ReservationModel> get_user_reservation_list(int id)
        { 
            OracleConnection connection = new OracleConnection();
            connection.ConnectionString = ConnectionStr;
            connection.Open();

            List<ReservationModel> reservation_list = new List<ReservationModel>();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "olerning.pkg_lo_clinic_reservation.get_user_reservations";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = id;

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            { 
                ReservationModel reservation = new ReservationModel();
                reservation.Id = int.Parse(reader["ID"].ToString());
                reservation.Employee_Id = int.Parse(reader["EMPLOYEE_ID"].ToString());
                reservation.User_Id = int.Parse(reader["USER_ID"].ToString());
                reservation.Start_Date = Convert.ToDateTime(reader["START_DATE"].ToString());

                if (reader["STATUS"] != DBNull.Value)
                {
                    int statusValue = int.Parse(reader["STATUS"].ToString());
                    reservation.Status = statusValue == 1 ? "myVisit" : statusValue == 2 ? "oldVisit" : "";

                }

                reservation.Reservation_Description = reader["RESERVATION_DESCRIPTION"] != DBNull.Value ? reader["RESERVATION_DESCRIPTION"].ToString() : "";

                reservation_list.Add(reservation);
            }

            connection.Close();
            return reservation_list;
        }

        public List<ReservationModel> get_doctor_reservation_list(int id) 
        {
            OracleConnection connection = new OracleConnection();
            connection.ConnectionString = ConnectionStr;
            connection.Open();

            List<ReservationModel> reservation_list = new List<ReservationModel>();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "olerning.pkg_lo_clinic_reservation.get_doctors_reservation_list";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = id;

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ReservationModel reservation = new ReservationModel();
                reservation.Id = int.Parse(reader["ID"].ToString());
                reservation.Employee_Id = int.Parse(reader["EMPLOYEE_ID"].ToString());
                reservation.User_Id = int.Parse(reader["USER_ID"].ToString());
                reservation.Start_Date = Convert.ToDateTime(reader["START_DATE"].ToString());

                if (reader["STATUS"] != DBNull.Value)
                {
                    int statusValue = int.Parse(reader["STATUS"].ToString());
                    reservation.Status = statusValue == 1 ? "myVisit" : statusValue == 2 ? "oldVisit" : "";

                }

                reservation.Reservation_Description = reader["RESERVATION_DESCRIPTION"] != DBNull.Value ? reader["RESERVATION_DESCRIPTION"].ToString() : "";

                reservation_list.Add(reservation);
            }

            connection.Close();
            return reservation_list;
        }

        public List<ReservationModel> get_doctor_reservation_list_public(int id, int? userId)
        {
            OracleConnection connection = new OracleConnection();
            connection.ConnectionString = ConnectionStr;
            connection.Open();

            List<ReservationModel> reservation_list = new List<ReservationModel>();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "olerning.pkg_lo_clinic_reservation.get_doctors_reservation_list";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_resault", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_doctor_id", OracleDbType.Int32).Value = id;

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ReservationModel reservation = new ReservationModel();
                reservation.Employee_Id = int.Parse(reader["EMPLOYEE_ID"].ToString());

                int uId = int.Parse(reader["USER_ID"].ToString());

                if(userId > 0 && uId == userId)
                {
                    reservation.User_Id = uId;
                }
                reservation.Start_Date = Convert.ToDateTime(reader["START_DATE"].ToString());

                if (reader["STATUS"] != DBNull.Value)
                {
                    int statusValue = int.Parse(reader["STATUS"].ToString());
                    reservation.Status = statusValue == 1 ? reservation.User_Id > 0 ? "myVisit" : "visit" : statusValue == 2 ? "oldVisit" : "";
                    reservation.Id = reservation.Status == "myVisit" ? int.Parse(reader["ID"].ToString()) : null;

                }

                //reservation.Reservation_Description = reader["RESERVATION_DESCRIPTION"] != DBNull.Value ? reader["RESERVATION_DESCRIPTION"].ToString() : "";

                reservation_list.Add(reservation);
            }

            connection.Close();
            return reservation_list;
        }

        public void delete_reservation(int reservation_id, int emp_person_id, int user_person_id)
        {
            OracleConnection connection = new OracleConnection();
            connection.ConnectionString = ConnectionStr;
            connection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "olerning.pkg_lo_clinic_reservation.delete_reservation";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_reservation_id", OracleDbType.Int32).Value = reservation_id;
            cmd.Parameters.Add("p_employee_person_id",  OracleDbType.Int32).Value = emp_person_id;
            cmd.Parameters.Add("p_user_person_id",  OracleDbType.Int32).Value = user_person_id;

            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void update_reservation(ReservationModel reservation)
        {
            OracleConnection connection = new OracleConnection();
            connection.ConnectionString = ConnectionStr;
            connection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "olerning.pkg_lo_clinic_reservation.update_reservation";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_reservation_id", OracleDbType.Int32).Value = reservation.Id;
            cmd.Parameters.Add("p_employee_id", OracleDbType.Int32).Value = reservation.Employee_Id;
            cmd.Parameters.Add("p_user_id", OracleDbType.Int32).Value = reservation.User_Id;
            cmd.Parameters.Add("p_start_date", OracleDbType.Date).Value = reservation.Start_Date;
            cmd.Parameters.Add("p_end_date", OracleDbType.Date).Value = reservation.Start_Date.AddHours(1);
            cmd.Parameters.Add("p_reservation_description", OracleDbType.Varchar2).Value = reservation.Reservation_Description;
            cmd.Parameters.Add("p_status", OracleDbType.Int32).Value = reservation.Status;

            cmd.ExecuteNonQuery();

            connection.Close();

        }
    }
}
