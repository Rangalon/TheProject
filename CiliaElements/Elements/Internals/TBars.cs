using CiliaElements.Format3DXml;
using System.Collections.Generic;
using System.Linq;

namespace CiliaElements.Internals
{
    public class TBars : TAssemblyElement
    {
        #region Public Constructors

        public TBars()
        {
            PartNumber = "Bars";

            //TViewManager.LoadResource("ViewBar", Properties.Resources.ViewBar, this);

            //
            T3DXmlElement ViewBar = new T3DXmlElement("ViewBar", Properties.Resources.ViewBar);
            ViewBar.LaunchLoad();
            TManager.ViewBar = TManager.AttachElmt(ViewBar.OwnerLink, this.OwnerLink, null);
            TManager.ViewBar.NodeName = TManager.ViewBar.PartNumber;
            TManager.ViewBar.Expanded = true;
            //
            Stack<TLink> lnks = new Stack<TLink>();
            //
            foreach (TLink ll in TManager.ViewBar.Links.Values.Where(o => o != null))
            {
                lnks.Push(ll);
                TLink lll = TManager.ViewBar.Links.Values.FirstOrDefault(o => o != null && o.PartNumber == ll.PartNumber + "Icons");
                if (lll != null)
                {
                    lll.Visible = false;
                    TManager.BarsIcons.Add(ll, lll);
                }
            }
            while (lnks.Count > 0)
            {
                TLink l = lnks.Pop();
                switch (l.PartNumber)
                {
                    case "Open Files": l.ActionToDo = TManager.OpenFiles; break;
                    case "View Treee": l.ActionToDo = TManager.SwitchDrawTree; break;
                    case "Top View": l.ActionToDo = TManager.FitTop; break;
                    case "Bottom View": l.ActionToDo = TManager.FitBottom; break;
                    case "Left View": l.ActionToDo = TManager.FitLeft; break;
                    case "Right View": l.ActionToDo = TManager.FitRight; break;
                    case "Front View": l.ActionToDo = TManager.FitFront; break;
                    case "Rear View": l.ActionToDo = TManager.FitRear; break;
                    case "Fit Object": l.ActionToDo = TManager.FitSelected; break;
                    case "Fit All": l.ActionToDo = TManager.FitAll; break;
                    case "View Surfaces": l.ActionToDo = TManager.SwitchDrawFaces; break;
                    case "View Lines": l.ActionToDo = TManager.SwitchDrawLines; break;
                    case "View Points": l.ActionToDo = TManager.SwitchDrawPoints; break;
                    case "View Annotations": l.ActionToDo = TManager.SwitchDrawAnnotations; break;
                    case "View Measures": l.ActionToDo = TManager.SwitchDrawMeasures; break;
                    case "Fixed": l.ActionToDo = TManager.SwitchEntitiesMoving; break;
                    case "Rotate": l.ActionToDo = TManager.SwitchMoveMode; break;
                    default: foreach (TLink ll in l.Links.Values.Where(o => o != null)) lnks.Push(ll); break;
                }
            }
        }

        #endregion Public Constructors
    }
}