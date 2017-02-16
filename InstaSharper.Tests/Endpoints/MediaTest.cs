﻿using System;
using System.Linq;
using InstaSharper.Classes;
using InstaSharper.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace InstaSharper.Tests.Endpoints
{
    [Collection("Endpoints")]
    public class MediaTest
    {
        private readonly ITestOutputHelper _output;

        public MediaTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnlyTheory]
        [InlineData("1379932752706850783")]
        public async void GetMediaByCodeTest(string mediaId)
        {
            //arrange
            var username = "alex_codegarage";
            var password = Environment.GetEnvironmentVariable("instaapiuserpassword");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            //act
            _output.WriteLine($"Trying to login as user: {username}");
            if (!TestHelpers.Login(apiInstance, _output)) return;
            _output.WriteLine($"Getting media by ID: {mediaId}");
            var media = await apiInstance.GetMediaByCodeAsync(mediaId);
            //assert
            Assert.NotNull(media);
        }

        [RunnableInDebugOnlyTheory]
        [InlineData("therock")]
        public async void GetUserMediaListTest(string userToFetch)
        {
            //arrange
            var username = "alex_codegarage";
            var password = Environment.GetEnvironmentVariable("instaapiuserpassword");
            var apiInstance = TestHelpers.GetDefaultInstaApiInstance(new UserSessionData
            {
                UserName = username,
                Password = password
            });
            var random = new Random(DateTime.Today.Millisecond);
            var pages = 2;
            //act
            _output.WriteLine($"Trying to login as user: {username}");
            if (!TestHelpers.Login(apiInstance, _output)) return;
            _output.WriteLine($"Getting posts of user: {userToFetch}");

            var posts = await apiInstance.GetUserMediaAsync(userToFetch, pages);
            var anyDuplicate = posts.Value.GroupBy(x => x.Code).Any(g => g.Count() > 1);

            //assert
            Assert.NotNull(posts);
            Assert.Equal(userToFetch, posts.Value[random.Next(0, posts.Value.Count)].User.UserName);
            Assert.Equal(pages, posts.Value.Pages);
            Assert.False(anyDuplicate);
        }
    }
}