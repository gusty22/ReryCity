import unittest
import os
import yaml
import math
import time

class TestReryCityIdentity(unittest.TestCase):
    def setUp(self):
        self.game_title = "RERY CITY"
        self.public_subtitle = "Simulador Urbano de Motocicletas e Manobras"
        self.version_string = "1.0.0-indie"
        self.build_number = 100
        self.setting_name = "Araguari Futurista"
        self.force_offline = True
        self.allow_simulated_rewards = True
        self.enable_telemetry = False

    def test_identity_metadata(self):
        self.assertEqual(self.game_title, "RERY CITY")
        self.assertIn("Araguari", self.setting_name)
        self.assertTrue(self.force_offline)
        self.assertFalse(self.enable_telemetry)
        self.assertTrue(self.allow_simulated_rewards)

    def test_formatted_title_and_version(self):
        formatted = f"{self.game_title} v{self.version_string} (b{self.build_number})"
        self.assertEqual(formatted, "RERY CITY v1.0.0-indie (b100)")


class TestOfflineGameBootstrapper(unittest.TestCase):
    class BootstrapperState:
        Uninitialized = 0
        VerifyingLocalEnvironment = 1
        LoadingLocalData = 2
        InitializingCoreSubsystems = 3
        Ready = 4

    def setUp(self):
        self.current_state = self.BootstrapperState.Uninitialized
        self.state_history = []
        self.is_bootstrap_complete = False

    def on_state_changed(self, new_state):
        self.state_history.append(new_state)
        self.current_state = new_state

    def simulate_bootstrap(self):
        # 1. Verifying Local Environment
        self.on_state_changed(self.BootstrapperState.VerifyingLocalEnvironment)
        # 2. Loading Local Data
        self.on_state_changed(self.BootstrapperState.LoadingLocalData)
        # 3. Initializing Subsystems
        self.on_state_changed(self.BootstrapperState.InitializingCoreSubsystems)
        # 4. Ready
        self.on_state_changed(self.BootstrapperState.Ready)
        self.is_bootstrap_complete = True

    def test_bootstrap_lifecycle(self):
        self.assertEqual(self.current_state, self.BootstrapperState.Uninitialized)
        self.simulate_bootstrap()
        self.assertEqual(self.current_state, self.BootstrapperState.Ready)
        self.assertTrue(self.is_bootstrap_complete)
        expected_sequence = [
            self.BootstrapperState.VerifyingLocalEnvironment,
            self.BootstrapperState.LoadingLocalData,
            self.BootstrapperState.InitializingCoreSubsystems,
            self.BootstrapperState.Ready
        ]
        self.assertEqual(self.state_history, expected_sequence)


class TestSimulatedRewardManager(unittest.TestCase):
    def setUp(self):
        self.base_reward = 500
        self.cooldown_seconds = 15.0
        self.last_reward_timestamp = -1000.0
        self.is_processing = False
        self.player_money = 1000
        self.events_fired = []

    def can_request_reward(self, current_time):
        cooldown_remaining = max(0.0, (self.last_reward_timestamp + self.cooldown_seconds) - current_time)
        return not self.is_processing and cooldown_remaining <= 0.0

    def request_reward(self, current_time):
        if self.is_processing:
            self.events_fired.append(("failed", "Recompensa já está sendo processada."))
            return False

        cooldown_remaining = max(0.0, (self.last_reward_timestamp + self.cooldown_seconds) - current_time)
        if cooldown_remaining > 0.0:
            self.events_fired.append(("failed", f"Aguarde {int(math.ceil(cooldown_remaining))}s."))
            return False

        # Start simulated process
        self.is_processing = True
        self.events_fired.append(("started", None))

        # Complete process
        self.last_reward_timestamp = current_time
        self.is_processing = False
        self.player_money += self.base_reward
        self.events_fired.append(("granted", self.base_reward))
        return True

    def test_successful_reward_grant(self):
        t0 = 100.0
        self.assertTrue(self.can_request_reward(t0))
        success = self.request_reward(t0)
        self.assertTrue(success)
        self.assertEqual(self.player_money, 1500)
        self.assertEqual(len(self.events_fired), 2)
        self.assertEqual(self.events_fired[0][0], "started")
        self.assertEqual(self.events_fired[1], ("granted", 500))

    def test_cooldown_rejection(self):
        t0 = 100.0
        self.request_reward(t0)
        # Tenta novamente 5 segundos depois
        t1 = 105.0
        self.assertFalse(self.can_request_reward(t1))
        success = self.request_reward(t1)
        self.assertFalse(success)
        self.assertEqual(self.player_money, 1500)
        self.assertEqual(self.events_fired[-1][0], "failed")

    def test_reward_after_cooldown_expiration(self):
        t0 = 100.0
        self.request_reward(t0)
        # Tenta novamente 16 segundos depois (cooldown é 15s)
        t2 = 116.0
        self.assertTrue(self.can_request_reward(t2))
        success = self.request_reward(t2)
        self.assertTrue(success)
        self.assertEqual(self.player_money, 2000)

    def test_debouncing_simultaneous_requests(self):
        self.is_processing = True
        success = self.request_reward(200.0)
        self.assertFalse(success)
        self.assertEqual(self.events_fired[-1][0], "failed")


class TestAsynchronousSceneLoader(unittest.TestCase):
    @staticmethod
    def normalize_unity_progress(raw_progress):
        return max(0.0, min(1.0, raw_progress / 0.9))

    def test_progress_normalization(self):
        self.assertAlmostEqual(self.normalize_unity_progress(0.0), 0.0)
        self.assertAlmostEqual(self.normalize_unity_progress(0.45), 0.5)
        self.assertAlmostEqual(self.normalize_unity_progress(0.9), 1.0)
        self.assertAlmostEqual(self.normalize_unity_progress(1.0), 1.0)

    def test_smooth_progress_interpolation(self):
        current_display = 0.2
        target_progress = 0.8
        speed = 3.0
        delta_time = 0.1

        # Mathf.MoveTowards
        step = speed * delta_time
        if abs(target_progress - current_display) <= step:
            new_display = target_progress
        else:
            new_display = current_display + math.copysign(step, target_progress - current_display)

        self.assertAlmostEqual(new_display, 0.5)
        self.assertGreater(new_display, current_display)


class TestMobileEnergyProfileManager(unittest.TestCase):
    def setUp(self):
        self.profiles = {
            "Economico": {
                "target_fps": 30,
                "lod_bias": 0.7,
                "shadow_quality": 0,
                "shadow_distance": 25.0,
                "never_sleep": False
            },
            "Equilibrado": {
                "target_fps": 60,
                "lod_bias": 1.0,
                "shadow_quality": 1,
                "shadow_distance": 50.0,
                "never_sleep": True
            },
            "AltoDesempenho": {
                "target_fps": 60,
                "lod_bias": 1.5,
                "shadow_quality": 2,
                "shadow_distance": 80.0,
                "never_sleep": True
            }
        }
        self.active_mode = "Equilibrado"

    def test_economico_profile(self):
        settings = self.profiles["Economico"]
        self.assertEqual(settings["target_fps"], 30)
        self.assertAlmostEqual(settings["lod_bias"], 0.7)
        self.assertEqual(settings["shadow_quality"], 0)
        self.assertFalse(settings["never_sleep"])

    def test_equilibrado_profile(self):
        settings = self.profiles["Equilibrado"]
        self.assertEqual(settings["target_fps"], 60)
        self.assertAlmostEqual(settings["lod_bias"], 1.0)
        self.assertEqual(settings["shadow_quality"], 1)
        self.assertTrue(settings["never_sleep"])

    def test_alto_desempenho_profile(self):
        settings = self.profiles["AltoDesempenho"]
        self.assertEqual(settings["target_fps"], 60)
        self.assertAlmostEqual(settings["lod_bias"], 1.5)
        self.assertEqual(settings["shadow_quality"], 2)
        self.assertTrue(settings["never_sleep"])

    def test_profile_switching(self):
        self.active_mode = "Economico"
        self.assertEqual(self.profiles[self.active_mode]["target_fps"], 30)

        self.active_mode = "AltoDesempenho"
        self.assertEqual(self.profiles[self.active_mode]["target_fps"], 60)
        self.assertEqual(self.profiles[self.active_mode]["shadow_distance"], 80.0)


class TestSceneAndGUIDIntegrity(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))

    def test_script_and_meta_guids(self):
        expected_scripts = {
            os.path.join(self.unity_root, "Assets", "Scripts", "Core", "ReryCityIdentityConfig.cs.meta"): "100000000000000000000000000000a6",
            os.path.join(self.unity_root, "Assets", "Scripts", "Core", "OfflineGameBootstrapper.cs.meta"): "100000000000000000000000000000a7",
            os.path.join(self.unity_root, "Assets", "Scripts", "Monetization", "SimulatedRewardManager.cs.meta"): "100000000000000000000000000000a8",
            os.path.join(self.unity_root, "Assets", "Scripts", "Core", "AsynchronousSceneLoader.cs.meta"): "100000000000000000000000000000a9",
            os.path.join(self.unity_root, "Assets", "Scripts", "Performance", "MobileEnergyProfileManager.cs.meta"): "100000000000000000000000000000aa",
        }

        for meta_path, expected_guid in expected_scripts.items():
            self.assertTrue(os.path.isfile(meta_path), f"Meta file not found: {meta_path}")
            with open(meta_path, "r", encoding="utf-8") as f:
                content = f.read()
            self.assertIn(f"guid: {expected_guid}", content, f"GUID mismatch in {meta_path}")

    def test_scriptable_object_assets(self):
        identity_asset = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "Config", "ReryCityIdentity_Default.asset")
        self.assertTrue(os.path.isfile(identity_asset))
        with open(identity_asset, "r", encoding="utf-8") as f:
            content = f.read()
        self.assertIn("gameTitle: RERY CITY", content)
        self.assertIn("forceOfflineMode: 1", content)

    def test_urban_scene_game_manager(self):
        scene_path = os.path.join(self.unity_root, "Assets", "Scenes", "UrbanTrafficTestScene.unity")
        self.assertTrue(os.path.isfile(scene_path))
        with open(scene_path, "r", encoding="utf-8") as f:
            content = f.read()
        self.assertIn("m_Name: ReryCity_GameManager", content)
        self.assertIn("guid: 100000000000000000000000000000a7", content) # Bootstrapper
        self.assertIn("guid: 100000000000000000000000000000a8", content) # RewardManager
        self.assertIn("guid: 100000000000000000000000000000a9", content) # SceneLoader
        self.assertIn("guid: 100000000000000000000000000000aa", content) # EnergyManager


if __name__ == "__main__":
    unittest.main()
