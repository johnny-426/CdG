# Validação — 18/10/2026

## Executado nesta entrega

- Build do projeto original integrado em .NET 10: sucesso, zero erros e zero avisos.
- Compilação Razor incluída no build.
- `dotnet publish -c Release`: publicação em pasta gerada com sucesso. Isso não publica o site na internet.
- Migration `20261018123556_AdicionarFinanceiro` gerada pelo EF Core 10.0.12, com Designer e snapshot.
- `dotnet ef migrations has-pending-model-changes`: nenhuma alteração de modelo pendente.
- SQL idempotente PostgreSQL gerado em `MIGRACOES.sql`.
- Migrations anteriores, exceto snapshot naturalmente atualizado, comparadas byte a byte e preservadas.
- Teste executável `tests/CdG.Verificacao`: 21 verificações concluídas, incluindo 700 combinações de valor/quantidade para o cálculo de cotas e cenários de persistência, autorização, pagamentos parciais, confirmação repetida, cancelamento e renúncia.
- Integridade dos cinco ZIPs recuperados verificada. Inventário contém 34 arquivos originais localizados.

## Limites da validação

O teste de persistência usa SQLite em memória com um interceptor EXCLUSIVO do projeto de testes que remove `FOR UPDATE`. O aplicativo entregue usa PostgreSQL e mantém os bloqueios. Portanto, os testes não comprovam concorrência nem execução das migrations no PostgreSQL.

Não foi possível iniciar PostgreSQL neste ambiente. Não foram executados testes de navegação com várias contas, SMTP real, construção de imagem Docker, implantação externa ou testes de carga. Execute `TESTES.md` no ambiente local antes de tratar esta versão como homologada.

## Limites funcionais explícitos

- Financeiro limitado a divisão igual entre confirmados; sem grupos de consumo.
- Despesas e cotas imutáveis após fechamento; não há reabertura nem estorno de recebimento confirmado.
- Acertos são registros manuais, sem processamento bancário.
- Renúncia é integral sobre o crédito restante, definitiva e aplicada a devedores em ordem fixa; não redistribui proporcionalmente.
- Cancelamento e financeiro são independentes. O evento pode aparecer cancelado enquanto ainda existem acertos. Não foi implementado um novo estado de cancelamento aguardando reembolso.
- Os organizadores mantêm os poderes existentes, inclusive cancelar. Não foi introduzido um papel especial de Criador com exclusividade de cancelamento.
- O financeiro não envia novas notificações a cada acerto; o usuário consulta a tela. As notificações de eventos/convites anteriores foram preservadas.
- Retorno após saída continua indisponível, como no pacote de correções anterior.
- Exclusão automática de contas com histórico não está implementada. Algumas páginas padrão de conta ainda estão em inglês.
- Diagramas históricos ainda precisam ser atualizados para o financeiro entregue. O documento final da extensão depende dos requisitos da faculdade.

## Resultado

Versão integrada compilada e com regras financeiras verificadas. Pronta para instalação e validação local; publicação externa e homologação PostgreSQL permanecem pendentes.

## Complemento — instalação local informada em 18/10/2026

O mantenedor informou posteriormente que instalou o SDK 10.0.402, executou `dotnet build` com sucesso, configurou um banco PostgreSQL separado (`cdg_v1_teste`) e verificou as tabelas criadas após a aplicação das migrations. Também confirmou funcionamento local de página inicial, cadastro/login, criação de evento, listagem, detalhes e edição.

Esse relato complementa os registros da preparação original. **Não substitui** a execução e o registro dos demais cenários de `docs/TESTES.md`, especialmente financeiro ponta a ponta com múltiplas contas, concorrência PostgreSQL, SMTP e deploy público. A primeira tentativa de migration foi direcionada ao banco anterior antes de salvar/recarregar a configuração; convém revisar o estado desse banco caso ele ainda seja utilizado.
