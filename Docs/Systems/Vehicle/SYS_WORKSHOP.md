---
id: SYS_WORKSHOP
tipo: sistema
codigo: SYS-15
status: IMPLEMENTADO_EM_FASE_3
responsavel: Engenheiro de Sistemas & Arquiteto de Software
data_criacao: 2026-11-23
data_atualizacao: 2026-11-23
dependencias: [SYS_CUSTOMIZATION, SYS_ENGINE, SYS_TRANSMISSION, PartItemData]
arquivos_relacionados: [WorkshopManager.cs, PartItemData.cs]
referencia_tecnica: [[ADR_003_SCRIPTABLE_OBJECTS_DATA]]
---

# SISTEMA DE OFICINA E GERENCIAMENTO DE PEÇAS (SYS-15: WORKSHOP)

## 1. Identificação e Objetivo
* **Código**: `SYS-15`
* **Nome**: Sistema de Oficina e Gerenciamento de Peças (*Workshop Manager*).
* **Status**: `IMPLEMENTADO_EM_FASE_3`.
* **Objetivo**: Prover o ambiente 3D onde o jogador inspeciona a motocicleta em $360^\circ$, consulta o catálogo de componentes categorizados, instala, substitui ou remove peças com aplicação mecânica e visual imediata.

## 2. Categorias de Peças Suportadas
1. `Engine`: Kits de cilindro, comandos e motores completos.
2. `Exhaust`: Escapamentos esportivos (Dore, Fortuna, Torbal) alterando o perfil acústico e torque.
3. `Transmission`: Conjuntos de coroa e pinhão (modificando a relação final de transmissão).
4. `Suspension`: Molas e amortecedores (alterando rigidez e molejo).
5. `Brakes`: Discos esportivos e pastilhas de alta performance.
6. `TiresWheels`: Pneus de manobra com diferentes níveis de aderência e inércia.
7. `Handlebars`, `Bodywork`, `Paint`: Customizações estéticas e ergonômicas.
