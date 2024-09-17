# AntiVPN
## for TShock5 by [FrankV22](https://github.com/itsFrankV22)

> [!NOTE]
> Si hablas otro idioma porfa revisa [README_SPANISH.md](https://github.com/itsFrankV22/AntiVPN/blob/main/README_SPANISH.md)

This Plugin prevents players from entering the server with VPN and other types of proxies

To change the player ban messages when using VPN, modify the following section

```var result = await message.Result.Task;
if (result == "Y")
{
    Console.WriteLine($"It has been detected that {player.Name} is using a proxy and has been kicked.");
    player.Disconnect("Using VPN is not allowed on this server.");
}
