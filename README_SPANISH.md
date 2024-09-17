# AntiVPN
## para TShock5 por [FrankV22](https://github.com/itsFrankV22)

> [!NOTE]  
> If you speak another language, please check [README_ENGLISH.md](https://github.com/itsFrankV22/AntiVPN/blob/main/README.md)

Este plugin impide que los jugadores entren al servidor usando VPN y otros tipos de proxies.

Para cambiar los mensajes de Expulsion de jugadores que usan VPN, modifica la siguiente sección:

```csharp
var result = await message.Result.Task;
if (result == "Y")
{
    Console.WriteLine($"Se ha detectado que {player.Name} está usando un proxy y ha sido expulsado.");
    player.Disconnect("No está permitido el uso de VPN en este servidor.");
}
