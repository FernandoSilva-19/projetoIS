using Newtonsoft.Json.Linq;
using Somiod.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;

namespace Somiod.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SomiodController : ApiController
    {
        string connectionString = Properties.Settings.Default.connStr;

        #region Addicional Methods
        private int GetUserIDbyApp(string appName)
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

        private int GetUserIDbyModule(string modName)
        {
            int id = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select id FROM Modules WHERE name= @modName", conn))
                {
                    cmd.Parameters.AddWithValue("@modName", modName);

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

        private IEnumerable<Models.Module> getModulesFromAppID(int id)
        {
            List<Models.Module> listModules = new List<Models.Module>();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Modules WHERE parent=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idApp", id);

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

        private IEnumerable<Models.DataSub> getDataFromModID(int id)
        {
            List<Models.DataSub> listDatas = new List<Models.DataSub>();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Datas WHERE parent=@idMod";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idMod", id);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Models.DataSub data = new Models.DataSub
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Content = (string)reader["Content"],
                        CreatedDate = (DateTime)reader["Creation_dt"],
                        Parent = (int)reader["parent"]
                    };

                    listDatas.Add(data);
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
            return listDatas;
        }

        private IEnumerable<Models.DataSub> getSubsFromModID(int id)
        {
            List<Models.DataSub> listSubs = new List<Models.DataSub>();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Subscriptions WHERE parent=@idMod";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idMod", id);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Models.DataSub subscription = new Models.DataSub
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Name = (string)reader["Name"],
                        CreatedDate = (DateTime)reader["Creation_dt"],
                        Parent = (int)reader["parent"],
                        Event = (string)reader["Event"],
                        Endpoint = (string)reader["Endpoint"]
                    };

                    listSubs.Add(subscription);
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
            return listSubs;
        }

        private string GetNameAppbyIdApp(int id)
        {
            string appName = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select name FROM Applications WHERE id= @idApp", conn))
                {
                    cmd.Parameters.AddWithValue("@idApp", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            appName = (string)reader["Name"];
                        }
                    }
                    conn.Close();
                }
            }
            return appName;
        }

        private string GetNameModbyIdMod(int id)
        {
            string modApp = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select name FROM Modules WHERE id= @idMod", conn))
                {
                    cmd.Parameters.AddWithValue("@idMod", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            modApp = (string)reader["Name"];
                        }
                    }
                    conn.Close();
                }
            }
            return modApp;
        }

        #endregion

        #region Applications
        // GET: api/<controller>
        [Route("")]
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
        [Route("{id}")]
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
        [Route("")]
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
        [Route("{id}")]
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
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {

            List<Models.Module> listModules = (List<Models.Module>)getModulesFromAppID(id);
            string nameApp = GetNameAppbyIdApp(id);
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                if (listModules.Count() > 0)
                {
                    for (int i = 0; i < listModules.Count(); i++)
                    {
                        Delete(listModules.ElementAt(i).Id, nameApp);
                    }
                }
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

        #endregion

        #region Modules

        // GET: api/<controller>
        [Route("{appName}/modules")]
        public IEnumerable<Models.Module> GetAllModules(string appName)
        {
            int id = GetUserIDbyApp(appName);

            if (id == -1)
            {
                return null;
            }

            List<Models.Module> listModules = new List<Models.Module>();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Modules WHERE parent=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idApp", id);

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
        [Route("{appName}/modules/{id}")]
        public IHttpActionResult GetModuleById(int id, string appName)
        {
            int idApp = GetUserIDbyApp(appName);

            if (idApp == -1)
            {
                return null;
            }
            Models.Module module = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT * FROM Modules WHERE parent = @idApp AND Id = @idMod";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idMod", id);
                cmd.Parameters.AddWithValue("@idApp", idApp);

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
        [Route("{appName}")]
        public IHttpActionResult Post([FromBody] Models.Module value, string appName)
        {
            int id = GetUserIDbyApp(appName);

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
        [Route("{appName}/{id}")]
        public IHttpActionResult Put(int id, [FromBody] Models.Module value, string appName)
        {
            int idApp = GetUserIDbyApp(appName);

            if (idApp == -1)
            {
                return null;
            }

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "UPDATE Modules SET Name=@name WHERE Id=@idMod AND parent=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", value.Name);
                cmd.Parameters.AddWithValue("@idMod", id);
                cmd.Parameters.AddWithValue("@idApp", idApp);


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
        [Route("{appName}/{id}")]
        public IHttpActionResult Delete(int id, string appName)
        {
            List<Models.DataSub> listDatas = (List<Models.DataSub>)getDataFromModID(id);
            List<Models.DataSub> listSubs = (List<Models.DataSub>)getSubsFromModID(id);
            string modName = GetNameModbyIdMod(id);
            int idApp = GetUserIDbyApp(appName);
            
            if (idApp == -1)
            {
                return null;
            }
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                
                    if(listSubs.Count() > 0)
                    {
                        for (int i = 0; i < listSubs.Count(); i++)
                        {
                            DeleteSub(listSubs.ElementAt(i).Id, appName, modName);
                        }
                    }
                if (listDatas.Count() > 0)
                {
                    for (int i = 0; i < listDatas.Count(); i++)
                    {
                        Delete(listDatas.ElementAt(i).Id, appName, modName);
                    }
                }
                string sql = "Delete FROM Modules WHERE Id=@idMod AND parent=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idMod", id);
                cmd.Parameters.AddWithValue("@idApp", idApp);

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

        #region Datas and Subscriptions

        // POST api/<controller>
        [Route("{appName}/{modName}")]
        public IHttpActionResult Post([FromBody] Models.DataSub value, string appName, string modName)
        {
            int id = GetUserIDbyModule(modName);

            if (id == -1)
            {
                return InternalServerError();
            }

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                
                if (value.Res_type == "data") {
                    string sql = "INSERT INTO Datas Values(@res_type, @content, @date, @parent)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@res_type", value.Res_type);
                    cmd.Parameters.AddWithValue("@content", value.Content);
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
                else if(value.Res_type == "subscription")
                {
                    string sql = "INSERT INTO Subscriptions Values(@res_type, @name, @date, @parent, @event, @endpoint)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@res_type", value.Res_type);
                    cmd.Parameters.AddWithValue("@name", value.Name);
                    cmd.Parameters.AddWithValue("@date", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@parent", id);
                    cmd.Parameters.AddWithValue("@event", value.Event);
                    cmd.Parameters.AddWithValue("@endpoint", value.Endpoint);

                    int numRegistos = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (numRegistos > 0)
                    {
                        return Ok();
                    }
                    return InternalServerError();
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


        // DELETE api/<controller>/5
        [Route("{appName}/{modName}/datas/{id}")]
        public IHttpActionResult Delete(int id, string appName, string modName)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "Delete FROM Datas WHERE Id=@idData";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idData", id);

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
        [Route("{appName}/{modName}/subscriptions/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteSub(int id, string appName, string modName)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "Delete FROM Subscriptions WHERE Id=@idSub";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idSub", id);

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

    }
}
