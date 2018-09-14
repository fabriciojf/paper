import DataTypeEnum from './DataTypeEnum'
export default class Grid {

  constructor (options, entity) {
    this.store = options.store
    this.moment = require('moment')
    this.dataType = new DataTypeEnum()
    this.entity = entity
  }

  get selectedItems () {
    return this.store.getters['selection/items']
  }

  get items () {
    var items = []
    var entities = this.entity.getPropertiesByRel('row')
    entities.forEach((item, index) => {
      var formattedProperties = Object.assign(
        {'_indexRowItemTable': index}, item
      )
      items.push(formattedProperties)
    })
    return items
  }

  get headers () {
    if (this.entity.headers) {
      var headers = this._getHeadersByRowsHeaders()
      return headers
    }

    headers = this._getHeadersByItemsKeys()
    return headers
  }

  getColumnType (item) {
    var dataHeader = this.entity.getHeader(item)
    if (dataHeader.properties) {
      var columnType = dataHeader.properties.type
      return columnType
    }
  }

  getColumnDataType (item) {
    var dataHeader = this.entity.getHeader(item)
    if (dataHeader.properties) {
      var columnDataType = dataHeader.properties.dataType
      return columnDataType
    }
  }

  getHeaderLinks (headerName) {
    var headerLinks = this.entity.getHeaderLinks(headerName)
    return headerLinks
  }

  getPrimaryLink (headerName) {
    return this.entity.getPrimaryLink(headerName)
  }

  getRowLinks (index) {
    var entity = this.getItemByRow(index)
    if (entity && entity.links && entity.links.length > 0) {
      var links = entity.links.filter(link => !link.rel.includes('self'))
      return links
    }
  }

  getItemByRow (index) {
    var entity = this.entity.getEntitiesByRel('row')[index]
    return entity
  }

  getLinkByCel (indexRow, column) {
    var item = this.getItemByRow(indexRow)
    if (item) {
      var link = item.getLinkByRel(column)
      return link
    }
  }

  hasLinkByCel (indexRow, column) {
    var link = this.getLinkByCel(indexRow, column)
    if (link && link.href && link.href.length > 0) {
      return true
    }
    return false
  }

  hasRowLinks (index) {
    var links = this.getRowLinks(index)
    if (links && links.length > 0) {
      return true
    }
    return false
  }

  getSelfLinkByRow (index) {
    var item = this.getItemByRow(index)
    var link = item.getLinkByRel('self')
    if (link && link.href && link.href.length > 0) {
      return link
    }
  }

  getLinksByRow (index) {
    var item = this.getItemByRow(index)
    var links = item.links.filter(link => !link.rel.includes('self'))
    if (links && links.length > 0) {
      return links
    }
  }

  hasLinksByRow (index) {
    var links = this.getLinksByRow(index)
    if (links) {
      return true
    }
    return false
  }

  hasSelfLinkByRow (index) {
    var link = this.getSelfLinkByRow(index)
    if (link) {
      return true
    }
    return false
  }

  shortenText (text) {
    if (text && text.length > this.textColumnMaxLength) {
      text = text.substr(0, this.textColumnMaxLength - 3) + '...'
    }
    return text
  }

  hasPrimaryLink (headerName) {
    return this.entity.hasPrimaryLink(headerName)
  }

  hasHeaderLinks (headerName) {
    return this.entity.hasHeaderLinks(headerName)
  }

  hasActions () {
    return false
  }

  setSelectedItems (items) {
    this.store.commit('selection/setSelectedItems', items)
  }

  _getHeadersByItemsKeys () {
    var headers = []
    var items = this.items
    if (!items) {
      return
    }
    items.forEach(item => {
      if (!item.properties) {
        return
      }
      var keys = item.__dataHeaders
      keys.forEach(key => {
        headers.push({
          text: key,
          align: 'left',
          sortable: false,
          value: key
        })
      })
    })
    return headers
  }

  _getHeadersByRowsHeaders () {
    var headers = []
    var headersEntities = this.entity.headers
    if (headersEntities && headersEntities.length > 0) {
      headersEntities.forEach(header => {
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

}
