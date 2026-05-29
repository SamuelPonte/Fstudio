# Fstudio - Wedding Photography

Aplicação web para estúdio de fotografia de casamentos, desenvolvida como Trabalho Prático da Unidade Curricular de Desenvolvimento Web.

**Licenciatura em Engenharia Informática** — 2º Ano, 2º Semestre

---

## Sobre o Projeto

O **Fstudio** é uma aplicação web moderna para um estúdio de fotografia de casamentos, inspirada no site [fstudio.info](http://fstudio.info). A aplicação inclui:

- **Frontend público** — Portfólio, páginas informativas e formulário de contacto
- **Área de Cliente** — Galeria privada para visualização e download de fotografias
- **Backoffice Admin** — Gestão completa de conteúdos (CRUD)
- **API REST** — Endpoints para integração externa
- **Notificações em tempo real** — Via SignalR

---

## Tecnologias Utilizadas

| Tecnologia | Versão | Utilização |
|------------|--------|------------|
| ASP.NET Core | 10.0 | Framework principal |
| Razor Pages | - | Interface do utilizador (Componente 1) |
| ASP.NET Core MVC | - | API REST (Componente 2) |
| Entity Framework Core | 10.0 | Acesso à base de dados |
| SQLite | - | Base de dados |
| ASP.NET Core Identity | - | Autenticação e autorização |
| SignalR | - | Comunicação em tempo real |
| Bootstrap | 5.3 | Layout responsivo |
| Bootstrap Icons | 1.11 | Iconografia |

---

## Funcionalidades

### Frontend Público
- Homepage com portfólio em destaque
- Galeria de portfólio com filtro por categoria
- Página "Sobre Nós"
- Página de FAQ
- Página de Filmes (embed Vimeo)
- Formulário de Contacto (com notificação SignalR)
- Página de Créditos
- Páginas de erro personalizadas (404, 500)

### Área de Cliente
- Login seguro
- Galeria privada com fotografias do evento
- Download individual e em massa

### Backoffice Admin
- Dashboard com estatísticas
- CRUD de Fotografias (upload, editar, eliminar)
- CRUD de Categorias
- Gestão de Clientes e suas galerias
- Visualização de Contactos/Pedidos
- Aprovação de Testemunhos
- Notificações em tempo real (novos contactos)

### API REST
- `GET /api/fotografias` — Listar fotografias
- `GET /api/fotografias/{id}` — Obter fotografia
- `POST /api/fotografias` — Criar fotografia (requer autenticação)
- `PUT /api/fotografias/{id}` — Atualizar fotografia (requer autenticação)
- `DELETE /api/fotografias/{id}` — Eliminar fotografia (requer autenticação)
- Endpoints similares para Categorias e Contactos

---

## Base de Dados

### Modelo de Dados (6 Tabelas)

```
┌─────────────┐       ┌─────────────────┐       ┌─────────────┐
│  Categoria  │───1:N─│   Fotografia    │───N:N─│   Cliente   │
└─────────────┘       └─────────────────┘       └─────────────┘
                              │                        │
                              │                        │
                      ┌───────┴───────┐               1:N
                      │ClienteFotografia│              │
                      └───────────────┘        ┌──────┴──────┐
                                               │  Testemunho │
┌─────────────┐                                └─────────────┘
│   Contacto  │
└─────────────┘

┌─────────────────┐
│ ApplicationUser │ (Identity)
└─────────────────┘
```

### Relacionamentos
- **Muitos-para-um**: Fotografia → Categoria
- **Muitos-para-muitos**: Cliente ↔ Fotografia (via ClienteFotografia)

---

## Instalação e Execução

### Pré-requisitos
- [.NET SDK 10.0](https://dotnet.microsoft.com/download)

### Comandos

```bash
# Clonar o repositório
git clone https://github.com/[utilizador]/Fstudio.git
cd Fstudio

# Restaurar dependências
dotnet restore Fstudio/Fstudio.csproj

# Compilar
dotnet build Fstudio/Fstudio.csproj

# Executar
dotnet run --project Fstudio/Fstudio.csproj
```

### URLs
- **HTTP**: http://localhost:5173
- **HTTPS**: https://localhost:7086

### Migrações (Entity Framework)

```bash
# Criar nova migração
dotnet ef migrations add <NomeMigracao> --project Fstudio/Fstudio.csproj

# Aplicar migrações
dotnet ef database update --project Fstudio/Fstudio.csproj
```

---

## Credenciais de Teste

| Tipo | Email | Password |
|------|-------|----------|
| **Administrador** | admin@fstudio.info | Admin123! |
| **Cliente** | cliente@demo.com | Cliente123! |

> As credenciais são criadas automaticamente pelo DataSeeder no primeiro arranque.

---

## Estrutura do Projeto

```
Fstudio/
├── Areas/
│   ├── Admin/Pages/           # Backoffice
│   │   ├── Fotografias/       # CRUD fotografias
│   │   ├── Categorias/        # CRUD categorias
│   │   ├── Clientes/          # Gestão clientes
│   │   ├── Contactos/         # Ver mensagens
│   │   └── Testemunhos/       # Aprovar testemunhos
│   ├── Cliente/Pages/         # Área privada do cliente
│   └── Identity/Pages/        # Login, Registo, etc.
├── Controllers/Api/           # API REST
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Seed/DataSeeder.cs
├── Hubs/
│   └── NotificacaoHub.cs      # SignalR
├── Models/
│   ├── Entities/              # Entidades EF Core
│   └── ViewModels/            # DTOs
├── Pages/                     # Frontend público
│   ├── Portfolio/
│   ├── Contacto.cshtml
│   ├── Sobre.cshtml
│   ├── FAQ.cshtml
│   └── Creditos.cshtml
├── Services/                  # Camada de serviços
└── wwwroot/                   # Ficheiros estáticos
```

---

## Créditos

### Informação Académica
- **Curso**: Licenciatura em Engenharia Informática
- **Unidade Curricular**: Desenvolvimento Web
- **Ano Letivo**: 2024/2025
- **Semestre**: 2º

### Autores
| Número | Nome |
|--------|------|
| 27454 | André Vassalo |
| - | Samuel Ponte |

### Bibliotecas e Frameworks de Terceiros

| Biblioteca | Versão | Licença | Utilização |
|------------|--------|---------|------------|
| [ASP.NET Core](https://dotnet.microsoft.com/) | 10.0 | MIT | Framework web |
| [Entity Framework Core](https://docs.microsoft.com/ef/) | 10.0 | MIT | ORM |
| [Bootstrap](https://getbootstrap.com/) | 5.3.2 | MIT | CSS Framework |
| [Bootstrap Icons](https://icons.getbootstrap.com/) | 1.11.1 | MIT | Iconografia |
| [Google Fonts](https://fonts.google.com/) | - | OFL | Tipografia |

### Fontes Tipográficas
- **Great Vibes** — Títulos decorativos
- **Cormorant Garamond** — Títulos e headings
- **Montserrat** — Texto corrido

---

## Licença

Este projeto foi desenvolvido para fins académicos.

---

## Contacto

Para questões relacionadas com o projeto:
- Email: aluno27454@ipt.pt
