namespace Cloudents.Core.Entities.Db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Zbox.Box")]
    public partial class Box
    {
        public long BoxId { get; set; }

        [Required]
        [StringLength(255)]
        public string BoxName { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }

        [Required]
        [StringLength(255)]
        public string CreatedUser { get; set; }

        [Required]
        [StringLength(255)]
        public string UpdatedUser { get; set; }

        public int PrivacySetting { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        public bool? IsDeleted { get; set; }

        public bool? Star { get; set; }

        public long? OwnerId { get; set; }

        public int? Discriminator { get; set; }

        [StringLength(400)]
        public string Description { get; set; }

        [StringLength(255)]
        public string CourseCode { get; set; }

        [StringLength(255)]
        public string ProfessorName { get; set; }

        [StringLength(255)]
        public string BoxPicture { get; set; }

        public int MembersCount { get; set; }

        public int ItemCount { get; set; }

        public int CommentCount { get; set; }

        [StringLength(255)]
        public string PictureUrl { get; set; }

        [StringLength(255)]
        public string Url { get; set; }

        public long? University { get; set; }

        public int? QuizCount { get; set; }

        public Guid? LibraryId { get; set; }

        public bool? IsDirty { get; set; }

        public Guid? LibraryParentId { get; set; }

        public int? FlashcardCount { get; set; }
    }
}
