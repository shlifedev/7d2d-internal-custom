using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ExampleAssembly
{
    public class Config
    {  
     
    
        bool aimbot;
        bool infiniteAmmo;
        bool noWeaponBob;

        public bool Aimbot
        {
            get => aimbot;
            set => aimbot = value;
        }

        public bool InfiniteAmmo
        {
            get => infiniteAmmo;
            set => infiniteAmmo = value;
        }

        public bool NoWeaponBob
        {
            get => noWeaponBob;
            set => noWeaponBob = value;
        }

        public bool MagicBullet
        {
            get => magicBullet;
            set => magicBullet = value;
        }

        public bool Chams
        {
            get => chams;
            set => chams = value;
        }

        [JsonIgnore]
        public bool Speed
        {
            get => speed;
            set => speed = value;
        }

        public bool CrossHair
        {
            get => crossHair;
            set => crossHair = value;
        }

        public bool DrawFox
        {
            get => drawFox;
            set => drawFox = value;
        }

        public bool PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        public bool PlayerHealth
        {
            get => playerHealth;
            set => playerHealth = value;
        }

        public bool ZombieBox
        {
            get => zombieBox;
            set => zombieBox = value;
        }

        public bool PlayerBox
        {
            get => playerBox;
            set => playerBox = value;
        }

        public bool ZombieNAme
        {
            get => zombieNAme;
            set => zombieNAme = value;
        }

        public bool ZombieHealth
        {
            get => zombieHealth;
            set => zombieHealth = value;
        }

        public bool PlayerCornerBox
        {
            get => playerCornerBox;
            set => playerCornerBox = value;
        }

        public bool ZombieCornerBox
        {
            get => zombieCornerBox;
            set => zombieCornerBox = value;
        }

        bool magicBullet; 
        bool chams; 
        bool speed; 
        private bool crossHair; 
        private bool drawFox; 
        private bool playerName; 
        private bool playerHealth;
        private bool zombieBox; 
        private bool playerBox;
        private bool zombieNAme;
        private bool zombieHealth;
        private bool playerCornerBox;
        private bool zombieCornerBox;
        
        
        public Config LoadConfig()
        {
            FileInfo info = new FileInfo("cheatconfig.json");
            if (info.Exists)
            {
                var str = System.IO.File.ReadAllText("cheatconfig.json");
                var config = JsonConvert.DeserializeObject<Config>(str);

                return config;
            }
            else
            {
                return new Config();
            }
        }

        public void SaveConfig()
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText("cheatconfig.json", str, Encoding.UTF8);
        }
    }
}
