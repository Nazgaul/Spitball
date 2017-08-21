﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IItemWriteSearchProvider
    {
        Task UpdateDataAsync(DocumentSearchDto itemToUpload, IEnumerable<long> itemToDelete, CancellationToken token);
    }

    public interface IContentWriteSearchProvider
    {
        Task UpdateDataAsync(DocumentSearchDto itemToUpload, IEnumerable<ItemToDeleteSearchDto> itemToDelete, CancellationToken token);

        Task UpdateDataAsync(Document itemToUpload, CancellationToken token);

        Task DeleteDataAsync(IEnumerable<string> ids, CancellationToken token);
    }

    public interface ISearchServiceWrite<in T> where T : class, ISearchObject, new()
    {
        Task UpdateDataAsync(IEnumerable<T> items, CancellationToken token);

        Task DeleteDataAsync(IEnumerable<string> ids, CancellationToken token);

        Task UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> ids, CancellationToken token);
    }

    public interface ISearchObject
    {
        string Id { get; set; }
    }

}