import { useState } from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import { getApiErrorMessage } from '../utils/errors.js'

export default function Login() {
  const { login, register, isAuthenticated, loading } = useAuth()
  const [mode, setMode] = useState('login')
  const [name, setName] = useState('')
  const [loginValue, setLoginValue] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  if (isAuthenticated) {
    return <Navigate to="/" replace />
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')

    try {
      if (mode === 'login') {
        await login(loginValue, password)
      } else {
        await register(name, email, password)
      }
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  function changeMode(nextMode) {
    setMode(nextMode)
    setError('')
  }

  return (
    <div className="login-page">
      <section className="login-card premium-card">
        <div className="login-hero">
          <div className="cup-mark">🏆</div>
          <span className="eyebrow">Copa do Mundo 2026</span>
          <h1>Bolão da Família</h1>
          <p>Palpites, ranking automático e disputa saudável.</p>
        </div>

        <div className="tabs" role="tablist" aria-label="Acesso ao bolão">
          <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => changeMode('login')}>Entrar</button>
          <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => changeMode('register')}>Criar conta</button>
        </div>

        <form onSubmit={handleSubmit} className="form">
          {mode === 'register' ? (
            <>
              <label>
                Nome
                <input
                  value={name}
                  onChange={(event) => setName(event.target.value)}
                  required
                  placeholder="Ex.: Wilson"
                  autoComplete="name"
                />
              </label>

              <label>
                E-mail <small className="optional">opcional</small>
                <input
                  type="email"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  placeholder="Pode deixar em branco"
                  autoComplete="email"
                />
              </label>
            </>
          ) : (
            <label>
              Nome ou e-mail
              <input
                value={loginValue}
                onChange={(event) => setLoginValue(event.target.value)}
                required
                placeholder="Digite seu nome ou e-mail"
                autoComplete="username"
              />
            </label>
          )}

          <label>
            Senha
            <input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
              minLength={6}
              placeholder="Mínimo de 6 caracteres"
              autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
            />
          </label>

          {error && <div className="alert error">{error}</div>}

          <button className="primary-btn" disabled={loading}>
            {loading ? 'Carregando...' : mode === 'login' ? 'Entrar no bolão' : 'Criar minha conta'}
          </button>
        </form>
      </section>
    </div>
  )
}
