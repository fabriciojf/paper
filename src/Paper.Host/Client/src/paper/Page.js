import {Enum} from 'enumify'
class PageType extends Enum {}

export default class Page {

  constructor (options, requester, parser, demo, entity) {
    this.router = options.router
    this.requester = requester
    this.demo = demo
    this.parser = parser
    this.pageType = PageType.initEnum(['GRID', 'VIEW', 'CARDS'])
    this.entity = entity
  }

  get title () {
    return this.entity.title
  }

  get type () {
    var classes = this.entity.classes
    if (!classes) {
      return
    }
    if (classes.includes('cards')) {
      return PageType.CARDS
    } else if (classes.includes('rows')) {
      return PageType.GRID
    }
    return PageType.VIEW
  }

  setPagePath (path) {
    if (this.entity) {
      this.entity.setPath(path)
    }
  }

  hasTitle () {
    return this.title && this.title.length > 0
  }

  isRoot () {
    var isRoot = window.location.hash.toLowerCase() === '#/index.html' || window.location.hash === '#/'
    return isRoot
  }

  load () {
    var entityPath = this.entity.path
    if (this.demo.isDemoPage(entityPath)) {
      this.demo.load(entityPath)
      return
    }
    this.requester.httpRequest('get', entityPath, {}).then(response => {
      if (response.ok) {
        var json = response.data.data
        if (json) {
          var data = this.parser.parse(json)
          this.entity.setEntity(data)
        }
      } else {
        if (response.data.status === 404) {
          this.router.replace({name: 'notFound', params: { routerName: entityPath }})
        } else {
          this.router.replace({name: 'error', params: { error: response.data }})
        }
      }
    })
  }

  loadAction (action, queryParams) {
    return this.requester.httpRequest(action.method, action.href, queryParams).then(response => {
      if (response.ok) {
        var json = response.data.data
        if (json) {
          var data = this.parser.parse(json)
          this.entity.setEntity(data)
        }
        return {
          ok: true,
          data: response
        }
      }
      return {
        ok: false,
        data: response
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
    this.entity.setPath(null)
    this.entity.setEntity(null)
  }

  save (path, data) {
    this.requester.httpRequest('POST', path, data).then(response => {
      if (response.ok) {
        this.requester.redirectToPage(path)
      }
    })
  }

}
