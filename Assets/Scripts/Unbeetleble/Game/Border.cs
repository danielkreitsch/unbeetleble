using System.Collections.Generic;
using UnityEngine;

namespace Unbeetleble.Game
{
    public class Border : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField]
        private Transform top;

        [SerializeField]
        private Transform bottom;

        [SerializeField]
        private Transform left;

        [SerializeField]
        private Transform right;

        [Header("Settings")]
        [SerializeField]
        private float minDistanceToCorner;

        [SerializeField]
        private float holeSpacing;

        public List<HoleEntry> HoleEntries { get; } = new List<HoleEntry>();

        void Awake()
        {
            for (float x = -(this.top.localScale.x / 2 - 0.5f - this.minDistanceToCorner); x < this.top.localScale.x / 2 - 0.5f - this.minDistanceToCorner; x += this.holeSpacing)
            {
                this.HoleEntries.Add(new HoleEntry(this.top.position + Vector3.right * x + Vector3.down * 0.5f, Facing.Bottom));
            }

            for (float x = -(this.bottom.localScale.x / 2 - 0.5f - this.minDistanceToCorner); x < this.bottom.localScale.x / 2 - 0.5f - this.minDistanceToCorner; x += this.holeSpacing)
            {
                this.HoleEntries.Add(new HoleEntry(this.bottom.position + Vector3.right * x + Vector3.up * 0.5f, Facing.Top));
            }

            for (float y = -(this.left.localScale.y / 2 - 0.5f - this.minDistanceToCorner); y < this.left.localScale.y / 2 - 0.5f - this.minDistanceToCorner; y += this.holeSpacing)
            {
                this.HoleEntries.Add(new HoleEntry(this.left.position + Vector3.up * y + Vector3.right * 0.5f, Facing.Right));
            }

            for (float y = -(this.right.localScale.y / 2 - 0.5f - this.minDistanceToCorner); y < this.right.localScale.y / 2 - 0.5f - this.minDistanceToCorner; y += this.holeSpacing)
            {
                this.HoleEntries.Add(new HoleEntry(this.right.position + Vector3.up * y + Vector3.left * 0.5f, Facing.Left));
            }
        }

        public HoleEntry GetClosestHoleEntry(params Vector2[] positions)
        {
            HoleEntry closestHoleEntry = null;
            float closestHoleEntryDistance = 0;

            foreach (HoleEntry holeEntry in this.HoleEntries)
            {
                float holeEntryDistance = 0;

                foreach (Vector2 position in positions)
                {
                    holeEntryDistance += Vector2.Distance(position, holeEntry.position);
                }

                if (closestHoleEntry == null || holeEntryDistance < closestHoleEntryDistance)
                {
                    closestHoleEntry = holeEntry;
                    closestHoleEntryDistance = holeEntryDistance;
                }
            }

            return closestHoleEntry;
        }

        public HoleEntry GetFurthestHoleEntry(params Vector2[] positions)
        {
            HoleEntry furthestHoleEntry = null;
            float furthestHoleEntryDistance = 0;

            foreach (HoleEntry holeEntry in this.HoleEntries)
            {
                float holeEntryDistance = 0;

                foreach (Vector2 position in positions)
                {
                    holeEntryDistance += Vector2.Distance(position, holeEntry.position);
                }

                if (furthestHoleEntry == null || holeEntryDistance > furthestHoleEntryDistance)
                {
                    furthestHoleEntry = holeEntry;
                    furthestHoleEntryDistance = holeEntryDistance;
                }
            }

            return furthestHoleEntry;
        }
        
        public enum Facing
        {
            Top,
            Bottom,
            Left,
            Right
        }
    }
}