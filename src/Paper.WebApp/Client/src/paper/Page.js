export default class Page {

  constructor (options, requester, parser, demo) {
    this.store = options.store
    this.router = options.router
    this.requester = requester
    this.demo = demo
    this.parser = parser
  }

  get latest () {
    return 'teste'
  }

  get title () {
    return (this.store.state.entity && this.store.state.entity.title) ? this.store.state.entity.title : ''
  }

  isRoot () {
    var isRoot = window.location.hash.toLowerCase() === '#/index.html' || window.location.hash === '#/'
    return isRoot
  }

  load () {
    var pathEntity = this.store.state.pathEntity
    if (this.demo.isDemoPage(pathEntity)) {
      this.demo.load(pathEntity)
      return
    }
    this.requester.httpRequest('get', pathEntity, {}).then(response => {
      if (response.ok) {
        var json = response.data.data
        if (json) {
          var data = this.parser.parse(json)
          this.store.commit('setEntity', data)
        }
      } else {
        if (response.data.status === 404) {
          this.router.replace({name: 'notFound', params: { routerName: pathEntity }})
        } else {
          this.router.replace({name: 'error', params: { error: response.data }})
        }
      }
    })
  }

  parse (path) {
    return this.requester.httpRequest('get', path, {}).then(response => {
      if (response.ok) {
        if (!response.data) {
          return
        }
        var json = response.data.data
        if (json) {
          return {
            ok: true,
            data: this.parser.parse(json)
          }
        }
      }
      return {
        ok: false,
        data: response.data
      }
    })
  }

  unload () {
    this.store.commit('setEntity', null)
    this.store.commit('setEntityPath', null)
  }

  save (path, data) {
    this.requester.httpRequest('POST', path, data).then(response => {
      if (response.ok) {
        this.requester.redirectToPage(path)
      }
    })
  }

  hasTitle () {
    return this.store.state.entity && this.store.state.entity.title
  }

}
