using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SCP914
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("The chance that 914 spawns in any given round. 0 means never, 1 means always.")]
        public float SpawnChance { get; set; } = 0f;

        [Description("Can 914 move around?")]
        public bool AllowRoaming { get; set; } = true;

        [Description("Should 914's items be dropped when they change roles?")]
        public bool DropItemsOnRoleChange { get; set; } = false;

        [Description("Should 914 be allowed to turn into SCPs (excluding 049-2)?")]
        public bool AllowScpRoleChange { get; set; } = false;

        [Description("Should players (who are not SCPs) have their role changed to 914 upon entering the room?")]
        public bool ChangeOnEnter { get; set; } = false;

        [Description("The minimum number of players needed for 914 to be spawned automatically.")]
        public int MinimumPlayers { get; set; } = 4;
    }
}