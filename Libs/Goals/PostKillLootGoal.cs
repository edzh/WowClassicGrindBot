﻿using Libs.GOAP;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Libs.Goals
{
    public class PostKillLootGoal : LootGoal
    {
        public PostKillLootGoal(WowProcess wowProcess, PlayerReader playerReader, BagReader bagReader, StopMoving stopMoving, ILogger logger, ClassConfiguration classConfiguration)
            : base(wowProcess, playerReader, bagReader, stopMoving, logger, classConfiguration)
        {
        }

        public override void AddPreconditions()
        {
            AddPrecondition(GoapKey.incombat, false);
            AddPrecondition(GoapKey.hastarget, false);
            AddPrecondition(GoapKey.shouldloot, true);
        }

        public override float CostOfPerformingAction { get => 5f; }

        public override async Task PerformAction()
        {
            SendActionEvent(new ActionEventArgs(GoapKey.shouldloot, false));
            await base.PerformAction();
        }
    }
}