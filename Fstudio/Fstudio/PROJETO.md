é # Projecto — Redesign fstudio.info
Trabalho Prático de Desenvolvimento Web
Licenciatura em Eng. Informática — 2º ano, 2º semestre

---

## Contexto do Projecto

O **fstudio** é um estúdio de fotografia e vídeo de casamentos e sessões sediado em Lisboa, Portugal.
Site actual: http://fstudio.info

### Sobre o estúdio
- Fotógrafos: **Filipa e Rodrigo** ("The Framers")
- Estilo: natural, espontâneo e criativo
- Serviços: casamentos, noivados, sessões de família, gravidez
- Também produzem filmes (Vimeo: vimeo.com/fstudiofilms)
- Trabalham em Portugal e no estrangeiro
- Entregam 700–800 fotografias por casamento (dia inteiro, 14h de serviço)
- Entregam álbuns fine art (30x30cm, 60 páginas)

---

## Problemas do Site Actual

O site actual (Squarespace, copyright 2015) tem vários problemas evidentes:

- **Desactualizado** — copyright 2015, design antiquado
- **Conteúdo de terceiros misturado** — a página /info tem texto de outros estúdios (Lounge Fotografia do Porto, Forged in the North de NYC) que claramente ficou por engano
- **Navegação confusa** — links quebrados, página "AMAZING..." leva a "/2016"
- **Sem área de vídeo integrada** — os filmes estão apenas num link externo para Vimeo
- **Área de cliente externa** — redireciona para Zenfolio em vez de ser integrada
- **Sem preços visíveis** — apenas envio de email para brochura PDF
- **Não responsivo** — layout não adaptado para mobile
- **SEO fraco** — pouco conteúdo textual, sem metadados adequados
- **Sem SignalR / interactividade** — site completamente estático

---

## Objectivo do Projecto

Recriar o site do fstudio como uma aplicação web moderna em **ASP.NET Core**, mantendo a identidade visual minimalista e elegante, mas com funcionalidades dinâmicas e uma área de gestão de conteúdo.

---

## Arquitectura Proposta

### Tecnologias (obrigatórias pela UC)
- **ASP.NET Core Razor Pages** — aplicação web (frontend)
- **ASP.NET Core MVC** — API REST
- **Entity Framework Core** — base de dados
- **LINQ** — consultas
- **SignalR** — notificações em tempo real (ex: novo contacto/pedido de orçamento)
- **Bootstrap** — layout responsivo
- **jQuery** — interactividade no frontend

---

## Base de Dados — Modelo Sugerido

### Tabelas principais (mínimo 3 obrigatório)

**Fotografia** (trabalhos do portfólio)
- Id, Titulo, Descricao, DataSessao, Categoria, ImagemUrl, Destaque, DataCriacao

**Categoria** (Wedding, Engagement, Família, Gravidez, Vídeo)
- Id, Nome, Slug, Descricao, OrdemExibicao

**Cliente** (casais/clientes)
- Id, Nome, Email, Telefone, DataEvento, TipoServico, Estado, Notas, DataCriacao

**Testemunho** (depoimentos de clientes)
- Id, ClienteId, Texto, Avaliacao, Aprovado, DataCriacao

**Contacto** (formulário de contacto / pedidos de orçamento)
- Id, Nome, Email, Telefone, DataEvento, Mensagem, Lido, DataEnvio

**Utilizador** (backoffice)
- Id, Nome, Email, PasswordHash, Role (Admin / Editor)

### Relacionamentos
- Fotografia → Categoria: **muitos-para-um** ✅
- Cliente ↔ Fotografia (galeria privada): **muitos-para-muitos** ✅

---

## Páginas / Funcionalidades

### Frontend (público)
- [ ] **Homepage** — portfólio em grid com filtro por categoria
- [ ] **Portfólio** — galeria com lightbox por categoria (Wedding, Engagement, etc.)
- [ ] **Filmes** — embed de vídeos (Vimeo/YouTube) integrado no site
- [ ] **Sobre Nós** — biografia da Filipa e Rodrigo (corrigida e actualizada)
- [ ] **FAQ** — perguntas frequentes (já existe conteúdo no site actual)
- [ ] **Contacto** — formulário de contacto com validação
- [ ] **Página de Créditos** — info do trabalho académico (obrigatório pela UC)
- [ ] **Páginas de erro** — 404, 500 personalizadas

### Área de Cliente (privada)
- [ ] Login com password
- [ ] Galeria privada com as fotos do seu evento
- [ ] Download de fotografias

### Backoffice (admin)
- [ ] Login de administrador
- [ ] CRUD de fotografias (upload, editar, eliminar)
- [ ] CRUD de categorias
- [ ] Gestão de contactos/pedidos recebidos
- [ ] Aprovação de testemunhos
- [ ] **Notificações em tempo real via SignalR** quando chega novo pedido de contacto

---

## SignalR — Casos de Uso

- Notificação em tempo real no backoffice quando um novo contacto é submetido
- Indicador de "novo pedido" no painel de administração sem necessitar refresh

---

## Utilizadores do Sistema

| Role | Descrição |
|---|---|
| Admin | Acesso total — gere todo o conteúdo e vê contactos |
| Cliente | Acesso à galeria privada do seu evento |

---

## Identidade Visual a Preservar

- Paleta: tons escuros, minimalista, elegante
- Tipografia: clean, sem serifa
- Imagens a ocupar grande parte do ecrã
- Logo "fstudio" simples
- Estilo fotojornalístico / fine art

---

## Notas para o Claude Code

- O site actual está em Squarespace — não há código fonte disponível, tudo será construído de raiz
- As imagens do portfólio estão hospedadas em Squarespace CDN — para desenvolvimento usar imagens placeholder ou solicitar ao cliente
- A página /info do site actual tem conteúdo errado (texto de outros fotógrafos) — ignorar esse conteúdo
- Prioridade máxima: Homepage + Portfólio + Formulário de Contacto + Backoffice básico
- Componentes menos prioritárias: Área de cliente privada, sistema de álbuns

---

## Referências de Estilo

Sites de fotografia com boa referência de design:
- https://vsco.co
- https://format.com
- https://pixieset.com
