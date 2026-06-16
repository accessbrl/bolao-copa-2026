# Rotas da API

Base local:

```txt
http://localhost:5080/api
```

## Auth

```txt
POST /auth/register
POST /auth/login
GET  /auth/me
```

## Matches

```txt
GET /matches
GET /matches/upcoming?take=8
GET /matches/{id}
```

## Predictions

```txt
GET  /predictions/me
POST /predictions
```

## Ranking

```txt
GET /ranking
```

## Admin

```txt
GET  /admin/users
POST /admin/users
POST /admin/seed-worldcup
POST /admin/recalculate
PUT  /admin/matches/{id}/result
```
