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

void Main()
{
	
}

public class PlayersSchema : Schema
{
	public PlayersSchema(IServiceProvider services)
	{
		Query = services.GetService<PlayersQuery>();
		Mutation = services.GetService<PlayersMutation>();
	}
}

public class PlayersQuery : ObjectGraphType<List<Player>>
{
	public PlayersQuery(PlayerRepository repo)
	{
		Field<ListGraphType<PlayerGraphType>>("players").Resolve(ctx => repo.Players);
	}
}

public class PlayersMutation : ObjectGraphType
{
	public PlayersMutation(PlayerRepository repo)
	{
		Field<PlayerGraphType>("playerCreate")
		  .Argument<NonNullGraphType<PlayerInputType>>("player")
		  .Resolve(context =>
		  {
			  var human = context.GetArgument<Player>("player");
			  return repo.PlayerAdd(human);
		  });
	}
}

public class PlayerInputType : InputObjectGraphType
{
	public PlayerInputType()
	{
		Name = "PlayerInput";
		Field<NonNullGraphType<IntGraphType>>("jerseyNumber");
		Field<NonNullGraphType<StringGraphType>>("name");
	}
}

public class Team
{
	public string Name { get; set; }
}

public class TeamGraphType : ObjectGraphType<Team>
{
	public TeamGraphType()
	{
		Field(x => x.Name).Description("team name");
	}
}

public class Player
{
	public int Id { get; set; }
	public int JerseyNumber { get; set; }
	public string Name { get; set; }

	public static List<Player> DEFAULTS = new List<Player> {
		new Player {JerseyNumber = 9, Name = "Tony Romo"},
		new Player {JerseyNumber = 23, Name = "Michael Jordan"}
	};
}

public class PlayerGraphType : ObjectGraphType<Player>
{
	public PlayerGraphType()
	{
		Field(x => x.Id).Description("player id");
		Field(x => x.JerseyNumber).Description("player jersey number");
		Field(x => x.Name).Description("player name");

		AddField(new FieldType
		{
			Name = "team",
			Description = "player team name",
			Type = typeof(TeamGraphType),
			Resolver = new FuncFieldResolver<Team>(context =>
			{
				//context.Dump("context");
				return context.Source switch
				{
					Player p when p.Name == "Michael Jordan" => new Team { Name = "North Carolina" },
					Player p when p.Name == "Tony Romo" => new Team { Name = "Eastern Illinois" },
					Player p when p.Name == "Messi" => new Team { Name = "Argentina" },
					_ => null
				};
			})
		});
	}
}

public class PlayerRepository
{
	private readonly List<Player> _players = new List<Player>();

	public List<Player> Players => _players.ToList();

	public PlayerRepository()
	{
		_players.AddRange(
			Player.DEFAULTS.Select((x, i) =>
			{
				x.Id = i + 1;
				return x;
			}).ToList()
		);
	}

	public Player PlayerAdd(Player player)
	{
		int maxId = _players.Max(x => x.Id);
		player.Id = maxId + 1;
		_players.Add(player);
		return player;
	}
}