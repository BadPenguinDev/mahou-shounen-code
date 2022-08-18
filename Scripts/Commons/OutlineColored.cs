using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/Outline", 15)]
    public class OutlineColored : Shadow
    {
        public bool diagonalOption;

        protected OutlineColored()
        { }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) { return; }

            List<UIVertex> verts = new List<UIVertex>();
            vh.GetUIVertexStream(verts);

            for (int i = 0; i <= verts.Count - 1; i++)
            {
                UIVertex uiVertex = verts[i];
                uiVertex.color = effectColor;
            }

            var neededCapacity = verts.Count * 5;
            if (verts.Capacity < neededCapacity)
                verts.Capacity = neededCapacity;


            var start = 0;
            var end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, 0, effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, 0, -effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, 0);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, -effectDistance.x, 0);

            if (diagonalOption == true)
            {
                start = end;
                end = verts.Count;
                ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, effectDistance.y);

                start = end;
                end = verts.Count;
                ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, -effectDistance.y);

                start = end;
                end = verts.Count;
                ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, -effectDistance.x, effectDistance.y);

                start = end;
                end = verts.Count;
                ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, -effectDistance.x, -effectDistance.y);
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
            ListPool<UIVertex>.Release(verts);
        }
    }
}