using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using O = ExampleAssembly.Objects;

namespace ExampleAssembly {
    public class Menu : MonoBehaviour {
        private void Start() {
            windowID = new System.Random(Environment.TickCount).Next(1000, 65535);
            windowRect = new Rect(5f, 5f, 300f, 150f);
        }

        private void Update() {
            if (!Input.anyKey || !Input.anyKeyDown) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Insert)) {
                drawMenu = !drawMenu;
            }
        }

        private void OnGUI() {
            if (drawMenu) {
                windowRect = GUILayout.Window(windowID, windowRect, Window, "메뉴");
            }
        }

        private void ToggleCmDm() {
            GameStats.Set(EnumGameStats.ShowSpawnWindow, cmDm);
            GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, cmDm);
            GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, cmDm);
        }

        private void Window(int windowID) {
            GUILayout.Label(MakeEnable("[MouseX3] 으로 스피드핵 사용", Loader.config.Speed));
            GUILayout.Label("총알무한");
            
            if (GUILayout.Button("치트모드 활성화")) {
                cmDm = !cmDm;

                ToggleCmDm();
            }

            if (GUILayout.Button("레벨 업")) {
                if (O.localPlayer) {
                    Progression prog = O.localPlayer.Progression;
                    prog.AddLevelExp(prog.ExpToNextLevel);
                }
            }

            if (GUILayout.Button("스킬포인트 1 추가")) {
                if (O.localPlayer) {
                    Progression prog = O.localPlayer.Progression;
                    prog.SkillPoints += 1;
                }
            }
            if (GUILayout.Button("스킬포인트 1 감소")) {
                if (O.localPlayer) {
                    Progression prog = O.localPlayer.Progression;
                    prog.SkillPoints += 1;
                }
            }

            GUILayout.BeginVertical("옵션", GUI.skin.box); {
                GUILayout.Space(20f);

                GUILayout.BeginHorizontal();
                {
                    Loader.config.MagicBullet = GUILayout.Toggle(Loader.config.MagicBullet, "Fov즉사");
                    Loader.config.Chams = GUILayout.Toggle(Loader.config.Chams, "월핵");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    Loader.config.NoWeaponBob = GUILayout.Toggle(Loader.config.NoWeaponBob, "Rig길이");
                    Loader.config.Aimbot= GUILayout.Toggle(Loader.config.Aimbot, "에임봇");
                }
                GUILayout.EndHorizontal();

                GUILayout.Label($"에임봇 Smooth = {Cheat.smooth}");
                Cheat.smooth = GUILayout.HorizontalSlider(Cheat.smooth, 0, 10);
             
            
        }

        private string MakeEnable(string label, bool toggle) {
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
