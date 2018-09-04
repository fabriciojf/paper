export default class Cards {

  constructor (options) {
    this.store = options.store
  }

  get items () {
    var entity = this.store.getters.entity
    if (entity && entity.hasSubEntityByRel('card')) {
      return entity.getSubEntitiesByRel('card')
    }
  }

  get selfLink () {
    var items = this.items
    if (items) {
      var selfLink = items.getLinkByRel('primaryLink')
      return selfLink
    }
  }

}
