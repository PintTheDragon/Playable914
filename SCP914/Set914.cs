using System;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using RemoteAdmin;

namespace SCP914
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Set914 : ICommand
    {
        public string Command => SCP914.Singleton.Translation.Set914Command;
        public string[] Aliases { get; } = new string[] { };
        public string Description => SCP914.Singleton.Translation.Set914CommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var selectedPlayer = Player.Get(Utils.RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out _)[0]);

            bool hasPerms;
            if (sender is PlayerCommandSender ply)
            {
                hasPerms = ply.CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions) ||
                           (selectedPlayer == Player.Get(ply) && ply.CheckPermission(PlayerPermissions.ForceclassSelf));
            }
            else
            {
                // The console can always send commands.
                hasPerms = true;
            }

            if (!hasPerms)
            {
                response = SCP914.Singleton.Translation.NoPermissionsMessage;
                return false;
            }

            if (SCP914.Ply914 != null)
            {
                SCP914.Ply914.Broadcast(5, SCP914.Singleton.Translation.Scp914ChangedMessage);
                SCP914.Ply914.Role.Set(RoleTypeId.Spectator);
                SCP914.Ply914.CustomInfo = "";
            }

            SCP914.Set914(selectedPlayer);

            response = SCP914.Singleton.Translation.CommandSuccess;
            return true;
        }
    }
}