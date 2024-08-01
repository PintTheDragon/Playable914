using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp914;
using MapGeneration;
using MEC;
using PlayerRoles;
using Scp914;

namespace SCP914
{
    public class SCP914 : Plugin<Config, Translation>
    {
        public override string Name { get; } = "SCP914";
        public override string Prefix { get; } = "scp_914";
        public override string Author { get; } = "PintTheDragon";
        public override Version Version { get; } = new Version(1, 0, 0);

        private static readonly Random random = new Random();

        public static Player Ply914 = null;
        private static bool _scp914Existed = false;

        private static RoleTypeId _nextRole = RoleTypeId.None;

        private CoroutineHandle _handle;

        public static SCP914 Singleton;

        private static RoomIdentifier Get914()
        {
            return RoomIdentifier.AllRoomIdentifiers.First(findRoom => findRoom.Name == RoomName.Lcz914);
        }

        private static void SendTo914(bool msg, bool force)
        {
            if (Ply914 == null) return;

            var room = Get914();
            if (!InRoom(Ply914, room) || force)
            {
                var pos = room.transform.position;
                pos.y += 0.5f;

                Ply914.Position = pos;
                if(msg) Ply914.Broadcast(2, Singleton.Translation.Scp914SummonMessage);
            }
        }

        public static void Set914(Player new914)
        {
            if (new914 == null) return;
            
            _nextRole = RoleTypeId.None;
            _scp914Existed = true;

            Ply914 = new914;
            
            Ply914.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.None, RoleSpawnFlags.None);
            Ply914.CustomInfo = Singleton.Translation.Scp914RoleName;

            Timing.CallDelayed(0.1f, () =>
            {
                if (Ply914 != new914) return;

                Ply914.Broadcast(
                    30, Singleton.Translation.Scp914SetRoleMessage,
                    Broadcast.BroadcastFlags.Normal, true);
                SendTo914(false, true);
            });
        }

        private static bool InRoom(Player ply, RoomIdentifier room)
        {
            var plyRoom = ply.CurrentRoom;
            if (plyRoom == null) return false;

            return plyRoom.Identifier == room;
        }

        public override void OnEnabled()
        {
            Singleton = this;
            
            _handle = Timing.RunCoroutine(HandlePlayer());

            Exiled.Events.Handlers.Server.RoundStarted += OnGameStart;
            Exiled.Events.Handlers.Player.Hurting += OnDamage;
            Exiled.Events.Handlers.Player.Died += OnDie;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting += OnKnobChange;
            Exiled.Events.Handlers.Scp914.Activating += OnActivate;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += OnUpgradePlayer;
            Exiled.Events.Handlers.Warhead.Detonated += OnBigDestructionInvolvingLcz;
            Exiled.Events.Handlers.Map.Decontaminating += OnDecontamination;
            Exiled.Events.Handlers.Player.Left += OnLeave;
            Exiled.Events.Handlers.Player.InteractingDoor += OnDoorInteract;
            Exiled.Events.Handlers.Player.PickingUpItem += DisallowOutside914;
            Exiled.Events.Handlers.Player.DroppingItem += DisallowOutside914;
            Exiled.Events.Handlers.Player.DroppingAmmo += DisallowOutside914;
            Exiled.Events.Handlers.Warhead.ChangingLeverStatus += DisallowOutside914;
            Exiled.Events.Handlers.Warhead.Starting += DisallowOutside914;
            Exiled.Events.Handlers.Warhead.Stopping += DisallowOutside914;
            Exiled.Events.Handlers.Scp330.InteractingScp330 += DisallowOutside914;
            Exiled.Events.Handlers.Scp330.DroppingScp330 += DisallowOutside914;
            Exiled.Events.Handlers.Player.UsingItem += DisallowOutside914;
            Exiled.Events.Handlers.Player.UnlockingGenerator += DisallowOutside914;
            Exiled.Events.Handlers.Player.ActivatingGenerator += DisallowOutside914;
            Exiled.Events.Handlers.Player.StoppingGenerator += DisallowOutside914;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += OnEnterPocketDimension;
            Exiled.Events.Handlers.Scp096.AddingTarget += OnAdd096Target;
            Exiled.Events.Handlers.Player.ChangingRole += OnRoleChange;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnGameStart;
            Exiled.Events.Handlers.Player.Hurting -= OnDamage;
            Exiled.Events.Handlers.Player.Died -= OnDie;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= OnKnobChange;
            Exiled.Events.Handlers.Scp914.Activating -= OnActivate;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer -= OnUpgradePlayer;
            Exiled.Events.Handlers.Warhead.Detonated -= OnBigDestructionInvolvingLcz;
            Exiled.Events.Handlers.Map.Decontaminating -= OnDecontamination;
            Exiled.Events.Handlers.Player.Left -= OnLeave;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnDoorInteract;
            Exiled.Events.Handlers.Player.PickingUpItem -= DisallowOutside914;
            Exiled.Events.Handlers.Player.DroppingItem -= DisallowOutside914;
            Exiled.Events.Handlers.Player.DroppingAmmo -= DisallowOutside914;
            Exiled.Events.Handlers.Warhead.ChangingLeverStatus -= DisallowOutside914;
            Exiled.Events.Handlers.Warhead.Starting -= DisallowOutside914;
            Exiled.Events.Handlers.Warhead.Stopping -= DisallowOutside914;
            Exiled.Events.Handlers.Scp330.InteractingScp330 -= DisallowOutside914;
            Exiled.Events.Handlers.Scp330.DroppingScp330 -= DisallowOutside914;
            Exiled.Events.Handlers.Player.UsingItem -= DisallowOutside914;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= DisallowOutside914;
            Exiled.Events.Handlers.Player.ActivatingGenerator -= DisallowOutside914;
            Exiled.Events.Handlers.Player.StoppingGenerator -= DisallowOutside914;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= OnEnterPocketDimension;
            Exiled.Events.Handlers.Scp096.AddingTarget -= OnAdd096Target;
            Exiled.Events.Handlers.Player.ChangingRole -= OnRoleChange;

            Timing.KillCoroutines(_handle);

            Singleton = null;
            
            base.OnDisabled();
        }

        private IEnumerator<float> HandlePlayer()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                if (Ply914 != null)
                {
                    if (Ply914.Health <= 0)
                    {
                        Cassie.Message(Singleton.Translation.Scp914TerminationMessage, false, true, true);

                        Ply914.DropItems();
                        Ply914.Role.Set(RoleTypeId.Spectator);
                        Ply914.CustomInfo = "";
                        Ply914 = null;
                        _nextRole = RoleTypeId.None;

                        continue;
                    }

                    if (!Config.AllowRoaming)
                    {
                        SendTo914(true, false);
                    }
                } else if (Config.ChangeOnEnter)
                {
                    // No 914 if player count is too small :(
                    if (Player.List.Count() < Config.MinimumPlayers) continue;
                    
                    // Find a new 914.
                    var room = Get914();
                    var new914 = Player.List.FirstOrDefault(ply => InRoom(ply, room) && ply.Role.Team != Team.SCPs);
                    Set914(new914);
                }
            }
        }

        private void OnGameStart()
        {
            _scp914Existed = false;
            Ply914 = null;

            // No 914 if player count is too small :(
            if (Player.List.Count() < Config.MinimumPlayers) return;
            
            // Check if 914 is allowed to spawn.
            var spawnChance = Config.SpawnChance;
            if (spawnChance <= 0) return;

            if (spawnChance < 1)
            {
                // Roll the dice.
                if (random.NextDouble() > spawnChance) return;
            }

            // Who's the lucky player?
            var players = Player.List.Where(ply => ply.Role.Team != Team.SCPs).ToList();
            Ply914 = players[random.Next(players.Count)];
            _scp914Existed = true;

            Timing.CallDelayed(0.5f, () =>
            {
                if (Ply914 == null) return;

                Set914(Ply914);
            });
        }

        private static void OnDamage(HurtingEventArgs ev)
        {
            if (ev.Player == Ply914)
            {
                ev.IsAllowed = false;
                return;
            }

            if (Ply914 != null && ev.Attacker == Ply914 && !InRoom(Ply914, Get914()))
            {
                ev.Attacker.Broadcast(5, Singleton.Translation.Scp914NotInRoomMessage);

                ev.IsAllowed = false;
                return;
            }
        }

        private static void OnDie(DiedEventArgs ev)
        {
            if (ev.Player == Ply914)
            {
                Cassie.Message(Singleton.Translation.Scp914TerminationMessage, false, true, true);

                Ply914 = null;
                _nextRole = RoleTypeId.None;
            }
        }

        private void OnKnobChange(ChangingKnobSettingEventArgs ev)
        {
            // This event just prevents 914 from clicking the knob.
            if (Ply914 == null) return;

            SendTo914(true, false);

            // 914 cannot change the knob :(
            if (ev.Player == Ply914)
            {
                ev.Player.Broadcast(2, Singleton.Translation.Scp914NeedsHelpMessage);
                
                ev.IsAllowed = false;
                return;
            }

            Ply914.Health -= 0.25f;

            var allowedRole = Config.AllowScpRoleChange || ev.Player.Role.Team != Team.SCPs ||
                            ev.Player.Role == RoleTypeId.Scp0492;
            var validRole = ev.Player.Role.Team != Team.Dead && ev.Player.Role.Team != Team.OtherAlive;

            if (allowedRole && validRole)
            {
                _nextRole = ev.Player.Role;
                if (_nextRole == RoleTypeId.ClassD)
                {
                    _nextRole = RoleTypeId.ChaosConscript;
                }
                else if (_nextRole == RoleTypeId.Scientist)
                {
                    _nextRole = RoleTypeId.NtfPrivate;
                }
            }
        }

        private static void OnActivate(ActivatingEventArgs ev)
        {
            SendTo914(true, false);

            if (ev.Player != Ply914 && _scp914Existed)
            {
                ev.Player.Broadcast(2, Singleton.Translation.PlayerActionNotAllowed);
                
                ev.IsAllowed = false;
                return;
            }

            if (Ply914 != null)
            {
                Ply914.Health -= 0.75f;
            }
        }

        private void OnUpgradePlayer(UpgradingPlayerEventArgs ev)
        {
            if (ev.Player == Ply914 && ev.KnobSetting == Scp914KnobSetting.Rough && _nextRole != RoleTypeId.None)
            {
                Cassie.Message(Singleton.Translation.Scp914EscapeMessage, false, true, true);

                if (Config.DropItemsOnRoleChange)
                {
                    ev.Player.DropItems();
                }

                ev.Player.ReferenceHub.roleManager.ServerSetRole(_nextRole, RoleChangeReason.None, RoleSpawnFlags.None);
                ev.Player.CustomInfo = "";
                
                _nextRole = RoleTypeId.None;
                Ply914 = null;
            }
        }

        private static void OnBigDestructionInvolvingLcz()
        {
            if (Ply914 != null)
            {
                Cassie.Message(Singleton.Translation.Scp914TerminationMessage, false, true, true);

                Ply914.DropItems();
                Ply914.Role.Set(RoleTypeId.Spectator);
                Ply914.CustomInfo = "";
                Ply914 = null;
                _nextRole = RoleTypeId.None;
            }
        }

        private static void OnDecontamination(DecontaminatingEventArgs ev)
        {
            OnBigDestructionInvolvingLcz();
        }

        private static void OnLeave(LeftEventArgs ev)
        {
            if (ev.Player == Ply914)
            {
                Ply914 = null;
                _nextRole = RoleTypeId.None;
                // It's not fair to disable 914 when they leave.
                _scp914Existed = false;
            }
        }

        private static void OnDoorInteract(InteractingDoorEventArgs ev)
        {
            if (ev.Door == null || ev.Door.Room == null) return;

            if (ev.Player == Ply914 && ev.Door.Room.Identifier == Get914())
            {
                ev.IsAllowed = true;
            }
        }

        private static void DisallowOutside914<T>(T ev)
            where T : IPlayerEvent, IDeniableEvent
        {
            if (Ply914 != null && ev.Player == Ply914 && !InRoom(Ply914, Get914()))
            {
                ev.Player.Broadcast(5, Singleton.Translation.Scp914NotInRoomMessage);
                
                ev.IsAllowed = false;
                return;
            }
        }

        private static void OnEnterPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (Ply914 != null && ev.Player == Ply914)
            {
                ev.IsAllowed = false;
                return;
            }
        }

        private static void OnAdd096Target(AddingTargetEventArgs ev)
        {
            if (Ply914 != null && ev.Target == Ply914)
            {
                ev.IsAllowed = false;
                return;
            }
        }
        
        /*[PluginEvent(ServerEventType.Scp173NewObserver)]
        public bool OnAdd173Observer(Scp173NewObserverEvent ev)
        {
            if (player != null && !player.IsOffline && ev.Target == player)
            {
                return false;
            }

            return true;
        }*/

        private static void OnRoleChange(ChangingRoleEventArgs ev)
        {
            if (Ply914 != null && ev.Player == Ply914 && ev.NewRole != RoleTypeId.Tutorial)
            {
                // Ensure that 914 role is cleared.
                ev.Player.CustomInfo = "";
                Ply914 = null;
            }
        }
    }
}