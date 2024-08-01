using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCP914
{
    public class Translation : ITranslation
    {
        [Description("The command used to set a player to SCP-914.")]
        public string Set914Command { get; set; } = "set914";

        [Description("The description of the set 914 command.")]
        public string Set914CommandDescription { get; set; } = "Set the 914 player.";

        [Description("The text to display when a user doesn't have permissions to execute a command.")]
        public string NoPermissionsMessage { get; set; } = "No perms :(";

        [Description("The text shown to SCP-914 when another player takes their place.")]
        public string Scp914ChangedMessage { get; set; } = "Sorry buddy, there's a new 914 in town!";

        [Description("The message to show when a command succeeds.")]
        public string CommandSuccess { get; set; } = "Success!";

        [Description("The message SCP-914 will see when they get brought back to the 914 room.")]
        public string Scp914SummonMessage { get; set; } = "<color=red>YOU HAVE BEEN SUMMONED!</color>";

        [Description("The name of the SCP-914 role.")]
        public string Scp914RoleName { get; set; } = "SCP-914";

        [Description("The message to send to SCP-914 when they are first set to the role.")]
        public string Scp914SetRoleMessage { get; set; } =
            "You are SCP-914. You're the only one who can activate the machine. If you're tired of playing, go into the machine on rough and your role will change.";

        [Description("The (cassie-friendly) message to send when SCP-914 is terminated.")]
        public string Scp914TerminationMessage { get; set; } = "SCP 9 14 HAS BEEN TERMINATED";

        [Description("The (cassie-friendly) message to send when SCP-914 escapes.")]
        public string Scp914EscapeMessage { get; set; } = "SCP 9 14 HAS ESCAPED";

        [Description(
            "The message to send to SCP-914 when they try to perform an action that only works inside their room.")]
        public string Scp914NotInRoomMessage { get; set; } = "You need to be inside of 914 to do that!";

        [Description("The message send to SCP-914 when they try to perform an action that they can't do on their own.")]
        public string Scp914NeedsHelpMessage { get; set; } = "Sorry, you need help to do this.";

        [Description(
            "The message sent to a player when they try to perform an action that only SCP-914 is allowed to do.")]
        public string PlayerActionNotAllowed { get; set; } = "Nuh uh";
    }
}