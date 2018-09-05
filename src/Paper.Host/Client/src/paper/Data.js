export default class Data {

  constructor (options) {
    this.store = options.store
  }

  get items () {
    var data = this.store.getters.entity
    var items = []
    if (data && data.properties) {
      var keys = Object.keys(data.properties)
      keys.forEach((key) => {
        if (!key.startsWith('_')) {
          var properties = this._getProperties(key)
          if (properties) {
            var hidden = properties.hasOwnProperty('hidden') ? properties.hidden : false
            if (!hidden) {
              items.push({
                title: properties.title,
                value: data.properties[key],
                name: key,
                dataType: properties.dataType
              })
            }
          }
        }
      })
    }
    return items
  }

  getLinks (itemName) {
    var entity = this.store.getters.entity
    if (entity && entity.links && entity.links.length > 1) {
      var link = entity.links.filter(link => link.rel.includes(itemName))
      if (link) {
        return link
      }
    }
  }

  get dataHeaders () {
    var entity = this.store.getters.entity
    if (entity && entity.hasSubEntityByRel('dataHeader')) {
      return entity.getSubEntitiesByRel('dataHeader')
    }

    return []
  }

  _getProperties (name) {
    var dataHeaders = this.dataHeaders
    if (dataHeaders && dataHeaders.length > 0) {
      var dataHeader = dataHeaders.find(dataHeader => dataHeader.properties.name === name)
      if (dataHeader) {
        return dataHeader.properties
      }
    }
  }

  hasActions () {
    var exist = this.validItems.filter(entity => entity.hasAction())
    return exist && exist.length > 0
  }

}
