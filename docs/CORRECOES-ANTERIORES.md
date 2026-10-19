# CdG — correções, saída e histórico

Base: CdG Completo.zip enviado em 14/10/2026.
Este pacote contém SOMENTE os 22 arquivos novos ou alterados. Não é um projeto independente.

## Como aplicar no Visual Studio

1. Pare o aplicativo. Faça um commit do estado atual ou uma cópia da pasta do projeto.
2. Extraia este ZIP em uma pasta temporária.
3. Abra a pasta `CdG` extraída. Copie TODO o seu conteúdo para a pasta do seu projeto que contém `CdG.csproj` e `Program.cs`.
4. Aceite mesclar as pastas e substituir os arquivos com o mesmo nome. Não exclua a pasta antiga: o pacote não contém os arquivos que permaneceram iguais.
5. Preserve `.git`, a solução, os arquivos de configuração e todas as migrations existentes. Não há pasta `.git` neste pacote. Não é necessário mexer nos arquivos do Git.
6. Reabra/recarregue o projeto no Visual Studio, se solicitado. Use Compilar > Recompilar Solução. O projeto exige .NET 10, como antes.
7. No Console do Gerenciador de Pacotes, com CdG selecionado, consulte `Get-Migration`. Se houver migrations pendentes, confira quais são e aplique `Update-Database` no seu banco de desenvolvimento antes dos testes. Se todas já estiverem aplicadas, não precisa atualizar o banco. Não execute `Add-Migration` para este pacote.
8. Execute com Ctrl+F5 e faça os testes abaixo.

A migration PreencherAceitesAntigos existente foi preservada sem alterações. O Down dela continua bloqueando rollback através dessa migration; não a apague nem tente recriá-la. Faça backup do banco antes de aplicar migrations pendentes se precisar preservar os dados de teste.

## O que mudou

- Cancelar presença agora tem texto coerente: mantém o vínculo e permite confirmar novamente.
- Edição e reagendamento enviam uma versão dos dados exibidos. O servidor compara após bloquear o evento e rejeita formulários antigos. A mensagem pede recarregar; não sobrescreve silenciosamente alterações de outro organizador.
- Vínculos encerrados deixam de contar para lotação, presença, reagendamento, painel, convites ativos e envio de novas notificações.
- Notificações antigas mantêm o texto, mas deixam de exibir acesso aos detalhes quando o usuário já não pode abri-los.
- Menu corrigido, idioma pt-BR, nomes básicos em português e controles para os filtros de eventos.
- Dependência de geração de código fixada em 10.0.2, versão encontrada no project.assets.json enviado, em lugar de 10.*.
- Exclusão de conta: tela própria evita erro de banco para contas com registros relacionados. Contas sem histórico ainda podem ser excluídas com confirmação e validação de senha, quando houver senha. A exclusão/anomização completa de contas com histórico continua pendente.

## Regras desta entrega

### Sair do evento

Disponível nos detalhes de eventos agendados ou adiados, com aviso e confirmação. Libera a vaga, encerra permissões e marca avisos pendentes desse evento como lidos. O registro fica na lista com “Saiu do evento”, sem link para detalhes.

Se outro organizador ativo existir, ele continua administrando. Se quem sai for o último, assume automaticamente um participante ativo com aceite registrado, sem convite pendente: menor ParticipacaoAceitaEm, com desempate por Id. Presença cancelada não apaga um aceite anterior; portanto, alguém que aceitou e depois cancelou apenas a presença ainda pode assumir. A promoção não confirma presença nem consome outra vaga.

Sem sucessor elegível, a saída é bloqueada. O organizador pode aguardar um aceite, promover alguém elegível ou cancelar o evento e depois ocultá-lo.

Saída e promoção automática são gravadas na mesma transação, com o mesmo bloqueio de evento usado nas demais operações. Uma segunda saída concorrente reavalia os organizadores após obter o bloqueio.

Nesta versão, sair é definitivo para aquele evento. O fluxo não permite reenviar convite nem reativar o vínculo. Essa regra está explícita na interface. Se quisermos permitir retorno depois, teremos de definir novo aceite e antiguidade; os índices únicos foram preservados.

### Lixeira vermelha

Aparece em Meus eventos para eventos cancelados ou concluídos. Oculta SOMENTE o vínculo do usuário no histórico. Não exclui eventos ou registros das outras pessoas. Em “Ver eventos ocultos”, o botão Restaurar desfaz a ocultação. Restaurar não reativa alguém que já saiu.

### Aceites antigos

A ordenação usa as datas disponíveis, não uma garantia de reconstrução da ordem histórica real. A migration antiga pode ter recuperado a última confirmação disponível. Se uma data não foi preservada, não é inventada. O serviço ainda recupera datas de convites aceitos com RespondidoEm quando o campo está vazio. Quem não tiver aceite recuperável não participa da sucessão automática até registrar um aceite válido pelos fluxos normais.

## Testes no seu computador — ainda não executados

Use contas de teste A, B e C e eventos descartáveis.

1. A cria um evento; B aceita. A promove B. Confira notificação e acesso administrativo de B.
2. Abra Editar em duas abas/contas. Salve uma mudança de local na primeira. Envie a segunda: deve informar conflito e preservar a primeira mudança. Recarregue e salve de novo. Repita em Adiar/reagendar e também com Editar numa aba e Adiar na outra.
3. Em evento com limite de 2, A e B confirmam. C tenta aceitar e recebe aviso de lotação. B sai. C consegue aceitar. B não acessa detalhes nem ações administrativas, mesmo usando uma aba antiga.
4. Com A como único organizador, B aceita antes de C. A sai: B assume, recebe notificação e mantém sua presença anterior. Repita com B e C como organizadores: a saída de um mantém o outro.
5. Tente sair como único organizador sem participantes elegíveis: deve bloquear sem mudar o evento. Convites pendentes não podem ser escolhidos como sucessores.
6. Tente reenviar convite para quem saiu: a tela explica que o retorno ainda não está disponível. Reagende e confira que quem saiu não volta a ficar pendente nem recebe novo aviso.
7. Cancele ou conclua um evento. Use a lixeira com A: ele some apenas da lista de A. Entre em Ocultos e restaure. B continua vendo seu próprio histórico. Um POST tentando ocultar evento ativo deve ser recusado.
8. Confira notificações antigas de quem saiu: sem link para detalhes; mensagens preservadas. Confira filtros e o menu em janela estreita.
9. Na conta com histórico, abra a exclusão de dados pessoais: deve mostrar a limitação sem erro de banco. Teste a exclusão somente com uma conta descartável sem histórico, incluindo senha errada antes da correta.
10. Para concorrência real, tente aceitar a última vaga em duas sessões e sair com dois organizadores ao mesmo tempo. A lotação e a existência de um organizador ativo devem permanecer consistentes.

## Validação realizada aqui

Revisão das alterações, XML do csproj, correspondência entre formulários de eventos e handlers, posição das verificações de versão após o bloqueio e preservação byte a byte das migrations. O ZIP foi conferido quanto à integridade.

Não foi possível compilar, executar, renderizar o aplicativo ou testar PostgreSQL: o ambiente não tem o SDK .NET nem acesso ao seu banco. Os testes acima são necessários antes de tratar a versão como validada para uso real.

## Próximas etapas

- Configurar confirmação de e-mail, envio real e recuperação de senha antes de publicar.
- Definir exclusão/anomização de contas com histórico compartilhado.
- Paginação de eventos e convites, tradução completa das telas padrão e revisão visual.
- Definir se haverá retorno após saída.
- Para aplicativo: as novas regras de participação estão em ParticipacaoService, fora das páginas. Isso facilita reutilizá-las em uma API futura. Esta entrega ainda é o site Razor Pages; não inclui app móvel, API pública nem PWA.

Commit sugerido, depois de compilar e testar:
`feat: corrigir concorrencia e implementar saida e historico de eventos`
