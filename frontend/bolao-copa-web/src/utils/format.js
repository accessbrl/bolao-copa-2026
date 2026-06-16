const FIFA_TO_ISO = {
  MEX: 'MX', RSA: 'ZA', KOR: 'KR', CZE: 'CZ', CAN: 'CA', BIH: 'BA', QAT: 'QA', SUI: 'CH',
  BRA: 'BR', MAR: 'MA', HTI: 'HT', USA: 'US', PAR: 'PY', AUS: 'AU', TUR: 'TR', GER: 'DE',
  CUW: 'CW', CIV: 'CI', ECU: 'EC', NED: 'NL', JPN: 'JP', SWE: 'SE', TUN: 'TN', BEL: 'BE',
  EGY: 'EG', IRI: 'IR', NZL: 'NZ', ESP: 'ES', CPV: 'CV', KSA: 'SA', URU: 'UY', FRA: 'FR',
  SEN: 'SN', IRQ: 'IQ', NOR: 'NO', ARG: 'AR', DZA: 'DZ', AUT: 'AT', JOR: 'JO', POR: 'PT',
  COD: 'CD', UZB: 'UZ', COL: 'CO', CRO: 'HR', GHA: 'GH', PAN: 'PA'
}

const SPECIAL_FLAGS = {
  SCO: '🏴',
  ENG: '🏴'
}

export function formatDateTime(value) {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR', {
    weekday: 'short',
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

export function stageLabel(stage) {
  const map = {
    GROUP: 'Fase de grupos',
    ROUND_OF_32: 'Fase de 32',
    ROUND_OF_16: 'Oitavas de final',
    QUARTER_FINAL: 'Quartas de final',
    SEMI_FINAL: 'Semifinal',
    THIRD_PLACE: 'Disputa de 3º lugar',
    FINAL: 'Final'
  }
  return map[stage] || stage
}

export function roleLabel(role) {
  return role === 'admin' ? 'Administrador' : 'Participante'
}

export function flagEmoji(fifaCode) {
  if (!fifaCode) return '🌐'
  if (SPECIAL_FLAGS[fifaCode]) return SPECIAL_FLAGS[fifaCode]

  const iso = FIFA_TO_ISO[fifaCode] || fifaCode.slice(0, 2)
  if (!iso || iso.length !== 2) return '🌐'

  return iso
    .toUpperCase()
    .split('')
    .map(char => String.fromCodePoint(127397 + char.charCodeAt(0)))
    .join('')
}

export function matchStatusLabel(match) {
  if (match.status === 'finished') return 'Finalizado'
  return match.isLocked ? 'Bloqueado' : 'Aberto'
}
