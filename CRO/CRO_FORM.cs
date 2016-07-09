using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CRO
{
    public partial class CRO_FORM : Form
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr AttachThreadInput(IntPtr idAttach,
                      IntPtr idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint wMsg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint wMsg, UIntPtr wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, UIntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam);

        private const uint EM_GETSEL = 0xB0;
        private const uint EM_SETSEL = 0x00B1;
        private const uint EM_REPLACESEL = 0x00C2;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int VK_RETURN = 0x0D;

        private bool IsPosting = false;

        public CRO_FORM()
        {
            InitializeComponent();
        }

        private IntPtr FocusedWindows()
        {
            IntPtr activeWindowHandle = GetForegroundWindow();
            IntPtr activeWindowThread = GetWindowThreadProcessId(activeWindowHandle, IntPtr.Zero);
            IntPtr thisWindowThread = GetWindowThreadProcessId(this.Handle, IntPtr.Zero);

            AttachThreadInput(activeWindowThread, thisWindowThread, true);
            IntPtr focusedHandle = GetFocus();
            AttachThreadInput(activeWindowThread, thisWindowThread, false);
            return focusedHandle;
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            IntPtr WindowsHandle = FocusedWindows();
            int StartPos = 0, EndPos = 0;

            string hashText = "#CRO #OrangeSponsorsYou";
             foreach (char character in hashText) {
                SendMessage(WindowsHandle, EM_GETSEL, (UIntPtr)StartPos, (IntPtr)EndPos);
                SendMessage(WindowsHandle, EM_SETSEL, (UIntPtr)StartPos, (IntPtr)EndPos);
                PostMessage(WindowsHandle, 0x0102, (UIntPtr)(character), UIntPtr.Zero);
            }

            SendMessage(WindowsHandle, EM_GETSEL, (UIntPtr)StartPos, (IntPtr)EndPos);
            SendMessage(WindowsHandle, EM_SETSEL, (UIntPtr)StartPos, (IntPtr)EndPos);
            PostMessage(WindowsHandle, WM_KEYDOWN, (UIntPtr)VK_RETURN, UIntPtr.Zero);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if( !IsPosting ) {
                timer1.Interval = (int)numericUpDown1.Value * 1000;
                timer1.Enabled = true;
                button1.Text = "Stop";
                IsPosting = true;
            } else {
                timer1.Enabled = false;
                button1.Text = "Start";
                IsPosting = false;
            }
        }

        private void CRO_FORM_Load(object sender, EventArgs e)
        {
            button1.Text = "Start";
            IsPosting = false;
            timer1.Enabled = false;
        }
    }
}
