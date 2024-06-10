using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Phaos.CustomControls
{
    /// <summary>
    /// Customp NumericUpDown with flat appareance
    /// </summary>
    public class FlatNumericUpDown : NumericUpDown
    {
        #region ADDITIONNAL PROPERTIES
        /// <summary>
        /// Border color
        /// </summary>
        [Description("Color of the border")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    Invalidate();
                }
            }
        }
        private Color borderColor = Color.Gray;

        /// <summary>
        /// Button Highlight Color
        /// </summary>
        [Description("Hightlight color of the arrow buttons")]
        [DefaultValue(typeof(Color), "LightGray")]
        public Color ButtonHighlightColor
        {
            get { return buttonHighlightColor; }
            set
            {
                if (buttonHighlightColor != value)
                {
                    buttonHighlightColor = value;
                    Invalidate();
                }
            }
        }
        private Color buttonHighlightColor = Color.LightGray;

        /// <summary>
        /// Border Color in edit mode
        /// </summary>
        [Description("Color of the border in edition mode")]
        [DefaultValue(typeof(Color), "DarkOrange")]
        public Color EditBorderColor
        {
            get { return editBorderColor; }
            set
            {
                if (editBorderColor != value)
                {
                    editBorderColor = value;
                    Invalidate();
                }
            }
        }
        private Color editBorderColor = Color.DarkOrange;

        /// <summary>
        /// Border Color in edit mode
        /// </summary>
        [Description("Enable border color change in edition mode")]
        [DefaultValue(typeof(bool), "true")]
        public bool EnableBorderColoring
        {
            get { return enableBorderColoring; }
            set
            {
                if (enableBorderColoring != value)
                {
                    enableBorderColoring = value;
                    Invalidate();
                }
            }
        }
        private bool enableBorderColoring = true;

        /// <summary>
        /// Value increment when using mouse wheel
        /// </summary>
        [Description("Value increment when using mouse wheel. Set this value to -1 to use the system default.")]
        [DefaultValue(typeof(decimal), "1")]
        public decimal IncrementMouseWheel 
        { 
            get => incrementMouseWheel;
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("value must be greater than or equal to -1.");
                if (value == -1)
                    incrementMouseWheel = SystemInformation.MouseWheelScrollLines;
                else
                    incrementMouseWheel = value;
            }
        }
        private decimal incrementMouseWheel = 1;
        #endregion

        /// <summary>
        /// New FlatNumericUpDown control
        /// </summary>
        public FlatNumericUpDown() : base()
        {
            var renderer = new UpDownButtonRenderer(Controls[0]);
            this.KeyDown += FlatNumericUpDown_KeyDown;
            this.ValueChanged += FlatNumericUpDown_ValueChanged;
            this.KeyPress += FlatNumericUpDown_KeyPress;
        }

        #region INTERNAL CALLBACKS
        /// <summary>
        /// Accept both dot and comma as decimal separator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlatNumericUpDown_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('.') || e.KeyChar.Equals(','))
            {
                e.KeyChar = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];
            }
        }

        /// <summary>
        /// To handle "editing mode" visual effect on ValueChanged 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlatNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            isEditing = false;
            Invalidate();
        }

        /// <summary>
        /// To handle "editing mode" visual effect on KeyDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlatNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) isEditing = false;
            else isEditing = true;
            Invalidate();
        }
        private bool isEditing = false;

        /// <summary>
        /// Is in edit mode
        /// </summary>
        public bool IsEditing => isEditing;
        #endregion

        #region CONTROL OVERRIDES
        /// <inheritdoc/>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return cp;
            }
        }

        /// <inheritdoc/>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (BorderStyle == BorderStyle.FixedSingle)
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
                Color bColor = BorderColor;
                if (enableBorderColoring && isEditing) bColor = EditBorderColor;
                using (var pen = new Pen(bColor, 1))
                {
                    e.Graphics.DrawRectangle(pen,
                        ClientRectangle.Left, ClientRectangle.Top,
                        ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e is HandledMouseEventArgs hme)
                hme.Handled = true;

            if (e.Delta > 0 && Value + IncrementMouseWheel <= Maximum)
                this.Value += IncrementMouseWheel;
            else if (e.Delta < 0 && Value - IncrementMouseWheel >= Minimum)
                this.Value -= IncrementMouseWheel;
        }
        #endregion

        #region Arrow buttons rendered
        private class UpDownButtonRenderer : NativeWindow
        {
            [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "BeginPaint", CharSet = CharSet.Auto)]
            private static extern IntPtr IntBeginPaint(IntPtr hWnd, [In, Out] ref PAINTSTRUCT lpPaint);
            [StructLayout(LayoutKind.Sequential)]
            public struct PAINTSTRUCT
            {
                public IntPtr hdc;
                public bool fErase;
                public int rcPaint_left;
                public int rcPaint_top;
                public int rcPaint_right;
                public int rcPaint_bottom;
                public bool fRestore;
                public bool fIncUpdate;
                public int reserved1;
                public int reserved2;
                public int reserved3;
                public int reserved4;
                public int reserved5;
                public int reserved6;
                public int reserved7;
                public int reserved8;
            }
            [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "EndPaint", CharSet = CharSet.Auto)]
            private static extern bool IntEndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

            readonly Control updown;
            public UpDownButtonRenderer(Control c)
            {
                this.updown = c;
                if (updown.IsHandleCreated)
                    this.AssignHandle(updown.Handle);
                else
                    updown.HandleCreated += (s, e) => this.AssignHandle(updown.Handle);
            }
            private Point[] GetDownArrow(Rectangle r)
            {
                var middle = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
                return new Point[]
                {
                new Point(middle.X - 3, middle.Y - 2),
                new Point(middle.X + 4, middle.Y - 2),
                new Point(middle.X, middle.Y + 2)
                };
            }
            private Point[] GetUpArrow(Rectangle r)
            {
                var middle = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
                return new Point[]
                {
                new Point(middle.X - 4, middle.Y + 2),
                new Point(middle.X + 4, middle.Y + 2),
                new Point(middle.X, middle.Y - 3)
                };
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0xF /*WM_PAINT*/ && ((FlatNumericUpDown)updown.Parent).BorderStyle == BorderStyle.FixedSingle)
                {
                    var s = new PAINTSTRUCT();
                    IntBeginPaint(updown.Handle, ref s);
                    using (var g = Graphics.FromHdc(s.hdc))
                    {
                        var enabled = updown.Enabled;
                        //using (var backBrush = new SolidBrush(enabled ? ((FlatNumericUpDown)updown.Parent).BackColor : SystemColors.Control))
                        //{
                        //    g.FillRectangle(backBrush, updown.ClientRectangle);
                        //}
                        g.FillRectangle(new SolidBrush(((FlatNumericUpDown)updown.Parent).BackColor), updown.ClientRectangle);
                        var r1 = new Rectangle(0, 0, updown.Width, updown.Height / 2);
                        var r2 = new Rectangle(0, updown.Height / 2, updown.Width, updown.Height / 2 + 1);
                        var p = updown.PointToClient(MousePosition);
                        if (enabled && updown.ClientRectangle.Contains(p))
                        {
                            using (var b = new SolidBrush(((FlatNumericUpDown)updown.Parent).ButtonHighlightColor))
                            {
                                if (r1.Contains(p))
                                    g.FillRectangle(b, r1);
                                else
                                    g.FillRectangle(b, r2);
                            }
                            using (var pen = new Pen(((FlatNumericUpDown)updown.Parent).BorderColor))
                            {
                                g.DrawLines(pen,
                                    new[] { new Point(0, 0), new Point(0, updown.Height),
                                        new Point(0, updown.Height / 2), new Point(updown.Width, updown.Height / 2)
                                    });
                            }
                        }
                        g.FillPolygon(new SolidBrush(((FlatNumericUpDown)updown.Parent).ForeColor), GetUpArrow(r1));
                        g.FillPolygon(new SolidBrush(((FlatNumericUpDown)updown.Parent).ForeColor), GetDownArrow(r2));
                    }
                    m.Result = (IntPtr)1;
                    base.WndProc(ref m);
                    IntEndPaint(updown.Handle, ref s);
                }
                else if (m.Msg == 0x0014/*WM_ERASEBKGND*/)
                {
                    using (var g = Graphics.FromHdcInternal(m.WParam))
                        g.FillRectangle(Brushes.White, updown.ClientRectangle);
                    m.Result = (IntPtr)1;
                }
                else
                    base.WndProc(ref m);
            }
        }
        #endregion
    }
}
