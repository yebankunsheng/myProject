﻿using QFramework;

namespace PlatformShoot
{
    public interface IGameModel : IModel
    {
        BindableProperty<int> Score { get; }
    }

    public class GameModel : AbstractModel, IGameModel
    {
        BindableProperty<int> IGameModel.Score { get; } = new BindableProperty<int>(0);

        protected override void OnInit()
        {
            
        }
    }
}
