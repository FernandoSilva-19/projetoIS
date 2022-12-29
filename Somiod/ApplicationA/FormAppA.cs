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
            Application2 app = new Application2
            {
                Res_type = "application",
                Name = "lighting"
            };
            RestRequest request = new RestRequest("/api/somiod", Method.Post);
            request.AddBody(app);

            var response = client.Execute(request);
            MessageBox.Show(response.StatusCode + ", lighting application created! ");

            Module module = new Module
            {
                Res_type = "module",
                Name = "light_bulb"
            };
            RestRequest requestMod = new RestRequest("/api/somiod/lighting", Method.Post);
            requestMod.AddBody(module);

            var responseMod = client.Execute(requestMod);
            MessageBox.Show(responseMod.StatusCode + ", light_bulb module created! ");

            DataSub subscription = new DataSub
            {
                Res_type = "subscription",
                Name = "sub1",
                Event = "creation",
                Endpoint = "127.0.0.1"
            };
            RestRequest requestSub = new RestRequest("/api/somiod/lighting/light_bulb", Method.Post);
            requestSub.AddBody(subscription);

            var responseSub = client.Execute(requestSub);
            MessageBox.Show(responseSub.StatusCode + ", sub1 subscription created! ");
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RestRequest requestLastData = new RestRequest("/api/somiod/lighting/light_bulb/latestdata", Method.Get);

            var responseData = client.Execute<Data>(requestLastData).Data;
            if(responseData != null) {
            if (responseData.Content == "xml(on)")
            {
                pictureBox1.Image = Properties.Resources.light_on;
            }
            else if (responseData.Content == "xml(off)")
            {
                pictureBox1.Image = Properties.Resources.light_off;
            }
            }
        }
    }
}
