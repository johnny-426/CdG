# Roteiro de validação local

Use PostgreSQL e três contas fictícias A, B e C em sessões separadas. Não use dados reais nesta validação.

1. Cadastro/login: crie as contas, saia, entre novamente, altere nome. Em produção, teste confirmação e recuperação de senha com SMTP.
2. A cria evento com limite três; B e C aceitam. Confira notificações e confirme também A.
3. Abra uma conta D e tente aceitar sem vaga: deve bloquear. Uma presença cancelada libera a vaga.
4. Edite em duas abas: salve a primeira e tente salvar a segunda desatualizada. Deve rejeitar a alteração antiga. Repita em Adiar.
5. Promova B e confira suas permissões. Teste sair com o único organizador e um sucessor elegível. Não deve ficar sem organizador.
6. Em evento independente, A registra 100 e B registra 20. B não consegue anular a despesa de A. Quem registrou despesa ativa não consegue sair/cancelar a presença sem anulá-la.
7. Anule uma despesa errada e registre outra. O histórico deve mostrar a anulação; o total deve ignorá-la.
8. Chegado o horário, conclua o evento. A fecha rateio de 120 entre três. Cotas 40; A +60; B -20; C -40. B não pode fechar. Nova despesa não pode ser criada após fechar.
9. B informa 20 para A: saldos não mudam ainda. B não consegue confirmar recebimento. A confirma: A +40; B zero; C -40.
10. C informa 40, cancela o registro e confira que a dívida continua. Informe novamente e confirme com A: todos zerados.
11. Em outro evento, teste pagamento parcial; após confirmação, confira o saldo restante. Tente valor acima da dívida, zero, negativo, texto e três casas decimais: devem ser rejeitados.
12. Em outro evento fechado, A abre mão do crédito. Deve reduzir dívidas e registrar cada renúncia. Se houver acerto pendente, a renúncia deve ser bloqueada.
13. Cancele um evento com despesas. Confira que o financeiro permanece acessível, despesas devolvidas podem ser anuladas antes de fechar e custos restantes podem ser rateados. Cancelar não significa quitar.
14. Oculte/restaure um evento concluído. As demais contas continuam vendo seus próprios históricos.
15. Copie URL do financeiro para uma conta não vinculada: acesso deve ser negado. Tente enviar IDs de despesas/acertos de outro evento: nenhuma alteração deve ocorrer.
16. Concorrência PostgreSQL: duas sessões tentam aceitar a última vaga; apenas uma deve conseguir. Dois organizadores tentam sair juntos; deve sobrar organizador ou bloquear a saída.
17. Concorrência financeira: tente fechar e registrar despesa ao mesmo tempo; a despesa deve entrar antes do fechamento ou ser recusada depois. Dois devedores tentam informar pagamento para o mesmo credor ao mesmo tempo; o segundo deve aguardar o primeiro. Confirme o mesmo recebimento em duas abas: contabilização única.
18. Reinicie o aplicativo: eventos, despesas, cotas e acertos devem continuar disponíveis.

Para relatar erro, envie a mensagem completa, a tela e o passo acima. Não envie senha do banco nem credenciais SMTP.
