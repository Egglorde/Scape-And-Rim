using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AssemblyParasitesColonThree.MapRelated
{
    public class InfectionVector : IExposable
    {
        // The tile at which
        public int TileID;
        // Idk why its called health in srp, vectors can't die :<
        public int Health;
        // Radius in which the vector can grant points and create incursions
        public float Radius;
        // Whether or not the vector has evolved into an outbreak.
        public bool Outbreak;

        public void ExposeData()
        {
            Scribe_Values.Look(ref TileID, "ScramSRP_TileID");
            Scribe_Values.Look(ref Health, "ScramSRP_VectorHealth", defaultValue: 350);
            Scribe_Values.Look(ref Radius, "ScramSRP_VectorRadius", defaultValue: 1);
        }
        public InfectionVector(int _tileId, int _health, float _radius)
        {
            TileID = _tileId;
            Health = _health;
            Radius = _radius;
        }
        public InfectionVector()
        {
        }
    }
}
