import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import { roleLabel } from '../utils/format.js'

export default function Layout() {
  const { user, logout, isAdmin } = useAuth()

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-badge">26</span>
          <div>
            <strong>Bolão da Família</strong>
            <small>Copa do Mundo 2026</small>
          </div>
        </div>

        <nav className="nav">
          <NavLink to="/">Início</NavLink>
          <NavLink to="/jogos">Jogos e Palpites</NavLink>
          <NavLink to="/ranking">Ranking</NavLink>
          {isAdmin && <NavLink to="/admin">Administração</NavLink>}
        </nav>

        <div className="sidebar-note">
          <strong>Regra rápida</strong>
          <small>Palpite exato vale 10 pontos. Acerte o resultado e suba no ranking!</small>
        </div>

        <div className="user-box">
          <span>{user?.name}</span>
          <small>{roleLabel(user?.role)}</small>
          <button onClick={logout}>Sair</button>
        </div>
      </aside>

      <main className="content">
        <Outlet />
      </main>
    </div>
  )
}
