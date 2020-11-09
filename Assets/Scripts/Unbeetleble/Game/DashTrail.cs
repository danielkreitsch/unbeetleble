using UnityEngine;
using Utility;

namespace Unbeetleble.Game
{
    public class DashTrail : MonoBehaviour
    {
        public void Remove()
        {
            this.Invoke(() => Object.Destroy(this.gameObject), 2f);
        }
    }
}