using Google.Apis.Auth.OAuth2;
using Google.Apis.Blogger.v3;
using Google.Apis.Blogger.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace fabel_extractor_common
{
  public class BloggerManager
  {
		private string[] scopes = new string[]
		{
			BloggerService.Scope.Blogger
		};

		private static BloggerManager instance = null;
		private static readonly object instanceLock = new object();

		private string appName, appCredentialsPath;
		private BloggerService service;
		private UserCredential userCredentials;



		public static BloggerManager Instance
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
						instance = new BloggerManager();

					return instance;
				}
			}
		}

		public string AppName { get => appName; set => appName = value; }
		public string AppCrentialsPath { get => appCredentialsPath; set => appCredentialsPath = value; }

		private BloggerManager()
		{

		}

		public async System.Threading.Tasks.Task AuthAndIntitAsync(string appName, string appCrentialsPath)
		{
			this.appName = appName;
			this.appCredentialsPath = appCrentialsPath;

			using(var stream = new FileStream(this.appCredentialsPath, FileMode.Open, FileAccess.Read))
			{
				userCredentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					new[] { "https://www.googleapis.com/auth/blogger" },
					"user",
					CancellationToken.None,
					new FileDataStore("fabel-extractor-blogger")
				);
			}

			this.service = new BloggerService(new BaseClientService.Initializer
			{
				HttpClientInitializer = this.userCredentials,
				ApplicationName = this.appName
			});

			
		}

		public IList<Post> GetPosts(string blogId)
		{
			return this.service.Posts.List(blogId).Execute().Items;
		}

		public Post GetPost(string blogId, string postId)
		{
			PostsResource.GetRequest getRequest = new PostsResource.GetRequest(service, blogId, postId);
			return getRequest.Execute();
		}

		public string GetPostContent(string blogId, string postId)
		{
			return this.GetPost(blogId, postId).Content;
		}

		public void SetPostConent(string blogId, string postId, string content)
		{
			Post post = this.GetPost(blogId, postId);

			post.Content = content;

			PostsResource.UpdateRequest updateRequest = new PostsResource.UpdateRequest(service, post, blogId, postId);

			updateRequest.Execute();
		}
	}
}
