import DataTypeEnum from './DataTypeEnum'
export default class Data {

  constructor (options, entity) {
    this.store = options.store
    this.dataType = new DataTypeEnum()
    this.entity = entity
  }

  get items () {
    if (!this.entity || !this.entity.properties) {
      return
    }
    var items = []
    var keys = this.entity.properties.__dataHeaders
    if (keys) {
      keys.forEach(key => {
        var header = this.entity.getHeader(key)
        if (header) {
          var hidden = header.properties.hasOwnProperty('hidden')
                     ? header.properties.hidden === 1
                     : false
          items.push({
            title: header.properties.title,
            value: this.entity.properties[key],
            name: key,
            dataType: header.properties.dataType,
            hidden: hidden
          })
        }
      })
    }
    return items
  }

  getLinks (itemName) {
    if (this.entity.hasLinks()) {
      var link = this.entity.links.filter(link => link.rel.includes(itemName))
      return link
    }
  }

  hasActions () {
    return this.entity.hasActions()
  }

}
