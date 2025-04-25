using CiliaElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Cilia
{
    public partial class FSplash : Form
    {
        #region Private Fields

        private static FSplash f;

        #endregion Private Fields

        #region Public Constructors

        public FSplash()
        {
            InitializeComponent();
            Assembly ass = Assembly.GetExecutingAssembly();
            AssemblyName nm = ass.GetName();
            List<Assembly> Assemblies = new List<Assembly>();
            GetAssemblies(ass, Assemblies);
            Assemblies.Add(ass);
            Assemblies.Sort(SortByFullName);
            Int32 Cpt = 0;
            bool bSide = true;
            foreach (Assembly assy in Assemblies)
            {
                nm = assy.GetName();
                Icon ic = Icon.ExtractAssociatedIcon(assy.Location);
                PictureBox p = new PictureBox() { Image = ic.ToBitmap(), Visible = true };
                Controls.Add(p);
                p.Width = 32;
                p.Height = 32;
                p.SizeMode = PictureBoxSizeMode.Zoom;
                Label l = new Label();
                Controls.Add(l);
                l.AutoSize = true;
                l.Text = "® " + nm.Name + "\n" + System.String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", nm.Version.Major, nm.Version.Minor, nm.Version.Build, nm.Version.MinorRevision) + "\n";

                l.Font = label2.Font;
                l.ForeColor = label2.ForeColor;
                //
                p.Top = Cpt;
                l.Top = Cpt;
                if (bSide)
                {
                    p.Left = 0;
                    l.Left = 32;
                }
                else
                {
                    p.Left = Width - p.Width;
                    l.Left = Width - p.Width - l.Width;
                    Cpt += 32;
                }
                bSide = !bSide;
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public static void HideSplash()
        {
            f.Dispose();
        }

        public static void ShowSplash()
        {
            Thread th = new Thread(ShowSplashThread); th.Start();
        }

        #endregion Public Methods

        #region Private Methods

        private static void ShowSplashThread()
        {
            f = new FSplash
            {
                TopLevel = true,
                TopMost = true
            };
            f.Show();
            f.label1.Text = "Starting";
            while (!TManager.SetupDone)
            {
                Thread.Sleep(10);
                if (!f.label1.Text.StartsWith(TManager.LogLine))
                {
                    f.label1.Text = TManager.LogLine + "\n" + f.label1.Text;
                }

                f.Refresh();
            }
            f.Dispose();
        }

        private static Int32 SortByFullName(Assembly iAss1, Assembly iAss2)
        {
            return iAss1.FullName.Split(',')[0].CompareTo(iAss2.FullName.Split(',')[0]);
        }

        private void GetAssemblies(Assembly iAss, List<Assembly> Assemblies)
        {
            foreach (AssemblyName assName in iAss.GetReferencedAssemblies())
            {
                if (!assName.FullName.StartsWith("System") && !assName.FullName.StartsWith("ms"))
                {
                    Assembly ass = Assembly.ReflectionOnlyLoad(assName.FullName);
                    if (!Assemblies.Contains(ass))
                    {
                        Assemblies.Add(ass);
                        GetAssemblies(ass, Assemblies);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}