﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class AnswerToFriendDto
    {
        private string m_BoxPicture;
        public string BoxPicture { get { return m_BoxPicture; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    m_BoxPicture = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(value);
                }
            }
        }
        public long Boxid { get; set; }
        public string BoxName { get; set; }
        public long QUserId { get; set; }
        public string QContent { get; set; }
        public string QUserImage { get; set; }
        public string QUserName { get; set; }
        public string Content { get; set; }
        public int AnswersCount { get; set; }

        public string UniversityName { get; set; }

        public string Url { get; set; }
      
    }
}
