export const TEAMS = {
  MEX: { name: 'México', iso: 'mx' },
  RSA: { name: 'África do Sul', iso: 'za' },
  KOR: { name: 'Coreia do Sul', iso: 'kr' },
  CZE: { name: 'Tchéquia', iso: 'cz' },
  CAN: { name: 'Canadá', iso: 'ca' },
  BIH: { name: 'Bósnia e Herzegovina', iso: 'ba' },
  QAT: { name: 'Catar', iso: 'qa' },
  SUI: { name: 'Suíça', iso: 'ch' },
  BRA: { name: 'Brasil', iso: 'br' },
  MAR: { name: 'Marrocos', iso: 'ma' },
  HTI: { name: 'Haiti', iso: 'ht' },
  SCO: { name: 'Escócia', iso: 'gb-sct' },
  USA: { name: 'Estados Unidos', iso: 'us' },
  PAR: { name: 'Paraguai', iso: 'py' },
  AUS: { name: 'Austrália', iso: 'au' },
  TUR: { name: 'Turquia', iso: 'tr' },
  GER: { name: 'Alemanha', iso: 'de' },
  CUW: { name: 'Curaçao', iso: 'cw' },
  CIV: { name: 'Costa do Marfim', iso: 'ci' },
  ECU: { name: 'Equador', iso: 'ec' },
  NED: { name: 'Países Baixos', iso: 'nl' },
  JPN: { name: 'Japão', iso: 'jp' },
  SWE: { name: 'Suécia', iso: 'se' },
  TUN: { name: 'Tunísia', iso: 'tn' },
  BEL: { name: 'Bélgica', iso: 'be' },
  EGY: { name: 'Egito', iso: 'eg' },
  IRI: { name: 'Irã', iso: 'ir' },
  NZL: { name: 'Nova Zelândia', iso: 'nz' },
  ESP: { name: 'Espanha', iso: 'es' },
  CPV: { name: 'Cabo Verde', iso: 'cv' },
  KSA: { name: 'Arábia Saudita', iso: 'sa' },
  URU: { name: 'Uruguai', iso: 'uy' },
  FRA: { name: 'França', iso: 'fr' },
  SEN: { name: 'Senegal', iso: 'sn' },
  IRQ: { name: 'Iraque', iso: 'iq' },
  NOR: { name: 'Noruega', iso: 'no' },
  ARG: { name: 'Argentina', iso: 'ar' },
  DZA: { name: 'Argélia', iso: 'dz' },
  AUT: { name: 'Áustria', iso: 'at' },
  JOR: { name: 'Jordânia', iso: 'jo' },
  POR: { name: 'Portugal', iso: 'pt' },
  COD: { name: 'República Democrática do Congo', iso: 'cd' },
  UZB: { name: 'Uzbequistão', iso: 'uz' },
  COL: { name: 'Colômbia', iso: 'co' },
  ENG: { name: 'Inglaterra', iso: 'gb-eng' },
  CRO: { name: 'Croácia', iso: 'hr' },
  GHA: { name: 'Gana', iso: 'gh' },
  PAN: { name: 'Panamá', iso: 'pa' }
}

export function getTeamInfo(code) {
  if (!code) return null
  return TEAMS[String(code).toUpperCase()] || null
}

export function getTeamName(code, fallback = 'A definir') {
  return getTeamInfo(code)?.name || fallback || 'A definir'
}

export function getFlagUrl(code) {
  const iso = getTeamInfo(code)?.iso
  if (!iso) return null
  return `https://flagcdn.com/${iso}.svg`
}

export function getFlagAlt(code, fallback = '') {
  return `Bandeira ${getTeamName(code, fallback)}`
}
