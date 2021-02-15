# dotnet-03

This git repo is a kata using .NET Core 3.1 to create a simple REST API that manages data in a SQL Server database. It uses integration tests via TestHost and runs database in a docker container.

### Reference:

- Docker.DotNet ([link](https://github.com/dotnet/Docker.DotNet))
- EF Integration Tests by Geroge Dangl
  - SQL Server Container from Test Fixture ([blog post](https://blog.dangl.me/archive/running-sql-server-integration-tests-in-net-core-projects-via-docker/) | [code sample](https://gist.github.com/GeorgDangl/20ecb3873e78053abfc31c0d0458dfb2#file-dockersqldatabaseutilities-cs))
  - [DatabaseInitializationTestsFixture](https://github.com/GeorgDangl/WebDocu/blob/5709576b0c98da4a31d4f66d7f920e0e21e20d10/test/Dangl.WebDocumentation.Tests/Models/DatabaseInitializationTests.cs)
  - [link 1](https://github.com/GeorgDangl/WebDocu/blob/5709576b0c98da4a31d4f66d7f920e0e21e20d10/test/Dangl.WebDocumentation.Tests/Controllers/AccountControllerTests.cs)
- dotnet-env ([link 1](https://github.com/tonerdo/dotnet-env) | [link 2](https://mattcbaker.com/posts/using-dotenv-files-in-dotnet-core/))