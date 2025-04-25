using ICSharpCode.SharpZipLib.Zip;
using System;

namespace CiliaElements
{
    public class TFile
    {
        #region Public Fields

        public TBaseElement Element;

        #endregion Public Fields

        #region Private Fields

        private readonly System.IO.FileInfo _Fi;

        private readonly System.IO.Stream _Stream;

        #endregion Private Fields

        #region Public Constructors

        public TFile(string name, byte[] bts,TManager.ELoadStuff ls)
        {
            //
            Element = new Format3DXml.T3DXmlElement(name, bts,ls);
            _Fi = Element.Fi;
            TManager.UsedFiles.Push(this);
            this.Start();
            //
        }

        public TFile(System.IO.FileInfo iFi, System.IO.Stream iStream)
        {
            TManager.UsedFiles.Push(this);
            //
            //If iFi.ToString.StartsWith(".\") Then
            //_Fi = New   System.IO.FileInfo(Parent.Fi.Directory.FullName + iFi.ToString.Substring(1))
            //Else
            _Fi = iFi;
            //End If
            _Stream = iStream;
            //
            switch (_Fi.Extension.ToUpperInvariant())
            {
                case ".CILIASZ":
                    Element = new FormatCilia.TCiliaSolid
                    {
                        PartNumber = _Fi.Name.Split('.')[0],
                        Fi = _Fi
                    };
                    break;

                case ".CILIAAZ":
                    Element = new FormatCilia.TCiliaAssembly
                    {
                        PartNumber = _Fi.Name.Split('.')[0],
                        Fi = _Fi
                    };
                    break;

                //case ".DAE":
                //    Element = new FormatDAE.TDaeElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                case ".3DXML":
                    Element = new Format3DXml.T3DXmlElement
                    {
                        PartNumber = _Fi.Name.Split('.')[0],
                        Fi = _Fi
                    };
                    break;

                case ".JSON":
                    Element = new FormatJson.TJsonElement
                    {
                        PartNumber = _Fi.Name.Split('.')[0],
                        Fi = _Fi
                    };
                    break;

                //case ".JT":
                //    Element = new FormatJT.TJTAssembly
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;
                //case ".FBX":
                //    Element = new Format_FBX.TFBXElement();
                //    Element.PartNumber = _Fi.Name.Split(".")(0);
                //    Element.Fi = _Fi;
                //    break;
                //case ".WRL":
                //    Element = new FormatWRL.TWrlElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                //case ".3DS":
                //    Element = new Format3DS.T3dsElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;
                //case ".max":
                //    Element = new Format_MAX.TMaxElement();
                //    Element.PartNumber = _Fi.Name.Split(".")(0);
                //    Element.Fi = _Fi;
                //    break;
                case ".ZIP":
                    ZipFile z = new ZipFile(_Fi.FullName);
                    for (int i = 0; i < z.Count; i++)
                    {
                        ZipEntry ze = z[i];
                        //if (ze.Name.ToUpperInvariant().EndsWith(".OBJ", StringComparison.Ordinal))
                        //{
                        //    _Fi = new System.IO.FileInfo(ze.Name);
                        //    Element = new FormatOBJ.TObjElement() { zf = z };
                        //    Element.PartNumber = _Fi.Name.Split('.')[0];
                        //    Element.Fi = _Fi;
                        //    break;
                        //}
                    }
                    if (Element == null)
                    {
                        z.Close();
                    }

                    break;

                //case ".OBJ":
                //    Element = new FormatOBJ.TObjElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                //case ".STL":
                //    Element = new FormatSTL.TStlElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                //case ".BLEND":
                //    Element = new FormatBLEND.TBlendElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                //case ".PLY":
                //    Element = new FormatPLY.TPlyElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                //case ".MDL":
                //    Element = new FormatMDL.TModel
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                //case ".IGS":
                //    Element = new FormatIGES.TIgsElement
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                case ".3DREP":
                    Element = new Format3DXml.T3DRepElement { Stream = _Stream };
                    Element.PartNumber = _Fi.Name.Split('.')[0];
                    Element.Fi = _Fi;
                    break;

                case ".INTERNALSOLID":
                    Element = new TSolidElement();
                    break;

                case ".ELEMENT":
                case ".INTERNAL":
                    Element = new TSolidElement();
                    break;

                //case ".XML":
                //    Element = new FormatXml.TMap
                //    {
                //        PartNumber = _Fi.Name.Split('.')[0],
                //        Fi = _Fi
                //    };
                //    break;

                default:
                    Element = new TSolidElement
                    {
                        PartNumber = _Fi.Name.Split('.')[0],
                        Fi = _Fi
                    };
                    break;
                    //throw new Exception("Unmanaged Extension!");
            }
            TManager.FilesToload.Push(this);
        }

        public TFile(TSolidElement iSolid)
        {
            TManager.UsedFiles.Push(this);
            Element = iSolid;
            _Fi = iSolid.Fi;
            TManager.FilesToload.Push(this);
        }

        #endregion Public Constructors

        #region Public Methods

        public void Start()
        {
            switch (_Fi.Extension.ToUpperInvariant())
            {
                //case ".CILIAS":
                //case ".CILIASZ":
                //    if (_Fi.Exists)
                //    {
                //        // ------------------------------------------------------------------------
                //    }
                //if (_Fi.Exists)
                //{
                //    System.DateTime dt = DateTime.Now;
                //    XmlSerializer XS = new XmlSerializer(typeof(TSolidElement));
                //    System.IO.Stream St = null;
                //    if (_Fi.Extension == ".ciliaS")
                //    {
                //        St = _Fi.OpenRead();
                //    }
                //    else if (_Fi.Extension == ".ciliaSz")
                //    {
                //        System.IO.FileStream iFile = _Fi.OpenRead();
                //        System.IO.Compression.GZipStream cp = new System.IO.Compression.GZipStream(iFile, System.IO.Compression.CompressionMode.Decompress);
                //        St = new System.IO.MemoryStream();
                //        cp.CopyTo(St);
                //        St.Position = 0;
                //        //
                //        cp.Close();
                //        cp.Dispose();
                //        iFile.Close();
                //        iFile.Dispose();
                //    }
                //    //
                //    System.Xml.XmlTextReader rdr = new System.Xml.XmlTextReader(St);
                //    Element = (TBaseElement)XS.Deserialize(rdr);
                //    rdr.Close();
                //    St.Close();
                //    St.Dispose();
                //    //
                //    Element.Fi = _Fi;
                //    Element.Publish();
                //}
                //else
                //{
                //    Element = new TSolidElement();
                //}
                //break;

                case ".INTERNALSOLID":
                    Element.ElementLoader.Publish();
                    break;

                default:
                    Element.LaunchLoad();
                    break;
                    //throw new Exception("Unplanned extension!");
            }

            lock (TManager.LoadingCounterLocker)
            {
                TManager.LoadingCounter--;
            }
        }

        public void StartAsThread()
        {
            lock (TManager.LoadingCounterLocker)
            {
                TManager.LoadingCounter++;
            }

            System.Threading.Thread th = new System.Threading.Thread(Start)
            {
                Priority = System.Threading.ThreadPriority.Normal,
                IsBackground = true
            };
            th.Start();
        }

        #endregion Public Methods
    }
}