# Publicação no GitHub — passo a passo sem perder o histórico

Este roteiro é para o repositório **`johnny-426/CdG`**, que já existe no GitHub e está privado. A versão `CdG-v1-Completo` foi entregue como ZIP **sem a pasta `.git`**; portanto, não substitua a pasta de um clone antigo apagando `.git` e não crie um histórico novo desnecessariamente.

## 1. Antes de mexer no Git

1. Guarde o ZIP funcional como backup.
2. Mantenha seu PostgreSQL antigo e o banco de testes separados. Nenhum banco de dados ou senha precisa entrar no repositório.
3. Confirme que possui o Visual Studio e Git instalados.
4. Veja `docs/VALIDACAO.md` e `docs/TESTES.md`: ainda há cenários de concorrência, financeiro ponta a ponta, SMTP e hospedagem a validar.

## 2. Método recomendado: clonar o repositório e copiar os arquivos

1. No Visual Studio, use **Git → Clonar Repositório** e informe `https://github.com/johnny-426/CdG.git`. Escolha uma **pasta nova**, por exemplo `C:\Projetos\CdG-Git`.
2. Feche a aplicação que estiver em execução. Mantenha a pasta clonada e a pasta extraída do ZIP abertas no Explorador.
3. Copie para a **raiz do clone** o conteúdo de dentro da pasta `CdG` do ZIP atualizado: `CdG.slnx`, pasta interna `CdG/`, `docs/`, `tests/`, `README.md`, `.gitignore`, `Dockerfile` etc. Aceite mesclar pastas e substituir arquivos correspondentes.
4. **Não exclua nem substitua `.git` do clone.** O ZIP atualizado não a contém. Não copie `secrets.json`, `.env`, `bin`, `obj`, `.vs` nem cópias de banco.
5. Abra `CdG.slnx` no clone, configure a conexão local com **Gerenciar Segredos de Usuário**, compile e rode no banco de testes.
6. Compare as modificações na janela **Git Changes** do Visual Studio. Arquivos removidos, arquivos novos e alterações inesperadas merecem inspeção antes do commit.

Se quiser usar terminal PowerShell na pasta clonada:

```powershell
git status
git remote -v
git diff --stat
git diff --check
```

`git remote -v` deve mostrar `johnny-426/CdG`. Não use `git init` no clone nem `git push --force`.

### Atenção: a versão antiga ainda existe no GitHub

O repositório remoto já contém uma versão anterior do CdG, enquanto o ZIP mais recente inclui serviços de financeiro, novas páginas e documentação. Copiar somente o README não publicará a aplicação atual. É necessário integrar todo o código-fonte atualizado e revisar a lista de mudanças.

## 3. Revise informações sensíveis antes do primeiro push

- Nunca adicione o `secrets.json`, mesmo se estiver fora do diretório do repositório. User Secrets geralmente ficam numa pasta de perfil do Windows, mas podem existir cópias locais.
- Revise `.gitignore` e `git status --short` para assegurar que `.vs/`, `bin/`, `obj/`, `*.pfx`, `.env` e configurações locais não estejam sendo incluídos.
- Abra a aba de alterações do Visual Studio e confira se não há senhas, tokens, strings de conexão com senha nem dados pessoais em arquivos ou capturas de tela.
- **Cuidado com o histórico Git**: `.gitignore` impede novas adições, mas **não remove segredos já publicados em commits anteriores**. Se encontrar segredo no histórico, rotacione-o e trate o histórico antes de tornar o repositório público.
- Confira que `README.md` e guias descrevem somente funcionalidades existentes. Os diagramas de `docs/historico` devem estar marcados como históricos quando ainda não corresponderem à versão atual.

Um comando auxiliar de inspeção (não substitui revisão manual):

```powershell
git status --short
git diff --check
git diff --cached --stat
```

## 4. Commit e push (somente após revisar)

No Visual Studio, use a tela **Git Changes** para selecionar as alterações, criar um commit e fazer push. Alternativa pelo PowerShell:

```powershell
git add README.md .gitignore CdG/ docs/ tests/ Dockerfile CdG.slnx global.json .dockerignore .gitattributes
git status --short
git diff --cached --stat
git commit -m "docs: documenta CdG e integra versão financeira"
git push origin master
```

Antes do `commit`, veja se existem outras mudanças importantes fora dessa seleção ou arquivos inesperados dentro dela. `git add` não deve ser executado às cegas. Se a branch atual for diferente de `master`, use o nome real indicado por `git branch --show-current`. Se o push acusar divergência, pare e verifique o estado; **não** force a atualização.

## 5. Como tornar o projeto público

Depois de confirmar que o repositório está seguro:

1. Abra `https://github.com/johnny-426/CdG` no navegador.
2. Em **Settings → General → Danger Zone → Change repository visibility**, selecione **Public**, se esse menu estiver disponível para seu repositório.
3. Revise o README renderizado e todos os links de `docs/`.
4. Configure a descrição do repositório, por exemplo: “Aplicação web em ASP.NET Core e PostgreSQL para organização de eventos, convites e rateio de despesas.”
5. Adicione tópicos como `csharp`, `dotnet`, `aspnet-core`, `postgresql`, `entity-framework-core`, `razor-pages`, `portfolio`.
6. Fixe o repositório no seu perfil GitHub e crie uma seção com o projeto no README de perfil e no LinkedIn, mencionando claramente que a aplicação está em desenvolvimento e sem hospedagem pública, caso ainda seja o caso.

**Alternativa segura:** mantenha o repo privado enquanto finaliza as capturas, os testes e a revisão. O histórico de commits já pode ser organizado; a visibilidade pode ser alterada depois.

## 6. O que deve estar no README público

- Problema e proposta de solução em poucas frases.
- Principais funcionalidades e tecnologias realmente utilizadas.
- Imagens **reais** e legíveis da interface, sem dados pessoais.
- Arquitetura resumida e instruções que funcionem em uma máquina nova.
- Links para estudo, apresentação, validação e testes.
- Limitações conhecidas e próximos passos (sem anunciar funcionalidades futuras como concluídas).

## 7. Checklist final

- [ ] O repositório segue usando o histórico antigo.
- [ ] Código atual, `.csproj`, migrations, `tests/` e documentos estão presentes.
- [ ] `git status` não mostra dados locais sensíveis.
- [ ] O banco configurado na máquina é o banco de testes correto.
- [ ] Compilação e testes essenciais foram repetidos após copiar para o clone.
- [ ] O README abre no GitHub, com links funcionando.
- [ ] Imagens não expõem dados reais.
- [ ] Limitações de financeiro e hospedagem estão descritas.
- [ ] Só agora a visibilidade poderá mudar para pública.
