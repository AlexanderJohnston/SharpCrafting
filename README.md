# SharpCrafting
Exploring PostSharp AOP with .NET Core Generic Host.
See [my article on Medium](https://medium.com/dealeron-dev/automating-net-core-services-with-postsharp-and-aspect-oriented-code-a8a51d8d84ec) for a detailed explanation.

The "Semantic" branch is for an article on Exception Handling. It has a much better log handler implementation and a lot of small fixes to the general runtime to support this. An article will be linked here shortly to explain it.

# Requirements

* [PostSharp Essentials](https://www.postsharp.net/essentials) or [Ultimate](https://www.postsharp.net/purchase) version 6.1.18+
* [Visual Studio 2017](https://visualstudio.microsoft.com/vs/older-downloads/) version 15.9.11+
* [.NET Core SDK 2.2.106+](https://dotnet.microsoft.com/download) and C# 7.3+

# Goals

* Demonstrate cross-platform .NET Core services which are threaded for a multi-core computer and wrapped with logging and real-time analytics for Exception Handling over Application Insights.
* Create an example of utilizing PostSharp with the Generic Host in a console app.
* Explore the benefits of Aspect Oriented Programming by masking the boundaries solved through use of PostSharp.
