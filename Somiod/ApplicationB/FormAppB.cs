using RestSharp;
using Somiod.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationB
{
    public partial class FormAppB : Form
    {
        string baseURI = @"http://localhost:49356/"; //TODO: needs to be updated!
        RestClient client = null;

        public FormAppB()
        {
            InitializeComponent();
            client = new RestClient(baseURI);
        }

        private void button_lightOn_Click(object sender, EventArgs e)
        {

            var request = new RestRequest("/api/somiod/lighting/light_bulb", Method.Post);
            string rawXml = "<root><Res_type>Data</Res_type><Content>xml(on)</Content></root>";
            request.AddParameter("application/xml", rawXml, ParameterType.RequestBody);
            var response = client.Execute(request);
           
    }

        private void button_lightOff_Click(object sender, EventArgs e)
        {
            var request = new RestRequest("/api/somiod/lighting/light_bulb", Method.Post);
            string rawXml = "<root><Res_type>Data</Res_type><Content>xml(off)</Content></root>";
            request.AddParameter("application/xml", rawXml, ParameterType.RequestBody);
            var response = client.Execute(request);
        }

        private void FormAppB_Load(object sender, EventArgs e)
        {
            bool exists = false;

            while (exists == false)
            {
            RestRequest requestGetApp = new RestRequest("/api/somiod/", Method.Get);
                var responseGetApp = client.Execute(requestGetApp);

                if (responseGetApp.Content.Contains("lighting"))
                {
                    exists = true;
                }
            }

            var requestMod = new RestRequest("/api/somiod/lighting", Method.Post);
            string rawXmlMod = "<root><Name>light_command</Name><Res_type>Module</Res_type></root>";
            requestMod.AddParameter("application/xml", rawXmlMod, ParameterType.RequestBody);
            var responseMod = client.Execute(requestMod);
            MessageBox.Show(responseMod.StatusCode + ", light_command module created! ");
            
        }
    }
}
