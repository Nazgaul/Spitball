using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class Storage
    {        
        //Ctor        
        public Storage()
        {
            CreationTimeUtc = DateTime.UtcNow;
            this.Quota = new Quota();
        }
        
        public virtual int StorageId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual DateTime CreationTimeUtc { get; set; }

        public virtual Quota Quota { get; set; }

        public virtual IList<Box> Boxes { get; set; }   
        
        //Methods               
        public Box CreateBox(string boxName, BoxPrivacySettings boxPrivacySettings = BoxPrivacySettings.NotShared ,NotificationSettings boxNotificationSettings = NotificationSettings.Off, string boxPrivacyPassword = "")
        {
            if (IsBoxNameUnique(boxName))
            {
                Box box = new Box() { Storage = this, 
                                      BoxName = boxName };

                AddNotification(box,boxNotificationSettings);
                ChangePrivacySettings(box, boxPrivacySettings, boxPrivacyPassword);

                if (Boxes == null)
                {
                    Boxes = new List<Box>();
                }
                Boxes.Add(box);
                return box;
            }
            else
            {
                throw new ArgumentException("A box with the same name already exist");
            }
        }

        private void AddNotification(Box box, NotificationSettings notificationSettings)
        {
            box.Rules.Add(new NotificationRules { box = box, notificationSetting = notificationSettings, user = this.UserId });
        }

        private void ChangePrivacySettings(Box box, BoxPrivacySettings privacySettings, string password)
        {
            box.PrivacySettings = privacySettings;
            if (privacySettings == BoxPrivacySettings.PasswordProtected)
                box.SharePassword = password;
        }
       
        private bool IsBoxNameUnique(string boxName)
        {
            if (Boxes != null)
            {
                //Find exact macth
                var foundMatch = (from box in Boxes
                                  where box.BoxName.ToLower() == boxName.ToLower()
                                  select box).Count();

                return foundMatch == 0;
            }
            else
            {
                return true;
            }
        }
    }
}
