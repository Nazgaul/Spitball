using System.Drawing;

namespace Cloudents.FunctionsV2.GhostScript
{
    /// <summary>
    /// Output document physical dimensions
    /// </summary>
    public class GhostscriptPageSize
    {
        private GhostscriptPageSizes _fixed;
        private Size _manual;

        /// <summary>
        /// Custom document size
        /// </summary>
        public Size Manual
        {
            set
            {
                _fixed = GhostscriptPageSizes.UNDEFINED;
                _manual = value;
            }
            get
            {
                return _manual;
            }
        }

        /// <summary>
        /// Standard paper size
        /// </summary>
        public GhostscriptPageSizes Native
        {
            set
            {
                _fixed = value;
                _manual = new Size(0, 0);
            }
            get
            {
                return _fixed;
            }
        }

    }
}