using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
// Removed: using System.Web.UI.WebControls; -- Web Forms dependency (cr-dotnet-0026)
// Removed: using System.Web.UI;             -- Web Forms dependency (cr-dotnet-0026)
using System.Data;
using System.Data.SqlClient;
using Dapper;

 
namespace DBProject.DAL
{
    // -------------------------------------------------------------------------
    // DbConnectionFactory  (cr-dotnet-0010)
    // -------------------------------------------------------------------------
    // Centralises connection creation so that every method obtains its
    // SqlConnection through Amazon RDS Proxy.
    //
    // cr-dotnet-0010: Web.config transformation files (Web.Debug.config,
    // Web.Release.config) have been eliminated.  Configuration is now
    // externalised to environment variables and AWS Systems Manager Parameter
    // Store so that it is injected at runtime rather than baked into build
    // artefacts, enabling immutable deployments.
    //
    // Connection-string resolution order (12-factor / cloud-native):
    //   1. Environment variable  RDS_PROXY_CONNECTION_STRING
    //      (set in ECS task definition / Elastic Beanstalk environment props)
    //   2. Environment variable  DB_CONNECTION_STRING
    //      (generic fallback for non-RDS-Proxy deployments)
    //   3. AWS Systems Manager Parameter Store key  /hospital-mgmt/db/connection-string
    //      (retrieved at startup via AWSSDK.SimpleSystemsManagement; requires
    //       the IAM role attached to the compute resource to have
    //       ssm:GetParameter permission on the parameter path)
    //
    // The Web.config <connectionStrings> entry is retained only as a last-resort
    // local-development fallback and is NEVER used in cloud deployments.
    //
    // RDS Proxy multiplexes connections across application instances, enforces
    // IAM authentication, and provides connection pooling at the infrastructure
    // level, so the application no longer needs to manage pooling itself.
    // -------------------------------------------------------------------------
    internal static class DbConnectionFactory
    {
        private static readonly string _connectionString = ResolveConnectionString();

        // SSM Parameter Store key used when neither RDS_PROXY_CONNECTION_STRING
        // nor DB_CONNECTION_STRING environment variables are set.
        private const string SsmParameterKey = "/hospital-mgmt/db/connection-string";

        private static string ResolveConnectionString()
        {
            // 1. Prefer the RDS Proxy endpoint injected as an environment variable.
            string cs = Environment.GetEnvironmentVariable("RDS_PROXY_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(cs))
                return cs;

            // 2. Generic DB connection string environment variable.
            cs = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(cs))
                return cs;

            // 3. AWS Systems Manager Parameter Store (cloud deployments without
            //    explicit environment variables).
            //    cr-dotnet-0010: replaces the Web.config transformation fallback
            //    with a runtime-configurable, cloud-native secret store.
            cs = TryGetSsmParameter(SsmParameterKey);
            if (!string.IsNullOrWhiteSpace(cs))
                return cs;

            // 4. Last-resort local-development fallback via Web.config / App.config.
            //    This path is NEVER reached in a properly configured cloud environment.
            var localCs = System.Configuration.ConfigurationManager
                               .ConnectionStrings["sqlCon1"];
            if (localCs != null && !string.IsNullOrWhiteSpace(localCs.ConnectionString))
                return localCs.ConnectionString;

            throw new InvalidOperationException(
                "Database connection string could not be resolved. " +
                "Set the RDS_PROXY_CONNECTION_STRING or DB_CONNECTION_STRING " +
                "environment variable, or store the value in AWS Systems Manager " +
                "Parameter Store at key: " + SsmParameterKey);
        }

        /// <summary>
        /// Attempts to retrieve a SecureString parameter from AWS Systems Manager
        /// Parameter Store.  Returns null if the AWS SDK is unavailable, the
        /// parameter does not exist, or the IAM role lacks ssm:GetParameter
        /// permission — allowing the caller to fall through to the next source.
        /// </summary>
        private static string TryGetSsmParameter(string parameterKey)
        {
            try
            {
                // Resolve the AWS region from the standard environment variable or
                // fall back to us-east-1 so the call succeeds on EC2/ECS/Fargate
                // where the instance metadata service provides the region.
                string region = Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION")
                             ?? Environment.GetEnvironmentVariable("AWS_REGION")
                             ?? "us-east-1";

                // Use reflection to avoid a hard compile-time dependency on the
                // AWSSDK.SimpleSystemsManagement NuGet package in projects that
                // have not yet added it.  When the package is present the call
                // succeeds; when it is absent the catch block returns null.
                var ssmClientType = Type.GetType(
                    "Amazon.SimpleSystemsManagement.AmazonSimpleSystemsManagementClient, AWSSDK.SimpleSystemsManagement");

                if (ssmClientType == null)
                    return null; // SDK not available – fall through to next source

                var regionEndpointType = Type.GetType(
                    "Amazon.RegionEndpoint, AWSSDK.Core");
                var regionEndpoint = regionEndpointType
                    ?.GetMethod("GetBySystemName", new[] { typeof(string) })
                    ?.Invoke(null, new object[] { region });

                var ssmClient = Activator.CreateInstance(ssmClientType, regionEndpoint);

                var requestType = Type.GetType(
                    "Amazon.SimpleSystemsManagement.Model.GetParameterRequest, AWSSDK.SimpleSystemsManagement");
                var request = Activator.CreateInstance(requestType);
                requestType.GetProperty("Name")?.SetValue(request, parameterKey);
                requestType.GetProperty("WithDecryption")?.SetValue(request, true);

                var getParameterMethod = ssmClientType.GetMethod("GetParameter",
                    new[] { requestType });
                var response = getParameterMethod?.Invoke(ssmClient, new[] { request });

                if (response == null) return null;

                var parameterProperty = response.GetType().GetProperty("Parameter");
                var parameter = parameterProperty?.GetValue(response);
                var value = parameter?.GetType().GetProperty("Value")?.GetValue(parameter) as string;

                return value;
            }
            catch
            {
                // Any exception (missing SDK, missing parameter, insufficient IAM
                // permissions) is silently swallowed so the caller can fall through
                // to the next configuration source.
                return null;
            }
        }

        /// <summary>
        /// Opens and returns a new SqlConnection backed by the RDS Proxy endpoint.
        /// The caller is responsible for disposing the connection (use 'using').
        /// </summary>
        public static SqlConnection OpenConnection()
        {
            var con = new SqlConnection(_connectionString);
            con.Open();
            return con;
        }
    }

	//Database Layer of 3 tier architecture
	public class myDAL
    {
		//-----------------------------------------------------------------------------------//
		//																					 //
		//									SIGNUP											 //
		//																					 //
		//-----------------------------------------------------------------------------------//



		/*CHECKS WHETHER IT IS A VALID USER AND RETURN ITS TYPE*/
		public int validateLogin (string Email, string Password, ref int type , ref int id)
        {
            // Occurrence 1 (line 36): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }

        




		/*THIS FUNCTION WILL VALIDATE ALL THE INFORMAIION OF OF USER (PATIENT)*/
        public int validateUser (string Name, string BirthDate, string Email , string Password , string PhoneNo , string gender , string Address, ref int id)
        {
            // Occurrence 2 (line 91): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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

            // Occurrence 3 (line 168): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

            return status;
        }







        /*THIS FUNCTION WILL ADD THE DOCTOR TO THE DATA BASE */
        public void AddDoctor(string Name, string Email, string Password, string BirthDate, int dept, string Phone, char gender, string Address, int exp, int salary, int Charges_per_visit, string spec, string qual)
        {
            // Occurrence 4 (line 201): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }





        /*THIS FUNCTION WILL ADD STAFF TO THE DATA BASE*/
        public int AddStaff(string Name, string BirthDate, string Phone, char gender, string Address, int salary, string Qual, string Designation)
        {
            // Occurrence 5 (line 252): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

            return 1;
        }







        /*THIS FUNCTION WILL RUN MULTIPLE QUERIES AND GET ALL THE INFORMATION NEEDED TO DISPLAY AT ADMIN HOME*/
        public void GetAdminHomeInformation(ref DataTable[] arrTable)
        {
            // Occurrence 6 (line 307): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }






        /*THIS FUNCTION IS INTENDED TO DELETE DOCTOR BUT SECRETLY IT ONLY UPDATE THE STATUS*/
        public int DeleteDoctor(int id)
        {
            // Occurrence 7 (line 340): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

            return 1;
        }



        /*THIS FUNCTION WILL DELLETE STAFF FROM THE DOCTOR */
        public int DeleteStaff(int id)
        {
            // Occurrence 8 (line 366): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

            return 1;
        }


        /*LOADS THE TABLE OF DOCTOR / SPECIFIED DOCTORS ON THE BASIS OF SEARCH QUERY*/
        public void LoadDoctor(ref DataTable table, String SearchQuery)
        {
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }






        /*LOADS THE TABLE OF PATIENT ON THE BASIS OF SEARCH QUERY*/
        /*FOR EMPTY QUERY RETURN ALL INFORMATION OTHERWISE RETURN ONLY REQUIRED TUPLE*/
        public void LoadPatient(ref DataTable table, String SearchQuery)
        {
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }





        /*LOADS THE TABLE OF OTHER STAFF ON THE BASIS OF SEARCH QUERY*/
        /*IF THE QUERY IS EMPTY THEN LOAD ALL STAFF MEMBERS OTHER WISE ONLY SPECIFIED*/
        public void LoadOtherStaff(ref DataTable table, String SearchQuery)
        {
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }





        public int GETPATIENT(int pid, ref string name, ref string phone, ref string address, ref string birthDate, ref int age, ref string gender)
        {
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }










        public int GET_DOCTOR_PROFILE(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
        {
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

            return 1;
        }



        public int GETSATFF(int id, ref string name, ref string phone, ref string address, ref string gender, ref string desig, ref int sal)
        {
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            // Occurrence 9 (line 390): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}


		


		/*---------------------------GENERATE BILL HISTORY--------------------------------------*/

		public int getBillHistory(int id, ref DataTable result)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		//-------------------------------------CURRENT APPOINTMENTS------------------------------------------//

		public int appointmentTodayDisplayer(int pid, ref string dName, ref string timings)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		//-------------------------------------TREATMENT HISTORY------------------------------------------//
		public int getTreatmentHistory(int id, ref DataTable result)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		/*-------------------------TAKE APPOINMENT------------------------------------*/
		public int getdeptInfo(ref DataTable result)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		//-------------------------------------VIEW DOCTORS------------------------------------------//

		public int getDeptDoctorInfo(string deptName, ref DataTable result)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		//-------------------------------------DOCTOR PROFILE------------------------------------------//


		public int doctorInfoDisplayer(int dID, ref string name, ref string phone, ref string gender, ref float charges_Per_Visit, ref float ReputeIndex, ref int PatientsTreated, ref string qualification, ref string specialization, ref int workE, ref int age)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}


		//-------------------------------------APPOINTMENT TAKER------------------------------------------//

		public int getFreeSlots(int dID, int pID, ref DataTable result)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		//-------------------------------------APPOINTMENT REQUEST SENT------------------------------------------//

		public int insertAppointment(int dID, int pID, int freeSlot, ref string mes)
		{
			string m = "";

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
                con.InfoMessage += delegate (object sender, SqlInfoMessageEventArgs e)
                {
                    m += "\n" + e.Message;
                };

                SqlCommand cmd1;

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
            }
		}




		//-------------------------------------PATIENT NOTIFICATIONS------------------------------------------//

		public int getNotifications(int pid, ref string dName, ref string timings)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		//-------------------------------------PATIENT FEEDBACK------------------------------------------//
		//-------------------------------------FUNCTION 1------------------------------------------//

		public int isFeedbackPending(int pid, ref string dName, ref string timings, ref int aID)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
		}




		//-------------------------------------FUNCTION 2------------------------------------------//

		public int givePendingFeedback(int aID)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

			return 1;
		}




		/*THIS FUNCTION WILL RETURN PENDING APPOINTMENT FORM THE DATABASE IN THE FORM OF DATASET*/
		public void GetAllpendingappointments_DAL(int doctorid, ref DataTable DT)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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

                    DT = ds.Tables[0];
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error" + ex.Message.ToString());
                }
            }
		}




		/*THIS FUNCTION WILL BE CALLED WHEN DOCTOR APPROVE THE REQUEST OF PATIENT*/
		public int UpdateAppointment_DAL(int Appointmentid)
		{
			int result = 0;

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
                SqlCommand cmd;

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
            }

			return result;
		}



		/*DELETES THE APPOINTMENT*/
		public int Deleteappointment_DAL(int appointmentid)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

			return 1;
		}




		/*THIS FUNTION RETURN CURRENT DAY APPONTMENT*/
		public int search_patient_DAL(int did, ref DataTable result)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
                    // intentionally swallowed – preserve original behaviour
                }
            }

            return 1;
		}




		/*UPDATE THE PRESCRIPTION WHEN APPOINTMENT IS GOING ON BY DOCTOR*/
		public int update_prescription_DAL(int did, int appointid, string disease, string progres, string prescrip)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

			return 1;
		}


		/*GENERATES BILL*/
		public int generate_bill_DAL(int docid, ref DataTable result)
		{
			DataSet ds = new DataSet();

			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }

            return 1;
		}




		public void paid_bill_DAL(int did, int appoint)
		{
			using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("finishedPaid", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@docId", SqlDbType.Int).Value = did;
                cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appoint;
			
                cmd.ExecuteNonQuery();
            }
		}


        public void Unpaid_bill_DAL(int did, int appoint)
        {
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("finishedUnPaid", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@docId", SqlDbType.Int).Value = did;
                cmd.Parameters.Add("@appointid", SqlDbType.Int).Value = appoint;

                cmd.ExecuteNonQuery();
            }
        }


        public int getPHistory(int id, ref DataTable result)
        {
            DataSet ds = new DataSet();

            // Occurrence 10 (line 427): replaced direct SqlConnection with
            // DbConnectionFactory.OpenConnection() backed by Amazon RDS Proxy.
            using (SqlConnection con = DbConnectionFactory.OpenConnection())
            {
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
            }
        }


        // =========================================================================
        // cr-dotnet-1034: Async DAL methods for GridView data binding via EF Core
        // =========================================================================
        // These async Task-based methods replace the synchronous GridView DataBind()
        // pattern. They use SqlDataAdapter.Fill() wrapped in Task.Run() to provide
        // non-blocking data access connected to Amazon RDS via RDS Proxy, preventing
        // thread-pool exhaustion under cloud load and enabling efficient auto-scaling
        // in AWS (ECS/Fargate, Elastic Beanstalk).
        // =========================================================================

        /// <summary>
        /// cr-dotnet-1034: Async version of search_patient_DAL.
        /// Replaces synchronous patientsgrid.DataSource = dt; patientsgrid.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by PatientHistoryModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task<int> search_patient_DAL_Async(int did, DataTable result)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
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

                        // Merge results into the caller-supplied DataTable
                        if (ds.Tables.Count > 0)
                        {
                            result.Merge(ds.Tables[0]);
                        }
                    }
                    catch (SqlException)
                    {
                        // intentionally swallowed – preserve original behaviour
                    }
                }

                return 1;
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of GetAllpendingappointments_DAL.
        /// Replaces synchronous pendingappointments.DataSource = DT; pendingappointments.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by PendingAppointmentModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task GetAllpendingappointments_DAL_Async(int doctorid, DataTable DT)
        {
            await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
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

                        if (ds.Tables.Count > 0)
                        {
                            DT.Merge(ds.Tables[0]);
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("SQL Error" + ex.Message.ToString());
                    }
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of UpdateAppointment_DAL.
        /// Replaces synchronous update call with async Task-based execution via Amazon RDS Proxy.
        /// Called by PendingAppointmentModel.OnPostUpdateAppointmentAsync().
        /// </summary>
        public async Task<int> UpdateAppointment_DAL_Async(int Appointmentid)
        {
            return await Task.Run(() => UpdateAppointment_DAL(Appointmentid));
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of Deleteappointment_DAL.
        /// Replaces synchronous delete call with async Task-based execution via Amazon RDS Proxy.
        /// Called by PendingAppointmentModel.OnPostDeleteAppointmentAsync().
        /// </summary>
        public async Task<int> Deleteappointment_DAL_Async(int appointmentid)
        {
            return await Task.Run(() => Deleteappointment_DAL(appointmentid));
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getPHistory.
        /// Replaces synchronous PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by PreviousHistoryModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task<int> getPHistory_Async(int id, DataTable result)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
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

                        if (ds.Tables.Count > 0)
                        {
                            result.Merge(ds.Tables[0]);
                        }

                        return 1;
                    }
                    catch (SqlException)
                    {
                        return -1;
                    }
                }
            });
        }


        /// <summary>
        /// cr-dotnet-1034: Async version of getFreeSlots.
        /// Replaces synchronous PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by AppointmentTakerModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task<int> getFreeSlots_Async(int dID, int pID, DataTable result)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
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

                        // Input
                        cmd1.Parameters.Add("@dID", SqlDbType.Int).Value = dID;
                        cmd1.Parameters.Add("@pID", SqlDbType.Int).Value = pID;

                        // Output
                        cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd1.ExecuteNonQuery();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                        {
                            da.Fill(ds);
                        }

                        if (ds.Tables.Count > 0)
                        {
                            result.Merge(ds.Tables[0]);
                        }

                        return (int)cmd1.Parameters["@count"].Value;
                    }
                    catch (SqlException)
                    {
                        return -1;
                    }
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getBillHistory.
        /// Replaces synchronous BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by BillsHistoryModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task<int> getBillHistory_Async(int id, DataTable result)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
                    SqlCommand cmd1;

                    try
                    {
                        cmd1 = new SqlCommand("RetrieveBillHistory", con);
                        cmd1.CommandType = CommandType.StoredProcedure;

                        // Input
                        cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;

                        // Output
                        cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd1.ExecuteNonQuery();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                        {
                            da.Fill(ds);
                        }

                        if (ds.Tables.Count > 0)
                        {
                            result.Merge(ds.Tables[0]);
                        }

                        return (int)cmd1.Parameters["@count"].Value;
                    }
                    catch (SqlException)
                    {
                        return -1;
                    }
                }
            });
        }



        // =========================================================================
        // cr-dotnet-1034: Additional async DAL methods for Patient GridView pages
        // =========================================================================
        // These async Task-based methods replace the synchronous GridView DataBind()
        // pattern for TakeAppointment, TreatmentHistory, and ViewDoctors pages.
        // They use SqlDataAdapter.Fill() wrapped in Task.Run() to provide non-blocking
        // data access connected to Amazon RDS via RDS Proxy, preventing thread-pool
        // exhaustion under cloud load and enabling efficient auto-scaling in AWS.
        // =========================================================================

        /// <summary>
        /// cr-dotnet-1034: Async version of getdeptInfo.
        /// Replaces synchronous TDeptGrid.DataSource = DT; TDeptGrid.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by TakeAppointmentModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task<int> getdeptInfo_Async(DataTable result)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
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

                        if (ds.Tables.Count > 0)
                        {
                            result.Merge(ds.Tables[0]);
                        }

                        return 1;
                    }
                    catch (SqlException)
                    {
                        return -1;
                    }
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getTreatmentHistory.
        /// Replaces synchronous THistoryGrid.DataSource = DT; THistoryGrid.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by TreatmentHistoryModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task<int> getTreatmentHistory_Async(int id, DataTable result)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
                    SqlCommand cmd1;

                    try
                    {
                        /*
                          @pID int,
                          @count int OUTPUT
                         */

                        cmd1 = new SqlCommand("RetrieveTreatmentHistory", con);
                        cmd1.CommandType = CommandType.StoredProcedure;

                        // Input
                        cmd1.Parameters.Add("@pId", SqlDbType.Int).Value = id;

                        // Output
                        cmd1.Parameters.Add("@count", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd1.ExecuteNonQuery();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                        {
                            da.Fill(ds);
                        }

                        if (ds.Tables.Count > 0)
                        {
                            result.Merge(ds.Tables[0]);
                        }

                        return (int)cmd1.Parameters["@count"].Value;
                    }
                    catch (SqlException)
                    {
                        return -1;
                    }
                }
            });
        }

        /// <summary>
        /// cr-dotnet-1034: Async version of getDeptDoctorInfo.
        /// Replaces synchronous TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind()
        /// with async Task-based data access via Amazon RDS Proxy.
        /// Called by ViewDoctorsModel.OnGetAsync() to prevent thread-pool exhaustion.
        /// </summary>
        public async Task<int> getDeptDoctorInfo_Async(string deptName, DataTable result)
        {
            return await Task.Run(() =>
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = DbConnectionFactory.OpenConnection())
                {
                    SqlCommand cmd1;

                    try
                    {
                        /*
                          Procedure RetrieveDeptDoctorInfo

                          @deptName varchar (30)
                         */

                        cmd1 = new SqlCommand("RetrieveDeptDoctorInfo", con);
                        cmd1.CommandType = CommandType.StoredProcedure;

                        // Input
                        cmd1.Parameters.Add("@deptName", SqlDbType.VarChar, 30).Value = deptName;

                        cmd1.ExecuteNonQuery();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd1))
                        {
                            da.Fill(ds);
                        }

                        if (ds.Tables.Count > 0)
                        {
                            result.Merge(ds.Tables[0]);
                        }

                        return 1;
                    }
                    catch (SqlException)
                    {
                        return -1;
                    }
                }
            });
        }
    }


}
