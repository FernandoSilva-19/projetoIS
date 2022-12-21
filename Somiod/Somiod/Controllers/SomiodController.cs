using Somiod.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Somiod.Controllers
{
    public class SomiodController : ApiController
    {
        string connectionString = Properties.Settings.Default.connStr;

        #region Applications
        // GET: api/<controller>
        [Route("api/somiod")]
        public IEnumerable<Models.Application> GetAllApplications()
        {
            List<Models.Application> listApplications = new List<Models.Application>();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Applications";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Models.Application app = new Models.Application
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Name = (string)reader["Name"],
                        CreatedDate = (DateTime)reader["Creation_dt"]
                    };

                    listApplications.Add(app);
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return listApplications;
        }


        // GET api/<controller>/5
        [Route("api/somiod/{id}")]
        public IHttpActionResult GetApplicationById(int id)
        {
            Models.Application app = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Applications WHERE Id = @idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idApp", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    app = new Models.Application
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Name = (string)reader["Name"],
                        CreatedDate = (DateTime)reader["Creation_dt"]
                    };
                }
                reader.Close();
                conn.Close();
                if (app == null)
                {
                    return NotFound();
                }
                return Ok(app);
            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return NotFound();
            }
        }

        // POST api/<controller>
        [Route("api/somiod")]
        public IHttpActionResult Post([FromBody] Models.Application value)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "INSERT INTO Applications Values(@res_type, @name, @date)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@res_type", value.Res_type);
                cmd.Parameters.AddWithValue("@name", value.Name);
                cmd.Parameters.AddWithValue("@date", DateTime.UtcNow);

                int numRegistos = cmd.ExecuteNonQuery();
                conn.Close();
                if (numRegistos > 0)
                {
                    return Ok();
                }
                return InternalServerError();

            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return InternalServerError();
            }
        }

        // PUT api/<controller>/5
        [Route("api/somiod/{id}")]
        public IHttpActionResult Put(int id, [FromBody] Models.Application value)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "UPDATE Applications SET Name=@name WHERE Id=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", value.Name);
                cmd.Parameters.AddWithValue("@idApp", id);


                int numRegistos = cmd.ExecuteNonQuery();
                conn.Close();
                if (numRegistos > 0)
                {
                    return Ok();
                }
                return NotFound();

            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return InternalServerError();
            }
        }

        // DELETE api/<controller>/5
        [Route("api/somiod/{id}")]
        public IHttpActionResult Delete(int id)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "Delete FROM Applications WHERE Id=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idApp", id);

                int numRegistos = cmd.ExecuteNonQuery();
                conn.Close();
                if (numRegistos > 0)
                {
                    return Ok();
                }
                return NotFound();

            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return InternalServerError();
            }
        }

        private int GetUserID(string appName)
        {
            int id = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select id FROM Applications WHERE name= @appName", conn))
                {
                    cmd.Parameters.AddWithValue("@appName", appName);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = (int)reader["Id"];
                        }
                    }
                    conn.Close();
                }
            }
            return id;
        }

        #endregion

        #region Modules

        // GET: api/<controller>
        [Route("api/somiod/modules")]
        public IEnumerable<Models.Module> GetAllModules()
        {
            List<Models.Module> listModules = new List<Models.Module>();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Modules";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Models.Module module = new Models.Module
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Name = (string)reader["Name"],
                        CreatedDate = (DateTime)reader["Creation_dt"],
                        Parent = (int)reader["parent"]
                    };

                    listModules.Add(module);
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return listModules;
        }

        // GET api/<controller>/5
        [Route("api/somiod/modules/{id}")]
        public IHttpActionResult GetModuleById(int id)
        {
            Models.Module module = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Applications WHERE Id = @idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idApp", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    module = new Models.Module
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Name = (string)reader["Name"],
                        CreatedDate = (DateTime)reader["Creation_dt"],
                        Parent = (int)reader["parent"]
                    };
                }
                reader.Close();
                conn.Close();
                if (module == null)
                {
                    return NotFound();
                }
                return Ok(module);
            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return NotFound();
            }
        }

        // POST api/<controller>
        [Route("api/somiod/{appName}")]
        public IHttpActionResult Post([FromBody] Models.Module value, string appName)
        {
            int id = GetUserID(appName);

            if(id == -1)
            {
                return InternalServerError();
            }

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                
                string sql = "INSERT INTO Modules Values(@res_type, @name, @date, @parent)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@res_type", value.Res_type);
                cmd.Parameters.AddWithValue("@name", value.Name);
                cmd.Parameters.AddWithValue("@date", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@parent", id);


                int numRegistos = cmd.ExecuteNonQuery();
                conn.Close();
                if (numRegistos > 0)
                {
                    return Ok();
                }
                return InternalServerError();

            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return InternalServerError();
            }
        }

        // PUT api/<controller>/5
        [Route("api/somiod/{appName}/{id}")]
        public IHttpActionResult Put(int id, [FromBody] Models.Application value, string appName)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "UPDATE Modules SET Name=@name WHERE Id=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", value.Name);
                cmd.Parameters.AddWithValue("@idApp", id);


                int numRegistos = cmd.ExecuteNonQuery();
                conn.Close();
                if (numRegistos > 0)
                {
                    return Ok();
                }
                return NotFound();

            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return InternalServerError();
            }
        }

        // DELETE api/<controller>/5
        [Route("api/somiod/{appName}/{id}")]
        public IHttpActionResult Delete(int id, string appName)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "Delete FROM Modules WHERE Id=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idApp", id);

                int numRegistos = cmd.ExecuteNonQuery();
                conn.Close();
                if (numRegistos > 0)
                {
                    return Ok();
                }
                return NotFound();

            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return InternalServerError();
            }
        }
        #endregion

        #region Datas

        #endregion
    }
}
