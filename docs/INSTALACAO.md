# CdG — guia de instalação e atualização

Versão integrada de 18/10/2026 · João Paulo Ramalho · Sistemas de Informação / Uniara

Esta entrega continua o seu projeto. Base: `CdG Completo.zip` de 14/10/2026, com o pacote `CdG-Correcoes.zip` aplicado, acrescido do financeiro. Não foi criada uma aplicação substituta. Mantém Razor Pages, Identity, PostgreSQL, páginas existentes e migrations anteriores.

## 1. O que baixar e abrir

Extraia `CdG-v1-Completo.zip` em uma pasta NOVA, por exemplo `C:\Projetos\CdG-v1`. Dentro está a pasta `CdG`; nela há `CdG.slnx`, `README.md`, `Dockerfile`, `docs`, `tests` e a subpasta `CdG` com o projeto. Abra **CdG.slnx** no Visual Studio. Se a sua versão não abrir `.slnx`, abra diretamente `CdG\CdG.csproj`.

Não extraia por cima da única cópia anterior. Esta entrega contém o projeto completo; o ZIP antigo de correções era apenas um patch. Preserve seu repositório antigo e seu histórico Git até validar esta versão. Não execute `git reset`, nem substitua a pasta `.git`.

## 2. Pré-requisitos

- Windows com Visual Studio capaz de desenvolver para .NET 10; carga de trabalho **ASP.NET e desenvolvimento Web**.
- SDK .NET 10 (o `global.json` aceita a família 10.0 com avanço para versões de recursos posteriores).
- Seu PostgreSQL local em execução. O pgAdmin é opcional, mas facilita criar o banco.
- Acesso à internet no primeiro restore dos pacotes NuGet.

No Terminal do Visual Studio: `dotnet --list-sdks`. Deve existir uma versão `10.0.xxx`. Não altere o TargetFramework para versões anteriores.

## 3. Criar um banco limpo para os testes

Como você informou que os dados antigos são descartáveis, o caminho mais simples é usar outro banco, preservando o anterior sem precisar apagá-lo.

No pgAdmin: conecte ao servidor local → Databases → Create → Database → nome **cdg_v1_teste** → Save. Use o usuário PostgreSQL que você já utilizava. Guarde a senha apenas no seu computador.

## 4. Configurar a conexão sem colocar senha no código

No Visual Studio, clique com o botão direito no projeto **CdG** → **Gerenciar Segredos do Usuário**. Configure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cdg_v1_teste;Username=postgres;Password=SUA_SENHA"
  }
}
```

Troque usuário, senha e porta pelos seus dados. A chave precisa se chamar **DefaultConnection**. Como preservamos o UserSecretsId do projeto, essa configuração também pode ser lida pela cópia antiga no mesmo computador. Confira sempre o banco antes de executar migrations.

Alternativa no terminal, dentro da pasta que contém `CdG.csproj`:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=cdg_v1_teste;Username=postgres;Password=SUA_SENHA"
```

## 5. Restaurar, compilar e aplicar as migrations

No Visual Studio: **Compilar → Recompilar Solução**. Depois abra **Ferramentas → Gerenciador de Pacotes NuGet → Console do Gerenciador de Pacotes**. Selecione **CdG** como projeto padrão e execute, nessa ordem:

```powershell
Get-Migration
Update-Database
```

A última migration desta entrega é **20261018123556_AdicionarFinanceiro**. Não execute `Add-Migration`: ela já está incluída, com Designer e snapshot gerados pelo EF. Todas as migrations anteriores foram preservadas. A migration `PreencherAceitesAntigos` já bloqueava seu próprio rollback; não tente retroceder através dela.

Alternativa sem o console NuGet: na pasta do projeto, execute:

```powershell
dotnet restore
dotnet build
dotnet run -- --migrate
```

O último comando somente aplica migrations e encerra. Não inicia o site. Use um dos caminhos; não é necessário executar ambos. O arquivo `docs/MIGRACOES.sql` contém o SQL idempotente gerado pelo EF para análise ou aplicação administrada; não é necessário executá-lo manualmente junto com `Update-Database`.

## 6. Executar

Selecione o perfil **https** no Visual Studio e use **Ctrl+F5**. O perfil existente usa `https://localhost:7103` e `http://localhost:5171`. Se o Windows solicitar confiança no certificado local, aceite para desenvolvimento; alternativamente:

```powershell
dotnet dev-certs https --trust
```

No terminal, dentro de `CdG\CdG`:

```powershell
dotnet run --launch-profile https
```

Crie três contas fictícias, usando janelas/perfis separados do navegador. Em Development, não é exigida confirmação de e-mail e, sem SMTP, nenhuma mensagem é enviada. Recuperação de senha por e-mail depende de configurar SMTP.

## 7. Como usar o financeiro

1. Crie um evento com data próxima e convide as contas B e C. Confirme as três presenças, inclusive a do organizador se ele participar.
2. Em **Detalhes → Despesas e rateio**, cada pessoa registra os gastos que ela própria pagou. Não registre transferências entre participantes como despesas.
3. Antes do fechamento, o autor pode anular uma despesa errada e cadastrar a correta. O registro anulado permanece no histórico.
4. Depois de chegar o horário, conclua o evento. Em caso de cancelamento, registre somente custos efetivos que não foram devolvidos; anule despesas reembolsadas antes de fechar.
5. O organizador acessa o financeiro e **fecha o rateio**. A divisão usa todos os confirmados naquele momento. Cotas e despesas ficam imutáveis.
6. Quem deve informa um pagamento feito fora do sistema. O recebedor entra na sua conta e confirma. Só então o saldo muda.
7. Um pagamento incorreto ainda pendente pode ser cancelado por qualquer uma das duas pessoas. Acertos confirmados não têm estorno nesta versão: confira antes de confirmar.
8. Quem tem crédito pode abrir mão de todo o restante. A confirmação é definitiva; gera registros de renúncia, reduzindo dívidas em ordem fixa de IDs até consumir o crédito. Não é divisão proporcional da doação.

Exemplo: A pagou R$ 100, B pagou R$ 20 e C não pagou nada. Total R$ 120; cota R$ 40. A recebe R$ 60, B paga R$ 20 e C paga R$ 40. Após ambos os recebimentos confirmados, todos ficam com saldo zero.

## 8. Atualizar seu banco antigo, se preferir

Você também pode manter a conexão antiga e executar `Get-Migration` / `Update-Database`. A nova migration adiciona estruturas e não apaga os dados existentes. Ainda assim, faça backup antes de atualizar qualquer banco que deseje preservar. Não use `EnsureCreated` nem apague migrations para essa atualização.

## 9. Testes antes da apresentação

Use `docs/TESTES.md`. A compilação e os testes automatizados desta entrega estão registrados em `docs/VALIDACAO.md`. Para repetir os testes automatizados, a partir da pasta que contém `CdG.slnx`:

```powershell
dotnet run --project tests/CdG.Verificacao
```

Eles usam banco SQLite em memória e não modificam seu PostgreSQL. Não substituem os testes de concorrência no PostgreSQL e o uso das telas com três contas.

## 10. Problemas comuns

| Sintoma | O que conferir |
| --- | --- |
| SDK / net10.0 não suportado | Instale SDK .NET 10 e confira a compatibilidade do Visual Studio. |
| DefaultConnection não configurada | Segredos do usuário no projeto CdG; chave exata. |
| Connection refused | Serviço PostgreSQL, host e porta. |
| Password authentication failed | Usuário/senha PostgreSQL; não é a conta cadastrada no site. |
| Database does not exist | Crie `cdg_v1` ou ajuste o nome da conexão. |
| Tabela ou coluna não existe | Execute `Update-Database` no banco que está na conexão usada pelo aplicativo. |
| Certificado HTTPS | Execute `dotnet dev-certs https --trust`. |
| Financeiro não fecha | Evento deve estar concluído/cancelado; precisa de confirmados; todos os autores de despesas ativas precisam estar entre eles. |
| Sair/cancelar presença bloqueado | Anule suas despesas antes, ou permaneça no evento para os acertos. |
| Evento sumiu após lixeira | Em Meus eventos, exiba ocultos e restaure. Ocultar não apaga dados nem dívidas. |
| Pagamento aguarda confirmação | Entre como recebedor. Só ele confirma o recebimento. |

## 11. Colocar na internet

Não exige reescrever o CdG. São necessários um serviço que execute ASP.NET Core/.NET 10 ou Docker, PostgreSQL acessível ao serviço, HTTPS e configuração de e-mail. A dificuldade principal é configurar esses serviços e validar o funcionamento. A contratação e publicação não foram realizadas nesta entrega.

O projeto inclui um Dockerfile com compilação em duas etapas e execução sem root. A partir da pasta com `Dockerfile`, uma plataforma Docker pode executar:

```powershell
docker build -t cdg:v1 .
```

O container atende internamente na porta **8080**. A plataforma deve fornecer HTTPS externamente. Configure nela, sem commitar senhas:

| Variável | Valor / finalidade |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | Conexão Npgsql fornecida pelo PostgreSQL hospedado, com TLS conforme exigido pelo provedor. |
| `AllowedHosts` | Nome real do domínio do site. |
| `Smtp__Host` | Host SMTP. |
| `Smtp__Port` | Porta STARTTLS, normalmente 587. |
| `Smtp__Username` | Usuário SMTP. |
| `Smtp__Password` | Senha ou credencial de aplicação. |
| `Smtp__From` | Remetente autorizado no SMTP. |
| `ReverseProxy__KnownProxy` | IP do proxy confiável, quando necessário. A integração precisa ser ajustada à plataforma escolhida. |
| `DataProtection__KeysPath` | Pasta persistente e gravável para as chaves de autenticação, com acesso restrito. |

Em Production, o cadastro exige confirmação de e-mail. Configure e teste SMTP antes de abrir cadastros. O remetente implementado usa STARTTLS; não use uma configuração que exija TLS implícito na porta 465 sem adaptar o serviço.

Execute `dotnet CdG.dll --migrate` em uma tarefa de implantação com acesso ao banco, antes de iniciar o site. Migrations não são executadas automaticamente em cada inicialização. Preserve o volume de chaves de Data Protection entre deploys; sua proteção em repouso depende do ambiente de hospedagem. Não habilite confiança irrestrita em cabeçalhos de proxy.

Antes do uso externo: valide cadastro/confirmar e-mail/recuperar senha, configure backup do banco, revise o texto de privacidade com um contato real e trate a limitação de exclusão de contas com histórico. Esta versão pode servir como demonstração acadêmica; não está certificada para operação pública. O Dockerfile foi preparado, mas a imagem não foi construída aqui.

Referências oficiais consultadas em 18/10/2026:

- https://learn.microsoft.com/aspnet/core/host-and-deploy/docker/building-net-docker-images
- https://learn.microsoft.com/aspnet/core/fundamentals/environments?view=aspnetcore-10.0
- https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-10.0

## 12. O que fica para depois

A exigência específica da extensão, o documento acadêmico final, a hospedagem efetiva, o rateio seletivo por despesa, estornos de acertos confirmados e a exclusão/anônimização de contas com histórico. O módulo de futebol permanece um projeto separado. Os diagramas antigos recuperados são referências históricas e não representam automaticamente todas as tabelas desta versão.
