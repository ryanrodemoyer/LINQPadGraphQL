<Query Kind="Program">
  <NuGetReference>GraphQL</NuGetReference>
  <NuGetReference>GraphQL.MicrosoftDI</NuGetReference>
  <NuGetReference>GraphQL.Server.Transports.AspNetCore</NuGetReference>
  <NuGetReference>GraphQL.Server.Ui.GraphiQL</NuGetReference>
  <NuGetReference>GraphQL.SystemTextJson</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>GraphQL</Namespace>
  <Namespace>GraphQL.Server.Ui.GraphiQL</Namespace>
  <Namespace>GraphQL.SystemTextJson</Namespace>
  <Namespace>GraphQL.Types</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>GraphQL.Resolvers</Namespace>
  <RemoveNamespace>System.Collections</RemoveNamespace>
  <RemoveNamespace>System.Data</RemoveNamespace>
  <RemoveNamespace>System.Diagnostics</RemoveNamespace>
  <RemoveNamespace>System.IO</RemoveNamespace>
  <RemoveNamespace>System.Linq.Expressions</RemoveNamespace>
  <RemoveNamespace>System.Reflection</RemoveNamespace>
  <RemoveNamespace>System.Text</RemoveNamespace>
  <RemoveNamespace>System.Text.RegularExpressions</RemoveNamespace>
  <RemoveNamespace>System.Threading</RemoveNamespace>
  <RemoveNamespace>System.Transactions</RemoveNamespace>
  <RemoveNamespace>System.Xml</RemoveNamespace>
  <RemoveNamespace>System.Xml.Linq</RemoveNamespace>
  <RemoveNamespace>System.Xml.XPath</RemoveNamespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "./graphql_schema_player"

async Task Main()
{
	var builder = WebApplication.CreateBuilder();
	builder.Services
		.AddScoped<PlayersSchema>()
		.AddScoped<PlayersQuery>()
		.AddScoped<PlayersMutation>()
		.AddScoped<PlayerRepository>()
		.AddGraphQL(b => b
			.AddSelfActivatingSchema<PlayersSchema>()
			.AddSystemTextJson()
		);

	var app = builder.Build();
	app.UseDeveloperExceptionPage();

	app.UseGraphQL("/graphql");
	app.UseGraphQLGraphiQL("/",
		new GraphiQLOptions
		{
			GraphQLEndPoint = "/graphql",
		});
	await app.RunAsync();
}
