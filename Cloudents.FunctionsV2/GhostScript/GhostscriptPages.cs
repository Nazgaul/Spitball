namespace Cloudents.FunctionsV2.GhostScript
{
    /// <summary>
    /// Which pages to output
    /// </summary>
    public class GhostscriptPages
    {
        private bool _allPages = true;
        private int _start;
        private int _end;

        /// <summary>
        /// Output all pages avaialble in document
        /// </summary>
        public bool AllPages
        {
            set
            {
                _start = -1;
                _end = -1;
                _allPages = true;
            }
            get
            {
                return _allPages;
            }
        }

        /// <summary>
        /// Start output at this page (1 for page 1)
        /// </summary>
        public int Start
        {
            set
            {
                _allPages = false;
                _start = value;
            }
            get
            {
                return _start;
            }
        }

        /// <summary>
        /// Page to stop output at
        /// </summary>
        public int End
        {
            set
            {
                _allPages = false;
                _end = value;
            }
            get
            {
                return _end;
            }
        }
    }
}