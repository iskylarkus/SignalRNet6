using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRNet6.API.Models;

namespace SignalRNet6.API.Hubs
{
    public class MyHub : Hub
    {
        private readonly AppDbContext _context;

        public MyHub(AppDbContext context)
        {
            _context = context;
        }

        private static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;
        public static int TeamCount { get; set; } = 7;

        public async Task SendProduct(Product product)
        {
            await Clients.All.SendAsync("ReceiveProduct", product);
        }

        public async Task SendName(string name)
        {
            if (TeamCount > Names.Count)
            {
                Names.Add(name);

                await Clients.All.SendAsync("ReceiveName", name);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", $"Our team must be no more than {TeamCount} members..!");
            }
        }

        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }



        //Groups
        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task RemoveFromGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task SendNameByGroup(string userName, string teamName)
        {
            var team = _context.Teams.Where(x => x.Name == teamName).FirstOrDefault();

            if (team == null)
            {
                var newTeam = new Team { Name = teamName };

                newTeam.Users.Add(new User { Name = userName });

                _context.Teams.Add(newTeam);
            }
            else
            {
                team.Users.Add(new User { Name = userName });
            }

            await _context.SaveChangesAsync();

            await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup", userName, teamName);
        }

        public async Task GetNamesByGroup()
        {
            var teams = _context.Teams.Include(x => x.Users).Select(x => new
            {
                teamName = x.Name,
                Users = x.Users.ToList()
            }).ToList();

            await Clients.All.SendAsync("ReceiveNamesByGroup", teams);
        }



        public override async Task OnConnectedAsync()
        {
            ClientCount++;

            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;

            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
