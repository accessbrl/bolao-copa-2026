# Seed da Copa 2026

O seed principal está no arquivo:

```txt
backend/BolaoCopa.Api/Data/WorldCupSeeder.cs
```

Para executar:

1. Crie o primeiro usuário pelo frontend. Ele vira admin automaticamente.
2. Faça login.
3. Entre em Admin.
4. Clique em **Carregar tabela Copa 2026**.

O seed é idempotente: se executar mais de uma vez, ele não duplica seleções nem jogos.
