using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Rann.Components
{
    public class TCalculation
    {
        #region Public Fields

        [XmlIgnore]
        public List<TComponent> Components = new List<TComponent>();

        #endregion Public Fields

        #region Private Fields

        private static readonly Thread AutoSaveThread;
        private static readonly DirectoryInfo MainDirectory;
        private static readonly XmlSerializer XS = new XmlSerializer(typeof(TCalculation));
        private static TCalculation current;

        #endregion Private Fields

        #region Public Constructors

        static TCalculation()
        {
            //

            MainDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Maths");
            if (!MainDirectory.Exists) MainDirectory.Create();

            FileInfo[] fis = MainDirectory.GetFiles("*.xml");
            Array.Sort(fis, FileInfoComparer.Default);

            if (fis.Length > 0)
                Load(fis[0]);
            else
                Current = new TCalculation();

            List<string> cmds = new List<string>(Current.Commands);
            Current.Commands.Clear();
            foreach (string s in cmds)
                TCommand.DoCommand(s, false);

            AutoSaveThread = new Thread(AutoSave); AutoSaveThread.Start();
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void CurrentChanged_EventHandler();

        #endregion Public Delegates

        #region Public Events

        public static event CurrentChanged_EventHandler CurrentChanged;

        #endregion Public Events

        #region Public Properties

        public static TCalculation Current { get { return current; } set { current = value; CurrentChanged?.Invoke(); } }

        public List<string> Commands { get; set; } = new List<string>();

        [XmlIgnore]
        public List<TFormula> Formulas { get; set; } = new List<TFormula>();

        [XmlIgnore]
        public List<TMatrix4> Matrixes { get; set; } = new List<TMatrix4>();

        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        public List<TParameterComponent> Parameters { get; set; } = new List<TParameterComponent>();

        [XmlIgnore]
        public List<TVector4> Vectors { get; set; } = new List<TVector4>();

        #endregion Public Properties

        #region Public Methods

        public static void Load(FileInfo file)
        {
            TParameterComponent.AllParameters.Clear();
            StreamReader rdr = new StreamReader(file.FullName);
            try
            {
                Current = (TCalculation)XS.Deserialize(rdr);
            }
            catch
            {
                Current = new TCalculation();
            }
            rdr.Dispose();
            lock (Current.Parameters)
                foreach (TParameterComponent p in TParameterComponent.AllParameters.Where(o => !Current.Parameters.Contains(o)))
                    Current.Parameters.Add(p);
            //
            lock (Current.Parameters)
                foreach (TParameterComponent p in TParameterComponent.AllParameters.Where(o => o.Checked && TParameterComponent.AllParameters.Count(oo => oo.Name == o.Name && oo.SubName == o.SubName) > 1).ToArray())
                {
                    TParameterComponent[] prmtrs = TParameterComponent.AllParameters.Where(o => o.Name == p.Name && o.SubName == p.SubName).ToArray();
                    for (int i = 0; i < prmtrs.Length; i++)
                    {
                        if (prmtrs[i].GetHashCode() != p.GetHashCode())
                        {
                            foreach (TFormula f in Current.Formulas) f.Replace(prmtrs[i], p);
                            foreach (TParameterComponent pp in Current.Parameters) pp.Replace(prmtrs[i], p);
                            TParameterComponent.AllParameters.Remove(prmtrs[i]);
                            Current.Parameters.Remove(prmtrs[i]);
                        }
                    }
                }
        }

        public void Save()
        {
            Save(new FileInfo(MainDirectory.FullName + "\\" + Name + ".xml"));
        }

        #endregion Public Methods

        #region Private Methods

        private static void AutoSave()
        {
            while (true)
            {
                //lock (Current.Parameters)
                //    foreach (TParameterComponent p in Current.Parameters.Where(o => o.RFormula == null).ToArray())
                //    {
                //        int n = 0;
                //        foreach (TParameterComponent pp in Current.Parameters.ToArray())
                //            if (Array.IndexOf(pp.GetParameters(), p) >= 0) n++;

                //        foreach (TFormula ff in Current.Formulas)
                //            if (Array.IndexOf(ff.GetParameters(), p) >= 0) n++;

                //        foreach (TVector4 ff in Current.Vectors)
                //            if (Array.IndexOf(ff.GetParameters(), p) >= 0) n++;

                //        switch (n)
                //        {
                //            case 0:
                //                break;

                //            case 1:
                //                Current.Parameters.Remove(p);
                //                TParameterComponent.AllParameters.Remove(p);
                //                break;

                //            default:
                //                break;
                //        }
                //    }
                Thread.Sleep(5000);
                Current.Save(new FileInfo(MainDirectory.FullName + "\\Autosaved.xml"));
            }
        }

        private void Save(FileInfo file)
        {
            foreach (TParameterComponent p in Parameters) p.Checked = true;
            StreamWriter wtr = new StreamWriter(file.FullName);
            XS.Serialize(wtr, this);
            wtr.Dispose();
        }

        #endregion Private Methods

        #region Private Classes

        private class FileInfoComparer : IComparer<FileInfo>
        {
            #region Public Fields

            public static readonly FileInfoComparer Default = new FileInfoComparer();

            #endregion Public Fields

            #region Public Methods

            public int Compare(FileInfo x, FileInfo y)
            {
                return y.LastWriteTime.CompareTo(x.LastWriteTime);
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}