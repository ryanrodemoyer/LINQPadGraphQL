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
	var sc = new ServiceCollection();
	sc.AddScoped<PlayersQuery>();
	sc.AddScoped<PlayersMutation>();
	sc.AddScoped<PlayerRepository>();

	var schema = new PlayersSchema(sc.BuildServiceProvider());

	// get the initial list of players from our graphql api
	var json = await schema.ExecuteAsync(_ =>
	{
		_.Query = "{ players { id name jerseyNumber team { name } } }";
	});
	json.Dump("query 1");

	// perform a mutation, which we've coded to add one new player to our list
	var dict = new Dictionary<string, object>
	{
		{ "player", new Dictionary<string, object> { {"jerseyNumber", 12}, {"name", "Messi"}, } }
	};
	var json2 = await schema.ExecuteAsync(_ =>
	{
		_.Query = "mutation ($player:PlayerInput!) { playerCreate(player: $player) { id jerseyNumber name } }";
		_.Variables = new Inputs(dict);
	});
	json2.Dump("mutation 1");

	// get the list of players to see our new player has been added
	var json3 = await schema.ExecuteAsync(_ =>
	{
		_.Query = "{ players { id name jerseyNumber team { name } } }";
	});
	json3.Dump("query 2");
}
