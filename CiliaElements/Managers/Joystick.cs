
using Math3D;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CiliaElements
{
    public static partial class TManager
    {
        #region Public Methods

        public static void DoMoveAxis0(TimeSpan dt, double f)
        {
            if (f < 0.01 && f > -0.01)
            {
                return;
            }

            f *= f * f * TranslationSpeed * dt.TotalMilliseconds;
            JoystickTempMatrix = Mtx4.CreateTranslation(JoystickTempMatrix.Row0.X * f, JoystickTempMatrix.Row1.X * f, JoystickTempMatrix.Row2.X * f) * JoystickTempMatrix;
            JoystickTempCheck = true;
        }

        public static void DoMoveAxis1(TimeSpan dt, double f)
        {
            if (f < 0.01 && f > -0.01)
            {
                return;
            }

            f *= f * f * TranslationSpeed * dt.TotalMilliseconds;
            JoystickTempMatrix = Mtx4.CreateTranslation(-JoystickTempMatrix.Row0.Z * f, -JoystickTempMatrix.Row1.Z * f, -JoystickTempMatrix.Row2.Z * f) * JoystickTempMatrix;
            JoystickTempCheck = true;
        }

        public static void DoMoveAxis2(TimeSpan dt, double f)
        {
            if (f < 0.01 && f > -0.01)
            {
                return;
            }

            f *= f * f * RotationSpeed * dt.TotalMilliseconds;
            JoystickTempMatrix *= Mtx4.CreateRotationY(f);
            JoystickTempCheck = true;
        }

        public static void DoMoveAxis3(TimeSpan dt, double f)
        {
            if (f < 0.01 && f > -0.01)
            {
                return;
            }

            f *= f * f * RotationSpeed * dt.TotalMilliseconds;
            JoystickTempMatrix *= Mtx4.CreateRotationX(f);
            JoystickTempCheck = true;
        }

        #endregion Public Methods

        #region Private Methods


        static TimeSpan dtKeyboard;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckJoystickEntries()
        {


          
            JoystickCheck = DateTime.Now;

            dtKeyboard = JoystickCheck - LastJoystickCheck;
            Joystick = OpenTK.Input.Joystick.GetState(0);
            JoystickTempMatrix = VMatrix;


            MoveAxis0(dtKeyboard, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis0));
            MoveAxis1(dtKeyboard, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis1));
            MoveAxis2(dtKeyboard, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis2));
            MoveAxis3(dtKeyboard, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis3));
            //MoveAxis2(dt, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis4));
            //MoveAxis3(dt, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis5));
            //MoveAxis2(dt, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis6));
            //MoveAxis3(dt, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis7));
            //MoveAxis2(dt, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis8));
            //MoveAxis3(dt, Joystick.GetAxis(OpenTK.Input.JoystickAxis.Axis9));
            ClickButton0(Joystick.GetButton(OpenTK.Input.JoystickButton.Button0), dtKeyboard.TotalMilliseconds);
            ClickButton1(Joystick.GetButton(OpenTK.Input.JoystickButton.Button1), dtKeyboard.TotalMilliseconds);
            ClickButton2(Joystick.GetButton(OpenTK.Input.JoystickButton.Button2), dtKeyboard.TotalMilliseconds);
            ClickButton3(Joystick.GetButton(OpenTK.Input.JoystickButton.Button3), dtKeyboard.TotalMilliseconds);
            ClickButton4(Joystick.GetButton(OpenTK.Input.JoystickButton.Button4), dtKeyboard.TotalMilliseconds);
            ClickButton5(Joystick.GetButton(OpenTK.Input.JoystickButton.Button5), dtKeyboard.TotalMilliseconds);
            ClickButton6(Joystick.GetButton(OpenTK.Input.JoystickButton.Button6), dtKeyboard.TotalMilliseconds);
            ClickButton7(Joystick.GetButton(OpenTK.Input.JoystickButton.Button7), dtKeyboard.TotalMilliseconds);
            ClickButton8(Joystick.GetButton(OpenTK.Input.JoystickButton.Button8), dtKeyboard.TotalMilliseconds);
            ClickButton9(Joystick.GetButton(OpenTK.Input.JoystickButton.Button9), dtKeyboard.TotalMilliseconds);
            ClickButton10(Joystick.GetButton(OpenTK.Input.JoystickButton.Button10), dtKeyboard.TotalMilliseconds);

            //if (Joystick.GetButton(OpenTK.Input.JoystickButton.Button7) == OpenTK.Input.ButtonState.Pressed)
            //{
            //    f = 0.5;
            //}
            //else if (Joystick.GetButton(OpenTK.Input.JoystickButton.Button6) == OpenTK.Input.ButtonState.Pressed)
            //{
            //    f = -0.5;
            //}
            //else
            //{
            //    f = 0;
            //}

            //if (f > 0.1 || f < -0.1)
            //{
            //    JoystickTempCheck = true;
            //    f *= TranslationSpeed * dt.TotalMilliseconds;
            //    JoystickTempMatrix = Mtx4.CreateTranslation(JoystickTempMatrix.Row0.Y * f, JoystickTempMatrix.Row1.Y * f, JoystickTempMatrix.Row2.Y * f) * JoystickTempMatrix;
            //} //
            //if (Joystick.GetButton(OpenTK.Input.JoystickButton.Button5) == OpenTK.Input.ButtonState.Pressed)
            //{
            //    f = 0.5;
            //}
            //else if (Joystick.GetButton(OpenTK.Input.JoystickButton.Button4) == OpenTK.Input.ButtonState.Pressed)
            //{
            //    f = -0.5;
            //}
            //else
            //{
            //    f = 0;
            //}

            //if (f > 0.1 || f < -0.1)
            //{
            //    JoystickTempCheck = true;
            //    f *= RotationSpeed * dt.TotalMilliseconds;
            //    JoystickTempMatrix = JoystickTempMatrix * Mtx4.CreateRotationZ(f);
            //}
            if (JoystickTempCheck) { PendingViewPoint = JoystickTempMatrix; JoystickTempCheck = false; }
            PreviousJoystick = Joystick;

            LastJoystickCheck = JoystickCheck;

        }

        #endregion Private Methods
    }
}