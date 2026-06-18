import { useEffect, useState } from 'react'
import TeamBadge from '../components/TeamBadge.jsx'
import api from '../services/api.js'
import { formatDateTime, stageLabel } from '../utils/format.js'

export default function Dashboard() {
  const [upcoming, setUpcoming] = useState([])
  const [ranking, setRanking] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    Promise.all([
      api.get('/matches/upcoming?take=6'),
      api.get('/ranking')
    ])
      .then(([matchesResponse, rankingResponse]) => {
        setUpcoming(matchesResponse.data)
        setRanking(rankingResponse.data)
      })
      .finally(() => setLoading(false))
  }, [])

  const leader = ranking[0]

  return (
    <div className="page">
      <section className="hero-panel">
        <div>
          <span className="eyebrow">Resumo geral</span>
          <h1>Bolão da Família</h1>
          <p>Acompanhe os próximos jogos, confira o ranking e dispute ponto a ponto durante a Copa do Mundo 2026.</p>
        </div>
        <div className="hero-trophy" aria-hidden="true">🏆</div>
      </section>

      <div className="stats-grid">
        <div className="stat-card glow-card">
          <span>Próximos jogos</span>
          <strong>{upcoming.length}</strong>
        </div>
        <div className="stat-card glow-card">
          <span>Participantes</span>
          <strong>{ranking.length}</strong>
        </div>
        <div className="stat-card glow-card">
          <span>Líder</span>
          <strong>{leader?.name || '-'}</strong>
        </div>
        <div className="stat-card glow-card">
          <span>Pontos do líder</span>
          <strong>{leader?.totalPoints ?? 0}</strong>
        </div>
      </div>

      <section className="grid-2">
        <div className="panel premium-panel">
          <div className="panel-title">Próximos jogos</div>
          {loading && <p>Carregando...</p>}
          {!loading && upcoming.length === 0 && <p>Nenhum jogo encontrado.</p>}
          {upcoming.map(match => (
            <div className="match-row rich-row" key={match.id}>
              <div>
                <div className="mini-scoreboard">
                  <TeamBadge name={match.homeName} code={match.homeCode} compact />
                  <span>x</span>
                  <TeamBadge name={match.awayName} code={match.awayCode} compact />
                </div>
                <small>{stageLabel(match.stage)} {match.groupCode ? `• Grupo ${match.groupCode}` : ''}</small>
              </div>
              <span>{formatDateTime(match.kickoffAtUtc)}</span>
            </div>
          ))}
        </div>

        <div className="panel premium-panel">
          <div className="panel-title">Top ranking</div>
          {ranking.slice(0, 6).map(item => (
            <div className="ranking-row rich-row" key={item.userId}>
              <span className="position">#{item.position}</span>
              <strong>{item.name}</strong>
              <span>{item.totalPoints} pts</span>
            </div>
          ))}
          {!loading && ranking.length === 0 && <p>Nenhum participante no ranking ainda.</p>}
        </div>
      </section>
    </div>
  )
}
