import { useEffect, useState } from 'react'
import api from '../services/api.js'

export default function Ranking() {
  const [ranking, setRanking] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    api.get('/ranking')
      .then(response => setRanking(response.data))
      .finally(() => setLoading(false))
  }, [])

  return (
    <div className="page">
      <header className="page-header">
        <span className="eyebrow">Classificação</span>
        <h1>Ranking</h1>
        <p>Critérios: pontos, placares exatos, acertos de resultado e nome.</p>
      </header>

      <div className="panel premium-panel">
        {loading && <p>Carregando ranking...</p>}
        {!loading && (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Posição</th>
                  <th>Nome</th>
                  <th>Pontos</th>
                  <th>Exatos</th>
                  <th>Acertos</th>
                  <th>Palpites</th>
                </tr>
              </thead>
              <tbody>
                {ranking.map(item => (
                  <tr key={item.userId}>
                    <td><span className="ranking-position">#{item.position}</span></td>
                    <td><strong>{item.name}</strong></td>
                    <td><strong>{item.totalPoints}</strong></td>
                    <td>{item.exactScores}</td>
                    <td>{item.outcomeHits}</td>
                    <td>{item.predictionsCount}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
