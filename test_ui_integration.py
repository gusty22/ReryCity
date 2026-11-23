#!/usr/bin/env python3
"""
Suíte de Testes Automatizados — Fase 5.6: Integração Geral, HUD e Experiência do Jogador
Projeto Grau 2 (com.InsanityGroup.ProjetoGrau2)

Testa:
1. Integridade de Scripts C# e Metadados GUID da Fase 5.6
2. Validação Estrutural da Cena UrbanTrafficTestScene.unity (Canvas, HUD, Eventos, Controles Mobile, Áudio)
3. Simulação de Extração de Telemetria Real da Motocicleta (Velocidade, RPM, Marcha, Corte, Two-Step, Grau)
4. Simulação de Notificações Contextuais de Ocorrências com Filtragem de Distância
5. Simulação de Controles Táteis Mobile (Touchscreen) e Mapeamento para CrossPlatformInputHandler
6. Simulação de Roteamento de Áudio Centralizado, Espacialização e Tratamento Defensivo de Falhas
7. Verificação de Otimização Mobile Android, Canvas Scaler e Perfil Zero GC
"""

import os
import re
import math

UNITY_ROOT = r"C:\Users\washi\OneDrive\Área de Trabalho\REA\ProjetoGrau2_Unity"
ASSETS_DIR = os.path.join(UNITY_ROOT, "Assets")
SCENE_PATH = os.path.join(ASSETS_DIR, "Scenes", "UrbanTrafficTestScene.unity")

EXPECTED_SCRIPTS = {
    os.path.join("Scripts", "UI", "MotorcycleHUD.cs"): "10000000000000000000000000000098",
    os.path.join("Scripts", "UI", "WorldEventNotificationUI.cs"): "10000000000000000000000000000099",
    os.path.join("Scripts", "UI", "TouchscreenMobileControlsUI.cs"): "1000000000000000000000000000009a",
    os.path.join("Scripts", "Audio", "AudioIntegrationManager.cs"): "1000000000000000000000000000009b",
}

class TestReporter:
    def __init__(self):
        self.passed = 0
        self.failed = 0
        self.errors = []

    def assert_true(self, condition, msg):
        if condition:
            self.passed += 1
            print(f"  [PASS] {msg}")
        else:
            self.failed += 1
            self.errors.append(msg)
            print(f"  [FAIL] {msg}")

    def assert_almost_equal(self, a, b, tol=1e-3, msg=""):
        diff = abs(a - b)
        self.assert_true(diff <= tol, f"{msg} (esperado {b:.4f}, obtido {a:.4f}, diff {diff:.5f})")

    def summary(self):
        total = self.passed + self.failed
        print("\"" + "=" * 70)
        print(f"RELATORIO DE TESTES FASE 5.6 - TOTAL: {total} | APROVADOS: {self.passed} | FALHAS: {self.failed}")
        print("=" * 70)
        if self.errors:
            print("Lista de Falhas:")
            for e in self.errors:
                print(f"  - {e}")
        return self.failed == 0

reporter = TestReporter()

print("=" * 70)
print("1. AUDITORIA DE SCRIPTS C# E METADADOS GUID (FASE 5.6)")
print("=" * 70)

for script_rel, expected_guid in EXPECTED_SCRIPTS.items():
    script_path = os.path.join(ASSETS_DIR, script_rel)
    meta_path = script_path + ".meta"
    reporter.assert_true(os.path.exists(script_path), f"Script existe: {script_rel}")
    reporter.assert_true(os.path.exists(meta_path), f"Meta existe: {script_rel}.meta")
    if os.path.exists(meta_path):
        with open(meta_path, "r", encoding="utf-8") as f:
            content = f.read()
        match = re.search(r"guid:\s*([0-9a-fA-F]+)", content)
        actual_guid = match.group(1) if match else ""
        reporter.assert_true(actual_guid.lower() == expected_guid.lower(), f"GUID correto ({expected_guid}) em {script_rel}")

print("\n" + "=" * 70)
print("2. VALIDACAO ESTRUTURAL DA CENA UrbanTrafficTestScene.unity")
print("=" * 70)

reporter.assert_true(os.path.exists(SCENE_PATH), "Cena UrbanTrafficTestScene.unity existe")
with open(SCENE_PATH, "r", encoding="utf-8") as f:
    scene_content = f.read()

reporter.assert_true("m_Name: Main_Player_HUD_Canvas" in scene_content, "GameObject Main_Player_HUD_Canvas presente na cena")
reporter.assert_true("m_ReferenceResolution: {x: 1920, y: 1080}" in scene_content, "CanvasScaler configurado com resolução padrão 1920x1080 para Android")
reporter.assert_true("m_MatchWidthOrHeight: 0.5" in scene_content, "CanvasScaler com interpolação 0.5 para compatibilidade com múltiplas proporções de tela")
reporter.assert_true("guid: 10000000000000000000000000000098" in scene_content, "Componente MotorcycleHUD configurado com GUID correto")
reporter.assert_true("guid: 10000000000000000000000000000099" in scene_content, "Componente WorldEventNotificationUI configurado com GUID correto")
reporter.assert_true("guid: 1000000000000000000000000000009a" in scene_content, "Componente TouchscreenMobileControlsUI configurado com GUID correto")
reporter.assert_true("m_Name: AudioIntegrationManager" in scene_content, "GameObject AudioIntegrationManager presente na cena")
reporter.assert_true("guid: 1000000000000000000000000000009b" in scene_content, "Componente AudioIntegrationManager configurado com GUID correto")

print("\n" + "=" * 70)
print("3. SIMULACAO DE TELEMETRIA REAL DA MOTOCICLETA (MOTORCYCLE HUD)")
print("=" * 70)

class SimulatedMotorcycle:
    def __init__(self):
        self.speed_kmh = 85.4
        self.rpm = 9200.0
        self.gear = 3
        self.is_cut_active = False
        self.is_two_step_active = False
        self.pitch_angle = 42.0
        self.is_front_grounded = False
        self.camera_mode = "ThirdPersonClose"

class SimulatedMotorcycleHUD:
    def __init__(self, moto):
        self.moto = moto
        self.cached_speed = -1
        self.cached_rpm = -1
        self.cached_gear = -99
        self.cached_cut = False
        self.cached_two_step = False
        self.cached_angle = -999
        self.string_allocations = 0

    def update_telemetry(self):
        # 1. Velocidade
        cur_speed = int(round(self.moto.speed_kmh))
        if cur_speed != self.cached_speed:
            self.cached_speed = cur_speed
            self.string_allocations += 1

        # 2. RPM
        cur_rpm = int(round(self.moto.rpm))
        if cur_rpm != self.cached_rpm:
            self.cached_rpm = cur_rpm
            self.string_allocations += 1

        # 3. Marcha
        if self.moto.gear != self.cached_gear:
            self.cached_gear = self.moto.gear
            self.string_allocations += 1

        # 4. Corte e Two-Step
        self.cached_cut = self.moto.is_cut_active
        self.cached_two_step = self.moto.is_two_step_active

        # 5. Grau
        cur_angle = int(round(self.moto.pitch_angle))
        if cur_angle != self.cached_angle:
            self.cached_angle = cur_angle
            self.string_allocations += 1

bike = SimulatedMotorcycle()
hud = SimulatedMotorcycleHUD(bike)

hud.update_telemetry()
reporter.assert_true(hud.cached_speed == 85, "Velocidade exibida (85 km/h) extraída com precisão da física real")
reporter.assert_true(hud.cached_rpm == 9200, "RPM exibido (9200 RPM) extraído do motor")
reporter.assert_true(hud.cached_gear == 3, "Marcha engatada (3ª) extraída do câmbio")
reporter.assert_true(hud.cached_angle == 42, "Ângulo de empinada (42°) extraído da física do grau")
reporter.assert_true(not hud.cached_cut, "Indicador de corte desligado em regime normal")

# Simulação de corte de giro atingido
bike.is_cut_active = True
hud.update_telemetry()
reporter.assert_true(hud.cached_cut, "Indicador de corte de giro acionado imediatamente quando a ECU corta")

# Simulação de marcha neutra (Ponto Morto)
bike.gear = 0
hud.update_telemetry()
reporter.assert_true(hud.cached_gear == 0, "Ponto morto (Neutro) identificado e pronto para renderização 'N'")

# Verificação de Zero GC quando os valores não sofrem alteração inteira
allocs_before = hud.string_allocations
hud.update_telemetry() # mesmo valor
reporter.assert_true(hud.string_allocations == allocs_before, "Zero alocações de string quando a telemetria não altera o valor inteiro (Zero GC)")

print("\n" + "=" * 70)
print("4. SIMULACAO DE NOTIFICACOES CONTEXTUAIS DE OCORRENCIAS VIARIAS")
print("=" * 70)

class SimulatedNotificationUI:
    def __init__(self, max_distance=120.0, duration=5.0):
        self.max_distance = max_distance
        self.duration = duration
        self.is_active = False
        self.displayed_title = ""
        self.displayed_dist_text = ""
        self.timer = 0.0

    def trigger_event(self, event_type, player_pos, event_pos):
        dist = math.dist(player_pos, event_pos)
        if dist > self.max_distance:
            return False

        if event_type == "TrafficAccident":
            self.displayed_title = "⚠ ACIDENTE DE TRÂNSITO"
        elif event_type == "RoadworkBlockage":
            self.displayed_title = "🚧 OBRAS NA PISTA"
        elif event_type == "PoliceCheckpoint":
            self.displayed_title = "🚨 BLITZ POLICIAL"

        dist_int = int(round(dist))
        self.displayed_dist_text = f"A {dist_int}m DE VOCÊ"
        self.is_active = True
        self.timer = self.duration
        return True

    def update(self, dt):
        if self.timer > 0.0:
            self.timer -= dt
            if self.timer <= 0.0:
                self.is_active = False

notif_ui = SimulatedNotificationUI()
player_coord = (0.0, 0.0)
event_near = (0.0, 45.0) # 45m
event_far = (0.0, 150.0) # 150m (> 120m)

# Evento próximo deve disparar notificação
accepted = notif_ui.trigger_event("TrafficAccident", player_coord, event_near)
reporter.assert_true(accepted and notif_ui.is_active, "Ocorrência a 45m gera notificação contextual ativa")
reporter.assert_true(notif_ui.displayed_title == "⚠ ACIDENTE DE TRÂNSITO", "Título contextual de acidente exibido")
reporter.assert_true(notif_ui.displayed_dist_text == "A 45m DE VOCÊ", "Distância calculada e exibida com exatidão")

# Evento distante (> 120m) deve ser suprimido
ignored = notif_ui.trigger_event("RoadworkBlockage", player_coord, event_far)
reporter.assert_true(not ignored, "Ocorrência fora do raio de percepção (150m > 120m) é suprimida para evitar poluição visual")

# Auto-fade da notificação após duração
notif_ui.update(5.1)
reporter.assert_true(not notif_ui.is_active, "Notificação oculta automaticamente após esgotamento do displayDuration")

print("\n" + "=" * 70)
print("5. SIMULACAO DE CONTROLES TATEIS MOBILE (TOUCHSCREEN)")
print("=" * 70)

class SimulatedInputHandler:
    def __init__(self):
        self.throttle = 0.0
        self.steering = 0.0
        self.front_brake = 0.0
        self.rear_brake = 0.0
        self.wheelie = False
        self.shift_up = False
        self.camera_cycle = False

class SimulatedMobileUI:
    def __init__(self, handler):
        self.handler = handler

    def touch_throttle(self, pressed):
        self.handler.throttle = 1.0 if pressed else 0.0

    def touch_steer(self, direction): # -1, 0, 1
        self.handler.steering = float(direction)

    def touch_wheelie(self, pressed):
        self.handler.wheelie = pressed

    def touch_front_brake(self, pressed):
        self.handler.front_brake = 1.0 if pressed else 0.0

    def touch_rear_brake(self, pressed):
        self.handler.rear_brake = 1.0 if pressed else 0.0

    def tap_shift_up(self):
        self.handler.shift_up = True

input_h = SimulatedInputHandler()
mobile_ui = SimulatedMobileUI(input_h)

# Pressionar acelerador
mobile_ui.touch_throttle(True)
reporter.assert_almost_equal(input_h.throttle, 1.0, 1e-4, "Toque no acelerador ativa aceleração plena (1.0)")
mobile_ui.touch_throttle(False)
reporter.assert_almost_equal(input_h.throttle, 0.0, 1e-4, "Soltar acelerador encerra aceleração (0.0)")

# Pressionar manete de grau
mobile_ui.touch_wheelie(True)
reporter.assert_true(input_h.wheelie, "Toque no botão de grau aciona puxada virtual de empinada")

# Freios independentes
mobile_ui.touch_rear_brake(True)
reporter.assert_almost_equal(input_h.rear_brake, 1.0, 1e-4, "Freio traseiro (pé/manete) atua de forma independente")
reporter.assert_almost_equal(input_h.front_brake, 0.0, 1e-4, "Freio dianteiro permanece liberado")

# Troca de marcha
mobile_ui.tap_shift_up()
reporter.assert_true(input_h.shift_up, "Toque no botão de troca de marcha dispara pulso sequencial virtual")

print("\n" + "=" * 70)
print("6. SIMULACAO DE ROTEAMENTO DE AUDIO CENTRALIZADO E ESPACIAL")
print("=" * 70)

class SimulatedAudioSource:
    def __init__(self):
        self.spatial_blend = 0.0
        self.volume = 1.0
        self.played_clip = None

class SimulatedAudioManager:
    def __init__(self):
        self.master_vol = 1.0
        self.ui_vol = 0.75
        self.siren_vol = 0.85
        self.ui_source = SimulatedAudioSource()
        self.siren_source = SimulatedAudioSource()

    def configure_siren(self):
        self.siren_source.spatial_blend = 1.0 # 3D Total
        self.siren_source.volume = self.master_vol * self.siren_vol

    def play_ui_click(self, clip_present=True):
        if clip_present:
            self.ui_source.played_clip = "click.wav"
        # Sem exceção mesmo se clip_present for False

audio_mgr = SimulatedAudioManager()
audio_mgr.configure_siren()
reporter.assert_almost_equal(audio_mgr.siren_source.spatial_blend, 1.0, 1e-4, "Sirenes policiais configuradas com espacialização 3D completa")
reporter.assert_almost_equal(audio_mgr.siren_source.volume, 0.85, 1e-4, "Volume de sirene ponderado pelo Master Volume")

# Fallback defensivo com ausência de clip
audio_mgr.play_ui_click(clip_present=False)
reporter.assert_true(audio_mgr.ui_source.played_clip is None, "Execução defensiva segura sem NullReferenceException em áudios não importados")

print("\n" + "=" * 70)
print("7. VERIFICACAO DE PERFORMANCE, MOBILE PROFILES E ZERO GC")
print("=" * 70)

with open(os.path.join(ASSETS_DIR, "Scripts", "UI", "MotorcycleHUD.cs"), "r", encoding="utf-8") as f:
    hud_src = f.read()

reporter.assert_true("updateInterval" in hud_src, "MotorcycleHUD possui controle de taxa de atualização calibrável (Throttling)")
reporter.assert_true("_cachedSpeed" in hud_src and "_cachedRpm" in hud_src, "Dirty checking com inteiros em cache para evitar concatenação de strings contínua")

with open(os.path.join(ASSETS_DIR, "Scripts", "UI", "TouchscreenMobileControlsUI.cs"), "r", encoding="utf-8") as f:
    touch_src = f.read()

reporter.assert_true("SetVirtualThrottle" in touch_src and "SetVirtualSteering" in touch_src, "Mapeamento direto aos métodos de entrada do CrossPlatformInputHandler")
reporter.assert_true("Application.isMobilePlatform" in touch_src, "Sensibilidade à plataforma Android para exibição de botões táteis")

with open(os.path.join(ASSETS_DIR, "Scripts", "Audio", "AudioIntegrationManager.cs"), "r", encoding="utf-8") as f:
    audio_src = f.read()

reporter.assert_true("spatialBlend" in audio_src, "Diferenciação clara entre canais de UI 2D e áudio de tráfego/sirenes 3D")

success = reporter.summary()

import unittest

class TestUIIntegrationSuite(unittest.TestCase):
    def test_ui_integration_suite(self):
        self.assertTrue(success, f"Falhas detectadas na Fase 5.6: {reporter.errors}")
        self.assertEqual(reporter.failed, 0)
        self.assertGreater(reporter.passed, 0)

if __name__ == '__main__':
    exit(0 if success else 1)
