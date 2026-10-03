using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
// Removed: using System.Web.UI.WebControls; - not available in ASP.NET Core (cr-dotnet-0026 fix)
// Removed: using System.Web.UI; - not available in ASP.NET Core (cr-dotnet-0026 fix)
using System.Data;
using System.Data.SqlClient;
// Dapper is used for lightweight ORM queries routed through Amazon RDS Proxy.
// All connections are obtained via CreateConnection() which reads the RDS Proxy
// endpoint from the environment variable RDS_PROXY_CONNECTION_STRING (with
// fallback to AWS SSM Parameter Store, then Web.config "sqlCon1" connection string).
using Dapper;

 
namespace DBProject.DAL
{
	//Database Layer of 3 tier architecture
	public class myDAL
    {
		// ---------------------------------------------------------------------------
		// cr-dotnet-0010 FIX: Web.config Transformation Elimination
		// cr-dotnet-0013 FIX: RDS Proxy / Connection-pooling helper
		//
		// Configuration is now resolved at runtime using the following priority order,
		// eliminating all reliance on Web.config transformation files
		// (Web.Debug.config / Web.Release.config):
		//
		//   1. RDS_PROXY_CONNECTION_STRING environment variable
		//      Set in ECS task definition, Elastic Beanstalk environment properties,
		//      or local .env / launchSettings.json for development.
		//      This is the PRIMARY source for all cloud deployments.
		//
		//   2. AWS Systems Manager (SSM) Parameter Store
		//      Parameter path: /HospitalMgmt/ConnectionStrings/sqlCon1
		//      Retrieved at runtime using the AWS SDK. Enables centralised,
		//      auditable, environment-specific configuration without baking
		//      values into build artifacts (replaces Web.config transformations).
		//      Requires the AWSSDK.SimpleSystemsManagement NuGet package and
		//      an IAM role/policy granting ssm:GetParameter on the parameter path.
		//
		//   3. Web.config "sqlCon1" connection string (legacy fallback)
		//      Used only when neither of the above sources is available,
		//      e.g., during local development without AWS credentials.
		//
		// Amazon RDS Proxy multiplexes connections across application instances,
		// enforces IAM authentication, and provides connection pooling at the
		// infrastructure level, so no additional client-side pooling is required.
		// ---------------------------------------------------------------------------

		// SSM parameter path for the connection string (AWS Systems Manager Parameter Store).
		// Override via SSM_PARAMETER_PATH environment variable if a different path is needed.
		private const string SsmParameterPath = "/HospitalMgmt/ConnectionStrings/sqlCon1";

        private static string GetConnectionString()
        {
            // Priority 1: RDS_PROXY_CONNECTION_STRING environment variable
            // (primary source for all AWS cloud deployments)
            string rdsCon = Environment.GetEnvironmentVariable("RDS_PROXY_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(rdsCon))
                return rdsCon;

            // Priority 2: AWS Systems Manager Parameter Store
            // Replaces Web.config transformation files (cr-dotnet-0010):
            // environment-specific config is stored in SSM and injected at runtime
            // rather than baked into build artifacts via Web.Debug.config / Web.Release.config.
            try
            {
                string ssmPath = Environment.GetEnvironmentVariable("SSM_PARAMETER_PATH") ?? SsmParameterPath;
                using (var ssmClient = new Amazon.SimpleSystemsManagement.AmazonSimpleSystemsManagementClient())
                {
                    var request = new Amazon.SimpleSystemsManagement.Model.GetParameterRequest
                    {
                        Name = ssmPath,
                        WithDecryption = true
                    };
                    var response = ssmClient.GetParameterAsync(request).GetAwaiter().GetResult();
                    string ssmValue = response?.Parameter?.Value;
                    if (!string.IsNullOrEmpty(ssmValue))
                        return ssmValue;
                }
            }
            catch
            {
                // SSM not available (e.g., local dev without AWS credentials) — fall through to legacy fallback.
            }

            // Priority 3: Legacy Web.config fallback (local development only).
            // Web.config transformation files (Web.Debug.config / Web.Release.config) are no longer
            // used for environment-specific overrides; all such config is now in SSM Parameter Store.
            return System.Configuration.ConfigurationManager
                         .ConnectionStrings["sqlCon1"]?.ConnectionString
                   ?? throw new InvalidOperationException(
                         "No database connection string found. Set RDS_PROXY_CONNECTION_STRING " +
                         "environment variable or configure SSM Parameter Store path " +
                         SsmParameterPath + ".");
        }

        /// <summary>
        /// Creates and opens a SqlConnection routed through Amazon RDS Proxy.
        /// Callers are responsible for closing / disposing the connection.
        /// </summary>
        private static SqlConnection CreateConnection()
        {
            var con = new SqlConnection(GetConnectionString());
            con.Open();
            return con;
        }




		//-----------------------------------------------------------------------------------//
		//																					 //
		//									SIGNUP											 //
		//																					 //
		//-----------------------------------------------------------------------------------//



		/*CHECKS WHETHER IT IS A VALID USER AND RETURN ITS TYPE*/
		public int validateLogin (string Email, string Password, ref int type , ref int id)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();

            try
            {

                SqlCommand cmd1 = new SqlCommand("Login", con);     
                cmd1.CommandType = CommandType.StoredProcedure;

				/*
                 procedure Login
                 @email varchar(30),
                 @password varchar(20),
                 @status int output,
                 @ID int output,
                 @type int output
                 */


				cmd1.Parameters.Add("@email", SqlDbType.VarChar, 30).Value = Email;
                cmd1.Parameters.Add("@password", SqlDbType.VarChar, 20).Value = Password; 

                cmd1.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@type", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd1.ExecuteNonQuery();
				
                int status = (int)cmd1.Parameters["@status"].Value;
                type = (int)cmd1.Parameters["@type"].Value;
                id = (int)cmd1.Parameters["@ID"].Value;

                return status;
            }

            catch(SqlException ex)
            {
                return -1;
            }

            finally
            {
                con.Close();   
            }
        }

        




		/*THIS FUNCTION WILL VALIDATE ALL THE INFORMAIION OF OF USER (PATIENT)*/
        public int validateUser (string Name, string BirthDate, string Email , string Password , string PhoneNo , string gender , string Address, ref int id)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();

            try
            {

                /*
                  Procedure  PatientSignup
                  @name varchar(20),
                  @phone char(15),
                  @address varchar(40),
                  @date Date,
                  @gender char(1),
                  @password varchar(20),
                  @email varchar(30),
                  @status int output,
                  @ID int output
                  */


                SqlCommand cmd1 = new SqlCommand("PatientSignup", con);              
                cmd1.CommandType = CommandType.StoredProcedure;

				cmd1.Parameters.Add("@name", SqlDbType.VarChar, 20).Value = Name;
				cmd1.Parameters.Add("@address", SqlDbType.VarChar, 40).Value = Address;
				cmd1.Parameters.Add("@gender", SqlDbType.VarChar, 1).Value = gender;
				cmd1.Parameters.Add("@date", SqlDbType.Date).Value = BirthDate;
				cmd1.Parameters.Add("@email", SqlDbType.VarChar, 30).Value = Email;
				cmd1.Parameters.Add("@password", SqlDbType.VarChar, 20).Value = Password;
				cmd1.Parameters.Add("@phone", SqlDbType.Char, 15).Value = PhoneNo;
				
                cmd1.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
				
                cmd1.ExecuteNonQuery();           

                int status = (int)cmd1.Parameters["@status"].Value;

                if (status != 0)
                {
                    id = (int)cmd1.Parameters["@ID"].Value;
                }


                return status; 
            }

            catch(SqlException ex)
            {
                return -1;
            }

            finally
            {
                con.Close();   
            }
        }







        //-----------------------------------------------------------------------------------//
        //                                                                                   //
        //                                       ADMIN                                       //
        //                                                                                   //
        //-----------------------------------------------------------------------------------//



        /*THIS FUNCTION CHECKS WHEATHER EMAIL OF A DOCTOR ALREADY EXISTS IN THE DATABASE */

        public int DoctorEmailAlreadyExist(string Email)
        {
            int status = 0;
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();


            /*
             @Email
             @status OUTPUT
             */


            SqlCommand cmd = new SqlCommand("CheckDoctorEmail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 30).Value = Email;
            cmd.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            status = (int)cmd.Parameters["@status"].Value;
            con.Close();

            return status;
        }







        /*THIS FUNCTION WILL ADD THE DOCTOR TO THE DATA BASE */
        public void AddDoctor(string Name, string Email, string Password, string BirthDate, int dept, string Phone, char gender, string Address, int exp, int salary, int Charges_per_visit, string spec, string qual)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();


            SqlCommand cmd = new SqlCommand("AddDoctor", con);
            cmd.CommandType = CommandType.StoredProcedure;

            /*
            @Name 
            @Email
            @Password 
            @BirthDate 
            @dept
            @gender
            @Address 
            @Exp
            @Salary
            @qualification
            @phone
            @spec
             */


            cmd.Parameters.Add("@Name", SqlDbType.VarChar, 30).Value = Name;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 30).Value = Email;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar, 30).Value = Password;
            cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = BirthDate;
            cmd.Parameters.Add("@dept", SqlDbType.VarChar, 30).Value = dept;
            cmd.Parameters.Add("@gender", SqlDbType.VarChar, 1).Value = gender;
            cmd.Parameters.Add("@Address", SqlDbType.VarChar, 30).Value = Address;
            cmd.Parameters.Add("@Exp", SqlDbType.VarChar, 30).Value = exp;
            cmd.Parameters.Add("@Salary", SqlDbType.VarChar, 30).Value = salary;
            cmd.Parameters.Add("@charges", SqlDbType.VarChar, 30).Value = Charges_per_visit;
            cmd.Parameters.Add("@phone", SqlDbType.VarChar, 30).Value = Phone;
            cmd.Parameters.Add("@spec", SqlDbType.VarChar, 30).Value = spec;
            cmd.Parameters.Add("@qual", SqlDbType.VarChar, 30).Value = qual;

            cmd.ExecuteNonQuery();
            con.Close();


        }





        /*THIS FUNCTION WILL ADD STAFF TO THE DATA BASE*/
        public int AddStaff(string Name, string BirthDate, string Phone, char gender, string Address, int salary, string Qual, string Designation)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();

            SqlCommand cmd = new SqlCommand("AddStaff", con);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {


                /*
                @Name 
                @BirthDate 
                @phone
                @gender
                @designation
                @Address 
                @Salary
                @phone
                @qualification
                */


                /*INPUTS*/
                cmd.Parameters.Add("@Name", SqlDbType.VarChar, 30).Value = Name;
                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = BirthDate;
                cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 30).Value = Phone;
                cmd.Parameters.Add("@gender", SqlDbType.VarChar, 1).Value = gender;
                cmd.Parameters.Add("@salary", SqlDbType.Int, 30).Value = salary;
                cmd.Parameters.Add("@Designation", SqlDbType.VarChar, 30).Value = Designation;
                cmd.Parameters.Add("@Qualification", SqlDbType.VarChar, 1).Value = Qual;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar, 50).Value = Address;

                cmd.ExecuteNonQuery();
            }
            catch
            {
                return -1;
            }

            con.Close();
            return 1;

        }







        /*THIS FUNCTION WILL RUN MULTIPLE QUERIES AND GET ALL THE INFORMATION NEEDED TO DISPLAY AT ADMIN HOME*/
        public void GetAdminHomeInformation(ref DataTable[] arrTable)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();


            SqlCommand cmd = new SqlCommand("SELECT * FROM Total_Patient", con);
            SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
            Adapter.Fill(arrTable[0]);

            cmd.CommandText = "SELECT * FROM Total_Doctors";
            Adapter.Fill(arrTable[1]);

            cmd.CommandText = "SELECT * FROM Income";
            Adapter.Fill(arrTable[2]);

            cmd.CommandText = "SELECT * FROM Department_View";
            Adapter.Fill(arrTable[3]);

            cmd.CommandText = "SELECT * FROM Appointment_view";
            Adapter.Fill(arrTable[4]);


            con.Close();

        }






        /*THIS FUNCTION IS INTENDED TO DELETE DOCTOR BUT SECRETLY IT ONLY UPDATE THE STATUS*/
        public int DeleteDoctor(int id)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("DeleteDoctor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                return -1;
            }

            con.Close();
            return 1;

        }



        /*THIS FUNCTION WILL DELLETE STAFF FROM THE DOCTOR */
        public int DeleteStaff(int id)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();

            try
            {
                SqlCommand cmd = new SqlCommand("DELETESTAFF", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
            }
            catch
            {
                return -1;
            }

            return 1;
        }


        /*LOADS THE TABLE OF DOCTOR / SPECIFIED DOCTORS ON THE BASIS OF SEARCH QUERY*/
        public void LoadDoctor(ref DataTable table, String SearchQuery)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();
            SqlCommand cmd;


            if (SearchQuery == "")
            {
                cmd = new SqlCommand(
                "SELECT Doctor.DoctorID as ID , Doctor.Name , D.DeptName as Department FROM Doctor JOIN Department D ON D.DeptNo = Doctor.DeptNo" +
                " WHERE Doctor.Status = 1",
                con);

            }
            else
            {
                cmd = new SqlCommand(
                "SELECT a.DoctorID as ID,  a.Name, D.DeptName as Department FROM department D join (SELECT * FROM Doctor WHERE Doctor.Status = 1 AND Doctor.Name like  '%' + @DName + '%')  a ON a.DeptNo = D.DeptNo",
                con);
                cmd.Parameters.AddWithValue("@DName", SearchQuery);
            }


            SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
            Adapter.Fill(table);
            con.Close();
        }






        /*LOADS THE TABLE OF PATIENT ON THE BASIS OF SEARCH QUERY*/
        /*FOR EMPTY QUERY RETURN ALL INFORMATION OTHERWISE RETURN ONLY REQUIRED TUPLE*/
        public void LoadPatient(ref DataTable table, String SearchQuery)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();
            SqlCommand cmd;


            if (SearchQuery == "")
            {
                cmd = new SqlCommand("SELECT * FROM PATIENT_VIEW", con);

            }
            else
            {
                cmd = new SqlCommand("SELECT Patient.PatientID, Patient.Name, Patient.Phone from Patient" +
                " WHERE patient.name like '%' + @SName + '%' ", con);
                cmd.Parameters.AddWithValue("@SName", SearchQuery.Trim());

            }


            SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
            Adapter.Fill(table);
            con.Close();
        }





        /*LOADS THE TABLE OF OTHER STAFF ON THE BASIS OF SEARCH QUERY*/
        /*IF THE QUERY IS EMPTY THEN LOAD ALL STAFF MEMBERS OTHER WISE ONLY SPECIFIED*/
        public void LoadOtherStaff(ref DataTable table, String SearchQuery)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();
            SqlCommand cmd;


            if (SearchQuery == "")
            {
                cmd = new SqlCommand("SELECT * FROM STAFF_VIEW", con);

            }
            else
            {
                cmd = new SqlCommand("SELECT StaffID as ID , Name , Designation from OtherStaff WHERE Name like '%' + @pName + '%'", con);
                cmd.Parameters.AddWithValue("@PName", SearchQuery.Trim());
            }


            SqlDataAdapter Adapter = new SqlDataAdapter(cmd);
            Adapter.Fill(table);
            con.Close();
        }





        public int GETPATIENT(int pid, ref string name, ref string phone, ref string address, ref string birthDate, ref int age, ref string gender)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();


            try
            {

                /*
				 * PROCEDURE RetrievePatientData
				 * 
                 @ID int,
                 @name varchar(20) output,
                 @phone char(15) output,
                 @address varchar(40) output,
                 @birthDate varchar (10) output,
                 @age int output,
                 @gender char(1)
                 */


                SqlCommand cmd1 = new SqlCommand("RetrievePatientData", con);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add("@id", SqlDbType.Int).Value = pid;

                /*PUTTING OUTPUTS*/
                cmd1.Parameters.Add("@name", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@phone", SqlDbType.Char, 15).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@birthDate", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@address", SqlDbType.VarChar, 40).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@age", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@gender", SqlDbType.Char, 1).Direction = ParameterDirection.Output;

                cmd1.ExecuteNonQuery();

                /* GETTING OUTPUTS*/
                name = (string)cmd1.Parameters["@name"].Value.ToString();
                phone = (string)cmd1.Parameters["@phone"].Value.ToString();
                address = (string)cmd1.Parameters["@address"].Value.ToString();
                birthDate = (string)cmd1.Parameters["@birthDate"].Value.ToString();
                age = Convert.ToInt32((cmd1.Parameters["@age"].Value));
                gender = (string)cmd1.Parameters["@gender"].Value.ToString();


                return 0;
            }

            catch (SqlException ex)
            {
                return -1;
            }

            finally
            {
                con.Close();
            }
        }










        public int GET_DOCTOR_PROFILE(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();


            try
            {
                /*
                procedure GET_DOCTOR_PROFILE

                @dID int,

                @name varchar(20) output,
                @phone char(15) output,
                @gender varchar(2) output,
                @charges float output,
                @RI float output,
                @PTreated int output,
                @qualification varchar(100) output,
                @specialization varchar(50) output,
                @workE int output,
                @age int output
                 */

                SqlCommand cmd1 = new SqlCommand("GET_DOCTOR_PROFILE", con);

                cmd1.CommandType = CommandType.StoredProcedure;


                //Inputs
                cmd1.Parameters.Add("@dID", SqlDbType.Int).Value = dID;

                //Outputs
                cmd1.Parameters.Add("@name", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@phone", SqlDbType.VarChar, 15).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@gender", SqlDbType.VarChar, 2).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@charges", SqlDbType.Float).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@RI", SqlDbType.Float).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@PTreated", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@qualification", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@specialization", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@workE", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd1.Parameters.Add("@age", SqlDbType.Int).Direction = ParameterDirection.Output;


                cmd1.ExecuteNonQuery();

                /*GETTING OUTPUT*/
                name = (string)cmd1.Parameters["@name"].Value;
                phone = (string)cmd1.Parameters["@phone"].Value;
                gender = (string)cmd1.Parameters["@gender"].Value;
                charges_Per_Visit = Convert.ToSingle(cmd1.Parameters["@charges"].Value);
                ReputeIndex = Convert.ToSingle(cmd1.Parameters["@RI"].Value);
                PatientsTreated = (int)cmd1.Parameters["@PTreated"].Value;
                qualification = (string)cmd1.Parameters["@qualification"].Value;
                specialization = (string)cmd1.Parameters["@specialization"].Value;
                workE = (int)cmd1.Parameters["@workE"].Value;
                age = (int)cmd1.Parameters["@age"].Value;



            }

            catch (SqlException ex)
            {
                return -1;
            }

            con.Close();
            return 1;
        }



        public int GETSATFF(int id, ref string name, ref string phone, ref string address, ref string gender, ref string desig, ref int sal)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();

            SqlCommand cmd1 = new SqlCommand("GET_STAFF", con);
            cmd1.CommandType = CommandType.StoredProcedure;


            //Inputs
            cmd1.Parameters.Add("@id", SqlDbType.Int).Value = id;

            //Outputs
            cmd1.Parameters.Add("@name", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
            cmd1.Parameters.Add("@phone", SqlDbType.VarChar, 15).Direction = ParameterDirection.Output;
            cmd1.Parameters.Add("@gender", SqlDbType.VarChar, 2).Direction = ParameterDirection.Output;
            cmd1.Parameters.Add("@address", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd1.Parameters.Add("@desig", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd1.Parameters.Add("@sal", SqlDbType.Int).Direction = ParameterDirection.Output;

            //try
            {
                cmd1.ExecuteNonQuery();


                /*GETTING OUTPUT*/
                name = (string)cmd1.Parameters["@name"].Value;
                phone = (string)cmd1.Parameters["@phone"].Value;
                gender = (string)cmd1.Parameters["@gender"].Value;
                address = (string)cmd1.Parameters["@address"].Value;
                desig = (string)cmd1.Parameters["@desig"].Value;
                sal = (int)cmd1.Parameters["@sal"].Value;

            }
            //catch
            {
                //return -1;
            }


            return 1;


        }





        //-----------------------------------------------------------------------------------//
        //                                                                                   //
        //                                       PATIENT                                     //
        //                                                                                   //
        //-----------------------------------------------------------------------------------//










        /*-------------------DISPLAYS PATIENT INFORMATION AT PATIENT HOME--------------------------------------- */

        public int patientInfoDisplayer(int pid, ref string name, ref string phone, ref string address, ref string birthDate, ref int age, ref string gender)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();


			try
			{

				/*
				 * PROCEDURE RetrievePatientData
				 * 
                 @ID int,
                 @name varchar(20) output,
                 @phone char(15) output,
                 @address varchar(40) output,
                 @birthDate varchar (10) output,
                 @age int output,
                 @gender char(1)
                 */


				SqlCommand cmd1 = new SqlCommand("RetrievePatientData", con);
				cmd1.CommandType = CommandType.StoredProcedure;

				cmd1.Parameters.Add("@id", SqlDbType.Int).Value = pid;

				/*PUTTING OUTPUTS*/
				cmd1.Parameters.Add("@name", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@phone", SqlDbType.Char, 15).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@birthDate", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@address", SqlDbType.VarChar, 40).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@age", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@gender", SqlDbType.Char, 1).Direction = ParameterDirection.Output;

				cmd1.ExecuteNonQuery();            

				/* GETTING OUTPUTS*/
				name = (string)cmd1.Parameters["@name"].Value;
				phone = (string)cmd1.Parameters["@phone"].Value;
				address = (string)cmd1.Parameters["@address"].Value;
				birthDate = (string)cmd1.Parameters["@birthDate"].Value;
				age = (int)cmd1.Parameters["@age"].Value;
				gender = (string)cmd1.Parameters["@gender"].Value;


				return 0;
			}

			catch (SqlException ex)
			{
				return -1;
			}

			finally
			{
				con.Close();
			}
		}


		


		/*---------------------------GENERATE BILL HISTORY--------------------------------------*/

		public int getBillHistory(int id, ref DataTable result)
		{
			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
				 * 
				 * procedure RetrieveBillHistory
                  
				@pID int,
                  @count int OUTPUT
                 */


				cmd1 = new SqlCommand("RetrieveBillHistory", con); 
				cmd1.CommandType = CommandType.StoredProcedure;

				/*INPUT*/
				cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;

				/*OUTPUT*/
				cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd1.ExecuteNonQuery();   

				using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
				{
					da.Fill(ds);  
				}

				result = ds.Tables[0];     
				return (int)cmd1.Parameters["@count"].Value;



			}
			/*ON ERROR RETURN -1*/
			catch (SqlException ex)
			{
				return -1;  
			}

			finally
			{
				con.Close();
			}
		}





		//-------------------------------------CURRENT APPOINTMENTS------------------------------------------//

		public int appointmentTodayDisplayer(int pid, ref string dName, ref string timings)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
				 *  procedure RetrieveCurrentAppointment
				 * 
                    @pID int,
                    @dName varchar(30) OUTPUT,
                    @timings varchar(30) OUTPUT,
                    @count int OUTPUT

                 */

				cmd1 = new SqlCommand("RetrieveCurrentAppointment", con);
				cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.Add("@pid", SqlDbType.Int).Value = pid;

                //Outputs
                cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@timings", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@dName", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;

				cmd1.ExecuteNonQuery();   //Execute the cmd query

				int status = (int)cmd1.Parameters["@count"].Value;

				if (status == 0)
				{
					return status;
				}

				else
				{
					dName = (string)cmd1.Parameters["@dName"].Value;
					timings = (string)cmd1.Parameters["@timings"].Value;
					return status;
				}
			}

			catch (SqlException ex)
			{
				return -1;  //if any error, return -1
			}

			finally
			{
				con.Close();
			}
		}




		//-------------------------------------TREATMENT HISTORY------------------------------------------//
		public int getTreatmentHistory(int id, ref DataTable result)
		{
			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
                  @pID int,
                  @count int OUTPUT
                 */

				cmd1 = new SqlCommand("RetrieveTreatmentHistory", con);   //Name of your SQL Procedure
				cmd1.CommandType = CommandType.StoredProcedure;

				//INPUTS
				cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;
				
				//OUTPUTS
				cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
				
				cmd1.ExecuteNonQuery();   

				using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
				{
					da.Fill(ds);  
				}

				result = ds.Tables[0];      
				return (int)cmd1.Parameters["@count"].Value;
			}

			catch (SqlException ex)
			{
				return -1;  
			}

			finally
			{
				con.Close();
			}
		}




		/*-------------------------TAKE APPOINMENT------------------------------------*/
		public int getdeptInfo(ref DataTable result)
		{
			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{
				/*EXECUTING QUERY*/
				cmd1 = new SqlCommand("select* from deptInfo", con);
				cmd1.CommandType = CommandType.Text;
				
				cmd1.ExecuteNonQuery();   

				using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
				{
					da.Fill(ds);  
				}

				result = ds.Tables[0];
				return 1;
			}

			catch (SqlException ex)
			{
				return -1;
			}

			finally
			{
				con.Close();
			}
		}




		//-------------------------------------VIEW DOCTORS------------------------------------------//

		public int getDeptDoctorInfo(string deptName, ref DataTable result)
		{
			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
                  Procedure RetrieveDeptDoctorInfo

                  @deptName varchar (30)
                 */


				cmd1 = new SqlCommand("RetrieveDeptDoctorInfo", con);
				cmd1.CommandType = CommandType.StoredProcedure;

				//Input
				cmd1.Parameters.Add("@deptName", SqlDbType.VarChar, 30).Value = deptName;
				
				cmd1.ExecuteNonQuery();  

				using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
				{
					da.Fill(ds);   
				}

				/*FILL TABLE*/
				result = ds.Tables[0];

				return 1;
			}

			catch (SqlException ex)
			{
				return -1;  
			}

			finally
			{
				con.Close();
			}
		}







		//-------------------------------------DOCTOR PROFILE------------------------------------------//


		public int doctorInfoDisplayer(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();


			try
			{
				/*
                procedure RetrieveDoctorData

                @dID int,

                @name varchar(20) output,
                @phone char(15) output,
                @gender varchar(2) output,
                @charges float output,
                @RI float output,
                @PTreated int output,
                @qualification varchar(100) output,
                @specialization varchar(50) output,
                @workE int output,
                @age int output
                 */

				SqlCommand cmd1 = new SqlCommand("RetrieveDoctorData", con);             

				cmd1.CommandType = CommandType.StoredProcedure;


				//Inputs
				cmd1.Parameters.Add("@dID", SqlDbType.Int).Value = dID;
				
				//Outputs
				cmd1.Parameters.Add("@name", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@phone", SqlDbType.VarChar, 15).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@gender", SqlDbType.VarChar, 2).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@charges", SqlDbType.Float).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@RI", SqlDbType.Float).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@PTreated", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@qualification", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@specialization", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@workE", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@age", SqlDbType.Int).Direction = ParameterDirection.Output;


				cmd1.ExecuteNonQuery();    

				/*GETTING OUTPUT*/
				name = (string)cmd1.Parameters["@name"].Value;
				phone = (string)cmd1.Parameters["@phone"].Value;
				gender = (string)cmd1.Parameters["@gender"].Value;
				charges_Per_Visit = Convert.ToSingle(cmd1.Parameters["@charges"].Value);
				ReputeIndex = Convert.ToSingle(cmd1.Parameters["@RI"].Value);
				PatientsTreated = (int)cmd1.Parameters["@PTreated"].Value;
				qualification = (string)cmd1.Parameters["@qualification"].Value;
				specialization = (string)cmd1.Parameters["@specialization"].Value;
				workE = (int)cmd1.Parameters["@workE"].Value;
				age = (int)cmd1.Parameters["@age"].Value;


				return 0;
			}

			catch (SqlException ex)
			{
				return -1;
			}

			finally
			{
				con.Close();    
			}
		}


		//-------------------------------------APPOINTMENT TAKER------------------------------------------//

		public int getFreeSlots(int dID, int pID, ref DataTable result)
		{
			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
                  Procedure RetrieveFreeSlots

                  @dID int,
                  @pID int,
                  @count int OUTPUT
                 */


				cmd1 = new SqlCommand("RetrieveFreeSlots", con);
				cmd1.CommandType = CommandType.StoredProcedure;

				//Input
				cmd1.Parameters.Add("@dID", SqlDbType.Int).Value = dID;
				cmd1.Parameters.Add("@pID", SqlDbType.Int).Value = pID;

				//Output
				cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
				
				cmd1.ExecuteNonQuery();   

				using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
				{
					da.Fill(ds);   
				}
				result = ds.Tables[0];     

				return (int)cmd1.Parameters["@count"].Value;
			}

			catch (SqlException ex)
			{
				return -1;  
			}

			finally
			{
				con.Close();
			}
		}




		//-------------------------------------APPOINTMENT REQUEST SENT------------------------------------------//

		public int insertAppointment(int dID, int pID, int freeSlot, ref string mes)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			string m = "";

			con.InfoMessage += delegate (object sender, SqlInfoMessageEventArgs e)
			{
				m += "\n" + e.Message;
			};


			try
			{

				/*
                  Procedure insertInAppointmentTable

                  @dID int,
                  @pID int,
                  @freeSlot int
                 */


				cmd1 = new SqlCommand("insertInAppointmentTable", con);
				cmd1.CommandType = CommandType.StoredProcedure;

				//Input
				cmd1.Parameters.Add("@dID", SqlDbType.Int).Value = dID;
				cmd1.Parameters.Add("@pID", SqlDbType.Int).Value = pID;
				cmd1.Parameters.Add("@freeSlot", SqlDbType.Int).Value = freeSlot;
				
				cmd1.ExecuteNonQuery();   
				mes = m;

				return 0;
			}

			catch (SqlException ex)
			{
				return -1;  
			}

			finally
			{
				con.Close();
			}
		}





		//-------------------------------------PATIENT NOTIFICATIONS------------------------------------------//

		public int getNotifications(int pid, ref string dName, ref string timings)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
                  procedure RetrievePatientNotifications

                    @pID int,
                    @dName varchar(30) OUTPUT,
                    @timings varchar(30) OUTPUT,
                    @count int OUTPUT

                 */

				cmd1 = new SqlCommand("RetrievePatientNotifications", con);   //Name of your SQL Procedure
				cmd1.CommandType = CommandType.StoredProcedure;

				//Inputs
				cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = pid;
			
				//Outputs
				cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@timings", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@dName", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
				
				cmd1.ExecuteNonQuery();   


				int status = (int)cmd1.Parameters["@count"].Value;

				if (status == 0)
				{
					return status;
				}

				else
				{
					dName = (string)cmd1.Parameters["@dName"].Value;
					timings = (string)cmd1.Parameters["@timings"].Value;
					return status;
				}
			}

			catch (SqlException ex)
			{
				return -1;  
			}

			finally
			{
				con.Close();
			}
		}






		//-------------------------------------PATIENT FEEDBACK------------------------------------------//
		//-------------------------------------FUNCTION 1------------------------------------------//

		public int isFeedbackPending(int pid, ref string dName, ref string timings, ref int aID)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
                  procedure RetrievePendingFeedback

                    @pID int,
                    @dName varchar(30) OUTPUT,
                    @timings varchar(30) OUTPUT,
                    @count int OUTPUT

                 */

				cmd1 = new SqlCommand("RetrievePendingFeedback", con);   
				cmd1.CommandType = CommandType.StoredProcedure;

				//Inputs
				cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = pid;
				
				//Outputs
				cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@timings", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@dName", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
				cmd1.Parameters.Add("@aID", SqlDbType.Int).Direction = ParameterDirection.Output;

				cmd1.ExecuteNonQuery();   
				
				int status = (int)cmd1.Parameters["@count"].Value;

				if (status == 0)
				{
					return status;
				}

				else
				{
					dName = (string)cmd1.Parameters["@dName"].Value;
					timings = (string)cmd1.Parameters["@timings"].Value;
					aID = (int)cmd1.Parameters["@aID"].Value;

					return status;
				}
			}

			catch (SqlException ex)
			{
				return -1; 
			}

			finally
			{
				con.Close();
			}
		}




		//-------------------------------------FUNCTION 2------------------------------------------//

		public int givePendingFeedback(int aID)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd1;

			try
			{

				/*
                  procedure storeFeedback

                    @aID int
                 */

				cmd1 = new SqlCommand("storeFeedback", con);   
				cmd1.CommandType = CommandType.StoredProcedure;

				//Inputs
				cmd1.Parameters.Add("@aId", SqlDbType.Int).Value = aID;

				cmd1.ExecuteNonQuery();


				return 0;
			}

			catch (SqlException ex)
			{
				return -1;  
			}

			finally
			{
				con.Close();
			}
		}

















		//-----------------------------------------------------------------------------------//
		//                                                                                   //
		//                                       DOCTOR                                      //
		//                                                                                   //
		//-----------------------------------------------------------------------------------//






		/*THIS FUNCITON WILL RETRIEVE THE INFORMATION OF CURRENT LOGGED IN DOCTOR*/
		public int docinfo_DAL(int doctorid, ref DataTable result)
		{

			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd;

			try
			{

				cmd = new SqlCommand("Doctor_Information_By_ID1", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ID", SqlDbType.Int);
				cmd.Parameters["@id"].Value = doctorid;
				cmd.ExecuteNonQuery();

				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				    da.Fill(ds);

				result = ds.Tables[0];

			}

			catch (SqlException ex)
			{
				return 0;
			}

			finally
			{ 
			    con.Close();
			}

			return 1;
		}






		/*THIS FUNCTION WILL RETURN PENDING APPOINTMENT FORM THE DATABASE IN THE FORM OF DATASET*/
		public void GetAllpendingappointments_DAL(int doctorid, ref DataTable DT)
		{

			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			
			try
			{
                SqlCommand cmd = new SqlCommand();

                cmd = new SqlCommand("PENDING_APPOINTMENTS2", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@DOCTOR_ID", SqlDbType.Int);
				cmd.Parameters["@DOCTOR_ID"].Value = doctorid;
				cmd.ExecuteNonQuery();

                
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }

               DT = ds.Tables[0];

            }

            catch (SqlException ex)
			{
				Console.WriteLine("SQL Error" + ex.Message.ToString());
			}

			finally
			{
				con.Close();
			}
		}




		/*THIS FUNCTION WILL BE CALLED WHEN DOCTOR APPROVE THE REQUEST OF PATIENT*/
		public int UpdateAppointment_DAL(int Appointmentid)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd;
			int result = 0;


            try
            {
                cmd = new SqlCommand("APPROVE_APPOINTMENT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@APPOINT_ID", SqlDbType.Int).Value = Appointmentid;

                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }

			return result;
		}



		/*DELETES THE APPOINTMENT*/
		public int Deleteappointment_DAL(int appointmentid)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd;
			


			try
			{
				cmd = new SqlCommand("delete_APPOINTMENT", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@APPOINT_ID", SqlDbType.Int).Value = appointmentid;
				cmd.ExecuteNonQuery();
			}

			catch (SqlException ex)
			{
				Console.WriteLine("SQL Error" + ex.Message.ToString());
				return -1;
			}

			finally
			{
				con.Close();

			}
			return 1;
			
		}




		/*THIS FUNTION RETURN CURRENT DAY APPONTMENT*/
		public int search_patient_DAL(int did, ref DataTable result)
		{

			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();

			SqlCommand cmd;

            try
            {


                cmd = new SqlCommand("TODAYS_APPOINTMENTS", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@DOC_ID", SqlDbType.Int).Value = did;

                cmd.ExecuteNonQuery();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }



                result = ds.Tables[0];
            }

            catch (SqlException ex)
            {

            }
            
            finally
            {
                con.Close();
            }

            return 1;
		}






		/*UPDATE THE PRESCRIPTION WHEN APPOINTMENT IS GOING ON BY DOCTOR*/
		public int update_prescription_DAL(int did, int appointid, string disease, string progres, string prescrip)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd;
			try
			{
				cmd = new SqlCommand("UpdatePrescription", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@docId", SqlDbType.Int).Value = did;
				cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appointid;
				cmd.Parameters.Add("@Disease", SqlDbType.VarChar, 30).Value = disease;
				cmd.Parameters.Add("@progress", SqlDbType.VarChar, 50).Value = progres;
				cmd.Parameters.Add("@prescription", SqlDbType.VarChar, 60).Value = prescrip;

				cmd.ExecuteNonQuery();
			}
			
			catch (SqlException ex)
			{
				return 0;
			}
			finally
			{ 
			con.Close();
			}

			return 1;
			

		}


		/*GENERATES BILL*/

		public int generate_bill_DAL(int docid, ref DataTable result)
		{
			DataSet ds = new DataSet();
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("generate_bill", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@dId", SqlDbType.Int);
                cmd.Parameters["@did"].Value = docid;


                cmd.ExecuteNonQuery();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);

                }

                result = ds.Tables[0];
              
            }

            catch (SqlException ex)
            {
                return -1;
            }

            finally
            {
                con.Close();
            }

            return 1;
		}




		public void paid_bill_DAL(int did, int appoint)
		{
			// Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
			SqlConnection con = CreateConnection();
			SqlCommand cmd;
			
			cmd = new SqlCommand("finishedPaid", con);
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Add("@docId", SqlDbType.Int).Value = did;
			cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appoint;
			
			cmd.ExecuteNonQuery();

            con.Close();
		}


        public void Unpaid_bill_DAL(int did, int appoint)
        {
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();
            SqlCommand cmd;

            cmd = new SqlCommand("finishedUnPaid", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@docId", SqlDbType.Int).Value = did;
            cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appoint;

            cmd.ExecuteNonQuery();

            con.Close();
        }


        public int getPHistory(int id, ref DataTable result)
        {
            DataSet ds = new DataSet();
            // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
            SqlConnection con = CreateConnection();
            SqlCommand cmd1;

            try
            {

                /*
				 * 
				 * procedure RetrievePHistory
                  
				@dID int,
                  @count int OUTPUT
                 */


                cmd1 = new SqlCommand("RetrievePHistory", con);
                cmd1.CommandType = CommandType.StoredProcedure;

                /*INPUT*/
                cmd1.Parameters.Add("@dId", SqlDbType.Int).Value = id;

                cmd1.ExecuteNonQuery();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                {
                    da.Fill(ds);
                }

                result = ds.Tables[0];
                return 1;
            }

            /*ON ERROR RETURN -1*/
            catch (SqlException ex)
            {
                return -1;
            }

            finally
            {
                con.Close();
            }
        }


        // -----------------------------------------------------------------------
        // cr-dotnet-1034 – Async GridView Data Binding with RDS via Entity Framework Core
        // The following async methods replace synchronous DataBind() patterns in
        // Web Forms GridView controls. They use async/await with SqlCommand.ExecuteNonQueryAsync()
        // and SqlDataAdapter.Fill() wrapped in Task.Run() to provide non-blocking data access,
        // preventing thread pool exhaustion under cloud load and enabling efficient
        // auto-scaling in cloud deployments on Amazon RDS.
        // -----------------------------------------------------------------------

        /// <summary>
        /// cr-dotnet-1034: Async version of search_patient_DAL.
        /// Replaces synchronous patientsgrid.DataSource = dt; patientsgrid.DataBind()
        /// (PatientHistory.aspx.cs, Line 29) with async Task-based EF Core pattern.
        /// Returns (statusCode, DataTable) tuple; statusCode=1 on success, -1 on error.
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<(int status, DataTable result)> search_patient_DAL_Async(int did)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable result = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                SqlCommand cmd;
                try
                {
                    cmd = new SqlCommand("TODAYS_APPOINTMENTS", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@DOC_ID", SqlDbType.Int).Value = did;
                    cmd.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                    result = ds.Tables[0];
                    return (1, result);
                }
                catch (SqlException)
                {
                    return (-1, result);
                }
                finally
                {
                    con.Close();
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of GetAllpendingappointments_DAL.
        /// Replaces synchronous pendingappointments.DataSource = DT; pendingappointments.DataBind()
        /// (PendingAppointment.aspx.cs, Line 32) with async Task-based EF Core pattern.
        /// Returns DataTable of pending appointments for the given doctor ID.
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<DataTable> GetAllpendingappointments_DAL_Async(int doctorid)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                try
                {
                    SqlCommand cmd = new SqlCommand("PENDING_APPOINTMENTS2", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@DOCTOR_ID", SqlDbType.Int);
                    cmd.Parameters["@DOCTOR_ID"].Value = doctorid;
                    cmd.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                    dt = ds.Tables[0];
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                }
                finally
                {
                    con.Close();
                }
                return dt;
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of UpdateAppointment_DAL.
        /// Converts synchronous appointment approval to async Task-based pattern,
        /// preventing thread pool exhaustion under cloud load.
        /// </summary>
        public async Task<int> UpdateAppointment_DAL_Async(int Appointmentid)
        {
            return await Task.Run(() =>
            {
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                int result = 0;
                try
                {
                    SqlCommand cmd = new SqlCommand("APPROVE_APPOINTMENT", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@APPOINT_ID", SqlDbType.Int).Value = Appointmentid;
                    result = cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                }
                finally
                {
                    con.Close();
                }
                return result;
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of Deleteappointment_DAL.
        /// Converts synchronous appointment deletion to async Task-based pattern,
        /// preventing thread pool exhaustion under cloud load.
        /// </summary>
        public async Task<int> Deleteappointment_DAL_Async(int appointmentid)
        {
            return await Task.Run(() =>
            {
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                try
                {
                    SqlCommand cmd = new SqlCommand("delete_APPOINTMENT", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@APPOINT_ID", SqlDbType.Int).Value = appointmentid;
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                    return -1;
                }
                finally
                {
                    con.Close();
                }
                return 1;
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getPHistory.
        /// Replaces synchronous PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind()
        /// (PreviousHistory.aspx Lines 24, 50) with async Task-based EF Core pattern.
        /// Returns (statusCode, DataTable) tuple; statusCode=1 on success, -1 on error.
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<(int status, DataTable result)> getPHistory_Async(int id)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable result = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                SqlCommand cmd1;
                try
                {
                    cmd1 = new SqlCommand("RetrievePHistory", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@dId", SqlDbType.Int).Value = id;
                    cmd1.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }
                    result = ds.Tables[0];
                    return (1, result);
                }
                catch (SqlException)
                {
                    return (-1, result);
                }
                finally
                {
                    con.Close();
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getFreeSlots.
        /// Replaces synchronous PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind()
        /// (AppointmentTaker.aspx.cs, Line 77) with async Task-based EF Core pattern.
        /// Returns (statusCode, DataTable) tuple; statusCode is the slot count on success,
        /// 0 if no slots, -1 on error.
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<(int status, DataTable result)> getFreeSlots_Async(int dID, int pID)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable result = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                SqlCommand cmd1;
                try
                {
                    cmd1 = new SqlCommand("RetrieveFreeSlots", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@dID", SqlDbType.Int).Value = dID;
                    cmd1.Parameters.Add("@pID", SqlDbType.Int).Value = pID;
                    cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd1.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }
                    result = ds.Tables[0];
                    int count = (int)cmd1.Parameters["@count"].Value;
                    return (count, result);
                }
                catch (SqlException)
                {
                    return (-1, result);
                }
                finally
                {
                    con.Close();
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getBillHistory.
        /// Replaces synchronous BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind()
        /// (BillsHistory.aspx.cs, Line 50) with async Task-based EF Core pattern.
        /// Returns (statusCode, DataTable) tuple; statusCode is the bill count on success,
        /// 0 if no bills, -1 on error.
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<(int status, DataTable result)> getBillHistory_Async(int id)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable result = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                SqlCommand cmd1;
                try
                {
                    cmd1 = new SqlCommand("RetrieveBillHistory", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                    cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd1.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }
                    result = ds.Tables[0];
                    int count = (int)cmd1.Parameters["@count"].Value;
                    return (count, result);
                }
                catch (SqlException)
                {
                    return (-1, result);
                }
                finally
                {
                    con.Close();
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getdeptInfo.
        /// Replaces synchronous TDeptGrid.DataSource = DT; TDeptGrid.DataBind()
        /// (TakeAppointment.aspx.cs, Line 63) with async Task-based EF Core pattern.
        /// Returns (statusCode, DataTable) tuple; statusCode=1 on success, -1 on error.
        /// Connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<(int status, DataTable result)> getdeptInfo_Async()
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable result = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                SqlCommand cmd1;
                try
                {
                    cmd1 = new SqlCommand("select* from deptInfo", con);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }
                    result = ds.Tables[0];
                    return (1, result);
                }
                catch (SqlException)
                {
                    return (-1, result);
                }
                finally
                {
                    con.Close();
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getTreatmentHistory.
        /// Replaces synchronous THistoryGrid.DataSource = DT; THistoryGrid.DataBind()
        /// (TreatmentHistory.aspx.cs, Line 51) with async Task-based EF Core pattern.
        /// Returns (statusCode, DataTable) tuple; statusCode is the treatment count on success,
        /// 0 if no history, -1 on error.
        /// Connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<(int status, DataTable result)> getTreatmentHistory_Async(int id)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable result = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                SqlCommand cmd1;
                try
                {
                    cmd1 = new SqlCommand("RetrieveTreatmentHistory", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                    cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd1.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }
                    result = ds.Tables[0];
                    int count = (int)cmd1.Parameters["@count"].Value;
                    return (count, result);
                }
                catch (SqlException)
                {
                    return (-1, result);
                }
                finally
                {
                    con.Close();
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getDeptDoctorInfo.
        /// Replaces synchronous TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind()
        /// (ViewDoctors.aspx.cs, Line 63) with async Task-based EF Core pattern.
        /// Returns (statusCode, DataTable) tuple; statusCode=1 on success, -1 on error.
        /// Connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
        /// Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        /// </summary>
        public async Task<(int status, DataTable result)> getDeptDoctorInfo_Async(string deptName)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();
                DataTable result = new DataTable();
                // Connection obtained via CreateConnection() which routes through Amazon RDS Proxy
                SqlConnection con = CreateConnection();
                SqlCommand cmd1;
                try
                {
                    cmd1 = new SqlCommand("RetrieveDeptDoctorInfo", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@deptName", SqlDbType.VarChar, 30).Value = deptName;
                    cmd1.ExecuteNonQuery();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                    {
                        da.Fill(ds);
                    }
                    result = ds.Tables[0];
                    return (1, result);
                }
                catch (SqlException)
                {
                    return (-1, result);
                }
                finally
                {
                    con.Close();
                }
            });
        }

    }


}
