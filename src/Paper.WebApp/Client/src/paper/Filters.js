export default class Filters {

  constructor (options, actions) {
    this.store = options.store
    this.actions = actions
  }

  get entity () {
    var filters = this.actions.getAction('filters')
    return filters
  }

  hasFilters () {
    return this.entity !== undefined && this.entity !== null
  }

  openMenu () {
    this.store.commit('filters/openMenu')
  }

  closeMenu () {
    this.store.commit('filters/closeMenu')
  }

  changeMenuState () {
    this.store.commit('filters/changeMenuState')
  }

}
