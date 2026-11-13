# AppOrcamentacao_askblue

Plataforma web para registo de oportunidades de serviço, submissão uniformizada de orçamentos, colaboração direta entre clientes e prestadores de serviços e acompanhamento consistente da execução e avaliação final dos trabalhos. Para além disso Permite à empresa ter acesso a tudo com a role de admin.

---

## Problema e Dificuldades Endereçadas

O projeto visa substituir processos manuais e dispersos (como folhas de cálculo e trocas de e-mail) por um fluxo centralizado, auditável e estruturado, resolvendo quatro desafios principais:

* **Carga Operacional:** Eliminação de processos morosos e folhas de cálculo não uniformizadas na gestão de pedidos e orçamentos.
* **Ausência de Visão Global:** Controlo centralizado sobre o estado dos serviços e a execução dos trabalhos para garantir qualidade e escalabilidade.
* **Seleção de Prestadores Fiáveis:** Facilidade na identificação de prestadores adequados a cada necessidade, considerando especialidades e histórico de desempenho.
* **Riscos de Conformidade:** Redução da dependência de e-mails e registos informais, assegurando rastreabilidade, histórico auditável e gestão organizada da documentação.

---

## Fluxo da Plataforma (Visão Geral)

1. **Onboarding:** Clientes e prestadores registam-se na plataforma com perfis dedicados.
2. **Pedido de Serviço:** O cliente submete um novo pedido através de um formulário estruturado (com suporte a templates por tipo de serviço).
3. **Orçamentação Uniformizada:** Os prestadores pesquisam pedidos disponíveis e submetem propostas de orçamento num formato standard.
4. **Adjudicação e Gestão:** O cliente analisa os orçamentos recebidos, aceita ou recusa propostas e acompanha o estado do serviço.
5. **Execução e Fecho:** O prestador gere os serviços aceites até à conclusão, permitindo o acompanhamento e avaliação final do trabalho.

---

## 📋 Requisitos Funcionais (MVP)

| Ref | Requisito | Ator |
| :--- | :--- | :--- |
| **1.1** | O cliente deve conseguir criar novos serviços. | Cliente |
| **1.2** | O prestador deve poder pesquisar por serviços disponíveis. | Prestador |
| **1.3** | O prestador deve apresentar o orçamento de forma standard (uniformizada). | Prestador |
| **1.4** | O cliente deve conseguir registar-se na plataforma. | Cliente |
| **1.5** | O prestador deve conseguir registar-se na plataforma. | Prestador |
| **1.6** | O cliente deve conseguir gerir os seus serviços. | Cliente |
| **1.7** | O prestador deve conseguir gerir os serviços aceites. | Prestador |
| **1.8** | O cliente recebe propostas de orçamento dos prestadores e pode aceitar ou recusar. | Cliente |

---

## Funcionalidades e Diretrizes Técnicas

* **Formulários e Templates Uniformizados:** Estrutura padronizada para criação de pedidos e submissão de orçamentos (com exemplos pré-configurados por categoria de serviço).
* **Validação Cliente/Servidor:** Garantia de integridade dos dados submetidos tanto na interface (Frontend) como na API (Backend).
* **Persistência e Área de Gestão:** Armazenamento estruturado em base de dados relacional com painel de consulta e acompanhamento de estados.
* **Gestão Documental *(Em evolução)*:** Suporte previsto para anexos e documentação associada aos serviços e propostas.

---

## Stack Tecnológica *(Em definição / Atualizar conforme implementação)*


---

## Como Executar o Projeto


