using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Blink
{
    public partial class NativeListview : ListView
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        protected override void CreateHandle()
        {
            base.CreateHandle();
            // Aplicar el tema nativo del control ListView
            SetWindowTheme(this.Handle, "explorer", null);
        }

        public NativeListview()
        {
            // Activar double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Habilitar el evento OnNotifyMessage para luego poder filtrar
            // el mensaje WM_ERASEBKGND antes de que sea procesado
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);

            // Remover el rectangulo de selección de borde punteado
            SendMessage(Handle, 0x127, 0x10001, 0);
        }

        protected override void OnNotifyMessage(Message m)
        {
            // Filtrar el mensaje WM_ERASEBKGND
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            // Remover el rectangulo de selección de borde punteado
            SendMessage(Handle, 0x127, 0x10001, 0);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            // Remover el rectangulo de selección de borde punteado
            SendMessage(Handle, 0x127, 0x10001, 0);
        }
    }
}
