using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Google
{
    public sealed class GoogleDataStore : IDataStore, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoogleTokens> _googleTokenRepository;

        public GoogleDataStore(IUnitOfWork unitOfWork, IRepository<GoogleTokens> googleTokenRepository)
        {
            _unitOfWork = unitOfWork;
            _googleTokenRepository = googleTokenRepository;
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
            var entity = await _googleTokenRepository.GetAsync(key, CancellationToken.None);
            if (entity == null)
            {
                entity = new GoogleTokens(key, serialized);
                await _googleTokenRepository.AddAsync(entity, CancellationToken.None);
            }

            entity.Value = serialized;
            await _googleTokenRepository.UpdateAsync(entity, CancellationToken.None);


            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        public async Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var entity = await _googleTokenRepository.GetAsync(key, CancellationToken.None);
            if (entity != null)
            {
                await _googleTokenRepository.DeleteAsync(entity, CancellationToken.None);
                await _unitOfWork.CommitAsync(CancellationToken.None);
            }

        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            var entity = await _googleTokenRepository.GetAsync(key, CancellationToken.None);
            if (entity == null)
            {
                return default(T);
            }

            return NewtonsoftJsonSerializer.Instance.Deserialize<T>(entity.Value);

        }

        public Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _googleTokenRepository?.Dispose();
        }
    }
}
