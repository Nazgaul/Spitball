using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class UserFirstTime
    {
        public UserFirstTime()
        {
            Dashboard = true;
            Library = true;
            Item = true;
            Box = true;
        }
        public bool Dashboard { get; private set; }
        public bool Library { get; private set; }
        public bool Item { get; private set; }
        public bool Box { get; private set; }

        public void DashboardFirstTimeShow()
        {
            Dashboard = false;
        }
        public void LibraryFirstTimeShow()
        {
            Library = false;
        }
        public void ItemFirstTimeShow()
        {
            Item = false;
        }
        public void BoxFirstTimeShow()
        {
            Box = false;
        }
    }
}
