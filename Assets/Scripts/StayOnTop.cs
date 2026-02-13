#if UNITY_STANDALONE_WIN

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SuiLive;


public class AlwaysOnTop : MonoBehaviour {
    #region WIN32API

    public static readonly System.IntPtr HWND_TOPMOST = new System.IntPtr(-1);
    public static readonly System.IntPtr HWND_NOT_TOPMOST = new System.IntPtr(-2);
    const System.UInt32 SWP_SHOWWINDOW = 0x0040;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(System.Drawing.Rectangle r)
            : this(r.Left, r.Top, r.Right, r.Bottom) {
        }

        public int X {
            get {
                return Left;
            }
            set {
                Right -= (Left - value);
                Left = value;
            }
        }

        public int Y {
            get {
                return Top;
            }
            set {
                Bottom -= (Top - value);
                Top = value;
            }
        }

        public int Height {
            get {
                return Bottom - Top;
            }
            set {
                Bottom = value + Top;
            }
        }

        public int Width {
            get {
                return Right - Left;
            }
            set {
                Right = value + Left;
            }
        }

        public static implicit operator System.Drawing.Rectangle(RECT r) {
            return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator RECT(System.Drawing.Rectangle r) {
            return new RECT(r);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern System.IntPtr FindWindow(String lpClassName, String lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    #endregion

    // 缓存上次应用的置顶状态，避免每帧重复设置和打日志
    private bool? _lastAppliedTopmost;

    void Update() {
        bool wantTopmost = ConfigManager.Config.WindowSize.StayOnTop;
        if (_lastAppliedTopmost.HasValue && _lastAppliedTopmost.Value == wantTopmost)
            return;
        if (AssignTopmostWindow("SuiLive", wantTopmost))
            _lastAppliedTopmost = wantTopmost;
    }

    public bool AssignTopmostWindow(string WindowTitle, bool MakeTopmost) {
        System.IntPtr hWnd = FindWindow((string)null, WindowTitle);
        if (hWnd == IntPtr.Zero)
            return false;

        RECT rect = new RECT();
        if (!GetWindowRect(new HandleRef(this, hWnd), out rect))
            return false;

        bool ok = SetWindowPos(hWnd, MakeTopmost ? HWND_TOPMOST : HWND_NOT_TOPMOST, rect.X, rect.Y, rect.Width, rect.Height, SWP_SHOWWINDOW);
        if (ok)
            UnityEngine.Debug.Log("Assigning top most flag to window of title: " + WindowTitle + " -> " + (MakeTopmost ? "topmost" : "not topmost"));
        return ok;
    }

    private string[] GetWindowTitles() {
        List<string> WindowList = new List<string>();

        Process[] ProcessArray = Process.GetProcesses();
        foreach (Process p in ProcessArray) {
            if (!IsNullOrWhitespace(p.MainWindowTitle)) {
                WindowList.Add(p.MainWindowTitle);
            }
        }

        return WindowList.ToArray();
    }

    public bool IsNullOrWhitespace(string Str) {
        if (Str.Equals("null")) {
            return true;
        }
        foreach (char c in Str) {
            if (c != ' ') {
                return false;
            }
        }
        return true;
    }
}
#endif