using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.ObjectModel;
using Zbang.Zbox.Domain.Util;
using System.Data;

namespace Zbang.Zbox.Domain
{
    public class Box : ICommentTarget
    {
        private string m_sharePassword;

        private ICollection<File> m_Files;
        private ReadOnlyCollection<File> m_ReadonlyFiles;

        private ICollection<Link> m_Links;
        private ReadOnlyCollection<Link> m_ReadonlyLinks;

        private ICollection<ShareBoxInvitation> m_Invitations;
        private ReadOnlyCollection<ShareBoxInvitation> m_ReadonlyInvitations;

        private ICollection<NotificationRules> m_Rules;
        private ReadOnlyCollection<NotificationRules> m_ReadonlyRules;

        // comments is unidirectional one to many association and therefor doesn't need management, it can be exposed directly to clients
        private ICollection<Comment> m_Comments;

        //Ctor
        internal protected Box()
        {
            CreationTimeUtc = DateTime.UtcNow;
            uId = Guid.NewGuid();
            m_Files = new HashSet<File>();
            m_ReadonlyFiles = new ReadOnlyCollection<File>(new List<File>());
            m_Invitations = new HashSet<ShareBoxInvitation>();
            m_ReadonlyInvitations = new ReadOnlyCollection<ShareBoxInvitation>(new List<ShareBoxInvitation>());
            m_Links = new HashSet<Link>();
            m_ReadonlyLinks = new ReadOnlyCollection<Link>(new List<Link>());
            m_Comments = new HashSet<Comment>();
            Subscribers = new HashSet<Subscriber>();
            m_Rules = new HashSet<NotificationRules>();
            m_ReadonlyRules = new ReadOnlyCollection<NotificationRules>(new List<NotificationRules>());

            PrivacySettings = BoxPrivacySettings.NotShared;

        }

        //Properties
        public virtual int Id { get; protected set; }
        public virtual Storage Storage { get; set; }
        public virtual string BoxName { get; set; }
        public virtual DateTime CreationTimeUtc { get; internal protected set; }
        public virtual BoxPrivacySettings PrivacySettings { get; set; }
        public virtual ICollection<Subscriber> Subscribers { get; protected set; }
        public virtual Guid uId { get; protected set; }


        public int CommentTargetId
        {
            get { return Id; }
        }

        public virtual string SharePassword
        {
            get
            {
                if (this.PrivacySettings.Equals(BoxPrivacySettings.PasswordProtected))
                    return m_sharePassword;
                else
                    return "";
            }
            set
            {
                m_sharePassword = value;
            }
        }

        public virtual ICollection<Comment> Comments
        {
            get { return m_Comments; }
            set
            {
                m_Comments = value;
            }
        }

        protected virtual ICollection<File> Files
        {
            get { return m_Files; }
            set
            {
                m_Files = value;
                m_ReadonlyFiles = new ReadOnlyCollection<File>(new List<File>(value));
            }
        }

        protected virtual ICollection<ShareBoxInvitation> Invitations
        {
            get
            {
                return m_Invitations;
            }
            set
            {
                m_Invitations = value;
                m_ReadonlyInvitations = new ReadOnlyCollection<ShareBoxInvitation>(new List<ShareBoxInvitation>(value));
            }
        }

        protected virtual ICollection<Link> Links
        {
            get { return m_Links; }
            set
            {
                m_Links = value;
                m_ReadonlyLinks = new ReadOnlyCollection<Link>(new List<Link>(value));
            }
        }

        internal protected virtual ICollection<NotificationRules> Rules
        {
            get { return m_Rules; }
            set
            {
                m_Rules = value;
                m_ReadonlyRules = new ReadOnlyCollection<NotificationRules>(new List<NotificationRules>(value));
            }
        }

        public ReadOnlyCollection<File> GetFiles()
        {
            return m_ReadonlyFiles;
        }

        public ReadOnlyCollection<ShareBoxInvitation> GetInvitations()
        {
            return m_ReadonlyInvitations;
        }

        public ReadOnlyCollection<Link> GetLinks()
        {
            return m_ReadonlyLinks;
        }

        public ReadOnlyCollection<NotificationRules> GetRules()
        {
            return m_ReadonlyRules;
        }

        public bool DeleteLink(Link link)
        {
            return Links.Remove(link);
        }

        //Methods

        public void RemoveInvitation(ShareBoxInvitation toFriend)
        {
            Invitations.Remove(toFriend);
            //ICollection<ShareBoxInvitation> shareBoxInvitations = Invitations;
            //foreach (ShareBoxInvitation invitation in shareBoxInvitations)
            //{
            //    if (invitation.Friend.Equals(toFriend))
            //    {
            //        shareBoxInvitations.Remove(invitation);
            //        break;
            //    }
            //}            
        }

        //TODO this is not good, storage should contain a reference to user instead of user id, that way we could do addInvitation(friend) which is a better api and discard the user id equality check
        public ShareBoxInvitation AddInvitation(User sender, Friend friend)
        {
            ShareBoxInvitation invitation = null;
            var subscribers = this.Subscribers.SingleOrDefault(s => s.User.Email == friend.FriendEmailAddress);
            if (subscribers == null)
            {
                invitation = this.Invitations.SingleOrDefault(s => s.Friend == friend);
                //First time invite
                if (invitation == null)
                {
                    invitation = new ShareBoxInvitation(sender, this, friend);
                    this.Invitations.Add(invitation);
                }
                //ReInvite
                else
                {
                    invitation.CreationTimeUtc = DateTime.UtcNow;
                }
            }
            return invitation;
        }


        public File AddFile(string blobAddressUri, string thumbnailBlobAddressUri, string fileName, string contentType, long length, Guid uploaderId)
        {
            string uniqueFileName = GetUniqueFileName(fileName);

            File file = new File()
            {
                BoxId = this.Id,
                BlobAddressUri = blobAddressUri,
                ThumbnailBlobAddressUri = thumbnailBlobAddressUri,
                FileName = uniqueFileName,
                ContentType = contentType,
                Length = length,
                UploaderId = uploaderId
            };

            Files.Add(file);

            return file;
        }

        private string GetUniqueFileName(string fileName)
        {

            //Find exact macth
            var foundMatch = (from file in Files
                              where file.FileName.ToLower() == fileName.ToLower()
                              select file).Count();

            if (foundMatch > 0)
            {
                int index = 0;

                //Find next available index
                do
                {
                    index++;
                    foundMatch = (from file in Files
                                  where file.FileName.ToLower() == string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(fileName), index, Path.GetExtension(fileName)).ToLower()
                                  select file).Count();
                } while (foundMatch > 0);

                fileName = string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(fileName), index, Path.GetExtension(fileName));
            }

            return fileName;
        }

        public Link AddLink(string url, Guid uploaderId)
        {
            Link l = new Link(this, url, uploaderId);

            if (checkIfLinkExists(l))
                m_Links.Add(l);

            return l;
        }

        private bool checkIfLinkExists(Link newlink)
        {
            bool retval = true;
            Link existingLink = m_Links.SingleOrDefault(existingLinksInBox => existingLinksInBox.Equals(newlink));
            if (existingLink != null)
            {
                retval = false;
                throw new DuplicateNameException("This link already exists in the box");
            }

            return retval;
        }

        public Subscriber AddSubscriber(User user, UserPermissionSettings iPermission = UserPermissionSettings.ReadWrite)
        {
            var newSubscribe = new Subscriber() { User = user, permission = iPermission, Box = this };
            Subscribers.Add(newSubscribe);
            this.Rules.Add(new NotificationRules { box = this, user = user.UserId, notificationSetting = NotificationSettings.On });

            return newSubscribe;
        }

        public void RemoveSubscriber(Subscriber subscriber)
        {
            var NotificationRules = this.Rules.Where(w => w.user == subscriber.User.UserId).ToList();
            foreach( var subscriberNotification in NotificationRules)
                this.Rules.Remove(subscriberNotification);
            Subscribers.Remove(subscriber);
        }



        public Comment AddComment(User author, string commentText)
        {
            Comment comment = new Comment(author, commentText);
            this.Comments.Add(comment);

            return comment;
        }

        //Change null value in table to concreate values - should remove this and make concreate not null value in db.
        public void InitNotification(Guid owner)
        {
            var ownerUser = this.Rules.SingleOrDefault(x => x.user == owner);
            if (ownerUser == null)
                this.Rules.Add(new NotificationRules { box = this, user = owner, notificationSetting = NotificationSettings.On });

            var subscribers = this.Subscribers.Select(x => x.User.UserId).Except(this.Rules.Select(y => y.user));
            foreach (var user in subscribers)
            {
                this.Rules.Add(new NotificationRules { box = this, user = user, notificationSetting = NotificationSettings.On });
            }
            m_ReadonlyRules = new ReadOnlyCollection<NotificationRules>(m_Rules.ToList());
        }


        public UserPermissionSettings GetUserPermission(Guid user)
        {
            UserPermissionSettings retval = UserPermissionSettings.Read;

            if (this.Storage.UserId == user)
                retval = UserPermissionSettings.Owner;
            else
            {
                //Check if subscribe
                var subscriber = this.Subscribers.FirstOrDefault(x => x.User.UserId == user);
                if (subscriber == null)
                    throw new UnauthorizedAccessException(string.Format("the user {0} does not have permission to this box. id: {1}", user.ToString(), this.Id));
                retval = subscriber.permission;

            }
            return retval;

        }

        private const string TOKEN_DATA_SEPARATOR = "||";

        // minutes
        private const int MAX_TOKEN_AGE = 20;

        // A Margin in minutes between 0 and MAX_TOKEN_AGE (Inclusive) where we regenerate the token (instead of expiring it)
        private const int TOKEN_REGEN_MARGIN = 3;

        public string Authenticate(string pass)
        {
            if (!pass.Equals(this.SharePassword))
                throw new BoxAccessDeniedException();

            return generateAccessToken(pass);
        }

        public string RegenerateAccessToken(string token)
        {
            string[] parsedToken = decryptToken(token);

            //separated to different ifs just for clarity:

            if (!IsTokenValid(parsedToken))
                throw new BoxAccessTokenExpiredException();

            DateTime tokenCreationTime = DateTime.Parse(parsedToken[2]);
            DateTime now = DateTime.Now;

            if (now.Subtract(tokenCreationTime).Minutes > MAX_TOKEN_AGE)
                throw new BoxAccessTokenExpiredException();

            // token did not expire yet, lets check if we need to regenerate it
            if (now.Subtract(tokenCreationTime).Minutes > (MAX_TOKEN_AGE - TOKEN_REGEN_MARGIN))
                return generateAccessToken(this.SharePassword);
            else
                return "";
        }

        private bool IsTokenValid(string[] parsedToken)
        {
            if (parsedToken.Length != 3)
                return false;

            if (!parsedToken[0].Equals(this.SharePassword))
                return false;

            if (!parsedToken[1].Equals(Id.ToString()))
                return false;

            return true;
        }

        private string[] decryptToken(string token)
        {
            string decryptedToken = Cryptography.Decrypt(token, this.SharePassword);

            return decryptedToken.Split(new string[] { TOKEN_DATA_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string generateAccessToken(string pass)
        {
            StringBuilder tokenBuilder = new StringBuilder(pass).Append(TOKEN_DATA_SEPARATOR);
            tokenBuilder.Append(Id).Append(TOKEN_DATA_SEPARATOR);
            tokenBuilder.Append(DateTime.Now);

            return Cryptography.Encrypt(tokenBuilder.ToString(), pass);
        }

        public override bool Equals(object obj)
        {
            Box other = obj as Box;

            if (other == null)
                return false;

            return BoxName.Equals(other.BoxName) && uId.Equals(other.uId);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 11 * BoxName.GetHashCode() + 13 * uId.GetHashCode();
            }
        }
    }
}
