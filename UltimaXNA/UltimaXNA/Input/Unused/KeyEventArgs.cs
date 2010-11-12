﻿using System;

namespace UltimaXNA.Input.Events
{
    public class KeyEventArgs : EventArgs
    {
        private readonly WinKeys _keyCode;
        private readonly int _keyDataExtra;
        private readonly WinKeys _modifiers;

        public virtual bool Alt
        {
            get { return ((_modifiers & WinKeys.Alt) == WinKeys.Alt); }
        }

        public bool Control
        {
            get { return ((_modifiers & WinKeys.Control) == WinKeys.Control); }
        }

        public virtual bool Shift
        {
            get { return ((_modifiers & WinKeys.Shift) == WinKeys.Shift); }
        }

        public WinKeys KeyVirtual
        {
            get
            {
                if (!Enum.IsDefined(typeof(WinKeys), (int)_keyCode))
                {
                    return WinKeys.None;
                }

                return _keyCode;
            }
        }

        public WinKeys KeyCode
        {
            get { return _keyCode; }
        }

        public WinKeys Modifiers
        {
            get { return (_modifiers & ~WinKeys.KeyCode); }
        }

        /// <summary>
        /// The repeat count for the current message. The value is the number of times
        /// the keystroke is autorepeated as a result of the user holding down the key.
        /// If the keystroke is held long enough, multiple messages are sent. However,
        /// the repeat count is not cumulative.
        /// The repeat count is always 1 for a WM_KEYUP message.
        /// </summary>
        public int Data_RepeatCount
        {
            get { return (_keyDataExtra & 0x0000FFFF); }
        }

        /// <summary>
        /// Indicates whether the key is an extended key, such as the right-hand
        /// ALT and CTRL keys that appear on an enhanced 101- or 102-key keyboard.
        /// The value is 1 if it is an extended key; otherwise, it is 0.
        /// </summary>
        public int Data_IsExtendedKey
        {
            get { return ((_keyDataExtra >> 24) & 0x00000001); }
        }

        /// <summary>
        /// The context code.
        /// The value is always 0 for a WM_KEYDOWN or a WM_KEYUP message.
        /// The value is 1 if the ALT key is down while the key is pressed;
        /// it is 0 if the WM_SYSKEYDOWN or WM_SYSKEYUP message is posted to
        /// the active window because no window has the keyboard focus.
        /// </summary>
        public int Data_ContextCode
        {
            get { return ((_keyDataExtra >> 29) & 0x00000001); }
        }

        /// <summary>
        /// The previous key state. The value is 1 if the key is down before the
        /// message is sent, or it is zero if the key is up.
        /// The value is always 1 for a WM_(SYS)KEYUP message.
        /// </summary>
        public int Data_PreviousState
        {
            get { return ((_keyDataExtra >> 30) & 0x00000001); }
        }

        /// <summary>
        /// The transition state. The value is always 0 for a WM_(SYS)KEYDOWN message.
        /// The value is always 1 for a WM_(SYS)KEYUP message.
        /// </summary>
        public int Data_TransitionState
        {
            get { return ((_keyDataExtra >> 31) & 0x00000001); }
        }

        public KeyEventArgs(WinKeys wParam_VirtKeyCode, int lParam_KeyData, WinKeys modifiers)
        {
            _keyCode = wParam_VirtKeyCode;
            _keyDataExtra = lParam_KeyData;
            _modifiers = modifiers;
        }
    }
}