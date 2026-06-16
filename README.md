# Bolão Copa do Mundo 2026

Projeto para bolão familiar da Copa do Mundo 2026.

## Versão 1.1

Esta versão inclui:

- Login por **nome ou e-mail**.
- E-mail **opcional** no cadastro e na criação de participantes.
- Visual renovado em tema dark premium.
- Interface em português do Brasil.
- Times da Copa em português.
- Bandeiras por seleção nos cards dos jogos.
- Botão de admin para carregar/atualizar a tabela da Copa 2026.

> Observação: para Inglaterra e Escócia, alguns navegadores/sistemas podem exibir apenas a bandeira regional preta, pois o suporte a esses emojis depende do sistema operacional.

## Stack

- Backend: .NET 8 Web API
- Frontend: React + Vite
- Banco: PostgreSQL
- Deploy sugerido: Render
- Autenticação: JWT

## Estrutura

```txt
bolao-copa-2026/
├── backend/
│   └── BolaoCopa.Api/
├── frontend/
│   └── bolao-copa-web/
├── database/
├── docs/
├── render.yaml
└── README.md
```

## Como rodar localmente usando banco do Render

No PowerShell:

```powershell
cd C:\Users\GGONQ8\Desktop\bolao-copa-2026\backend\BolaoCopa.Api

$env:ASPNETCORE_URLS='http://localhost:5080'
$env:DATABASE_URL='COLE_AQUI_A_EXTERNAL_DATABASE_URL_DO_RENDER'
$env:JWT_SECRET='bolao-copa-2026-chave-local-desenvolvimento-123456789'

dotnet restore
dotnet run
```

API local:

```txt
http://localhost:5080
```

Swagger:

```txt
http://localhost:5080/swagger
```

Frontend:

```powershell
cd C:\Users\GGONQ8\Desktop\bolao-copa-2026\frontend\bolao-copa-web

$env:VITE_API_URL='http://localhost:5080/api'

npm install
npm run dev
```

Frontend local:

```txt
http://localhost:5173
```

## Como rodar localmente com Docker

```bash
docker compose up -d
```

O banco local sobe em:

```txt
Host: localhost
Porta: 5432
Database: bolao_copa_2026
User: bolao_user
Password: bolao_pass
```

Depois rode o backend e o frontend conforme as instruções acima.

## Primeiro acesso

1. Abra o frontend.
2. Clique em **Criar conta**.
3. Informe o **nome**, senha e, se quiser, o e-mail.
4. O primeiro usuário cadastrado vira **admin** automaticamente.
5. Entre com esse usuário.
6. Vá em **Administração**.
7. Clique em **Carregar/atualizar tabela Copa 2026**.

## Atualização da versão anterior

Se você já tinha carregado a tabela antes, entre em **Administração** e clique novamente em:

```txt
Carregar/atualizar tabela Copa 2026
```

Isso atualiza os nomes dos países para português no banco.

A API também aplica automaticamente um ajuste de compatibilidade no banco para permitir e-mail em branco nos usuários já existentes.

## Usuários seguintes

Todos os próximos cadastros entram como `participant`.

O admin também pode criar participantes pela tela **Administração**. O e-mail é opcional.

## Regras de pontuação padrão

| Regra | Pontos |
|---|---:|
| Placar exato | 10 |
| Acertou vencedor/empate e diferença de gols | 6 |
| Acertou vencedor/empate | 3 |
| Errou | 0 |

O palpite bloqueia 5 minutos antes do início do jogo.

## Variáveis de ambiente do backend

```env
DATABASE_URL=postgresql://bolao_user:bolao_pass@localhost:5432/bolao_copa_2026
JWT_SECRET=troque_essa_chave_por_uma_chave_grande_e_segura
FRONTEND_URL=http://localhost:5173
AUTO_CREATE_DATABASE=true
ASPNETCORE_URLS=http://+:5080
```

No Render, use a `DATABASE_URL` do banco PostgreSQL.

## Variáveis de ambiente do frontend

```env
VITE_API_URL=http://localhost:5080/api
```

## Observações sobre a tabela

O seed inicial inclui:

- 48 seleções separadas em 12 grupos.
- Jogos da fase de grupos.
- Jogos do mata-mata como placeholders.

Os jogos de mata-mata podem ser atualizados pelo admin quando os classificados forem definidos.
