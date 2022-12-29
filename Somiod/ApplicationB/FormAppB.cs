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
            Data data = new Data
            {
                Res_type = "data",
                Content = "xml(on)"
            };
            RestRequest request = new RestRequest("/api/somiod/lighting/light_bulb", Method.Post);
            request.AddBody(data);

            var response = client.Execute(request);
            //MessageBox.Show(response.StatusCode + ", " + response.ResponseStatus);
    }

        private void button_lightOff_Click(object sender, EventArgs e)
        {
            Data data = new Data
            {
                Res_type = "data",
                Content = "xml(off)"
            };
            RestRequest request = new RestRequest("/api/somiod/lighting/light_bulb", Method.Post);
            request.AddBody(data);

            var response = client.Execute(request);
            //MessageBox.Show(response.StatusCode + ", " + response.ResponseStatus);
        }

        private void FormAppB_Load(object sender, EventArgs e)
        {
            Module module = new Module
            {
                Res_type = "module",
                Name = "light_command"
            };
            RestRequest requestMod = new RestRequest("/api/somiod/lighting", Method.Post);
            requestMod.AddBody(module);

            var responseMod = client.Execute(requestMod);
            MessageBox.Show(responseMod.StatusCode + ", light_command module created! ");
        }
    }
}
