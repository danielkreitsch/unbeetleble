using UnityEngine;
using Zenject;

namespace Unbeetleble.Game
{
    public class GameSceneInstaller : MonoInstaller
    {
        public GameController gameController;
        public new Camera camera;
        public Player player;
        public Enemy enemy;

        public override void InstallBindings()
        {
            this.Container.BindInstances(
                this.gameController,
                this.camera,
                this.player,
                this.enemy
            );
        }
    }
}