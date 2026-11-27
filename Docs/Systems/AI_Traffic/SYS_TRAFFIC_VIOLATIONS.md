# SYS_TRAFFIC_VIOLATIONS — Detecção de Infrações de Trânsito

---
type: system_specification
id: SYS-12.3
status: IMPLEMENTADO_EM_FASE_5_3
created: 2026-11-23
phase: 5
tags:
  - traffic_violations
  - infractions
  - speed_camera
  - wheelie_detection
dependencies:
  - SYS_WANTED_LEVEL
  - SYS_WHEELIE
  - SYS_TRAFFIC_NAVIGATION
---

## 1. Visão Geral

O componente `TrafficViolationDetector.cs` atua como o sistema de fiscalização de trânsito em tempo real do *Projeto Grau 2*.

---

## 2. Tipos de Infrações e Pesos

| Tipo de Infração | Condição de Detecção | Pontuação | Cooldown |
| :--- | :--- | :--- | :--- |
| **Speeding** | $v_{player} > v_{lane} + 12 km/h$ | 15 pts | 4.0s |
| **WrongWay** | $\vec{v}_{player} \cdot \vec{f}_{lane} < -0.5$ (alinhamento $> 120^\circ$) | 25 pts | 3.5s |
| **RecklessWheelie** | Ângulo de arfagem $\ge 25^\circ$ por tempo contínuo $\ge 0.8s$ | 35 pts | 3.0s |
| **RedLightViolation** | Cruzar linha de retenção com semáforo em `Red` a $> 15 km/h$ | 40 pts | 6.0s |
| **EvadingPolice** | Arrancar bruscamente durante abordagem ou voz de prisão | 60 pts | 5.0s |

---

## 3. Prevenção de Sobrecarga e Duplicação (Zero GC)

* O detector utiliza dicionário de cooldowns em ponto flutuante atualizado no `Update`.
* Não aloca coleções ou eventos no heap durante frames normais de circulação.
