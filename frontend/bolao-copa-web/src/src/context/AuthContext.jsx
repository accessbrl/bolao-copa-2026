import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import api from '../services/api'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem('bolao_user')
    return saved ? JSON.parse(saved) : null
  })
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    const token = localStorage.getItem('bolao_token')
    if (!token) return

    api.get('/auth/me')
      .then((response) => {
        setUser(response.data)
        localStorage.setItem('bolao_user', JSON.stringify(response.data))
      })
      .catch(() => logout())
  }, [])

  async function login(loginValue, password) {
    setLoading(true)
    try {
      const response = await api.post('/auth/login', { login: loginValue, password })
      localStorage.setItem('bolao_token', response.data.token)
      localStorage.setItem('bolao_user', JSON.stringify(response.data.user))
      setUser(response.data.user)
      return response.data.user
    } finally {
      setLoading(false)
    }
  }

  async function register(name, email, password) {
    setLoading(true)
    try {
      const response = await api.post('/auth/register', { name, email: email || null, password })
      localStorage.setItem('bolao_token', response.data.token)
      localStorage.setItem('bolao_user', JSON.stringify(response.data.user))
      setUser(response.data.user)
      return response.data.user
    } finally {
      setLoading(false)
    }
  }

  function logout() {
    localStorage.removeItem('bolao_token')
    localStorage.removeItem('bolao_user')
    setUser(null)
  }

  const value = useMemo(() => ({
    user,
    loading,
    login,
    register,
    logout,
    isAuthenticated: Boolean(user),
    isAdmin: user?.role === 'admin'
  }), [user, loading])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  return useContext(AuthContext)
}
