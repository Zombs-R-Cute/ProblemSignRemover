using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;

namespace Zombs_R_Cute_ProblemSignRemoval
{
    public class RemoveSignsCommand : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, "/removesigns text | SteamID");
                return;
            }
            
            //Use the list to remove items since removing while iterating will crash
            List<BarricadeDrop> toRemove = new List<BarricadeDrop>();

            var is_steamid = ulong.TryParse(command[0], out ulong steamid);
            foreach (var barricadeRegion in BarricadeManager.regions)
            {
                foreach (var drop in barricadeRegion.drops)
                {
                    if (!(drop.interactable is InteractableSign))
                        continue;

                    if (is_steamid && steamid != drop.GetServersideData().owner)
                        continue;

                    var interactableSign = drop.interactable as InteractableSign;
                    if (!is_steamid && !interactableSign.text.Contains(command[0]))
                        continue;

                    toRemove.Add(drop);
                }
            }

            UnturnedChat.Say(caller, $"{toRemove.Count} signs removed");
            foreach (var drop in toRemove)
            {
                BarricadeManager.damage(drop.model, 65000, 65000, true,
                    (CSteamID)drop.GetServersideData().owner,
                    EDamageOrigin.Charge_Explosion);
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "removesigns";
        public string Help => "Removes the signs with SteamId or text";
        public string Syntax => "/removesign SteamId or Text";
        public List<string> Aliases => new List<string>() { "rs" };
        public List<string> Permissions => new List<string>() { "zombs.removesigns" };
    }
}