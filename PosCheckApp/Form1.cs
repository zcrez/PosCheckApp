using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System.Net.NetworkInformation;
using GMap.NET.MapProviders;
using System.IO;


namespace PosCheckApp
{
  
    public partial class MainMap : Form
    {
        readonly GMapOverlay top = new GMapOverlay();
        internal readonly GMapOverlay objects = new GMapOverlay("objects");
        internal readonly GMapOverlay routes = new GMapOverlay("routes");
        internal readonly GMapOverlay polygons = new GMapOverlay("polygons");

        List<PosPT> m_ppts = new List<PosPT>();
        public MainMap()
        {
            InitializeComponent();
            if (!GMapControl.IsDesignerHosted)
            {
                if (!PingNetwork("www.qq.com"))
                {
                    gMapControl1.Manager.Mode = AccessMode.CacheOnly;
                    MessageBox.Show("No internet connection available, going to CacheOnly mode.", "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // config map         
                GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly; 
                gMapControl1.MapProvider = GMapProviders.GoogleChinaSatelliteMap;
                gMapControl1.DragButton = MouseButtons.Left;
                gMapControl1.Position = new PointLatLng(26.6961334816182, 106.2985095977783);
                gMapControl1.MinZoom = 0;
                gMapControl1.MaxZoom = 24;
                gMapControl1.Zoom = 9;

                gMapControl1.MouseMove += new MouseEventHandler(MainMap_MouseMove);

                gMapControl1.Overlays.Add(routes);
                gMapControl1.Overlays.Add(polygons);
                gMapControl1.Overlays.Add(objects);
            }
        }

        public static bool PingNetwork(string hostNameOrAddress)
        {
            bool pingStatus = false;

            using (Ping p = new Ping())
            {
                byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                int timeout = 4444; // 4s

                try
                {
                    PingReply reply = p.Send(hostNameOrAddress, timeout, buffer);
                    pingStatus = (reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }

            return pingStatus;
        }

        // move current marker with left holding
        void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            localStatusLabel.Text = Position.Lat.ToString("F6") + "," + Position.Lng.ToString("F6");
            if (e.Button == MouseButtons.Left )
            {

                gMapControl1.Refresh(); // force instant invalidation
            }
        }

        private void PosImport_Click(object sender, EventArgs e)
        {
            string strFileName;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "POS文件|*.txt|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                 strFileName = ofd.FileName;
            }
            else
                return;

            StreamReader sr = new StreamReader(strFileName, Encoding.Default);
            String line;
            int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (i == 0) { i++; continue; }
                string[]  data=line.Split(',');  
                PosPT pt = new PosPT();
                pt.ID = data[0];
                pt.photoID = data[0];
                pt.lat = double.Parse(data[1]);
                pt.lng = double.Parse(data[2]);
                pt.alt = double.Parse(data[3]);
                pt.pitch = double.Parse(data[4]);
                pt.head = double.Parse(data[5]);
                pt.roll = double.Parse(data[6]);
                m_ppts.Add(pt);
                i++;
            }
            RefreshMap();
        }


        public void RefreshMap()
        {
            if (m_ppts.Count > 0)  //在地图上标示pos点位置
            {
                for (int i = 0; i < m_ppts.Count; i++)
                {
                    GMapMarker myCity = new GMarkerGoogle(new PointLatLng(m_ppts[i].lat, m_ppts[i].lng), GMarkerGoogleType.green_small);
                    myCity.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    myCity.ToolTipText = m_ppts[i].ID;
                    GMapToolTip tip = new GMapToolTip(myCity);
                    Font DefaultFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold, GraphicsUnit.Pixel);
                    tip.Font = DefaultFont;
                    myCity.ToolTip = tip;
                    objects.Markers.Add(myCity);
                }
            }
        }

    }

    
}
