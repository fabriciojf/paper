export default class Parser {

  constructor (options) {
    this.vue = options.vm
  }

  parse (file) {
    return this.sirenParser(file)
  }

  sirenParser (file) {
    try {
      var sirenParser = require('siren-parser')
      var resource = sirenParser(file)
      return resource
    } catch (err) {
      this.vue.$notify({
        message: 'Falhou a tentativa de realizar o parse do arquivo: ' + err,
        type: 'danger'
      })
    }
  }

}
