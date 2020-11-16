namespace aspnetcore.blog
{
    public static class Constants
    {
        public static class Blog
        {
            public static readonly string Name = "aspnetcore blog";
            public static readonly string Title = "Title";
            public static readonly string Description = "aspnetcore.blog is full-featured yet simple blog engine written in ASP.NET Core 5 MVC";
            public static readonly string AuthorName = "aspnetcore.blog";
            public static readonly string UseBlobStorage = "n"; // Set to "y" to use Azure Blob Storage
            public static readonly string BlobContainerName = "posts"; // Azure Blob Storage Conatiner Name to store posts image
            public static readonly string BlobContainerNameFeaturedImage = "featured";  // Azure Blob Storage Conatiner Name to store featured image
            public static readonly int PostPerPage = 10;
            public static readonly string UseRedisCache = "n"; // Set to "y" to use Azure Redis Cache
            public static readonly int AbsoluteExpirationRelativeToNow = 60; // Time in seconds
            public static readonly string ReturnUrl = "ReturnUrl";
            public static readonly string EmailAddress = "test@test.com"; // This email address will be used to send emails via SendGrid from contact page

            public static class User
            {
                public static readonly string Password = "user:password";
                public static readonly string Salt = "user:salt";
                public static readonly string UserName = "user:username";
            }
        }        
    }
}
