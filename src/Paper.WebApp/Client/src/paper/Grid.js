export default class Grid {

  constructor (options) {
    this.store = options.store
  }

  get selectedItems () {
    return this.store.getters['selection/items']
  }

  get items () {
    var items = []
    this.validItems.forEach((item, index) => {
      var itensWithIndex = Object.assign(
        { _indexRowItemTable: index }, item.properties
      )
      items.push(itensWithIndex)
    })
    return items
  }

  get validItems () {
    var entity = this.store.getters.entity
    if (entity && entity.hasSubEntityByClass('item')) {
      return entity.getSubEntitiesByClass('item')
    }
    if (entity && entity.hasSubEntityByClass('row')) {
      return entity.getSubEntitiesByClass('row')
    }
    return []
  }

  get headers () {
    var headers = []
    var keys = this._getDiffHeaders()
    if (keys && keys.length > 0) {
      keys.forEach((key) => {
        if (!key.startsWith('_')) {
          var header = this._getHeaderProperties(key)
          var title = header && header.title && header.title.length > 0 ? header.title : key
          headers.push({
            text: title,
            align: 'left',
            sortable: false,
            value: key
          })
        }
      })
    }
    return headers
  }

  hasActions () {
    var exist = this.validItems.filter(entity => entity.hasAction())
    return exist && exist.length > 0
  }

  setSelectedItems (items) {
    this.store.commit('selection/setSelectedItems', items)
  }

  _getHeaderProperties (headerKey) {
    var entity = this.store.getters.entity
    if (entity && entity.properties) {
      var headers = entity.properties['_rowsHeaders'] || entity.properties['rowsHeaders']
      if (headers) {
        var header = headers.find((header) => header.name === headerKey)
        return header
      }
    }
  }

  _getDiffHeaders () {
    var entity = this.store.getters.entity
    var flags = []
    if (entity && entity.entities && entity.entities.length > 0) {
      entity.entities.forEach(entity => {
        if (entity.properties) {
          var keys = Object.keys(entity.properties)
          keys.forEach(key => {
            if (!flags.includes(key)) {
              flags.push(key)
            }
          })
        }
      })
    }
    return flags
  }

}
