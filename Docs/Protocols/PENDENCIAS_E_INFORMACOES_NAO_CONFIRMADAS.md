# Pendências Técnicas e Informações Não Confirmadas: Projeto Grau 2

> [!NOTE]
> Registro formal das lacunas investigativas, hipóteses abertas e dados que ainda dependem de procedimentos adicionais de extração e descompilação de código nativo.

---

## 1. Quadro de Classificação de Incertezas

| Subsistema / Recurso | O que já está Comprovado | O que permanece Pendente / Não Confirmado | Nível de Impacto no Projeto |
| :--- | :--- | :--- | :--- |
| **Parâmetros Numéricos Individuais por Moto** | Estrutura de classes, variáveis de `BikeWheelie`, marchas e relações de transmissão. | Os valores decimais exatos configurados nos arquivos de Prefab serializados de cada moto (ex: massa exata da Titan vs. XJ6, torque padrão em Nm de fábrica). | **Médio**: Facilmente calibrável na nova implementação por telemetria e teste de pilotagem. |
| **Síntese Acústica dos Motores (`RoncoGranular`)** | O sistema utiliza controle de volume, tom e loops em rotação. | Se os roncos utilizam síntese granular algorítmica em tempo real via código C++ nativo ou interpolação clássica de pitch em 4 clips estáticos. | **Baixo**: O Unity Audio Mixer moderno resolve isso com facilidade e fidelidade idêntica. |
| **Roteamento Topológico do GPS** | A malha `RedeDeRuas` possui waypoints com custo de conversão e penalidade por contramão. | As coordenadas espaciais absolutas de cada nó viário do mapa 3D original. | **Baixo**: O novo mapa terá sua própria malha viária navegável com NavMesh / A*. |
| **Descompactação dos Modelos 3D de `datapack.unity3d`** | Tamanho, contêiner UnityFS v5.x.x, identificadores de assets e nomes de malhas. | Extração binária individual das malhas em formato `.obj` ou `.fbx` descompactadas. | **Médio**: Os modelos originais estão protegidos pelo contêiner LZ4 e podem ser extraídos com ferramentas de parse UnityFS. |
| **Comunicação com Servidor de Contas** | Mapeamento das classes `AtlasConta` e `ConfigDaConta`. | Se o jogo sincroniza o inventário em um backend REST próprio ou depende exclusivamente dos saves em nuvem do Google Play Games. | **Baixo**: A nova implementação adotará sua própria arquitetura de persistência local/nuvem. |

---

## 2. Próximos Procedimentos Recomendados para Liquidação das Pendências

1. **Leitura dos Prefabs de Motos**: Implementar o desempacotador LZ4 para ler as tabelas de nós de `datapack.unity3d` e extrair os blocos de dados `MonoBehaviour` correspondentes a cada prefab de moto.
2. **Extração das Faixas de Áudio**: Ler o arquivo `resources.resource` (65,8 MB) para extrair os cabeçalhos RIFF/WAV e identificar as amostras de ronco de escapamento da XJ6 e Hornet.
3. **Calibração Prática**: Iniciar a montagem da cena de protótipo em Unity 6 com as equações documentadas em [FORMULAS_FISICA_MOTOS.md](file:///C:/Users/washi/.gemini/antigravity/brain/878b361f-e377-4953-97fd-38f436efc1de/FORMULAS_FISICA_MOTOS.md).
