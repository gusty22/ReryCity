---
id: SYS_SAVE_SYSTEM
tipo: sistema
codigo: SYS-21
status: IMPLEMENTADO_EM_FASE_3
responsavel: Engenheiro de Arquitetura & Persistência
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_WORKSHOP, SYS_ATHLON, SYS_ECONOMY]
arquivos_relacionados: [SaveSystem.cs]
referencia_tecnica: [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]
---

# SISTEMA DE PERSISTÊNCIA E SALVAMENTO (SYS-21: SAVE SYSTEM)

## 1. Identificação e Objetivo
* **Código**: `SYS-21`
* **Nome**: Sistema de Persistência e Progresso (*Save System*).
* **Status**: `IMPLEMENTADO_EM_FASE_3`.
* **Objetivo**: Serializar e desserializar em formato JSON o progresso completo do jogador: motocicletas na garagem, peças instaladas na oficina, saldo financeiro, preferências de câmera e calibrações personalizadas da injeção Athlon.

## 2. Compatibilidade Multiplataforma
* Utiliza `Application.persistentDataPath` como diretório de armazenamento padrão.
  * No **PC Windows**: Salva em `%USERPROFILE%/AppData/LocalLow/InsanityGroup/ProjetoGrau2/savegame_progression.json`.
  * No **Android**: Salva na sandbox isolada da aplicação (`/data/data/com.InsanityGroup.ProjetoGrau2/files/`), preservando o savegame mesmo após atualizações de APK.
