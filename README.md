# InkWell — Production-Grade Event-Driven Blogging Platform

<div align="center">

![InkWell Banner](https://images.unsplash.com/photo-1499750310107-5fef28a66643?auto=format&fit=crop&q=80&w=1200&h=300)

**A modern, scalable blogging ecosystem built on .NET 8 Microservices**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7.0-DC382D?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Angular](https://img.shields.io/badge/Angular-21-DD0031?style=for-the-badge&logo=angular&logoColor=white)](https://angular.dev/)
[![Render](https://img.shields.io/badge/Deployed-Render-46E3B7?style=for-the-badge&logo=render&logoColor=white)](https://render.com/)

[🏠 Live Demo](https://inkwell-frontend-qv2r.onrender.com) · [📖 API Docs](https://inkwell-gateway-9rf3.onrender.com/swagger) · [⚙️ Architecture](#-architecture) · [🚀 Getting Started](#-getting-started)

</div>

---

## 📌 Table of Contents

- [What is InkWell?](#-what-is-inkwell)
- [Tech Stack](#-tech-stack)
- [Architecture Overview](#-architecture)
- [UML Diagrams](#-uml-diagrams)
  - [Use Case Diagram](#1-use-case-diagram)
  - [System Architecture Diagram](#2-system-architecture-diagram)
  - [Entity Class Diagram](#3-entity-class-diagram)
  - [Post Publication Flow](#4-post-publication--notification-flow)
  - [RabbitMQ Event Flow](#5-rabbitmq-event-flow-diagram)
  - [Redis Caching Flow](#6-redis-caching-flow)
  - [JWT Authentication Flow](#7-jwt-authentication-flow)
  - [Comment Flow](#8-comment--reply-flow)
  - [Media Upload Flow](#9-media-upload-flow)
  - [Admin Workflow](#10-admin-workflow-diagram)
- [Microservices Overview](#-microservices-overview)
- [Inter-Service Communication](#-inter-service-communication)
- [RabbitMQ Events & Consumers](#-rabbitmq-events--consumers)
- [Redis Caching Strategy](#-redis-caching-strategy)
- [Database Schema](#-database-schema)
- [Security](#-security--authentication)
- [Frontend (Angular)](#-frontend-angular)
- [Getting Started](#-getting-started)
- [DevOps & Deployment](#-devops--deployment)

---

## 🖊️ What is InkWell?

InkWell is a **full-stack, production-grade blogging platform** built using a **microservices architecture** and **event-driven design patterns**. It enables:

- 📝 **Authors** to write, publish, and manage blog posts with rich text, images, categories, and tags
- 👥 **Readers** to like, comment, reply, and subscribe to newsletters
- 🔔 **Real-time Notifications** for every social interaction (likes, comments, replies)
- 📧 **Automated Email Campaigns** when new posts are published
- 🛡️ **Role-based Access Control** (READER → AUTHOR → ADMIN)
- 🎛️ **Admin Dashboard** to manage users, posts, comments, media, and newsletters

> **Key Design Goal:** Each service is 100% independent — it has its own database, can be deployed and scaled independently, and communicates with other services only through message events (RabbitMQ), never through direct HTTP calls between services.

---

## 🛠 Tech Stack

| Layer | Technology | Version | Purpose |
|:---|:---|:---|:---|
| **Backend** | ASP.NET Core Web API | .NET 8 | 8 independent microservices |
| **Frontend** | Angular | 21 | Single Page Application (SPA) |
| **Gateway** | Ocelot API Gateway | 23.x | Single entry point, routing & aggregation |
| **Database** | PostgreSQL | 16 | Per-service isolated databases |
| **Cache** | Redis | 7.0 | Distributed caching (Cache-Aside Pattern) |
| **Messaging** | RabbitMQ + MassTransit | 3.13 / 8.x | Async event-driven communication |
| **Auth** | JWT (HS256) + Google OAuth | — | Stateless authentication & authorization |
| **Password** | BCrypt.Net | — | Secure password hashing |
| **Email** | MailKit + MimeKit | — | SMTP email delivery |
| **Rich Text** | Quill.js | 1.3.7 | WYSIWYG post editor |
| **Icons** | Lucide Angular + Font Awesome | — | UI icon libraries |
| **Containerization** | Docker + Docker Compose | 26.x | Full local orchestration |
| **Cloud Messaging** | CloudAMQP | — | Managed RabbitMQ on the cloud |
| **Deployment** | Render.com | — | Cloud production hosting |
| **Code Quality** | SonarQube | — | Static analysis & security auditing |

---

## 📐 Architecture

InkWell follows a **DB-per-service microservices architecture** with **event-driven communication** via RabbitMQ. The Angular frontend communicates exclusively through the Ocelot API Gateway, which routes requests to the appropriate service.

```
┌─────────────────────────────────────────────────────────────────┐
│                     Angular Frontend (SPA)                       │
│  Home · Explore · Post Detail · Author Dashboard · Admin Panel  │
└──────────────────────────┬──────────────────────────────────────┘
                           │ HTTPS (single URL)
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│              Ocelot API Gateway  (:5032 / :443)                 │
│         Routing · JWT Forwarding · Swagger Aggregation          │
└──┬────┬────┬────┬────┬────┬────┬────────────────────────────────┘
   │    │    │    │    │    │    │
   ▼    ▼    ▼    ▼    ▼    ▼    ▼
 Auth Post Cat  Comm  Med  News Notif
  DB   DB   DB   DB   FS   DB   DB
         ↕                ↕
       Redis           RabbitMQ
     (Cache)          (Messages)
```

---

## 📊 UML Diagrams

### 1. Use Case Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        InkWell System                           │
│                                                                 │
│  ┌──────────┐   ──── Register / Login                          │
│  │  Reader  │ ──┬─── Browse & Read Posts                       │
│  └──────────┘   ├─── Like Posts                                │
│                  ├─── Write Comments & Replies                  │
│                  └─── Subscribe to Newsletter                   │
│                                                                 │
│  ┌──────────┐   ──── Everything Reader Can Do                  │
│  │  Author  │ ──┬─── Create / Edit / Publish Posts             │
│  └──────────┘   ├─── Upload Images (Media)                     │
│                  ├─── Assign Categories & Tags                  │
│                  └─── View Author Dashboard                     │
│                                                                 │
│  ┌──────────┐   ──── Everything Author Can Do                  │
│  │  Admin   │ ──┬─── Manage All Users (promote/suspend/delete) │
│  └──────────┘   ├─── Moderate Comments (approve/reject)        │
│                  ├─── Manage Categories & Tags                  │
│                  ├─── Send Newsletter Campaigns                 │
│                  ├─── Broadcast Notifications                   │
│                  ├─── Manage All Media Files                    │
│                  └─── View Platform Analytics                   │
│                                                                 │
│  ┌──────────────┐  (triggered automatically by events)         │
│  │   System     │ ── Send Email on Post Published               │
│  │  (RabbitMQ)  │ ── Create Notification on Like/Comment       │
│  └──────────────┘ ── Global Alert on Newsletter Sent           │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

### 2. System Architecture Diagram

```mermaid
graph TD
    Client["🌐 Angular SPA<br/>(localhost:4200 / Render)"]
    GW["🚪 Ocelot API Gateway<br/>:5032"]

    Client -->|"HTTPS — Single Entry Point"| GW

    subgraph Services["🔧 Microservice Layer"]
        Auth["🔑 Auth Service<br/>Register · Login · Google OAuth<br/>JWT Tokens · User Management"]
        Post["📝 Post Service<br/>CRUD · Publish · Like · Search<br/>View Count · Featured Posts"]
        Cat["🏷️ Category Service<br/>Categories · Tags<br/>PostCategory · PostTag"]
        Comm["💬 Comment Service<br/>Comments · Replies<br/>Moderation · Like/Unlike"]
        Med["📁 Media Service<br/>Upload · Store · Serve<br/>Soft Delete · Cleanup"]
        News["✉️ Newsletter Service<br/>Subscribe · Unsubscribe<br/>Email Campaigns · Token Auth"]
        Notif["🔔 Notification Service<br/>In-App Alerts · Bell Badge<br/>Mark Read · Broadcast"]
    end

    GW --> Auth
    GW --> Post
    GW --> Cat
    GW --> Comm
    GW --> Med
    GW --> News
    GW --> Notif

    subgraph Storage["💾 Storage Layer"]
        AuthDB[("Auth DB<br/>PostgreSQL")]
        PostDB[("Post DB<br/>PostgreSQL")]
        CatDB[("Category DB<br/>PostgreSQL")]
        CommDB[("Comment DB<br/>PostgreSQL")]
        MedDB[("Media DB<br/>PostgreSQL<br/>+ wwwroot/")]
        NewsDB[("Newsletter DB<br/>PostgreSQL")]
        NotifDB[("Notification DB<br/>PostgreSQL")]
        Redis[("🔴 Redis Cache<br/>Pub Posts · Comments<br/>Categories · Subs")]
        RMQ[("🐰 RabbitMQ<br/>CloudAMQP<br/>Events Bus")]
    end

    Auth --- AuthDB
    Post --- PostDB
    Cat --- CatDB
    Comm --- CommDB
    Med --- MedDB
    News --- NewsDB
    Notif --- NotifDB

    Post <-->|Cache-Aside| Redis
    Cat <-->|Cache-Aside| Redis
    Comm <-->|Cache-Aside| Redis
    News <-->|Cache-Aside| Redis

    Post -->|"PostPublishedEvent<br/>PostLikedEvent<br/>PostDeletedEvent"| RMQ
    Comm -->|"CommentAddedEvent"| RMQ
    News -->|"NewsletterPublishedEvent"| RMQ

    RMQ -->|Consume| News
    RMQ -->|Consume| Notif
    RMQ -->|Consume| Comm
```

---

### 3. Entity Class Diagram

```
┌──────────────────┐         ┌──────────────────────┐
│      User        │         │        Post           │
├──────────────────┤         ├──────────────────────┤
│ UserId (PK)      │         │ PostId (PK)           │
│ Username         │  1    * │ AuthorId (FK→User)    │
│ Email            ├─────────┤ AuthorName (cached)   │
│ PasswordHash     │creates  │ Title                 │
│ FullName         │         │ Slug (unique)         │
│ Role             │         │ Content (HTML)        │
│ Bio              │         │ Excerpt               │
│ AvatarUrl        │         │ FeaturedImageUrl      │
│ Provider         │         │ Status                │
│ IsActive         │         │ ReadTimeMinutes       │
│ CreatedAt        │         │ ViewCount             │
└──────────────────┘         │ LikesCount            │
                             │ IsFeatured            │
                             │ PublishedAt           │
                             └──────────┬────────────┘
                                        │ 1
                                        │ *
┌──────────────────┐         ┌──────────┴────────────┐
│     LikeModel    │         │       Comment         │
├──────────────────┤         ├──────────────────────┤
│ LikeId (PK)      │         │ CommentId (PK)        │
│ PostId (FK)      │   1   * │ PostId (FK)           │
│ UserId (FK)      ├─────────┤ AuthorId (FK→User)    │
│ CreatedAt        │ liked by│ ParentCommentId (self)│
└──────────────────┘         │ Content               │
                             │ LikesCount            │
                             │ Status                │
                             │ CreatedAt             │
                             └───────────────────────┘

┌──────────────────┐         ┌──────────────────────┐
│    Category      │  1    * │    PostCategory       │
├──────────────────┤         ├──────────────────────┤
│ CategoryId (PK)  ├─────────┤ PostCategoryId (PK)  │
│ Name             │         │ PostId (FK)           │
│ Slug (unique)    │         │ CategoryId (FK)       │
│ Description      │         └──────────────────────┘
│ ParentCategoryId │
│ PostCount        │         ┌──────────────────────┐
└──────────────────┘    *  * │       PostTag        │
                             ├──────────────────────┤
┌──────────────────┐         │ PostTagId (PK)        │
│       Tag        ├─────────┤ PostId (FK)           │
├──────────────────┤  1    * │ TagId (FK)            │
│ TagId (PK)       │         └──────────────────────┘
│ Name             │
│ Slug (unique)    │         ┌──────────────────────┐
│ PostCount        │         │    Notification      │
└──────────────────┘         ├──────────────────────┤
                             │ NotificationId (PK)   │
┌──────────────────┐         │ RecipientId (0=global)│
│   Subscriber     │         │ ActorId (0=system)    │
├──────────────────┤         │ Type                  │
│ SubscriberId(PK) │         │ Title                 │
│ Email            │         │ Message               │
│ UserId (nullable)│         │ RelatedId             │
│ FullName         │         │ RelatedType           │
│ Status           │         │ IsRead                │
│ Token (GUID)     │         │ CreatedAt             │
│ Preferences      │         └──────────────────────┘
│ SubscribedAt     │
│ UnsubscribedAt   │         ┌──────────────────────┐
└──────────────────┘         │    MediaModel        │
                             ├──────────────────────┤
                             │ MediaId (PK)          │
                             │ UploaderId (FK→User)  │
                             │ FileName (unique)     │
                             │ OriginalName          │
                             │ Url                   │
                             │ MimeType              │
                             │ SizeKb                │
                             │ AltText               │
                             │ LinkedPostId          │
                             │ IsDeleted             │
                             │ UploadedAt            │
                             └──────────────────────┘
```

---

### 4. Post Publication & Notification Flow

```
Author (Angular)
      │
      │ PUT /api/post/5/publish
      │ Authorization: Bearer <JWT>
      ▼
┌─────────────┐
│   Gateway   │  Routes /api/post/* → Post Service
└──────┬──────┘
       │
       ▼
┌──────────────────────────────────────────────────┐
│              Post Service                         │
│                                                  │
│  1. Extract userId from JWT claims               │
│  2. Find post by ID in PostgreSQL                │
│  3. Verify post.AuthorId == userId               │
│  4. Set Status = "PUBLISHED"                     │
│  5. Set PublishedAt = DateTime.UtcNow            │
│  6. Save to Post's PostgreSQL database           │
│  7. Delete "published_posts" from Redis Cache    │
│  8. Publish PostPublishedEvent to RabbitMQ ───►  │
└──────────────────────┬───────────────────────────┘
                       │
               Return 200 OK to Author
               (Author sees "Published!")
                       │
    ┌──────────────────▼──────────────────┐
    │         RabbitMQ (CloudAMQP)         │
    │    Exchange: PostPublishedEvent      │
    └──────────────┬──────────┬───────────┘
                   │          │
     ┌─────────────▼──┐    ┌──▼───────────────┐
     │ Newsletter      │    │  Notification     │
     │ Service         │    │  Service          │
     │                 │    │                   │
     │ 1. Get all      │    │ 1. Create global  │
     │    ACTIVE subs  │    │    notification   │
     │ 2. Build HTML   │    │    RecipientId=0  │
     │    email body   │    │ 2. Type=NEW_POST  │
     │ 3. Add unsubsc  │    │ 3. Save to        │
     │    ribe link    │    │    Notification DB│
     │ 4. Send via     │    │                   │
     │    Gmail SMTP   │    │ All users now see │
     │    (MailKit)    │    │ bell badge + 1    │
     └─────────────────┘    └───────────────────┘
     
     ← These happen ASYNC after Author already got 200 OK
```

---

### 5. RabbitMQ Event Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    PUBLISHERS → EVENTS → CONSUMERS              │
└─────────────────────────────────────────────────────────────────┘

POST SERVICE (Publisher)
├── PostPublishedEvent { PostId, Title, Slug, AuthorId }
│   ├──► Newsletter Service → PostPublishedConsumer
│   │       Action: Email all ACTIVE subscribers
│   └──► Notification Service → PostPublishedConsumer
│           Action: Create global notification (RecipientId=0)
│
├── PostLikedEvent { PostId, PostAuthorId, ActorId, ActorName }
│   └──► Notification Service → PostLikedConsumer
│           Action: Notify post author (skip if self-like)
│
└── PostDeletedEvent { PostId }
    └──► Comment Service → PostDeletedConsumer
            Queue: "comment-post-deleted-queue" (permanent)
            Action: DELETE all comments WHERE PostId = X

COMMENT SERVICE (Publisher)
└── CommentAddedEvent { PostId, CommentId, CommentAuthorId,
                        PostAuthorId, ParentCommentId,
                        ParentCommentAuthorId,
                        NotificationType, ActorName, PostTitle }
    └──► Notification Service → CommentAddedConsumer
            If NEW_COMMENT  → Notify post author
            If COMMENT_REPLY → Notify parent comment author

NEWSLETTER SERVICE (Publisher)
└── NewsletterPublishedEvent { Subject, Body, SentAt }
    └──► Notification Service → NewsletterPublishedConsumer
            Action: Create global notification (RecipientId=0)
            "Check your inbox — new newsletter!"

┌─────────────────────────────────────────────────────────────────┐
│  CONSUMER REGISTRATION (in Program.cs)                          │
│                                                                 │
│  Newsletter Service:                                            │
│    x.AddConsumer<PostPublishedConsumer>()                       │
│    cfg.ConfigureEndpoints(context)  ← auto-creates queue       │
│                                                                 │
│  Notification Service:                                          │
│    x.AddConsumer<PostPublishedConsumer>()                       │
│    x.AddConsumer<PostLikedConsumer>()                           │
│    x.AddConsumer<CommentAddedConsumer>()                        │
│    x.AddConsumer<NewsletterPublishedConsumer>()                 │
│    cfg.ConfigureEndpoints(context)                              │
│                                                                 │
│  Comment Service:                                               │
│    cfg.ReceiveEndpoint("comment-post-deleted-queue", e =>       │
│        e.ConfigureConsumer<PostDeletedConsumer>(context))       │
│    ← Named queue so it survives service restarts               │
└─────────────────────────────────────────────────────────────────┘
```

---

### 6. Redis Caching Flow

```
┌─────────────────────────────────────────────────────────────────┐
│               CACHE-ASIDE PATTERN (Read Flow)                   │
└─────────────────────────────────────────────────────────────────┘

Request: GET /api/post/published
         │
         ▼
  ┌─────────────┐
  │ Post Service │
  └──────┬──────┘
         │
         ▼
  Check Redis: key = "InkWellPost_published_posts"
         │
    ┌────┴─────────────────────────┐
    │                              │
  CACHE HIT                    CACHE MISS
  (data in Redis)              (not in Redis)
    │                              │
    │                              ▼
    │                    Query PostgreSQL:
    │                    SELECT * FROM Posts
    │                    WHERE Status='PUBLISHED'
    │                    ORDER BY PublishedAt DESC
    │                              │
    │                              ▼
    │                    Store in Redis
    │                    Key: "InkWellPost_published_posts"
    │                    TTL: 10 minutes (auto-expire)
    │                              │
    ▼                              ▼
Return data              Return data to Angular
instantly                (future reads hit Redis)

┌─────────────────────────────────────────────────────────────────┐
│               CACHE INVALIDATION (Write Flow)                   │
└─────────────────────────────────────────────────────────────────┘

Any Write Operation → Redis.RemoveAsync(cacheKey)
(publish, like, create, delete, archive)

┌─────────────────────────────────────────────────────────────────┐
│               ALL CACHE KEYS IN INKWELL                        │
├─────────────────────┬──────────────────┬───────────────────────┤
│ Service             │ Cache Key        │ TTL                   │
├─────────────────────┼──────────────────┼───────────────────────┤
│ Post                │ InkWellPost_     │ 10 min (+ manual      │
│                     │ published_posts  │ invalidation)         │
├─────────────────────┼──────────────────┼───────────────────────┤
│ Comment             │ InkWellComment_  │ 5 min (+ manual)      │
│                     │ comments_post_N  │                       │
├─────────────────────┼──────────────────┼───────────────────────┤
│ Newsletter          │ InkWellComment_  │ 10 min (footer check) │
│                     │ sub_user_N       │                       │
├─────────────────────┼──────────────────┼───────────────────────┤
│ Category            │ InkWellCategory_ │ 1 hour (rarely        │
│                     │ all_categories   │ changes)              │
│                     │ all_tags         │                       │
│                     │ post_tags_N      │ 30 min                │
│                     │ post_categories_N│ 30 min                │
└─────────────────────┴──────────────────┴───────────────────────┘

InstanceName prefix prevents key collisions across services.
Example: "InkWellPost_" + "published_posts" = "InkWellPost_published_posts"
```

---

### 7. JWT Authentication Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    LOGIN & TOKEN ISSUANCE                       │
└─────────────────────────────────────────────────────────────────┘

User → POST /api/auth/login { email, password }
            │
            ▼
      Auth Service
            │
      1. Find user by email in Auth DB
      2. Check IsActive == true
      3. BCrypt.Verify(password, storedHash)
      4. GenerateToken(user):
         ┌────────────────────────────────┐
         │ JWT Payload (Claims):          │
         │  NameIdentifier = userId       │
         │  Email = user.Email            │
         │  Name = user.Username          │
         │  Role = "READER/AUTHOR/ADMIN"  │
         │  FullName = user.FullName      │
         │  Expiry = now + 24 hours       │
         │  Signed with: HMAC SHA256      │
         │  Secret Key: (same across      │
         │               ALL services)    │
         └────────────────────────────────┘
      5. Return AuthResponseDTO + token

← Angular stores token in localStorage

┌─────────────────────────────────────────────────────────────────┐
│               USING THE TOKEN (Every Request)                   │
└─────────────────────────────────────────────────────────────────┘

Angular Request
    │
    ▼
Auth Interceptor (auth.interceptor.ts)
    │ Reads token from localStorage
    │ Clones request + adds header:
    │   Authorization: Bearer eyJhbGci...
    ▼
Gateway → Post Service
    │
    ▼
[Authorize] attribute on Controller
    │ Reads JWT
    │ Verifies: signature, issuer, audience, expiry
    │ Populates User.Claims
    │
    ▼
Controller reads claims:
    User.FindFirstValue(ClaimTypes.NameIdentifier) → userId
    User.FindFirstValue(ClaimTypes.Role)           → role
    User.FindFirstValue("FullName")                → display name

NO CALL TO AUTH SERVICE NEEDED — token is self-contained!
Every service has the SAME secret key → can verify independently.

┌─────────────────────────────────────────────────────────────────┐
│              TOKEN REFRESH (Interceptor Auto-Handles)           │
└─────────────────────────────────────────────────────────────────┘

API returns 401 Unauthorized (token expired)
    │
    ▼
authInterceptor catches the 401
    │
    ▼
Calls authService.refreshToken(oldToken)
    │ POST /api/auth/refresh
    │ Auth service reads old token (ignores expiry),
    │ extracts userId, generates new 24hr token
    ▼
New token saved to localStorage
    │
    ▼
Original request RETRIED with new token
    │
    ▼
User never sees "session expired" — happens silently!

If refresh ALSO fails → clearSessionAndRedirect() → /login
```

---

### 8. Comment & Reply Flow

```
                    TOP-LEVEL COMMENT
                    ─────────────────
User → POST /api/comment/add
       { postId, content, postAuthorId, parentCommentId: null }
            │
            ▼
      Comment Service
            │
      1. parentCommentId == null → top-level comment
      2. Create CommentModel:
         ├── Status = "APPROVED" (if moderation OFF)
         │   Status = "PENDING"  (if moderation ON)
         └── ParentCommentId = null
      3. Save to Comment DB
      4. Invalidate Redis: "InkWellComment_comments_post_N"
      5. Publish CommentAddedEvent:
         NotificationType = "NEW_COMMENT"
            │
            ▼
      Notification Service picks it up:
      → Creates notification for PostAuthorId:
        "Sarah commented on your post"

                    REPLY TO COMMENT
                    ────────────────
User → POST /api/comment/add
       { postId, content, postAuthorId, parentCommentId: 100 }
            │
            ▼
      Comment Service
            │
      1. parentCommentId = 100 → this is a reply
      2. Fetch parent comment (ID=100)
      3. CHECK: parent.ParentCommentId == null?
         ✓ Yes → allowed (replying to top-level)
         ✗ No  → throw error "Can only reply to top-level"
      4. Save CommentModel with ParentCommentId = 100
      5. Publish CommentAddedEvent:
         NotificationType = "COMMENT_REPLY"
         ParentCommentAuthorId = parent.AuthorId
            │
            ▼
      Notification Service:
      → Creates notification for ParentCommentAuthorId:
        "Sarah replied to your comment"

                    COMMENT STRUCTURE (1 level deep)
                    ─────────────────────────────────
  ┌──────────────────────────────────┐
  │ Comment A (ParentCommentId=null) │
  │  └── Reply 1 (ParentCommentId=A)│
  │  └── Reply 2 (ParentCommentId=A)│
  │  └── Reply 3 (ParentCommentId=A)│
  └──────────────────────────────────┘
  ┌──────────────────────────────────┐
  │ Comment B (ParentCommentId=null) │
  │  └── Reply 4 (ParentCommentId=B)│
  └──────────────────────────────────┘
  (No reply-to-reply — max 1 level deep)
```

---

### 9. Media Upload Flow

```
Author → POST /api/media/upload
         (multipart/form-data, not JSON)
         FormData: file = [image bytes]
              │
              ▼
        Media Service
              │
  ┌───────────────────────────────────────┐
  │  VALIDATION                           │
  │  ✓ File not empty                     │
  │  ✓ Size ≤ 10MB                        │
  │  ✓ MIME type: image/jpeg, image/png,  │
  │              image/gif, image/webp,   │
  │              application/pdf          │
  └───────────────────────────────────────┘
              │
              ▼
  Generate unique filename:
  Guid.NewGuid().Substring(0,8) + "-" + file.FileName
  Example: "a3f8b2c1-cover-photo.jpg"
              │
              ▼
  Save file to disk:
  wwwroot/uploads/a3f8b2c1-cover-photo.jpg
              │
              ▼
  Build public URL:
  https://media-service.onrender.com/uploads/a3f8b2c1-cover-photo.jpg
  (UseStaticFiles() makes wwwroot/ public)
              │
              ▼
  Save metadata to MediaFiles table:
  { FileName, OriginalName, Url, MimeType, SizeKb, UploaderId }
              │
              ▼
  Return MediaResponseDTO with Url
              │
              ▼
  Author pastes Url into Post Editor → FeaturedImageUrl field

  SOFT DELETE vs HARD DELETE:
  ┌────────────────────┬────────────────────────────┐
  │ Soft Delete        │ Hard Delete (Admin Cleanup) │
  │ IsDeleted = true   │ File.Delete(filePath)       │
  │ File stays on disk │ DB record removed           │
  │ URL still works    │ URL permanently broken      │
  │ Post images safe   │ Run: DELETE /api/media/     │
  │                    │       cleanup               │
  └────────────────────┴────────────────────────────┘
```

---

### 10. Admin Workflow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                     ADMIN PANEL FEATURES                        │
└─────────────────────────────────────────────────────────────────┘

Admin Login → JWT with Role="ADMIN"
     │
     ├──► USER MANAGEMENT (Auth Service)
     │       GET  /api/auth/users          → List all users
     │       PUT  /api/auth/role/{id}      → Promote/demote role
     │       DEL  /api/auth/deactivate/{id}→ Suspend (IsActive=false)
     │       POST /api/auth/reactivate/{id}→ Restore (IsActive=true)
     │       DEL  /api/auth/delete/{id}    → Hard delete user
     │
     ├──► POST MANAGEMENT (Post Service)
     │       GET  /api/post/all            → All posts (any status)
     │       PUT  /api/post/feature/{id}   → Pin to top (IsFeatured)
     │       DEL  /api/post/delete/{id}    → Delete any post
     │       GET  /api/post/by-status      → Filter by status
     │
     ├──► COMMENT MODERATION (Comment Service)
     │       PUT  /api/comment/moderation  → Toggle moderation mode
     │       GET  /api/comment/status?     → View by status (PENDING)
     │       PUT  /api/comment/approve/{id}→ Approve comment
     │       PUT  /api/comment/reject/{id} → Reject comment
     │       DEL  /api/comment/delete/{id} → Delete comment
     │
     ├──► TAXONOMY (Category Service)
     │       POST /api/category/create     → Create category
     │       POST /api/category/tag/create → Create tag
     │       DEL  /api/category/delete/{id}→ Delete category
     │       DEL  /api/category/tag/delete → Delete tag
     │
     ├──► NEWSLETTER (Newsletter Service)
     │       GET  /api/newsletter/all      → All subscribers
     │       GET  /api/newsletter/count    → Active subscriber count
     │       POST /api/newsletter/send     → Send campaign to all
     │       DEL  /api/newsletter/delete/{id}→ Remove subscriber
     │
     ├──► MEDIA (Media Service)
     │       GET  /api/media/all           → All uploads
     │       DEL  /api/media/delete/{id}   → Soft delete
     │       DEL  /api/media/cleanup       → Hard delete all soft-deleted
     │       GET  /api/media/deleted       → View soft-deleted files
     │
     └──► NOTIFICATIONS (Notification Service)
             GET  /api/notification/all    → All platform notifications
             POST /api/notification/broadcast → Send to specific users
             GET  /api/notification/by-type  → Filter by type

MODERATION FLOW:
    Moderation ON → New comments → Status = "PENDING"
                              ↓
                   Admin reviews pending comments
                              ↓
              APPROVE → Status = "APPROVED" (visible)
              REJECT  → Status = "REJECTED" (hidden)
```

---

## 📦 Microservices Overview

### 🔑 InkWell.Auth — Identity & Profiles

**Port (local):** 5058 | **Database:** `InkWell_Auth`

Handles every aspect of user identity and access control.

| Feature | Details |
|---|---|
| **Registration** | Validates username (alphanumeric, unique), email (unique), password (min 6 chars), full name |
| **Login** | Verifies BCrypt hash, checks IsActive, generates 24hr JWT |
| **Google OAuth** | Validates Google ID token via `Google.Apis.Auth`, creates account if new user |
| **JWT Issuance** | Packs userId, email, username, role, fullName as claims; signs with HS256 |
| **Token Refresh** | Reads expired token without checking expiry, generates fresh 24hr token |
| **Role Management** | READER → AUTHOR → ADMIN promotion by admin |
| **Account Control** | Suspend (IsActive=false) / Restore / Permanent Delete |
| **Auto Admin** | Creates admin account on first startup; upgrades existing user if needed |

**Key endpoints:** `/register` · `/login` · `/google-login` · `/refresh` · `/validate` · `/profile` · `/role/{id}`

---

### 📝 InkWell.Post — Content Engine

**Port (local):** 5047 | **Database:** `InkWell_Post` | **Cache:** Redis (10 min TTL)

The core service managing the entire post lifecycle.

| Feature | Details |
|---|---|
| **State Machine** | DRAFT → PUBLISHED → UNPUBLISHED / ARCHIVED |
| **Slug Generation** | Auto-generates URL-safe slug; appends timestamp if duplicate |
| **Read Time** | Calculated as `wordCount / 200` (min 1 minute) |
| **Caching** | Published posts list cached in Redis; invalidated on any write |
| **Like System** | LikeModel tracks unique (PostId, UserId) pairs; fires `PostLikedEvent` |
| **Search** | PostgreSQL `ILike` for case-insensitive search across title, content, excerpt, author |
| **View Tracking** | Increments ViewCount on every slug/id fetch |
| **Events Published** | `PostPublishedEvent`, `PostLikedEvent`, `PostDeletedEvent` |

**Key endpoints:** `/published` · `/create` · `/publish/{id}` · `/like/{id}` · `/search` · `/feature/{id}`

---

### 💬 InkWell.Comment — Social Interaction

**Port (local):** 5192 | **Database:** `InkWell_Comment` | **Cache:** Redis (5 min TTL)

Manages threaded discussions with moderation.

| Feature | Details |
|---|---|
| **Threading** | Top-level comments + 1-level replies (no reply-to-reply) |
| **Moderation** | Static bool flag; when ON, new comments go to PENDING status |
| **Soft Delete** | Sets Status="DELETED"; replies still display as [deleted] |
| **Like/Unlike** | Comment-level likes with optimistic UI updates |
| **Event Published** | `CommentAddedEvent` with `NEW_COMMENT` or `COMMENT_REPLY` type |
| **Event Consumed** | `PostDeletedEvent` → deletes all comments for deleted post |
| **Durable Queue** | Named queue `comment-post-deleted-queue` survives service restarts |

**Key endpoints:** `/add` · `/post/{postId}` · `/replies/{parentId}` · `/approve/{id}` · `/moderation`

---

### 🏷️ InkWell.Category — Taxonomy Service

**Port (local):** 5087 | **Database:** `InkWell_Category` | **Cache:** Redis (30 min – 1 hour TTL)

Organizes content through hierarchical categories and tags.

| Feature | Details |
|---|---|
| **Category Hierarchy** | Parent → Child categories (e.g., "Technology" → "AI") |
| **Many-to-Many** | PostCategory and PostTag linking tables |
| **PostCount Tracking** | Increments/decrements on assign/remove |
| **Trending Tags** | Top 10 tags by PostCount, always fresh (no cache) |
| **Slug Uniqueness** | Enforced at DB level via unique index |
| **No RabbitMQ** | Pure HTTP service; no events published or consumed |

**Key endpoints:** `/create` · `/assign` · `/tag/assign` · `/tag/trending` · `/slug/{slug}/posts`

---

### ✉️ InkWell.Newsletter — Subscriptions & Email

**Port (local):** 5226 | **Database:** `InkWell_Newsletter` | **Cache:** Redis (10 min TTL)

Manages subscriber lists and automated email campaigns.

| Feature | Details |
|---|---|
| **Subscription States** | PENDING → ACTIVE → UNSUBSCRIBED |
| **Token-Based** | GUID token for one-click confirm/unsubscribe (no login needed) |
| **Anonymous Support** | Handles subscriptions before account creation |
| **Email Sending** | MailKit + Gmail SMTP, StartTLS encryption on port 587 |
| **Preference Filter** | Send campaigns only to subscribers with matching preference tags |
| **Event Consumed** | `PostPublishedEvent` → emails all ACTIVE subscribers |
| **Event Published** | `NewsletterPublishedEvent` after sending a campaign |
| **Caching** | Caches per-user subscription status (footer check on every page) |

**Key endpoints:** `/subscribe` · `/confirm/{token}` · `/unsubscribe/{token}` · `/send` · `/my-subscription`

---

### 🔔 InkWell.Notification — Event Consumer Hub

**Port (local):** 5283 | **Database:** `InkWell_Notification`

The central in-app notification system, triggered entirely by RabbitMQ events.

| Feature | Details |
|---|---|
| **Global Notifications** | `RecipientId = 0` appears in every user's notification list |
| **4 Event Consumers** | PostPublished · PostLiked · CommentAdded · NewsletterPublished |
| **Bell Badge** | Unread count API called on every login; updates Navbar reactively |
| **Self-Like Skip** | `postAuthorId == actorId` → skip notification |
| **Mark Read** | Individual or bulk; global notifications markable by any user |
| **Admin Broadcast** | Send to specific user list or globally (empty list = global) |
| **No Redis** | All reads go to DB (notification data is user-specific, high variety) |

**Key endpoints:** `/my` · `/unread-count` · `/read/{id}` · `/read-all` · `/broadcast`

---

### 📁 InkWell.Media — Asset Management

**Port (local):** 5171 | **Database:** `InkWell_Media` | **File Storage:** `wwwroot/uploads/`

Handles all file uploads and serves them publicly.

| Feature | Details |
|---|---|
| **Supported Types** | JPEG, PNG, GIF, WebP, PDF |
| **Max Size** | 10MB per file |
| **Unique Names** | 8-char GUID prefix prevents filename collisions |
| **Public Serving** | `UseStaticFiles()` makes uploads/ directory publicly accessible |
| **Soft Delete** | `IsDeleted = true`; file stays on disk, URL still works |
| **Hard Delete** | Admin cleanup: removes file from disk + DB record permanently |
| **No RabbitMQ** | Standalone service; no events published or consumed |
| **No Redis** | Files are static assets; no caching needed |

**Key endpoints:** `/upload` · `/my-files` · `/link/{id}` · `/alt-text/{id}` · `/cleanup`

---

### 🌐 InkWell.Gateway — Unified Entry Point

**Port (local):** 5032 | **No Database** | **Library:** Ocelot

Routes all Angular traffic to the correct microservice.

| Feature | Details |
|---|---|
| **Ocelot Routing** | URL prefix matching; `{everything}` wildcard |
| **Dual Config** | `ocelot.json` (Render/HTTPS) · `ocelot.Docker.json` (local/HTTP) |
| **Swagger Hub** | Aggregates all 7 services' Swagger docs in one UI |
| **Health Check** | `GET /health` → Render uses this to monitor liveness |
| **Root Redirect** | `GET /` → redirects to `/swagger` |
| **CORS** | Two policies: AllowFrontend (strict) + AllowAll (permissive for gateway) |
| **Hot Reload** | `reloadOnChange: true` on ocelot.json |
| **No Logic** | Zero business logic; pure router |

---

## 🔄 Inter-Service Communication

### Synchronous (HTTP via Gateway)

Used when the client needs an immediate response:

```
Angular → Gateway → Service → Database → Response to Angular
```

All external traffic goes through the Gateway. Services never receive direct external calls.

### Asynchronous (RabbitMQ + MassTransit)

Used for decoupled background processing:

```
Service A publishes event → RabbitMQ → Service B consumes event
(Service A doesn't wait; it returned 200 OK to the user already)
```

**Why not direct HTTP between services?**
- If Service B is down, the request fails
- Service A must know Service B's URL
- Tight coupling — changing B requires changing A
- With RabbitMQ: messages queue up until B recovers; A knows nothing about B

---

## 🐰 RabbitMQ Events & Consumers

### All Events

| Event | Publisher | Payload | Consumers |
|---|---|---|---|
| `PostPublishedEvent` | Post | PostId, Title, Slug, AuthorId | Newsletter, Notification |
| `PostLikedEvent` | Post | PostId, PostAuthorId, ActorId, ActorName | Notification |
| `PostDeletedEvent` | Post | PostId | Comment |
| `CommentAddedEvent` | Comment | PostId, CommentId, CommentAuthorId, PostAuthorId, ParentCommentId, ParentCommentAuthorId, NotificationType, ActorName, PostTitle | Notification |
| `NewsletterPublishedEvent` | Newsletter | Subject, Body, SentAt | Notification |

### Publishing Pattern

```csharp
// Try to publish; if RabbitMQ is down, log and continue (don't fail the user action)
try {
    await publishEndpoint.Publish(new PostPublishedEvent {
        PostId = post.PostId, Title = post.Title,
        Slug = post.Slug, AuthorId = post.AuthorId
    });
} catch (Exception ex) {
    Console.WriteLine($"[RabbitMQ] Error: {ex.Message}");
    // Post is still published — RabbitMQ is non-blocking
}
```

### Consuming Pattern

```csharp
// Registered in Program.cs:
x.AddConsumer<PostPublishedConsumer>();
cfg.ConfigureEndpoints(context); // MassTransit auto-creates the queue

// The Consumer:
public class PostPublishedConsumer : IConsumer<PostPublishedEvent> {
    public async Task Consume(ConsumeContext<PostPublishedEvent> context) {
        var message = context.Message;
        await _newsletterService.SendPostNotification(new NewPostNotificationDTO {
            PostId = message.PostId, Title = message.Title, ...
        });
    }
}
```

---

## 🔴 Redis Caching Strategy

### Pattern: Cache-Aside

```
Read:  Check cache → Hit: return | Miss: go to DB → store in cache → return
Write: Save to DB → Remove from cache (next read gets fresh data)
```

### Service-by-Service

| Service | Cache Key | TTL | What's Cached | Invalidated On |
|---|---|---|---|---|
| Post | `InkWellPost_published_posts` | 10 min | All published posts list | Create, publish, like, delete, archive |
| Comment | `InkWellComment_comments_post_{N}` | 5 min | Comments for one post | Add, edit, delete comment |
| Newsletter | `InkWellComment_sub_user_{N}` | 10 min | One user's subscription status | Subscribe, unsubscribe |
| Category | `InkWellCategory_all_categories` | 1 hour | All categories | Create, update, delete category |
| Category | `InkWellCategory_all_tags` | 1 hour | All tags | Create, delete tag |
| Category | `InkWellCategory_post_tags_{N}` | 30 min | Tags on one post | Assign, remove tag |
| Category | `InkWellCategory_post_categories_{N}` | 30 min | Categories on one post | Assign, remove category |

### Resilience

All Redis operations are wrapped in `try/catch`. If Redis is unavailable, the service falls back to PostgreSQL transparently — no crash, no error to the user.

---

## 🗄️ Database Schema

Each service owns its own isolated database. There are **no cross-service foreign keys** — services only reference each other by ID, and those IDs are obtained from JWT tokens or event payloads.

```
Auth DB:      Users
Post DB:      Posts, Likes
Comment DB:   Comments
Category DB:  Categories, Tags, PostCategories, PostTags
Newsletter DB: Subscribers
Notification DB: Notifications
Media DB:     MediaFiles
```

### Key Design Decisions

- **AuthorName cached in Post table** — avoids calling Auth service on every post list load
- **RecipientId = 0** in Notifications — single global record visible to all users (no N records for N users)
- **ParentCommentId self-reference** — enables threaded replies without a separate table
- **Token (GUID) on Subscriber** — enables passwordless unsubscribe/confirm via email links
- **IsDeleted on MediaModel** — soft delete protects posts from broken image URLs
- **Composite unique index** on PostCategory and PostTag — prevents duplicate assignments at DB level

---

## 🔐 Security & Authentication

### JWT (JSON Web Token)

```
Token Structure:
  Header:    { "alg": "HS256", "typ": "JWT" }
  Payload:   { "nameid": "42", "email": "john@gmail.com",
               "unique_name": "john_writes", "role": "AUTHOR",
               "FullName": "John Doe", "exp": 1714000000 }
  Signature: HMAC-SHA256(header + "." + payload, sharedSecret)
```

- Tokens expire after **24 hours**
- The **same secret key** is configured in all 8 services
- Expired tokens are **automatically refreshed** by the Angular interceptor
- Role change takes effect on the **next login** (stateless JWT trade-off)

### BCrypt Password Hashing

```
Registration: BCrypt.HashPassword("secret123") → "$2a$11$xyz..."
Login:        BCrypt.Verify("secret123", "$2a$11$xyz...") → true/false
```

Raw passwords are **never stored** — only the one-way BCrypt hash.

### Google OAuth

1. User clicks "Sign in with Google" on login page
2. Google GSI client library renders the button and handles the OAuth flow
3. Google returns an `idToken` (signed JWT from Google)
4. Angular sends `idToken` to `/api/auth/google-login`
5. Auth service calls `GoogleJsonWebSignature.ValidateAsync(idToken)` — verifies signature with Google's public keys
6. Extracts email, name, avatar from Google's verified payload
7. Creates/updates user account with `Provider = "GOOGLE"`
8. Returns our own InkWell JWT — same token format as regular login

### Route Protection

| Guard | Protects | Redirect On Fail |
|---|---|---|
| `authGuard` | `/profile`, `/notifications` | `/login?returnUrl=...` |
| `authorGuard` | `/author/**` | `/` (home) |
| `adminGuard` | `/admin/**` | `/` (home) |

---

## 🖥️ Frontend (Angular)

**Version:** Angular 21 | **Architecture:** Standalone Components | **Port:** 4200

### Key Features

| Feature | Implementation |
|---|---|
| **Routing** | `app.routes.ts` with lazy loading for admin section |
| **Auth State** | `BehaviorSubject<User>` in `AuthService` — reactive across all components |
| **HTTP Interceptor** | Auto-attaches JWT; silently refreshes on 401; redirects on auth failure |
| **Route Guards** | `authGuard`, `authorGuard`, `adminGuard` — check roles from BehaviorSubject |
| **Optimistic Updates** | Like button updates count instantly; rolls back if API fails |
| **Cache Busting** | `?t=timestamp` appended to `/published` endpoint URL |
| **Rich Text Editor** | Quill.js loaded from CDN; `ngAfterViewInit` for DOM-ready initialization |
| **File Upload** | `FormData` (multipart) sent to Media service; URL filled into form automatically |
| **Notification Bell** | `unreadCountSubject: BehaviorSubject<number>` updates badge reactively |
| **TimeAgo Pipe** | Converts ISO dates to "3h ago", "2d ago" etc. |
| **Skeleton Loading** | Gray placeholder cards while posts load (better UX than spinner) |

### Folder Structure

```
src/app/
├── core/
│   ├── guards/           authGuard · authorGuard · adminGuard
│   ├── interceptors/     auth.interceptor.ts (JWT + token refresh)
│   ├── models/           TypeScript interfaces for all entities
│   └── services/         auth · post · comment · notification
│                         newsletter · category · media · admin
├── features/
│   ├── auth/             login · register (with Google OAuth)
│   ├── public/           home · explore · post-detail · profile
│   │                     notifications · newsletter · newsletter-action
│   ├── author/           dashboard · post-editor (Quill + ReactiveForm)
│   └── admin/            dashboard · users · posts · comments
│                         taxonomy · newsletter · media · notifications
└── shared/
    ├── components/       navbar · footer · post-card · comment-section
    └── pipes/            time-ago.pipe.ts
```

### Data Flow (Component → Service → API)

```
Component calls Service method (e.g., postService.likePost(id))
    │
    ▼
Service builds HTTP request (HttpClient.post(...))
    │
    ▼
authInterceptor adds Authorization header
    │
    ▼
Request goes to Gateway → Post Service
    │
    ▼
Response returns as Observable
    │
    ▼
Component .subscribe({ next: ..., error: ... })
    │
    ▼
UI updates via Angular's change detection
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Node.js 20+](https://nodejs.org/) (for Angular frontend)
- PostgreSQL + Redis (if running without Docker)

### Option 1 — Docker Compose (Recommended)

Spins up all 8 services + PostgreSQL instances + Redis + RabbitMQ with a single command.

```bash
# Clone the repository
git clone https://github.com/Yashkumar1160/InkWell.git
cd InkWell

# Start everything
docker-compose up --build

# Services will be available at:
# Gateway:      http://localhost:5032
# Swagger UI:   http://localhost:5032/swagger
# RabbitMQ UI:  http://localhost:15672  (guest/guest)
```

### Option 2 — Manual (Service by Service)

```bash
# Start infrastructure first
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:16
docker run -d -p 6379:6379 redis:7
docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# Then run each service
dotnet run --project InkWell.Auth
dotnet run --project InkWell.Post
dotnet run --project InkWell.Comment
dotnet run --project InkWell.Category
dotnet run --project InkWell.Newsletter
dotnet run --project InkWell.Notification
dotnet run --project InkWell.Media
dotnet run --project InkWell.Gateway
```

### Option 3 — Frontend Only

```bash
cd InkWell.UI
npm install
ng serve
# App runs at http://localhost:4200
# Connects to production backend at https://inkwell-gateway-9rf3.onrender.com
```

### Default Admin Credentials

> Created automatically on first startup

| Field | Value |
|---|---|
| Email | `admin@gmail.com` |
| Password | `admin1234` |
| Role | `ADMIN` |

---

## ⚙️ DevOps & Deployment

### Docker — Multi-Stage Build

Each service uses a two-stage Dockerfile:

```dockerfile
# Stage 1: Compile (uses full SDK ~700MB)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["*.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

# Stage 2: Run (uses runtime only ~200MB)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "InkWell.ServiceName.dll"]
```

Final image is **~200MB** instead of **~700MB** — no compiler in production.

### Render.com Deployment

```
GitHub Push → Render detects Dockerfile → Build Docker image
→ Run container → Inject environment variables → Service live
```

**Production Environment Variables (set in Render dashboard):**

| Variable | Example Value |
|---|---|
| `ConnectionStrings__PostDB` | `postgres://user:pass@host/inkwell_post` |
| `Jwt__Secret` | `FPICUVNej03KufTJmHzbToS8jMYHWXIuUkNRGxWHoAg=` |
| `RabbitMQ__ConnectionString` | `amqps://user:pass@cloudamqp-host/vhost` |
| `Redis__ConnectionString` | `redis-host:6379,password=xxx` |
| `Email__Password` | `gmail-app-password` |

**Health Check:** Each service exposes `GET /health` → Render monitors and auto-restarts on failure.

### Live Production URLs

| Service | URL |
|---|---|
| Frontend | `https://inkwell-frontend-qv2r.onrender.com` |
| Gateway | `https://inkwell-gateway-9rf3.onrender.com` |
| API Docs | `https://inkwell-gateway-9rf3.onrender.com/swagger` |

---

## 📁 Repository Structure

```
InkWell/
├── InkWell.Auth/              # Identity & JWT service
├── InkWell.Post/              # Content engine
├── InkWell.Comment/           # Social interaction
├── InkWell.Category/          # Taxonomy (categories & tags)
├── InkWell.Newsletter/        # Email subscriptions
├── InkWell.Notification/      # In-app alerts
├── InkWell.Media/             # File upload & serving
├── InkWell.Gateway/           # API Gateway (Ocelot)
│   ├── ocelot.json            # Production routing
│   └── ocelot.Docker.json     # Local Docker routing
├── InkWell.UI/                # Angular 21 SPA frontend
│   └── src/app/
│       ├── core/              # Guards, interceptors, services, models
│       ├── features/          # Pages (auth, public, author, admin)
│       └── shared/            # Navbar, footer, post-card, comment-section
├── docker-compose.yml         # Full local orchestration
└── README.md                  # This file
```

---

## 🧠 Design Patterns Used

| Pattern | Where | Why |
|---|---|---|
| **Repository Pattern** | All services | Separates DB queries from business logic |
| **Service Layer Pattern** | All services | Business rules isolated from HTTP layer |
| **Event-Driven Architecture** | Post · Comment · Newsletter · Notification | Loose coupling between services |
| **Publish-Subscribe (Pub/Sub)** | RabbitMQ | One publisher → multiple independent consumers |
| **Cache-Aside Pattern** | Post · Comment · Category · Newsletter | Read from cache, fall back to DB on miss |
| **API Gateway Pattern** | Ocelot Gateway | Single entry point; hides internal topology |
| **DTO Pattern** | All services | Decouples HTTP shapes from database models |
| **Dependency Injection** | All services | Testable, swappable, lifecycle-managed dependencies |
| **Optimistic Updates** | Angular (Like button) | Instant UI feedback; rollback on API failure |
| **Interceptor Pattern** | Angular HTTP | Cross-cutting concern (JWT) without code duplication |
| **Guard Pattern** | Angular Routes | Declarative access control on routes |

---

## 👨‍💻 Author

**Yash Kumar**  
Full-Stack Developer · .NET & Angular Specialist

[![GitHub](https://img.shields.io/badge/GitHub-Yashkumar1160-181717?style=for-the-badge&logo=github)](https://github.com/Yashkumar1160)

---

<div align="center">
  <sub>Built with ❤️ using .NET 8 · Angular 21 · PostgreSQL · Redis · RabbitMQ · Docker · Render</sub>
</div>
