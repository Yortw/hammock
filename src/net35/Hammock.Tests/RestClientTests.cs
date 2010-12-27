﻿using System;
using System.Configuration;
using System.Net;
using System.Text;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using NUnit.Framework;

namespace Hammock.Tests
{
    [TestFixture]
    public partial class RestClientTests
    {
        private string _consumerKey;
        private string _consumerSecret;
        private string _accessToken;
        private string _accessTokenSecret;
        private string _twitPicKey;

        [SetUp]
        public void SetUp()
        {
            _consumerKey = ConfigurationManager.AppSettings["OAuthConsumerKey"];
            _consumerSecret = ConfigurationManager.AppSettings["OAuthConsumerSecret"];
            _accessToken = ConfigurationManager.AppSettings["OAuthAccessToken"];
            _accessTokenSecret = ConfigurationManager.AppSettings["OAuthAccessTokenSecret"];
            _twitPicKey = ConfigurationManager.AppSettings["TwitPicKey"];

            ServicePointManager.Expect100Continue = false;
        }

        [Test]
        public void Can_request_get()
        {
            var client = new RestClient
                             {
                                 Authority = "https://api.twitter.com", 
                                 UserAgent = "Hammock"
                             };

            var request = new RestRequest
                              {
                                  Path = "statuses/public_timeline.json",
                                  DecompressionMethods = DecompressionMethods.GZip |
                                                         DecompressionMethods.Deflate
                              };

            var response = client.Request(request);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Can_get_oauth_request_token()
        {
            var client = new RestClient
            {
                Authority = "http://api.twitter.com",
                UserAgent = "Hammock"
            };

            var request = new RestRequest
            {
                Path = "oauth/request_token",
                Credentials = OAuthCredentials.ForRequestToken(_consumerKey, _consumerSecret)
            };

            var response = client.Request(request);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Can_post_to_protected_resource()
        {
            var client = new RestClient
            {
                Authority = "http://api.twitter.com",
                VersionPath = "1",
                UserAgent = "Hammock"
            };

            var request = new RestRequest
            {
                Method = WebMethod.Post,
                Path = "statuses/update.json?status=" + DateTime.Now.Ticks,
                Credentials = OAuthCredentials.ForProtectedResource(
                _consumerKey, _consumerSecret, _accessToken, _accessTokenSecret
                )
            };

            var response = client.Request(request);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Can_post_raw_content()
        {
            var client = new RestClient {Authority = "http://www.apitize.com"};
            var request = new RestRequest();
            request.AddPostContent(Encoding.UTF8.GetBytes("Babbabooey!"));

            client.Request(request);
        }

        [Test]
        public void Can_stream_photo_over_delegated_credentials()
        {
            var client = new RestClient
            {
                Authority = "http://api.twitpic.com",
                VersionPath = "2",
                UserAgent = "Hammock"
            };

            var request = PrepareEchoRequest();
            request.Method = WebMethod.Get;
            request.Path = "upload.xml";
            request.AddField("key", _twitPicKey);
            request.AddFile("media", "failwhale", "_failwhale.jpg", "image/jpeg");
            
            var response = client.Request(request);
            Assert.IsNotNull(response);
            Console.WriteLine(response.Content);
        }

        [Test]
        [Ignore("Makes a live status update")]
        public void Can_prepare_oauth_with_url_parameters()
        {
            var client = new RestClient
            {
                Authority = "http://api.twitter.com",
                UserAgent = "Hammock"
            };

            var credentials = OAuthCredentials.ForProtectedResource(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);
            credentials.ParameterHandling = OAuthParameterHandling.UrlOrPostParameters;

            var request = new RestRequest
            {
                Path = "statuses/update.json",
                Method = WebMethod.Post,
                Credentials = credentials
            };

            request.AddParameter("status", DateTime.Now.Ticks.ToString());
            request.AddParameter("test", "value");

            var response = client.Request(request);
            Assert.IsNotNull(response);

            Console.WriteLine(response.Content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        public RestRequest PrepareEchoRequest()
        {
            var client = new RestClient
                             {
                                 Authority = "https://api.twitter.com",
                                 VersionPath = "1",
                                 UserAgent = "TweetSharp"
                             };

            var request = new RestRequest
                              {
                                  Method = WebMethod.Get,
                                  Path = "account/verify_credentials.json",
                                  Credentials = OAuthCredentials.ForProtectedResource(
                                      _consumerKey, _consumerSecret, _accessToken, _accessTokenSecret
                                      )
                              };

            return OAuthCredentials.DelegateWith(client, request);
        }
    }
}


