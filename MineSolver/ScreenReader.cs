using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSolver
{
    public class ScreenReader
    {
        public int XBoundsStart { get; set; }
        public int XBoundsEnd { get; set; }
        public int YBoundsStart { get; set; }
        public int YBoundsEnd { get; set; }

        public ScreenReader()
        {
            XBoundsStart = 500;
            YBoundsStart = 500;
            XBoundsEnd = 1800;
            YBoundsEnd = 1800;
        }

        public Bitmap Screenshot()
        {
            Bitmap bmpScreenshot = new Bitmap((XBoundsEnd - XBoundsStart), (YBoundsEnd - YBoundsStart));

            Graphics g = Graphics.FromImage(bmpScreenshot);
            var size = new Size((XBoundsEnd - XBoundsStart), (YBoundsEnd - YBoundsStart));
            g.CopyFromScreen(XBoundsStart, YBoundsStart, XBoundsEnd, YBoundsEnd, size);
            bmpScreenshot.Save("test2.jpg");
            
            return bmpScreenshot;
        }
    }
}
