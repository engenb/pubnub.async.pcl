﻿using System.Threading.Tasks;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.History;

namespace PubNub.Async.Services.History
{
    public interface IHistoryService
    {
        Task<HistoryResponse<TContent>> History<TContent>(
            long? start = null,
            long? end = null,
            int? count = null,
            bool reverse = false,
            bool includeTime = true);
    }
}