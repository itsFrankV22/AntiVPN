using System.Net;
using System.Net.Sockets;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AntiVPN;

[ApiVersion(2, 1)]
public class AntiVPN : TerrariaPlugin
{
    public override string Author => "FrankV22";
    public override string Description => "AntiVPN";
    public override string Name => "AntiVPN";
    public override Version Version => new Version(1, 0, 1);

    public AntiVPN(Main game) : base(game) { }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
        }
        base.Dispose(disposing);
    }

    public class Message
    {
        public TSPlayer Player { get; set; }
        public TaskCompletionSource<string> Result { get; set; }

        public async void ThreadMain()
        {
            try
            {
                await Task.Delay(3000);
                var ipAddress = this.Player.IP;
                var apiUrl = $"https://blackbox.ipinfo.app/lookup/{ipAddress}";

                using var client = new HttpClient();
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                this.Result.SetResult(responseContent);
            }
            catch (Exception ex)
            {
                this.Result.SetException(ex);
            }
        }
    }

    private async void OnJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];

        if (this.IsPrivateOrLoopbackAddress(player.IP))
        {
            Console.WriteLine($"{player.Name} is using a private or loopback address, skipping proxy detection.");
            return;
        }
        var message = new Message
        {
            Player = player,
            Result = new TaskCompletionSource<string>()
        };

        var thread = new Thread(message.ThreadMain);
        thread.Start();

        try
        {
            var result = await message.Result.Task;
            if (result == "Y")
            {
                Console.WriteLine($"It has been detected that {player.Name} is using a proxy and has been kicked.");
                player.Disconnect("Using VPN is not allowed on this server.");
            }
            else
            {
                Console.WriteLine($"{player.Name} has a valid IP.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking the IP of {player.Name}: {ex.Message}");
        }
    }

    private bool IsPrivateOrLoopbackAddress(string ipAddress)
    {
        if (!IPAddress.TryParse(ipAddress, out var ip))
        {
            return false;
        }

        // Check if it's a private IP
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            var bytes = ip.GetAddressBytes();
            if ((bytes[0] == 10) || // 10.0.0.0/8
                (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) || // 172.16.0.0/12
                (bytes[0] == 192 && bytes[1] == 168)) // 192.168.0.0/16
            {
                return true;
            }
        }

        return IPAddress.IsLoopback(ip);
    }
}
