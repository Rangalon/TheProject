using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rann.Components
{
    public class TCommand
    {
        #region Public Fields

        public static TCommand RootCommand = new TCommand("Root", null, null);
        public DoerTemplate Doer;
        public TCommand[] Kids = { };
        public string Name;
        public TCommand Parent;

        #endregion Public Fields

        #region Private Fields

        private static readonly char[] CommandSeparators = new char[] { ' ' };

        #endregion Private Fields

        #region Public Constructors

        static TCommand()
        {
            TCommand cmd1;
            TCommand cmd2;
            //TCommand cmd3;
            //
            cmd1 = new TCommand("Create", RootCommand, null);
            cmd2 = new TCommand("PPoint", cmd1, CreatePPoint);
            cmd2 = new TCommand("PVector", cmd1, CreatePVector);
            cmd2 = new TCommand("Matrix", cmd1, CreateMatrix);
            cmd2 = new TCommand("IMatrix", cmd1, CreateIMatrix);
            cmd2 = new TCommand("PMatrix", cmd1, CreatePMatrix);
            //
            cmd1 = new TCommand("Set", RootCommand, SetThing);
            //
            cmd1 = new TCommand("Remove", RootCommand, RemoveThing);
            //
            cmd1 = new TCommand("Push", RootCommand, null);
            cmd2 = new TCommand("Parameter", cmd1, PushParameter);
            cmd2 = new TCommand("Formula", cmd1, PushFormula);
            //
            cmd1 = new TCommand("Invert", RootCommand, null);
            cmd2 = new TCommand("Matrix", cmd1, InvertMatrix);
        }

        public TCommand(string iName, TCommand iParent, DoerTemplate iDoer)
        {
            Name = iName;
            Doer = iDoer;
            Parent = iParent;
            if (Parent != null)
            {
                Array.Resize(ref Parent.Kids, Parent.Kids.Length + 1);
                Parent.Kids[Parent.Kids.Length - 1] = this;
            }
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate bool DoerTemplate(Stack<string> stc, List<string> returns, bool forcheck);

        #endregion Public Delegates

        #region Public Methods

        public static string DoCommand(string iCommand, bool forCheck)
        {
            string[] wds = iCommand.Split(CommandSeparators, StringSplitOptions.RemoveEmptyEntries);
            Stack<string> stc = new Stack<string>(wds.Reverse());

            List<string> returns = new List<string>();
            if (CheckCommand(RootCommand, stc, returns, forCheck))
            {
                string s = "";
                foreach (string w in returns)
                    s += " " + w;
                s = s.Trim();
                if (forCheck) return s.Trim('\t');
                else { TCalculation.Current.Commands.Add(s); return ""; }
            }
            else
                return iCommand.Trim('\t');
        }

        #endregion Public Methods

        #region Private Methods

        private static bool CheckCommand(TCommand rootCommand, Stack<string> stc, List<string> returns, bool forCheck)
        {
            string wd = stc.Pop();
            TCommand[] cmds = rootCommand.Kids.Where(o => o.Name.ToLowerInvariant().StartsWith(wd.ToLowerInvariant())).ToArray();
            switch (cmds.Length)
            {
                case 0:
                    return false;

                case 1:
                    returns.Add(cmds[0].Name);
                    if (cmds[0].Doer != null) return cmds[0].Doer(stc, returns, forCheck);
                    else if (stc.Count > 0) return CheckCommand(cmds[0], stc, returns, forCheck);
                    else return forCheck;

                default:
                    return true;
            }
        }

        private static bool CreateIMatrix(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string nm = stc.Pop();
                returns.Add(nm);
                TMatrix4 mtx = new TMatrix4() { Name = nm };
                mtx.Xx = 1;
                mtx.Yy = 1;
                mtx.Zz = 1;
                mtx.Ow = 1;
                TCalculation.Current.Components.Add(mtx);
            }
            return true;
        }

        private static bool CreateMatrix(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string nm = stc.Pop();
                returns.Add(nm);
                TMatrix4 mtx = new TMatrix4() { Name = nm };
                TCalculation.Current.Components.Add(mtx);
            }
            return true;
        }

        private static bool CreatePMatrix(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string nm = stc.Pop();
                returns.Add(nm);
                TMatrix4 mtx = TMatrix4.CreateParametersMatrix(nm);
                TCalculation.Current.Components.Add(mtx);
            }
            return true;
        }

        private static bool CreatePPoint(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string nm = stc.Pop();
                returns.Add(nm);
                TCalculation.Current.Components.Add(TVector4.CreateParametersPoint(nm));
            }
            return true;
        }

        private static bool CreatePVector(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string nm = stc.Pop();
                returns.Add(nm);
                TCalculation.Current.Components.Add(TVector4.CreateParametersVector(nm));
            }
            return true;
        }

        private static bool InvertMatrix(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string wd = stc.Pop();
                returns.Add(wd);
                TMatrix4 m = TCalculation.Current.Matrixes.FirstOrDefault(o => o.Name == wd);
                TMatrix4 mi = TMatrix4.Invert(m);
                mi.Name = m.Name + "-1";
                TCalculation.Current.Components.Add(mi);
            }
            return true;
        }

        private static bool PushFormula(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string wd = stc.Pop();
                returns.Add(wd);
                TParameterComponent p = TComponent.GetParameter(wd);
                if (p.RFormula != null) p.PushFormula();
            }
            return true;
        }

        private static bool PushParameter(Stack<string> stc, List<string> returns, bool forcheck)
        {
            throw new NotImplementedException();
        }

        private static bool RemoveThing(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string wd = stc.Pop();
                returns.Add(wd);
                TParameterComponent p = TComponent.GetParameter(wd);
                p.RFormula = 0;
                p.PushFormula();
            }
            return true;
        }

        private static bool SetThing(Stack<string> stc, List<string> returns, bool forcheck)
        {
            if (stc.Count == 0) return forcheck;
            while (stc.Count > 0)
            {
                string wd = stc.Pop();
                string[] wds = wd.Split('=');
                if (wds.Length == 2)
                {
                    returns.Add(wd);
                    TParameterComponent p = TComponent.GetParameter(wds[0]);
                    p.RFormula = TComponent.ParseFormula(wds[1]);
                }
            }
            return true;
        }

        #endregion Private Methods
    }
}