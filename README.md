# Blog engine for ASP.NET Core 5
aspnetcore.blog is full-featured yet simple blog engine written in ASP.NET Core 5 MVC. It is small, easy to use and highly customizable.<br>
**Live Demo**: https://aspnetcoreblog.azurewebsites.net <br>
**/admin**: Admin panel<br>
Username: *admin*<br>
Password: *admin*

# Getting Started
1. Clone or download source code
2. Run application in Visual Studio or using your code editor
3. Use /admin to log in

# Features
- Search engine optimized
- All major browsers fully supported
- Social media integration (Facebook, Twitter, Linkedin)
- Lazy loads images for better performance
- Easy YouTube video embedding

#### Newsletter
Visitors can subscribe to the blog to be notified on new publications by email via newsletter (requires SendGrid email account)

# Technical features
- Responsive web design
- Mobile friendly
    - [Run Mobile-Friendly Test](https://search.google.com/test/mobile-friendly?id=vZcXJeBQsN7gKcSZoie_5A)
- Supports JSON-LD (JavaScript Object Notation for Linked Data)
    - [Run Rich Results Test](https://search.google.com/test/rich-results?utm_campaign=sdtt&utm_medium=url&id=4KUbFQ6LUtOfDAaYBOYVFA)
- OpenGraph support for Facebook, Twitter, Pinterest and more
    - [Check the tags](https://www.opengraph.xyz/url/https:%2F%2Faspnetcoreblog.azurewebsites.net%2F/)
- Seach engine optimized
- COnfigured to use CDN to make it easy to serve the images from any CDN using [CDN Tag Helper](https://github.com/madskristensen/WebEssentials.AspNetCore.CdnTagHelpers).
- Uses Azure Blob Storage to store static files (images,css,js)
- Security HTTP headers set
    - [Run security scan](https://securityheaders.com/?q=https%3A%2F%2Faspnetcoreblog.azurewebsites.net&hide=on&followRedirects=on)

### YouTube embeds
The following syntax can be used to embed any Youtube video in the source of a blog post:
>[youtube:xxxxxxxx]<br>
<em>xxxxxxxx</em> is the ID of the youtube video which can be found in any YouTube link looking this <em>youtube.com/watch?v=xxxxxxxx</em>

