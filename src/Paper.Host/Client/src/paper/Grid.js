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
    if (entity && entity.hasSubEntityByRel('item')) {
      return entity.getSubEntitiesByRel('item')
    }
    if (entity && entity.hasSubEntityByRel('row')) {
      return entity.getSubEntitiesByRel('row')
    }
    return []
  }

  get headers () {
    var headers = []
    var entity = this.store.getters.entity
    if (entity && entity.hasSubEntityByRel('rowHeader')) {
      headers = this._getHeadersByRowsHeaders(entity)
      return headers
    }

    headers = this._getHeadersByItemsKeys(entity)
    return headers
  }

  _getHeadersByItemsKeys () {
    var validKeys = []
    this.validItems.forEach(item => {
      var keys = Object.keys(item.properties)
      keys.filter(key => {
        if (!key.startsWith('_') && !validKeys.includes(key)) {
          validKeys.push(key)
        }
      })
    })
    var headers = []
    validKeys.forEach(key => {
      headers.push({
        text: key,
        align: 'left',
        sortable: false,
        value: key
      })
    })
    return headers
  }

  _getHeadersByRowsHeaders (entity) {
    var headers = []
    var headersEntities = entity.getSubEntitiesByRel('rowHeader')
    if (headersEntities && headersEntities.length > 0) {
      headersEntities.forEach((header) => {
        var properties = header.properties
        if (!properties.hidden) {
          var title = properties && properties.title && properties.title.length > 0
                      ? properties.title
                      : properties.name
          headers.push({
            text: title,
            align: 'left',
            sortable: false,
            value: properties.name
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

}
