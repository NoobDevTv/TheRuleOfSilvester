using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Core.Observation;

namespace TheRuleOfSilvester.Runtime.RoundComponents
{
    class WaitingRoundComponent : IRoundComponent
    {
        public RoundMode Round => RoundMode.Waiting;

        public bool RoundEnd { get; set; }

        private IDisposable subscription;

        public void Start(Game game)
        {
            subscription = game.MultiplayerComponent?
                  .GetNotifications()
                  .Select(n => n.Notification)
                  .Where(x => x.Type == NotificationType.PlayerActions)
                  .Subscribe(x =>
                  {
                      game.CurrentUpdateSets = x
                          .Deserialize(SerializeHelper.DeserializeToList<PlayerAction>)
                          .OrderBy(x => x.Order)
                          .ToObservable();
                      RoundEnd = true;
                  });
        }

        public void Stop(Game game)
        {
            subscription?.Dispose();
        }

        public void Update(Game game)
        {
            if (game.IsMutliplayer)
                return;

            RoundEnd = true;
        }
    }
}
