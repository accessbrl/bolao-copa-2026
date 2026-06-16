import { useEffect, useState } from 'react'
import TeamBadge from '../components/TeamBadge.jsx'
import api from '../services/api.js'
import { formatDateTime, roleLabel, stageLabel } from '../utils/format.js'

export default function Admin() {
  const [users, setUsers] = useState([])
  const [matches, setMatches] = useState([])
  const [resultInputs, setResultInputs] = useState({})
  const [message, setMessage] = useState('')
  const [newUser, setNewUser] = useState({ name: '', email: '', password: '', role: 'participant' })

  async function load() {
    const [usersResponse, matchesResponse] = await Promise.all([
      api.get('/admin/users'),
      api.get('/matches')
    ])
    setUsers(usersResponse.data)
    setMatches(matchesResponse.data)

    const next = {}
    for (const match of matchesResponse.data) {
      next[match.id] = {
        homeScore: match.homeScore ?? '',
        awayScore: match.awayScore ?? ''
      }
    }
    setResultInputs(next)
  }

  useEffect(() => {
    load().catch(() => setMessage('Erro ao carregar dados administrativos.'))
  }, [])

  async function seedWorldCup() {
    setMessage('')
    try {
      const response = await api.post('/admin/seed-worldcup')
      setMessage(`${response.data.message} Times: ${response.data.totalTeams}. Jogos: ${response.data.totalMatches}.`)
      await load()
    } catch (err) {
      setMessage(err.response?.data?.message || 'Erro ao carregar tabela da Copa.')
    }
  }

  async function recalculate() {
    setMessage('')
    try {
      const response = await api.post('/admin/recalculate')
      setMessage(response.data.message)
    } catch (err) {
      setMessage(err.response?.data?.message || 'Erro ao recalcular.')
    }
  }

  async function createUser(event) {
    event.preventDefault()
    setMessage('')
    try {
      await api.post('/admin/users', {
        ...newUser,
        email: newUser.email || null
      })
      setMessage('Participante criado com sucesso.')
      setNewUser({ name: '', email: '', password: '', role: 'participant' })
      await load()
    } catch (err) {
      setMessage(err.response?.data?.message || 'Erro ao criar participante.')
    }
  }

  function updateResultInput(matchId, field, value) {
    setResultInputs(current => ({
      ...current,
      [matchId]: {
        ...current[matchId],
        [field]: value
      }
    }))
  }

  async function saveResult(match) {
    setMessage('')
    const values = resultInputs[match.id] || {}
    const homeScore = Number(values.homeScore)
    const awayScore = Number(values.awayScore)

    if (Number.isNaN(homeScore) || Number.isNaN(awayScore)) {
      setMessage('Informe o placar do jogo.')
      return
    }

    try {
      await api.put(`/admin/matches/${match.id}/result`, {
        homeScore,
        awayScore,
        winnerTeamId: null
      })
      setMessage(`Resultado do jogo ${match.matchNumber} salvo.`)
      await load()
    } catch (err) {
      setMessage(err.response?.data?.message || 'Erro ao salvar resultado.')
    }
  }

  return (
    <div className="page">
      <header className="page-header page-header-row">
        <div>
          <span className="eyebrow">Controle</span>
          <h1>Administração</h1>
          <p>Cadastre participantes, carregue a tabela da Copa e atualize os resultados.</p>
        </div>
        <div className="header-chip">
          <strong>{users.length}</strong>
          <span>participantes</span>
        </div>
      </header>

      {message && <div className="alert info">{message}</div>}

      <div className="admin-actions floating-toolbar">
        <button className="primary-btn" onClick={seedWorldCup}>Carregar/atualizar tabela Copa 2026</button>
        <button onClick={recalculate}>Recalcular pontuação</button>
        <button onClick={load}>Atualizar tela</button>
      </div>

      <section className="grid-2">
        <div className="panel premium-panel">
          <div className="panel-title">Criar participante</div>
          <form className="form compact" onSubmit={createUser}>
            <label>
              Nome
              <input value={newUser.name} onChange={(e) => setNewUser({ ...newUser, name: e.target.value })} required placeholder="Ex.: Tio Carlos" />
            </label>
            <label>
              E-mail <small className="optional">opcional</small>
              <input type="email" value={newUser.email} onChange={(e) => setNewUser({ ...newUser, email: e.target.value })} placeholder="Pode deixar em branco" />
            </label>
            <label>
              Senha
              <input type="password" value={newUser.password} onChange={(e) => setNewUser({ ...newUser, password: e.target.value })} required minLength={6} />
            </label>
            <label>
              Perfil
              <select value={newUser.role} onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}>
                <option value="participant">Participante</option>
                <option value="admin">Administrador</option>
              </select>
            </label>
            <button className="primary-btn">Criar participante</button>
          </form>
        </div>

        <div className="panel premium-panel">
          <div className="panel-title">Participantes</div>
          {users.map(user => (
            <div className="user-line rich-row" key={user.id}>
              <div>
                <strong>{user.name}</strong>
                <small>{user.email || 'Sem e-mail cadastrado'}</small>
              </div>
              <span className="badge">{roleLabel(user.role)}</span>
            </div>
          ))}
        </div>
      </section>

      <section className="panel premium-panel">
        <div className="panel-title">Atualizar resultados</div>
        <div className="admin-matches">
          {matches.map(match => {
            const values = resultInputs[match.id] || {}
            return (
              <div className="admin-match-row rich-row" key={match.id}>
                <div>
                  <strong>Jogo {match.matchNumber}</strong>
                  <div className="mini-scoreboard admin-scoreboard">
                    <TeamBadge name={match.homeName} code={match.homeCode} compact />
                    <span>x</span>
                    <TeamBadge name={match.awayName} code={match.awayCode} compact />
                  </div>
                  <small>{stageLabel(match.stage)} {match.groupCode ? `• Grupo ${match.groupCode}` : ''} • {formatDateTime(match.kickoffAtUtc)}</small>
                </div>
                <div className="result-inputs">
                  <input type="number" min="0" value={values.homeScore ?? ''} onChange={(e) => updateResultInput(match.id, 'homeScore', e.target.value)} />
                  <span>x</span>
                  <input type="number" min="0" value={values.awayScore ?? ''} onChange={(e) => updateResultInput(match.id, 'awayScore', e.target.value)} />
                  <button onClick={() => saveResult(match)}>Salvar</button>
                </div>
              </div>
            )
          })}
        </div>
      </section>
    </div>
  )
}
