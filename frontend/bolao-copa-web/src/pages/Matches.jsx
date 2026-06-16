import { useEffect, useMemo, useState } from 'react'
import TeamBadge from '../components/TeamBadge.jsx'
import api from '../services/api.js'
import { formatDateTime, matchStatusLabel, stageLabel } from '../utils/format.js'

export default function Matches() {
  const [matches, setMatches] = useState([])
  const [predictions, setPredictions] = useState([])
  const [inputs, setInputs] = useState({})
  const [filter, setFilter] = useState('ALL')
  const [message, setMessage] = useState('')
  const [loading, setLoading] = useState(true)

  async function load() {
    setLoading(true)
    const [matchesResponse, predictionsResponse] = await Promise.all([
      api.get('/matches'),
      api.get('/predictions/me')
    ])
    setMatches(matchesResponse.data)
    setPredictions(predictionsResponse.data)

    const nextInputs = {}
    for (const prediction of predictionsResponse.data) {
      nextInputs[prediction.matchId] = {
        home: prediction.predictedHomeScore,
        away: prediction.predictedAwayScore
      }
    }
    setInputs(nextInputs)
    setLoading(false)
  }

  useEffect(() => {
    load().catch(() => setLoading(false))
  }, [])

  const predictionMap = useMemo(() => {
    return Object.fromEntries(predictions.map(x => [x.matchId, x]))
  }, [predictions])

  const filteredMatches = useMemo(() => {
    if (filter === 'ALL') return matches
    if (filter === 'PENDING') return matches.filter(x => !predictionMap[x.id] && !x.isLocked)
    if (filter === 'LOCKED') return matches.filter(x => x.isLocked)
    if (filter === 'SAVED') return matches.filter(x => predictionMap[x.id])
    return matches.filter(x => x.groupCode === filter || x.stage === filter)
  }, [matches, filter, predictionMap])

  function updateInput(matchId, field, value) {
    setInputs(current => ({
      ...current,
      [matchId]: {
        ...current[matchId],
        [field]: value
      }
    }))
  }

  async function savePrediction(match) {
    setMessage('')
    const values = inputs[match.id] || {}
    const home = Number(values.home)
    const away = Number(values.away)

    if (Number.isNaN(home) || Number.isNaN(away)) {
      setMessage('Preencha os dois placares antes de salvar.')
      return
    }

    try {
      await api.post('/predictions', {
        matchId: match.id,
        predictedHomeScore: home,
        predictedAwayScore: away,
        predictedWinnerTeamId: null
      })
      setMessage('Palpite salvo com sucesso.')
      await load()
    } catch (err) {
      setMessage(err.response?.data?.message || 'Erro ao salvar palpite.')
    }
  }

  return (
    <div className="page">
      <header className="page-header page-header-row">
        <div>
          <span className="eyebrow">Copa 2026</span>
          <h1>Jogos e Palpites</h1>
          <p>Faça seus palpites antes do bloqueio de cada partida.</p>
        </div>
        <div className="header-chip">
          <strong>{filteredMatches.length}</strong>
          <span>jogos na tela</span>
        </div>
      </header>

      <div className="toolbar floating-toolbar">
        <select value={filter} onChange={(e) => setFilter(e.target.value)} aria-label="Filtro de jogos">
          <option value="ALL">Todos os jogos</option>
          <option value="PENDING">Pendentes de palpite</option>
          <option value="SAVED">Com palpite salvo</option>
          <option value="LOCKED">Bloqueados</option>
          <option value="A">Grupo A</option>
          <option value="B">Grupo B</option>
          <option value="C">Grupo C</option>
          <option value="D">Grupo D</option>
          <option value="E">Grupo E</option>
          <option value="F">Grupo F</option>
          <option value="G">Grupo G</option>
          <option value="H">Grupo H</option>
          <option value="I">Grupo I</option>
          <option value="J">Grupo J</option>
          <option value="K">Grupo K</option>
          <option value="L">Grupo L</option>
          <option value="ROUND_OF_32">Fase de 32</option>
          <option value="ROUND_OF_16">Oitavas de final</option>
          <option value="QUARTER_FINAL">Quartas de final</option>
          <option value="SEMI_FINAL">Semifinais</option>
          <option value="FINAL">Final</option>
        </select>
        <button onClick={load}>Atualizar</button>
      </div>

      {message && <div className="alert info">{message}</div>}
      {loading && <p>Carregando jogos...</p>}

      <div className="matches-list">
        {filteredMatches.map(match => {
          const prediction = predictionMap[match.id]
          const values = inputs[match.id] || {}
          const canEdit = !match.isLocked

          return (
            <article className="match-card modern-match-card" key={match.id}>
              <div className="match-meta">
                <span>Jogo {match.matchNumber}</span>
                <span>{stageLabel(match.stage)} {match.groupCode ? `• Grupo ${match.groupCode}` : ''}</span>
                <span>{formatDateTime(match.kickoffAtUtc)}</span>
              </div>

              <div className="scoreboard">
                <TeamBadge name={match.homeName} code={match.homeCode} />
                <div className="versus-pill">x</div>
                <TeamBadge name={match.awayName} code={match.awayCode} align="right" />
              </div>

              {match.status === 'finished' && (
                <div className="result-line">
                  Resultado oficial: <strong>{match.homeScore} x {match.awayScore}</strong>
                </div>
              )}

              <div className="prediction-box premium-prediction-box">
                <label>
                  <span>{match.homeCode || 'Casa'}</span>
                  <input
                    type="number"
                    min="0"
                    disabled={!canEdit}
                    value={values.home ?? ''}
                    onChange={(e) => updateInput(match.id, 'home', e.target.value)}
                  />
                </label>
                <span className="score-separator">x</span>
                <label>
                  <span>{match.awayCode || 'Fora'}</span>
                  <input
                    type="number"
                    min="0"
                    disabled={!canEdit}
                    value={values.away ?? ''}
                    onChange={(e) => updateInput(match.id, 'away', e.target.value)}
                  />
                </label>

                <button disabled={!canEdit} onClick={() => savePrediction(match)}>
                  {prediction ? 'Atualizar palpite' : 'Salvar palpite'}
                </button>
              </div>

              <div className="match-footer">
                <span className={match.isLocked ? 'badge danger' : 'badge success'}>
                  {matchStatusLabel(match)}
                </span>
                {prediction
                  ? <span className="badge highlight">Seu palpite: {prediction.predictedHomeScore} x {prediction.predictedAwayScore} • {prediction.points} pts</span>
                  : <span className="badge">Sem palpite</span>}
              </div>
            </article>
          )
        })}
      </div>
    </div>
  )
}
