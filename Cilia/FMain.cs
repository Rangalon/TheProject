using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace Cilia
{
    public partial class FMain : Form
    {
     
        public FMain()
        {
            InitializeComponent();
            Assembly ass = Assembly.GetExecutingAssembly();
            AssemblyName nm = ass.GetName();
            Text = "Cilia - " + System.String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", nm.Version.Major, nm.Version.Minor, nm.Version.Build, nm.Version.MinorRevision);
            if (System.Diagnostics.Debugger.IsAttached) Text += " - Debug";
        }


    }
}