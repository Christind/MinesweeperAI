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
        }

        public Bitmap Screenshot()
        {
            var size = new Size((XBoundsEnd - XBoundsStart), (YBoundsEnd - YBoundsStart));
            using (var bmpScreenshot = new Bitmap(size.Width, size.Height))
            {
                Graphics g = Graphics.FromImage(bmpScreenshot);

                g.CopyFromScreen(XBoundsStart, YBoundsStart, 0, 0, size);
                bmpScreenshot.Save("test2.jpg");

                g.Flush();

                return bmpScreenshot;
            }
        }
    }
}
