using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExampleAssembly {
    public class Objects : MonoBehaviour {
        
        private float lastCachePlayer;
        private float lastCacheZombies;
        private float lastCacheChunks;

        public static List<EntityPlayer> PlayerList {
            get {
                if (GameManager.Instance != null)
                    if (GameManager.Instance.World != null)
                        return GameManager.Instance.World.GetPlayers();
                return new List<EntityPlayer>();
            }
        }

        public static EntityPlayerLocal localPlayer;
        public static List<EntityEnemy> zombieList;
        public static List<ChunkGameObject> chunkList;  

        private void Start() {
            zombieList = new List<EntityEnemy>();
            chunkList = new List<ChunkGameObject>(); 
            lastCachePlayer = Time.time + 5f;
            lastCacheZombies = Time.time + 3f;
            lastCacheChunks = Time.time + 4f;
        }

   
        private void Update() {
            /* 
             * Only a little bit of spaghetti : ^)
             * This is much more efficient than Coroutines are, which is why I'm using this spaghetti over them.
             * I could and should have made a helper util which lets you plug in an entity list and it will fetch it every x seconds,
             * but this will do just fine.
             */

            if (Time.time >= lastCachePlayer) {
                localPlayer = FindObjectOfType<EntityPlayerLocal>();

                lastCachePlayer = Time.time + 5f;
            } else if (Time.time >= lastCacheZombies) {
                zombieList = FindObjectsOfType<EntityEnemy>().ToList(); 
                lastCacheZombies = Time.time + 3f;
            } else if (Time.time >= lastCacheChunks) {
                chunkList = FindObjectsOfType<ChunkGameObject>().ToList(); 
                lastCacheChunks = Time.time + 4f;
            }
             
        }

 
    }
}
