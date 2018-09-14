export default class Cards {

  constructor (entity) {
    this.entity = entity
  }

  get items () {
    var entities = this.entity.getEntitiesByRel('card')
    return entities
  }

  get selfLink () {
    var items = this.items
    if (items) {
      var selfLink = items.getLinkByRel('primaryLink')
      return selfLink
    }
  }

}
