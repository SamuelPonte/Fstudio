# Fstudio — Website de Fotografia de Casamentos

Trabalho Prático de Desenvolvimento Web  
Licenciatura em Engenharia Informática — 2.º ano, 2.º semestre  
Instituto Politécnico de Tomar

---

## Índice

1. [Descrição do Projeto](#descrição-do-projeto)
2. [Tecnologias Utilizadas](#tecnologias-utilizadas)
3. [Arquitetura da Aplicação](#arquitetura-da-aplicação)
4. [Base de Dados](#base-de-dados)
5. [Funcionalidades](#funcionalidades)
6. [API REST](#api-rest)
7. [SignalR](#signalr)
8. [Estrutura de Ficheiros](#estrutura-de-ficheiros)
9. [Instalação e Execução](#instalação-e-execução)
10. [Credenciais de Demonstração](#credenciais-de-demonstração)
11. [Autores](#autores)

---

## Descrição do Projeto

O **Fstudio** é uma aplicação web para um estúdio de fotografia e vídeo de casamentos sediado em Lisboa, Portugal. O projeto é um redesign do site real [fstudio.info](http://fstudio.info).

A aplicação permite:
- Apresentar publicamente o portfólio de fotografias e filmes do estúdio
- Gerir todo o conteúdo através de um painel de administração protegido
- Fornecer aos clientes uma área privada para visualizar e descarregar as suas fotografias
- Receber pedidos de contacto com notificações em tempo real para o administrador
- Expor uma API REST para consulta e gestão de conteúdos

---

## Tecnologias Utilizadas

### Obrigatórias (conforme requisitos)

| Tecnologia | Versão | Finalidade |
|---|---|---|
| ASP.NET Core Razor Pages | 10.0 | Interface Web (Componente 1) |
| ASP.NET Core MVC (Controllers) | 10.0 | API REST (Componente 2) |
| Entity Framework Core | 10.0 | Acesso à base de dados |
| LINQ | — | Consultas à base de dados |
| SignalR | 10.0 | Notificações em tempo real |
| ASP.NET Core Identity | 10.0 | Autenticação e autorização |

### Opcionais

| Tecnologia | Finalidade |
|---|---|
| Bootstrap 5.3 | Framework CSS responsivo |
| Bootstrap Icons 1.11 | Ícones vetoriais |
| Google Fonts (Cormorant Garamond, Great Vibes, Montserrat) | Tipografia artística |
| jQuery + jQuery Validation | Validação de formulários no cliente |
| Swashbuckle (Swagger) | Documentação interativa da API |
| Microsoft SignalR (cliente JS) | Ligação WebSocket no browser |

### Base de Dados

- **Desenvolvimento:** SQL Server LocalDB (`(localdb)\mssqllocaldb`)
- **Produção:** SQL Server (Azure SQL / servidor dedicado)

---

## Arquitetura da Aplicação

A aplicação segue o padrão **Razor Pages** para a interface web e **MVC Controllers** para a API REST, ambos sobre ASP.NET Core 10.

```
Fstudio/
├── Areas/
│   ├── Admin/         ← Painel de administração (Razor Pages, role "Admin")
│   ├── Cliente/       ← Área privada do cliente (Razor Pages, role "Cliente")
│   └── Identity/      ← Páginas de autenticação (Login, Registo, etc.)
├── Controllers/
│   └── Api/           ← API REST (MVC Controllers)
├── Data/
│   ├── Models/        ← Entidades da base de dados
│   │   └── ViewModels/ ← DTOs para a API REST
│   └── Seed/          ← Dados iniciais (DataSeeder)
├── Hubs/              ← Hub SignalR
├── Migrations/        ← Migrações EF Core
├── Pages/             ← Páginas públicas (Razor Pages)
└── wwwroot/           ← Ficheiros estáticos (CSS, JS, imagens)
```

### Tipos de Utilizadores

| Role | Acesso | Credenciais de Demo |
|---|---|---|
| **Admin** | Painel de administração completo + site público | `admin@fstudio.info` / `Admin123!` |
| **Cliente** | Área privada com galeria e download | `cliente@demo.com` / `Cliente123!` |
| **Visitante** | Site público (portfólio, contacto, FAQ, etc.) | — |

Os novos registos ficam em estado **Pendente** até o administrador os ativar.

---

## Base de Dados

### Diagrama de Entidades

```
ApplicationUser (Identity)
    │
    ├──< Cliente (1:1 com ApplicationUser)
    │       │
    │       └──< ClienteFotografia >── Fotografia ──> Categoria
    │       │
    │       └──< Testemunho
    │
Contacto (independente — enviado por visitantes)
```

### Tabelas

| Tabela | Descrição |
|---|---|
| `AspNetUsers` | Utilizadores (gerida pelo Identity) |
| `AspNetRoles` | Roles: Admin, Cliente |
| `Clientes` | Dados adicionais dos clientes (evento, estado, etc.) |
| `Categorias` | Categorias de fotografias (Casamentos, Noivados, etc.) |
| `Fotografias` | Fotografias do portfólio |
| `ClienteFotografias` | Relação N:M entre Clientes e Fotografias |
| `Testemunhos` | Testemunhos deixados pelos clientes |
| `Contactos` | Mensagens de contacto do formulário público |

### Relações

- **1:N** — Categoria → Fotografias
- **1:N** — Cliente → Testemunhos
- **N:M** — Clientes ↔ Fotografias (tabela `ClienteFotografias`)
- **1:1** — ApplicationUser ↔ Cliente

---

## Funcionalidades

### Site Público

| Página | URL | Descrição |
|---|---|---|
| Início | `/` | Hero, portfólio em destaque, testemunhos e contacto |
| Portfólio | `/Portfolio` | Galeria com filtro por categoria |
| Filmes | `/Filmes` | Vídeos de casamentos |
| Sobre | `/Sobre` | Informação sobre o estúdio |
| Contacto | `/Contacto` | Formulário de contacto |
| FAQ | `/FAQ` | Perguntas frequentes |
| Créditos | `/Creditos` | Informação do projeto académico |

### Painel de Administração (`/Admin`)

| Secção | Funcionalidades |
|---|---|
| Dashboard | Estatísticas gerais (total de fotografias, clientes, contactos não lidos) |
| Fotografias | CRUD completo + upload de imagem (JPG, PNG, WebP, GIF, máx. 10 MB) |
| Categorias | CRUD completo com slug para URL amigável |
| Clientes | CRUD + gestão de estado (Pendente / Ativo / Inativo) + galeria privada |
| Contactos | Listagem, detalhe, marcar como lido, arquivar |
| Testemunhos | Listagem e aprovação para exibição pública |

### Área do Cliente (`/Cliente`)

| Página | Descrição |
|---|---|
| Galeria | Visualização das fotografias do evento do cliente |
| Download | Download das fotografias em alta resolução |

---

## API REST

Documentação interativa disponível em `/swagger` (apenas em modo de desenvolvimento).

### Base URL: `/api`

#### Categorias — `/api/categorias`

| Método | Endpoint | Auth | Descrição |
|---|---|---|---|
| GET | `/api/categorias` | Público | Listar categorias (filtro: `?activa=true`) |
| GET | `/api/categorias/{id}` | Público | Obter categoria por ID |
| GET | `/api/categorias/slug/{slug}` | Público | Obter categoria por slug |
| POST | `/api/categorias` | Admin | Criar categoria |
| PUT | `/api/categorias/{id}` | Admin | Atualizar categoria |
| DELETE | `/api/categorias/{id}` | Admin | Eliminar categoria |

#### Fotografias — `/api/fotografias`

| Método | Endpoint | Auth | Descrição |
|---|---|---|---|
| GET | `/api/fotografias` | Público | Listar fotografias (filtros: `categoriaId`, `destaque`, `visivel`) |
| GET | `/api/fotografias/{id}` | Público | Obter fotografia por ID |
| POST | `/api/fotografias` | Admin | Criar fotografia |
| PUT | `/api/fotografias/{id}` | Admin | Atualizar fotografia |
| DELETE | `/api/fotografias/{id}` | Admin | Eliminar fotografia |

#### Contactos — `/api/contactos`

| Método | Endpoint | Auth | Descrição |
|---|---|---|---|
| POST | `/api/contactos` | Público | Submeter mensagem de contacto |
| GET | `/api/contactos` | Admin | Listar contactos (filtros: `lido`, `arquivado`) |
| GET | `/api/contactos/{id}` | Admin | Obter contacto (marca como lido) |
| PUT | `/api/contactos/{id}/marcar-lido` | Admin | Marcar como lido |
| PUT | `/api/contactos/{id}/arquivar` | Admin | Arquivar contacto |
| DELETE | `/api/contactos/{id}` | Admin | Eliminar contacto |
| GET | `/api/contactos/nao-lidos/count` | Admin | Contagem de não lidos |

---

## SignalR

O hub de notificações está disponível em `/hubs/notificacao` e é acessível apenas por utilizadores com role **Admin**.

### Eventos

| Evento | Sentido | Descrição |
|---|---|---|
| `NovoContacto` | Servidor → Cliente | Notifica quando alguém submete o formulário de contacto |
| `ReceberNotificacao` | Servidor → Cliente | Notificação genérica com título e mensagem |

O painel de administração conecta-se automaticamente ao hub e exibe um toast de notificação quando um novo contacto é recebido, atualizando também o badge na sidebar.

---

## Estrutura de Ficheiros

```
Fstudio/
├── Fstudio/                          ← Projeto principal
│   ├── Areas/
│   │   ├── Admin/Pages/
│   │   │   ├── Categorias/           ← CRUD categorias
│   │   │   ├── Clientes/             ← CRUD clientes + galeria
│   │   │   ├── Contactos/            ← Gestão de contactos
│   │   │   ├── Fotografias/          ← CRUD fotografias + upload
│   │   │   ├── Testemunhos/          ← Gestão de testemunhos
│   │   │   ├── Index.cshtml          ← Dashboard
│   │   │   └── Shared/_AdminLayout.cshtml
│   │   ├── Cliente/Pages/
│   │   │   ├── Index.cshtml          ← Galeria privada
│   │   │   └── Download.cshtml       ← Download de fotografias
│   │   └── Identity/Pages/Account/
│   │       ├── Login.cshtml
│   │       ├── Register.cshtml
│   │       ├── Logout.cshtml
│   │       ├── ForgotPassword.cshtml
│   │       └── AccessDenied.cshtml
│   ├── Controllers/Api/
│   │   ├── CategoriasController.cs
│   │   ├── FotografiasController.cs
│   │   └── ContactosController.cs
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Models/                   ← Entidades EF Core
│   │   │   ├── ApplicationUser.cs
│   │   │   ├── Categoria.cs
│   │   │   ├── Cliente.cs
│   │   │   ├── ClienteFotografia.cs
│   │   │   ├── Contacto.cs
│   │   │   ├── Fotografia.cs
│   │   │   ├── Testemunho.cs
│   │   │   └── ViewModels/           ← DTOs para a API
│   │   └── Seed/DataSeeder.cs
│   ├── Hubs/NotificacaoHub.cs
│   ├── Migrations/
│   ├── Pages/                        ← Páginas públicas
│   │   ├── Index.cshtml              ← Página inicial
│   │   ├── Portfolio/Index.cshtml
│   │   ├── Filmes.cshtml
│   │   ├── Sobre.cshtml
│   │   ├── Contacto.cshtml
│   │   ├── FAQ.cshtml
│   │   ├── Creditos.cshtml
│   │   ├── StatusCode.cshtml
│   │   └── Shared/_Layout.cshtml
│   ├── wwwroot/
│   │   └── uploads/fotografias/      ← Imagens carregadas pelo admin
│   ├── Program.cs
│   ├── appsettings.json
│   └── Fstudio.csproj
├── REQUISITOS.md
└── README.md
```

---

## Instalação e Execução

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server ou SQL Server LocalDB (Windows) / Docker (macOS/Linux)

### SQL Server via Docker (macOS/Linux)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Fstudio123!" \
  -p 1433:1433 --name fstudio-sqlserver -d \
  mcr.microsoft.com/mssql/server:2022-latest
```

Atualizar `appsettings.json`:
```json
"DefaultConnection": "Server=localhost,1433;Database=FstudioDb;User Id=sa;Password=Fstudio123!;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Executar a aplicação

```bash
# Clonar o repositório
git clone https://github.com/SamuelPonte/Fstudio.git
cd Fstudio

# Restaurar dependências e executar
dotnet run --project Fstudio/Fstudio/Fstudio.csproj

# A aplicação fica disponível em:
# http://localhost:5173
# https://localhost:7086
```

A base de dados é criada e populada automaticamente no primeiro arranque (migrações + seed de dados).

### Comandos úteis

```bash
# Criar nova migração
dotnet ef migrations add NomeDaMigracao --project Fstudio/Fstudio/Fstudio.csproj

# Aplicar migrações manualmente
dotnet ef database update --project Fstudio/Fstudio/Fstudio.csproj

# Build do projeto
dotnet build Fstudio/Fstudio/Fstudio.csproj
```

---

## Credenciais de Demonstração

| Conta | Email | Password | Acesso |
|---|---|---|---|
| Administrador | `admin@fstudio.info` | `Admin123!` | Painel de administração completo |
| Cliente Demo | `cliente@demo.com` | `Cliente123!` | Área privada com galeria de demonstração |

---

## Autores

| Nome | Número | Contribuições principais |
|---|---|---|
| André Vassalo | aluno27454 | Páginas públicas, Identity, área de administração, API REST, seed de dados, validação de uploads, Azure |
| Samuel Ponte | aluno27227 | Modelos de dados, DbContext, SignalR, área do cliente, SQL Server, responsividade admin |

**Instituto Politécnico de Tomar**  
Licenciatura em Engenharia Informática — 2.º ano, 2.º semestre  
Ano letivo 2025/2026
