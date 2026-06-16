import { getFlagAlt, getFlagUrl, getTeamName } from '../utils/teams.js'

function initialsFrom(code, name) {
  if (code) return String(code).slice(0, 3).toUpperCase()
  return String(name || 'A definir')
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map(part => part[0])
    .join('')
    .toUpperCase()
}

export default function TeamBadge({ name, code, align = 'left', compact = false }) {
  const displayName = getTeamName(code, name)
  const flagUrl = getFlagUrl(code)

  return (
    <span className={`team-badge ${align} ${compact ? 'compact' : ''}`}>
      <span className="team-flag" aria-hidden="true">
        {flagUrl
          ? <img src={flagUrl} alt={getFlagAlt(code, name)} loading="lazy" />
          : <span>{initialsFrom(code, name)}</span>}
      </span>
      <span className="team-text">
        <strong>{displayName}</strong>
        {code && <small>{code}</small>}
      </span>
    </span>
  )
}
