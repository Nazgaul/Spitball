using System.Drawing;

namespace Cloudents.FunctionsV2.GhostScript
{
    /// <summary>
    /// Ghostscript settings
    /// </summary>
    public class GhostscriptSettings
    {
        private GhostscriptDevices _device;
        private GhostscriptPages _pages = new GhostscriptPages();
        private Size _resolution;
        private GhostscriptPageSize _size = new GhostscriptPageSize();

        public GhostscriptDevices Device
        {
            get { return _device; }
            set { _device = value; }
        }

        public GhostscriptPages Page
        {
            get { return _pages; }
            set { _pages = value; }
        }

        public Size Resolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }

        public GhostscriptPageSize Size
        {
            get { return _size; }
            set { _size = value; }
        }
    }
}