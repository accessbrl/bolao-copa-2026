export function getApiErrorMessage(error) {
  if (!error.response) {
    return 'Não foi possível conectar com a API. Aguarde alguns segundos e tente novamente.'
  }

  const data = error.response.data

  if (typeof data === 'string' && data.trim()) {
    return data
  }

  if (data?.message) {
    return data.message
  }

  if (data?.title) {
    return data.title
  }

  if (data?.errors) {
    const messages = Object.values(data.errors).flat().filter(Boolean)
    if (messages.length > 0) {
      return messages.join(' ')
    }
  }

  if (error.response.status === 400) {
    return 'Dados inválidos. Confira as informações e tente novamente.'
  }

  if (error.response.status === 401) {
    return 'Nome/e-mail ou senha inválidos.'
  }

  if (error.response.status >= 500) {
    return 'Erro na API. Aguarde alguns instantes e tente novamente.'
  }

  return 'Não foi possível continuar. Verifique os dados.'
}
