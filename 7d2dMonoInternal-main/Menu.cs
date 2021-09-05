using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using O = ExampleAssembly.Objects;

namespace ExampleAssembly
{
    public class Menu : MonoBehaviour
    {
        private void Start()
        {
            windowID = new System.Random(Environment.TickCount).Next(1000, 65535);
            windowRect = new Rect(5f, 5f, 300f, 150f);
        }

        private void Update()
        {
            if (!Input.anyKey || !Input.anyKeyDown)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Insert))
            {
                drawMenu = !drawMenu;
            }
        }

        private void OnGUI()
        {
            if (drawMenu)
            {
                windowRect = GUILayout.Window(windowID, windowRect, Window, "메뉴");
            }
        }

        private void ToggleCmDm()
        {
            GameStats.Set(EnumGameStats.ShowSpawnWindow, cmDm);
            GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, cmDm);
            GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, cmDm);
        }

       
        private void Window(int windowID)
        {  
            Loader.config.Speed = GUILayout.Toggle(Loader.config.Speed, "스피드핵 활성화"); 
            if (GUILayout.Button("신 모드"))
            {
                cmDm = !cmDm;

                ToggleCmDm();
            }

            if (GUILayout.Button("레벨업"))
            {
                if (O.localPlayer)
                {
                    Progression prog = O.localPlayer.Progression;
                    prog.AddLevelExp(prog.ExpToNextLevel);
                }
            }

            if (GUILayout.Button("스킬포인트 1 추가"))
            {
                if (O.localPlayer)
                {
                    Progression prog = O.localPlayer.Progression;
                    prog.SkillPoints += 1;
                }
            }
            if (GUILayout.Button("스킬포인트 1 제거"))
            {
                if (O.localPlayer)
                {
                    Progression prog = O.localPlayer.Progression;
                    prog.SkillPoints -= 1;
                }
            }

            GUILayout.BeginVertical("옵션", GUI.skin.box);
            {
                GUILayout.Space(20f);

                GUILayout.BeginHorizontal();
                {
                    Loader.config.MagicBullet = GUILayout.Toggle(Loader.config.MagicBullet, "즉사에임봇");
                    Loader.config.Chams = GUILayout.Toggle(Loader.config.Chams, "월핵");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    Loader.config.NoWeaponBob = GUILayout.Toggle(Loader.config.NoWeaponBob, "No Weapon Bob");
                    Loader.config.Aimbot = GUILayout.Toggle(Loader.config.Aimbot, "에임봇");
                }
                GUILayout.EndHorizontal();


                GUILayout.Label($"에임봇 Smooth = {Cheat.smooth}");
                Cheat.smooth = GUILayout.HorizontalSlider(Cheat.smooth, 0, 10);






                GUILayout.BeginHorizontal();
                {
                    ESP.crosshair = GUILayout.Toggle(ESP.crosshair, "Crosshair");
                    ESP.fovCircle = GUILayout.Toggle(ESP.fovCircle, "Draw FOV");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    ESP.playerBox = GUILayout.Toggle(ESP.playerBox, "Player Box");
                    ESP.playerName = GUILayout.Toggle(ESP.playerName, "Player Name");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    ESP.playerHealth = GUILayout.Toggle(ESP.playerHealth, "Player Health");
                    ESP.zombieBox = GUILayout.Toggle(ESP.zombieBox, "Zombie Box");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    ESP.zombieName = GUILayout.Toggle(ESP.zombieName, "Zombie Name");
                    ESP.zombieHealth = GUILayout.Toggle(ESP.zombieHealth, "Zombie Health");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    ESP.playerCornerBox = GUILayout.Toggle(ESP.playerCornerBox, "Player Corner Box");
                    ESP.zombieCornerBox = GUILayout.Toggle(ESP.zombieCornerBox, "Zombie Corner Box");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Teleport", GUI.skin.box);
            {
                GUILayout.Space(20f);

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MaxWidth(300f));
                {
                    if (O.PlayerList.Count > 1)
                    {
                        foreach (EntityPlayer player in O.PlayerList)
                        {
                            if (!player || player == O.localPlayer || !player.IsAlive())
                            {
                                continue;
                            }

                            if (GUILayout.Button(player.EntityName))
                            {
                                O.localPlayer.TeleportToPosition(player.GetPosition());
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label("No players found.");
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private string MakeEnable(string label, bool toggle)
        {
            string status = toggle ? "<color=green>ON</color>" : "<color=red>OFF</color>";
            return $"{label} {status}";
        }

        private bool drawMenu = true;
        private bool cmDm;

        private int windowID;
        private Rect windowRect;
        private Vector2 scrollPosition;
    }
}
