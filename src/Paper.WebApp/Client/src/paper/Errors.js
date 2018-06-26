export default class Errors {

  httpTranslate (errorcode) {
    switch (errorcode) {
      case 400:
        return 'Não é possível localizar a página da Web'
      case 401:
        return 'Acesso negado'
      case 403:
        return 'Acesso proibido.'
      case 404:
        return 'Página não encontrada.'
      case 405:
        return 'O verbo HTTP usado para acessar esta página não é permitido'
      case 406:
        return 'O navegador do cliente não aceita o tipo da página solicitada'
      case 407:
        return 'Autenticação proxy solicitada'
      case 408:
        return 'O site está ocupado demais para mostrar a página da Web'
      case 409:
        return 'O site está ocupado demais para mostrar a página da Web'
      case 410:
        return 'A página da Web não existe mais'
      case 412:
        return 'Erro de precondicionamento'
      case 413:
        return 'Entidade solicitada muito grande'
      case 414:
        return 'URI solicitada muito grande'
      case 415:
        return 'Tipo de mídia sem suporte'
      case 416:
        return 'Intervalo solicitado não satisfatório'
      case 417:
        return 'Erro na execução'
      case 423:
        return 'Erro no bloqueio'
      case 500:
        return 'Erro interno de servidor'
      case 501:
        return 'Os valores de cabeçalho especificam uma configuração que não está implementada'
      case 502:
        return 'O servidor recebeu uma resposta inválida ao atuar como gateway ou proxy'
      case 503:
        return 'Serviço indisponível'
      case 504:
        return 'Tempo limite de gateway'
      case 505:
        return 'Versão HTTP não suportada'
      default:
        return 'Erro ao carregar a página'
    }
  }

}
