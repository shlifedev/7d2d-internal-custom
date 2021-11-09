using System;
using System.Threading;

namespace ExampleAssembly
{
    public static class Loader
    {
        public static Config config;
        public static Cheat cheat;
        public static ESP esp;
        public static Objects objects;
        public static Menu menu; 

        public static UnityEngine.GameObject gameObject;

        public static void Load()
        {
            config = new Config();
            config = config.LoadConfig();
            gameObject = new UnityEngine.GameObject();
            cheat = gameObject.AddComponent<Cheat>();
            objects = gameObject.AddComponent<Objects>();
            esp = gameObject.AddComponent<ESP>();
            menu = gameObject.AddComponent<Menu>();
            //gameObject.AddComponent<SceneDebugger>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            Log.Out("Inject!");
        }

        public static void Unload()
        {
            UnityEngine.Object.Destroy(gameObject);
            Thread thread = new Thread(() =>
            {
               
                System.Threading.Thread.Sleep(5000);
                GC.Collect(); 
            });
        }
    }
}
