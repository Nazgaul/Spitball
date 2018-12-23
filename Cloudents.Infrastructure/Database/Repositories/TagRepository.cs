﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class TagRepository : NHibernateRepository<Tag>, ITagRepository
    {
        public TagRepository(ISession session) : base(session)
        {
        }

        public async Task<Tag> GetOrAddAsync(string name, CancellationToken token)
        {
            if (name == null)
            {
                throw new System.ArgumentNullException(nameof(name));
            }
            name = name.Trim();
            var tag = await GetAsync(name, token);

            if (tag == null)
            {

                tag = new Tag(name);
                await AddAsync(tag, token).ConfigureAwait(true);
            }

            return tag;
        }
    }
}