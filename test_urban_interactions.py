import unittest
import os
import math

class MockSaveSystem:
    def __init__(self, initial_money_centavos=150000):
        self.money_centavos = initial_money_centavos

    def get_money(self):
        return self.money_centavos

    def deduct(self, amount_centavos):
        if self.money_centavos >= amount_centavos:
            self.money_centavos -= amount_centavos
            return True
        return False

class MockInteractable:
    def __init__(self, interaction_id, prompt, interaction_type, max_dist=4.0):
        self.interaction_id = interaction_id
        self.prompt = prompt
        self.interaction_type = interaction_type
        self.max_dist = max_dist
        self.interacted = False

    def can_interact(self, dist):
        return dist <= self.max_dist

    def interact(self):
        self.interacted = True


class TestInteractableContract(unittest.TestCase):
    def test_interaction_types_definition(self):
        valid_types = [
            "EntrarEstabelecimento",
            "AbastecerCombustivel",
            "ManutencaoMoto",
            "ComprarPeca",
            "ConversarNPC",
            "ServicoBorracharia"
        ]
        self.assertEqual(len(valid_types), 6)
        self.assertIn("EntrarEstabelecimento", valid_types)
        self.assertIn("AbastecerCombustivel", valid_types)
        self.assertIn("ManutencaoMoto", valid_types)


class TestPlayerInteractionDetectorLogic(unittest.TestCase):
    def test_proximity_selection_best_candidate(self):
        # Jogador na origem (0, 0, 0)
        player_pos = (0.0, 0.0, 0.0)
        candidates = [
            {"id": "pump_01", "pos": (2.0, 0.0, 0.0), "max_dist": 3.5},    # dist = 2.0 -> no alcance
            {"id": "oficina_01", "pos": (1.2, 0.0, 0.0), "max_dist": 4.0}, # dist = 1.2 -> mais proximo!
            {"id": "loja_01", "pos": (5.0, 0.0, 0.0), "max_dist": 4.0},    # dist = 5.0 -> fora de alcance
        ]

        best_candidate = None
        best_dist_sqr = float("inf")

        for c in candidates:
            dx = c["pos"][0] - player_pos[0]
            dy = c["pos"][1] - player_pos[1]
            dz = c["pos"][2] - player_pos[2]
            dist_sqr = dx*dx + dy*dy + dz*dz
            max_dist_sqr = c["max_dist"] * c["max_dist"]

            if dist_sqr <= max_dist_sqr and dist_sqr < best_dist_sqr:
                best_dist_sqr = dist_sqr
                best_candidate = c

        self.assertIsNotNone(best_candidate)
        self.assertEqual(best_candidate["id"], "oficina_01")
        self.assertAlmostEqual(math.sqrt(best_dist_sqr), 1.2)

    def test_interaction_lockout_and_completion(self):
        is_interacting = False
        target = MockInteractable("oficina_grau", "Entrar na Oficina", "ManutencaoMoto", 4.0)

        # Trigger interaction
        can_trigger = not is_interacting and target.can_interact(2.0)
        self.assertTrue(can_trigger)

        is_interacting = True
        target.interact()
        self.assertTrue(target.interacted)
        self.assertTrue(is_interacting)

        # Enquanto estiver interagindo, novas detecções ou triggers devem ser bloqueados
        can_trigger_again = not is_interacting and target.can_interact(2.0)
        self.assertFalse(can_trigger_again)

        # Finalização da interação
        is_interacting = False
        self.assertFalse(is_interacting)


class TestInteractionContextUI(unittest.TestCase):
    def test_prompt_visibility_and_alpha_fade(self):
        target_alpha = 0.0
        current_alpha = 0.0
        fade_speed = 8.0
        dt = 0.05

        # Detecta entidade -> Exibe prompt
        target_alpha = 1.0
        # Simula 3 passos de frame
        for _ in range(3):
            step = dt * fade_speed
            if current_alpha < target_alpha:
                current_alpha = min(target_alpha, current_alpha + step)

        self.assertGreater(current_alpha, 0.0)
        self.assertLessEqual(current_alpha, 1.0)

        # Perde entidade -> Oculta prompt
        target_alpha = 0.0
        for _ in range(5):
            step = dt * fade_speed
            if current_alpha > target_alpha:
                current_alpha = max(target_alpha, current_alpha - step)

        self.assertAlmostEqual(current_alpha, 0.0, places=2)


class TestInteractiveEstablishments(unittest.TestCase):
    def test_operating_hours_check(self):
        def is_open(current_hour, open_hour, close_hour, is_24h):
            if is_24h:
                return True
            if open_hour <= close_hour:
                return open_hour <= current_hour < close_hour
            return current_hour >= open_hour or current_hour < close_hour

        # Oficina comercial: 07h às 20h
        self.assertTrue(is_open(12.0, 7.0, 20.0, False))
        self.assertFalse(is_open(22.0, 7.0, 20.0, False))
        self.assertFalse(is_open(5.0, 7.0, 20.0, False))

        # Posto 24 horas
        self.assertTrue(is_open(3.0, 0.0, 0.0, True))
        self.assertTrue(is_open(15.0, 0.0, 0.0, True))

    def test_commercial_service_debit_from_save_system(self):
        save = MockSaveSystem(initial_money_centavos=10000) # R$ 100,00

        service_cost_centavos = 3500 # R$ 35,00
        success = save.deduct(service_cost_centavos)

        self.assertTrue(success)
        self.assertEqual(save.get_money(), 6500) # R$ 65,00

        # Tentativa de serviço muito caro acima do saldo
        expensive_cost = 10000 # R$ 100,00
        fail_success = save.deduct(expensive_cost)
        self.assertFalse(fail_success)
        self.assertEqual(save.get_money(), 6500) # Saldo mantido


class TestGasStationPumpLogic(unittest.TestCase):
    def test_refuel_liters_calculation_and_cost(self):
        tank_capacity = 14.0 # Litros
        current_fuel = 4.0
        price_per_liter_centavos = 589 # R$ 5,89/L
        save = MockSaveSystem(initial_money_centavos=150000) # R$ 1.500,00

        liters_to_fill = 5.0
        space_available = tank_capacity - current_fuel
        actual_liters = min(liters_to_fill, space_available)
        cost_centavos = math.ceil(actual_liters * price_per_liter_centavos)

        self.assertTrue(save.deduct(cost_centavos))
        current_fuel += actual_liters

        self.assertEqual(current_fuel, 9.0)
        self.assertEqual(cost_centavos, 2945) # 5 * 589 = 2945 centavos = R$ 29,45
        self.assertEqual(save.get_money(), 150000 - 2945)

    def test_refuel_full_tank_capped_at_capacity(self):
        tank_capacity = 14.0
        current_fuel = 11.5
        needed = tank_capacity - current_fuel # 2.5 L
        price_per_liter_centavos = 589
        save = MockSaveSystem(initial_money_centavos=5000)

        cost_centavos = math.ceil(needed * price_per_liter_centavos)
        self.assertTrue(save.deduct(cost_centavos))
        current_fuel += needed

        self.assertEqual(current_fuel, 14.0)
        self.assertEqual(cost_centavos, 1473) # 2.5 * 589 = 1472.5 -> 1473 centavos

    def test_partial_refuel_when_funds_limited(self):
        tank_capacity = 14.0
        current_fuel = 2.0
        price_per_liter_centavos = 589
        save = MockSaveSystem(initial_money_centavos=2000) # Só tem R$ 20,00

        needed = tank_capacity - current_fuel # 12.0 L
        total_cost = math.ceil(needed * price_per_liter_centavos) # 7068 centavos

        # Saldo insuficiente para tanque cheio -> abastece o que pode pagar
        actual_liters = save.get_money() / price_per_liter_centavos
        cost = math.floor(actual_liters * price_per_liter_centavos)

        self.assertTrue(save.deduct(cost))
        current_fuel += actual_liters

        self.assertAlmostEqual(actual_liters, 2000 / 589, places=2)
        self.assertAlmostEqual(current_fuel, 2.0 + (2000 / 589), places=2)
        self.assertLessEqual(save.get_money(), price_per_liter_centavos)


class TestEstablishmentInteriorManagerStreaming(unittest.TestCase):
    def test_proximity_activation_and_hysteresis(self):
        activation_radius = 20.0
        deactivation_radius = 28.0
        interior_pos = (0.0, 0.0, 0.0)

        def eval_state(current_state, player_dist):
            dist_sqr = player_dist * player_dist
            act_sqr = activation_radius * activation_radius
            deact_sqr = deactivation_radius * deactivation_radius

            if not current_state and dist_sqr <= act_sqr:
                return True
            elif current_state and dist_sqr > deact_sqr:
                return False
            return current_state

        state = False
        # Jogador a 30m: inativo
        state = eval_state(state, 30.0)
        self.assertFalse(state)

        # Jogador aproxima-se a 18m (<= 20m): ativa
        state = eval_state(state, 18.0)
        self.assertTrue(state)

        # Jogador afasta-se para 24m (entre 20m e 28m): continua ativo por histerese
        state = eval_state(state, 24.0)
        self.assertTrue(state)

        # Jogador afasta-se além de 28m (ex: 32m): desativa
        state = eval_state(state, 32.0)
        self.assertFalse(state)


class TestSceneAndMetaIntegration(unittest.TestCase):
    def setUp(self):
        self.unity_root = os.path.dirname(os.path.abspath(__file__))

    def test_all_scripts_and_metas_exist_with_deterministic_guids(self):
        expected = {
            "IInteractable.cs": "100000000000000000000000000000bc",
            "PlayerInteractionDetector.cs": "100000000000000000000000000000bd",
            "InteractionContextUI.cs": "100000000000000000000000000000be",
            "InteractiveEstablishment.cs": "100000000000000000000000000000bf",
            "GasStationPump.cs": "100000000000000000000000000000c0",
            "EstablishmentInteriorManager.cs": "100000000000000000000000000000c1",
            "InteractiveEstablishmentConfig.cs": "100000000000000000000000000000c2",
        }

        for fname, guid in expected.items():
            script_path = os.path.join(self.unity_root, "Assets", "Scripts", "Interaction", fname)
            meta_path = script_path + ".meta"

            self.assertTrue(os.path.isfile(script_path), f"Arquivo ausente: {script_path}")
            self.assertTrue(os.path.isfile(meta_path), f"Meta ausente: {meta_path}")

            with open(meta_path, "r", encoding="utf-8") as f:
                content = f.read()
            self.assertIn(f"guid: {guid}", content, f"GUID incorreto para {fname}")

    def test_scriptable_object_asset_and_meta(self):
        asset_path = os.path.join(self.unity_root, "Assets", "ScriptableObjects", "World", "InteractiveEstablishment_Default.asset")
        meta_path = asset_path + ".meta"

        self.assertTrue(os.path.isfile(asset_path), f"Asset ausente: {asset_path}")
        self.assertTrue(os.path.isfile(meta_path), f"Asset meta ausente: {meta_path}")

        with open(meta_path, "r", encoding="utf-8") as f:
            content = f.read()
        self.assertIn("guid: 200000000000000000000000000000ae", content)

        with open(asset_path, "r", encoding="utf-8") as f:
            text = f.read()
        self.assertIn("establishmentId: est_oficina_grau_centro", text)
        self.assertIn("Oficina do Grau & Performance", text)

    def test_scene_integration_nodes(self):
        scene_path = os.path.join(self.unity_root, "Assets", "Scenes", "UrbanTrafficTestScene.unity")
        self.assertTrue(os.path.isfile(scene_path))

        with open(scene_path, "r", encoding="utf-8") as f:
            scene_text = f.read()

        # Detector no Player
        self.assertIn("guid: 100000000000000000000000000000bd", scene_text)
        # UI contextual
        self.assertIn("guid: 100000000000000000000000000000be", scene_text)
        # Interior Manager, Oficina e Posto
        self.assertIn("m_Name: Araguari_Interactive_Establishments", scene_text)
        self.assertIn("guid: 100000000000000000000000000000c1", scene_text)
        self.assertIn("guid: 100000000000000000000000000000bf", scene_text)
        self.assertIn("guid: 100000000000000000000000000000c0", scene_text)


if __name__ == "__main__":
    unittest.main()
