import DataTypeEnum from './DataTypeEnum'
export default class Entity {

  constructor (options) {
    this.store = options.store
    this.moment = require('moment')
    this.dataType = new DataTypeEnum()
  }

  get title () {
    var entity = this.store.state.entity.self
    if (entity && entity.title) {
      return entity.title
    }
    return ''
  }

  get classes () {
    var entity = this.store.state.entity.self
    if (entity) {
      return entity.class
    }
  }

  get path () {
    return this.store.state.entity.path
  }

  get properties () {
    var entity = this.store.state.entity.self
    if (entity) {
      var formattedEntity = this._formatProperties(entity)
      return formattedEntity
    }
  }

  get links () {
    var entity = this.store.state.entity.self
    if (entity && entity.links) {
      return entity.links
    }
  }

  get headers () {
    var entity = this.store.state.entity.self
    if (entity && entity.hasSubEntityByClass('header')) {
      var headers = entity.getSubEntitiesByClass('header')
      return headers
    }
  }

  get selfLink () {
    var entity = this.store.state.entity.self
    if (entity && entity.hasLinkByRel('self')) {
      var link = entity.getLinkByRel('self')
      return link
    }
  }

  get actions () {
    var entity = this.store.state.entity.self
    if (entity) {
      return entity.actions
    }
  }

  setPath (path) {
    this.store.commit('entity/setPath', path)
  }

  setEntity (entity) {
    this.store.commit('entity/setEntity', entity)
  }

  getEntitiesByRel (rel) {
    var entity = this.store.state.entity.self
    if (entity && entity.hasSubEntityByRel(rel)) {
      var entities = entity.getSubEntitiesByRel(rel)
      return entities
    }
  }

  getPropertiesByRel (rel) {
    var entities = this.getEntitiesByRel(rel)
    if (entities && entities.length > 0) {
      var properties = []
      entities.forEach(entity => {
        var formattedEntity = this._formatProperties(entity)
        properties.push(formattedEntity)
      })
      return properties
    }
  }

  hasLinks () {
    if (this.links && this.links.length > 0) {
      return true
    }
    return false
  }

  hasActions () {
    if (this.actions && this.actions.length > 0) {
      return true
    }
    return false
  }

  hasLinkByRel (rel) {
    var entity = this.store.state.entity.self
    if (entity) {
      return entity.hasLinkByRel(rel)
    }
    return false
  }

  getLinkByRel (rel) {
    if (this.hasLinkByRel(rel)) {
      var entity = this.store.state.entity.self
      return entity.getLinkByRel(rel)
    }
  }

  getHeader (headerName) {
    if (this.headers) {
      var header = this.headers.find(header => header.properties.name === headerName)
      return header
    }
  }

  getHeaderLinks (headerName) {
    var rowHeader = this.getHeader(headerName)
    if (rowHeader && rowHeader.links && rowHeader.links.length > 0) {
      var links = rowHeader.links.filter(link => !link.rel.includes('primaryLink'))
      return links
    }
  }

  getPrimaryLink (headerName) {
    if (this.hasPrimaryLink(headerName)) {
      var header = this.getHeader(headerName)
      var primaryLink = header.getLinkByRel('primaryLink')
      return primaryLink
    }
  }

  hasPrimaryLink (headerName) {
    var header = this.getHeader(headerName)
    if (header) {
      var hasPrimaryLink = header.hasLinkByRel('primaryLink')
      return hasPrimaryLink
    }
    return false
  }

  hasHeaderLinks (headerName) {
    var headerLink = this.getHeaderLinks(headerName)
    if (headerLink && headerLink.length > 0) {
      return true
    }
    return false
  }

  _formatProperties (entity) {
    if (!entity) {
      return
    }
    var formattedEntity = {}
    for (var property in entity.properties) {
      var value = entity.properties[property]
      var formattedValue = this._getFormattedProperty(property, value)
      formattedEntity[property] = formattedValue
    }
    return formattedEntity
  }

  _getFormattedProperty (property, value) {
    var header = this.getHeader(property)
    if (header && header.properties) {
      switch (header.properties.dataType) {
        case this.dataType.DATETIME:
          return this.moment(value).format('DD/MM/YYYY')
      }
    }
    return value
  }

}
