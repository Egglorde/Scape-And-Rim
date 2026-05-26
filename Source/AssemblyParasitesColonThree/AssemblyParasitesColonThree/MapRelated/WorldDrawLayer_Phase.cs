using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AssemblyParasitesColonThree.MapRelated
{
    public class WorldDrawLayer_Phase : WorldDrawLayer
    {
        private const int TilesPerRegion = 100;
        private readonly List<Vector3> verts = new List<Vector3>();
        private readonly Dictionary<int, List<LayerSubMesh>> subMeshesByRegion = new Dictionary<int, List<LayerSubMesh>>();
        private readonly Queue<int> regionsToRegenerate = new Queue<int>();
        private Dictionary<int, Material> phaseMats = new Dictionary<int, Material>();
        private Material MakeMaterialForPhase(Shader shader)
        {
            MaterialRequest matReq = new MaterialRequest(ContentFinder<Texture2D>.Get("Dirt-Infested"), shader);
            matReq.secondaryTex = ContentFinder<Texture2D>.Get("Noise3");
            matReq.renderQueue = 3590;
            return MaterialPool.MatFrom(matReq);
        }
        private Material FetchPhaseMat(int points)
        {
            int phase = ParasiteUtils.phase(points);
            if (phase <= 0 && points > 0) phase = 1;
            if (phaseMats.ContainsKey(phase))
            {
                return phaseMats[phase];
            }
            Shader phaseShader;
            switch(phase)
            {
                case > 10:
                    phaseShader = ShaderDatabase.LoadShader("Phase6");
                    break;
                case > 8:
                    phaseShader = ShaderDatabase.LoadShader("Phase5");
                    break;
                case > 6:
                    phaseShader = ShaderDatabase.LoadShader("Phase4");
                    break;
                case > 4:
                    phaseShader = ShaderDatabase.LoadShader("Phase3");
                    break;
                case > 2:
                    phaseShader = ShaderDatabase.LoadShader("Phase2");
                    break;
                case > 0:
                    phaseShader = ShaderDatabase.LoadShader("Phase1");
                    break;
                default:
                    return null;
            };
            return MakeMaterialForPhase(phaseShader);
        }
        private int FetchRegion(int TileId)
        {
            return Mathf.FloorToInt((float)TileId / (float)TilesPerRegion);
        }
        private List<LayerSubMesh> FetchSubmeshesByRegion(int RegionId)
        {
            if (!subMeshesByRegion.ContainsKey(RegionId))
            {
                subMeshesByRegion[RegionId] = new List<LayerSubMesh>();
            }
            return subMeshesByRegion[RegionId];
        }
        private LayerSubMesh SubMeshFromRegionAndPhase(int RegionId, Material PhaseMaterial)
        {
            List<LayerSubMesh> subMeshesForRegion = FetchSubmeshesByRegion(RegionId);
            for (int i = 0; i < subMeshesForRegion.Count; i++)
            {
                if (subMeshesForRegion[i].material == PhaseMaterial)
                {
                    return subMeshesForRegion[i];
                }
            }
            Mesh mesh = new Mesh();
            if (UnityData.isEditor)
            {
                mesh.name = "WorldLayerSubMesh_" + GetType().Name + "_" + Find.World.info.seedString;
            }
            LayerSubMesh layerSubMesh = new LayerSubMesh(mesh, PhaseMaterial);
            subMeshesForRegion.Add(layerSubMesh);
            subMeshes.Add(layerSubMesh);
            return layerSubMesh;
        }
        private bool TryAddMeshForTile(PlanetTile tileId)
        {
            int infPoints = Find.World.GetComponent<WorldComp_UbiquitousDevelopment>().tileInfection[tileId.tileId];
            Material phaseMaterial = FetchPhaseMat(infPoints);
            if (phaseMaterial == null)
            {
                return false;
            }
            int infByRegion = FetchRegion(tileId);
            LayerSubMesh subMeshForMaterialAndRegion = SubMeshFromRegionAndPhase(infByRegion, phaseMaterial);
            Find.WorldGrid.GetTileVertices(tileId, verts);
            int count = subMeshForMaterialAndRegion.verts.Count;
            int num2 = 0;
            for (int count2 = verts.Count; num2 < count2; num2++)
            {
                Vector3 vector = verts[num2] + verts[num2].normalized * 0.02f;
                subMeshForMaterialAndRegion.verts.Add(vector);
                subMeshForMaterialAndRegion.uvs.Add(vector * 0.1f);
                if (num2 < count2 - 2)
                {
                    subMeshForMaterialAndRegion.tris.Add(count + num2 + 2);
                    subMeshForMaterialAndRegion.tris.Add(count + num2 + 1);
                    subMeshForMaterialAndRegion.tris.Add(count);
                }
            }
            return true;
        }
        public void Notify_TileInfectionChanged(int tileId)
        {
            int regionIdForTile = FetchRegion(tileId);
            if (!regionsToRegenerate.Contains(regionIdForTile))
            {
                regionsToRegenerate.Enqueue(regionIdForTile);
            }
        }
        private void RegenRegion(int regionId)
        {
            List<LayerSubMesh> subMeshesForRegion = FetchSubmeshesByRegion(regionId);
            for (int i = 0; i < subMeshesForRegion.Count; i++)
            {
                subMeshesForRegion[i].Clear(MeshParts.All);
            }
            int num = regionId * TilesPerRegion;
            int num2 = num + TilesPerRegion;
            for (int j = num; j < num2; j++)
            {
                PlanetTile tile = new PlanetTile(j, planetLayer);
                if (!Find.World.grid.InBounds(tile))
                {
                    break;
                }
                TryAddMeshForTile(tile);
            }
            for (int k = 0; k < subMeshesForRegion.Count; k++)
            {
                if (subMeshesForRegion[k].verts.Count > 0)
                {
                    subMeshesForRegion[k].FinalizeMesh(MeshParts.All);
                }
            }
        }
        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
            {
                yield return item;
            }
            int phaseMeshesPrinted = 0;
            verts.Clear();
            subMeshesByRegion.Clear();
            regionsToRegenerate.Clear();
            for (int i = 0; i < planetLayer.TilesCount; i++)
            {
                if (TryAddMeshForTile(planetLayer.PlanetTileForID(i)))
                {
                    phaseMeshesPrinted++;
                    if (phaseMeshesPrinted % 1000 == 0)
                    {
                        yield return null;
                    }
                }
            }
            FinalizeMesh(MeshParts.All);
        }
        public override void Render()
        {
            if (regionsToRegenerate.Count > 0)
            {
                int regionId = regionsToRegenerate.Dequeue();
                RegenRegion(regionId);
            }
            base.Render();
        }
    }
}
