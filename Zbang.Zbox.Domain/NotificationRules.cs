using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class NotificationRules 
    {
        //Ctors
        public NotificationRules(Box box, Guid user, NotificationSettings notificationSettings)
        {
            this.box = box;
            this.user = user;
            this.notificationSetting = notificationSetting;
            
        }
        
        internal NotificationRules()
        {
            notificationSetting = NotificationSettings.On;
        }

        //Properties
        public virtual int NotificationRuleId { get; set; }
        public virtual NotificationSettings notificationSetting { get; set; }
        public virtual Guid user { get; set; }
        public virtual Box box { get; set; }
    }   
}
