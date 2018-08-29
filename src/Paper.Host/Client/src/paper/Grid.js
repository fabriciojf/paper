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
      headers = this._getHeadersByRowsHeaders()
      return headers
    }

    headers = this._getHeadersByItemsKeys()
    return headers
  }

  getHeaderLinks (headerName) {
    var rowHeader = this._getRowHeader(headerName)
    if (rowHeader) {
      var links = rowHeader.links.filter(link => !link.rel.includes('primaryLink'))
      return links
    }
  }

  getPrimaryLink (headerName) {
    var rowHeader = this._getRowHeader(headerName)
    if (rowHeader) {
      var primaryLink = rowHeader.getLinkByRel('primaryLink')
      return primaryLink
    }
  }

  getPropertiesLink (headerName) {
    var rowHeader = this._getRowHeader(headerName)
    if (rowHeader) {
      return rowHeader.properties
    }
  }

  hasPrimaryLink (headerName) {
    var primaryLink = this.getPrimaryLink(headerName)
    return primaryLink !== null && primaryLink !== undefined
  }

  hasHeaderLinks (headerName) {
    var headerLink = this.getHeaderLinks(headerName)
    return headerLink && headerLink.length > 0
  }

  _getRowHeader (headerName) {
    var rowsHeaders = this.store.getters.entity.getSubEntitiesByRel('rowHeader')
    if (rowsHeaders && rowsHeaders.length > 0) {
      var rowHeader = rowsHeaders.find(rowHeader => rowHeader.properties.name === headerName)
      return rowHeader
    }
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

  _getHeadersByRowsHeaders () {
    var headers = []
    var entity = this.store.getters.entity
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
            value: properties.name,
            order: properties.order
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
