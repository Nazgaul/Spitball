﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Places
{
    [UsedImplicitly]
    public class PlacesSearch : IPlacesSearch
    {
        private readonly IGooglePlacesSearch _placesSearch;
        private readonly IReadRepositoryAsync<IEnumerable<HookedDto>, IEnumerable<string>> _hookedRepository;

        public PlacesSearch(IGooglePlacesSearch placesSearch, IReadRepositoryAsync<IEnumerable<HookedDto>, IEnumerable<string>> hookedRepository)
        {
            _placesSearch = placesSearch;
            _hookedRepository = hookedRepository;
        }

        public async Task<PlacesNearbyDto> SearchAsync(IEnumerable<string> term, PlacesRequestFilter filter, GeoPoint location, string nextPageToken,
            CancellationToken token)
        {
            var result = await _placesSearch.SearchNearbyAsync(term, filter, location, nextPageToken, token).ConfigureAwait(false);
            if (result?.Data == null)
            {
                return null;
            }
            var data = result.Data.ToList();
            var resultHooked = await _hookedRepository.GetAsync(data.Select(s => s.PlaceId), token).ConfigureAwait(false);
            var hash = new HashSet<string>(resultHooked.Select(s => s.Id));
            foreach (var place in data)
            {
                place.Hooked = hash.Contains(place.PlaceId);
            }

            return new PlacesNearbyDto
            {
                Data = data,
                Token = result.Token
            };
        }
    }
}
