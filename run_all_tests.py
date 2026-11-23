import subprocess
import sys
import os

TEST_FILES = [
    ("Fase 5.1 - Navegacao Urbana", "test_traffic_navigation.py"),
    ("Fase 5.2 - Veiculos Civis NPC", "test_ai_vehicles.py"),
    ("Fase 5.3 - IA Policial & Procurado", "test_police_ai.py"),
    ("Fase 5.4 - Ocorrencias & Bloqueios", "test_world_events.py"),
    ("Fase 5.5 - Perseguicao Policial Avancada", "test_police_pursuit_advanced.py"),
    ("Fase 5.6 - HUD & Experiencia Touchscreen", "test_ui_integration.py"),
    ("Fase 5.7 - Infraestrutura de Mundo & LOD", "test_world_infrastructure.py"),
    ("Fase 5.8 - Rery City & Inicializacao Offline", "test_rery_city_offline.py"),
    ("Fase 5.9 - Araguari Real & Sistema Geografico", "test_araguari_world.py"),
    ("Fase 5.10 - Araguari Visual & Arquitetura Urbana", "test_city_visual_infrastructure.py"),
    ("Fase 5.11 - Araguari Detalhes & Identidade dos Bairros", "test_city_environment_detail.py"),
    ("Fase 5.12 - Interacoes Urbanas & Estabelecimentos", "test_urban_interactions.py"),
    ("Reconstrucao Fiel - Geografia Real de Araguari (IBGE)", "test_araguari_faithful_reconstruction.py"),
]

def main():
    unity_dir = os.path.dirname(os.path.abspath(__file__))
    python_exe = sys.executable

    print("=" * 80)
    print("REGRESSAO COMPLETA DE TESTES AUTOMATIZADOS — RERY CITY (FASES 5.1 A 5.12 + RECONSTRUCAO IBGE)")
    print("=" * 80)

    total_suites = len(TEST_FILES)
    passed_suites = 0
    failed_suites = 0

    for title, script_name in TEST_FILES:
        script_path = os.path.join(unity_dir, script_name)
        print(f"\n>> Executando: {title} ({script_name})...")

        res = subprocess.run([python_exe, script_path], cwd=unity_dir, capture_output=True, text=True)

        if res.returncode == 0:
            print(f"   [PASS] {title} executada com SUCESSO (Exit Code: 0)")
            # Imprime últimas 2 linhas do output para ver resumo
            lines = [l for l in res.stdout.strip().splitlines() if l.strip()]
            for l in lines[-2:]:
                print(f"          {l}")
            passed_suites += 1
        else:
            print(f"   [FAIL] {title} FALHOU (Exit Code: {res.returncode})")
            print("--- STDOUT ---")
            print(res.stdout)
            print("--- STDERR ---")
            print(res.stderr)
            failed_suites += 1

    print("\n" + "=" * 80)
    print(f"RESUMO GERAL: {passed_suites}/{total_suites} SUITES APROVADAS | {failed_suites} FALHAS")
    print("=" * 80)

    if failed_suites > 0:
        sys.exit(1)
    else:
        print(f"CONCLUSAO: TODAS AS {total_suites} SUITES FORAM APROVADAS COM 100% DE SUCESSO!")
        sys.exit(0)

if __name__ == "__main__":
    main()
