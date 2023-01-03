using Newtonsoft.Json.Linq;
using Somiod.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Somiod.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SomiodController : ApiController
    {
        MqttClient subscriber = null;
        MqttClient publisher = null;
        string connectionString = Properties.Settings.Default.connStr;
        private static int x = 0;

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
                        CreatedDate = (string)reader["Creation_dt"],
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
                        CreatedDate = (string)reader["Creation_dt"],
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
                        CreatedDate = (string)reader["Creation_dt"],
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
        public IHttpActionResult GetAllApplications()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            XmlElement applicationsElement = doc.CreateElement(string.Empty, "Applications", string.Empty);
            doc.AppendChild(applicationsElement);

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
                        CreatedDate = (string)reader["Creation_dt"]
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
            foreach (Models.Application app in listApplications)
            {
                XmlElement applicationElement = doc.CreateElement(string.Empty, "Application", string.Empty);
                applicationsElement.AppendChild(applicationElement);

                XmlElement idElement = doc.CreateElement(string.Empty, "Id", string.Empty);
                idElement.AppendChild(doc.CreateTextNode(app.Id.ToString()));
                applicationElement.AppendChild(idElement);

                XmlElement resTypeElement = doc.CreateElement(string.Empty, "Res_Type", string.Empty);
                resTypeElement.AppendChild(doc.CreateTextNode(app.Res_type));
                applicationElement.AppendChild(resTypeElement);

                XmlElement nameElement = doc.CreateElement(string.Empty, "Name", string.Empty);
                nameElement.AppendChild(doc.CreateTextNode(app.Name));
                applicationElement.AppendChild(nameElement);

                XmlElement createdDateElement = doc.CreateElement(string.Empty, "Date", string.Empty);
                createdDateElement.AppendChild(doc.CreateTextNode(app.CreatedDate));
                applicationElement.AppendChild(createdDateElement);
            }

            return Ok(doc.OuterXml);
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
                        CreatedDate = (string)reader["Creation_dt"]
                    };
                }
                reader.Close();
                conn.Close();
                if (app == null)
                {
                    return NotFound();
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDeclaration, root);

                XmlElement applicationElement = xmlDoc.CreateElement("Application");
                xmlDoc.AppendChild(applicationElement);

                XmlElement idElement = xmlDoc.CreateElement("Id");
                idElement.InnerText = app.Id.ToString();
                applicationElement.AppendChild(idElement);

                XmlElement resTypeElement = xmlDoc.CreateElement("Res_type");
                resTypeElement.InnerText = app.Res_type;
                applicationElement.AppendChild(resTypeElement);

                XmlElement nameElement = xmlDoc.CreateElement("Name");
                nameElement.InnerText = app.Name;
                applicationElement.AppendChild(nameElement);

                XmlElement createdDateElement = xmlDoc.CreateElement("Date");
                createdDateElement.InnerText = app.CreatedDate;
                applicationElement.AppendChild(createdDateElement);

                return Ok(xmlDoc.OuterXml);
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
        public async Task<IHttpActionResult> Post()
        {
            XmlDocument doc = new XmlDocument();
            using (var content = await Request.Content.ReadAsStreamAsync())
            {
                content.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(content))
                {
                    string raw = sr.ReadToEnd();
                    doc.LoadXml(raw);
                }
            }

            XmlNode nodeName = doc.SelectSingleNode("/root/Name");
            string name = nodeName.InnerText;
            XmlNode nodeResType = doc.SelectSingleNode("/root/Res_type");
            string res_Type = nodeResType.InnerText;

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "INSERT INTO Applications Values(@res_type, @name, @date)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@res_type", res_Type);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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
        public async Task<IHttpActionResult> Put(int id)
        {
            XmlDocument doc = new XmlDocument();
            using (var content = await Request.Content.ReadAsStreamAsync())
            {
                content.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(content))
                {
                    string raw = sr.ReadToEnd();
                    doc.LoadXml(raw);
                    MessageBox.Show(raw);
                }
            }

            XmlNode nodeName = doc.SelectSingleNode("/root/Name");
            string name = nodeName.InnerText;

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "UPDATE Applications SET Name=@name WHERE Id=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", name);
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
        [Route("{appName:alpha}")]
        public IHttpActionResult GetAllModules(string appName)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            XmlElement applicationsElement = doc.CreateElement(string.Empty, "Modules", string.Empty);
            doc.AppendChild(applicationsElement);

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
                        CreatedDate = (string)reader["Creation_dt"],
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

            foreach (Models.Module module in listModules)
            {
                XmlElement applicationElement = doc.CreateElement(string.Empty, "Module", string.Empty);
                applicationsElement.AppendChild(applicationElement);

                XmlElement idElement = doc.CreateElement(string.Empty, "Id", string.Empty);
                idElement.AppendChild(doc.CreateTextNode(module.Id.ToString()));
                applicationElement.AppendChild(idElement);

                XmlElement resTypeElement = doc.CreateElement(string.Empty, "Res_Type", string.Empty);
                resTypeElement.AppendChild(doc.CreateTextNode(module.Res_type));
                applicationElement.AppendChild(resTypeElement);

                XmlElement nameElement = doc.CreateElement(string.Empty, "Name", string.Empty);
                nameElement.AppendChild(doc.CreateTextNode(module.Name));
                applicationElement.AppendChild(nameElement);

                XmlElement createdDateElement = doc.CreateElement(string.Empty, "Date", string.Empty);
                createdDateElement.AppendChild(doc.CreateTextNode(module.CreatedDate));
                applicationElement.AppendChild(createdDateElement);
            }

            return Ok(doc.OuterXml);
        }

        // GET api/<controller>/5
        [Route("{appName}/{id}")]
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
                string sql = "SELECT * FROM Modules WHERE parent = @idApp AND Id = @idMod";
                string sqlData = "SELECT * FROM Datas WHERE parent = @idMod";
                Console.WriteLine(sql);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlCommand cmd2 = new SqlCommand(sqlData, conn);
                cmd.Parameters.AddWithValue("@idMod", id);
                cmd.Parameters.AddWithValue("@idApp", idApp);
                cmd2.Parameters.AddWithValue("@idMod", id);

                List<Models.Data> listDatas = new List<Models.Data>();


                SqlDataReader readerData = cmd2.ExecuteReader();
                while (readerData.Read())
                {
                    listDatas.Add(new Data
                    {
                        Id = (int)readerData["Id"],
                        Res_type = (string)readerData["Res_Type"],
                        Content = (string)readerData["Content"],
                        CreatedDate = (string)readerData["Creation_dt"],
                        Parent = (int)readerData["parent"]
                    });
                }
                readerData.Close();
                conn.Close();
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    module = new Models.Module
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Name = (string)reader["Name"],
                        CreatedDate = (string)reader["Creation_dt"],
                        Parent = (int)reader["parent"],
                        datas = listDatas
                    };
                }
                reader.Close();
                conn.Close();
                if (module == null)
                {
                    return NotFound();
                }
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDeclaration, root);

                XmlElement moduleElement = xmlDoc.CreateElement("Module");
                xmlDoc.AppendChild(moduleElement);

                XmlElement idElement = xmlDoc.CreateElement("Id");
                idElement.InnerText = module.Id.ToString();
                moduleElement.AppendChild(idElement);

                // Create the "res_type" element
                XmlElement resTypeElement = xmlDoc.CreateElement("Res_type");
                resTypeElement.InnerText = module.Res_type;
                moduleElement.AppendChild(resTypeElement);

                // Create the "name" element
                XmlElement nameElement = xmlDoc.CreateElement("Name");
                nameElement.InnerText = module.Name;
                moduleElement.AppendChild(nameElement);

                // Create the "created_date" element
                XmlElement createdDateElement = xmlDoc.CreateElement("Date");
                createdDateElement.InnerText = module.CreatedDate;
                moduleElement.AppendChild(createdDateElement);

                foreach (Models.Data data in listDatas)
                {
                    XmlElement Data = xmlDoc.CreateElement("Data");
                    moduleElement.AppendChild(Data);

                    XmlElement idData = xmlDoc.CreateElement("Id");
                    idData.InnerText = data.Id.ToString();
                    Data.AppendChild(idData);

                    XmlElement resTypeData = xmlDoc.CreateElement("Res_type");
                    resTypeData.InnerText = data.Res_type.ToString();
                    Data.AppendChild(resTypeData);

                    XmlElement contentData = xmlDoc.CreateElement("Content");
                    contentData.InnerText = data.Content.ToString();
                    Data.AppendChild(contentData);

                    XmlElement creationData = xmlDoc.CreateElement("Creation_Date");
                    creationData.InnerText = data.CreatedDate.ToString();
                    Data.AppendChild(creationData);

                    XmlElement parentData = xmlDoc.CreateElement("Parent");
                    parentData.InnerText = data.Parent.ToString();
                    Data.AppendChild(parentData);
                }

                return Ok(xmlDoc.OuterXml);

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
        public async Task<IHttpActionResult> Post(string appName)
        {
            XmlDocument doc = new XmlDocument();
            using (var content = await Request.Content.ReadAsStreamAsync())
            {
                content.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(content))
                {
                    string raw = sr.ReadToEnd();
                    doc.LoadXml(raw);
                }
            }

            XmlNode nodeName = doc.SelectSingleNode("/root/Name");
            string name = nodeName.InnerText;
            XmlNode nodeResType = doc.SelectSingleNode("/root/Res_type");
            string res_Type = nodeResType.InnerText;

            int id = GetUserIDbyApp(appName);

            if (id == -1)
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
                cmd.Parameters.AddWithValue("@res_type", res_Type);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
        public async Task<IHttpActionResult> Put(int id, string appName)
        {
            XmlDocument doc = new XmlDocument();
            using (var content = await Request.Content.ReadAsStreamAsync())
            {
                content.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(content))
                {
                    string raw = sr.ReadToEnd();
                    doc.LoadXml(raw);
                }
            }

            XmlNode nodeName = doc.SelectSingleNode("/root/Name");
            string name = nodeName.InnerText;
            XmlNode nodeParent = doc.SelectSingleNode("/root/Parent");
            string Parent = nodeParent.InnerText;

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
                string sql = "UPDATE Modules SET Name=@name, Parent=@parent WHERE Id=@idMod AND parent=@idApp";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@parent", Parent);
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

                if (listSubs.Count() > 0)
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

        #region Datas and Subscriptions Post

        // POST api/<controller>
        [Route("{appName}/{modName}")]
        public async Task<IHttpActionResult> Post(string appName, string modName)
        {
            XmlDocument doc = new XmlDocument();
            using (var content = await Request.Content.ReadAsStreamAsync())
            {
                content.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(content))
                {
                    string raw = sr.ReadToEnd();
                    doc.LoadXml(raw);
                }
            }

            XmlNode nodeResType = doc.SelectSingleNode("/root/Res_type");
            string res_Type = nodeResType.InnerText;

            int id = GetUserIDbyModule(modName);
            var sub = getSubsFromModID(id);
            if (x == 0)
            {
                for (int i = 0; i < sub.Count(); i++)
                {
                    if (String.Equals("creation", sub.ElementAt(i).Event, StringComparison.OrdinalIgnoreCase) || String.Equals("both", sub.ElementAt(i).Event, StringComparison.OrdinalIgnoreCase))
                    {
                        Subscribe(modName, sub.ElementAt(i).Endpoint);
                    }
                }
                x = 1;
            }
            if (id == -1)
            {
                return InternalServerError();
            }

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                if (String.Equals("data", res_Type, StringComparison.OrdinalIgnoreCase))
                {
                    XmlNode nodeContent = doc.SelectSingleNode("/root/Content");
                    string contentData = nodeContent.InnerText;

                    string sql = "INSERT INTO Datas Values(@res_type, @content, @date, @parent)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@res_type", res_Type);
                    cmd.Parameters.AddWithValue("@content", contentData);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@parent", id);

                    int numRegistos = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (numRegistos > 0)
                    {
                        for (int i = 0; i < sub.Count(); i++)
                        {
                            // Se for preciso só contar as subs que são criadas em run time -> & Convert.ToDateTime(sub.ElementAt(i).CreatedDate) > System.Diagnostics.Process.GetCurrentProcess().StartTime
                            if (String.Equals("creation", sub.ElementAt(i).Event, StringComparison.OrdinalIgnoreCase) || String.Equals("both", sub.ElementAt(i).Event, StringComparison.OrdinalIgnoreCase))
                            {
                                Publish(modName, contentData, sub.ElementAt(i).Endpoint);
                                return Ok();
                            }
                        }
                        return Ok();

                    }
                    return InternalServerError();
                }
                else if (String.Equals("subscription", res_Type, StringComparison.OrdinalIgnoreCase))
                {
                    XmlNode nodeName = doc.SelectSingleNode("/root/Name");
                    string name = nodeName.InnerText;
                    XmlNode nodeEvento = doc.SelectSingleNode("/root/Event");
                    string evento = nodeEvento.InnerText;
                    XmlNode nodeEndpoint = doc.SelectSingleNode("/root/Endpoint");
                    string endpoint = nodeEndpoint.InnerText;

                    string sql = "INSERT INTO Subscriptions Values(@res_type, @name, @date, @parent, @event, @endpoint)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@res_type", res_Type);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@parent", id);
                    cmd.Parameters.AddWithValue("@event", evento);
                    cmd.Parameters.AddWithValue("@endpoint", endpoint);

                    int numRegistos = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (numRegistos > 0)
                    {
                        if (String.Equals("creation", evento, StringComparison.OrdinalIgnoreCase) || String.Equals("both", evento, StringComparison.OrdinalIgnoreCase))
                        {
                            Subscribe(modName, endpoint);
                        }

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

        #endregion

        #region Data
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

        // GET: api/<controller>
        [Route("{appName}/{modName}/latestdata")]
        public IHttpActionResult getLastData(string modName)
        {
            Models.Data data = null;
            int id = GetUserIDbyModule(modName);
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                string sql = "SELECT TOP 1 * FROM Datas where parent=@idMod ORDER BY ID DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idMod", id);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data = new Models.Data
                    {
                        Id = (int)reader["Id"],
                        Res_type = (string)reader["Res_Type"],
                        Content = (string)reader["Content"],
                        CreatedDate = (string)reader["Creation_dt"],
                        Parent = (int)reader["parent"]
                    };
                }
                reader.Close();
                conn.Close();
                if (data == null)
                {
                    return NotFound();
                }
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDeclaration, root);

                XmlElement dataElement = xmlDoc.CreateElement("Data");
                xmlDoc.AppendChild(dataElement);

                XmlElement idElement = xmlDoc.CreateElement("Id");
                idElement.InnerText = data.Id.ToString();
                dataElement.AppendChild(idElement);

                XmlElement resTypeElement = xmlDoc.CreateElement("Res_type");
                resTypeElement.InnerText = data.Res_type;
                dataElement.AppendChild(resTypeElement);

                XmlElement contentElement = xmlDoc.CreateElement("Content");
                contentElement.InnerText = data.Content;
                dataElement.AppendChild(contentElement);

                XmlElement createdDateElement = xmlDoc.CreateElement("CreationDate");
                createdDateElement.InnerText = data.CreatedDate;
                dataElement.AppendChild(createdDateElement);

                XmlElement parentElement = xmlDoc.CreateElement("Parent");
                parentElement.InnerText = data.Parent.ToString();
                dataElement.AppendChild(parentElement);

                return Ok(xmlDoc.OuterXml);
            }
            catch (Exception)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return NotFound();
        }

        #endregion

        #region Subscription

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

        #region Subscribe to a channel method

        public void Subscribe(string modName, string endpoint)
        {
            string[] topics = { modName };
            subscriber = new MqttClient(IPAddress.Parse(endpoint));
            subscriber.Connect(Guid.NewGuid().ToString());
            if (!subscriber.IsConnected)
            {
                Console.WriteLine("Error connecting to message broker...");
                return;
            }
                subscriber.MqttMsgPublishReceived += client_MqttMsgPublishReceived;


            subscriber.Subscribe(topics, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }


        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            MessageBox.Show("Publish in endpoint, channel " + e.Topic + "; data = " + Encoding.UTF8.GetString(e.Message) + " related with a creation event");
        }

        void client_MqttMsgPublishReceivedDeletion(object sender, MqttMsgPublishEventArgs e)
        {
            MessageBox.Show("Publish in endpoint, channel " + e.Topic + "; data = " + Encoding.UTF8.GetString(e.Message) + " related with a deletion event");
        }

        #endregion

        #region Publish to a channel method

        private void Publish(string modName, string content, string endpoint)
        {
            publisher = new MqttClient(IPAddress.Parse(endpoint));
            publisher.Connect(Guid.NewGuid().ToString());
            if (publisher.IsConnected)
            {
                publisher.Publish(modName, Encoding.UTF8.GetBytes(content));
            }
        }
        #endregion
    }
}
