using UnityEngine;
using Zenject;

namespace Unbeetleble
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField]
        private MusicController musicController;

        public override void InstallBindings()
        {
            this.Container.Bind<MusicController>().FromInstance(this.musicController);
        }
    }
}