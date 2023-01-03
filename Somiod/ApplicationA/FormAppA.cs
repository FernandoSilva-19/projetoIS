using RestSharp;
using Somiod.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationA
{
    public partial class FormAppA : Form
    {
        string baseURI = @"http://localhost:49356/"; //TODO: needs to be updated!
        RestClient client = null;
        public FormAppA()
        {
            InitializeComponent();
            client = new RestClient(baseURI);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var request = new RestRequest("/api/somiod", Method.Post);
            string rawXml = "<root><Name>lighting</Name><Res_type>application</Res_type></root>";
            request.AddParameter("application/xml", rawXml, ParameterType.RequestBody);
            var response = client.Execute(request);
            MessageBox.Show(response.StatusCode + ", lighting application created! ");

            var requestMod = new RestRequest("/api/somiod/lighting", Method.Post);
            string rawXmlMod = "<root><Name>light_bulb</Name><Res_type>Module</Res_type></root>";
            requestMod.AddParameter("application/xml", rawXmlMod, ParameterType.RequestBody);
            var responseMod = client.Execute(requestMod);
            MessageBox.Show(responseMod.StatusCode + ", light_bulb module created! ");

            var requestSub = new RestRequest("/api/somiod/lighting/light_bulb", Method.Post);
            string rawXmlSub = "<root><Name>sub1</Name><Res_type>subscription</Res_type><Event>creation</Event><Endpoint>127.0.0.1</Endpoint></root>";
            requestSub.AddParameter("application/xml", rawXmlSub, ParameterType.RequestBody);
            var responseSub = client.Execute(requestSub);
            MessageBox.Show(responseSub.StatusCode + ", sub1 module created! ");

            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RestRequest requestLastData = new RestRequest("/api/somiod/lighting/light_bulb/latestdata", Method.Get);

            var responseData = client.Execute(requestLastData);
            if(responseData != null) {
            if (responseData.Content.Contains("xml(on)"))
            {
                pictureBox1.Image = Properties.Resources.light_on;
            }
            else if (responseData.Content.Contains("xml(off)"))
            {
                pictureBox1.Image = Properties.Resources.light_off;
            }
            }
        }



    }
}
