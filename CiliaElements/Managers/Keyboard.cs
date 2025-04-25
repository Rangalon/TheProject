
using Math3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using static CiliaElements.TSolidElement;

namespace CiliaElements
{
    public static partial class TManager
    {
        #region Private Methods

        private static void AddPendingKey(OpenTK.Input.Key key, EKeybordModifiers modifiers)
        {
            if (!modifiers.HasFlag(EKeybordModifiers.Ctrl))
            {
                int ik = (int)key;
                switch (ik)
                {
                    case int n when (n > 9 && n < 45): KeyPressedEvent?.Invoke(key, modifiers); break;
                    case 53: if (pendingKeys.Length > 0) { pendingKeys = pendingKeys.Substring(0, pendingKeys.Length - 1); }; break;
                    case 77: case 128: pendingKeys += "/"; break;
                    case 78: pendingKeys += "*"; break;
                    case 79: case 120: pendingKeys += "-"; break;
                    case 80: case 121: pendingKeys += "+"; break;
                    case 81: pendingKeys += "."; break;
                    case 51: pendingKeys += " "; ; break;
                    case int n when (n > 66 && n < 77): pendingKeys += (char)(ik - 19); break;
                    case int n when (n > 108 && n < 119): pendingKeys += (char)(ik - 61); break;
                    case int n when (n > 82 && n < 109):
                        if (modifiers.HasFlag(EKeybordModifiers.Shift))
                        {
                            pendingKeys += (char)(ik - 18);
                        }
                        else
                        {
                            pendingKeys += (char)(ik + 14);
                        }
                        break;

                    case 49:
                    case 82:
                        TCommand cmd = new TCommand() { Command = pendingKeys, State = ECommandState.Success };
                        cmd.Words = pendingKeys.Split(' ');
                        Stack<string> wds = new Stack<string>();
                        Array.Reverse(cmd.Words);
                        foreach (string w in cmd.Words)
                        {
                            wds.Push(w);
                        }

                        switch (wds.Pop())
                        {
                            case "vp":
                            case "viewpoint":
                                switch (wds.Pop())
                                {
                                    case "origin": SetViewPoint(new Vec3(0, 0, 0), new Vec3(1, 0, 0), new Vec3(0, 0, 1), new Vec3(10, 0, 0)); break;
                                    case "top": FitTop(); break;
                                    case "bottom": FitBottom(); break;
                                    case "left": FitLeft(); break;
                                    case "right": FitRight(); break;
                                    case "front": FitFront(); break;
                                    case "rear": FitRear(); break;
                                    case "all": FitAll(); break;
                                    case "sel":
                                    case "selected": FitSelected(); break;
                                    default: cmd.State = ECommandState.Miss; break;
                                }
                                break;

                            case "open":
                                switch (wds.Pop())
                                {
                                    case null: OpenFiles(); break;
                                    default:
                                        try
                                        {
                                            FileInfo fi = new FileInfo(cmd.Words[1]);
                                            if (fi.Exists)
                                            {
                                                LoadFile(fi);
                                            }
                                        }
                                        catch
                                        {
                                            cmd.State = ECommandState.Miss;
                                        }
                                        break;
                                }
                                break;

                            case "find":
                                Regex pn = null;
                                Regex nn = null;
                                while (wds.Count > 0)
                                {
                                    string w = wds.Pop();
                                    switch (w)
                                    {
                                        case "-pn": case "-partnumber": try { pn = new Regex(wds.Pop()); } catch { } break;
                                        case "-nn": case "-nodename": try { nn = new Regex(wds.Pop()); } catch { } break;
                                    }
                                }
                                ClearSelection();
                                foreach (TLink l in View.OwnerLink.FindAll(pn, nn))
                                {
                                    AddSelectedLink(l);
                                }

                                FitSelected();
                                break;

                            case "pack":
                                PackSelected();
                                break;

                            case "extract":
                                if (wds.Count > 0 && wds.Peek() == "full")
                                {
                                    ExtractSelected();
                                }
                                else
                                {
                                    ExtractSelected();
                                }

                                break;

                            default:
                                cmd.State = ECommandState.Miss;
                                DoKeys?.Invoke(cmd);
                                break;
                        }
                        Commands.Insert(0, cmd);
                        pendingKeys = "";
                        break;

                    default:
                        KeyPressedEvent?.Invoke(key, modifiers); break;
                }
                commandsPanel.Visible = pendingKeys.Length > 0;
                if (commandsPanel.Visible)
                {
                    string s = pendingKeys + '\n';
                    foreach (TCommand cmd in Commands)
                    {
                        s += cmd.Command + '\n';
                    }
                    commandsPanel.Message = s;
                    commandsPanel.ToRedraw = true;
                }
            }
            else
            {
                if (KeyPressedEvent != null)
                {
                    KeyPressed(key, modifiers);
                }
            }
        }

        static OpenTK.Input.KeyboardState kb;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckKeyBoardEntries()
        {



            kb = OpenTK.Input.Keyboard.GetState();
            //
            EKeybordModifiers modifiers = EKeybordModifiers.None;
            if (kb.IsKeyDown(OpenTK.Input.Key.ControlLeft) || kb.IsKeyDown(OpenTK.Input.Key.ControlRight))
            {
                Ctrl = true;
                modifiers |= EKeybordModifiers.Ctrl;
            }
            else
            {
                Ctrl = false;
            }
            //
            if (kb.IsKeyDown(OpenTK.Input.Key.ShiftLeft) || kb.IsKeyDown(OpenTK.Input.Key.ShiftRight))
            {
                modifiers |= EKeybordModifiers.Shift;
            }
            //
            KeyboardModifiers = modifiers;
            //

            foreach (OpenTK.Input.Key key in Keys)
            {
                int i = (int)key;
                if (kb.IsKeyDown(key))
                {
                    if (UsedKeys[i])
                    {
                        if (AddingKeys)
                        {
                            if (DateTime.Now - LastAddedKey > AddedKeyTimeSpan)
                            {
                                AddPendingKey(key, modifiers);
                                LastAddedKey = DateTime.Now;
                            }
                        }
                        else if (DateTime.Now - LastAddedKey > AddingKeyTimeSpan)
                        {
                            AddingKeys = true;
                        }
                    }
                    else
                    {
                        UsedKeys[i] = true;
                        LastAddedKey = DateTime.Now;
                    }
                }
                else if (UsedKeys[i])
                {
                    if (AddingKeys)
                    {
                        AddingKeys = false;
                    }
                    else
                    {
                        UsedKeys[i] = false;
                        AddPendingKey(key, modifiers);
                    }
                }
            }

        }

        private static void ExtractSelected()
        {
            TSolidElement solid = OverFlownLink.Solid;
            TTriangle t = new TTriangle
            {
                I1 = solid.DataIndexes[OverFlownIndex],
                I2 = solid.DataIndexes[OverFlownIndex + 1],
                I3 = solid.DataIndexes[OverFlownIndex + 2]
            };
            Vec3 v1 = solid.DataPositions[t.I1];
            Vec3 v2 = solid.DataPositions[t.I2];
            Vec3 v3 = solid.DataPositions[t.I3];
            t.N = Vec3.Cross(v2 - v1, v3 - v1);
            t.N.Normalize();
        }

        #endregion Private Methods
    }
}